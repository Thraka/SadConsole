using System;
using System.Collections.Generic;

namespace SadConsole.Renderers;

/// <summary>
/// Draws a surface.
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>
    /// The name used to create the renderer from the host.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// The output texture created by the renderer.
    /// </summary>
    ITexture Output { get; }

    /// <summary>
    /// A 0 to 255 value represening how transparent the surface is when drawn to the screen. 255 represents full visibility.
    /// </summary>
    byte Opacity { get; set; }

    /// <summary>
    /// <see langword="true"/> when the renderer is being forced to be redrawn this frame; otherwise <see langword="false"/>.
    /// </summary>
    bool IsForced { get; set; }

    /// <summary>
    /// The render steps for the renderer.
    /// </summary>
    List<IRenderStep> Steps { get; set; }

    /// <summary>
    /// Refreshes a cached drawing state.
    /// </summary>
    /// <param name="surface">Target surface.</param>
    /// <param name="force">When <see langword="true"/>, indicates the refresh should happen even if a surface isn't dirty.</param>
    void Refresh(IScreenSurface surface, bool force = false);

    /// <summary>
    /// Creates a drawcall in the drawing pipeline.
    /// </summary>
    /// <param name="surface">Target surface.</param>
    void Render(IScreenSurface surface);

    /// <summary>
    /// Called when various states in the host change.
    /// </summary>
    /// <param name="host">The host that uses this component.</param>
    public void OnHostUpdated(IScreenObject host)
    {
        foreach (IRenderStep step in Steps)
            step.OnHostUpdated(host);
    }
}
