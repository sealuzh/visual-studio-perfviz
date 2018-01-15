using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSIX_InSituVisualization.TelemetryMapper;

namespace VSIX_InSituVisualization
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
            // Method
            var methodSymbol = _semanticModel.GetDeclaredSymbol(memberDeclarationSyntax);
            if (methodSymbol == null)
            {
                return;
            }

            var methodPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(methodSymbol);
            _methodAdornmentLayer.DrawMethodPerformanceInfo(memberDeclarationSyntax, methodPerformanceInfo);

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
            // TODO RR:
        }
    }
}
