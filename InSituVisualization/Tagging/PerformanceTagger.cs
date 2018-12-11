using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace InSituVisualization.Tagging
{
    /// <inheritdoc />
    /// <summary>
    /// Determines which spans of text get a PerformanceTag
    /// </summary>
    /// <remarks>
    /// This is a data-only component. The tagging system is a good fit for presenting data-about-text.
    /// The <see cref="T:InSituVisualization.Tagging.PerformanceAdornmentTagger" /> takes performance tags produced by this tagger and creates corresponding UI for this data.
    /// </remarks>
    internal class PerformanceTagger : ITagger<PerformanceTag>
    {

        #region Cache

        private class RoslynCache
        {
            private RoslynCache() { }

            public SemanticModel SemanticModel { get; private set; }
            public SyntaxTree SyntaxTree { get; private set; }
            public ITextSnapshot Snapshot { get; private set; }

            public static async Task<RoslynCache> Resolve(ITextSnapshot snapshot)
            {
                var document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
                if (document == null)
                {
                    return null;
                }
                return new RoslynCache
                {
                    SemanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false),
                    SyntaxTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false),
                    Snapshot = snapshot
                };
            }
        }

        #endregion

        // TODO RR: Remove
        private SyntaxTree _originalTree;
        private RoslynCache _roslynCache;
        private IList<TagSpan<PerformanceTag>> _performanceTagSpansCache;

        public PerformanceTagger(ITextBuffer buffer)
        {
            buffer.Changed += (sender, args) => HandleBufferChanged(args);
        }

        #region ITagger implementation

        public IEnumerable<ITagSpan<PerformanceTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            if (!UpdateCache(spans))
            {
                yield break;
            }

            // Only returning requested spans
            foreach (var span in spans)
            {
                foreach (var performanceTag in _performanceTagSpansCache)
                {
                    var newSnapshotSpan = performanceTag.Span.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive);
                    if (span.IntersectsWith(newSnapshotSpan))
                    {
                        yield return performanceTag;
                    }
                }
            }
        }

        private bool UpdateCache(NormalizedSnapshotSpanCollection spans)
        {
            var currentSnapshot = spans[0].Snapshot;
            if (_roslynCache != null && _roslynCache.Snapshot == currentSnapshot && _performanceTagSpansCache != null)
            {
                // no changes
                return true;
            }

            _roslynCache = RoslynCache.Resolve(currentSnapshot).Result;
            if (_roslynCache == null)
            {
                return false;
            }
            if (_roslynCache.SyntaxTree.GetDiagnostics().Any())
            {
                // there are errors in the code -> do not perform operations
                return false;
            }

            if (_originalTree == null)
            {
                _originalTree = _roslynCache.SyntaxTree;
            }

            try
            {
                var performanceSyntaxWalker = new AsyncSyntaxWalker(_roslynCache.SemanticModel);
                var nodeTags = performanceSyntaxWalker.VisitAsync(_roslynCache.SyntaxTree, _originalTree).Result;
                _performanceTagSpansCache = nodeTags.Select(perfTagKvp => perfTagKvp.Key.GetTagSpan(currentSnapshot, perfTagKvp.Value)).ToList();
                return true;
            }
            catch (Exception e)
            {
                ErrorHandler.ReportError(e);
                return false;
            }
        }


        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion

        /// <summary>
        /// Handle buffer changes. The default implementation expands changes to full lines and sends out
        /// a <see cref="TagsChanged"/> event for these lines.
        /// </summary>
        /// <param name="args">The buffer change arguments.</param>
        private void HandleBufferChanged(TextContentChangedEventArgs args)
        {
            if (args.Changes.Count == 0)
                return;

            var temp = TagsChanged;
            if (temp == null)
                return;

            // Combine all changes into a single span so that
            // the ITagger<>.TagsChanged event can be raised just once for a compound edit
            // with many parts.

            var snapshot = args.After;
            var start = args.Changes[0].NewPosition;
            var end = args.Changes[args.Changes.Count - 1].NewEnd;

            var totalAffectedSpan = new SnapshotSpan(snapshot.GetLineFromPosition(start).Start, snapshot.GetLineFromPosition(end).End);

            temp(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }
    }
}
