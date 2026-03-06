using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.Terminal;

/// <summary>
/// Specifies how a terminal color value was set.
/// </summary>
public enum ColorMode
{
    /// <summary>Use the default foreground or background color.</summary>
    Default,
    /// <summary>Use a 256-color palette index.</summary>
    Palette,
    /// <summary>Use an explicit RGB truecolor value.</summary>
    TrueColor
}

/// <summary>
/// Tracks the mutable state of a terminal emulator session.
/// </summary>
public class State
{
    private readonly int _width;
    private int _height;
    private SavedCursorState? _savedCursor;
    private readonly SortedSet<int> _tabStops = new();

    // ── Cursor position ──

    /// <summary>Current cursor row (0-based).</summary>
    public int CursorRow { get; set; }

    /// <summary>Current cursor column (0-based).</summary>
    public int CursorColumn { get; set; }

    /// <summary>True when the cursor is at the right margin and the next printable character should wrap.</summary>
    public bool PendingWrap { get; set; }

    // ── SGR attributes ──

    /// <summary>Foreground color mode.</summary>
    public ColorMode ForegroundMode { get; set; }

    /// <summary>Foreground palette index (valid when <see cref="ForegroundMode"/> is <see cref="ColorMode.Palette"/>).</summary>
    public int ForegroundIndex { get; set; }

    /// <summary>Foreground RGB value (valid when <see cref="ForegroundMode"/> is <see cref="ColorMode.TrueColor"/>).</summary>
    public Color ForegroundRgb { get; set; }

    /// <summary>Background color mode.</summary>
    public ColorMode BackgroundMode { get; set; }

    /// <summary>Background palette index (valid when <see cref="BackgroundMode"/> is <see cref="ColorMode.Palette"/>).</summary>
    public int BackgroundIndex { get; set; }

    /// <summary>Background RGB value (valid when <see cref="BackgroundMode"/> is <see cref="ColorMode.TrueColor"/>).</summary>
    public Color BackgroundRgb { get; set; }

    /// <summary>Bold / increased intensity (SGR 1).</summary>
    public bool Bold { get; set; }

    /// <summary>Dim / faint (SGR 2).</summary>
    public bool Dim { get; set; }

    /// <summary>Italic (SGR 3).</summary>
    public bool Italic { get; set; }

    /// <summary>Underline (SGR 4).</summary>
    public bool Underline { get; set; }

    /// <summary>Blink (SGR 5).</summary>
    public bool Blink { get; set; }

    /// <summary>Reverse video (SGR 7).</summary>
    public bool Reverse { get; set; }

    /// <summary>Concealed / hidden (SGR 8).</summary>
    public bool Concealed { get; set; }

    /// <summary>Strikethrough (SGR 9).</summary>
    public bool Strikethrough { get; set; }

    // ── Default colors ──

    /// <summary>Default foreground color used when <see cref="ForegroundMode"/> is <see cref="ColorMode.Default"/>.</summary>
    public Color DefaultForeground { get; set; } = new Color(170, 170, 170);

    /// <summary>Default background color used when <see cref="BackgroundMode"/> is <see cref="ColorMode.Default"/>.</summary>
    public Color DefaultBackground { get; set; } = new Color(0, 0, 0);

    // ── Scroll region ──

    /// <summary>Top margin of the scroll region (0-based, inclusive).</summary>
    public int ScrollTop { get; set; }

    /// <summary>Bottom margin of the scroll region (0-based, inclusive).</summary>
    public int ScrollBottom { get; set; }

    // ── Mode flags ──

    /// <summary>Auto-wrap mode (DECAWM). Default on.</summary>
    public bool AutoWrap { get; set; } = true;

    /// <summary>Cursor visibility (DECTCEM). Default on.</summary>
    public bool CursorVisible { get; set; } = true;

    /// <summary>Origin mode (DECOM). Default off.</summary>
    public bool OriginMode { get; set; }

    /// <summary>Cursor key mode (DECCKM). Default off.</summary>
    public bool CursorKeyMode { get; set; }

    /// <summary>Screen reverse video mode (DECSCNM). Default off.</summary>
    public bool ScreenReverseVideo { get; set; }

    // ── Dimensions ──

    /// <summary>Surface width in columns.</summary>
    public int Width => _width;

    /// <summary>Surface height in rows.</summary>
    public int Height => _height;

    /// <summary>
    /// Creates a new terminal state for the given surface dimensions.
    /// </summary>
    public State(int width, int height)
    {
        _width = width;
        _height = height;
        ScrollBottom = height - 1;
        InitializeTabStops();
    }

    /// <summary>
    /// Resets all SGR attributes to their defaults.
    /// </summary>
    public void ResetSgr()
    {
        ForegroundMode = ColorMode.Default;
        ForegroundIndex = 0;
        ForegroundRgb = default;
        BackgroundMode = ColorMode.Default;
        BackgroundIndex = 0;
        BackgroundRgb = default;
        Bold = false;
        Dim = false;
        Italic = false;
        Underline = false;
        Blink = false;
        Reverse = false;
        Concealed = false;
        Strikethrough = false;
    }

    /// <summary>
    /// Resets all terminal state to power-on defaults.
    /// </summary>
    public void Reset()
    {
        CursorRow = 0;
        CursorColumn = 0;
        PendingWrap = false;
        ResetSgr();
        ScrollTop = 0;
        ScrollBottom = _height - 1;
        AutoWrap = true;
        CursorVisible = true;
        OriginMode = false;
        CursorKeyMode = false;
        ScreenReverseVideo = false;
        _savedCursor = null;
        InitializeTabStops();
    }

    /// <summary>
    /// Saves the current cursor state (DECSC).
    /// </summary>
    public void SaveCursor()
    {
        _savedCursor = new SavedCursorState
        {
            Row = CursorRow,
            Column = CursorColumn,
            ForegroundMode = ForegroundMode,
            ForegroundIndex = ForegroundIndex,
            ForegroundRgb = ForegroundRgb,
            BackgroundMode = BackgroundMode,
            BackgroundIndex = BackgroundIndex,
            BackgroundRgb = BackgroundRgb,
            Bold = Bold,
            Dim = Dim,
            Italic = Italic,
            Underline = Underline,
            Blink = Blink,
            Reverse = Reverse,
            Concealed = Concealed,
            Strikethrough = Strikethrough,
            OriginMode = OriginMode,
        };
    }

    /// <summary>
    /// Restores the previously saved cursor state (DECRC).
    /// </summary>
    public void RestoreCursor()
    {
        if (_savedCursor is not { } saved)
            return;

        CursorRow = saved.Row;
        CursorColumn = saved.Column;
        PendingWrap = false;
        ForegroundMode = saved.ForegroundMode;
        ForegroundIndex = saved.ForegroundIndex;
        ForegroundRgb = saved.ForegroundRgb;
        BackgroundMode = saved.BackgroundMode;
        BackgroundIndex = saved.BackgroundIndex;
        BackgroundRgb = saved.BackgroundRgb;
        Bold = saved.Bold;
        Dim = saved.Dim;
        Italic = saved.Italic;
        Underline = saved.Underline;
        Blink = saved.Blink;
        Reverse = saved.Reverse;
        Concealed = saved.Concealed;
        Strikethrough = saved.Strikethrough;
        OriginMode = saved.OriginMode;
    }

    /// <summary>
    /// Returns the next tab-stop column after the given column, or the last column.
    /// </summary>
    public int NextTabStop(int currentColumn)
    {
        foreach (int stop in _tabStops)
        {
            if (stop > currentColumn)
                return Math.Min(stop, _width - 1);
        }

        return _width - 1;
    }

    /// <summary>
    /// Returns the previous tab-stop column before the given column, or column 0.
    /// </summary>
    public int PreviousTabStop(int currentColumn)
    {
        int result = 0;

        foreach (int stop in _tabStops)
        {
            if (stop >= currentColumn)
                break;
            result = stop;
        }

        return result;
    }

    /// <summary>
    /// Sets a tab stop at the specified column (HTS).
    /// </summary>
    public void SetTabStop(int column)
    {
        if (column >= 0 && column < _width)
            _tabStops.Add(column);
    }

    /// <summary>
    /// Clears the tab stop at the specified column (TBC 0).
    /// </summary>
    public void ClearTabStop(int column) =>
        _tabStops.Remove(column);

    /// <summary>
    /// Clears all tab stops (TBC 3).
    /// </summary>
    public void ClearAllTabStops() =>
        _tabStops.Clear();

    /// <summary>
    /// Updates the height after the backing surface has been resized.
    /// If the scroll bottom was at the previous last row (no custom scroll region),
    /// it moves to the new last row.
    /// </summary>
    internal void UpdateHeight(int newHeight)
    {
        if (newHeight <= _height) return;

        if (ScrollBottom == _height - 1)
            ScrollBottom = newHeight - 1;

        _height = newHeight;
    }

    private void InitializeTabStops()
    {
        _tabStops.Clear();
        for (int col = 0; col < _width; col += 8)
            _tabStops.Add(col);
    }

    private struct SavedCursorState
    {
        public int Row;
        public int Column;
        public ColorMode ForegroundMode;
        public int ForegroundIndex;
        public Color ForegroundRgb;
        public ColorMode BackgroundMode;
        public int BackgroundIndex;
        public Color BackgroundRgb;
        public bool Bold;
        public bool Dim;
        public bool Italic;
        public bool Underline;
        public bool Blink;
        public bool Reverse;
        public bool Concealed;
        public bool Strikethrough;
        public bool OriginMode;
    }
}
