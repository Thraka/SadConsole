using System;
using SadConsole.Entities;

namespace SadConsole.Components;

/// <summary>
/// Add to a <see cref="IScreenSurface"/> to have the <see cref="ICellSurface.ViewPosition"/> center on a specific object.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Follow Target")]
public class SurfaceComponentFollowTarget : UpdateComponent
{
    /// <summary>
    /// Target to have the surface follow.
    /// </summary>
    public IScreenObject Target { get; set; }

    /// <inheritdoc/>
    public override void OnAdded(IScreenObject host)
    {
        if (!(host is IScreenSurface))
            throw new ArgumentException($"{nameof(SurfaceComponentFollowTarget)} can only be added to an {nameof(IScreenSurface)}.");
    }

    /// <inheritdoc />
    public override void Update(IScreenObject hostObject, TimeSpan delta)
    {
        var host = (IScreenSurface)hostObject;

        if (Target != null)
        {
            if (Target is Entity ent)
                host.Surface.View = host.Surface.View.WithCenter(ent.UsePixelPositioning ? ent.AbsolutePosition / host.FontSize : ent.AbsolutePosition);
            else if (Target is IScreenSurface screenSurface)
                host.Surface.View = host.Surface.View.WithCenter(screenSurface.UsePixelPositioning ? screenSurface.Position / host.FontSize : host.Position);
            else
                host.Surface.View = host.Surface.View.WithCenter(Target.Position);
        }
    }
}
