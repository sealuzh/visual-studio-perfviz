using System;
using System.Collections.Generic;
using InSituVisualization.ViewModels;
using InSituVisualization.Views;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace InSituVisualization.Tagging
{
    /// <summary>
    /// Provides performance adornments
    /// </summary>
    internal sealed class PerformanceAdornmentTagger: IntraTextAdornmentTagger<PerformanceTag, InSituMethodControl>
    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<PerformanceTag>> colorTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new PerformanceAdornmentTagger(view, colorTagger.Value));
        }



        private readonly ITagAggregator<PerformanceTag> _performanceTagger;

        private PerformanceAdornmentTagger(IWpfTextView view, ITagAggregator<PerformanceTag> performanceTagger)
            : base(view)
        {
            _performanceTagger = performanceTagger;
        }

        public void Dispose()
        {
            _performanceTagger.Dispose();
            View.Properties.RemoveProperty(typeof(PerformanceAdornmentTagger));
        }

        // To produce adornments that don't obscure the text, the adornment tags
        // should have zero length spans. Overriding this method allows control
        // over the tag spans.
        protected override IEnumerable<Tuple<SnapshotSpan, PositionAffinity?, PerformanceTag>> GetAdornmentData(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            var snapshot = spans[0].Snapshot;

            var colorTags = _performanceTagger.GetTags(spans);

            foreach (IMappingTagSpan<PerformanceTag> dataTagSpan in colorTags)
            {
                var tagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (tagSpans.Count != 1)
                {
                    continue;
                }

                // -2 for the /r/n
                var adornmentSpan = new SnapshotSpan(tagSpans[0].End - 2, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override InSituMethodControl CreateAdornment(PerformanceTag dataTag, SnapshotSpan span)
        {
            return new InSituMethodControl
            {
                DataContext = new InSituMethodControlViewModel(dataTag.PerformanceInfo)
            };
        }

        protected override bool UpdateAdornment(InSituMethodControl adornment, PerformanceTag dataTag)
        {
            // TODO RR: 
            //adornment.Update(dataTag);
            return true;
        }
    }
}
