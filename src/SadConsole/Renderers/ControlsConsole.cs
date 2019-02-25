#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System;
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

            foreach (var control in Controls)
                RenderControlCells(control, surface.IsDirty || force);

            //RenderControls(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        public void RenderControlCells(ControlBase control, bool draw = false)
        {
            if (!draw || !control.IsVisible) return;
            //if (control.Surface.Tint.A == 255) return;

            if (control.Surface.DefaultBackground.A != 0)
            {
                var (x, y) = (control.Position - control.Parent.ViewPort.Location).ConsoleLocationToPixel(control.Parent.Font);
                var (width, height) = new Point(control.Surface.Width, control.Surface.Height) * control.Parent.Font.Size;
                Global.SpriteBatch.Draw(control.Parent.Font.FontImage, new Rectangle(x, y, width, height), control.Parent.Font.GlyphRects[control.Parent.Font.SolidGlyphIndex], control.Surface.DefaultBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);
            }

            for (var i = 0; i < control.Surface.Cells.Length; i++)
            {
                ref var cell = ref control.Surface.Cells[i];

                if (!cell.IsVisible) continue;

                var cellRenderPosition = i.ToPoint(control.Surface.Width) + control.Position;

                if (!control.Parent.ViewPort.Contains(cellRenderPosition)) continue;

                Rectangle renderRect = control.Parent.RenderRects[(cellRenderPosition - control.Parent.ViewPort.Location).ToIndex(control.Parent.Width)];

                if (cell.Background != Color.Transparent && cell.Background != control.Surface.DefaultBackground)
                    Global.SpriteBatch.Draw(control.Parent.Font.FontImage, renderRect, control.Parent.Font.GlyphRects[control.Parent.Font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

                if (cell.Foreground != Color.Transparent)
                    Global.SpriteBatch.Draw(control.Parent.Font.FontImage, renderRect, control.Parent.Font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);

                foreach (var decorator in cell.Decorators)
                {
                    if (decorator.Color != Color.Transparent)
                        Global.SpriteBatch.Draw(control.Parent.Font.FontImage, renderRect, control.Parent.Font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
                }
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
        }
    }
}
