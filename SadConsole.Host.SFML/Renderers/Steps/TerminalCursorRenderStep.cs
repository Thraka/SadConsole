using System;
using SadRogue.Primitives;
using SFML.Graphics;

namespace SadConsole.Renderers;

/// <summary>
/// Renders a terminal cursor.
/// </summary>
[System.Diagnostics.DebuggerDisplay("TerminalCursor")]
public class TerminalCursorRenderStep : IRenderStep
{
    private Terminal.TerminalCursor? _cursor;
    private TimeSpan _blinkTimer = TimeSpan.Zero;
    private bool _isVisible = true;
    private readonly TimeSpan _blinkSpeed = TimeSpan.FromSeconds(0.35);

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.TerminalCursor;

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.TerminalCursor;

    /// <summary>
    /// Sets the <see cref="Terminal.TerminalCursor"/> to render.
    /// </summary>
    /// <param name="data">A <see cref="Terminal.TerminalCursor"/> instance.</param>
    public void SetData(object data)
    {
        if (data is Terminal.TerminalCursor cursor)
            _cursor = cursor;
        else
            _cursor = null;
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        _blinkTimer = TimeSpan.Zero;
        _isVisible = true;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        if (_cursor != null && _cursor.RenderCellActiveState.IsDirty)
            return true;

        return false;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        // Update blink timer
        _blinkTimer += GameHost.Instance.GameRunningTotalTime;
        if (_blinkTimer >= _blinkSpeed)
        {
            _isVisible = !_isVisible;
            _blinkTimer = TimeSpan.Zero;
        }

        if (_cursor == null || !_cursor.IsVisible)
            return;

        if (!screenObject.Surface.IsValidCell(_cursor.Position.X, _cursor.Position.Y) || !screenObject.Surface.View.Contains(_cursor.Position))
            return;

        // Determine if cursor should be drawn (blinking vs steady)
        if ((int)_cursor.Shape % 2 == 1 && !_isVisible) // Odd values = blinking
            return;

        // Get the glyph to render based on cursor shape
        int glyph = GetGlyphForShape(_cursor.Shape);

        IntRect rect = screenObject.Font.GetRenderRect(_cursor.Position.X - screenObject.Surface.ViewPosition.X,
                                                               _cursor.Position.Y - screenObject.Surface.ViewPosition.Y,
                                                               screenObject.FontSize).ToIntRect();

        // Create a temporary cell with the cursor glyph
        var cursorCell = _cursor.RenderCellActiveState.Clone();
        cursorCell.Glyph = glyph;

        Host.Global.SharedSpriteBatch.DrawCell(cursorCell, rect,
                                               cursorCell.Background != SadRogue.Primitives.Color.Transparent
                                               && cursorCell.Background != screenObject.Surface.DefaultBackground,
                                               screenObject.Font);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();

    private int GetGlyphForShape(Terminal.CursorShape shape)
    {
        return shape switch
        {
            Terminal.CursorShape.BlinkingBlock => 219,  // █
            Terminal.CursorShape.SteadyBlock => 219,    // █
            Terminal.CursorShape.BlinkingUnderline => 95,  // _
            Terminal.CursorShape.SteadyUnderline => 95,    // _
            Terminal.CursorShape.BlinkingBar => 124,    // |
            Terminal.CursorShape.SteadyBar => 124,      // |
            _ => 219  // Default to block
        };
    }
}
