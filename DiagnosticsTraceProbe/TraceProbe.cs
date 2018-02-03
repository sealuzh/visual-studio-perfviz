using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DiagnosticsTraceProbe
{
    // ReSharper disable once UnusedMember.Global Justification: Reflection
    public class TraceProbe
    {

        private static readonly ConcurrentDictionary<string, DateTime> MethodStartDateTimes = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// This is the Method executed before the Method Body
        /// Important: Name must match <see cref="OnBeforeMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnBeforeMethod(string documentationCommentId)
        {
            Trace.WriteLine($"{DateTime.UtcNow} - {documentationCommentId}. Started");
            MethodStartDateTimes.TryAdd(documentationCommentId, DateTime.UtcNow);
        }

        /// <summary>
        /// This is the Method executed after the Method Body
        /// Important: Name must match <see cref="OnAfterMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnAfterMethod(string documentationCommentId)
        {
            if (!MethodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }
            var duration = DateTime.UtcNow - startDateTime;
            Trace.WriteLine($"{DateTime.UtcNow} - {documentationCommentId}. Duration: {duration.Milliseconds} ms");
        }

        /// <summary>
        /// This is the Method executed before exceptions
        /// Important: Name must match <see cref="OnException"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnException(string documentationCommentId)
        {
            Trace.WriteLine($"{DateTime.UtcNow} - {documentationCommentId}. Exception Occurred");
        }
    }
}
