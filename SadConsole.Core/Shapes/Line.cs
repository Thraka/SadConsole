using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Cell StartingCellAppearance { get; set; }

        /// <summary>
        /// Gets or sets the appearance of the ending cell in the line.
        /// </summary>
        public Cell EndingCellAppearance { get; set; }

        /// <summary>
        /// Gets or sets the appearance of a cell in the line.
        /// </summary>
        public Cell CellAppearance { get; set; }

        /// <summary>
        /// When true, uses the <see cref="StartingCellAppearance"/> for the starting cell instead of <see cref="CellAppearance"/>.
        /// </summary>
        public bool UseStartingCell { get; set; }

        /// <summary>
        /// When true, uses the <see cref="EndingCellAppearance"/> for the starting cell instead of <see cref="CellAppearance"/>.
        /// </summary>
        public bool UseEndingCell { get; set; }

        /// <summary>
        /// Creates a new instance of the line class with default values.
        /// </summary>
        public Line()
        {
            var colors = new CellAppearance(Color.White, Color.Black);
            StartingCellAppearance = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                CharacterIndex = 195
            };
            EndingCellAppearance = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                CharacterIndex = 180
            };
            CellAppearance = new Cell()
            {
                Background = colors.Background,
                Foreground = colors.Foreground,
                CharacterIndex = 196
            };

            UseStartingCell = true;
            UseEndingCell = true;
        }

        /// <summary>
        /// Draws the line shape.
        /// </summary>
        /// <param name="surface">The cell surface to draw on.</param>
        public void Draw(TextSurface surface)
        {
            List<Cell> cells = new List<Cell>();

            Algorithms.Line(StartingLocation.X, StartingLocation.Y, EndingLocation.X, EndingLocation.Y, (x, y) => { cells.Add(surface[x, y]); return true; });

            if (cells.Count > 1)
            {
                if (UseStartingCell)
                {
                    StartingCellAppearance.Copy(cells[0]);
                    cells[0].Effect = StartingCellAppearance.Effect;
                }
                else
                {
                    CellAppearance.Copy(cells[0]);
                    cells[0].Effect = StartingCellAppearance.Effect;
                }

                if (UseEndingCell)
                {
                    EndingCellAppearance.Copy(cells[cells.Count - 1]);
                    cells[cells.Count - 1].Effect = EndingCellAppearance.Effect;
                }
                else
                {
                    CellAppearance.Copy(cells[cells.Count - 1]);
                    cells[cells.Count - 1].Effect = CellAppearance.Effect;
                }

                for (int i = 1; i < cells.Count - 1; i++)
                {
                    CellAppearance.Copy(cells[i]);
                    cells[i].Effect = CellAppearance.Effect;
                }
            }
            else if (cells.Count == 1)
            {
                CellAppearance.Copy(cells[0]);
                cells[0].Effect = CellAppearance.Effect;
            }
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
                    StartingCellAppearance.Copy(newCells[0]);
                else
                    CellAppearance.Copy(newCells[0]);

                if (UseEndingCell)
                    EndingCellAppearance.Copy(newCells[newCells.Count - 1]);
                else
                    CellAppearance.Copy(newCells[newCells.Count - 1]);

                for (int i = 1; i < newCells.Count - 1; i++)
                {
                    CellAppearance.Copy(newCells[i]);
                }
            }
            else if (newCells.Count == 1)
                CellAppearance.Copy(newCells[0]);
        }

        /// <summary>
        /// Determines the cells that would be drawn on and returns them instead of drawing the line.
        /// </summary>
        /// <param name="surface">The surface to get the cells from.</param>
        /// <returns>The cells the line would have drawn on.</returns>
        public IEnumerable<Cell> GetCells(TextSurface surface)
        {
            List<Cell> cells = new List<Cell>();

            Algorithms.Line(StartingLocation.X, StartingLocation.Y, EndingLocation.X, EndingLocation.Y, (x, y) => { cells.Add(surface[x, y]); return true; });

            return cells;
        }
    }
}
