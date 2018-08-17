using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryMapper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
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
        private SyntaxTree _originalTree;

        public Document Document => _textView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPerformanceAdorner"/> class.
        /// </summary>
        /// <param name="textView">Text textView to create the adornment for</param>
        public MemberPerformanceAdorner(IWpfTextView textView)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
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
            var syntaxTree = await GetValidSyntaxTreeAsync();
            if (syntaxTree == null)
            {
                return;
            }

            try
            {
                var semanticModel = await Document.GetSemanticModelAsync();

                var telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>();
                var predictionEngine = IocHelper.Container.Resolve<IPredictionEngine>();
                var performanceSyntaxWalker = new AsyncSyntaxWalker(
                    predictionEngine,
                    Document,
                    semanticModel,
                    telemetryDataMapper, _methodAdornerLayer);
                await performanceSyntaxWalker.VisitAsync(syntaxTree, _originalTree);
            }
            catch (Exception exception)
            {
                ActivityLog.LogWarning(GetType().FullName,exception.Message);
            }
        }

        private async Task<SyntaxTree> GetValidSyntaxTreeAsync()
        {
            if (Document == null)
            {
                return null;
            }
            var syntaxTree = await Document.GetSyntaxTreeAsync().ConfigureAwait(false);
            if (syntaxTree.GetDiagnostics().Any())
            {
                // there are errors in the code -> do not perform operations
                return null;
            }
            if (_originalTree == null)
            {
                _originalTree = syntaxTree;
            }
            return syntaxTree;
        }
    }
}
