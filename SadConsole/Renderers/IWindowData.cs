using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.UI;

namespace SadConsole.Renderers;

/// <summary>
/// Provides the methods and properties used by the Window renderer.
/// </summary>
public interface IWindowData
{
    /// <summary>
    /// The status of whether or not the window is being shown in modal mode.
    /// </summary>
    bool IsModal { get; }

    /// <summary>
    /// Access to the controls used by the window.
    /// </summary>
    ControlHost Controls { get; }
}
