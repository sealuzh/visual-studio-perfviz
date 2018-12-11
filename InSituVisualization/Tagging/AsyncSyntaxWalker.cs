using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using InSituVisualization.Model;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryMapper;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace InSituVisualization.Tagging
{
    /// <summary>
    /// Class does not inherit from CSharpSyntaxWalker since it wasn't possible to walk the tree in an async way...
    /// </summary>
    internal class AsyncSyntaxWalker
    {
        private readonly IPredictionEngine _predictionEngine;
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;


        public AsyncSyntaxWalker(SemanticModel semanticModel)
        {
            _predictionEngine = IocHelper.Container.Resolve<IPredictionEngine>() ?? throw new ArgumentNullException(nameof(_predictionEngine));
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>() ?? throw new ArgumentNullException(nameof(_telemetryDataMapper));
        }


        private Dictionary<SyntaxNode, PerformanceTag> PerformanceTags { get; set; }

        private List<TextChange> TreeChanges { get; set; }

        public async Task<IDictionary<SyntaxNode, PerformanceTag>> VisitAsync(SyntaxTree syntaxTree, SyntaxTree originalTree)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }
            if (originalTree == null)
            {
                throw new ArgumentNullException(nameof(originalTree));
            }

            PerformanceTags = new Dictionary<SyntaxNode, PerformanceTag>();

            // getting Changes
            // https://github.com/dotnet/roslyn/issues/17498
            // https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn
            TreeChanges = syntaxTree.GetChanges(originalTree).Where(treeChange => !string.IsNullOrWhiteSpace(treeChange.NewText)).ToList();

            var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var methodDeclarationSyntax in methods)
            {
                try
                {
                    await VisitMethodDeclarationAsync(methodDeclarationSyntax).ConfigureAwait(false);
                }
                catch (ArgumentException e)
                {
                    // syntaxNode is not within tree anymore ...
                }
            }

            return PerformanceTags;
        }

        public async Task VisitMethodDeclarationAsync(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (methodSymbol == null)
            {
                return;
            }

            var methodPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(methodSymbol).ConfigureAwait(false);
            if (methodPerformanceInfo == null)
            {
                return;
            }

            // TODO RR: var syntaxReference = methodSymbol.DeclaringSyntaxReferences
            // syntaxReference.GetSyntax(context.CancellationToken);

            // Invocations in Method
            IList<MethodPerformanceInfo> invocationsList = new List<MethodPerformanceInfo>();
            var invocationExpressionSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
            foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
            {
                var invocationsTuple = await VisitInvocationAsync(invocationExpressionSyntax).ConfigureAwait(false);
                if (invocationsTuple != null)
                {
                    invocationsList.Add(invocationsTuple);
                }
            }
            methodPerformanceInfo.UpdateCallees(invocationsList);

            // Loops
            // TODO RR: Clean and only Iterate once...
            var loopSyntaxs = methodDeclarationSyntax.DescendantNodes(node => true).Where(n => n.IsLoopSyntax());
            var loopPerformanceInfos = new List<LoopPerformanceInfo>();
            foreach (var loopSyntax in loopSyntaxs)
            {
                loopPerformanceInfos.Add(await VisitLoopAsync(loopSyntax, methodPerformanceInfo).ConfigureAwait(false));
            }
            methodPerformanceInfo.UpdateLoops(loopPerformanceInfos);

            // TODO RR: Problem with textchanges: Insertions aren't recognized as change, since the space did not exist before...
            methodPerformanceInfo.HasChanged = methodDeclarationSyntax.HasTextChanges(TreeChanges);
            if (methodPerformanceInfo.HasChanged)
            {
                methodPerformanceInfo.PredictExecutionTime();
            }
            PerformanceTags.Add(methodDeclarationSyntax, new MethodPerformanceTag(methodPerformanceInfo));


            // TODO RR: all References:
            //var referencesToMethod = await SymbolFinder.FindCallersAsync(methodSymbol, _document.Project.Solution).ConfigureAwait(false);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private async Task<MethodPerformanceInfo> VisitInvocationAsync(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
            if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
            {
                return null;
            }

            var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol).ConfigureAwait(false);

            if (invocationPerformanceInfo != null)
            {
                PerformanceTags.Add(invocationExpressionSyntax, new MethodInvocationPerformanceTag(invocationPerformanceInfo));
            }
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
                if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
                {
                    continue;
                }
                var invocationPerformanceInfo = await _telemetryDataMapper.GetMethodPerformanceInfoAsync(invokedMethodSymbol).ConfigureAwait(false);
                loopInvocationsList.Add(invocationPerformanceInfo);
            }


            var loopPerformanceInfo = new LoopPerformanceInfo(_predictionEngine, methodPerformanceInfo, loopInvocationsList);

            var derivedLoopIterations = TryGetLoopIterationsFromCodeFlow(loopSyntax);
            if (derivedLoopIterations != 0)
            {
                loopPerformanceInfo.PredictedLoopIterations = derivedLoopIterations;
            }

            PerformanceTags.Add(loopSyntax, new LoopPerformanceTag(loopPerformanceInfo));
            return loopPerformanceInfo;
        }

        /// <summary>
        /// Performs Code Flow Analysis to determine the loop iterations
        /// </summary>
        /// <param name="loopSyntax">Loop to examine</param>
        /// <returns>Non-zero if successful, zero otherwise</returns>
        private int TryGetLoopIterationsFromCodeFlow(SyntaxNode loopSyntax)
        {
            try
            {
                switch (loopSyntax)
                {
                    case ForStatementSyntax forStatementSyntax:
                        return GetForLoopIterationsFromCodeFlow(forStatementSyntax);
                    case WhileStatementSyntax whileStatementSyntax:
                        // TODO RR:
                        return 0;
                    case ForEachStatementSyntax forEachStatementSyntax:
                        // TODO RR:
                        return 0;
                    case DoStatementSyntax doStatementSyntax:
                        // TODO RR:
                        return 0;
                }
            }
            catch (Exception e)
            {
                // ignore
            }
            return 0;
        }

        private int GetForLoopIterationsFromCodeFlow(ForStatementSyntax forStatementSyntax)
        {
            if (!(forStatementSyntax.Condition is BinaryExpressionSyntax binaryCondition))
            {
                return 0;
            }

            if (binaryCondition.Kind() == SyntaxKind.LessThanExpression ||
                binaryCondition.Kind() == SyntaxKind.LessThanOrEqualExpression)
            {
                // Left is variable
                var left = GetNumericLiteralExpression(binaryCondition.Left);
                if (left.Kind() == SyntaxKind.NumericLiteralExpression && int.TryParse(left.ToString(), out var leftValue))
                {
                    // Right is value
                    var right = GetNumericLiteralExpression(binaryCondition.Right);
                    if (right.Kind() == SyntaxKind.NumericLiteralExpression && int.TryParse(right.ToString(), out var rightValue))
                    {
                        return rightValue - leftValue;
                    }
                }

                //var conditiondataFlowAnalysis = _semanticModel.AnalyzeDataFlow(binaryCondition);
                //if (conditiondataFlowAnalysis.Succeeded)
                //{
                //    var symbol = conditiondataFlowAnalysis.ReadInside.FirstOrDefault();
                //    var syntax = symbol?.OriginalDefinition.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax();
                //    if (!(syntax is VariableDeclaratorSyntax variableDeclaratorSyntax))
                //    {
                //        return 0;
                //    }

                //    var value = GetNumericLiteralExpression(variableDeclaratorSyntax.Initializer.Value);
                //    if (value.Kind() == SyntaxKind.NumericLiteralExpression && int.TryParse(value.ToString(), out var intValue))
                //    {
                //        return intValue;
                //    }
                //}
            }
            return 0;
        }

        /// <summary>
        /// following variables until we find a declaration with a value
        /// </summary>
        /// <param name="expressionSyntax">An Expression</param>
        /// <returns>A SyntaxExpression of type SyntaxKind.NumericLiteralExpression or the topmost one found</returns>
        private ExpressionSyntax GetNumericLiteralExpression(ExpressionSyntax expressionSyntax)
        {
            do
            {
                if (expressionSyntax.Kind() == SyntaxKind.NumericLiteralExpression)
                {
                    break;
                }

                var valueSymbolInfo = _semanticModel.GetSymbolInfo(expressionSyntax);
                if (valueSymbolInfo.Symbol.OriginalDefinition.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax() is VariableDeclaratorSyntax valueDeclaration)
                {
                    if (valueDeclaration.Initializer == null)
                    {
                        break;
                    }
                    expressionSyntax = valueDeclaration.Initializer.Value;
                }
            } while (expressionSyntax is IdentifierNameSyntax);
            return expressionSyntax;
        }
    }
}
