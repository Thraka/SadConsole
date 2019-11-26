using System;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SadConsole.Host;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="IScreenSurface"/> when it uses a <see cref="SadConsole.LayeredScreenSurface"/> type for the <see cref="IScreenSurface.Surface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class LayeredScreenObject : ScreenObjectRenderer
    {
        ///  <inheritdoc/>
        public override void Attach(IScreenSurface screen)
        {
            if (!(screen.Surface is SadConsole.LayeredScreenSurface))
                throw new Exception($"The {nameof(Renderers.LayeredScreenObject)} renderer must be added to a screen object that has a {nameof(SadConsole.LayeredScreenSurface)} for a surface.");
        }

        ///  <inheritdoc/>
        public override void Refresh(IScreenSurface screen, bool force = false)
        {
            if (!force && !screen.IsDirty && BackingTexture != null) return;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != (int)BackingTexture.Size.X || screen.AbsoluteArea.Height != (int)BackingTexture.Size.Y)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTexture((uint)screen.AbsoluteArea.Width, (uint)screen.AbsoluteArea.Height);
            }

            // Update cached drawing rectangles if something is out of size.
            if (_renderRects == null || _renderRects.Length != screen.Surface.ViewWidth * screen.Surface.ViewHeight || _renderRects[0].Width != screen.FontSize.X || _renderRects[0].Height != screen.FontSize.Y)
            {
                _renderRects = new IntRect[screen.Surface.ViewWidth * screen.Surface.ViewHeight];

                for (int i = 0; i < _renderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.ViewWidth);
                    _renderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToIntRect();
                }
            }

            // Rendering code from sadconsole
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
            {
                foreach (var layer in ((SadConsole.LayeredScreenSurface)screen).Layers)
                    base.RefreshCells(layer.Surface, screen.Font);
            }

            RefreshTint(screen);

            RefreshEnd(screen);

            screen.IsDirty = false;
        }
    }
}
