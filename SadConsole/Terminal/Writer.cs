using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Terminal;

/// <summary>
/// Determines how the writer maps incoming characters to font glyphs.
/// </summary>
public enum CharacterEncoding
{
    /// <summary>Characters are checked against the font first, then converted to CP437 if not found.</summary>
    Codepage437,
    /// <summary>Characters are passed through as-is (for Unicode-capable fonts).</summary>
    Unicode
}

/// <summary>
/// Controls how line feed (LF, 0x0A) is handled.
/// </summary>
public enum LineFeedMode
{
    /// <summary>LF moves cursor down only. CR must be explicit for column 0. (standard ANSI/VT behavior)</summary>
    Strict,
    /// <summary>LF implies CR+LF — moves cursor to column 0 AND down one line. (Linux terminal default, common for BBS art)</summary>
    Implicit
}

/// <summary>
/// Renders parsed terminal sequences onto an <see cref="ICellSurface"/>.
/// Implements <see cref="ITerminalHandler"/> and drives a <see cref="Parser"/>.
/// </summary>
public class Writer : ITerminalHandler
{
    private readonly ICellSurface _surface;
    private readonly IFont _font;
    private char _lastPrintedChar;
    private readonly Dictionary<int, IFont> _loadedFonts = new();

    /// <summary>
    /// A terminal cursor for visual display. Set to null for headless rendering.
    /// Controlled by DECTCEM (visibility) and DECSCUSR (shape).
    /// </summary>
    public TerminalCursor? Cursor { get; set; }

    /// <summary>
    /// The response channel for terminal queries (DA, DSR, DECRQM).
    /// When <see langword="null"/>, the terminal operates in silent data-stream mode
    /// and query responses are discarded. When set, responses are sent through this channel,
    /// enabling interactive terminal emulation.
    /// </summary>
    public ITerminalOutput? Output { get; set; }

    /// <summary>
    /// The terminal state (cursor position, SGR attributes, modes, scroll region).
    /// </summary>
    public State State { get; }

    /// <summary>
    /// The 256-color palette (plus truecolor).
    /// </summary>
    public Palette Palette { get; }

    /// <summary>
    /// The parser wired to this writer.
    /// </summary>
    public Parser Parser { get; }

    /// <summary>
    /// The character encoding mode. Determines how incoming characters are mapped to font glyphs.
    /// Default is <see cref="CharacterEncoding.Codepage437"/>.
    /// </summary>
    public CharacterEncoding Encoding { get; set; } = CharacterEncoding.Codepage437;

    /// <summary>
    /// How LF (0x0A) is interpreted. Default is <see cref="LineFeedMode.Implicit"/> (LF moves to column 0 and down).
    /// </summary>
    public LineFeedMode LineFeeds { get; set; } = LineFeedMode.Implicit;

    /// <summary>
    /// The terminal emulation behavior profile. Default is <see cref="TerminalMode.CTerm"/>.
    /// Changing this property adjusts C0 control-code rendering and CSI J cursor-home behavior
    /// without altering the underlying parser.
    /// </summary>
    public TerminalMode Mode { get; set; } = TerminalMode.CTerm;

    /// <summary>
    /// When <see langword="true"/> and the surface implements <see cref="ICellSurfaceResize"/>,
    /// the writer grows the surface instead of scrolling content up.
    /// The view size stays the same; only the total backing height increases.
    /// </summary>
    public bool AutoGrow { get; set; }

    /// <summary>
    /// Creates a new writer targeting the specified surface.
    /// </summary>
    public Writer(ICellSurface surface, IFont font)
    {
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _font = font ?? throw new ArgumentNullException(nameof(font));
        State = new State(surface.Width, surface.Height);
        Palette = new Palette();
        Parser = new Parser(this);
    }

    /// <summary>
    /// Feeds raw bytes through the parser.
    /// </summary>
    public void Feed(ReadOnlySpan<byte> data) => Parser.Feed(data);

    /// <summary>
    /// Encodes a string as UTF-8 and feeds it through the parser.
    /// </summary>
    public void Feed(string text)
    {
        if (text is null) return;

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        Parser.Feed(bytes.AsSpan());
    }

    // ═══════════════════════════════════════════════════════════
    //  ITerminalHandler
    // ═══════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public void OnPrint(char ch)
    {
        if (State.PendingWrap)
        {
            if (State.AutoWrap)
            {
                State.CursorColumn = 0;
                LineFeed();
            }

            State.PendingWrap = false;
        }

        var (fg, bg) = ResolveColors();

        ColoredGlyphBase cell = _surface[State.CursorColumn, State.CursorRow];

        cell.Glyph = ResolveGlyph(ch);
        cell.Foreground = fg;
        cell.Background = bg;
        ApplyDecorators(cell, fg);
        _surface.IsDirty = true;
        _lastPrintedChar = ch;

        if (State.CursorColumn < _surface.Width - 1)
        {
            State.CursorColumn++;
        }
        else if (State.AutoWrap)
        {
            State.PendingWrap = true;
        }

        SyncCursorPosition();
    }

    /// <inheritdoc/>
    public void OnC0Control(byte controlCode)
    {
        // HT needs special PendingWrap handling (same pattern as CUF/CHT):
        // forward tab from the right margin is a no-op without wrap resolution.
        if (controlCode == 0x09)
        {
            if (State.PendingWrap && State.AutoWrap)
            {
                State.CursorColumn = 0;
                LineFeed();
            }
            State.PendingWrap = false;
            State.CursorColumn = State.NextTabStop(State.CursorColumn);
            SyncCursorPosition();
            return;
        }

        State.PendingWrap = false;

        // ANSI-BBS mode: FF (0x0C) clears screen and homes cursor instead of rendering a glyph.
        if (Mode == TerminalMode.AnsiBbs && controlCode == 0x0C)
        {
            HandleEraseDisplay(2);
            State.CursorColumn = 0;
            State.CursorRow = 0;
            SyncCursorPosition();
            return;
        }

        // ANSI-BBS mode: render printable CP437 glyphs for C0 bytes that have IBM glyph assignments.
        if (Mode == TerminalMode.AnsiBbs && IsAnsiBbsGlyphByte(controlCode))
        {
            OnPrint((char)controlCode);
            return;
        }

        switch (controlCode)
        {
            case 0x07: // BEL — ignore
                break;
            case 0x08: // BS
                if (State.CursorColumn > 0)
                    State.CursorColumn--;
                break;
            case 0x0A: // LF
                if (LineFeeds == LineFeedMode.Implicit)
                    State.CursorColumn = 0;
                LineFeed();
                break;
            case 0x0D: // CR
                State.CursorColumn = 0;
                break;
        }

        SyncCursorPosition();
    }

    /// <inheritdoc/>
    public void OnEscDispatch(byte intermediate, byte final)
    {
        if (intermediate != 0) return;

        switch ((char)final)
        {
            case '7': // DECSC — save cursor
                State.SaveCursor();
                break;
            case '8': // DECRC — restore cursor
                State.RestoreCursor();
                SyncCursorPosition();
                break;
            case 'c': // RIS — full reset
                FullReset();
                break;
            case 'E': // NEL — newline (CR+LF)
                State.PendingWrap = false;
                State.CursorColumn = 0;
                LineFeed();
                SyncCursorPosition();
                break;
            case 'M': // RI — reverse index
                State.PendingWrap = false;
                ReverseIndex();
                SyncCursorPosition();
                break;
            case 'H': // HTS — set horizontal tab stop
                State.SetTabStop(State.CursorColumn);
                break;
        }
    }

    /// <inheritdoc/>
    public void OnCsiDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, byte? privatePrefix)
    {
        // NOTE: PendingWrap is NOT cleared here as a blanket epilogue. Each handler
        // that moves the cursor is responsible for clearing State.PendingWrap itself.
        // This is intentional per ECMA-48 — non-cursor-moving sequences (SGR, erase,
        // ECH, REP, etc.) must preserve pending-wrap state.

        if (privatePrefix == (byte)'?')
        {
            HandleDecPrivateMode(parameters, final);
            SyncCursorPosition();
            return;
        }

        if (privatePrefix == (byte)'=')
        {
            HandleCtermExtension(parameters, final);
            SyncCursorPosition();
            return;
        }

        // Ignore other unknown private prefixes
        if (privatePrefix is not null) return;

        switch ((char)final)
        {
            case 'm': // SGR — attribute change only, does NOT clear PendingWrap
                HandleSgr(parameters);
                break;
            case 'H': // CUP
            case 'f': // HVP
                State.PendingWrap = false;
                HandleCursorPosition(parameters);
                break;
            case 'A': // CUU
                State.PendingWrap = false;
                MoveCursorUp(Param(parameters, 0, 1));
                break;
            case 'B': // CUD
                State.PendingWrap = false;
                MoveCursorDown(Param(parameters, 0, 1));
                break;
            case 'C': // CUF
                if (State.PendingWrap && State.AutoWrap)
                {
                    // Resolve pending wrap: the cursor conceptually wrapped already,
                    // so advance to next line col 0 before applying the forward move.
                    // Without this, CUF from the right margin is a no-op (clamped),
                    // which breaks ANSI art that relies on immediate wrap semantics.
                    State.PendingWrap = false;
                    State.CursorColumn = 0;
                    LineFeed();
                }
                else
                {
                    State.PendingWrap = false;
                }
                MoveCursorForward(Param(parameters, 0, 1));
                break;
            case 'D': // CUB — or FNT if intermediate is SP (0x20)
                if (intermediates.Length == 1 && intermediates[0] == 0x20)
                {
                    // CSI Ps1 ; Ps2 SP D — Font Selection (FNT)
                    HandleFontSelection(parameters);
                }
                else
                {
                    State.PendingWrap = false;
                    MoveCursorBackward(Param(parameters, 0, 1));
                }
                break;
            case 'E': // CNL
                State.PendingWrap = false;
                MoveCursorDown(Param(parameters, 0, 1));
                State.CursorColumn = 0;
                break;
            case 'F': // CPL
                State.PendingWrap = false;
                MoveCursorUp(Param(parameters, 0, 1));
                State.CursorColumn = 0;
                break;
            case 'G': // CHA
                State.PendingWrap = false;
                State.CursorColumn = Math.Clamp(Param(parameters, 0, 1) - 1, 0, _surface.Width - 1);
                break;
            case 'd': // VPA — Line Position Absolute
                State.PendingWrap = false;
                {
                    int vpaRow = Param(parameters, 0, 1) - 1;
                    if (State.OriginMode)
                    {
                        vpaRow += State.ScrollTop;
                        State.CursorRow = Math.Clamp(vpaRow, State.ScrollTop, State.ScrollBottom);
                    }
                    else
                    {
                        State.CursorRow = Math.Clamp(vpaRow, 0, _surface.Height - 1);
                    }
                }
                break;
            case 'q': // DECSCUSR — Set Cursor Style (CSI Ps SP q)
                if (intermediates.Length == 1 && intermediates[0] == 0x20)
                {
                    HandleDecscusr(parameters);
                }
                break;
            case 'J': // ED — erase display, cursor stays put, does NOT clear PendingWrap
            {
                int edParam = Param(parameters, 0, 0);
                HandleEraseDisplay(edParam);
                // ANSI-BBS mode: ED 2 (erase entire display) also homes the cursor.
                if (Mode == TerminalMode.AnsiBbs && edParam == 2)
                {
                    State.PendingWrap = false;
                    State.CursorColumn = 0;
                    State.CursorRow = 0;
                    SyncCursorPosition();
                }
                break;
            }
            case 'K': // EL — erase in line, cursor stays put, does NOT clear PendingWrap
                HandleEraseInLine(Param(parameters, 0, 0));
                break;
            case 's': // Save Cursor Position (ANSI.SYS) — does NOT clear PendingWrap
                if (parameters.Length == 0)
                    State.SaveCursor();
                break;
            case 'u': // Restore cursor position — restores position (RestoreCursor clears PendingWrap)
                State.RestoreCursor();
                break;
            case 'n': // DSR — Device Status Report — does NOT clear PendingWrap
                HandleDeviceStatusReport(parameters);
                break;

            // Insert/Delete/Scroll
            case '@': // ICH — Insert Character
                State.PendingWrap = false;
                InsertCharacters(Param(parameters, 0, 1));
                break;
            case 'P': // DCH — Delete Character
                State.PendingWrap = false;
                DeleteCharacters(Param(parameters, 0, 1));
                break;
            case 'L': // IL — Insert Line
                State.PendingWrap = false;
                InsertLines(Param(parameters, 0, 1));
                break;
            case 'M': // DL — Delete Line
                State.PendingWrap = false;
                DeleteLines(Param(parameters, 0, 1));
                break;
            case 'S': // SU — Scroll Up — region scrolls, does NOT clear PendingWrap
                ScrollUp(Param(parameters, 0, 1));
                break;
            case 'T': // SD — Scroll Down — region scrolls, does NOT clear PendingWrap
                ScrollDown(Param(parameters, 0, 1));
                break;
            case 'X': // ECH — Erase Character — erase in place, does NOT clear PendingWrap
                EraseCharacters(Param(parameters, 0, 1));
                break;
            case 'b': // REP — Repeat last printed character — manages wrap itself via OnPrint
                RepeatLastCharacter(Param(parameters, 0, 1));
                break;

            // Tab Stop Commands
            case 'I': // CHT — Cursor Forward Tabulation
                if (State.PendingWrap && State.AutoWrap)
                {
                    // Resolve pending wrap: same pattern as CUF — forward tab from
                    // the right margin is a no-op without this (NextTabStop(width-1) = width-1).
                    State.PendingWrap = false;
                    State.CursorColumn = 0;
                    LineFeed();
                }
                else
                {
                    State.PendingWrap = false;
                }
                CursorForwardTab(Param(parameters, 0, 1));
                break;
            case 'Z': // CBT — Cursor Backward Tabulation
                State.PendingWrap = false;
                CursorBackwardTab(Param(parameters, 0, 1));
                break;
            case 'g': // TBC — Tab Clear — no cursor movement, does NOT clear PendingWrap
                HandleTabClear(Param(parameters, 0, 0));
                break;

            // DECSTBM (Set Top and Bottom Margins)
            case 'r': // DECSTBM — homes cursor on set
                State.PendingWrap = false;
                HandleSetScrollMargins(parameters);
                break;

            case 'c': // DA — Device Attributes — does NOT clear PendingWrap
                HandleDeviceAttributes(parameters);
                break;

            // TODO: CSI h/l — SM/RM (Set/Reset Mode) — standard (non-DEC-private) modes not yet implemented
            // TODO: CSI t — Window manipulation — intentionally not supported
            // TODO: CSI !p — DECSTR (Soft Terminal Reset) — requires intermediate byte handling
        }

        SyncCursorPosition();
    }

    /// <inheritdoc/>
    public void OnOscDispatch(ReadOnlySpan<byte> payload)
    {
        // Parse OSC command number (digits before first ';')
        int cmdEnd = 0;
        while (cmdEnd < payload.Length && payload[cmdEnd] != (byte)';')
            cmdEnd++;

        if (cmdEnd == 0 || cmdEnd >= payload.Length) return;

        int command = ParseDecimal(payload.Slice(0, cmdEnd));
        if (command < 0) return;

        ReadOnlySpan<byte> data = payload.Slice(cmdEnd + 1);

        switch (command)
        {
            case 4: // OSC 4 — Set palette color
                HandleOsc4(data);
                break;
            case 10: // OSC 10 — Set default foreground
                HandleOscDefaultColor(data, isForeground: true);
                break;
            case 11: // OSC 11 — Set default background
                HandleOscDefaultColor(data, isForeground: false);
                break;
        }
    }

    /// <inheritdoc/>
    public void OnDcsDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, ReadOnlySpan<byte> payload)
    {
        // DCS CTerm:Font:<slot>:<base64data> ST — Loadable font (CTLF)
        // The payload is the full string between DCS and ST.
        if (payload.Length == 0) return;

        string payloadStr = System.Text.Encoding.ASCII.GetString(payload);

        // Expected format: CTerm:Font:<slot>:<base64>
        const string prefix = "CTerm:Font:";
        if (!payloadStr.StartsWith(prefix, StringComparison.Ordinal))
            return;

        ReadOnlySpan<char> rest = payloadStr.AsSpan(prefix.Length);
        int colonIdx = rest.IndexOf(':');
        if (colonIdx < 0) return;

        if (!int.TryParse(rest.Slice(0, colonIdx), out int slot))
            return;

        // Per spec, DCS-loaded fonts use slots > 42
        if (slot < 0 || slot > 255) return;

        ReadOnlySpan<char> base64 = rest.Slice(colonIdx + 1);
        if (base64.Length == 0) return;

        try
        {
            byte[] fontBytes = Convert.FromBase64String(base64.ToString());
            using var stream = new System.IO.MemoryStream(fontBytes);
            IFont font = SadFont.ImportVGABiosFont($"CTerm-Font-{slot}", stream);
            _loadedFonts[slot] = font;
        }
        catch
        {
            // Invalid base64 or unsupported font size — silently ignore per terminal convention
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  Cursor helpers
    // ═══════════════════════════════════════════════════════════

    private void SyncCursorPosition()
    {
        if (Cursor is null) return;
        Cursor.Position = new Point(State.CursorColumn, State.CursorRow);
    }

    private void MoveCursorUp(int n) =>
        State.CursorRow = Math.Max(State.ScrollTop, State.CursorRow - n);

    private void MoveCursorDown(int n) =>
        State.CursorRow = Math.Min(State.ScrollBottom, State.CursorRow + n);

    private void MoveCursorForward(int n) =>
        State.CursorColumn = Math.Min(_surface.Width - 1, State.CursorColumn + n);

    private void MoveCursorBackward(int n) =>
        State.CursorColumn = Math.Max(0, State.CursorColumn - n);

    private void HandleCursorPosition(ReadOnlySpan<int> parameters)
    {
        int row = Param(parameters, 0, 1) - 1; // 1-based → 0-based
        int col = Param(parameters, 1, 1) - 1;

        if (State.OriginMode)
        {
            // Origin mode: row is relative to scroll region top, clamped within scroll region
            row += State.ScrollTop;
            State.CursorRow = Math.Clamp(row, State.ScrollTop, State.ScrollBottom);
        }
        else
        {
            State.CursorRow = Math.Clamp(row, 0, _surface.Height - 1);
        }

        State.CursorColumn = Math.Clamp(col, 0, _surface.Width - 1);
    }

    // ═══════════════════════════════════════════════════════════
    //  Scroll
    // ═══════════════════════════════════════════════════════════

    private void LineFeed()
    {
        if (State.CursorRow < State.ScrollBottom)
            State.CursorRow++;
        else if (State.CursorRow == State.ScrollBottom)
            ScrollUp(1);
    }

    private void ReverseIndex()
    {
        if (State.CursorRow > State.ScrollTop)
            State.CursorRow--;
        else if (State.CursorRow == State.ScrollTop)
            ScrollDown(1);
    }

    private void ScrollUp(int lines)
    {
        // Auto-grow: extend the surface instead of scrolling content
        if (AutoGrow && _surface is ICellSurfaceResize resizable)
        {
            int newTotalHeight = _surface.Height + lines;
            resizable.Resize(_surface.ViewWidth, _surface.ViewHeight, _surface.Width, newTotalHeight, false);
            State.UpdateHeight(newTotalHeight);
            State.CursorRow += lines;
            return;
        }

        int top = State.ScrollTop;
        int bottom = State.ScrollBottom;
        int width = _surface.Width;
        Color bg = ResolveBackground();

        for (int n = 0; n < lines; n++)
        {
            for (int row = top; row < bottom; row++)
                for (int col = 0; col < width; col++)
                    CopyCell(col, row + 1, col, row);

            ClearRow(bottom, bg);
        }

        _surface.IsDirty = true;
    }

    private void ScrollDown(int lines)
    {
        int top = State.ScrollTop;
        int bottom = State.ScrollBottom;
        int width = _surface.Width;
        Color bg = ResolveBackground();

        for (int n = 0; n < lines; n++)
        {
            for (int row = bottom; row > top; row--)
                for (int col = 0; col < width; col++)
                    CopyCell(col, row - 1, col, row);

            ClearRow(top, bg);
        }

        _surface.IsDirty = true;
    }

    // ═══════════════════════════════════════════════════════════
    //  Insert / Delete / Scroll operations
    // ═══════════════════════════════════════════════════════════

    private void InsertCharacters(int n)
    {
        int row = State.CursorRow;
        int col = State.CursorColumn;
        int width = _surface.Width;
        n = Math.Min(n, width - col);

        // Shift right: copy from right to left to avoid overwriting
        for (int c = width - 1; c >= col + n; c--)
            CopyCell(c - n, row, c, row);

        // Fill inserted positions with blanks
        Color bg = ResolveBackground();
        for (int c = col; c < col + n; c++)
            ClearCell(c, row, bg);

        _surface.IsDirty = true;
    }

    private void DeleteCharacters(int n)
    {
        int row = State.CursorRow;
        int col = State.CursorColumn;
        int width = _surface.Width;
        n = Math.Min(n, width - col);

        // Shift left
        for (int c = col; c < width - n; c++)
            CopyCell(c + n, row, c, row);

        // Fill vacated positions at end with blanks
        Color bg = ResolveBackground();
        for (int c = width - n; c < width; c++)
            ClearCell(c, row, bg);

        _surface.IsDirty = true;
    }

    private void InsertLines(int n)
    {
        int top = State.CursorRow;
        int bottom = State.ScrollBottom;

        // Only operates if cursor is within the scroll region
        if (top < State.ScrollTop || top > bottom)
            return;

        n = Math.Min(n, bottom - top + 1);
        Color bg = ResolveBackground();

        // Shift lines down within scroll region
        for (int i = 0; i < n; i++)
        {
            for (int row = bottom; row > top; row--)
                for (int col = 0; col < _surface.Width; col++)
                    CopyCell(col, row - 1, col, row);

            ClearRow(top, bg);
        }

        _surface.IsDirty = true;
    }

    private void DeleteLines(int n)
    {
        int top = State.CursorRow;
        int bottom = State.ScrollBottom;

        // Only operates if cursor is within the scroll region
        if (top < State.ScrollTop || top > bottom)
            return;

        n = Math.Min(n, bottom - top + 1);
        Color bg = ResolveBackground();

        // Shift lines up within scroll region
        for (int i = 0; i < n; i++)
        {
            for (int row = top; row < bottom; row++)
                for (int col = 0; col < _surface.Width; col++)
                    CopyCell(col, row + 1, col, row);

            ClearRow(bottom, bg);
        }

        _surface.IsDirty = true;
    }

    private void EraseCharacters(int n)
    {
        int row = State.CursorRow;
        int col = State.CursorColumn;
        int end = Math.Min(col + n, _surface.Width);
        Color bg = ResolveBackground();

        for (int c = col; c < end; c++)
            ClearCell(c, row, bg);

        _surface.IsDirty = true;
    }

    private void RepeatLastCharacter(int n)
    {
        if (_lastPrintedChar == '\0') return;

        for (int i = 0; i < n; i++)
            OnPrint(_lastPrintedChar);
    }

    // ═══════════════════════════════════════════════════════════
    //  Tab Stop Commands
    // ═══════════════════════════════════════════════════════════

    private void CursorForwardTab(int n)
    {
        for (int i = 0; i < n; i++)
            State.CursorColumn = State.NextTabStop(State.CursorColumn);
    }

    private void CursorBackwardTab(int n)
    {
        for (int i = 0; i < n; i++)
            State.CursorColumn = State.PreviousTabStop(State.CursorColumn);
    }

    private void HandleTabClear(int mode)
    {
        switch (mode)
        {
            case 0:
                State.ClearTabStop(State.CursorColumn);
                break;
            case 3:
                State.ClearAllTabStops();
                break;
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  DEC Private Modes + Scroll Margins
    // ═══════════════════════════════════════════════════════════

    private void HandleSetScrollMargins(ReadOnlySpan<int> parameters)
    {
        int top = Param(parameters, 0, 1) - 1;  // 1-based → 0-based, default=0
        int bottom = parameters.Length >= 2 && parameters[1] != 0
            ? parameters[1] - 1
            : _surface.Height - 1;

        top = Math.Clamp(top, 0, _surface.Height - 1);
        bottom = Math.Clamp(bottom, 0, _surface.Height - 1);

        if (top < bottom)
        {
            State.ScrollTop = top;
            State.ScrollBottom = bottom;
        }

        // After DECSTBM, cursor moves home
        if (State.OriginMode)
        {
            State.CursorRow = State.ScrollTop;
            State.CursorColumn = 0;
        }
        else
        {
            State.CursorRow = 0;
            State.CursorColumn = 0;
        }
    }

    private void HandleDecPrivateMode(ReadOnlySpan<int> parameters, byte final)
    {
        bool set = (char)final == 'h'; // 'h' = DECSET, 'l' = DECRST
        if ((char)final != 'h' && (char)final != 'l')
            return;

        for (int i = 0; i < parameters.Length; i++)
        {
            switch (parameters[i])
            {
                case 1: // DECCKM — Cursor Key Mode
                    State.CursorKeyMode = set;
                    break;
                case 5: // DECSCNM — Screen Reverse Video
                    State.ScreenReverseVideo = set;
                    break;
                case 6: // DECOM — Origin Mode — homes cursor, so clear PendingWrap
                    State.OriginMode = set;
                    State.PendingWrap = false;
                    // Setting/resetting origin mode moves cursor to home
                    if (set)
                    {
                        State.CursorRow = State.ScrollTop;
                        State.CursorColumn = 0;
                    }
                    else
                    {
                        State.CursorRow = 0;
                        State.CursorColumn = 0;
                    }
                    break;
                case 7: // DECAWM — Auto-Wrap Mode
                    State.AutoWrap = set;
                    break;
                case 25: // DECTCEM — Cursor Visibility
                    State.CursorVisible = set;
                    if (Cursor is not null)
                        Cursor.IsVisible = set;
                    break;
                case 31: // Bright alt character set — use font slot 1 for SGR 1
                    State.BrightFontEnabled = set;
                    break;
                case 32: // Bright intensity disable — suppress bright color when using font slot
                    State.BrightIntensityDisabled = set;
                    break;
                case 34: // Blink alt character set — use font slot 2 for SGR 5/6
                    State.BlinkFontEnabled = set;
                    break;
                case 35: // Blink disable — suppress blink animation when using font slot
                    State.BlinkDisabled = set;
                    break;
            }
        }
    }

    /// <summary>
    /// Handles CSI Ps SP q — DECSCUSR (Set Cursor Style).
    /// Maps DECSCUSR parameter values to CursorShape enum.
    /// </summary>
    private void HandleDecscusr(ReadOnlySpan<int> parameters)
    {
        if (Cursor is null) return;

        int ps = Param(parameters, 0, 1); // Default to 1 (BlinkingBlock)

        // DECSCUSR parameter 0 means "use default" which is BlinkingBlock
        if (ps == 0) ps = 1;

        // Map parameter to CursorShape enum (values align 1:1)
        if (ps >= 1 && ps <= 6)
        {
            Cursor.Shape = (CursorShape)ps;
            Cursor.UpdateGlyphForShape();
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  Terminal Query Responses (DA, DSR)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Handles CSI Ps n — Device Status Report (DSR).
    /// Ps 5 = status report (OK), Ps 6 = cursor position report (CPR).
    /// Responses are sent via <see cref="Output"/>; silently ignored when Output is <see langword="null"/>.
    /// </summary>
    private void HandleDeviceStatusReport(ReadOnlySpan<int> parameters)
    {
        if (Output is null) return;

        int ps = Param(parameters, 0, 0);
        switch (ps)
        {
            case 5: // Status report — respond "OK"
                Output.Write("\x1b[0n");
                break;
            case 6: // Cursor position report — respond with row;column (1-based)
                Output.Write($"\x1b[{State.CursorRow + 1};{State.CursorColumn + 1}R");
                break;
        }
    }

    /// <summary>
    /// Handles CSI Ps c — Device Attributes (DA1).
    /// Ps 0 (or omitted) requests primary device attributes.
    /// Responds as a VT220 with ANSI color support.
    /// </summary>
    private void HandleDeviceAttributes(ReadOnlySpan<int> parameters)
    {
        if (Output is null) return;

        int ps = Param(parameters, 0, 0);
        if (ps == 0)
        {
            // Respond as VT220: CSI ? 62 ; 1 ; 2 ; 6 ; 7 ; 8 ; 9 c
            // 62 = VT220, 1 = 132 cols, 2 = printer port, 6 = selective erase,
            // 7 = DRCS, 8 = UDK, 9 = national replacement charsets
            Output.Write("\x1b[?62;1;2;6;7;8;9c");
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  Font Selection (CTerm FNT)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Handles CSI Ps1 ; Ps2 SP D — Font Selection per cterm.adoc.
    /// Ps1 = font slot (0-3), Ps2 = font ID (0-255).
    /// </summary>
    private void HandleFontSelection(ReadOnlySpan<int> parameters)
    {
        int slot = Param(parameters, 0, 0);
        int fontId = Param(parameters, 1, 0);

        if (slot < 0 || slot > 3 || fontId < 0 || fontId > 255)
        {
            State.LastFontSelectionResult = 1; // failure
            return;
        }

        State.SetFontSlot(slot, fontId);
        State.LastFontSelectionResult = 0; // success
    }

    /// <summary>
    /// Handles CSI = Ps n — CTerm State/Mode Request/Report (CTSMRR).
    /// Currently supports Ps=1 (Font State Report).
    /// </summary>
    private void HandleCtermExtension(ReadOnlySpan<int> parameters, byte final)
    {
        if ((char)final != 'n') return;

        int ps = Param(parameters, 0, 0);
        if (ps == 1)
        {
            // Font State Report: CSI = 1 ; pF ; pR ; pS0 ; pS1 ; pS2 ; pS3 n
            // pF = first available loadable-font slot (43)
            // pR = last font selection result
            // pS0-pS3 = font slot values
            // Note: response would be written to the input stream; for now we just track the state.
            // The host application can read State.LastFontSelectionResult and font slots directly.
        }
    }

    /// <summary>
    /// Returns the <see cref="IFont"/> for the given font ID. Custom fonts loaded via DCS CTLF
    /// are looked up first; if not found, the default constructor font is returned.
    /// </summary>
    public IFont GetFontForSlot(int fontId)
    {
        if (_loadedFonts.TryGetValue(fontId, out IFont? font))
            return font;

        return _font;
    }

    /// <summary>
    /// Returns the <see cref="IFont"/> that should be used for the current character based on
    /// the active font slot, mode flags, and SGR attributes.
    /// </summary>
    public IFont GetActiveFont() => GetFontForSlot(State.GetActiveFontSlot());

    // ═══════════════════════════════════════════════════════════
    //  Erase
    // ═══════════════════════════════════════════════════════════

    private void HandleEraseDisplay(int mode)
    {
        Color bg = ResolveBackground();

        switch (mode)
        {
            case 0: // From cursor to end
                EraseRange(State.CursorColumn, State.CursorRow, _surface.Width - 1, State.CursorRow, bg);
                for (int row = State.CursorRow + 1; row < _surface.Height; row++)
                    ClearRow(row, bg);
                break;

            case 1: // From start to cursor
                for (int row = 0; row < State.CursorRow; row++)
                    ClearRow(row, bg);
                EraseRange(0, State.CursorRow, State.CursorColumn, State.CursorRow, bg);
                break;

            case 2: // Entire display
            case 3: // Entire display + scrollback (treat same for now)
                for (int row = 0; row < _surface.Height; row++)
                    ClearRow(row, bg);
                break;
        }

        _surface.IsDirty = true;
    }

    private void HandleEraseInLine(int mode)
    {
        Color bg = ResolveBackground();

        switch (mode)
        {
            case 0: // From cursor to end of line
                EraseRange(State.CursorColumn, State.CursorRow, _surface.Width - 1, State.CursorRow, bg);
                break;

            case 1: // From start of line to cursor
                EraseRange(0, State.CursorRow, State.CursorColumn, State.CursorRow, bg);
                break;

            case 2: // Entire line
                ClearRow(State.CursorRow, bg);
                break;
        }

        _surface.IsDirty = true;
    }

    // ═══════════════════════════════════════════════════════════
    //  SGR (Select Graphic Rendition)
    // ═══════════════════════════════════════════════════════════

    private void HandleSgr(ReadOnlySpan<int> parameters)
    {
        if (parameters.Length == 0)
        {
            State.ResetSgr();
            return;
        }

        int i = 0;
        while (i < parameters.Length)
        {
            int p = parameters[i];
            switch (p)
            {
                case 0:
                    State.ResetSgr();
                    break;

                // Intensity
                case 1:
                    State.Bold = true;
                    State.Dim = false;
                    break;
                case 2:
                    State.Dim = true;
                    State.Bold = false;
                    break;

                // Style
                case 3: State.Italic = true; break;
                case 4: State.Underline = true; break;
                case 5: State.Blink = true; break;
                case 7: State.Reverse = true; break;
                case 8: State.Concealed = true; break;
                case 9: State.Strikethrough = true; break;

                // Reset style
                case 22: State.Bold = false; State.Dim = false; break;
                case 23: State.Italic = false; break;
                case 24: State.Underline = false; break;
                case 25: State.Blink = false; break;
                case 27: State.Reverse = false; break;
                case 28: State.Concealed = false; break;
                case 29: State.Strikethrough = false; break;

                // Standard foreground 30–37
                case >= 30 and <= 37:
                    State.ForegroundMode = ColorMode.Palette;
                    State.ForegroundIndex = p - 30;
                    break;

                // Extended foreground
                case 38:
                    i = HandleExtendedColor(parameters, i, isForeground: true);
                    continue;

                // Default foreground
                case 39:
                    State.ForegroundMode = ColorMode.Default;
                    break;

                // Standard background 40–47
                case >= 40 and <= 47:
                    State.BackgroundMode = ColorMode.Palette;
                    State.BackgroundIndex = p - 40;
                    break;

                // Extended background
                case 48:
                    i = HandleExtendedColor(parameters, i, isForeground: false);
                    continue;

                // Default background
                case 49:
                    State.BackgroundMode = ColorMode.Default;
                    break;

                // Bright foreground 90–97
                case >= 90 and <= 97:
                    State.ForegroundMode = ColorMode.Palette;
                    State.ForegroundIndex = p - 90 + 8;
                    break;

                // Bright background 100–107
                case >= 100 and <= 107:
                    State.BackgroundMode = ColorMode.Palette;
                    State.BackgroundIndex = p - 100 + 8;
                    break;
            }

            i++;
        }
    }

    private int HandleExtendedColor(ReadOnlySpan<int> parameters, int index, bool isForeground)
    {
        if (index + 1 >= parameters.Length)
            return index + 1;

        int mode = parameters[index + 1];

        // 256-color: 38;5;n / 48;5;n
        if (mode == 5 && index + 2 < parameters.Length)
        {
            int colorIndex = Math.Clamp(parameters[index + 2], 0, 255);
            if (isForeground)
            {
                State.ForegroundMode = ColorMode.Palette;
                State.ForegroundIndex = colorIndex;
            }
            else
            {
                State.BackgroundMode = ColorMode.Palette;
                State.BackgroundIndex = colorIndex;
            }

            return index + 3;
        }

        // Truecolor: 38;2;r;g;b / 48;2;r;g;b
        if (mode == 2 && index + 4 < parameters.Length)
        {
            Color color = Palette.FromTrueColor(
                parameters[index + 2],
                parameters[index + 3],
                parameters[index + 4]);

            if (isForeground)
            {
                State.ForegroundMode = ColorMode.TrueColor;
                State.ForegroundRgb = color;
            }
            else
            {
                State.BackgroundMode = ColorMode.TrueColor;
                State.BackgroundRgb = color;
            }

            return index + 5;
        }

        return index + 2;
    }

    // ═══════════════════════════════════════════════════════════
    //  Color resolution
    // ═══════════════════════════════════════════════════════════

    private (Color fg, Color bg) ResolveColors()
    {
        Color fg = ResolveForeground();
        Color bg = ResolveBackground();

        if (State.Reverse)
            (fg, bg) = (bg, fg);

        if (State.Concealed)
            fg = bg;

        return (fg, bg);
    }

    private Color ResolveForeground()
    {
        Color color;

        switch (State.ForegroundMode)
        {
            case ColorMode.TrueColor:
                color = State.ForegroundRgb;
                break;
            case ColorMode.Palette:
                int idx = State.ForegroundIndex;
                if (State.Bold && idx < 8)
                    idx += 8;
                color = Palette.GetColor(idx);
                break;
            default:
                // CGA convention: default foreground is palette 7.
                // Bold brightens it to palette 15 (bright white).
                color = State.Bold ? Palette.GetColor(15) : State.DefaultForeground;
                break;
        }

        if (State.Dim)
            color = new Color(color.R / 2, color.G / 2, color.B / 2);

        return color;
    }

    private Color ResolveBackground()
    {
        return State.BackgroundMode switch
        {
            ColorMode.TrueColor => State.BackgroundRgb,
            ColorMode.Palette => Palette.GetColor(State.BackgroundIndex),
            _ => State.DefaultBackground,
        };
    }

    // ═══════════════════════════════════════════════════════════
    //  Cell helpers
    // ═══════════════════════════════════════════════════════════

    private void CopyCell(int srcCol, int srcRow, int dstCol, int dstRow)
    {
        ColoredGlyphBase src = _surface[srcCol, srcRow];
        ColoredGlyphBase dst = _surface[dstCol, dstRow];
        dst.Glyph = src.Glyph;
        dst.Foreground = src.Foreground;
        dst.Background = src.Background;
        dst.Mirror = src.Mirror;
        dst.Decorators = src.Decorators != null ? new List<CellDecorator>(src.Decorators) : null;
    }

    private void ClearRow(int row, Color bg)
    {
        int width = _surface.Width;
        for (int col = 0; col < width; col++)
            ClearCell(col, row, bg);
    }

    private void ClearCell(int col, int row, Color bg)
    {
        ColoredGlyphBase cell = _surface[col, row];
        cell.Glyph = ' ';
        cell.Foreground = State.DefaultForeground;
        cell.Background = bg;
        cell.Mirror = Mirror.None;
        cell.Decorators = null;
    }

    private void EraseRange(int startCol, int row, int endCol, int endRow, Color bg)
    {
        for (int col = startCol; col <= endCol; col++)
            ClearCell(col, row, bg);
    }

    private void FullReset()
    {
        State.Reset();
        Palette.ResetAll();

        Color bg = State.DefaultBackground;
        for (int row = 0; row < _surface.Height; row++)
            ClearRow(row, bg);

        _surface.IsDirty = true;
        SyncCursorPosition();
    }

    /// <summary>
    /// Gets a parameter value, returning <paramref name="defaultValue"/> when the
    /// parameter is missing or zero (ECMA-48: zero means "use default").
    /// </summary>
    private static int Param(ReadOnlySpan<int> parameters, int index, int defaultValue)
    {
        if (index >= parameters.Length) return defaultValue;
        int v = parameters[index];
        return v == 0 ? defaultValue : v;
    }

    /// <summary>
    /// Returns true for C0 bytes that carry IBM CP437 glyph assignments in ANSI-BBS mode.
    /// 0x0C (FF) is intentionally excluded — it is a clear-screen command, not a glyph.
    /// </summary>
    private static bool IsAnsiBbsGlyphByte(byte b) => b switch
    {
        0x01 or 0x02 or 0x03 or 0x04 or 0x05 or 0x06 => true, // ☺ ☻ ♥ ♦ ♣ ♠
        0x0B => true,                                           // ♂
        0x0E or 0x0F => true,                                   // ♫ ☼
        0x10 or 0x11 or 0x12 or 0x13 or 0x14 or 0x15 or 0x16 => true, // ► ◄ ↕ ‼ ¶ § ▬
        0x17 or 0x18 or 0x19 or 0x1A => true,                  // ↨ ↑ ↓ →
        0x1C or 0x1D or 0x1E or 0x1F => true,                  // ∟ ↔ ▲ ▼
        _ => false
    };

    // ═══════════════════════════════════════════════════════════
    //  Glyph resolution
    // ═══════════════════════════════════════════════════════════

    private int ResolveGlyph(char ch)
    {
        if (Encoding == CharacterEncoding.Unicode)
            return ch;

        // Codepage437 mode: check if the font knows this code point directly
        if (_font.GlyphRectangles.ContainsKey((int)ch))
            return ch;

        // Try mapping Unicode → CP437 via the static table
        if (ch < (char)Cp437Table.Length && Cp437Table[ch] != 0xFFFF)
            return Cp437Table[ch];

        // Unmappable — pass through as-is (font will show its fallback glyph)
        return ch;
    }

    /// <summary>
    /// Lookup table mapping Unicode code points (index) to CP437 byte values.
    /// Covers the ASCII/Latin range where most CP437 characters live.
    /// Entries of 0xFFFF mean "no mapping available".
    /// </summary>
    private static readonly ushort[] Cp437Table = BuildCp437Table();

    private static ushort[] BuildCp437Table()
    {
        // The 256 characters of CP437, indexed by their CP437 byte value.
        // This is the well-known IBM PC character set.
        char[] cp437Chars = new char[256]
        {
            // 0x00–0x0F
            '\0',    '☺', '☻', '♥', '♦', '♣', '♠', '•',
            '◘',    '○', '◙', '♂', '♀', '♪', '♫', '☼',
            // 0x10–0x1F
            '►',    '◄', '↕', '‼', '¶', '§', '▬', '↨',
            '↑',    '↓', '→', '←', '∟', '↔', '▲', '▼',
            // 0x20–0x2F (standard ASCII)
            ' ',    '!', '"', '#', '$', '%', '&', '\'',
            '(',    ')', '*', '+', ',', '-', '.', '/',
            // 0x30–0x3F
            '0',    '1', '2', '3', '4', '5', '6', '7',
            '8',    '9', ':', ';', '<', '=', '>', '?',
            // 0x40–0x4F
            '@',    'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H',    'I', 'J', 'K', 'L', 'M', 'N', 'O',
            // 0x50–0x5F
            'P',    'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X',    'Y', 'Z', '[', '\\', ']', '^', '_',
            // 0x60–0x6F
            '`',    'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h',    'i', 'j', 'k', 'l', 'm', 'n', 'o',
            // 0x70–0x7F
            'p',    'q', 'r', 's', 't', 'u', 'v', 'w',
            'x',    'y', 'z', '{', '|', '}', '~', '⌂',
            // 0x80–0x8F
            'Ç',    'ü', 'é', 'â', 'ä', 'à', 'å', 'ç',
            'ê',    'ë', 'è', 'ï', 'î', 'ì', 'Ä', 'Å',
            // 0x90–0x9F
            'É',    'æ', 'Æ', 'ô', 'ö', 'ò', 'û', 'ù',
            'ÿ',    'Ö', 'Ü', '¢', '£', '¥', '₧', 'ƒ',
            // 0xA0–0xAF
            'á',    'í', 'ó', 'ú', 'ñ', 'Ñ', 'ª', 'º',
            '¿',    '⌐', '¬', '½', '¼', '¡', '«', '»',
            // 0xB0–0xBF (box drawing light)
            '░',    '▒', '▓', '│', '┤', '╡', '╢', '╖',
            '╕',    '╣', '║', '╗', '╝', '╜', '╛', '┐',
            // 0xC0–0xCF
            '└',    '┴', '┬', '├', '─', '┼', '╞', '╟',
            '╚',    '╔', '╩', '╦', '╠', '═', '╬', '╧',
            // 0xD0–0xDF
            '╨',    '╤', '╥', '╙', '╘', '╒', '╓', '╫',
            '╪',    '┘', '┌', '█', '▄', '▌', '▐', '▀',
            // 0xE0–0xEF
            'α',    'ß', 'Γ', 'π', 'Σ', 'σ', 'µ', 'τ',
            'Φ',    'Θ', 'Ω', 'δ', '∞', 'φ', 'ε', '∩',
            // 0xF0–0xFF
            '≡',    '±', '≥', '≤', '⌠', '⌡', '÷', '≈',
            '°',    '∙', '·', '√', 'ⁿ', '²', '■', '\u00A0'
        };

        // Find the max Unicode code point in the table to size the reverse map
        int maxCodePoint = 0;
        for (int i = 0; i < 256; i++)
        {
            if (cp437Chars[i] > maxCodePoint)
                maxCodePoint = cp437Chars[i];
        }

        ushort[] table = new ushort[maxCodePoint + 1];
        Array.Fill(table, (ushort)0xFFFF);

        // Build reverse mapping: Unicode → CP437 byte
        for (int i = 0; i < 256; i++)
        {
            int codePoint = cp437Chars[i];
            if (codePoint < table.Length)
                table[codePoint] = (ushort)i;
        }

        return table;
    }

    // ═══════════════════════════════════════════════════════════
    //  Cell decorator application
    // ═══════════════════════════════════════════════════════════

    private void ApplyDecorators(ColoredGlyphBase cell, Color fg)
    {
        // Italic: SadConsole fonts are tile-based — true italic is not possible. Attribute is tracked in State but not rendered.
        // Blink: tracked in State. TODO: integrate with a blink component or timer-based toggling when available.

        if (!State.Underline && !State.Strikethrough)
        {
            cell.Decorators = null;
            return;
        }

        var decorators = new List<CellDecorator>();

        if (State.Underline)
        {
            decorators.Add(_font.HasGlyphDefinition("underline")
                ? _font.GetDecorator("underline", fg)
                : new CellDecorator(fg, 95, Mirror.None));
        }

        if (State.Strikethrough)
        {
            decorators.Add(_font.HasGlyphDefinition("strikethrough")
                ? _font.GetDecorator("strikethrough", fg)
                : new CellDecorator(fg, 196, Mirror.None));
        }

        cell.Decorators = decorators;
    }

    // ═══════════════════════════════════════════════════════════
    //  OSC helpers
    // ═══════════════════════════════════════════════════════════

    private void HandleOsc4(ReadOnlySpan<byte> data)
    {
        // Format: {index};{color}[;{index};{color}]...
        // Multiple palette entries can be set in a single OSC 4 sequence.
        while (data.Length > 0)
        {
            int sepIdx = ByteIndexOf(data, (byte)';');
            if (sepIdx < 0) return;

            int index = ParseDecimal(data.Slice(0, sepIdx));
            if (index < 0 || index > 255) return;

            data = data.Slice(sepIdx + 1);

            // Find the end of the color spec (next ';' that isn't part of the color)
            // rgb:rr/gg/bb uses '/' as separator, so next ';' is the entry boundary
            int colorEnd = ByteIndexOf(data, (byte)';');
            ReadOnlySpan<byte> colorData = colorEnd >= 0 ? data.Slice(0, colorEnd) : data;

            Color? color = ParseOscColor(colorData);
            if (color.HasValue)
                Palette[index] = color.Value;

            if (colorEnd < 0) break;
            data = data.Slice(colorEnd + 1);
        }
    }

    private void HandleOscDefaultColor(ReadOnlySpan<byte> data, bool isForeground)
    {
        Color? color = ParseOscColor(data);
        if (!color.HasValue) return;

        if (isForeground)
            State.DefaultForeground = color.Value;
        else
            State.DefaultBackground = color.Value;
    }

    private static Color? ParseOscColor(ReadOnlySpan<byte> data)
    {
        // Format 1: rgb:{rr}/{gg}/{bb} (hex, each component 1–4 digits, X11-style scaling)
        if (data.Length >= 4 && data[0] == (byte)'r' && data[1] == (byte)'g' &&
            data[2] == (byte)'b' && data[3] == (byte)':')
        {
            ReadOnlySpan<byte> rgb = data.Slice(4);
            int slash1 = ByteIndexOf(rgb, (byte)'/');
            if (slash1 < 0) return null;

            ReadOnlySpan<byte> rest = rgb.Slice(slash1 + 1);
            int slash2 = ByteIndexOf(rest, (byte)'/');
            if (slash2 < 0) return null;

            int r = ParseHexScaled(rgb.Slice(0, slash1));
            int g = ParseHexScaled(rest.Slice(0, slash2));
            int b = ParseHexScaled(rest.Slice(slash2 + 1));

            if (r < 0 || g < 0 || b < 0) return null;
            return new Color(r, g, b);
        }

        // Format 2: #{rrggbb} (6 hex digits)
        if (data.Length >= 7 && data[0] == (byte)'#')
        {
            int r = ParseHex(data.Slice(1, 2));
            int g = ParseHex(data.Slice(3, 2));
            int b = ParseHex(data.Slice(5, 2));

            if (r < 0 || g < 0 || b < 0) return null;
            return new Color(r, g, b);
        }

        return null;
    }

    /// <summary>
    /// Parses 1–4 hex digits and scales to 0–255 per X11 color spec.
    /// </summary>
    private static int ParseHexScaled(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0 || data.Length > 4) return -1;

        int value = 0;
        for (int i = 0; i < data.Length; i++)
        {
            int digit = HexDigit(data[i]);
            if (digit < 0) return -1;
            value = (value << 4) | digit;
        }

        return data.Length switch
        {
            1 => value * 17,    // 0x0–0xF → 0x00–0xFF
            2 => value,         // 0x00–0xFF → 0x00–0xFF
            3 => value >> 4,    // 0x000–0xFFF → 0x00–0xFF
            4 => value >> 8,    // 0x0000–0xFFFF → 0x00–0xFF
            _ => -1
        };
    }

    private static int HexDigit(byte b)
    {
        if (b >= (byte)'0' && b <= (byte)'9') return b - '0';
        if (b >= (byte)'a' && b <= (byte)'f') return b - 'a' + 10;
        if (b >= (byte)'A' && b <= (byte)'F') return b - 'A' + 10;
        return -1;
    }

    private static int ParseHex(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0) return -1;
        int value = 0;
        for (int i = 0; i < data.Length; i++)
        {
            int digit = HexDigit(data[i]);
            if (digit < 0) return -1;
            value = (value << 4) | digit;
        }
        return value;
    }

    private static int ParseDecimal(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0) return -1;
        int value = 0;
        for (int i = 0; i < data.Length; i++)
        {
            byte b = data[i];
            if (b < (byte)'0' || b > (byte)'9') return -1;
            value = value * 10 + (b - '0');
        }
        return value;
    }

    private static int ByteIndexOf(ReadOnlySpan<byte> data, byte target)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == target) return i;
        }
        return -1;
    }
}
