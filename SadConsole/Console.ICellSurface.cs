using System;
using System.Collections;
using System.Collections.Generic;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole;

public partial class Console : ICellSurface
{
    /// <inheritdoc/>
    public ColoredGlyph this[int index] => Surface[index];

    /// <inheritdoc/>
    public ColoredGlyph this[int x, int y] => Surface[x, y];

    /// <inheritdoc />
    public ColoredGlyph this[Point position] => Surface[position.ToIndex(Width)];

    /// <inheritdoc/>
    public int TimesShiftedDown { get => Surface.TimesShiftedDown; set => Surface.TimesShiftedDown = value; }
    /// <inheritdoc/>
    public int TimesShiftedRight { get => Surface.TimesShiftedRight; set => Surface.TimesShiftedRight = value; }
    /// <inheritdoc/>
    public int TimesShiftedLeft { get => Surface.TimesShiftedLeft; set => Surface.TimesShiftedLeft = value; }
    /// <inheritdoc/>
    public int TimesShiftedUp { get => Surface.TimesShiftedUp; set => Surface.TimesShiftedUp = value; }
    /// <inheritdoc/>
    public bool UsePrintProcessor { get => Surface.UsePrintProcessor; set => Surface.UsePrintProcessor = value; }

    /// <inheritdoc/>
    public EffectsManager Effects => Surface.Effects;

    /// <inheritdoc/>
    public Rectangle Area => Surface.Area;

    /// <inheritdoc/>
    public int Height => Surface.Height;

    /// <inheritdoc/>
    public int Width => Surface.Width;

    /// <inheritdoc/>
    public Color DefaultBackground { get => Surface.DefaultBackground; set => Surface.DefaultBackground = value; }
    /// <inheritdoc/>
    public Color DefaultForeground { get => Surface.DefaultForeground; set => Surface.DefaultForeground = value; }
    /// <inheritdoc/>
    public int DefaultGlyph { get => Surface.DefaultGlyph; set => Surface.DefaultGlyph = value; }

    /// <inheritdoc/>
    public bool IsScrollable => Surface.IsScrollable;

    /// <inheritdoc/>
    public Rectangle View { get => Surface.View; set => Surface.View = value; }
    /// <inheritdoc/>
    public int ViewHeight { get => Surface.ViewHeight; set => Surface.ViewHeight = value; }
    /// <inheritdoc/>
    public Point ViewPosition { get => Surface.ViewPosition; set => Surface.ViewPosition = value; }
    /// <inheritdoc/>
    public int ViewWidth { get => Surface.ViewWidth; set => Surface.ViewWidth = value; }
    /// <inheritdoc/>
    public int Count => Surface.Count;

    /// <inheritdoc/>
    public event EventHandler IsDirtyChanged
    {
        add
        {
            Surface.IsDirtyChanged += value;
        }

        remove
        {
            Surface.IsDirtyChanged -= value;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<ColoredGlyph> GetEnumerator()
    {
        return Surface.GetEnumerator();
    }

    /// <summary>
    /// Resizes the surface to the specified width and height.
    /// </summary>
    /// <param name="width">The viewable width of the surface.</param>
    /// <param name="height">The viewable height of the surface.</param>
    /// <param name="bufferWidth">The maximum width of the surface.</param>
    /// <param name="bufferHeight">The maximum height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
    public void Resize(int width, int height, int bufferWidth, int bufferHeight, bool clear)
    {
        if (Surface is ICellSurfaceResize surface)
            surface.Resize(width, height, bufferWidth, bufferHeight, clear);
        else
            throw new Exception("Surface doesn't support resize.");
    }

    /// <summary>
    /// Resizes the surface and view to the specified width and height.
    /// </summary>
    /// <param name="width">The viewable width of the surface.</param>
    /// <param name="height">The viewable height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="DefaultForeground"/>, <see cref="DefaultBackground"/> and glyph 0.</param>
    public void Resize(int width, int height, bool clear)
    {
        if (Surface is ICellSurfaceResize surface)
            surface.Resize(width, height, clear);
        else
            throw new Exception("Surface doesn't support resize.");
    }

    IEnumerator IEnumerable.GetEnumerator() => Surface.GetEnumerator();
}
