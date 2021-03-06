﻿using System.Runtime.InteropServices;
using DryIoc;
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
    [Guid("209575cb-7e5f-4565-a3ad-c7c3d4fdcfe1")]
    public sealed class DetailWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailWindow"/> class.
        /// </summary>
        public DetailWindow() : base(null)
        {
            Caption = "Performance Details";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var settings = IocHelper.Container.Resolve<Settings>();
            Content = new PerformanceDetailWindowControl { DataContext = settings.DetailWindowViewModel };
        }
    }
}
