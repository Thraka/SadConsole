using System;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// A surface that's rendered on top of a host surface.
/// </summary>
public class Overlay : UpdateComponent
{
    private Renderers.IRenderStep? RenderStep;

    /// <summary>
    /// A surface that's sized to match.
    /// </summary>
    public ScreenSurface Surface { get; private set; }

    /// <inheritdoc/>
    public override void OnAdded(IScreenObject host)
    {
        IScreenSurface hostObj = (IScreenSurface)host;

        if (RenderStep != null)
        {
            hostObj.Renderer?.Steps.Remove(RenderStep);
            RenderStep.Dispose();
            RenderStep = null;
        }

        Surface = new(hostObj.Surface.ViewWidth, hostObj.Surface.ViewHeight);
        Surface.Surface.DefaultBackground = Color.Transparent;
        Surface.Clear();
        MatchSurface(hostObj);

        RenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Surface);
        RenderStep.SetData(Surface);
        RenderStep.SortOrder = 100;

        hostObj.Renderer?.Steps.Add(RenderStep);
        hostObj.Renderer?.Steps.Sort(Renderers.RenderStepComparer.Instance);
    }

    /// <inheritdoc/>
    public override void OnRemoved(IScreenObject host)
    {
        if (RenderStep != null)
        {
            ((IScreenSurface)host).Renderer?.Steps.Remove(RenderStep);
            RenderStep.Dispose();
            RenderStep = null;
        }
    }

    /// <inheritdoc/>
    public override void Update(IScreenObject host, TimeSpan delta) =>
        MatchSurface((IScreenSurface)host);

    private void MatchSurface(IScreenSurface host)
    {
        if (Surface.Width != host.Surface.ViewWidth || Surface.Height != host.Surface.ViewHeight)
            Surface.Resize(host.Surface.ViewWidth, host.Surface.ViewHeight, false);
        if (Surface.Font != host.Font)
            Surface.Font = host.Font;
        if (Surface.FontSize != host.FontSize)
            Surface.FontSize = host.FontSize;
    }
}
