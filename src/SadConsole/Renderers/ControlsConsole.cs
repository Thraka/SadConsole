#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System.Collections.Generic;
using System.Runtime.Serialization;
using SadConsole.Controls;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    [DataContract]
    public class ControlsConsole : Console
    {
        /// <summary>
        /// Controls to render.
        /// </summary>
        public List<Controls.ControlBase> Controls { get; set; } = new List<SadConsole.Controls.ControlBase>();

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        /// <param name="force">When <see langword="true"/> draws the surface even if <see cref="CellSurface.IsDirty"/> is <see langword="false"/>.</param>
        public override void Render(SadConsole.Console surface, bool force = false)
        {
            RenderBegin(surface, force);
            RenderCells(surface, force);

            foreach (ControlBase control in Controls)
            {
                RenderControlCells(control, surface.IsDirty || force);
            }

            //RenderControls(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        public void RenderControlCells(ControlBase control, bool draw = false)
        {
            if (!draw || !control.IsVisible)
            {
                return;
            }

            // This disabled code supported drawing alternative fonts IN the size they defined. This did not play
            // well with mouse handling and requires a much bigger investment to get that working.
            // Is it worth it? I'm not sure..

            //if (control.AlternateFont == null || control.AlternateFont.Size == control.Parent.Font.Size)
            {
                Font font = control.AlternateFont ?? control.Parent.Font;

                if (control.Surface.DefaultBackground.A != 0)
                {
                    (int x, int y) = (control.Position - control.Parent.ViewPort.Location).ConsoleLocationToPixel(font);
                    (int width, int height) = new Point(control.Surface.Width, control.Surface.Height) * font.Size;
                    Global.SpriteBatch.Draw(font.FontImage, new Rectangle(x, y, width, height), font.GlyphRects[font.SolidGlyphIndex], control.Surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
                }

                for (int i = 0; i < control.Surface.Cells.Length; i++)
                {
                    ref Cell cell = ref control.Surface.Cells[i];

                    if (!cell.IsVisible)
                    {
                        continue;
                    }

                    Point cellRenderPosition = i.ToPoint(control.Surface.Width) + control.Position;

                    if (!control.Parent.ViewPort.Contains(cellRenderPosition))
                    {
                        continue;
                    }

                    Rectangle renderRect = control.Parent.RenderRects[(cellRenderPosition - control.Parent.ViewPort.Location).ToIndex(control.Parent.Width)];

                    if (cell.Background != Color.Transparent && cell.Background != control.Surface.DefaultBackground)
                    {
                        Global.SpriteBatch.Draw(font.FontImage, renderRect, font.GlyphRects[font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                    }

                    if (cell.Foreground != Color.Transparent)
                    {
                        Global.SpriteBatch.Draw(font.FontImage, renderRect, font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);
                    }

                    foreach (CellDecorator decorator in cell.Decorators)
                    {
                        if (decorator.Color != Color.Transparent)
                        {
                            Global.SpriteBatch.Draw(font.FontImage, renderRect, font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
                        }
                    }
                }
            }
            //else
            //{
            //    var (x, y) = (control.Position - control.Parent.ViewPort.Location).ConsoleLocationToPixel(control.Parent.Font);
            //    if (control.Surface.DefaultBackground.A != 0)
            //    {
            //        var (width, height) = new Point(control.Surface.Width, control.Surface.Height) * control.AlternateFont.Size;
            //        Global.SpriteBatch.Draw(control.AlternateFont.FontImage, new Rectangle(x, y, width, height), control.AlternateFont.GlyphRects[control.AlternateFont.SolidGlyphIndex], control.Surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            //    }

            //    for (var i = 0; i < control.Surface.Cells.Length; i++)
            //    {
            //        ref var cell = ref control.Surface.Cells[i];

            //        if (!cell.IsVisible) continue;

            //        var (cellX, cellY) = i.ToPoint(control.Surface.Width).ConsoleLocationToPixel(control.AlternateFont);

            //        var renderRect = new Rectangle(x + cellX, y + cellY, control.AlternateFont.Size.X, control.AlternateFont.Size.Y);

            //        if (cell.Background != Color.Transparent && cell.Background != control.Surface.DefaultBackground)
            //            Global.SpriteBatch.Draw(control.AlternateFont.FontImage, renderRect, control.AlternateFont.GlyphRects[control.AlternateFont.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            //        if (cell.Foreground != Color.Transparent)
            //            Global.SpriteBatch.Draw(control.AlternateFont.FontImage, renderRect, control.AlternateFont.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);

            //        foreach (var decorator in cell.Decorators)
            //        {
            //            if (decorator.Color != Color.Transparent)
            //                Global.SpriteBatch.Draw(control.AlternateFont.FontImage, renderRect, control.AlternateFont.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
            //        }
            //    }
            //}
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
        }
    }
}
