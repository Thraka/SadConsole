using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Host.MonoGame;
using SadConsole.Renderers;
using SadRogue.Primitives;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using Color = SadRogue.Primitives.Color;

namespace FeatureDemo
{

    // Not working yet

    public class CellPalette : SadConsole.ColoredGlyph
    {
        public int ForegroundIndex { get; set; }
        public int BackgroundIndex { get; set; }

        public CellPalette(int foregroundIndex, int backgroundIndex, int glyph)
        {
            Glyph = glyph;
            ForegroundIndex = foregroundIndex;
            BackgroundIndex = backgroundIndex;
        }
    }

    public class PaletteSurface : CellSurface
    {
        private Palette palette;

        public Palette Palette
        {
            get => palette;
            set
            {
                palette = value;
                ValidateCells();
            }
        }


        public PaletteSurface(int width, int height, Palette palette) : base(width, height)
        {
            this.palette = palette;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new CellPalette(0, 1, 0);
            }
        }


        protected void ValidateCells()
        {
            foreach (CellPalette cell in Cells.Cast<CellPalette>())
            {
                if (cell.ForegroundIndex >= palette.Length)
                {
                    cell.ForegroundIndex = 0;
                }

                if (cell.BackgroundIndex >= palette.Length)
                {
                    cell.BackgroundIndex = 0;
                }
            }
        }
    }

    public class PaletteSurfaceRenderer : IRenderStep, IRenderStepTexture
    {

        private SadConsole.Host.GameTexture _cachedTexture;

        /// <summary>
        /// The cached texture of the drawn surface.
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.RenderTarget2D BackingTexture { get; private set; }

        /// <inheritdoc/>//
        public ITexture CachedTexture => _cachedTexture;

        /// <inheritdoc/>
        public uint SortOrder { get; set; } = SadConsole.Renderers.Constants.RenderStepSortValues.Surface;

        /// <summary>
        /// Not used.
        /// </summary>
        public void SetData(object data) { }

        ///  <inheritdoc/>
        public void Reset()
        {
            BackingTexture?.Dispose();
            BackingTexture = null;
            _cachedTexture?.Dispose();
            _cachedTexture = null;
        }

        public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
        {
            bool result = true;

            // Update texture if something is out of size.
            if (backingTextureChanged || BackingTexture == null || screenObject.AbsoluteArea.Width != BackingTexture.Width || screenObject.AbsoluteArea.Height != BackingTexture.Height)
            {
                BackingTexture?.Dispose();
                BackingTexture = new RenderTarget2D(SadConsole.Host.Global.GraphicsDevice, screenObject.AbsoluteArea.Width, screenObject.AbsoluteArea.Height, false, SadConsole.Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                _cachedTexture?.Dispose();
                _cachedTexture = new SadConsole.Host.GameTexture(BackingTexture);
                result = true;
            }

            var monoRenderer = (ScreenSurfaceRenderer)renderer;

            // Redraw is needed
            if (result || screenObject.IsDirty || isForced)
            {
                SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
                SadConsole.Host.Global.GraphicsDevice.Clear(XnaColor.Transparent);
                SadConsole.Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, monoRenderer.MonoGameBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                IFont font = screenObject.Font;
                Texture2D fontImage = ((SadConsole.Host.GameTexture)font.Image).Texture;
                PaletteSurface surface = (PaletteSurface)screenObject.Surface;
                CellPalette cell;
                Color background;
                Color foreground;

                if (screenObject.Surface.DefaultBackground.A != 0)
                    SadConsole.Host.Global.SharedSpriteBatch.Draw(fontImage, new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), font.SolidGlyphRectangle.ToMonoRectangle(), screenObject.Surface.DefaultBackground.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                int rectIndex = 0;

                for (int y = 0; y < screenObject.Surface.View.Height; y++)
                {
                    int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) + screenObject.Surface.ViewPosition.X;

                    for (int x = 0; x < screenObject.Surface.View.Width; x++)
                    {
                        cell = (CellPalette)surface[i];
                        cell.IsDirty = false;

                        if (cell.IsVisible)
                        {
                            background = surface.Palette[cell.BackgroundIndex];
                            foreground = surface.Palette[cell.ForegroundIndex];

                            if (background != SadRogue.Primitives.Color.Transparent && background != screenObject.Surface.DefaultBackground)
                                SadConsole.Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.SolidGlyphRectangle.ToMonoRectangle(), background.ToMonoColor(), 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (foreground != SadRogue.Primitives.Color.Transparent && foreground != background)
                                SadConsole.Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(), foreground.ToMonoColor(), 0f, Vector2.Zero, cell.Mirror.ToMonoGame(), 0.4f);

                            foreach (CellDecorator decorator in cell.Decorators)
                                if (decorator.Color != SadRogue.Primitives.Color.Transparent)
                                    SadConsole.Host.Global.SharedSpriteBatch.Draw(fontImage, monoRenderer.CachedRenderRects[rectIndex], font.GetGlyphSourceRectangle(decorator.Glyph).ToMonoRectangle(), decorator.Color.ToMonoColor(), 0f, Vector2.Zero, decorator.Mirror.ToMonoGame(), 0.5f);
                        }

                        i++;
                        rectIndex++;
                    }
                }

                SadConsole.Host.Global.SharedSpriteBatch.End();
                SadConsole.Host.Global.GraphicsDevice.SetRenderTarget(null);

                result = true;
                screenObject.IsDirty = false;
            }

            return result;
        }

        ///  <inheritdoc/>
        public void Composing(IRenderer renderer, IScreenSurface screenObject)
        {
            SadConsole.Host.Global.SharedSpriteBatch.Draw(BackingTexture, Vector2.Zero, XnaColor.White);
        }

        ///  <inheritdoc/>
        public void Render(IRenderer renderer, IScreenSurface screenObject) { }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to indicate this method was called from <see cref="Dispose()"/>.</param>
        protected void Dispose(bool disposing) =>
            Reset();

        ///  <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes the object for collection.
        /// </summary>
        ~PaletteSurfaceRenderer() =>
            Dispose(false);
    }
}
