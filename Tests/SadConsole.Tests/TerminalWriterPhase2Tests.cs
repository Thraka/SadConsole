using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Integration tests for Terminal.Writer — Phases 5, 6, and 8.
/// Defines the contract for insert/delete/scroll operations, tab stop commands,
/// DEC modes, and scroll margins BEFORE Roy's implementation exists.
/// </summary>
[TestClass]
public class TerminalWriterPhase2Tests
{
    private static readonly Color DefaultFg = new Color(170, 170, 170);
    private static readonly Color DefaultBg = new Color(0, 0, 0);
    private static readonly Color AnsiRed = new Color(170, 0, 0);

    private SadConsole.CellSurface _surface;
    private Writer _writer;

    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
        _surface = new SadConsole.CellSurface(80, 25);
        _writer = new Writer(_surface, GameHost.Instance.EmbeddedFont);
    }

    // ══════════════════════════════════════════════════════════════
    //  Insert / Delete / Scroll Operations
    // ══════════════════════════════════════════════════════════════

    // ── ICH (Insert Character, CSI @) ──

    [TestMethod]
    public void Ich_InsertOne_ShiftsCharsRight()
    {
        // Write "ABCDE", move cursor to col 1, insert 1 character
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;2H");  // cursor at col 1 (1-based col 2)
        _writer.Feed("\x1b[1@");    // ICH 1

        // 'A' stays at col 0, blank at col 1, 'B' shifted to col 2, etc.
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[1, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[2, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'D', _surface[4, 0].Glyph);
        Assert.AreEqual((int)'E', _surface[5, 0].Glyph);
    }

    [TestMethod]
    public void Ich_InsertN_ShiftsNBlanks()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;1H");  // cursor at col 0
        _writer.Feed("\x1b[3@");    // ICH 3

        // 3 blanks inserted at start, A B C D E shifted right; D E pushed past col 79 are fine (80-wide)
        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph);
        Assert.AreEqual((int)'A', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Ich_AtEndOfLine_InsertsBlanks()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");
        writer.Feed("\x1b[1;5H");  // cursor at col 4 (last col)
        writer.Feed("\x1b[@");     // ICH 1 (default)

        // 'E' pushed off right edge (lost), blank at col 4
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', surface[2, 0].Glyph);
        Assert.AreEqual((int)'D', surface[3, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Ich_InsertedBlanks_UseCurrentBackground()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;2H");      // cursor at col 1
        _writer.Feed("\x1b[41m");       // red background
        _writer.Feed("\x1b[1@");        // ICH 1

        Assert.AreEqual(AnsiRed, _surface[1, 0].Background);
    }

    // ── DCH (Delete Character, CSI P) ──

    [TestMethod]
    public void Dch_DeleteOne_ShiftsCharsLeft()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");
        writer.Feed("\x1b[1;2H");  // cursor at col 1
        writer.Feed("\x1b[1P");    // DCH 1

        // 'B' deleted, C D E shift left, blank fills from right
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[1, 0].Glyph);
        Assert.AreEqual((int)'D', surface[2, 0].Glyph);
        Assert.AreEqual((int)'E', surface[3, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Dch_DeleteN_NCharsRemoved()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");
        writer.Feed("\x1b[1;1H");  // cursor at col 0
        writer.Feed("\x1b[3P");    // DCH 3

        Assert.AreEqual((int)'D', surface[0, 0].Glyph);
        Assert.AreEqual((int)'E', surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[2, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[3, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Dch_DeleteMoreThanRemaining_ClearsToEnd()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");
        writer.Feed("\x1b[1;3H");  // cursor at col 2
        writer.Feed("\x1b[99P");   // DCH 99

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[2, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[3, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[4, 0].Glyph);
    }

    // ── IL (Insert Line, CSI L) ──

    [TestMethod]
    public void Il_InsertOneLine_ShiftsLinesDown()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;1H");  // cursor at row 1 (1-based row 2)
        writer.Feed("\x1b[L");     // IL 1

        // Row 0 unchanged (A's), row 1 is now blank, B's shifted to row 2, etc.
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 1].Glyph);
        Assert.AreEqual((int)'B', surface[0, 2].Glyph);
        Assert.AreEqual((int)'C', surface[0, 3].Glyph);
        Assert.AreEqual((int)'D', surface[0, 4].Glyph);
        // E's pushed past bottom are lost
    }

    [TestMethod]
    public void Il_AtScrollTop_TopLineBlank()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[1;1H");  // cursor at row 0
        writer.Feed("\x1b[L");     // IL 1

        Assert.AreEqual(' ', (char)surface[0, 0].Glyph);
        Assert.AreEqual((int)'A', surface[0, 1].Glyph);
        Assert.AreEqual((int)'B', surface[0, 2].Glyph);
        // C's pushed past bottom are lost
    }

    [TestMethod]
    public void Il_LinesPastScrollBottom_AreLost()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[1;1H");
        writer.Feed("\x1b[2L");    // IL 2

        // 2 blank lines at top, only A row remains at bottom; B and C lost
        Assert.AreEqual(' ', (char)surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 1].Glyph);
        Assert.AreEqual((int)'A', surface[0, 2].Glyph);
    }

    [TestMethod]
    public void Il_WithNonDefaultScrollRegion()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;4r");  // scroll region rows 2-4 (1-based → rows 1-3 in 0-based)
        writer.Feed("\x1b[2;1H");  // cursor at row 1 (0-based), inside scroll region
        writer.Feed("\x1b[L");     // IL 1

        // Row 0 (A's) untouched, row 4 (E's) untouched (outside region)
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 1].Glyph);   // new blank line
        Assert.AreEqual((int)'B', surface[0, 2].Glyph);    // shifted down
        Assert.AreEqual((int)'C', surface[0, 3].Glyph);    // shifted down; D lost
        Assert.AreEqual((int)'E', surface[0, 4].Glyph);    // outside region, untouched
    }

    // ── DL (Delete Line, CSI M) ──

    [TestMethod]
    public void Dl_DeleteOneLine_ShiftsLinesUp()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;1H");  // cursor at row 1
        writer.Feed("\x1b[M");     // DL 1

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', surface[0, 2].Glyph);
        Assert.AreEqual((int)'E', surface[0, 3].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 4].Glyph);   // blank at bottom
    }

    [TestMethod]
    public void Dl_AtScrollTop_ContentShiftsUp()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[1;1H");
        writer.Feed("\x1b[M");     // DL 1

        Assert.AreEqual((int)'B', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 2].Glyph);
    }

    [TestMethod]
    public void Dl_WithNonDefaultScrollRegion()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;4r");  // scroll region rows 2-4 (1-based → 1-3 0-based)
        writer.Feed("\x1b[2;1H");  // cursor at row 1 (inside region)
        writer.Feed("\x1b[M");     // DL 1

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);    // outside region, untouched
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);    // shifted up
        Assert.AreEqual((int)'D', surface[0, 2].Glyph);    // shifted up
        Assert.AreEqual(' ', (char)surface[0, 3].Glyph);   // blank at scroll bottom
        Assert.AreEqual((int)'E', surface[0, 4].Glyph);    // outside region, untouched
    }

    // ── SU (Scroll Up, CSI S) ──

    [TestMethod]
    public void Su_ScrollUp1_ContentShiftsUp()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[1S");    // SU 1

        Assert.AreEqual((int)'B', surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 2].Glyph);
    }

    [TestMethod]
    public void Su_ScrollUpN_ContentShiftsUpN()
    {
        var surface = new SadConsole.CellSurface(5, 4);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD");
        writer.Feed("\x1b[2S");    // SU 2

        Assert.AreEqual((int)'C', surface[0, 0].Glyph);
        Assert.AreEqual((int)'D', surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 2].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 3].Glyph);
    }

    // ── SD (Scroll Down, CSI T) ──

    [TestMethod]
    public void Sd_ScrollDown1_ContentShiftsDown()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[1T");    // SD 1

        Assert.AreEqual(' ', (char)surface[0, 0].Glyph);
        Assert.AreEqual((int)'A', surface[0, 1].Glyph);
        Assert.AreEqual((int)'B', surface[0, 2].Glyph);
        // C's pushed off bottom, lost
    }

    [TestMethod]
    public void Sd_ScrollDownN_ContentShiftsDownN()
    {
        var surface = new SadConsole.CellSurface(5, 4);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD");
        writer.Feed("\x1b[2T");    // SD 2

        Assert.AreEqual(' ', (char)surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 1].Glyph);
        Assert.AreEqual((int)'A', surface[0, 2].Glyph);
        Assert.AreEqual((int)'B', surface[0, 3].Glyph);
    }

    // ── ECH (Erase Character, CSI X) ──

    [TestMethod]
    public void Ech_EraseOne_BlanksAtCursor_CursorStays()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;2H");  // cursor at col 1
        _writer.Feed("\x1b[1X");    // ECH 1

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[2, 0].Glyph);
        // Cursor should NOT move
        Assert.AreEqual(1, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void Ech_EraseN_NBlanked()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;1H");  // cursor at col 0
        _writer.Feed("\x1b[3X");    // ECH 3

        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph);
        Assert.AreEqual((int)'D', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'E', _surface[4, 0].Glyph);
        // Cursor should NOT move
        Assert.AreEqual(0, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void Ech_UsesCurrentBackground()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;1H");
        _writer.Feed("\x1b[41m");   // red background
        _writer.Feed("\x1b[2X");    // ECH 2

        Assert.AreEqual(AnsiRed, _surface[0, 0].Background);
        Assert.AreEqual(AnsiRed, _surface[1, 0].Background);
    }

    // ── REP (Repeat, CSI b) ──

    [TestMethod]
    public void Rep_RepeatLastChar_NTimes()
    {
        _writer.Feed("X");
        _writer.Feed("\x1b[4b");   // REP 4: repeat 'X' four more times

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[2, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'X', _surface[4, 0].Glyph);
        // Cursor should have advanced
        Assert.AreEqual(5, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void Rep_AdvancesCursor()
    {
        _writer.Feed("A");
        _writer.Feed("\x1b[2b");   // REP 2

        Assert.AreEqual(3, _writer.State.CursorColumn);
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'A', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'A', _surface[2, 0].Glyph);
    }

    [TestMethod]
    public void Rep_WorksWithAutoWrap()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("Z");
        writer.Feed("\x1b[6b");    // REP 6: repeat 'Z' 6 more times → should wrap

        // Row 0: Z Z Z Z Z
        for (int x = 0; x < 5; x++)
            Assert.AreEqual((int)'Z', surface[x, 0].Glyph);

        // Row 1: Z Z (wrapped)
        Assert.AreEqual((int)'Z', surface[0, 1].Glyph);
        Assert.AreEqual((int)'Z', surface[1, 1].Glyph);
    }

    // ══════════════════════════════════════════════════════════════
    //  Tab Stop Commands
    // ══════════════════════════════════════════════════════════════

    // ── CHT (Cursor Forward Tab, CSI I) ──

    [TestMethod]
    public void Cht_Forward1Tab_DefaultStops()
    {
        // Default tab stops every 8 columns; cursor at col 0 → col 8
        _writer.Feed("\x1b[I");    // CHT 1
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[8, 0].Glyph);
    }

    [TestMethod]
    public void Cht_ForwardNTabs()
    {
        // 2 tabs from col 0: col 0 → col 8 → col 16
        _writer.Feed("\x1b[2I");   // CHT 2
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[16, 0].Glyph);
    }

    [TestMethod]
    public void Cht_ClampsAtRightEdge()
    {
        // Forward 99 tab stops → clamp at right edge
        _writer.Feed("\x1b[99I");

        Assert.IsTrue(_writer.State.CursorColumn <= _surface.Width - 1);
    }

    // ── CBT (Cursor Backward Tab, CSI Z) ──

    [TestMethod]
    public void Cbt_Backward1Tab()
    {
        // Start at col 10, back 1 tab → col 8
        _writer.Feed("\x1b[1;11H"); // cursor at col 10 (1-based col 11)
        _writer.Feed("\x1b[Z");     // CBT 1
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[8, 0].Glyph);
    }

    [TestMethod]
    public void Cbt_BackwardNTabs()
    {
        // Start at col 20, back 2 tabs → col 8 (col 20 → 16 → 8)
        _writer.Feed("\x1b[1;21H"); // cursor at col 20 (1-based col 21)
        _writer.Feed("\x1b[2Z");    // CBT 2
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[8, 0].Glyph);
    }

    [TestMethod]
    public void Cbt_ClampsAtColumnZero()
    {
        _writer.Feed("ABC");        // cursor at col 3
        _writer.Feed("\x1b[99Z");   // CBT 99

        Assert.AreEqual(0, _writer.State.CursorColumn);
    }

    // ── TBC (Tab Clear, CSI g) ──

    [TestMethod]
    public void Tbc_ClearCurrentColumn_TabSkipsIt()
    {
        // Move to col 8 (a default tab stop), clear it
        _writer.Feed("\x1b[1;9H");  // cursor at col 8 (1-based col 9)
        _writer.Feed("\x1b[0g");    // TBC 0: clear tab at current column

        // Now tab from col 0 should skip col 8 and go to col 16
        _writer.Feed("\x1b[1;1H");  // back to col 0
        _writer.Feed("\x1b[I");     // CHT 1
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[16, 0].Glyph);
    }

    [TestMethod]
    public void Tbc_ClearAllTabs_TabDoesNotMove()
    {
        _writer.Feed("\x1b[3g");    // TBC 3: clear all tab stops

        // HT from col 0 should go to right edge (no stops to hit)
        _writer.Feed("\t");

        Assert.AreEqual(_surface.Width - 1, _writer.State.CursorColumn);
    }

    // ══════════════════════════════════════════════════════════════
    //  DEC Modes + Scroll Margins
    // ══════════════════════════════════════════════════════════════

    // ── DECSTBM (Set Scroll Region, CSI r) ──

    [TestMethod]
    public void Decstbm_SetsScrollRegion()
    {
        // ESC[5;20r → scroll region rows 5-20 (1-based → 4-19 in 0-based)
        _writer.Feed("\x1b[5;20r");

        Assert.AreEqual(4, _writer.State.ScrollTop);
        Assert.AreEqual(19, _writer.State.ScrollBottom);
    }

    [TestMethod]
    public void Decstbm_Default_FullScreen()
    {
        // Set a custom region, then reset to full screen
        _writer.Feed("\x1b[5;20r");
        _writer.Feed("\x1b[r");     // default: full screen

        Assert.AreEqual(0, _writer.State.ScrollTop);
        Assert.AreEqual(_surface.Height - 1, _writer.State.ScrollBottom);
    }

    [TestMethod]
    public void Decstbm_CursorMovesToHome()
    {
        // After setting scroll region, cursor should move to home
        _writer.Feed("\x1b[10;10H"); // move cursor away
        _writer.Feed("\x1b[5;20r");  // set scroll region

        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);
    }

    [TestMethod]
    public void Decstbm_ScrollOperationsRespectRegion()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;4r");   // scroll region rows 2-4 (1-based → rows 1-3 0-based)

        // Now scroll up within the region
        writer.Feed("\x1b[1S");     // SU 1

        // Row 0 (A's) untouched (above region)
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        // Within region: B scrolled out, C and D shift up
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', surface[0, 2].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 3].Glyph);
        // Row 4 (E's) untouched (below region)
        Assert.AreEqual((int)'E', surface[0, 4].Glyph);
    }

    [TestMethod]
    public void Decstbm_LfAtScrollBottom_ScrollsWithinRegion()
    {
        var surface = new SadConsole.CellSurface(5, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC\nDDDDD\nEEEEE");
        writer.Feed("\x1b[2;4r");  // scroll region rows 2-4 (1-based → 1-3 0-based)
        writer.Feed("\x1b[4;1H");  // cursor at row 3 (scroll bottom, 1-based row 4)
        writer.Feed("\n");          // LF at scroll bottom → scroll within region

        // Row 0 (A's) and row 4 (E's) untouched
        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'E', surface[0, 4].Glyph);
        // Within region: content shifted up
        Assert.AreEqual((int)'C', surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', surface[0, 2].Glyph);
        Assert.AreEqual(' ', (char)surface[0, 3].Glyph);
    }

    // ── DECSET/DECRST — Origin Mode (mode 6) ──

    [TestMethod]
    public void OriginMode_Enable_CupRelativeToScrollRegion()
    {
        _writer.Feed("\x1b[5;20r");   // scroll region rows 5-20 (0-based: 4-19)
        _writer.Feed("\x1b[?6h");     // enable origin mode

        // CUP 1;1 should position at scroll region top (row 4, col 0)
        _writer.Feed("\x1b[1;1H");
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[0, 4].Glyph);
    }

    [TestMethod]
    public void OriginMode_Disable_CupIsAbsolute()
    {
        _writer.Feed("\x1b[5;20r");
        _writer.Feed("\x1b[?6h");     // enable
        _writer.Feed("\x1b[?6l");     // disable

        // CUP 1;1 should position at absolute top-left (row 0, col 0)
        _writer.Feed("\x1b[1;1H");
        _writer.Feed("X");

        Assert.AreEqual((int)'X', _surface[0, 0].Glyph);
    }

    [TestMethod]
    public void OriginMode_CursorClampedToScrollRegion()
    {
        _writer.Feed("\x1b[5;10r");   // scroll region rows 5-10 (0-based: 4-9)
        _writer.Feed("\x1b[?6h");     // enable origin mode

        // Try to move cursor far beyond scroll region
        _writer.Feed("\x1b[99;1H");

        // Cursor should be clamped to scroll bottom (row 9)
        Assert.AreEqual(9, _writer.State.CursorRow);
    }

    // ── DECSET/DECRST — Auto-wrap (mode 7) ──

    [TestMethod]
    public void AutoWrap_EnabledByDefault()
    {
        Assert.IsTrue(_writer.State.AutoWrap);
    }

    [TestMethod]
    public void AutoWrap_Enable_ConfirmsDefault()
    {
        _writer.Feed("\x1b[?7h");   // enable auto-wrap

        Assert.IsTrue(_writer.State.AutoWrap);
    }

    [TestMethod]
    public void AutoWrap_Disable_NoWrap()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("\x1b[?7l");    // disable auto-wrap
        writer.Feed("ABCDEFGH");    // writing past right margin

        // Last column should have the last char written (overwriting)
        Assert.AreEqual((int)'H', surface[4, 0].Glyph);
        // Should NOT have wrapped to row 1
        Assert.AreEqual(0, writer.State.CursorRow);
    }

    [TestMethod]
    public void AutoWrap_Disable_CharsOverwriteAtRightMargin()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("\x1b[?7l");    // disable auto-wrap
        writer.Feed("ABCDE");
        writer.Feed("XY");         // these overwrite at col 4

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', surface[2, 0].Glyph);
        Assert.AreEqual((int)'D', surface[3, 0].Glyph);
        Assert.AreEqual((int)'Y', surface[4, 0].Glyph);
    }

    // ── DECSET/DECRST — Cursor Visibility (mode 25) ──

    [TestMethod]
    public void CursorVisibility_Show()
    {
        _writer.Feed("\x1b[?25l");  // hide cursor
        _writer.Feed("\x1b[?25h");  // show cursor

        Assert.IsTrue(_writer.Cursor.IsVisible);
    }

    [TestMethod]
    public void CursorVisibility_Hide()
    {
        _writer.Feed("\x1b[?25l");  // hide cursor

        Assert.IsFalse(_writer.Cursor.IsVisible);
    }

    [TestMethod]
    public void CursorVisibility_StateProperty()
    {
        _writer.Feed("\x1b[?25l");
        Assert.IsFalse(_writer.State.CursorVisible);

        _writer.Feed("\x1b[?25h");
        Assert.IsTrue(_writer.State.CursorVisible);
    }

    // ── Saved Cursor with Modes ──

    [TestMethod]
    public void SavedCursor_PreservesOriginMode()
    {
        _writer.Feed("\x1b[5;20r");   // set scroll region
        _writer.Feed("\x1b[?6h");     // enable origin mode
        _writer.Feed("\x1b" + "7");   // DECSC: save cursor

        _writer.Feed("\x1b[?6l");     // disable origin mode
        Assert.IsFalse(_writer.State.OriginMode);

        _writer.Feed("\x1b" + "8");   // DECRC: restore cursor
        Assert.IsTrue(_writer.State.OriginMode);
    }

    // ══════════════════════════════════════════════════════════════
    //  Integration / Edge Cases
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Integration_IchDch_RoundTrip()
    {
        // Insert then delete should return to original state
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;3H");  // cursor at col 2
        _writer.Feed("\x1b[2@");    // ICH 2
        _writer.Feed("\x1b[2P");    // DCH 2

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[2, 0].Glyph);
        Assert.AreEqual((int)'D', _surface[3, 0].Glyph);
        Assert.AreEqual((int)'E', _surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Integration_ScrollRegion_ThenFillWithText()
    {
        var surface = new SadConsole.CellSurface(3, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("\x1b[2;4r");   // scroll region rows 2-4 (0-based: 1-3)
        writer.Feed("\x1b[2;1H");   // cursor at row 1 (0-based)
        writer.Feed("AAA\nBBB\nCCC\nDDD");  // write 4 lines in 3-line region → triggers scroll

        // Row 0 should be untouched (blank, above region)
        Assert.AreEqual(0, surface[0, 0].Glyph);
        // Row 4 should be untouched (blank, below region)
        Assert.AreEqual(0, surface[0, 4].Glyph);
    }

    [TestMethod]
    public void Integration_DecstbmReset_ResetsScrollRegion()
    {
        _writer.Feed("\x1b[5;20r");
        _writer.Feed("\x1b[r");

        Assert.AreEqual(0, _writer.State.ScrollTop);
        Assert.AreEqual(24, _writer.State.ScrollBottom);
    }

    [TestMethod]
    public void Ech_Default_ErasesOneChar()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;3H");  // cursor at col 2
        _writer.Feed("\x1b[X");     // ECH with no param → default 1

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph);
        Assert.AreEqual((int)'D', _surface[3, 0].Glyph);
    }

    [TestMethod]
    public void Ich_Default_InsertsOneChar()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;3H");  // cursor at col 2
        _writer.Feed("\x1b[@");     // ICH with no param → default 1

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[1, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[2, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[3, 0].Glyph);
    }

    [TestMethod]
    public void Dch_Default_DeletesOneChar()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;2H");  // cursor at col 1
        _writer.Feed("\x1b[P");     // DCH default 1

        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'C', _surface[1, 0].Glyph);
    }

    [TestMethod]
    public void Cht_Default_ForwardOneTab()
    {
        // CHT with no param → default 1 tab stop forward
        _writer.Feed("\x1b[I");
        Assert.AreEqual(8, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void Cbt_Default_BackwardOneTab()
    {
        // Move to col 10, then CBT with no param → back to col 8
        _writer.Feed("\x1b[1;11H");
        _writer.Feed("\x1b[Z");
        Assert.AreEqual(8, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void Su_Default_ScrollUpOne()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[S");     // SU default 1

        Assert.AreEqual((int)'B', surface[0, 0].Glyph);
    }

    [TestMethod]
    public void Sd_Default_ScrollDownOne()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("AAAAA\nBBBBB\nCCCCC");
        writer.Feed("\x1b[T");     // SD default 1

        Assert.AreEqual(' ', (char)surface[0, 0].Glyph);
        Assert.AreEqual((int)'A', surface[0, 1].Glyph);
    }
}
