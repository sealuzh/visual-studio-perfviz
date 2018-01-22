using System;
using System.Diagnostics;
using DryIoc;
using InSituVisualization.TelemetryMapper;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace InSituVisualization
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
        private readonly MethodAdornmentLayer _methodAdornerLayer;

        private SnapshotPoint CaretPosition => _textView.Caret.Position.BufferPosition;

        private Document Document => CaretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPerformanceAdorner"/> class.
        /// </summary>
        /// <param name="textView">Text textView to create the adornment for</param>
        public MemberPerformanceAdorner(IWpfTextView textView)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _textView.LayoutChanged += OnLayoutChanged;
            var spanProvider = IocHelper.Container.Resolve<CustomSpanProvider>();
            _methodAdornerLayer = new MethodAdornmentLayer(textView, spanProvider);
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

            try
            {
                var semanticModel = await Document.GetSemanticModelAsync();

                var telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>();

                var performanceSyntaxWalker = new PerformanceSyntaxWalker(
                    semanticModel, 
                    telemetryDataMapper, _methodAdornerLayer);

                var root = await Document.GetSyntaxRootAsync();
                performanceSyntaxWalker.Visit(root);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }
}
