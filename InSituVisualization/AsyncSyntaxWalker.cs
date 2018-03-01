using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using InSituVisualization.Model;
using InSituVisualization.TelemetryMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace InSituVisualization
{
    internal class AsyncSyntaxWalker
    {
        private readonly Document _document;
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly MethodAdornmentLayer _methodAdornmentLayer;

        public AsyncSyntaxWalker(Document document, SemanticModel semanticModel, ITelemetryDataMapper telemetryDataMapper, MethodAdornmentLayer methodAdornmentLayer)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _telemetryDataMapper = telemetryDataMapper ?? throw new ArgumentNullException(nameof(telemetryDataMapper));
            _methodAdornmentLayer = methodAdornmentLayer ?? throw new ArgumentNullException(nameof(methodAdornmentLayer)); ;
        }

        public async Task VisitAsync(SyntaxTree syntaxTree, SyntaxTree originalTree)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }
            if (originalTree == null)
            {
                throw new ArgumentNullException(nameof(originalTree));
            }

            // getting Changes
            // https://github.com/dotnet/roslyn/issues/17498
            // https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn
            var treeChanges = syntaxTree.GetChanges(originalTree).Where(treeChange => !string.IsNullOrWhiteSpace(treeChange.NewText)).ToList();

            var root = await syntaxTree.GetRootAsync();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclarationSyntax in methods)
            {
                var methodPerformanceInfo = await VisitMethodDeclarationAsync(methodDeclarationSyntax);
                if (methodPerformanceInfo != null)
                {
                    methodPerformanceInfo.HasChanged = HasTextChangesInNode(treeChanges, methodDeclarationSyntax);
                }
            }
        }

        public async Task<MethodPerformanceInfo> VisitMethodDeclarationAsync(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (methodSymbol == null)
            {
                return null;
            }

            var methodPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(methodSymbol);
            if (methodPerformanceInfo == null)
            {
                return null;
            }

            // TODO RR: var syntaxReference = methodSymbol.DeclaringSyntaxReferences
            // syntaxReference.GetSyntax(context.CancellationToken);

            // Invocations in Method
            var invocationExpressionSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
            foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
            {
                var invocationPerformanceInfo = await VisitInvocationAsync(invocationExpressionSyntax);
                if (invocationPerformanceInfo != null)
                {
                    methodPerformanceInfo.AddCalleePerformanceInfo(invocationPerformanceInfo);
                }
            }

            // Loops
            // TODO RR: Clean and only Iterate once...
            var loopSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).Where(IsLoopSyntax);
            foreach (var loopSyntax in loopSyntaxs)
            {
                var loopPerformanceInfo = await VisitLoopAsync(loopSyntax, methodPerformanceInfo);
                if (loopPerformanceInfo != null)
                {
                    methodPerformanceInfo.LoopPerformanceInfo.Add(loopPerformanceInfo);
                }
            }

            // TODO RR: all References:
            //var referencesToMethod = await SymbolFinder.FindReferencesAsync(methodSymbol, _document.Project.Solution);

            _methodAdornmentLayer.DrawMethodPerformanceInfo(methodDeclarationSyntax, methodPerformanceInfo);
            return methodPerformanceInfo;
        }

        private async Task<MethodPerformanceInfo> VisitInvocationAsync(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
            if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
            {
                return null;
            }
            var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol);
            if (invocationPerformanceInfo == null)
            {
                return null;
            }
            _methodAdornmentLayer.DrawMethodInvocationPerformanceInfo(invocationExpressionSyntax, invocationPerformanceInfo);
            return invocationPerformanceInfo;
        }

        private async Task<LoopPerformanceInfo> VisitLoopAsync(SyntaxNode loopSyntax, MethodPerformanceInfo methodPerformanceInfo)
        {
            var invocationExpressionSyntaxsInLoop = loopSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
            var loopInvocationsList = new List<MethodPerformanceInfo>();
            foreach (var invocationExpressionSyntax in invocationExpressionSyntaxsInLoop)
            {
                var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
                // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
                if (invokedMethodSymbol == null ||
                    !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
                {
                    continue;
                }
                var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol);
                loopInvocationsList.Add(invocationPerformanceInfo);
            }
            var loopPerformanceInfo = new LoopPerformanceInfo(methodPerformanceInfo, loopInvocationsList);
            _methodAdornmentLayer.DrawLoopPerformanceInfo(loopSyntax, loopPerformanceInfo);
            return loopPerformanceInfo;
        }


        private static bool IsLoopSyntax(SyntaxNode node)
        {
            return node is ForStatementSyntax ||
                   node is WhileStatementSyntax ||
                   node is DoStatementSyntax ||
                   node is ForEachStatementSyntax;
        }

        private static bool HasTextChangesInNode(IEnumerable<TextChange> textChanges, SyntaxNode syntaxNode)
        {
            return textChanges.Any(textChange => syntaxNode.Span.IntersectsWith(textChange.Span));
        }
    }
}
