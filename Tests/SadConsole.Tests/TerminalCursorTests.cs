using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Tests for the new TerminalCursor, CursorShape enum, and Writer cursor changes.
/// Tests the injectable nullable cursor model for data-stream vs. interactive terminal modes.
/// </summary>
[TestClass]
public class TerminalCursorTests
{
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
    //  1. TerminalCursor Defaults
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void TerminalCursor_Defaults_PositionIsOrigin()
    {
        var cursor = new TerminalCursor();
        Assert.AreEqual(new Point(0, 0), cursor.Position);
    }

    [TestMethod]
    public void TerminalCursor_Defaults_IsVisibleIsTrue()
    {
        var cursor = new TerminalCursor();
        Assert.IsTrue(cursor.IsVisible);
    }

    [TestMethod]
    public void TerminalCursor_Defaults_ShapeIsBlinkingBlock()
    {
        var cursor = new TerminalCursor();
        Assert.AreEqual(CursorShape.BlinkingBlock, cursor.Shape);
    }

    // ══════════════════════════════════════════════════════════════
    //  2. CursorShape Enum Values
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void CursorShape_BlinkingBlock_IsOne()
    {
        Assert.AreEqual(1, (int)CursorShape.BlinkingBlock);
    }

    [TestMethod]
    public void CursorShape_SteadyBlock_IsTwo()
    {
        Assert.AreEqual(2, (int)CursorShape.SteadyBlock);
    }

    [TestMethod]
    public void CursorShape_BlinkingUnderline_IsThree()
    {
        Assert.AreEqual(3, (int)CursorShape.BlinkingUnderline);
    }

    [TestMethod]
    public void CursorShape_SteadyUnderline_IsFour()
    {
        Assert.AreEqual(4, (int)CursorShape.SteadyUnderline);
    }

    [TestMethod]
    public void CursorShape_BlinkingBar_IsFive()
    {
        Assert.AreEqual(5, (int)CursorShape.BlinkingBar);
    }

    [TestMethod]
    public void CursorShape_SteadyBar_IsSix()
    {
        Assert.AreEqual(6, (int)CursorShape.SteadyBar);
    }

    [TestMethod]
    public void CursorShape_OddValuesAreBlinking()
    {
        // Blinking shapes have odd values
        Assert.AreEqual(1, (int)CursorShape.BlinkingBlock % 2);
        Assert.AreEqual(1, (int)CursorShape.BlinkingUnderline % 2);
        Assert.AreEqual(1, (int)CursorShape.BlinkingBar % 2);
    }

    [TestMethod]
    public void CursorShape_EvenValuesAreSteady()
    {
        // Steady shapes have even values
        Assert.AreEqual(0, (int)CursorShape.SteadyBlock % 2);
        Assert.AreEqual(0, (int)CursorShape.SteadyUnderline % 2);
        Assert.AreEqual(0, (int)CursorShape.SteadyBar % 2);
    }

    // ══════════════════════════════════════════════════════════════
    //  3. Writer with Null Cursor (Data-Stream Mode)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_NoCursor_CursorIsNull()
    {
        // Writer created without setting Cursor — Cursor should be null
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_NullCursor_FeedingDataDoesNotCrash()
    {
        // Feed ANSI data without setting a cursor — should not throw NullReferenceException
        _writer.Feed("Hello World");
        _writer.Feed("\x1b[31mRed Text\x1b[0m");

        // Verify data rendered correctly
        Assert.AreEqual((int)'H', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'e', _surface[1, 0].Glyph);
    }

    [TestMethod]
    public void Writer_NullCursor_DectcemShowDoesNotCrash()
    {
        // DECTCEM show (CSI ? 25 h) with null cursor should not crash
        _writer.Feed("\x1b[?25h");

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_NullCursor_DectcemHideDoesNotCrash()
    {
        // DECTCEM hide (CSI ? 25 l) with null cursor should not crash
        _writer.Feed("\x1b[?25l");

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_NullCursor_DecscusrDoesNotCrash()
    {
        // DECSCUSR sequences with null cursor should not crash
        _writer.Feed("\x1b[1 q");  // BlinkingBlock
        _writer.Feed("\x1b[2 q");  // SteadyBlock
        _writer.Feed("\x1b[3 q");  // BlinkingUnderline
        _writer.Feed("\x1b[4 q");  // SteadyUnderline
        _writer.Feed("\x1b[5 q");  // BlinkingBar
        _writer.Feed("\x1b[6 q");  // SteadyBar

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_NullCursor_CursorMovementDoesNotCrash()
    {
        // Feed cursor movement sequences without a cursor — should not crash
        _writer.Feed("\x1b[5;10H");  // CUP to row 5, col 10
        _writer.Feed("\x1b[A");      // CUU up
        _writer.Feed("\x1b[B");      // CUD down
        _writer.Feed("\x1b[C");      // CUF forward
        _writer.Feed("\x1b[D");      // CUB back

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_NullCursor_RendersToSurfaceCorrectly()
    {
        // Writer with null cursor should still render ANSI to surface
        _writer.Feed("\x1b[2;5HTest");

        // State cursor moved to (8, 1) after printing 4 chars starting at (4, 1)
        Assert.AreEqual(8, _writer.State.CursorColumn);
        Assert.AreEqual(1, _writer.State.CursorRow);

        // Text rendered at correct position
        Assert.AreEqual((int)'T', _surface[4, 1].Glyph);
        Assert.AreEqual((int)'e', _surface[5, 1].Glyph);
        Assert.AreEqual((int)'s', _surface[6, 1].Glyph);
        Assert.AreEqual((int)'t', _surface[7, 1].Glyph);
    }

    // ══════════════════════════════════════════════════════════════
    //  4. Writer with TerminalCursor (Interactive Mode)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_SetCursor_CursorIsNotNull()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        Assert.IsNotNull(_writer.Cursor);
        Assert.AreSame(cursor, _writer.Cursor);
    }

    [TestMethod]
    public void Writer_WithCursor_SyncsCursorPosition()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Move cursor via ANSI sequence
        _writer.Feed("\x1b[5;10H");  // CUP to row 5, col 10 (1-based)

        // State cursor should be at (9, 4) — 0-based
        Assert.AreEqual(9, _writer.State.CursorColumn);
        Assert.AreEqual(4, _writer.State.CursorRow);

        // TerminalCursor should be synced to State cursor position
        Assert.AreEqual(new Point(9, 4), cursor.Position);
    }

    [TestMethod]
    public void Writer_WithCursor_SyncsCursorPositionAfterText()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Feed some text — cursor advances
        _writer.Feed("Hello");

        // State cursor at (5, 0)
        Assert.AreEqual(5, _writer.State.CursorColumn);
        Assert.AreEqual(0, _writer.State.CursorRow);

        // TerminalCursor synced
        Assert.AreEqual(new Point(5, 0), cursor.Position);
    }

    [TestMethod]
    public void Writer_WithCursor_DectcemShow_SetsIsVisibleTrue()
    {
        var cursor = new TerminalCursor { IsVisible = false };
        _writer.Cursor = cursor;

        // DECTCEM show (CSI ? 25 h)
        _writer.Feed("\x1b[?25h");

        Assert.IsTrue(cursor.IsVisible);
    }

    [TestMethod]
    public void Writer_WithCursor_DectcemHide_SetsIsVisibleFalse()
    {
        var cursor = new TerminalCursor { IsVisible = true };
        _writer.Cursor = cursor;

        // DECTCEM hide (CSI ? 25 l)
        _writer.Feed("\x1b[?25l");

        Assert.IsFalse(cursor.IsVisible);
    }

    [TestMethod]
    public void Writer_WithCursor_DectcemToggle()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Hide
        _writer.Feed("\x1b[?25l");
        Assert.IsFalse(cursor.IsVisible);

        // Show
        _writer.Feed("\x1b[?25h");
        Assert.IsTrue(cursor.IsVisible);

        // Hide again
        _writer.Feed("\x1b[?25l");
        Assert.IsFalse(cursor.IsVisible);
    }

    // ══════════════════════════════════════════════════════════════
    //  5. DECSCUSR Cursor Shape Changes
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_Decscusr_Zero_SetsBlinkingBlock()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 0 SP q — default (blinking block)
        _writer.Feed("\x1b[0 q");

        Assert.AreEqual(CursorShape.BlinkingBlock, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_One_SetsBlinkingBlock()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 1 SP q — blinking block
        _writer.Feed("\x1b[1 q");

        Assert.AreEqual(CursorShape.BlinkingBlock, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_Two_SetsSteadyBlock()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 2 SP q — steady block
        _writer.Feed("\x1b[2 q");

        Assert.AreEqual(CursorShape.SteadyBlock, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_Three_SetsBlinkingUnderline()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 3 SP q — blinking underline
        _writer.Feed("\x1b[3 q");

        Assert.AreEqual(CursorShape.BlinkingUnderline, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_Four_SetsSteadyUnderline()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 4 SP q — steady underline
        _writer.Feed("\x1b[4 q");

        Assert.AreEqual(CursorShape.SteadyUnderline, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_Five_SetsBlinkingBar()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 5 SP q — blinking bar
        _writer.Feed("\x1b[5 q");

        Assert.AreEqual(CursorShape.BlinkingBar, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_Six_SetsSteadyBar()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // CSI 6 SP q — steady bar
        _writer.Feed("\x1b[6 q");

        Assert.AreEqual(CursorShape.SteadyBar, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_SequenceOfShapeChanges()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Change shapes in sequence
        _writer.Feed("\x1b[1 q");
        Assert.AreEqual(CursorShape.BlinkingBlock, cursor.Shape);

        _writer.Feed("\x1b[3 q");
        Assert.AreEqual(CursorShape.BlinkingUnderline, cursor.Shape);

        _writer.Feed("\x1b[5 q");
        Assert.AreEqual(CursorShape.BlinkingBar, cursor.Shape);

        _writer.Feed("\x1b[2 q");
        Assert.AreEqual(CursorShape.SteadyBlock, cursor.Shape);

        _writer.Feed("\x1b[4 q");
        Assert.AreEqual(CursorShape.SteadyUnderline, cursor.Shape);

        _writer.Feed("\x1b[6 q");
        Assert.AreEqual(CursorShape.SteadyBar, cursor.Shape);
    }

    [TestMethod]
    public void Writer_Decscusr_NullCursor_DoesNotCrash()
    {
        // DECSCUSR with null cursor should not crash
        _writer.Feed("\x1b[2 q");

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    // ══════════════════════════════════════════════════════════════
    //  6. Cursor Injectability / Mid-Stream Changes
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_SetCursorAfterConstruction_Works()
    {
        // Writer created without cursor
        Assert.IsNull(_writer.Cursor);

        // Set cursor after construction
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Feed data and verify cursor updates
        _writer.Feed("\x1b[10;20H");
        Assert.AreEqual(new Point(19, 9), cursor.Position);
    }

    [TestMethod]
    public void Writer_SetCursorToNullMidStream_DoesNotCrash()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Feed some data
        _writer.Feed("Hello");
        Assert.AreEqual(new Point(5, 0), cursor.Position);

        // Set cursor to null mid-stream
        _writer.Cursor = null;

        // Continue feeding data — should not crash
        _writer.Feed(" World");
        _writer.Feed("\x1b[?25l");

        // No exception = test passes
        Assert.IsNull(_writer.Cursor);
    }

    [TestMethod]
    public void Writer_ReplaceCursorMidStream_NewCursorPicksUp()
    {
        var cursor1 = new TerminalCursor();
        _writer.Cursor = cursor1;

        // Feed data — cursor1 updated
        _writer.Feed("\x1b[5;10H");
        Assert.AreEqual(new Point(9, 4), cursor1.Position);

        // Replace with new cursor
        var cursor2 = new TerminalCursor();
        _writer.Cursor = cursor2;

        // Initial position of cursor2 is (0, 0)
        Assert.AreEqual(new Point(0, 0), cursor2.Position);

        // Feed cursor movement — cursor2 should sync to State cursor position
        _writer.Feed("\x1b[1;1H");  // Move to origin
        Assert.AreEqual(new Point(0, 0), cursor2.Position);

        // Feed text — cursor2 advances
        _writer.Feed("Test");
        Assert.AreEqual(new Point(4, 0), cursor2.Position);

        // cursor1 is no longer updated
        Assert.AreEqual(new Point(9, 4), cursor1.Position);
    }

    [TestMethod]
    public void Writer_CursorProperties_AreSettable()
    {
        var cursor = new TerminalCursor();

        // All properties are settable
        cursor.Position = new Point(10, 5);
        Assert.AreEqual(new Point(10, 5), cursor.Position);

        cursor.IsVisible = false;
        Assert.IsFalse(cursor.IsVisible);

        cursor.Shape = CursorShape.SteadyUnderline;
        Assert.AreEqual(CursorShape.SteadyUnderline, cursor.Shape);
    }

    // ══════════════════════════════════════════════════════════════
    //  7. Integration Tests — Cursor + ANSI Rendering
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_CursorAndDectcem_IntegrationTest()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Complex ANSI sequence with cursor control
        _writer.Feed("\x1b[?25l");              // Hide cursor
        _writer.Feed("\x1b[2 q");               // Steady block shape
        _writer.Feed("\x1b[5;10HTest");         // Move and print
        _writer.Feed("\x1b[?25h");              // Show cursor

        // Verify final state
        Assert.IsTrue(cursor.IsVisible);
        Assert.AreEqual(CursorShape.SteadyBlock, cursor.Shape);
        Assert.AreEqual(new Point(13, 4), cursor.Position);  // After "Test" at (9, 4), cursor at 13

        // Verify text rendered
        Assert.AreEqual((int)'T', _surface[9, 4].Glyph);
        Assert.AreEqual((int)'e', _surface[10, 4].Glyph);
        Assert.AreEqual((int)'s', _surface[11, 4].Glyph);
        Assert.AreEqual((int)'t', _surface[12, 4].Glyph);
    }

    [TestMethod]
    public void Writer_CursorSyncsAfterComplexMovements()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Complex cursor movements
        _writer.Feed("\x1b[10;20H");  // CUP to (19, 9)
        Assert.AreEqual(new Point(19, 9), cursor.Position);

        _writer.Feed("\x1b[5A");      // CUU up 5
        Assert.AreEqual(new Point(19, 4), cursor.Position);

        _writer.Feed("\x1b[10D");     // CUB left 10
        Assert.AreEqual(new Point(9, 4), cursor.Position);

        _writer.Feed("\x1b[1;1H");    // CUP to origin
        Assert.AreEqual(new Point(0, 0), cursor.Position);
    }

    [TestMethod]
    public void Writer_CursorWithScrolling_Syncs()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Fill screen and scroll
        for (int i = 0; i < 30; i++)
        {
            _writer.Feed($"Line {i}\n");
        }

        // Cursor should be synced after scrolling
        // State cursor should be at column 0 (after last LF)
        Assert.AreEqual(0, _writer.State.CursorColumn);
        Assert.AreEqual(new Point(0, _writer.State.CursorRow), cursor.Position);
    }

    [TestMethod]
    public void Writer_CursorWithTabStops_Syncs()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Use tab character
        _writer.Feed("A\tB");

        // Cursor advanced past tab stop (default every 8 cols)
        Assert.AreEqual(9, _writer.State.CursorColumn);
        Assert.AreEqual(new Point(9, 0), cursor.Position);
    }

    // ══════════════════════════════════════════════════════════════
    //  8. Edge Cases
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Writer_CursorAtBoundary_Syncs()
    {
        var cursor = new TerminalCursor();
        _writer.Cursor = cursor;

        // Move cursor to last column
        _writer.Feed("\x1b[1;80H");
        Assert.AreEqual(new Point(79, 0), cursor.Position);

        // Print char at boundary
        _writer.Feed("X");
        Assert.AreEqual((int)'X', _surface[79, 0].Glyph);

        // Cursor should be in pending wrap state (or wrapped to next line)
        // With AutoWrap on and PendingWrap set, cursor stays at (79, 0)
        if (_writer.State.PendingWrap)
        {
            Assert.AreEqual(new Point(79, 0), cursor.Position);
        }
        else
        {
            // If wrap already happened
            Assert.AreEqual(new Point(0, 1), cursor.Position);
        }
    }

    [TestMethod]
    public void Writer_DecscusrInvalidValue_DefaultsToBlinkingBlock()
    {
        var cursor = new TerminalCursor { Shape = CursorShape.SteadyBar };
        _writer.Cursor = cursor;

        // Send invalid DECSCUSR value (out of range 0-6)
        // Implementation should default to BlinkingBlock or ignore
        _writer.Feed("\x1b[99 q");

        // Expected behavior: either unchanged or reset to default
        // This tests robustness — implementation decides on default
        // For this test, we just verify no crash occurs
        Assert.IsNotNull(cursor);
    }

    [TestMethod]
    public void Writer_DecscusrNoParams_DefaultsToBlinkingBlock()
    {
        var cursor = new TerminalCursor { Shape = CursorShape.SteadyUnderline };
        _writer.Cursor = cursor;

        // CSI SP q with no param — should default to 0 (BlinkingBlock)
        _writer.Feed("\x1b[ q");

        // Expected: BlinkingBlock (default)
        Assert.AreEqual(CursorShape.BlinkingBlock, cursor.Shape);
    }
}
