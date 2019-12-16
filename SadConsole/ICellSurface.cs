using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial interface ICellSurface
    {
        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> changes.
        /// </summary>
        event EventHandler IsDirtyChanged;

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        ColoredGlyph this[int index] { get; }

        /// <summary>
        /// Gets a cell based on its coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        ColoredGlyph this[int x, int y] { get; }

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface down.
        /// </summary>
        public int TimesShiftedDown { get; set; }

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface right.
        /// </summary>
        public int TimesShiftedRight { get; set; }

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface left.
        /// </summary>
        public int TimesShiftedLeft { get; set; }

        /// <summary>
        /// A variable that tracks how many times this editor shifted the surface up.
        /// </summary>
        public int TimesShiftedUp { get; set; }

        /// <summary>
        /// When true, the <see cref="ColoredString.Parse(string, int, ICellSurface, ParseCommandStacks)"/> command is used to print strings.
        /// </summary>
        public bool UsePrintProcessor { get; set; }

        /// <summary>
        /// Processes the effects added to cells with <see cref="o:SetEffect"/>.
        /// </summary>
        public Effects.EffectsManager Effects { get; }

        /// <summary>
        /// Returns a rectangle that represents the size of the buffer.
        /// </summary>
        Rectangle Buffer { get; }

        /// <summary>
        /// Height of the surface buffer.
        /// </summary>
        int BufferHeight { get; }

        /// <summary>
        /// Width of the surface buffer.
        /// </summary>
        int BufferWidth { get; }

        /// <summary>
        /// All cells of the surface.
        /// </summary>
        ColoredGlyph[] Cells { get; }

        /// <summary>
        /// The default background for glyphs on this surface.
        /// </summary>
        Color DefaultBackground { get; set; }

        /// <summary>
        /// The default foreground for glyphs on this surface.
        /// </summary>
        Color DefaultForeground { get; set; }

        /// <summary>
        /// The default glyph used in clearing and erasing.
        /// </summary>
        int DefaultGlyph { get; set; }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Returns <see langword="true"/> when the <see cref="ICellSurface.View"/> width or height is different from <see cref="BufferHeight"/> or <see cref="BufferWidth"/>.
        /// </summary>
        bool IsScrollable { get; }

        /// <summary>
        /// The view presented by the surface.
        /// </summary>
        Rectangle View { get; set; }

        /// <summary>
        /// Gets or sets the visible height of the surface in cells.
        /// </summary>
        int ViewHeight { get; set; }

        /// <summary>
        /// The position of the buffer.
        /// </summary>
        Point ViewPosition { get; set; }

        /// <summary>
        /// Gets or sets the visible width of the surface in cells.
        /// </summary>
        int ViewWidth { get; set; }

        IEnumerator<ColoredGlyph> GetEnumerator();
    }
}
