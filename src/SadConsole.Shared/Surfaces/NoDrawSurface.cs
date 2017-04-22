using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// A <see cref="BasicSurface"/> that does not create a backing texture for rendering. Do not try to render.
    /// </summary>
    public class NoDrawSurface : BasicSurface
    {
        /// <summary>
        /// Creates a new surface from an existing surface.
        /// </summary>
        /// <param name="surface">The original surface</param>
        /// <returns></returns>
        public NoDrawSurface FromSurface(ISurface surface)
        {
            return new NoDrawSurface(surface.Width, surface.Height, surface.Font, surface.RenderArea, surface.Cells);
        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        public NoDrawSurface(int width, int height) : this(width, height, Global.FontDefault, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public NoDrawSurface(int width, int height, Rectangle renderArea) : this(width, height, Global.FontDefault, renderArea, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public NoDrawSurface(int width, int height, Font font) : this(width, height, font, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public NoDrawSurface(int width, int height, Font font, Rectangle renderArea) : this(width, height, font, renderArea, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="renderArea">Initial value for the <see cref="RenderArea"/> view.</param>
        public NoDrawSurface(int width, int height, Font font, Rectangle renderArea, Cell[] initialCells)
        {
            this.area = renderArea;
            this.width = width;
            this.height = height;

            if (initialCells == null)
                InitializeCells();
            else if (initialCells.Length != width * height)
                throw new ArgumentOutOfRangeException("initialCells", "initialCells length must equal width * height");
            else
            {
                cells = new Cell[initialCells.Length];
                RenderCells = new Cell[initialCells.Length];
                initialCells.CopyTo(cells, 0);
                initialCells.CopyTo(RenderCells, 0);
            }

            Font = font;
        }

        protected override void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = font.GetRenderRect(x, y);
                    RenderCells[index] = cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            AbsoluteArea = new Rectangle(0, 0, area.Width * font.Size.X, area.Height * font.Size.Y);
        }

        /// <summary>
        /// Saves the <see cref="NoDrawSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save((SerializedTypes.BasicSurfaceSerialized)this, file);
        }

        /// <summary>
        /// Loads a <see cref="NoDrawSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static NoDrawSurface Load(string file)
        {
            return Serializer.Load<SerializedTypes.BasicSurfaceSerialized>(file);
        }
    }
}
