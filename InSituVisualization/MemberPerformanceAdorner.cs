using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DryIoc;
using InSituVisualization.TelemetryCollector;
using InSituVisualization.TelemetryMapper;
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
        private SyntaxTree _originalTree;

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

            var syntaxTree = await Document.GetSyntaxTreeAsync().ConfigureAwait(false);
            if (_originalTree == null)
            {
                _originalTree = syntaxTree;
            }
            var diagnostics = syntaxTree.GetDiagnostics();
            if (diagnostics.Any())
            {
                // there are errors in the code -> do not perform operations
                return;
            }

            try
            {
                // getting Changes
                // https://github.com/dotnet/roslyn/issues/17498
                // https://stackoverflow.com/questions/34243031/reliably-compare-type-symbols-itypesymbol-with-roslyn
                var treeChanges = syntaxTree.GetChanges(_originalTree).Where(treeChange => !string.IsNullOrWhiteSpace(treeChange.NewText)).ToList();

                var semanticModel = await Document.GetSemanticModelAsync();

                // TODO RR: THIS IS A MAYOR WORKAROUND TO FIX THE DEFERRED DLL LOADING PROBLEM:
                // SOME DDLS, SUCH AS NEWTONSOFT.JSON ARE NOT LOADABLE IN THE FIRST SECONDS...
                var telemetryProvider = IocHelper.Container.Resolve<ITelemetryProvider>();
                await (telemetryProvider as StoreManager)?.StartBackgroundWorkerAsync(CancellationToken.None);


                var telemetryDataMapper = IocHelper.Container.Resolve<ITelemetryDataMapper>();

                var performanceSyntaxWalker = new PerformanceSyntaxWalker(
                    semanticModel, treeChanges,
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
