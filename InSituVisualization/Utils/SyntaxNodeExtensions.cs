using DryIoc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

namespace InSituVisualization.Utils
{
    internal static class SyntaxNodeExtensions
    {
        private static CustomSpanProvider SpanProvider { get; } = IocHelper.Container.Resolve<CustomSpanProvider>();

        public static SnapshotSpan GetIdentifierSnapshotSpan(this SyntaxNode syntax, ITextSnapshot textSnapshot)
        {
            return new SnapshotSpan(textSnapshot, SpanProvider.GetSpan(syntax));
        }
    }
}
