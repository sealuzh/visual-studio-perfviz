using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using VSIX_InSituVisualization.TelemetryMapper;

namespace VSIX_InSituVisualization
{
    class PerformanceSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly IWpfTextView _textView;
        private readonly SemanticModel _semanticModel;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly MethodAdornmentLayer _methodAdornmentLayer;
        private readonly CustomSpanProvider _spanProvider;

        public PerformanceSyntaxWalker(IWpfTextView textView, SemanticModel semanticModel, ITelemetryDataMapper telemetryDataMapper, MethodAdornmentLayer methodAdornmentLayer, CustomSpanProvider spanProvider)
        {
            _textView = textView;
            _semanticModel = semanticModel;
            _telemetryDataMapper = telemetryDataMapper;
            _methodAdornmentLayer = methodAdornmentLayer;
            _spanProvider = spanProvider;
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
            _methodAdornmentLayer.DrawMethodPerformanceInfo(GetSnapshotSpan(memberDeclarationSyntax), methodPerformanceInfo);

#if DEBUG_SPANS
            _methodAdornerLayer.DrawRedSpan(snapshotSpan);
#endif

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
                _methodAdornmentLayer.DrawMethodInvocationPerformanceInfo(GetSnapshotSpan(invocationExpressionSyntax), invocationPerformanceInfo);
            }

            // Loops
            // TODO RR:
        }


        private SnapshotSpan GetSnapshotSpan(CSharpSyntaxNode syntax)
        {
            var methodSyntaxSpan = _spanProvider.GetSpan(syntax);
            return new SnapshotSpan(_textView.TextSnapshot, methodSyntaxSpan);
        }
    }
}
