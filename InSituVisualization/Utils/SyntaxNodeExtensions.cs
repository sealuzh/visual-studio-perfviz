using DryIoc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace InSituVisualization.Utils
{
    internal static class SyntaxNodeExtensions
    {
        private static CustomSpanProvider SpanProvider { get; } = IocHelper.Container.Resolve<CustomSpanProvider>();

        public static SnapshotSpan GetIdentifierSnapshotSpan(this SyntaxNode syntax, ITextSnapshot textSnapshot)
        {
            return new SnapshotSpan(textSnapshot, SpanProvider.GetSpan(syntax));
        }

        public static TagSpan<T> GetTagSpan<T>(this SyntaxNode syntax, ITextSnapshot textSnapshot, T tag)
            where T : ITag
        {
            return new TagSpan<T>(syntax.GetIdentifierSnapshotSpan(textSnapshot), tag);
        }

    }
}
