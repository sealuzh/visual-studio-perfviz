using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{
    /// <summary>
    /// MemberPerformanceAdorner shows the PerformanceInfo on the specified Members
    /// There is one <see cref="MemberPerformanceAdorner"/> per open document.
    /// </summary>
    internal sealed class MemberPerformanceAdorner
    {
        private readonly CustomSpanProvider _spanProvider;

        /// <summary>
        /// Text textView where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _textView;
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly MethodAdornmentLayer _methodAdornerLayer;

        private SnapshotPoint CaretPosition => _textView.Caret.Position.BufferPosition;

        private Document Document => CaretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPerformanceAdorner"/> class.
        /// </summary>
        /// <param name="textView">Text textView to create the adornment for</param>
        /// <param name="telemetryDataMapper"></param>
        /// <param name="spanProvider"></param>
        public MemberPerformanceAdorner(IWpfTextView textView, ITelemetryDataMapper telemetryDataMapper, CustomSpanProvider spanProvider)
        {
            _spanProvider = spanProvider ?? throw new ArgumentNullException(nameof(spanProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _telemetryDataMapper = telemetryDataMapper ?? throw new ArgumentNullException(nameof(textView));
            _textView.LayoutChanged += OnLayoutChanged;
            _methodAdornerLayer = new MethodAdornmentLayer(textView);
        }


        /// <summary>
        /// Handles whenever the text displayed in the textView changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the textView does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the textView scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (Document == null)
            {
                return;
            }
            var root = await Document.GetSyntaxRootAsync();
            var semanticModel = await Document.GetSemanticModelAsync();
            try
            {
                DrawTelemetryData(root, semanticModel);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

        }

        private void DrawTelemetryData(SyntaxNode root, SemanticModel semanticModel)
        {
            var memberDeclarationSyntaxs = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var memberDeclarationSyntax in memberDeclarationSyntaxs)
            {
                // Method
                var methodSymbol = semanticModel.GetDeclaredSymbol(memberDeclarationSyntax);
                if (methodSymbol == null)
                {
                    continue;
                }
                //TODO RR: Hier muss zuerst auf neue Inhalte geprüft werden - ansonsten wenn einmal null --> immer null.
                var methodPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(methodSymbol);
                _methodAdornerLayer.DrawMethodDeclarationPerformanceInfo(memberDeclarationSyntax, GetSnapshotSpan(memberDeclarationSyntax), methodPerformanceInfo);
#if DEBUG_SPANS
                _methodAdornerLayer.DrawRedSpan(snapshotSpan);
#endif

                // Invocations in Method
                var invocationExpressionSyntaxs = memberDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
                foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
                {
                    var invokedMethodSymbol = semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
                    // Only Drawing invocationSymbols that refer to the current assembly. Not drawing Information about other assemblies...
                    if (invokedMethodSymbol == null || !Equals(semanticModel.Compilation.Assembly, invokedMethodSymbol.ContainingAssembly) )
                    {
                        continue;
                    }
                    var invocationPerformanceInfo = _telemetryDataMapper.GetMethodPerformanceInfo(invokedMethodSymbol);
                    // Setting Caller and CalleeInformation
                    invocationPerformanceInfo.CallerPerformanceInfo.Add(methodPerformanceInfo);
                    methodPerformanceInfo.CalleePerformanceInfo.Add(invocationPerformanceInfo);
                    _methodAdornerLayer.DrawMethodInvocationPerformanceInfo(invocationExpressionSyntax, GetSnapshotSpan(invocationExpressionSyntax), invocationPerformanceInfo);
                }
            }
        }

        private SnapshotSpan GetSnapshotSpan(CSharpSyntaxNode syntax)
        {
            var methodSyntaxSpan = _spanProvider.GetSpan(syntax);
            return new SnapshotSpan(_textView.TextSnapshot, methodSyntaxSpan);
        }
    }
}
