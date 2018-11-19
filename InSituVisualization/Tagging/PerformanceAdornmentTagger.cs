using System;
using System.Collections.Generic;
using System.Windows.Controls;
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
    internal sealed class PerformanceAdornmentTagger: IntraTextAdornmentTagger<PerformanceTag, UserControl>
    {
        private readonly ITagAggregator<PerformanceTag> _performanceTagAggregator;

        internal PerformanceAdornmentTagger(IWpfTextView view, ITagAggregator<PerformanceTag> performanceTagAggregator)
            : base(view)
        {
            _performanceTagAggregator = performanceTagAggregator;
        }

        public void Dispose()
        {
            _performanceTagAggregator.Dispose();
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

            var tags = _performanceTagAggregator.GetTags(spans);

            foreach (IMappingTagSpan<PerformanceTag> dataTagSpan in tags)
            {
                var tagSpans = dataTagSpan.Span.GetSpans(snapshot);

                // Ignore data tags that are split by projection.
                // This is theoretically possible but unlikely in current scenarios.
                if (tagSpans.Count != 1)
                {
                    continue;
                }
                var adornmentSpan = new SnapshotSpan(tagSpans[0].End, 0);

                yield return Tuple.Create(adornmentSpan, (PositionAffinity?)PositionAffinity.Successor, dataTagSpan.Tag);
            }
        }

        protected override UserControl CreateAdornment(PerformanceTag performanceTag, SnapshotSpan span)
        {
            switch (performanceTag)
            {
                case MethodPerformanceTag mt:
                    return new InSituMethodControl
                    {
                        DataContext = new InSituMethodControlViewModel(mt.PerformanceInfo)
                    };
                case MethodInvocationPerformanceTag mit:
                    return new InSituMethodInvocationControl
                    {
                        DataContext = new InSituMethodInvocationControlViewModel(mit.PerformanceInfo)
                    };
                case LoopPerformanceTag lt:
                    return new InSituLoopControl
                    {
                        DataContext = new InSituLoopControlViewModel(lt.LoopPerformanceInfo)
                    };
                default:
                    throw new NotImplementedException("Unknown PerformanceTag");
            }


        }

        protected override bool UpdateAdornment(UserControl adornment, PerformanceTag dataTag)
        {
            // TODO RR: 
            //adornment.Update(dataTag);
            return true;
        }
    }
}
