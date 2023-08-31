using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
/// </summary>
public partial interface ICellSurface : SadRogue.Primitives.GridViews.IGridView<ColoredGlyphBase>, IEnumerable<ColoredGlyphBase>, ISurface
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
    /// When true, the <see cref="ColoredString.Parser"/> is used to generate a <see cref="ColoredString"/> before printing.
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
    /// Returns <see langword="true"/> when the <see cref="ICellSurface.View"/> width or height is different from <see cref="Area"/>; otherwise <see langword="false"/>.
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
}
