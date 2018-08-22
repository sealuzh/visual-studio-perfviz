using System.Collections.Generic;
using System.Linq;
using DryIoc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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

        public static bool IsLoopSyntax(this SyntaxNode node)
        {
            return node is ForStatementSyntax ||
                   node is WhileStatementSyntax ||
                   node is DoStatementSyntax ||
                   node is ForEachStatementSyntax;
        }

        public static bool HasTextChanges(this SyntaxNode syntaxNode, IEnumerable<TextChange> textChanges)
        {
            return textChanges.Any(textChange => syntaxNode.Span.IntersectsWith(new TextSpan(textChange.Span.Start, textChange.NewText.Length)));
        }

    }
}
