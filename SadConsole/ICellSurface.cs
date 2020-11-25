using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial interface ICellSurface : SadRogue.Primitives.GridViews.IGridView<ColoredGlyph>, IEnumerable<ColoredGlyph>
    {
        /// <summary>
        /// An event that is raised when <see cref="IsDirty"/> changes.
        /// </summary>
        event EventHandler IsDirtyChanged;

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
        /// When true, the <see cref="ColoredString.Parse(string, int, ICellSurface, StringParser.ParseCommandStacks)"/> command is used to print strings.
        /// </summary>
        public bool UsePrintProcessor { get; set; }

        /// <summary>
        /// Processes the effects added to cells with <see cref="M:CellSurfaceEditor.SetEffect*"/>.
        /// </summary>
        public Effects.EffectsManager Effects { get; }

        /// <summary>
        /// Returns a rectangle that represents the maximum size of the surface.
        /// </summary>
        Rectangle Area { get; }

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
        /// Returns <see langword="true"/> when the <see cref="ICellSurface.View"/> width or height is different from <see cref="Height"/> or <see cref="Width"/>.
        /// </summary>
        bool IsScrollable { get; }

        /// <summary>
        /// The visible portion of the surface.
        /// </summary>
        Rectangle View { get; set; }

        /// <summary>
        /// Gets or sets the visible height of the surface in cells.
        /// </summary>
        int ViewHeight { get; set; }

        /// <summary>
        /// The position of the view within the buffer.
        /// </summary>
        Point ViewPosition { get; set; }

        /// <summary>
        /// Gets or sets the visible width of the surface in cells.
        /// </summary>
        int ViewWidth { get; set; }

        /// <summary>
        /// Resizes the surface to the specified width and height.
        /// </summary>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="bufferWidth">The maximum width of the surface.</param>
        /// <param name="bufferHeight">The maximum height of the surface.</param>
        /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
        void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear);

        /// <summary>
        /// Returns a new surface instance from the current instance based on the <paramref name="view"/>.
        /// </summary>
        /// <param name="view">An area of the surface to create a view of.</param>
        /// <returns>A new surface</returns>
        ICellSurface GetSubSurface(Rectangle view);

        /// <summary>
        /// Remaps the cells of this surface to a view of the <paramref name="surface"/>.
        /// </summary>
        /// <param name="surface">The target surface to map cells from.</param>
        /// <param name="view">A view rectangle of the target surface.</param>
        void SetSurface(in ICellSurface surface, Rectangle view = default);

        /// <summary>
        /// Changes the cells of the surface to the provided array.
        /// </summary>
        /// <param name="cells">The cells to replace in this surface.</param>
        /// <param name="width">The viewable width of the surface.</param>
        /// <param name="height">The viewable height of the surface.</param>
        /// <param name="bufferWidth">The maximum width of the surface.</param>
        /// <param name="bufferHeight">The maximum height of the surface.</param>
        void SetSurface(in ColoredGlyph[] cells, int width, int height, int bufferWidth, int bufferHeight);
    }
}
