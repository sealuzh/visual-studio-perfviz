using DryIoc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace InSituVisualization.Utils
{
    internal static class SyntaxNodeExtensions
    {
        private static CustomSpanProvider SpanProvider { get; } = IocHelper.Container.Resolve<CustomSpanProvider>();

        public static SnapshotSpan GetIdentifierSnapshotSpan(this SyntaxNode syntax, IWpfTextView textView)
        {
            var methodSyntaxSpan = SpanProvider.GetSpan(syntax);
            return new SnapshotSpan(textView.TextSnapshot, methodSyntaxSpan);
        }

        public static SnapshotSpan GetSnapshotSpan(this SyntaxNode syntax, IWpfTextView textView)
        {
            return new SnapshotSpan(textView.TextSnapshot, Span.FromBounds(syntax.SpanStart, syntax.FullSpan.End));
        }
    }
}
