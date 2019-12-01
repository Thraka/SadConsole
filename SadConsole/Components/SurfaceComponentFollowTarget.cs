using System;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to a <see cref="IScreenSurface"/> to have the <see cref="CellSurface.ViewPosition"/> center on a specific object.
    /// </summary>
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
                throw new Exception($"{nameof(SurfaceComponentEntityOffsets)} can only be added to an {nameof(IScreenSurface)}.");
        }

        /// <inheritdoc />
        public override void Update(IScreenObject hostObject)
        {
            var host = (IScreenSurface)hostObject;

            host.Surface.View = host.Surface.View.WithCenter(Target?.Position ?? Point.None);
        }
    }
}
