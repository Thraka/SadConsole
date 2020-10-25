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
        protected IScreenSurface _screen;

        /// <summary>
        /// Color used with drawing the texture to the screen. Let's a surface become transparent.
        /// </summary>
        public Color _finalDrawColor = SadRogue.Primitives.Color.White.ToMonoColor();

        /// <summary>
        /// Render steps to process.
        /// </summary>
        protected List<IRenderStep> RenderSteps = new List<IRenderStep>();

        /// <summary>
        /// The blend state used by this renderer.
        /// </summary>
        public BlendState MonoGameBlendState { get; set; } = SadConsole.Host.Settings.MonoGameSurfaceBlendState;

        /// <summary>
        /// Name of this renderer type.
        /// </summary>
        public static string Name => "screenobject";

        /// <summary>
        /// A 0 to 255 value represening how transparent the <see cref="BackingTexture"/> is when drawn to the screen. 255 represents full visibility.
        /// </summary>
        public byte Opacity
        {
            get => _finalDrawColor.A;
            set => _finalDrawColor = new Color(_finalDrawColor.R, _finalDrawColor.G, _finalDrawColor.B, value);
        }

        /// <summary>
        /// The texture the surface is drawn to.
        /// </summary>
        public RenderTarget2D BackingTexture;

        /// <inheritdoc/>
        public bool IsForced { get; set; }

        /// <summary>
        /// Cached set of rectangles used in rendering each cell.
        /// </summary>
        public XnaRectangle[] CachedRenderRects;

        ///  <inheritdoc/>
        public virtual void Attach(IScreenSurface screen)
        {
            _screen = screen;

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].OnAdded(this, screen);
        }

        ///  <inheritdoc/>
        public virtual void Detatch(IScreenSurface screen)
        {
            BackingTexture?.Dispose();
            BackingTexture = null;

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].OnRemoved(this, screen);

            _screen = null;
        }

        ///  <inheritdoc/>
        public virtual void Render(IScreenSurface screen)
        {
            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RenderStart();

            // If the tint is covering the whole area, don't draw anything
            if (screen.Tint.A != 255)
            {
                // Draw call for surface
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(BackingTexture, new Vector2(screen.AbsoluteArea.Position.X, screen.AbsoluteArea.Position.Y), _finalDrawColor));
            }

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RenderBeforeTint();

            // If tint is visible, draw it
            if (screen.Tint.A != 0)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallColor(screen.Tint.ToMonoColor(), ((SadConsole.Host.GameTexture)screen.Font.Image).Texture, screen.AbsoluteArea.ToMonoRectangle(), screen.Font.SolidGlyphRectangle.ToMonoRectangle()));

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RenderEnd();
        }

        ///  <inheritdoc/>
        public virtual void Refresh(IScreenSurface screen, bool force = false)
        {
            IsForced = force;

            // Update texture if something is out of size.
            if (BackingTexture == null || screen.AbsoluteArea.Width != BackingTexture.Width || screen.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(Host.Global.GraphicsDevice, screen.AbsoluteArea.Width, screen.AbsoluteArea.Height, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                IsForced = true;
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

            for (int i = 0; i < RenderSteps.Count; i++)
                IsForced |= RenderSteps[i].RefreshPreStart();

            if (!IsForced && !screen.IsDirty) return;

            // Render parts of the surface
            RefreshBegin(screen);

            if (screen.Tint.A != 255)
                RefreshCells(screen.Surface, screen.Font);

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RefreshEnding();

            RefreshEnd(screen);

            for (int i = 0; i < RenderSteps.Count; i++)
                RenderSteps[i].RefreshEnd();

            screen.IsDirty = false;
        }

        /// <summary>
        /// Starts the sprite batch with the <see cref="BackingTexture"/>.
        /// </summary>
        /// <param name="surface">Object being used with the sprite batch.</param>
        protected virtual void RefreshBegin(IScreenSurface surface)
        {
            Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
            Host.Global.GraphicsDevice.Clear(Color.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
        }   

        /// <summary>
        /// Ends the sprite batch.
        /// </summary>
        /// <param name="surface">Object being used with the sprite batch.</param>
        protected virtual void RefreshEnd(IScreenSurface surface)
        {
            Host.Global.SharedSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws each cell with the sprite batch.
        /// </summary>
        /// <param name="surface">The surface being drawn.</param>
        /// <param name="font">The font used with drawing.</param>
        protected virtual void RefreshCells(ICellSurface surface, Font font)
        {
            var fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;

            if (surface.DefaultBackground.A != 0)
                Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            int rectIndex = 0;

            for (int y = 0; y < surface.View.Height; y++)
            {
                int i = ((y + surface.ViewPosition.Y) * surface.BufferWidth) + surface.ViewPosition.X;

                for (int x = 0; x < surface.View.Width; x++)
                {
                    ColoredGlyph cell = surface.Cells[i];

                    cell.IsDirty = false;

                    if (!cell.IsVisible) continue;

                    if (!cell.Background.Equals(Color.Transparent) && cell.Background != surface.DefaultBackground)
                        Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), cell.Background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                    if (!cell.Foreground.Equals(Color.Transparent))
                        Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), cell.Foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                    foreach (CellDecorator decorator in cell.Decorators)
                        if (!decorator.Color.Equals(Color.Transparent))
                            Host.Global.SharedSpriteBatch.Draw(fontImage, CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);

                    i++;
                    rectIndex++;
                }
            }
        }

        /// <inheritdoc/>
        public void AddRenderStep(IRenderStep step)
        {
            RenderSteps.Add(step);
            RenderSteps.Sort(CompareStep);

            step.OnAdded(this, _screen);
        }

        /// <inheritdoc/>
        public void RemoveRenderStep(IRenderStep step)
        {
            RenderSteps.Remove(step);
            step.OnRemoved(this, _screen);
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IRenderStep> GetRenderSteps() =>
            RenderSteps.AsReadOnly();

        private static int CompareStep(IRenderStep left, IRenderStep right)
        {
            if (left.SortOrder > right.SortOrder)
                return 1;

            if (left.SortOrder < right.SortOrder)
                return -1;

            return 0;
        }

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    CachedRenderRects = null;

                BackingTexture?.Dispose();
                BackingTexture = null;

                disposedValue = true;
            }
        }

        ~ScreenSurfaceRenderer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
