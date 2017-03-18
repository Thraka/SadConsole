using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    [DataContract]
    public class ControlsConsoleRenderer : SurfaceRenderer
    {
        /// <summary>
        /// Controls to render.
        /// </summary>
        public List<Controls.ControlBase> Controls { get; set; }

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        public override void Render(ISurface surface, bool force = false)
        {
            RenderBegin(surface, force);
            RenderCells(surface, force);
            RenderControls(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        public virtual void RenderControls(ISurface surface, bool force = false)
        {
            if ((surface.IsDirty || force) && surface.Tint.A != 255)
            {
                int cellCount;
                Rectangle rect;
                Point point;
                Controls.ControlBase control;
                Cell cell;
                Font font;
                // For each control
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].IsVisible)
                    {
                        control = Controls[i];
                        cellCount = control.TextSurface.Cells.Length;

                        font = control.AlternateFont == null ? surface.Font : control.AlternateFont;

                        // Draw background of each cell for the control
                        for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
                        {
                            cell = control[cellIndex];

                            if (cell.IsVisible)
                            {
                                point = BasicSurface.GetPointFromIndex(cellIndex, control.TextSurface.Width);
                                point = new Point(point.X + control.Position.X, point.Y + control.Position.Y);

                                if (surface.RenderArea.Contains(point.X, point.Y))
                                {
                                    point = new Point(point.X - surface.RenderArea.Left, point.Y - surface.RenderArea.Top);
                                    rect = surface.RenderRects[surface.GetIndexFromPoint(point)];

                                    if (cell.Background != Color.Transparent)
                                        Global.SpriteBatch.Draw(font.FontImage, rect, font.GlyphRects[font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.23f);

                                    if (cell.Foreground != Color.Transparent)
                                        Global.SpriteBatch.Draw(font.FontImage, rect, font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.26f);
                                }
                            }
                        }
                    }
                }
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
        }
    }
}
