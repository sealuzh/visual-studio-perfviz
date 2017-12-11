using System;
using System.Linq;
using Microsoft.CodeAnalysis;
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
        private CachedTelemetryDataMapper _cachedTelemetryDataMapper;

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
            _cachedTelemetryDataMapper = new CachedTelemetryDataMapper(telemetryDataMapper);
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
            var memberDeclarationSyntaxs = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var memberDeclarationSyntax in memberDeclarationSyntaxs)
            {
                var performanceInfo = _cachedTelemetryDataMapper.GetPerformanceInfo(memberDeclarationSyntax);
                if (performanceInfo == null)
                {
                    continue;
                }

                var methodSyntaxSpan = _spanProvider.GetSpan(memberDeclarationSyntax);
                if (methodSyntaxSpan == default(Span))
                {
                    continue;
                }

#if DEBUG_SPANS
                _methodAdornerLayer.DrawRedSpan(snapshotSpan);
#endif

                _methodAdornerLayer.DrawMethodDeclarationPerformanceInfo(memberDeclarationSyntax, new SnapshotSpan(_textView.TextSnapshot, methodSyntaxSpan), performanceInfo);

                var invocationExpressionSyntaxs = memberDeclarationSyntax.DescendantNodes(node => true).OfType<InvocationExpressionSyntax>();
                foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
                {
                    var invocationPerformanceInfo = _telemetryDataMapper.GetPerformanceInfo(invocationExpressionSyntax.ToString());
                    var invocationSynataxSpan = _spanProvider.GetSpan(invocationExpressionSyntax);
                    var invocationSnapshotSpan = new SnapshotSpan(_textView.TextSnapshot, invocationSynataxSpan);
                    _methodAdornerLayer.DrawRedSpan(invocationSnapshotSpan);
                    _methodAdornerLayer.DrawMethodInvocationPerformanceInfo(invocationExpressionSyntax, invocationSnapshotSpan, invocationPerformanceInfo);
                }

                // Setting PerformanceInfo to Method from Caret
                if (CaretPosition.Position > memberDeclarationSyntax.SpanStart &&
                    CaretPosition.Position < memberDeclarationSyntax.FullSpan.End &&
                    Settings.PerformanceInfoDetailWindowViewModel != null)
                {
                    Settings.PerformanceInfoDetailWindowViewModel.PerformanceInfo = performanceInfo;
                }
            }
        }
    }
}
