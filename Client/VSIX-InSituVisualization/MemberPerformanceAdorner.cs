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
        public MemberPerformanceAdorner(IWpfTextView textView, ITelemetryDataMapper telemetryDataMapper)
        {
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
            var memberDeclarationSyntaxs = root.DescendantNodes().OfType<MemberDeclarationSyntax>();
            var customSpanObtainer = new CustomSpanProvider();
            foreach (var memberDeclarationSyntax in memberDeclarationSyntaxs)
            {
                var performanceInfo = _telemetryDataMapper.GetPerformanceInfo(memberDeclarationSyntax);
                if (performanceInfo == null)
                {
                    continue;
                }

                var methodSyntaxSpan = customSpanObtainer.GetSpan(memberDeclarationSyntax);
                if (methodSyntaxSpan == default(Span))
                {
                    continue;
                }

#if DEBUG_SPANS
                _methodAdornerLayer.DrawRedSpan(snapshotSpan);
#endif

                _methodAdornerLayer.DrawMethodDeclarationPerformanceInfo(new SnapshotSpan(_textView.TextSnapshot, methodSyntaxSpan), performanceInfo);

                var invocationExpressionSyntaxs = memberDeclarationSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocationExpressionSyntax in invocationExpressionSyntaxs)
                {
                    var invocationSynataxSpan = customSpanObtainer.GetSpan(invocationExpressionSyntax);

                    _methodAdornerLayer.DrawMethodInvocationPerformanceInfo(new SnapshotSpan(_textView.TextSnapshot, invocationSynataxSpan), performanceInfo);
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
