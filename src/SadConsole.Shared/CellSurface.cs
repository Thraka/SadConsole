using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.DrawCalls;

namespace SadConsole
{
    public partial class CellSurface: IEnumerable<Cell>
    {
        private bool _isDirty;

        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> is set to true.
        /// </summary>
        public event EventHandler DirtyChanged;

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty == value) return;
                
                _isDirty = value;
                OnDirtyChanged();
            }
        }

        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        public Color DefaultForeground { get; set; } = Color.White;

        /// <summary>
        /// The default background for glyphs on this surface.
        /// </summary>
        public Color DefaultBackground { get; set; } = Color.Transparent;

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        public Cell[] Cells { get; protected set; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get => Cells[y * Width + x];
            protected set => Cells[y * Width + x] = value;
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get => Cells[index];
            protected set => Cells[index] = value;
        }
        
        public CellSurface(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[width * height];

            for (var i = 0; i < Cells.Length; i++)
                Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);

            Effects = new Effects.EffectsManager(this);
        }

        public CellSurface(int width, int height, Cell[] initialCells)
        {
            Width = width;
            Height = height;

            if (initialCells == null)
            {
                Cells = new Cell[width * height];

                for (var i = 0; i < Cells.Length; i++)
                    Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
            }
            else
            {
                if (initialCells.Length != Width * Height) throw new Exception("Width * Height does not match initialCells.Length");
                Cells = initialCells;
            }

            Effects = new Effects.EffectsManager(this);
        }

        /// <summary>
        /// Called when the <see cref="IsDirty"/> property changes.
        /// </summary>
        protected virtual void OnDirtyChanged() => DirtyChanged?.Invoke(this, EventArgs.Empty);

        protected virtual void OnCellsReset() { }

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        public IEnumerator<Cell> GetEnumerator() => ((IEnumerable<Cell>)Cells).GetEnumerator();

        /// <summary>
        /// Gets an enumerator for <see cref="Cells"/>.
        /// </summary>
        /// <returns>An enumerator for <see cref="Cells"/>.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Cells.GetEnumerator();
    }
}