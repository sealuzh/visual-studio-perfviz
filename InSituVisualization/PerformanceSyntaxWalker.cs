using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;
using InSituVisualization.TelemetryMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InSituVisualization
{
    internal class PerformanceSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly MethodAdornmentLayer _methodAdornmentLayer;

        public PerformanceSyntaxWalker(SemanticModel semanticModel, ITelemetryDataMapper telemetryDataMapper, MethodAdornmentLayer methodAdornmentLayer)
        {
            _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            _telemetryDataMapper = telemetryDataMapper ?? throw new ArgumentNullException(nameof(telemetryDataMapper));
            _methodAdornmentLayer = methodAdornmentLayer ?? throw new ArgumentNullException(nameof(methodAdornmentLayer)); ;
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax memberDeclarationSyntax)
        {
            var methodSymbol = _semanticModel.GetDeclaredSymbol(memberDeclarationSyntax);
            if (methodSymbol == null)
            {
                return;
            }

            var methodPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(methodSymbol);
            _methodAdornmentLayer.DrawMethodPerformanceInfo(memberDeclarationSyntax, methodPerformanceInfo);

            // TODO RR: var syntaxReference = methodSymbol.DeclaringSyntaxReferences
            // syntaxReference.GetSyntax(context.CancellationToken);

            // Invocations in Method
            var invocationExpressionSyntaxs = memberDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
            foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
            {
                var invokedMethodSymbol = _semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
                // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
                if (invokedMethodSymbol == null || !Equals(_semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly))
                {
                    continue;
                }
                var invocationPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(invokedMethodSymbol);
                // Setting Caller and CalleeInformation
                invocationPerformanceInfo.CallerPerformanceInfo.Add(methodPerformanceInfo);
                methodPerformanceInfo.CalleePerformanceInfo.Add(invocationPerformanceInfo);
                _methodAdornmentLayer.DrawMethodInvocationPerformanceInfo(invocationExpressionSyntax, invocationPerformanceInfo);
            }

            // Loops
            // TODO RR: Clean and only Iterate once...
            var loopSyntaxs = memberDeclarationSyntax.DescendantNodes(node => true).Where(node =>
                 node is ForStatementSyntax ||
                 node is WhileStatementSyntax ||
                 node is DoStatementSyntax ||
                 node is ForEachStatementSyntax);

            foreach (var loopSyntax in loopSyntaxs)
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
                    var invocationPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(invokedMethodSymbol);
                    loopInvocationsList.Add(invocationPerformanceInfo);
                }
                _methodAdornmentLayer.DrawLoopPerformanceInfo(loopSyntax, new LoopPerformanceInfo(methodPerformanceInfo, loopInvocationsList));
            }
        }
    }
}
