using System;

namespace SadConsole.Terminal;

/// <summary>
/// A lightweight <see cref="ITerminalHandler"/> that tracks cursor movement and scroll counts
/// without rendering to a surface. Use this to measure the total height needed for ANSI content
/// before allocating a surface of the correct size.
/// </summary>
/// <remarks>
/// <para>
/// Feed the same data through the <see cref="Measurer"/> first, then create a surface
/// of size <c>(Width, TotalHeight)</c> and feed the data through a <see cref="Writer"/>.
/// </para>
/// <para>
/// The measurer mirrors cursor movement, wrap, and scroll logic from the writer but
/// performs no cell rendering or color resolution.
/// </para>
/// </remarks>
public class Measurer : ITerminalHandler
{
    private int _cursorRow;
    private int _cursorColumn;
    private bool _pendingWrap;
    private int _scrollCount;
    private int _maxRow;

    /// <summary>Width of the virtual surface in columns.</summary>
    public int Width { get; }

    /// <summary>Height of the virtual surface in rows (the viewport).</summary>
    public int Height { get; }

    /// <summary>Current cursor row (0-based).</summary>
    public int CursorRow => _cursorRow;

    /// <summary>Current cursor column (0-based).</summary>
    public int CursorColumn => _cursorColumn;

    /// <summary>Number of times the content scrolled up past the bottom row.</summary>
    public int ScrollCount => _scrollCount;

    /// <summary>The maximum row index the cursor visited.</summary>
    public int MaxRow => _maxRow;

    /// <summary>
    /// Total height needed to display all content without scrolling.
    /// Equal to <see cref="Height"/> + <see cref="ScrollCount"/>.
    /// </summary>
    public int TotalHeight => Height + _scrollCount;

    /// <summary>Auto-wrap mode. Default: true.</summary>
    public bool AutoWrap { get; set; } = true;

    /// <summary>How LF is interpreted. Default: <see cref="LineFeedMode.Implicit"/> (LF = CR+LF).</summary>
    public LineFeedMode LineFeeds { get; set; } = LineFeedMode.Implicit;

    /// <summary>The parser wired to this measurer.</summary>
    public Parser Parser { get; }

    /// <summary>
    /// Creates a measurer with the given virtual surface dimensions.
    /// </summary>
    /// <param name="width">Width of the virtual surface in columns.</param>
    /// <param name="height">Height of the virtual surface in rows (viewport height).</param>
    public Measurer(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        Parser = new Parser(this);
    }

    /// <summary>Feeds raw bytes through the parser.</summary>
    public void Feed(ReadOnlySpan<byte> data) => Parser.Feed(data);

    /// <summary>Encodes a string as UTF-8 and feeds it through the parser.</summary>
    public void Feed(string text)
    {
        if (text is null) return;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        Parser.Feed(bytes.AsSpan());
    }

    /// <summary>Resets all measurement state to initial values.</summary>
    public void Reset()
    {
        _cursorRow = 0;
        _cursorColumn = 0;
        _pendingWrap = false;
        _scrollCount = 0;
        _maxRow = 0;
    }

    // ═══════════════════════════════════════════════════════════
    //  ITerminalHandler — cursor tracking only, no rendering
    // ═══════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public void OnPrint(char ch)
    {
        if (_pendingWrap)
        {
            if (AutoWrap)
            {
                _cursorColumn = 0;
                DoLineFeed();
            }

            _pendingWrap = false;
        }

        if (_cursorColumn < Width - 1)
        {
            _cursorColumn++;
        }
        else if (AutoWrap)
        {
            _pendingWrap = true;
        }

        TrackMaxRow();
    }

    /// <inheritdoc/>
    public void OnC0Control(byte controlCode)
    {
        _pendingWrap = false;

        switch (controlCode)
        {
            case 0x07: // BEL — ignore
                break;
            case 0x08: // BS
                if (_cursorColumn > 0) _cursorColumn--;
                break;
            case 0x09: // HT — advance to next 8-column tab stop
                _cursorColumn = Math.Min((_cursorColumn / 8 + 1) * 8, Width - 1);
                break;
            case 0x0A: // LF
                if (LineFeeds == LineFeedMode.Implicit)
                    _cursorColumn = 0;
                DoLineFeed();
                break;
            case 0x0D: // CR
                _cursorColumn = 0;
                break;
        }
    }

    /// <inheritdoc/>
    public void OnEscDispatch(byte intermediate, byte final)
    {
        if (intermediate != 0) return;

        switch ((char)final)
        {
            case 'c': // RIS — full reset
                Reset();
                break;
            case 'E': // NEL — newline (CR+LF)
                _cursorColumn = 0;
                DoLineFeed();
                break;
            case 'M': // RI — reverse index
                if (_cursorRow > 0) _cursorRow--;
                break;
        }
    }

    /// <inheritdoc/>
    public void OnCsiDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, byte? privatePrefix)
    {
        if (privatePrefix is not null) return;

        switch ((char)final)
        {
            case 'H': // CUP — cursor position
            case 'f': // HVP
            {
                int row = Param(parameters, 0, 1) - 1;
                int col = Param(parameters, 1, 1) - 1;
                _cursorRow = Math.Clamp(row, 0, Height - 1);
                _cursorColumn = Math.Clamp(col, 0, Width - 1);
                break;
            }
            case 'A': // CUU — cursor up
                _cursorRow = Math.Max(0, _cursorRow - Param(parameters, 0, 1));
                break;
            case 'B': // CUD — cursor down
                _cursorRow = Math.Min(Height - 1, _cursorRow + Param(parameters, 0, 1));
                break;
            case 'C': // CUF — cursor forward
                _cursorColumn = Math.Min(Width - 1, _cursorColumn + Param(parameters, 0, 1));
                break;
            case 'D': // CUB — cursor backward
                _cursorColumn = Math.Max(0, _cursorColumn - Param(parameters, 0, 1));
                break;
            case 'E': // CNL — cursor next line
                _cursorRow = Math.Min(Height - 1, _cursorRow + Param(parameters, 0, 1));
                _cursorColumn = 0;
                break;
            case 'F': // CPL — cursor previous line
                _cursorRow = Math.Max(0, _cursorRow - Param(parameters, 0, 1));
                _cursorColumn = 0;
                break;
            case 'G': // CHA — cursor horizontal absolute
                _cursorColumn = Math.Clamp(Param(parameters, 0, 1) - 1, 0, Width - 1);
                break;
            case 'S': // SU — scroll up (explicit)
                _scrollCount += Param(parameters, 0, 1);
                break;
            // SGR ('m'), ED ('J'), EL ('K'), save/restore ('s'/'u'), DSR ('n') — ignored for measurement
        }

        _pendingWrap = false;
        TrackMaxRow();
    }

    /// <inheritdoc/>
    public void OnOscDispatch(ReadOnlySpan<byte> payload) { }

    /// <inheritdoc/>
    public void OnDcsDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, ReadOnlySpan<byte> payload) { }

    // ═══════════════════════════════════════════════════════════
    //  Internal helpers
    // ═══════════════════════════════════════════════════════════

    private void DoLineFeed()
    {
        if (_cursorRow < Height - 1)
        {
            _cursorRow++;
        }
        else
        {
            // Would scroll — count it
            _scrollCount++;
        }

        TrackMaxRow();
    }

    private void TrackMaxRow()
    {
        if (_cursorRow > _maxRow)
            _maxRow = _cursorRow;
    }

    private static int Param(ReadOnlySpan<int> parameters, int index, int defaultValue)
    {
        if (index >= parameters.Length) return defaultValue;
        int v = parameters[index];
        return v == 0 ? defaultValue : v;
    }
}
