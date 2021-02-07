using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using SadConsole.Host.MonoGame;
using System.Collections.Generic;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a <see cref="IScreenSurface"/>.
    /// </summary>
    /// <remarks>
    /// This renderer caches the entire drawing of the surface's cells, including the tint of the object.
    /// </remarks>
    public class ScreenSurfaceRenderer : IRenderer
    {
        private Host.GameTexture _renderTexture;

        /// <summary>
        /// Quick access to backing texture.
        /// </summary>
        public RenderTarget2D _backingTexture;
        
        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public ITexture Output => _renderTexture;

        /// <summary>
        /// Color used with drawing the texture to the screen. Let's a surface become transparent.
        /// </summary>
        public Color _finalDrawColor = SadRogue.Primitives.Color.White.ToMonoColor();

        /// <summary>
        /// The blend state used by this renderer.
        /// </summary>
        public BlendState MonoGameBlendState { get; set; } = SadConsole.Host.Settings.MonoGameSurfaceBlendState;

        /// <summary>
        /// A 0 to 255 value represening how transparent the surface is when it's drawn to the screen. 255 represents full visibility.
        /// </summary>
        public byte Opacity
        {
            get => _finalDrawColor.A;
            set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
        }

        /// <inheritdoc/>
        public bool IsForced { get; set; }

        /// <summary>
        /// Cached set of rectangles used in rendering each cell.
        /// </summary>
        public XnaRectangle[] CachedRenderRects;

        ///  <inheritdoc/>
        public virtual void Refresh(IScreenSurface screen, bool force = false)
        {
            bool backingTextureChanged = false;
            IsForced = force;

            // Update texture if something is out of size.
            if (_backingTexture == null || screen.AbsoluteArea.Width != _backingTexture.Width || screen.AbsoluteArea.Height != _backingTexture.Height)
            {
                IsForced = true;
                backingTextureChanged = true;
                _backingTexture?.Dispose();
                _backingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _renderTexture?.Dispose();
                _renderTexture = new Host.GameTexture(_backingTexture);
            }
            
            // Update cached drawing rectangles if something is out of size.
            if (CachedRenderRects == null || CachedRenderRects.Length != screen.Surface.View.Width * screen.Surface.View.Height || CachedRenderRects[0].Width != screen.FontSize.X || CachedRenderRects[0].Height != screen.FontSize.Y)
            {
                CachedRenderRects = new XnaRectangle[screen.Surface.View.Width * screen.Surface.View.Height];

                for (int i = 0; i < CachedRenderRects.Length; i++)
                {
                    var position = SadRogue.Primitives.Point.FromIndex(i, screen.Surface.View.Width);
                    CachedRenderRects[i] = screen.Font.GetRenderRect(position.X, position.Y, screen.FontSize).ToMonoRectangle();
                }

                IsForced = true;
            }

            bool composeRequested = IsForced;

            // Let everything refresh before compose.
            foreach (IRenderStep step in screen.RenderSteps)
                composeRequested |= step.Refresh(this, screen, backingTextureChanged, IsForced);

            // If any step (or IsForced) requests a compose, process them.
            if (composeRequested)
            {
                // Setup spritebatch for compose
                Host.Global.GraphicsDevice.SetRenderTarget(_backingTexture);
                Host.Global.GraphicsDevice.Clear(Color.Transparent);
                Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                // Compose each step
                foreach (IRenderStep step in screen.RenderSteps)
                    step.Composing(this, screen);

                // End sprite batch
                Host.Global.SharedSpriteBatch.End();
                Host.Global.GraphicsDevice.SetRenderTarget(null);
            }
        }

        ///  <inheritdoc/>
        public virtual void Render(IScreenSurface screen)
        {
            foreach (IRenderStep step in screen.RenderSteps)
                step.Render(this, screen);
        }

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _backingTexture?.Dispose();
                _renderTexture?.Dispose();

                disposedValue = true;
            }
        }

        ~ScreenSurfaceRenderer() =>
            Dispose(false);

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
