using System;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to a <see cref="IScreenSurface"/> to sync all child <see cref="Entity"/> object's visibility and position offsets.
    /// </summary>
    public class SurfaceComponentEntityOffsets : UpdateComponent
    {
        /// <summary>
        /// If set to true, controls the <see cref="IScreenObject.IsVisible"/> property of the attached object.
        /// </summary>
        public bool HandleIsVisible { get; set; } = true;

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

            Point viewPosition = new Point(-host.Surface.ViewPosition.X, -host.Surface.ViewPosition.Y);

            foreach (IScreenObject child in host.Children)
            {
                if (child is Entity entity)
                {
                    entity.PositionOffset = viewPosition.TranslateFont(host.FontSize, entity.Animation.FontSize);

                    if (HandleIsVisible)
                        host.IsVisible = host.AbsoluteArea.Contains(host.AbsolutePosition);
                }
            }
        }

        /// <inheritdoc />
        public override void OnRemoved(IScreenObject host)
        {
            foreach (IScreenObject child in host.Children)
            {
                if (child is Entity entity)
                {
                    if (HandleIsVisible)
                        entity.IsVisible = true;

                    entity.PositionOffset = (0, 0);
                }
            }
        }
    }
}
