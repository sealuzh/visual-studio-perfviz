using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryMapper;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace InSituVisualization.Tagging
{
    /// <summary>
    /// Class does not inherit from CSharpSyntaxWalker since it wasn't possible to walk the tree in an async way...
    /// </summary>
    internal class AsyncSyntaxWalker
    {
        private readonly ITextBuffer _buffer;
        private readonly IPredictionEngine _predictionEngine;
        private readonly Document _document;
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;


        public AsyncSyntaxWalker(
            IPredictionEngine predictionEngine,
            Document document, 
            SemanticModel semanticModel,
            ITelemetryDataMapper telemetryDataMapper,
            ITextBuffer buffer)
        {
            _buffer = buffer;
            _predictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _telemetryDataMapper = telemetryDataMapper ?? throw new ArgumentNullException(nameof(telemetryDataMapper));
        }

        public async Task<IEnumerable<ITagSpan<PerformanceTag>>> VisitAsync(SyntaxTree syntaxTree, SyntaxTree originalTree)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }
            if (originalTree == null)
            {
                throw new ArgumentNullException(nameof(originalTree));
            }

            var list = new List<ITagSpan<PerformanceTag>>();

            // getting Changes
            // https://github.com/dotnet/roslyn/issues/17498
            // https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn
            var treeChanges = syntaxTree.GetChanges(originalTree).Where(treeChange => !string.IsNullOrWhiteSpace(treeChange.NewText)).ToList();

            var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclarationSyntax in methods)
            {
                try
                {
                    var methodPerformanceInfo = await VisitMethodDeclarationAsync(methodDeclarationSyntax, list).ConfigureAwait(false); 
                    if (methodPerformanceInfo != null)
                    {
                        methodPerformanceInfo.HasChanged = HasTextChangesInNode(treeChanges, methodDeclarationSyntax);
                        if (methodPerformanceInfo.HasChanged)
                        {
                            methodPerformanceInfo.PredictExecutionTime();
                        }
                    }
                    list.Add(new TagSpan<PerformanceTag>(methodDeclarationSyntax.GetIdentifierSnapshotSpan(_buffer.CurrentSnapshot), 
                        new MethodPerformanceTag(methodPerformanceInfo)));
                }
                catch (ArgumentException e)
                {
                    // syntaxNode is not within tree anymore ...
                }
            }

            return list;
        }

        public async Task<MethodPerformanceInfo> VisitMethodDeclarationAsync(MethodDeclarationSyntax methodDeclarationSyntax, List<ITagSpan<PerformanceTag>> list)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (methodSymbol == null)
            {
                return null;
            }

            var methodPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(methodSymbol).ConfigureAwait(false);
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
                var invocationPerformanceInfo = await VisitInvocation(invocationExpressionSyntax).ConfigureAwait(false);
                if (invocationPerformanceInfo != null)
                {
                    methodPerformanceInfo.AddCalleePerformanceInfo(invocationPerformanceInfo);
                }
                list.Add(new TagSpan<PerformanceTag>(invocationExpressionSyntax.GetIdentifierSnapshotSpan(_buffer.CurrentSnapshot), 
                    new MethodInvocationPerformanceTag(invocationPerformanceInfo)));
            }

            // Loops
            // TODO RR: Clean and only Iterate once...
            var loopSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).Where(IsLoopSyntax);
            foreach (var loopSyntax in loopSyntaxs)
            {
                var loopPerformanceInfo = await VisitLoopAsync(loopSyntax, methodPerformanceInfo).ConfigureAwait(false);
                if (loopPerformanceInfo != null)
                {
                    methodPerformanceInfo.LoopPerformanceInfos.Add(loopPerformanceInfo);
                }
                list.Add(new TagSpan<PerformanceTag>(loopSyntax.GetIdentifierSnapshotSpan(_buffer.CurrentSnapshot),
                    new LoopPerformanceTag(loopPerformanceInfo)));
            }

            // TODO RR: all References:
            //var referencesToMethod = await SymbolFinder.FindReferencesAsync(methodSymbol, _document.Project.Solution).ConfigureAwait(false);
            return methodPerformanceInfo;
        }

        private Task<MethodPerformanceInfo> VisitInvocation(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
            if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
            {
                return null;
            }
            return _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol);
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
                var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol).ConfigureAwait(false);
                loopInvocationsList.Add(invocationPerformanceInfo);
            }
            var loopPerformanceInfo = new LoopPerformanceInfo(_predictionEngine, methodPerformanceInfo, loopInvocationsList);
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
