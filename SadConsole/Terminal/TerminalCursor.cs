using System;
using SadRogue.Primitives;

namespace SadConsole.Terminal;

/// <summary>
/// Lightweight terminal cursor data class for ANSI/VT rendering.
/// This is NOT an IComponent — it's a plain data object for the render step to read.
/// </summary>
public class TerminalCursor
{
    private readonly ColoredGlyphBase _renderCell;
    private TimeSpan _blinkTimer;
    private readonly TimeSpan _blinkSpeed = TimeSpan.FromSeconds(0.35);

    /// <summary>
    /// Cursor position on the surface.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Whether the cursor is in the visible phase of its blink cycle.
    /// Render steps should draw the cursor only when both <see cref="IsVisible"/> and <see cref="IsBlinkVisible"/> are <see langword="true"/>.
    /// Steady cursor shapes (even DECSCUSR values) always return <see langword="true"/>.
    /// </summary>
    public bool IsBlinkVisible { get; private set; } = true;

    /// <summary>
    /// Cursor visibility. Controlled by DECTCEM (CSI ? 25 h/l).
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Cursor shape. Controlled by DECSCUSR (CSI Ps SP q).
    /// </summary>
    public CursorShape Shape { get; set; } = CursorShape.BlinkingBlock;

    /// <summary>
    /// The rendered cell state exposed to the render step.
    /// Contains glyph, foreground, and background information.
    /// </summary>
    public ColoredGlyphBase RenderCellActiveState => _renderCell;

    /// <summary>
    /// Creates a new terminal cursor with default colors (white on black).
    /// </summary>
    public TerminalCursor() : this(Color.White, Color.Black)
    {
    }

    /// <summary>
    /// Creates a new terminal cursor with the specified foreground and background colors.
    /// </summary>
    /// <param name="foreground">Cursor foreground color.</param>
    /// <param name="background">Cursor background color.</param>
    public TerminalCursor(Color foreground, Color background)
    {
        _renderCell = new ColoredGlyph
        {
            Glyph = 219, // Block character (█)
            Foreground = foreground,
            Background = background
        };
    }

    /// <summary>
    /// Advances the blink timer by the frame delta and toggles <see cref="IsBlinkVisible"/> when the interval elapses.
    /// Steady cursor shapes never blink — <see cref="IsBlinkVisible"/> stays <see langword="true"/>.
    /// </summary>
    /// <param name="delta">Elapsed time since the last frame.</param>
    public void Update(TimeSpan delta)
    {
        if (Shape is CursorShape.SteadyBlock or CursorShape.SteadyUnderline or CursorShape.SteadyBar)
        {
            IsBlinkVisible = true;
            _blinkTimer = TimeSpan.Zero;
            return;
        }

        _blinkTimer += delta;

        if (_blinkTimer >= _blinkSpeed)
        {
            _blinkTimer -= _blinkSpeed;
            IsBlinkVisible = !IsBlinkVisible;
        }
    }

    /// <summary>
    /// Updates the cursor glyph based on the current shape.
    /// Block uses glyph 219 (█), underline uses 95 (_), bar uses 124 (|).
    /// </summary>
    public void UpdateGlyphForShape()
    {
        _renderCell.Glyph = Shape switch
        {
            CursorShape.BlinkingBlock or CursorShape.SteadyBlock => 219, // █
            CursorShape.BlinkingUnderline or CursorShape.SteadyUnderline => 95, // _
            CursorShape.BlinkingBar or CursorShape.SteadyBar => 124, // |
            _ => 219
        };
    }

    /// <summary>
    /// Sets the cursor foreground color.
    /// </summary>
    public void SetForeground(Color color) => _renderCell.Foreground = color;

    /// <summary>
    /// Sets the cursor background color.
    /// </summary>
    public void SetBackground(Color color) => _renderCell.Background = color;
}
