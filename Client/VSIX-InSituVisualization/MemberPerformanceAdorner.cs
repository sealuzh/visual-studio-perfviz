using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSIX_InSituVisualization.TelemetryCollector;

namespace VSIX_InSituVisualization
{
    /// <summary>
    /// MemberPerformanceAdorner shows the PerformanceInfo on the specified Members
    /// </summary>
    internal sealed class MemberPerformanceAdorner
    {

        /// <summary>
        /// Text textView where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _textView;
        private readonly MethodAdornmentLayer _methodAdornerLayer;

        private readonly Dictionary<MemberDeclarationSyntax, PerformanceInfo> _performanceInfos = new Dictionary<MemberDeclarationSyntax, PerformanceInfo>();

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



        private PerformanceInfo GetPerformanceInfo(MemberDeclarationSyntax memberDeclaraitonSyntax)
        {
            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            if (_performanceInfos.TryGetValue(memberDeclaraitonSyntax, out var perfInfo))
            {
                return perfInfo;
            }

            var memberName = memberDeclaraitonSyntax.GetMemberIdentifier().ToString();
            try
            {
                // TODO RR: Do Real Mapping
                var dataStore = AzureTelemetryFactory.GetInstance();
                //dataStore.AddFilter(dataStore.GetFilterProperties()["City"], "IsEqual", "Zurich");
                var averageMemberTelemetries = dataStore.GetCurrentAveragedMemberTelemetry();
                // is null when being written to at the same time
                if (averageMemberTelemetries != null)
                {
                    // if no information given for this method it does not exist in dict
                    if (!averageMemberTelemetries.ContainsKey(memberName))
                    {
                        return null;
                    }
                    else
                    {
                        var performanceInfo = new PerformanceInfo(memberName)
                        {
                            MeanExecutionTime = averageMemberTelemetries[memberName]
                        };

                        _performanceInfos.Add(memberDeclaraitonSyntax, performanceInfo);
                        return performanceInfo;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
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
                var performanceInfo = GetPerformanceInfo(memberDeclarationSyntax);
                if (performanceInfo == null)
                {
                    continue;
                }

                var span = customSpanObtainer.GetSpan(memberDeclarationSyntax);
                if (span == default(Span))
                {
                    continue;
                }

                var snapshotSpan = new SnapshotSpan(_textView.TextSnapshot, span);

#if DEBUG_SPANS
                _methodAdornerLayer.DrawRedSpan(snapshotSpan);
#endif

                _methodAdornerLayer.DrawPerformanceInfo(snapshotSpan, performanceInfo);

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
