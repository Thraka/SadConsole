using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SadConsole.Surfaces;
using System.Collections.Generic;

namespace SadConsole.Shapes
{
    /// <summary>
    /// Draws a line.
    /// </summary>
    public class Line : IShape
    {
        /// <summary>
        /// Gets or sets the starting point of the line.
        /// </summary>
        public Point StartingLocation { get; set; }

        /// <summary>
        /// Gets or sets the ending point of the line.
        /// </summary>
        public Point EndingLocation{ get; set; }


        /// <summary>
        /// Gets or sets the appearance of the starting cell in the line.
        /// </summary>
        public Cell StartingCell { get; set; }

        /// <summary>
        /// Gets or sets the appearance of the ending cell in the line.
        /// </summary>
        public Cell EndingCell { get; set; }

        /// <summary>
        /// Gets or sets the appearance of a cell in the line.
        /// </summary>
        public Cell Cell { get; set; }

        /// <summary>
        /// When true, uses the <see cref="StartingCell"/> for the starting cell instead of <see cref="Cell"/>.
        /// </summary>
        public bool UseStartingCell { get; set; }

        /// <summary>
        /// When true, uses the <see cref="EndingCell"/> for the starting cell instead of <see cref="Cell"/>.
        /// </summary>
        public bool UseEndingCell { get; set; }

        /// <summary>
        /// Creates a new instance of the line class with default values.
        /// </summary>
        public Line()
        {
            var colors = new Cell(Color.White, Color.Black);
            StartingCell = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                Glyph = 195
            };
            EndingCell = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                Glyph = 180
            };
            Cell = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                Glyph = 196
            };

            UseStartingCell = true;
            UseEndingCell = true;
        }

        /// <summary>
        /// Draws the line shape.
        /// </summary>
        /// <param name="surface">The cell surface to draw on.</param>
        public void Draw(SurfaceEditor surface)
        {
            List<Cell> cells = new List<Cell>();

            Algorithms.Line(StartingLocation.X, StartingLocation.Y, EndingLocation.X, EndingLocation.Y, (x, y) => { if (surface.IsValidCell(x, y)) cells.Add(surface[x, y]); return true; });

            if (cells.Count > 1)
            {
                if (UseStartingCell)
                {
                    cells[0].CopyAppearanceFrom(StartingCell);
                }
                else
                {
                    cells[0].CopyAppearanceFrom(Cell);
                }

                if (UseEndingCell)
                {
                    cells[cells.Count - 1].CopyAppearanceFrom(EndingCell);
                }
                else
                {
                    cells[cells.Count - 1].CopyAppearanceFrom(Cell);
                }

                for (int i = 1; i < cells.Count - 1; i++)
                {
                    cells[i].CopyAppearanceFrom(Cell);
                }
            }
            else if (cells.Count == 1)
            {
                cells[0].CopyAppearanceFrom(Cell);
            }

            surface.TextSurface.IsDirty = true;
        }

        /// <summary>
        /// Draws the line shape across all of the cells. Will not draw the effect. Must be done outside of this method.
        /// </summary>
        /// <param name="cells">The cells to draw on.</param>
        public void Draw(IEnumerable<Cell> cells)
        {
            List<Cell> newCells = new List<Cell>(cells);

            if (newCells.Count > 1)
            {
                if (UseStartingCell)
                    newCells[0].CopyAppearanceFrom(StartingCell);
                else
                    newCells[0].CopyAppearanceFrom(Cell);

                if (UseEndingCell)
                    newCells[newCells.Count - 1].CopyAppearanceFrom(EndingCell);
                else
                    newCells[newCells.Count - 1].CopyAppearanceFrom(Cell);

                for (int i = 1; i < newCells.Count - 1; i++)
                {
                    newCells[i].CopyAppearanceFrom(Cell);
                }
            }
            else if (newCells.Count == 1)
                newCells[0].CopyAppearanceFrom(Cell);
        }

        /// <summary>
        /// Determines the cells that would be drawn on and returns them instead of drawing the line.
        /// </summary>
        /// <param name="surface">The surface to get the cells from.</param>
        /// <returns>The cells the line would have drawn on.</returns>
        public IEnumerable<Cell> GetCells(ISurface surface)
        {
            List<Cell> cells = new List<Cell>();

            Algorithms.Line(StartingLocation.X, StartingLocation.Y, EndingLocation.X, EndingLocation.Y, (x, y) => { cells.Add(surface[x, y]); return true; });

            return cells;
        }
    }
}
