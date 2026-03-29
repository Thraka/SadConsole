using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;

namespace SadConsole.Tests;

/// <summary>
/// Tests for Terminal.Measurer — lightweight measurement without surface rendering.
/// </summary>
[TestClass]
public class MeasurerTests
{
    private Measurer _measurer;

    [TestInitialize]
    public void Setup()
    {
        _measurer = new Measurer(80, 25);
    }

    // ──────────────────────────────────────────────
    // 1. Basic cursor tracking
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Initial_CursorAtOrigin()
    {
        Assert.AreEqual(0, _measurer.CursorRow);
        Assert.AreEqual(0, _measurer.CursorColumn);
        Assert.AreEqual(0, _measurer.ScrollCount);
    }

    [TestMethod]
    public void Print_AdvancesCursor()
    {
        _measurer.Feed("Hello");

        Assert.AreEqual(0, _measurer.CursorRow);
        Assert.AreEqual(5, _measurer.CursorColumn);
    }

    [TestMethod]
    public void Print_LineFeed_MovesDown()
    {
        _measurer.Feed("AB\nCD");

        // Default Implicit: LF = CR+LF
        Assert.AreEqual(1, _measurer.CursorRow);
        Assert.AreEqual(2, _measurer.CursorColumn);
    }

    [TestMethod]
    public void Print_StrictLineFeed_NoCarriageReturn()
    {
        _measurer.LineFeeds = LineFeedMode.Strict;
        _measurer.Feed("AB\nCD");

        Assert.AreEqual(1, _measurer.CursorRow);
        Assert.AreEqual(4, _measurer.CursorColumn); // AB(col2) + LF(col stays 2) + CD(col4)
    }

    // ──────────────────────────────────────────────
    // 2. Auto-wrap
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Wrap_AtEndOfLine_WrapsToNextRow()
    {
        var m = new Measurer(5, 3);
        m.Feed("ABCDE"); // fills row 0 completely, pending wrap
        m.Feed("F");     // triggers wrap to row 1

        Assert.AreEqual(1, m.CursorRow);
        Assert.AreEqual(1, m.CursorColumn); // F printed at (0,1), cursor advances to col 1
    }

    [TestMethod]
    public void Wrap_Disabled_StaysAtLastColumn()
    {
        var m = new Measurer(5, 3);
        m.AutoWrap = false;
        m.Feed("ABCDEFGH");

        Assert.AreEqual(0, m.CursorRow); // Never wraps
    }

    // ──────────────────────────────────────────────
    // 3. Scroll counting
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Scroll_LineFeedAtBottom_IncrementsScrollCount()
    {
        var m = new Measurer(80, 3); // 3-row surface
        m.Feed("A\nB\nC\nD"); // 4 lines → needs 1 scroll

        Assert.AreEqual(1, m.ScrollCount);
        Assert.AreEqual(4, m.TotalHeight); // 3 + 1
    }

    [TestMethod]
    public void Scroll_MultipleScrolls_CountsCorrectly()
    {
        var m = new Measurer(80, 3);
        // Row 0: line 1, Row 1: line 2, Row 2: line 3, then 7 more lines = 7 scrolls
        m.Feed("1\n2\n3\n4\n5\n6\n7\n8\n9\n10");

        Assert.AreEqual(7, m.ScrollCount);
        Assert.AreEqual(10, m.TotalHeight);
    }

    [TestMethod]
    public void Scroll_NoScroll_WhenContentFits()
    {
        var m = new Measurer(80, 25);
        m.Feed("Line 1\nLine 2\nLine 3");

        Assert.AreEqual(0, m.ScrollCount);
        Assert.AreEqual(25, m.TotalHeight);
    }

    [TestMethod]
    public void Scroll_WrapCausesScroll()
    {
        var m = new Measurer(5, 2); // 5 cols, 2 rows
        m.Feed("ABCDE"); // fills row 0
        m.Feed("FGHIJ"); // wraps to row 1, fills row 1
        m.Feed("K");     // wraps, would scroll

        Assert.AreEqual(1, m.ScrollCount);
        Assert.AreEqual(3, m.TotalHeight);
    }

    // ──────────────────────────────────────────────
    // 4. CSI cursor movement
    // ──────────────────────────────────────────────

    [TestMethod]
    public void CsiCUP_MovesCursor()
    {
        _measurer.Feed("\x1b[5;10H"); // CUP row 5, col 10

        Assert.AreEqual(4, _measurer.CursorRow);  // 1-based → 0-based
        Assert.AreEqual(9, _measurer.CursorColumn);
    }

    [TestMethod]
    public void CsiCUU_MovesUp()
    {
        _measurer.Feed("\x1b[10;1H"); // Start at row 10
        _measurer.Feed("\x1b[3A");    // Move up 3

        Assert.AreEqual(6, _measurer.CursorRow); // 9 - 3 = 6
    }

    [TestMethod]
    public void CsiCUD_MovesDown()
    {
        _measurer.Feed("\x1b[5B"); // Move down 5

        Assert.AreEqual(5, _measurer.CursorRow);
    }

    [TestMethod]
    public void CsiCHA_SetsColumn()
    {
        _measurer.Feed("ABCDE");
        _measurer.Feed("\x1b[1G"); // CHA to column 1

        Assert.AreEqual(0, _measurer.CursorColumn);
    }

    [TestMethod]
    public void CsiCNL_NextLineAndColumn0()
    {
        _measurer.Feed("ABCDE");
        _measurer.Feed("\x1b[2E"); // CNL — down 2 lines, column 0

        Assert.AreEqual(2, _measurer.CursorRow);
        Assert.AreEqual(0, _measurer.CursorColumn);
    }

    [TestMethod]
    public void CsiSU_ExplicitScrollUp_CountsScroll()
    {
        _measurer.Feed("\x1b[3S"); // Scroll up 3 lines

        Assert.AreEqual(3, _measurer.ScrollCount);
    }

    // ──────────────────────────────────────────────
    // 5. ESC sequences
    // ──────────────────────────────────────────────

    [TestMethod]
    public void EscNEL_NewLine()
    {
        _measurer.Feed("AB");
        _measurer.Feed("\x1b" + "E"); // NEL (CR+LF)

        Assert.AreEqual(1, _measurer.CursorRow);
        Assert.AreEqual(0, _measurer.CursorColumn);
    }

    [TestMethod]
    public void EscRI_ReverseIndex()
    {
        _measurer.Feed("\x1b[5;1H"); // Row 5
        _measurer.Feed("\x1b" + "M");  // RI — up 1

        Assert.AreEqual(3, _measurer.CursorRow); // 4 - 1 = 3
    }

    [TestMethod]
    public void EscRIS_ResetsMeasurer()
    {
        _measurer.Feed("ABCDE\n\n\n");
        _measurer.Feed("\x1b" + "c"); // RIS

        Assert.AreEqual(0, _measurer.CursorRow);
        Assert.AreEqual(0, _measurer.CursorColumn);
        Assert.AreEqual(0, _measurer.ScrollCount);
    }

    // ──────────────────────────────────────────────
    // 6. TotalHeight property
    // ──────────────────────────────────────────────

    [TestMethod]
    public void TotalHeight_NoScroll_EqualsViewHeight()
    {
        Assert.AreEqual(25, _measurer.TotalHeight);
    }

    [TestMethod]
    public void TotalHeight_WithScroll_AddsCounts()
    {
        var m = new Measurer(80, 5);
        // 10 lines on a 5-row surface → 5 scrolls
        m.Feed("1\n2\n3\n4\n5\n6\n7\n8\n9\n10");

        Assert.AreEqual(10, m.TotalHeight); // 5 + 5
    }

    // ──────────────────────────────────────────────
    // 7. Reset
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Reset_ClearsAllState()
    {
        _measurer.Feed("Hello\n\n\nWorld");
        _measurer.Reset();

        Assert.AreEqual(0, _measurer.CursorRow);
        Assert.AreEqual(0, _measurer.CursorColumn);
        Assert.AreEqual(0, _measurer.ScrollCount);
        Assert.AreEqual(0, _measurer.MaxRow);
    }

    // ──────────────────────────────────────────────
    // 8. SGR and other non-movement sequences are ignored
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Sgr_DoesNotAffectMeasurement()
    {
        _measurer.Feed("\x1b[31;42mHello"); // Red on green, then text

        Assert.AreEqual(0, _measurer.CursorRow);
        Assert.AreEqual(5, _measurer.CursorColumn);
        Assert.AreEqual(0, _measurer.ScrollCount);
    }

    // ──────────────────────────────────────────────
    // 9. MaxRow tracking
    // ──────────────────────────────────────────────

    [TestMethod]
    public void MaxRow_TracksHighestRow()
    {
        _measurer.Feed("\x1b[20;1H"); // Go to row 20
        _measurer.Feed("\x1b[1;1H");   // Go back to row 1

        Assert.AreEqual(19, _measurer.MaxRow); // 0-based: row 20 = index 19
        Assert.AreEqual(0, _measurer.CursorRow);
    }

    // ──────────────────────────────────────────────
    // 10. Constructor validation
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Constructor_ZeroWidth_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Measurer(0, 25));
    }

    [TestMethod]
    public void Constructor_ZeroHeight_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Measurer(80, 0));
    }
}
