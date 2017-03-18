using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Runtime.Serialization;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Caches a text surface by rendering to a texture. That texture is then rendered at draw time. Reduces draw calls for a non-changing console.
    /// </summary>
    [DataContract]
    public class SurfaceRenderer : ISurfaceRenderer
    {
        /// <summary>
        /// A method called when the <see cref="SpriteBatch"/> has been created and transformed, but before any text is drawn.
        /// </summary>
        public Action<SpriteBatch> BeforeRenderCallback { get; set; }

        /// <summary>
        /// A method called when all text characters have been drawn but before any tinting has been applied.
        /// </summary>
        public Action<SpriteBatch> BeforeRenderTintCallback { get; set; }

        /// <summary>
        /// A method called when all text has been drawn and any tinting has been applied.
        /// </summary>
        public Action<SpriteBatch> AfterRenderCallback { get; set; }

        /// <summary>
        /// Renders the cached surface from a previous call to the constructor or the <see cref="Update(ISurfaceRendered)"/> method.
        /// </summary>
        /// <param name="surface">Used only for tinting.</param>
        /// <param name="renderingMatrix">Display matrix for the rendered console.</param>
        public virtual void Render(Surfaces.ISurface surface, bool force = false)
        {
            RenderBegin(surface, force);
            RenderCells(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        public virtual void RenderBegin(Surfaces.ISurface surface, bool force = false)
        {
            if (surface.IsDirty || force)
            {
                Global.GraphicsDevice.SetRenderTarget(surface.LastRenderResult);
                Global.GraphicsDevice.Clear(Color.Transparent);

                Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

                BeforeRenderCallback?.Invoke(Global.SpriteBatch);
            }
        }

        public virtual void RenderEnd(Surfaces.ISurface surface, bool force = false)
        {
            if (surface.IsDirty || force)
            {
                AfterRenderCallback?.Invoke(Global.SpriteBatch);

                Global.SpriteBatch.End();

                surface.IsDirty = false;
            }
        }

        public virtual void RenderCells(Surfaces.ISurface surface, bool force = false)
        {
            if (surface.IsDirty || force)
            {
                if (surface.Tint.A != 255)
                {
                    Cell cell;

                    if (surface.DefaultBackground.A != 0)
                        Global.SpriteBatch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

                    for (int i = 0; i < surface.RenderCells.Length; i++)
                    {
                        cell = surface.RenderCells[i];

                        if (cell.IsVisible)
                        {
                            if (cell.Background != Color.Transparent && cell.Background != surface.DefaultBackground)
                                Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                            if (cell.Foreground != Color.Transparent)
                                Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);
                        }
                    }
                }
            }
        }

        public virtual void RenderTint(Surfaces.ISurface surface, bool force = false)
        {
            if (surface.IsDirty || force)
            {
                BeforeRenderTintCallback?.Invoke(Global.SpriteBatch);

                if (surface.Tint.A != 0)
                    Global.SpriteBatch.Draw(surface.Font.FontImage, surface.AbsoluteArea, surface.Font.GlyphRects[surface.Font.SolidGlyphIndex], surface.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }
        }
    }
}
