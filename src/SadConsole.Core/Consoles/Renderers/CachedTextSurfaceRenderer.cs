#if SFML
using Point = SFML.System.Vector2i;
using Vector2 = SFML.System.Vector2f;
using SFML.System;
using Matrix = SFML.Graphics.Transform;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System;
using System.Runtime.Serialization;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Caches a text surface by rendering to a texture. That texture is then rendered at draw time. Reduces draw calls for a non-changing console.
    /// </summary>
    [DataContract]
    public class CachedTextSurfaceRenderer : ITextSurfaceRenderer, ITextSurfaceRendererUpdate
    {
#if SFML
        private RenderTexture renderedConsole;
#elif MONOGAME
        private RenderTarget2D renderedConsole;
#endif
        /// <summary>
        /// The sprite batch used for drawing to the screen.
        /// </summary>
        public SpriteBatch Batch { get; private set; }

        /// <summary>
        /// A method called when the <see cref="SpriteBatch"/> has been created and transformed, but before any text is drawn.
        /// </summary>
        public Action<SpriteBatch> BeforeRenderCallback { get; set; }

        /// <summary>
        /// A method called when all text has been drawn and any tinting has been applied.
        /// </summary>
        public Action<SpriteBatch> AfterRenderCallback { get; set; }

        /// <summary>
        /// Creates a new renderer.
        /// </summary>
        public CachedTextSurfaceRenderer(ITextSurfaceRendered source)
        {
#if SFML
            Batch = new SpriteBatch();
            renderedConsole = new RenderTexture((uint)source.AbsoluteArea.Width, (uint)source.AbsoluteArea.Height, false);
#elif MONOGAME
            Batch = new SpriteBatch(Engine.Device);
#endif
            Update(source);
        }

        /// <summary>
        /// Updates the cache based on the <paramref name="source"/> surface.
        /// </summary>
        /// <param name="source">The surface to render and cache.</param>
        public void Update(ITextSurfaceRendered source)
        {

#if SFML
            if (renderedConsole != null && renderedConsole.Size.X != (uint)source.AbsoluteArea.Width && renderedConsole.Size.Y != (uint)source.AbsoluteArea.Height)
            {
                renderedConsole.Dispose();
                renderedConsole = new RenderTexture((uint)source.AbsoluteArea.Width, (uint)source.AbsoluteArea.Height, false);
            }
            else if (renderedConsole == null)
            {
                renderedConsole = new RenderTexture((uint)source.AbsoluteArea.Width, (uint)source.AbsoluteArea.Height, false);
            }

            renderedConsole.Clear(Color.Transparent);
            TextSurfaceRenderer renderer = new TextSurfaceRenderer();
            //renderer.CallBatchEnd = false;
            //renderer.AfterRenderCallback = (batch) => batch.End(renderedConsole, RenderStates.Default);
            renderer.AlternativeRenderTarget = renderedConsole;
            renderer.Render(source, new Vector2i(0, 0));
            renderedConsole.Display();
#elif MONOGAME
            if (renderedConsole != null)
                renderedConsole.Dispose();

            renderedConsole = new RenderTarget2D(Engine.Device, source.AbsoluteArea.Width, source.AbsoluteArea.Height, false, Engine.Device.DisplayMode.Format, DepthFormat.Depth24);
            Engine.Device.SetRenderTarget(renderedConsole);
            Engine.Device.Clear(Color.Transparent);
            TextSurfaceRenderer renderer = new TextSurfaceRenderer();
            renderer.Render(source, new Point(0, 0));
            Engine.Device.SetRenderTarget(null);
#endif
        }

        /// <summary>
        /// Renders the cached surface from a previous call to the constructor or the <see cref="Update(ITextSurfaceRendered)"/> method.
        /// </summary>
        /// <param name="surface">Used only for tinting.</param>
        /// <param name="renderingMatrix">Display matrix for the rendered console.</param>
        public virtual void Render(ITextSurfaceRendered surface, Matrix renderingMatrix)
        {
#if SFML
            //Batch.Start(1, renderedConsole.Texture, renderingMatrix);
            Batch.Reset(Engine.Device, RenderStates.Default, renderingMatrix);
            BeforeRenderCallback?.Invoke(Batch);
            if (surface.Tint == Color.Transparent)
                Batch.DrawQuad(surface.AbsoluteArea, surface.AbsoluteArea, Color.White, renderedConsole.Texture);
            else
                Batch.DrawQuad(surface.AbsoluteArea, surface.AbsoluteArea, surface.Tint, renderedConsole.Texture);

            AfterRenderCallback?.Invoke(Batch);

            Batch.End();

#elif MONOGAME
            Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, renderingMatrix);

            BeforeRenderCallback?.Invoke(Batch);

            if (surface.Tint.A != 255)
            {
                Batch.Draw(renderedConsole, Vector2.Zero, Color.White);

                if (surface.Tint.A != 0)
                    Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
            else
                Batch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphIndexRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

            AfterRenderCallback?.Invoke(Batch);

            Batch.End();

#endif
        }

        /// <summary>
        /// Renders the cached surface from a previous call to the constructor or the <see cref="Update(ITextSurfaceRendered)"/> method.
        /// </summary>
        /// <param name="surface">Only used for tinting and calculation the position from the font.</param>
        /// <param name="position">Calculates the rendering position on the screen based on the size of the <paramref name="surface"/> parameter.</param>
        /// <param name="usePixelPositioning">Ignores the <paramref name="surface"/> font for positioning and instead treats the <paramref name="position"/> parameter in pixels.</param>
        public void Render(ITextSurfaceRendered surface, Point position, bool usePixelPositioning = false)
        {
            Matrix matrix;

            if (usePixelPositioning)
            {
                //if (oldAbsolutePosition != position)
                //{
                //    absolutePositionTransform = GetPositionTransform(position, surface.Font.Size, true);
                //    oldAbsolutePosition = position;
                //}

                //matrix = absolutePositionTransform;

                matrix = GetPositionTransform(position, surface.Font.Size, true);
            }
            else
            {
                //if (position != oldPosition)
                //{
                //    positionTransform = GetPositionTransform(position, surface.Font.Size, false);
                //    oldPosition = position;
                //}

                //matrix = positionTransform;

                matrix = GetPositionTransform(position, surface.Font.Size, false);
            }

            Render(surface, matrix);
        }

        /// <summary>
        /// Gets the Matrix transform that positions the console on the screen.
        /// </summary>
        /// <returns>The transform.</returns>
        public virtual Matrix GetPositionTransform(Point position, Point CellSize, bool absolutePositioning)
        {
            Point worldLocation;

            if (absolutePositioning)
                worldLocation = position;
            else
                worldLocation = position.ConsoleLocationToWorld(CellSize.X, CellSize.Y);

#if SFML
            var transform = Matrix.Identity;
            transform.Translate(worldLocation.X, worldLocation.Y);

            return transform;
#elif MONOGAME
            return Matrix.CreateTranslation(worldLocation.X, worldLocation.Y, 0f);
#endif
        }
    }
}
