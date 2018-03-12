using System.Runtime.InteropServices;
using DryIoc;
using InSituVisualization.Filter;
using InSituVisualization.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace InSituVisualization.Views
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
    [Guid("8f900c40-6134-4fd6-86f1-4b7a85142554")]
    public class PerformanceFilterWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceFilterWindow"/> class.
        /// </summary>
        public PerformanceFilterWindow() : base(null)
        {
            this.Caption = "PerformanceFilterWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new PerformanceFilterWindowControl { DataContext = new PerformanceFilterWindowControlViewModel(IocHelper.Container.Resolve<IFilterController>()) };
        }
    }
}
