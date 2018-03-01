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
using Microsoft.CodeAnalysis.Text;

namespace InSituVisualization
{
    internal class AsyncSyntaxWalker
    {

        private readonly IList<TextChange> _textChanges;
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly MethodAdornmentLayer _methodAdornmentLayer;

        public AsyncSyntaxWalker(SemanticModel semanticModel, IList<TextChange> textChanges, ITelemetryDataMapper telemetryDataMapper, MethodAdornmentLayer methodAdornmentLayer)
        {
            _textChanges = textChanges;
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _telemetryDataMapper = telemetryDataMapper ?? throw new ArgumentNullException(nameof(telemetryDataMapper));
            _methodAdornmentLayer = methodAdornmentLayer ?? throw new ArgumentNullException(nameof(methodAdornmentLayer)); ;
        }

        public async Task VisitAsync(SyntaxTree syntaxTree)
        {
            var root = await syntaxTree.GetRootAsync();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclarationSyntax in methods)
            {
                await VisitMethodDeclarationAsync(methodDeclarationSyntax);
            }
        }

        public async Task VisitMethodDeclarationAsync(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (methodSymbol == null)
            {
                return;
            }

            if (HasTextChanges(methodDeclarationSyntax))
            {
                // TODO RR: Method Changed...
                _methodAdornmentLayer.DrawSpan(methodDeclarationSyntax, Colors.Red);
                return;
            }


            var methodPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(methodSymbol);
            if (methodPerformanceInfo == null)
            {
                return;
            }
            _methodAdornmentLayer.DrawMethodPerformanceInfo(methodDeclarationSyntax, methodPerformanceInfo);

            // TODO RR: var syntaxReference = methodSymbol.DeclaringSyntaxReferences
            // syntaxReference.GetSyntax(context.CancellationToken);

            // Invocations in Method
            var invocationExpressionSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
            foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
            {
                await VisitInvocationAsync(invocationExpressionSyntax, methodPerformanceInfo);
            }

            // Loops
            // TODO RR: Clean and only Iterate once...
            var loopSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).Where(IsLoopSyntax);
            foreach (var loopSyntax in loopSyntaxs)
            {
                await VisitLoopAsync(loopSyntax, methodPerformanceInfo);
            }
        }

        private async Task VisitInvocationAsync(InvocationExpressionSyntax invocationExpressionSyntax, MethodPerformanceInfo methodPerformanceInfo)
        {
            var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
            if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
            {
                return;
            }
            var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol);
            if (invocationPerformanceInfo == null)
            {
                return;
            }
            // Setting Caller and CalleeInformation
            invocationPerformanceInfo.CallerPerformanceInfo.Add(methodPerformanceInfo);
            methodPerformanceInfo.CalleePerformanceInfo.Add(invocationPerformanceInfo);

            _methodAdornmentLayer.DrawMethodInvocationPerformanceInfo(invocationExpressionSyntax, invocationPerformanceInfo);
        }

        private async Task VisitLoopAsync(SyntaxNode loopSyntax, MethodPerformanceInfo methodPerformanceInfo)
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
            _methodAdornmentLayer.DrawLoopPerformanceInfo(loopSyntax, new LoopPerformanceInfo(methodPerformanceInfo, loopInvocationsList));
        }


        private static bool IsLoopSyntax(SyntaxNode node)
        {
            return node is ForStatementSyntax ||
                   node is WhileStatementSyntax ||
                   node is DoStatementSyntax ||
                   node is ForEachStatementSyntax;
        }

        private bool HasTextChanges(SyntaxNode syntaxNode)
        {
            return _textChanges.Any(textChange => syntaxNode.Span.IntersectsWith(textChange.Span));
        }
    }
}
