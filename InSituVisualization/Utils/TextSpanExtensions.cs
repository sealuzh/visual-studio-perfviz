using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace InSituVisualization.Utils
{
    internal static class TextSpanExtensions
    {
        /// <summary>
        /// Convert a <see cref="TextSpan"/> instance to a <see cref="TextSpan"/>.
        /// </summary>
        public static Span ToSpan(this TextSpan textSpan)
        {
            return new Span(textSpan.Start, textSpan.Length);
        }

        /// <summary>
        /// Convert a <see cref="TextSpan"/> to a <see cref="SnapshotSpan"/> on the given <see cref="ITextSnapshot"/> instance
        /// </summary>
        public static SnapshotSpan ToSnapshotSpan(this TextSpan textSpan, ITextSnapshot snapshot)
        {
            Debug.Assert(snapshot != null);
            var span = textSpan.ToSpan();
            return new SnapshotSpan(snapshot, span);
        }
    }
}
