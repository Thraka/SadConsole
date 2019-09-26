#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Draws a text surface to the screen.
    /// </summary>
    public class LayeredConsole : Console
    {
        /// <summary>
        /// Controls to render.
        /// </summary>
        public List<CellSurfaceLayer> Layers { get; set; } = new List<CellSurfaceLayer>(0);

        /// <summary>
        /// Renders a surface to the screen.
        /// </summary>
        /// <param name="surface">The surface to render.</param>
        /// <param name="force">When <see langword="true"/> draws the surface even if <see cref="CellSurface.IsDirty"/> is <see langword="false"/>.</param>
        public override void Render(SadConsole.Console surface, bool force = false)
        {
            foreach (CellSurfaceLayer layer in Layers)
            {
                if (layer.IsDirty)
                {
                    force = true;
                    break;
                }
            }

            RenderBegin(surface, force);
            RenderCells(surface, force);

            foreach (CellSurfaceLayer layer in Layers)
            {
                if (layer.IsVisible)
                {
                    RenderLayer(layer, surface, surface.IsDirty || force);
                }
            }

            //RenderControls(surface, force);
            RenderTint(surface, force);
            RenderEnd(surface, force);
        }

        /// <summary>
        /// Renders a layer on top of a console.
        /// </summary>
        /// <param name="layer">The layer to render.</param>
        /// <param name="drawingHost">The console that will be drawn on.</param>
        /// <param name="draw">If <see langword="false"/>, skips rendering.</param>
        public void RenderLayer(CellSurfaceLayer layer, SadConsole.Console drawingHost, bool draw = true)
        {
            if (!draw)
            {
                return;
            }

            if (drawingHost.Tint.A != 255)
            {
                for (int i = 0; i < layer.Cells.Length; i++)
                {
                    ref Cell cell = ref layer.Cells[i];

                    if (!cell.IsVisible)
                    {
                        continue;
                    }

                    if (cell.Background != Color.Transparent)
                    {
                        Global.SpriteBatch.Draw(drawingHost.Font.FontImage, drawingHost.RenderRects[i], drawingHost.Font.GlyphRects[drawingHost.Font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
                    }

                    if (cell.Foreground != Color.Transparent)
                    {
                        Global.SpriteBatch.Draw(drawingHost.Font.FontImage, drawingHost.RenderRects[i], drawingHost.Font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.4f);
                    }

                    foreach (CellDecorator decorator in cell.Decorators)
                    {
                        if (decorator.Color != Color.Transparent)
                        {
                            Global.SpriteBatch.Draw(drawingHost.Font.FontImage, drawingHost.RenderRects[i], drawingHost.Font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
                        }
                    }
                }
            }
        }

        //public virtual void RenderControls(CellSurface surface, bool force = false)
        //{
        //    if ((surface.IsDirty || force) && surface.Tint.A != 255)
        //    {
        //        int cellCount;
        //        Rectangle rect;
        //        Point point;
        //        Controls.ControlBase control;
        //        Cell cell;
        //        Font font;
        //        CellDecorator decorator;

        //        // For each control
        //        for (int i = 0; i < Controls.Count; i++)
        //        {
        //            if (Controls[i].IsVisible)
        //            {
        //                control = Controls[i];
        //                cellCount = control.Cells.Length;

        //                font = control.AlternateFont ?? surface.Font;

        //                // Draw background of each cell for the control
        //                for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
        //                {
        //                    cell = control[cellIndex];

        //                    if (cell.IsVisible)
        //                    {
        //                        point = control.GetPointFromIndex(cellIndex);
        //                        point = new Point(point.X + control.Position.X, point.Y + control.Position.Y);

        //                        if (surface.ViewPort.Contains(point.X, point.Y))
        //                        {
        //                            point = new Point(point.X - surface.ViewPort.Left, point.Y - surface.ViewPort.Top);
        //                            rect = surface.RenderRects[surface.GetIndexFromPoint(point)];

        //                            if (cell.Background != Color.Transparent)
        //                                Global.SpriteBatch.Draw(font.FontImage, rect, font.GlyphRects[font.SolidGlyphIndex], cell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.23f);

        //                            if (cell.Foreground != Color.Transparent)
        //                                Global.SpriteBatch.Draw(font.FontImage, rect, font.GlyphRects[cell.Glyph], cell.Foreground, 0f, Vector2.Zero, cell.Mirror, 0.26f);

        //                            for (int d = 0; d < cell.Decorators.Length; d++)
        //                            {
        //                                decorator = cell.Decorators[d];

        //                                if (decorator.Color != Color.Transparent)
        //                                    Global.SpriteBatch.Draw(surface.Font.FontImage, surface.RenderRects[i], surface.Font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
        }
    }
}
