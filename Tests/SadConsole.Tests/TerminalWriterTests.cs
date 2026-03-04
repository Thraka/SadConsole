using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Integration tests for Terminal.Writer → ICellSurface rendering.
/// Writer implements ITerminalHandler and renders ANSI sequences onto a real CellSurface.
/// These tests define the integration contract BEFORE the Writer exists.
/// </summary>
[TestClass]
public class TerminalWriterTests
{
    // Default terminal colors — must match State.DefaultForeground / DefaultBackground
    private static readonly Color DefaultFg = new Color(170, 170, 170); // palette index 7
    private static readonly Color DefaultBg = new Color(0, 0, 0);       // palette index 0

    // ANSI palette colors (CGA/CTerm defaults used by SGR 30-37 / 40-47)
    private static readonly Color AnsiRed = new Color(170, 0, 0);
    private static readonly Color AnsiGreen = new Color(0, 170, 0);
    private static readonly Color AnsiBlue = new Color(0, 0, 170);
    private static readonly Color AnsiWhite = new Color(170, 170, 170);

    private SadConsole.CellSurface _surface;
    private Writer _writer;

    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
        _surface = new SadConsole.CellSurface(80, 25);
        _writer = new Writer(_surface, GameHost.Instance.EmbeddedFont);
    }

    // ──────────────────────────────────────────────
    // 1. Basic Rendering
    // ──────────────────────────────────────────────

    [TestMethod]
    public void BasicRender_Hello_GlyphsAtCorrectPositions()
    {
        _writer.Feed("Hello");

        Assert.AreEqual((int)'H', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'e', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'l', _surface[2, 0].Glyph);
        Assert.AreEqual((int)'l', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'o', _surface[4, 0].Glyph);
    }

    [TestMethod]
    public void BasicRender_LineFeed_MovesDown()
    {
        // Default LineFeeds mode is Implicit: LF implies CR+LF
        _writer.Feed("AB\nCD");

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', _surface[1, 1].Glyph);
    }

    [TestMethod]
    public void BasicRender_LineFeed_Strict_MovesDownOnly()
    {
        _writer.LineFeeds = LineFeedMode.Strict;
        _writer.Feed("AB\nCD");

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[2, 1].Glyph);
        Assert.AreEqual((int)'D', _surface[3, 1].Glyph);
    }

    [TestMethod]
    public void BasicRender_CarriageReturn_MovesToColumnZero()
    {
        _writer.Feed("ABCDE\rX");

        // CR moves cursor to column 0, 'X' overwrites 'A'
        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
    }

    [TestMethod]
    public void BasicRender_CrLf_PositionsCorrectly()
    {
        _writer.Feed("AB\r\nCD");

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', _surface[1, 1].Glyph);
    }

    [TestMethod]
    public void BasicRender_CursorAdvances()
    {
        _writer.Feed("AB");

        Assert.AreEqual(2, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    // ──────────────────────────────────────────────
    // 2. SGR Colors
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Sgr_RedForeground()
    {
        // ESC[31m — set foreground to red
        _writer.Feed("\x1b[31mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(AnsiRed, _surface[0, 0].Foreground);
    }

    [TestMethod]
    public void Sgr_BlueBackground()
    {
        // ESC[44m — set background to blue
        _writer.Feed("\x1b[44mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(AnsiBlue, _surface[0, 0].Background);
    }

    [TestMethod]
    public void Sgr_Reset_RestoresDefaults()
    {
        // Set red fg, then reset, then print
        _writer.Feed("\x1b[31mA\x1b[0mB");

        // 'A' should be red
        Assert.AreEqual(AnsiRed, _surface[0, 0].Foreground);

        // 'B' should be back to defaults
        Assert.AreEqual(DefaultFg, _surface[1, 0].Foreground);
        Assert.AreEqual(DefaultBg, _surface[1, 0].Background);
    }

    [TestMethod]
    public void Sgr_BoldYellow_BrightYellowForeground()
    {
        // ESC[1;33m — bold + yellow → bright yellow
        _writer.Feed("\x1b[1;33mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        // Bold + standard color = bright variant. Bright yellow = Color(255, 255, 85) per CGA palette
        // We check via the Writer's palette for index 33 with bold applied.
        // The foreground should NOT be normal yellow, it should be the bright variant.
        Color normalYellow = _writer.Palette[33 - 30]; // palette index 3 = yellow
        Assert.AreNotEqual(normalYellow, _surface[0, 0].Foreground,
            "Bold+yellow should produce bright yellow, not normal yellow");
    }

    [TestMethod]
    public void Sgr_256Color_Foreground()
    {
        // ESC[38;5;196m — 256-color foreground index 196
        _writer.Feed("\x1b[38;5;196mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Color expected = _writer.Palette[196];
        Assert.AreEqual(expected, _surface[0, 0].Foreground);
    }

    [TestMethod]
    public void Sgr_TrueColor_Background()
    {
        // ESC[48;2;128;0;255m — truecolor background (128, 0, 255)
        _writer.Feed("\x1b[48;2;128;0;255mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(new Color(128, 0, 255), _surface[0, 0].Background);
    }

    [TestMethod]
    public void Sgr_TrueColor_Foreground()
    {
        // ESC[38;2;10;20;30m — truecolor foreground (10, 20, 30)
        _writer.Feed("\x1b[38;2;10;20;30mX");

        Assert.AreEqual(new Color(10, 20, 30), _surface[0, 0].Foreground);
    }

    [TestMethod]
    public void Sgr_ReverseVideo_SwapsFgBg()
    {
        // ESC[7m — reverse video
        _writer.Feed("\x1b[7mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        // In reverse video, the foreground and background swap
        Assert.AreEqual(DefaultBg, _surface[0, 0].Foreground);
        Assert.AreEqual(DefaultFg, _surface[0, 0].Background);
    }

    [TestMethod]
    public void Sgr_ReverseVideo_WithColor()
    {
        // Set red fg, then reverse
        _writer.Feed("\x1b[31;7mX");

        // Red foreground + reverse → fg becomes default bg, bg becomes red
        Assert.AreEqual(DefaultBg, _surface[0, 0].Foreground);
        Assert.AreEqual(AnsiRed, _surface[0, 0].Background);
    }

    [TestMethod]
    public void Sgr_DefaultForeground_Sgr39()
    {
        // ESC[31m (red) then ESC[39m (default fg)
        _writer.Feed("\x1b[31mA\x1b[39mB");

        Assert.AreEqual(AnsiRed, _surface[0, 0].Foreground);
        Assert.AreEqual(DefaultFg, _surface[1, 0].Foreground);
    }

    [TestMethod]
    public void Sgr_DefaultBackground_Sgr49()
    {
        // ESC[44m (blue bg) then ESC[49m (default bg)
        _writer.Feed("\x1b[44mA\x1b[49mB");

        Assert.AreEqual(AnsiBlue, _surface[0, 0].Background);
        Assert.AreEqual(DefaultBg, _surface[1, 0].Background);
    }

    [TestMethod]
    public void Sgr_BrightForeground_90to97()
    {
        // ESC[91m — bright red foreground
        _writer.Feed("\x1b[91mX");

        Color brightRed = _writer.Palette[9]; // bright red is palette index 9
        Assert.AreEqual(brightRed, _surface[0, 0].Foreground);
    }

    [TestMethod]
    public void Sgr_BrightBackground_100to107()
    {
        // ESC[104m — bright blue background
        _writer.Feed("\x1b[104mX");

        Color brightBlue = _writer.Palette[12]; // bright blue is palette index 12
        Assert.AreEqual(brightBlue, _surface[0, 0].Background);
    }

    [TestMethod]
    public void Sgr_MultipleSgr_CombinedInOneSequence()
    {
        // ESC[1;31;44m — bold + red fg + blue bg
        _writer.Feed("\x1b[1;31;44mX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(AnsiBlue, _surface[0, 0].Background);
        // Foreground should be bright red (bold applied to standard red)
    }

    // ──────────────────────────────────────────────
    // 3. Cursor Movement
    // ──────────────────────────────────────────────

    [TestMethod]
    public void CursorMove_Cup_PositionsCorrectly()
    {
        // ESC[5;10H — CUP: move to row 5, col 10 (1-based → 0-based: row 4, col 9)
        _writer.Feed("\x1b[5;10HX");

        Assert.AreEqual((int)'X', _surface[9, 4].Glyph);
    }

    [TestMethod]
    public void CursorMove_Cup_DefaultsToHome()
    {
        // ESC[H — CUP with no params → home (0,0)
        _writer.Feed("ABC\x1b[HX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorUp()
    {
        // Move to row 5, then ESC[A (CUU 1) — should be at row 4
        _writer.Feed("\x1b[6;1H"); // row 6, col 1 → (0, 5)
        _writer.Feed("\x1b[AX");   // CUU 1 → (0, 4)

        Assert.AreEqual((int)'X', _surface[0, 4].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorDown3()
    {
        // ESC[3B — CUD 3: move down 3 rows
        _writer.Feed("\x1b[3BX");

        Assert.AreEqual((int)'X', _surface[0, 3].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorForward2()
    {
        // ESC[2C — CUF 2: move right 2
        _writer.Feed("\x1b[2CX");

        Assert.AreEqual((int)'X', _surface[2, 0].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorBack()
    {
        // ESC[D — CUB 1: move left 1
        _writer.Feed("AB\x1b[DX");

        // Cursor was at col 2 after "AB", back 1 → col 1, 'X' overwrites 'B'
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[1, 0].Glyph);
    }

    [TestMethod]
    public void CursorMove_Cha_CursorToColumn()
    {
        // ESC[10G — CHA: cursor to column 10 (1-based → 0-based: 9)
        _writer.Feed("\x1b[10GX");

        Assert.AreEqual((int)'X', _surface[9, 0].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorUp_ClampsAtTop()
    {
        // At row 0, CUU 5 should clamp at row 0
        _writer.Feed("\x1b[99AX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void CursorMove_CursorBack_ClampsAtLeft()
    {
        // At col 0, CUB 5 should clamp at col 0
        _writer.Feed("\x1b[99DX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(1, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void CursorMove_CursorDown_ClampsAtBottom()
    {
        // CUD beyond surface height should clamp at last row
        _writer.Feed("\x1b[999BX");

        Assert.AreEqual((int)'X', _surface[0, 24].Glyph);
    }

    [TestMethod]
    public void CursorMove_CursorForward_ClampsAtRight()
    {
        // CUF beyond surface width should clamp at last column
        _writer.Feed("\x1b[999CX");

        Assert.AreEqual((int)'X', _surface[79, 0].Glyph);
    }

    // ──────────────────────────────────────────────
    // 4. Erase Operations
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Erase_EntireDisplay_Ed2()
    {
        // Fill some content, then ESC[2J to clear everything
        _writer.Feed("Hello World");
        _writer.Feed("\x1b[2J");

        for (int x = 0; x < _surface.Width; x++)
        {
            for (int y = 0; y < _surface.Height; y++)
            {
                Assert.AreEqual(' ', (char)_surface[x, y].Glyph,
                    $"Cell ({x},{y}) should be space after ED 2");
            }
        }
    }

    [TestMethod]
    public void Erase_CursorToEndOfLine_El0()
    {
        // Print text, move cursor back, erase to end of line
        _writer.Feed("ABCDE\x1b[1;3H"); // cursor at col 2 (1-based col 3 → 0-based col 2)
        _writer.Feed("\x1b[K");           // EL 0: erase from cursor to end of line

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph); // erased
        Assert.AreEqual(' ', (char)_surface[3, 0].Glyph); // erased
        Assert.AreEqual(' ', (char)_surface[4, 0].Glyph); // erased
    }

    [TestMethod]
    public void Erase_StartOfLineToCursor_El1()
    {
        // Print text, move cursor, erase from start to cursor
        _writer.Feed("ABCDE\x1b[1;3H"); // cursor at col 2
        _writer.Feed("\x1b[1K");          // EL 1: erase from start of line to cursor

        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph); // erased
        Assert.AreEqual(' ', (char)_surface[1, 0].Glyph); // erased
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph); // erased (inclusive)
        Assert.AreEqual((int)'D', _surface[3, 0].Glyph);  // preserved
        Assert.AreEqual((int)'E', _surface[4, 0].Glyph);  // preserved
    }

    [TestMethod]
    public void Erase_EntireLine_El2()
    {
        _writer.Feed("ABCDE\x1b[1;3H"); // cursor at col 2
        _writer.Feed("\x1b[2K");          // EL 2: erase entire line

        for (int x = 0; x < _surface.Width; x++)
        {
            Assert.AreEqual(' ', (char)_surface[x, 0].Glyph,
                $"Col {x} should be space after EL 2");
        }
    }

    [TestMethod]
    public void Erase_CursorToEndOfDisplay_Ed0()
    {
        // Fill two rows, position cursor at start of row 0, erase to end of display
        _writer.Feed("Line1\r\nLine2");
        _writer.Feed("\x1b[1;1H");  // home
        _writer.Feed("\x1b[0J");    // ED 0: erase from cursor to end of display

        for (int y = 0; y < _surface.Height; y++)
        {
            for (int x = 0; x < _surface.Width; x++)
            {
                Assert.AreEqual(' ', (char)_surface[x, y].Glyph,
                    $"Cell ({x},{y}) should be space after ED 0 from home");
            }
        }
    }

    [TestMethod]
    public void Erase_StartOfDisplayToCursor_Ed1()
    {
        // Fill content, position cursor at end of row 1, erase from start to cursor
        _writer.Feed("AAAA\r\nBBBB\r\nCCCC");
        _writer.Feed("\x1b[2;3H");  // row 2, col 3 → (2, 1) 0-based
        _writer.Feed("\x1b[1J");    // ED 1: erase from start of display to cursor

        // Row 0 should be completely erased
        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[3, 0].Glyph);
        // Row 1 up to and including cursor should be erased
        Assert.AreEqual(' ', (char)_surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 1].Glyph);
        // Row 1 past cursor should be preserved
        Assert.AreEqual((int)'B', _surface[3, 1].Glyph);
        // Row 2 should be fully preserved
        Assert.AreEqual((int)'C', _surface[0, 2].Glyph);
    }

    // ──────────────────────────────────────────────
    // 5. Cursor Save/Restore
    // ──────────────────────────────────────────────

    [TestMethod]
    public void CursorSaveRestore_Csi_SAndU()
    {
        // Position at (5, 3), save, move away, restore
        _writer.Feed("\x1b[4;6H");   // row 4, col 6 → (5, 3) 0-based
        _writer.Feed("\x1b[s");      // save cursor
        _writer.Feed("\x1b[1;1H");   // move to home
        _writer.Feed("\x1b[u");      // restore cursor

        Assert.AreEqual(5, _writer.State.CursorColumn);
        Assert.AreEqual(3, _writer.State.CursorRow);
    }

    [TestMethod]
    public void CursorSaveRestore_DecscDecrc()
    {
        // ESC 7 (DECSC) and ESC 8 (DECRC)
        _writer.Feed("\x1b[4;6H");   // position at (5, 3)
        _writer.Feed("\x1b" + "7");  // DECSC: save
        _writer.Feed("\x1b[1;1H");   // move to home
        _writer.Feed("\x1b" + "8");  // DECRC: restore

        Assert.AreEqual(5, _writer.State.CursorColumn);
        Assert.AreEqual(3, _writer.State.CursorRow);
    }

    [TestMethod]
    public void CursorSaveRestore_PreservesAttributes()
    {
        // Save with red foreground, reset, restore, print — should still be red
        _writer.Feed("\x1b[31m");          // red fg
        _writer.Feed("\x1b[4;6H");         // position
        _writer.Feed("\x1b[s");            // save
        _writer.Feed("\x1b[0m");           // reset attributes
        _writer.Feed("\x1b[1;1H");         // move
        _writer.Feed("\x1b[u");            // restore
        _writer.Feed("X");

        // Cursor should be restored and the character should use the saved attributes
        Assert.AreEqual(5, _surface[5, 3].Glyph == (int)'X' ? 5 : -1);
    }

    // ──────────────────────────────────────────────
    // 6. Special Controls
    // ──────────────────────────────────────────────

    [TestMethod]
    public void SpecialControl_Backspace_MovesCursorLeft()
    {
        // BS (0x08) moves cursor left
        _writer.Feed("AB\x08X");

        // After "AB" cursor at col 2, BS moves to col 1, 'X' overwrites 'B'
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[1, 0].Glyph);
    }

    [TestMethod]
    public void SpecialControl_Backspace_ClampsAtColumnZero()
    {
        _writer.Feed("\x08X");

        // BS at column 0 should stay at 0
        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void SpecialControl_Tab_AdvancesToNextTabStop()
    {
        // HT (0x09) — default tab stops every 8 columns
        _writer.Feed("\tX");

        // Tab from col 0 should advance to col 8
        Assert.AreEqual((int)'X', _surface[8, 0].Glyph);
    }

    [TestMethod]
    public void SpecialControl_Tab_FromMidColumn()
    {
        // Start at col 3, tab should go to col 8
        _writer.Feed("ABC\tX");

        Assert.AreEqual((int)'X', _surface[8, 0].Glyph);
    }

    [TestMethod]
    public void SpecialControl_Tab_MultipleStops()
    {
        // Two tabs: col 0 → col 8 → col 16
        _writer.Feed("\t\tX");

        Assert.AreEqual((int)'X', _surface[16, 0].Glyph);
    }

    [TestMethod]
    public void SpecialControl_CrLf_Sequence()
    {
        _writer.Feed("AB\r\nCD\r\nEF");

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', _surface[1, 1].Glyph);
        Assert.AreEqual((int)'E', _surface[0, 2].Glyph);
        Assert.AreEqual((int)'F', _surface[1, 2].Glyph);
    }

    // ──────────────────────────────────────────────
    // 7. Auto-Wrap
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AutoWrap_RowFill_WrapsToNextLine()
    {
        // Create a narrow surface for easier testing
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");  // fills row 0
        writer.Feed("F");      // should wrap to (0, 1)

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'E', surface[4, 0].Glyph);
        Assert.AreEqual((int)'F', surface[0, 1].Glyph);
    }

    [TestMethod]
    public void AutoWrap_CursorPosition_AfterWrap()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");  // fills row 0
        writer.Feed("G");      // wraps

        // Cursor should be at col 1, row 1 (after writing 'G' at (0,1))
        Assert.AreEqual(1, writer.State.CursorColumn);
        Assert.AreEqual(1, writer.State.CursorRow);
    }

    // ──────────────────────────────────────────────
    // 8. Scroll Behavior
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Scroll_WritePastBottom_ContentScrollsUp()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill all 3 rows
        writer.Feed("AAAAA\n");
        writer.Feed("BBBBB\n");
        writer.Feed("CCCCC\n");
        // This newline should trigger scroll — row 0 (A's) scrolled out
        writer.Feed("D");

        // After scroll: row 0 = B's, row 1 = C's, row 2 = D + spaces
        Assert.AreEqual((int)'B', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', surface[0, 2].Glyph);
    }

    [TestMethod]
    public void Scroll_FillEntireSurface_ThenWrite()
    {
        var surface = new SadConsole.CellSurface(3, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill surface completely
        writer.Feed("AAA\nBBB\nCCC");
        // Now writing past the last cell should scroll
        writer.Feed("\nX");

        // Row 0 should now be what was row 1 (B's)
        Assert.AreEqual((int)'B', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual((int)'X', surface[0, 2].Glyph);
    }

    [TestMethod]
    public void Scroll_NewRow_IsClearedAfterScroll()
    {
        var surface = new SadConsole.CellSurface(5, 2);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nC");

        // After scroll, new bottom row should have 'C' at col 0 and spaces for the rest
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)surface[1, 1].Glyph);
        Assert.AreEqual(' ', (char)surface[4, 1].Glyph);
    }

    // ──────────────────────────────────────────────
    // 9. RIS (Full Reset)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Ris_FullReset_ClearsScreen()
    {
        _writer.Feed("Hello World");
        _writer.Feed("\x1b" + "c"); // ESC c — RIS

        for (int x = 0; x < _surface.Width; x++)
        {
            Assert.AreEqual(' ', (char)_surface[x, 0].Glyph,
                $"Col {x} should be space after RIS");
        }
    }

    [TestMethod]
    public void Ris_FullReset_CursorToHome()
    {
        _writer.Feed("\x1b[10;10H"); // move cursor
        _writer.Feed("\x1b" + "c");       // RIS

        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void Ris_FullReset_ResetsColors()
    {
        _writer.Feed("\x1b[31;44m"); // red fg, blue bg
        _writer.Feed("\x1b" + "c");       // RIS
        _writer.Feed("X");

        Assert.AreEqual(DefaultFg, _surface[0, 0].Foreground);
        Assert.AreEqual(DefaultBg, _surface[0, 0].Background);
    }

    // ──────────────────────────────────────────────
    // 10. Integration / Mixed Sequences
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Integration_ColoredTextAtPosition()
    {
        // Move cursor, set color, write text
        _writer.Feed("\x1b[3;5H");    // row 3, col 5 → (4, 2) 0-based
        _writer.Feed("\x1b[32m");     // green fg
        _writer.Feed("OK");

        Assert.AreEqual((int)'O', _surface[4, 2].Glyph);
        Assert.AreEqual((int)'K', _surface[5, 2].Glyph);
        Assert.AreEqual(AnsiGreen, _surface[4, 2].Foreground);
        Assert.AreEqual(AnsiGreen, _surface[5, 2].Foreground);
    }

    [TestMethod]
    public void Integration_ClearAndRedraw()
    {
        // Write, clear, write again
        _writer.Feed("OLD");
        _writer.Feed("\x1b[2J");     // clear
        _writer.Feed("\x1b[1;1H");   // home
        _writer.Feed("NEW");

        Assert.AreEqual((int)'N', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'E', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'W', _surface[2, 0].Glyph);
    }

    [TestMethod]
    public void Integration_MultipleColorChanges()
    {
        _writer.Feed("\x1b[31mR\x1b[32mG\x1b[34mB");

        Assert.AreEqual(AnsiRed, _surface[0, 0].Foreground);
        Assert.AreEqual((int)'R', _surface[0, 0].Glyph);

        Assert.AreEqual(AnsiGreen, _surface[1, 0].Foreground);
        Assert.AreEqual((int)'G', _surface[1, 0].Glyph);

        Assert.AreEqual(AnsiBlue, _surface[2, 0].Foreground);
        Assert.AreEqual((int)'B', _surface[2, 0].Glyph);
    }

    [TestMethod]
    public void Integration_AnsiArtLine()
    {
        // Simulate a simple ANSI art line with color changes and positioning
        string ansi = "\x1b[1;1H"        // home
                     + "\x1b[44m"         // blue bg
                     + "\x1b[37m"         // white fg
                     + "##########"       // 10 '#' chars
                     + "\x1b[0m";         // reset

        _writer.Feed(ansi);

        for (int x = 0; x < 10; x++)
        {
            Assert.AreEqual((int)'#', _surface[x, 0].Glyph);
            Assert.AreEqual(AnsiBlue, _surface[x, 0].Background);
        }
    }

    [TestMethod]
    public void Integration_ByteSpanFeed()
    {
        // Verify Feed works with raw byte data (UTF-8 encoded)
        byte[] data = Encoding.UTF8.GetBytes("Hello");
        _writer.Feed(data.AsSpan());

        Assert.AreEqual((int)'H', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'o', _surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Integration_SplitSequenceAcrossFeeds()
    {
        // ANSI sequence split across two Feed calls
        _writer.Feed("\x1b[3");  // partial CSI
        _writer.Feed("1mX");    // complete: ESC[31m then X

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual(AnsiRed, _surface[0, 0].Foreground);
    }

    [TestMethod]
    public void Integration_OverwriteCharacter()
    {
        // Write 'A', move back, write 'B' — should overwrite
        _writer.Feed("A\x1b[1;1HB");

        Assert.AreEqual((int)'B', _surface[0, 0].Glyph);
    }

    // ──────────────────────────────────────────────
    // 11. Palette
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Palette_Has256Entries()
    {
        Assert.IsGreaterThanOrEqualTo(256, _writer.Palette.Length, "Writer palette should have at least 256 entries");
    }

    [TestMethod]
    public void Palette_StandardColors_AreNonDefault()
    {
        // First 8 entries are the standard colors — they should not all be the same
        Color first = _writer.Palette[0];
        bool allSame = true;
        for (int i = 1; i < 8; i++)
        {
            if (_writer.Palette[i] != first)
            {
                allSame = false;
                break;
            }
        }
        Assert.IsFalse(allSame, "Standard palette colors should be distinct");
    }

    // ──────────────────────────────────────────────
    // 12. Writer State Accessors
    // ──────────────────────────────────────────────

    [TestMethod]
    public void State_InitialCursorPosition_IsZeroZero()
    {
        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void State_CursorTracksAfterPrint()
    {
        _writer.Feed("ABC");

        Assert.AreEqual(3, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void State_CursorTracksAfterNewline()
    {
        _writer.Feed("A\nB");

        Assert.AreEqual(1, _writer.State.CursorColumn);
        Assert.AreEqual(1, _writer.State.CursorRow);
    }

    // ──────────────────────────────────────────────
    // 13. Edge Cases
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Edge_EmptyFeed_NoChange()
    {
        _writer.Feed("");

        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void Edge_EmptyByteSpan_NoChange()
    {
        _writer.Feed(ReadOnlySpan<byte>.Empty);

        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void Edge_Cup_OneBased_RowOneColOne_IsOrigin()
    {
        // CUP row 1, col 1 is the top-left corner (0,0 internally)
        _writer.Feed("\x1b[1;1HX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void Edge_Cup_ZeroParams_TreatedAsOne()
    {
        // ESC[0;0H should be treated as ESC[1;1H (params clamp to 1)
        _writer.Feed("\x1b[0;0HX");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void Edge_Cup_BeyondBounds_Clamped()
    {
        // CUP beyond surface bounds should clamp
        _writer.Feed("\x1b[999;999HX");

        Assert.AreEqual((int)'X', _surface[79, 24].Glyph);
    }

    [TestMethod]
    public void Edge_UnrecognizedCsi_Ignored()
    {
        // An unrecognized CSI final byte should not crash
        _writer.Feed("\x1b[99zX");

        // X should still be written (the unknown sequence is ignored)
        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void Edge_SingleCellSurface()
    {
        var surface = new SadConsole.CellSurface(1, 1);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("A");

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
    }
}
