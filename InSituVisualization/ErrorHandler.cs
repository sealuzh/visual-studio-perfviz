using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace InSituVisualization
{
    internal static class ErrorHandler
    {
        public static Guid ErrorOutputGuid = new Guid("21FB39E4-70E4-4474-91A1-B1CC319F00F1");
        public const string ErrorOutputTitle = "In Situ Visualization Output";

        public static void ReportError(Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    ReportError(innerException.Message);
                }
            }
            else
            {
                ReportError(exception.Message);
            }
        }

        public static void ReportError(string error)
        {
            var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            outWindow?.CreatePane(ref ErrorOutputGuid, ErrorOutputTitle, 1, 1);

            // ReSharper disable once InlineOutVariableDeclaration
            IVsOutputWindowPane customPane = null;
            outWindow?.GetPane(ref ErrorOutputGuid, out customPane);

            customPane?.OutputString($"ERROR: {error}");
            customPane?.Activate(); // Brings this pane into view
        }
    }
}
