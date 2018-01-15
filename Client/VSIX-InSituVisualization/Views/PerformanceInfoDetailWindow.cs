using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSIX_InSituVisualization.ViewModels;

namespace VSIX_InSituVisualization.Views
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("209575cb-7e5f-4565-a3ad-c7c3d4fdcfe1")]
    public sealed class PerformanceInfoDetailWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceInfoDetailWindow"/> class.
        /// </summary>
        public PerformanceInfoDetailWindow() : base(null)
        {
            Caption = "MethodPerformanceInfo";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Settings.PerformanceInfoDetailWindowViewModel = new PerformanceInfoDetailWindowViewModel();
            Content = new Views.PerformanceInfoDetailWindowControl { DataContext = Settings.PerformanceInfoDetailWindowViewModel };
        }
    }
}
