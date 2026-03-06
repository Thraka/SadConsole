using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Integration tests for Terminal.Writer — Phases 3, 9, and 10.
/// Phase 3: Visual SGR rendering (decorators, reverse, blink, italic).
/// Phase 9: OSC palette redefinition (OSC 4, 10, 11).
/// Phase 10: Polish (ED modes, CSI s edge cases, pending wrap clearing).
/// </summary>
[TestClass]
public class TerminalWriterPhase3Tests
{
    private static readonly Color DefaultFg = new Color(170, 170, 170);
    private static readonly Color DefaultBg = new Color(0, 0, 0);
    private static readonly Color AnsiRed = new Color(170, 0, 0);
    private static readonly Color AnsiGreen = new Color(0, 170, 0);
    private static readonly Color AnsiBlue = new Color(0, 0, 170);

    // Decorator glyph constants (from Label.Theme defaults)
    private const int UnderlineGlyph = 95;      // '_' underscore
    private const int StrikethroughGlyph = 196;  // '─' box horizontal

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
    //  PHASE 3 — Visual SGR Rendering
    // ══════════════════════════════════════════════════════════════

    // ── Underline (SGR 4 / SGR 24) ──

    [TestMethod]
    public void Sgr4_Underline_On_CellHasUnderlineDecorator()
    {
        // SGR 4 = underline on, print 'A'
        _writer.Feed("\x1b[4mA");

        var cell = _surface[0, 0];
        Assert.IsNotNull(cell.Decorators, "Cell should have decorators after SGR 4");
        Assert.IsTrue(cell.Decorators.Count > 0, "Decorators list should not be empty");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == UnderlineGlyph),
            $"Cell should have an underline decorator (glyph {UnderlineGlyph})");
    }

    [TestMethod]
    public void Sgr24_Underline_Off_CellHasNoUnderlineDecorator()
    {
        // SGR 4 on, then SGR 24 off, then print
        _writer.Feed("\x1b[4m");   // underline on
        _writer.Feed("\x1b[24m");  // underline off
        _writer.Feed("B");

        var cell = _surface[0, 0];
        if (cell.Decorators != null && cell.Decorators.Count > 0)
        {
            Assert.IsFalse(
                cell.Decorators.Any(d => d.Glyph == UnderlineGlyph),
                "Cell should NOT have underline decorator after SGR 24");
        }
        // null or empty decorators is also valid (no decorator)
    }

    // ── Strikethrough (SGR 9 / SGR 29) ──

    [TestMethod]
    public void Sgr9_Strikethrough_On_CellHasStrikethroughDecorator()
    {
        _writer.Feed("\x1b[9mX");

        var cell = _surface[0, 0];
        Assert.IsNotNull(cell.Decorators, "Cell should have decorators after SGR 9");
        Assert.IsTrue(cell.Decorators.Count > 0, "Decorators list should not be empty");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == StrikethroughGlyph),
            $"Cell should have a strikethrough decorator (glyph {StrikethroughGlyph})");
    }

    [TestMethod]
    public void Sgr29_Strikethrough_Off_CellHasNoStrikethroughDecorator()
    {
        _writer.Feed("\x1b[9m");   // strikethrough on
        _writer.Feed("\x1b[29m");  // strikethrough off
        _writer.Feed("Y");

        var cell = _surface[0, 0];
        if (cell.Decorators != null && cell.Decorators.Count > 0)
        {
            Assert.IsFalse(
                cell.Decorators.Any(d => d.Glyph == StrikethroughGlyph),
                "Cell should NOT have strikethrough decorator after SGR 29");
        }
    }

    // ── Reverse Video (SGR 7 / SGR 27) ──

    [TestMethod]
    public void Sgr7_ReverseVideo_SwapsForegroundAndBackground()
    {
        // Set fg=red bg=blue, then reverse, then print
        _writer.Feed("\x1b[31;44m");  // fg=red (palette 1), bg=blue (palette 4)
        _writer.Feed("\x1b[7m");      // reverse on
        _writer.Feed("R");

        var cell = _surface[0, 0];
        // With reverse: rendered fg = original bg, rendered bg = original fg
        Assert.AreEqual(AnsiBlue, cell.Foreground, "Reverse: foreground should be original background (blue)");
        Assert.AreEqual(AnsiRed, cell.Background, "Reverse: background should be original foreground (red)");
    }

    [TestMethod]
    public void Sgr27_ReverseOff_RestoresNormalColors()
    {
        _writer.Feed("\x1b[31;44m");  // fg=red, bg=blue
        _writer.Feed("\x1b[7m");      // reverse on
        _writer.Feed("A");            // prints reversed
        _writer.Feed("\x1b[27m");     // reverse off
        _writer.Feed("B");            // prints normal

        var cellB = _surface[1, 0];
        Assert.AreEqual(AnsiRed, cellB.Foreground, "After reverse off, fg should be normal (red)");
        Assert.AreEqual(AnsiBlue, cellB.Background, "After reverse off, bg should be normal (blue)");
    }

    [TestMethod]
    public void Sgr7_ReverseVideo_WithDefaultColors()
    {
        // Default fg=170,170,170; bg=0,0,0. Reverse should swap them.
        _writer.Feed("\x1b[7mZ");

        var cell = _surface[0, 0];
        Assert.AreEqual(DefaultBg, cell.Foreground, "Reverse with defaults: fg should become black");
        Assert.AreEqual(DefaultFg, cell.Background, "Reverse with defaults: bg should become light gray");
    }

    // ── Blink (SGR 5) ──

    [TestMethod]
    public void Sgr5_Blink_SetsStateFlag()
    {
        _writer.Feed("\x1b[5m");
        Assert.IsTrue(_writer.State.Blink, "State.Blink should be true after SGR 5");
    }

    [TestMethod]
    public void Sgr25_Blink_Off_ClearsStateFlag()
    {
        _writer.Feed("\x1b[5m");   // blink on
        _writer.Feed("\x1b[25m");  // blink off
        Assert.IsFalse(_writer.State.Blink, "State.Blink should be false after SGR 25");
    }

    // ── Italic (SGR 3) ──

    [TestMethod]
    public void Sgr3_Italic_SetsStateFlag()
    {
        _writer.Feed("\x1b[3m");
        Assert.IsTrue(_writer.State.Italic, "State.Italic should be true after SGR 3");
    }

    [TestMethod]
    public void Sgr23_Italic_Off_ClearsStateFlag()
    {
        _writer.Feed("\x1b[3m");   // italic on
        _writer.Feed("\x1b[23m");  // italic off
        Assert.IsFalse(_writer.State.Italic, "State.Italic should be false after SGR 23");
    }

    // ── Combined attributes ──

    [TestMethod]
    public void CombinedAttributes_UnderlineAndStrikethrough_BothDecoratorsPresent()
    {
        // Turn on underline + strikethrough, print a char
        _writer.Feed("\x1b[4;9mC");

        var cell = _surface[0, 0];
        Assert.IsNotNull(cell.Decorators, "Cell should have decorators with combined attributes");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == UnderlineGlyph),
            "Cell should have underline decorator");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == StrikethroughGlyph),
            "Cell should have strikethrough decorator");
    }

    [TestMethod]
    public void CombinedAttributes_UnderlineStrikethroughReverse_AllApply()
    {
        // Turn on underline + strikethrough + reverse + known colors
        _writer.Feed("\x1b[31;42m");     // fg=red, bg=green
        _writer.Feed("\x1b[4;9;7mD");   // underline + strikethrough + reverse

        var cell = _surface[0, 0];

        // Reverse should swap colors
        Assert.AreEqual(AnsiGreen, cell.Foreground, "Reverse should swap: fg = original bg (green)");
        Assert.AreEqual(AnsiRed, cell.Background, "Reverse should swap: bg = original fg (red)");

        // Decorators should still be present
        Assert.IsNotNull(cell.Decorators, "Decorators should be present with combined attributes");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == UnderlineGlyph),
            "Underline decorator should be present with reverse");
        Assert.IsTrue(
            cell.Decorators.Any(d => d.Glyph == StrikethroughGlyph),
            "Strikethrough decorator should be present with reverse");
    }

    // ── SGR 0 reset ──

    [TestMethod]
    public void Sgr0_ClearsAllDecoratorsAndAttributes()
    {
        // Set underline + strikethrough + italic + blink + reverse
        _writer.Feed("\x1b[4;9;3;5;7m");
        _writer.Feed("A");  // print with all attributes

        // Now reset and print
        _writer.Feed("\x1b[0m");
        _writer.Feed("B");

        // Verify state is cleared
        Assert.IsFalse(_writer.State.Underline, "Underline should be off after SGR 0");
        Assert.IsFalse(_writer.State.Strikethrough, "Strikethrough should be off after SGR 0");
        Assert.IsFalse(_writer.State.Italic, "Italic should be off after SGR 0");
        Assert.IsFalse(_writer.State.Blink, "Blink should be off after SGR 0");
        Assert.IsFalse(_writer.State.Reverse, "Reverse should be off after SGR 0");

        // Verify the 'B' cell has no decorators
        var cellB = _surface[1, 0];
        bool noDecorators = cellB.Decorators == null || cellB.Decorators.Count == 0;
        Assert.IsTrue(noDecorators, "Cell after SGR 0 should have no decorators");
    }

    [TestMethod]
    public void Sgr0_ClearsDecoratorFromPreviousChar()
    {
        // Print underlined char, then reset, print normal char
        _writer.Feed("\x1b[4mU");   // underlined 'U'
        _writer.Feed("\x1b[0mN");   // normal 'N'

        // 'U' at col 0 should have decorator
        var cellU = _surface[0, 0];
        if (cellU.Decorators != null)
        {
            // 'U' should retain its underline decorator
            Assert.IsTrue(
                cellU.Decorators.Any(d => d.Glyph == UnderlineGlyph),
                "'U' should keep its underline decorator");
        }

        // 'N' at col 1 should NOT have decorator
        var cellN = _surface[1, 0];
        bool noDecorators = cellN.Decorators == null || cellN.Decorators.Count == 0;
        Assert.IsTrue(noDecorators, "'N' after SGR 0 should have no decorators");
    }

    // ══════════════════════════════════════════════════════════════
    //  PHASE 9 — OSC Palette Redefinition
    // ══════════════════════════════════════════════════════════════

    // ── OSC 4 — Set palette color ──

    [TestMethod]
    public void Osc4_SetPaletteColor_ChangesSingleEntry()
    {
        // OSC 4;1;rgb:ff/00/ff BEL — set palette index 1 to magenta
        _writer.Feed("\x1b]4;1;rgb:ff/00/ff\x07");

        Color expected = new Color(255, 0, 255);
        Assert.AreEqual(expected, _writer.Palette[1],
            "Palette index 1 should be changed to (255,0,255) after OSC 4");
    }

    [TestMethod]
    public void Osc4_SetPaletteColor_Index0()
    {
        // Set palette index 0 (black) to a custom color
        _writer.Feed("\x1b]4;0;rgb:11/22/33\x07");

        Color expected = new Color(0x11, 0x22, 0x33);
        Assert.AreEqual(expected, _writer.Palette[0],
            "Palette index 0 should be updated by OSC 4");
    }

    [TestMethod]
    public void Osc4_SetPaletteColor_Index255()
    {
        // Set palette index 255 (end of 256-color range)
        _writer.Feed("\x1b]4;255;rgb:aa/bb/cc\x07");

        Color expected = new Color(0xaa, 0xbb, 0xcc);
        Assert.AreEqual(expected, _writer.Palette[255],
            "Palette index 255 should be updated by OSC 4");
    }

    [TestMethod]
    public void Osc4_SetPaletteColor_WithStTerminator()
    {
        // OSC 4 terminated with ST (\x1b\\) instead of BEL
        _writer.Feed("\x1b]4;2;rgb:00/ff/00\x1b\\");

        Color expected = new Color(0, 255, 0);
        Assert.AreEqual(expected, _writer.Palette[2],
            "OSC 4 with ST terminator should update palette");
    }

    [TestMethod]
    public void Osc4_MultipleEntries_SetsBothColors()
    {
        // Some terminals support multiple entries in a single OSC 4:
        //   OSC 4;1;rgb:ff/00/00;2;rgb:00/ff/00 BEL
        _writer.Feed("\x1b]4;1;rgb:ff/00/00;2;rgb:00/ff/00\x07");

        Color expectedRed = new Color(255, 0, 0);
        Color expectedGreen = new Color(0, 255, 0);

        // At minimum, the first entry should be set
        Assert.AreEqual(expectedRed, _writer.Palette[1],
            "First entry in multi-OSC 4 should be set");

        // Second entry: test if multiple entries are supported
        // If not supported, this will still have the old value
        Assert.AreEqual(expectedGreen, _writer.Palette[2],
            "Second entry in multi-OSC 4 should be set (if supported)");
    }

    // ── OSC 10 — Default foreground ──

    [TestMethod]
    public void Osc10_SetDefaultForeground()
    {
        _writer.Feed("\x1b]10;rgb:ff/ff/00\x07");

        Color expected = new Color(255, 255, 0);
        Assert.AreEqual(expected, _writer.State.DefaultForeground,
            "DefaultForeground should be updated by OSC 10");
    }

    [TestMethod]
    public void Osc10_AffectsSubsequentRendering()
    {
        // Change default foreground, then print a char with default color mode
        _writer.Feed("\x1b]10;rgb:ff/ff/00\x07");
        _writer.Feed("A");

        Color expected = new Color(255, 255, 0);
        Assert.AreEqual(expected, _surface[0, 0].Foreground,
            "Character printed after OSC 10 should use new default foreground");
    }

    // ── OSC 11 — Default background ──

    [TestMethod]
    public void Osc11_SetDefaultBackground()
    {
        _writer.Feed("\x1b]11;rgb:00/00/ff\x07");

        Color expected = new Color(0, 0, 255);
        Assert.AreEqual(expected, _writer.State.DefaultBackground,
            "DefaultBackground should be updated by OSC 11");
    }

    [TestMethod]
    public void Osc11_AffectsSubsequentRendering()
    {
        _writer.Feed("\x1b]11;rgb:00/00/ff\x07");
        _writer.Feed("B");

        Color expected = new Color(0, 0, 255);
        Assert.AreEqual(expected, _surface[0, 0].Background,
            "Character printed after OSC 11 should use new default background");
    }

    // ── Palette change affects rendering ──

    [TestMethod]
    public void Osc4_PaletteChange_AffectsSubsequentSgrRendering()
    {
        // Change palette index 1 (red) to a custom magenta
        _writer.Feed("\x1b]4;1;rgb:ff/00/ff\x07");

        // Now print with SGR 31 (foreground = palette index 1)
        _writer.Feed("\x1b[31mA");

        Color expectedMagenta = new Color(255, 0, 255);
        Assert.AreEqual(expectedMagenta, _surface[0, 0].Foreground,
            "SGR 31 after OSC 4 redefining index 1 should use the NEW color");
    }

    [TestMethod]
    public void Osc4_PaletteChange_BackgroundAlsoAffected()
    {
        // Change palette index 4 (blue) to cyan
        _writer.Feed("\x1b]4;4;rgb:00/ff/ff\x07");

        // Print with SGR 44 (background = palette index 4)
        _writer.Feed("\x1b[44mC");

        Color expectedCyan = new Color(0, 255, 255);
        Assert.AreEqual(expectedCyan, _surface[0, 0].Background,
            "SGR 44 after OSC 4 redefining index 4 should use the NEW color");
    }

    // ══════════════════════════════════════════════════════════════
    //  PHASE 10 — Polish
    // ══════════════════════════════════════════════════════════════

    // ── ED modes (CSI J) ──

    [TestMethod]
    public void Ed0_EraseCursorToEnd_ClearsFromCursorToEnd()
    {
        // Fill first two rows
        _writer.Feed("ABCDEFGHIJ");                    // row 0 cols 0-9
        _writer.Feed("\x1b[2;1H");                     // cursor to row 1 col 0
        _writer.Feed("KLMNOPQRST");                    // row 1 cols 0-9

        // Move cursor to row 0, col 5
        _writer.Feed("\x1b[1;6H");
        _writer.Feed("\x1b[0J");                       // ED 0 — erase from cursor to end

        // Cols 0-4 on row 0 should be preserved
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'E', _surface[4, 0].Glyph);

        // Col 5 on row 0 should be cleared
        Assert.AreEqual(' ', (char)_surface[5, 0].Glyph);

        // Row 1 should be fully cleared
        Assert.AreEqual(' ', (char)_surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)_surface[9, 1].Glyph);
    }

    [TestMethod]
    public void Ed1_EraseStartToCursor_ClearsFromStartToCursor()
    {
        // Fill first two rows
        _writer.Feed("ABCDEFGHIJ");
        _writer.Feed("\x1b[2;1H");
        _writer.Feed("KLMNOPQRST");

        // Move cursor to row 1, col 4
        _writer.Feed("\x1b[2;5H");
        _writer.Feed("\x1b[1J");                       // ED 1 — erase from start to cursor

        // Row 0 should be fully cleared
        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[9, 0].Glyph);

        // Row 1 cols 0-4 should be cleared
        Assert.AreEqual(' ', (char)_surface[0, 1].Glyph);
        Assert.AreEqual(' ', (char)_surface[4, 1].Glyph);

        // Row 1 cols 5+ should be preserved
        Assert.AreEqual((int)'P', _surface[5, 1].Glyph);
    }

    [TestMethod]
    public void Ed2_EraseEntireDisplay_ClearsAll()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[2;1H");
        _writer.Feed("FGHIJ");

        _writer.Feed("\x1b[2J");                       // ED 2 — erase entire display

        for (int col = 0; col < 10; col++)
        {
            Assert.AreEqual(' ', (char)_surface[col, 0].Glyph, $"Row 0 col {col} should be cleared");
            Assert.AreEqual(' ', (char)_surface[col, 1].Glyph, $"Row 1 col {col} should be cleared");
        }
    }

    [TestMethod]
    public void Ed3_AliasForEd2_ClearsEntireDisplay()
    {
        _writer.Feed("HELLO");

        _writer.Feed("\x1b[3J");                       // ED 3 — treat as ED 2

        for (int col = 0; col < 5; col++)
        {
            Assert.AreEqual(' ', (char)_surface[col, 0].Glyph, $"Col {col} should be cleared by ED 3");
        }
    }

    [TestMethod]
    public void Ed0_DoesNotMoveCursor()
    {
        _writer.Feed("ABCDE");
        _writer.Feed("\x1b[1;3H");   // cursor at row 0, col 2
        _writer.Feed("\x1b[0J");

        Assert.AreEqual(0, _writer.State.CursorRow, "ED 0 should not change cursor row");
        Assert.AreEqual(2, _writer.State.CursorColumn, "ED 0 should not change cursor column");
    }

    // ── CSI s with no params — Save cursor position ──

    [TestMethod]
    public void CsiS_NoParams_SaveAndRestoreCursorPosition()
    {
        _writer.Feed("\x1b[3;10H");   // move to row 2, col 9
        _writer.Feed("\x1b[s");       // save cursor (no params)
        _writer.Feed("\x1b[1;1H");    // move to home
        _writer.Feed("\x1b[u");       // restore cursor

        Assert.AreEqual(2, _writer.State.CursorRow, "Cursor row should be restored");
        Assert.AreEqual(9, _writer.State.CursorColumn, "Cursor column should be restored");
    }

    // ── CSI s with params — should be ignored or handled gracefully ──

    [TestMethod]
    public void CsiS_WithParams_HandledGracefully()
    {
        // CSI 1;10 s — DECSLRM (set left/right margin) is not supported
        // Should either be ignored entirely or still treated as save-cursor
        _writer.Feed("\x1b[5;20H");   // move to row 4, col 19
        _writer.Feed("\x1b[1;10s");   // CSI with params — DECSLRM not supported

        // If treated as save, a subsequent restore should work.
        // If ignored, subsequent restore returns to default (0,0).
        // Either way, the writer should not crash.
        _writer.Feed("\x1b[1;1H");    // move to home
        _writer.Feed("\x1b[u");       // restore

        // We verify no crash occurred and cursor is in a valid position
        Assert.IsTrue(_writer.State.CursorRow >= 0 && _writer.State.CursorRow < 25,
            "Cursor row should be valid after CSI s with params");
        Assert.IsTrue(_writer.State.CursorColumn >= 0 && _writer.State.CursorColumn < 80,
            "Cursor column should be valid after CSI s with params");
    }

    // ── Pending wrap cleared by CUP ──

    [TestMethod]
    public void PendingWrap_ClearedByCup_PrintsAtCupPosition()
    {
        // Use a narrow surface to easily reach end of line
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill row 0 completely — cursor at col 4, pending wrap set
        writer.Feed("ABCDE");
        Assert.IsTrue(writer.State.PendingWrap, "Pending wrap should be set after filling row");

        // CUP to row 1, col 2 — should clear pending wrap
        writer.Feed("\x1b[2;3H");

        Assert.IsFalse(writer.State.PendingWrap, "CUP should clear pending wrap");
        Assert.AreEqual(1, writer.State.CursorRow, "Cursor should be at row 1");
        Assert.AreEqual(2, writer.State.CursorColumn, "Cursor should be at col 2");

        // Next print should go at CUP position, NOT wrap to next line
        writer.Feed("X");
        Assert.AreEqual((int)'X', surface[2, 1].Glyph, "Character should print at CUP position");
    }

    // ── Pending wrap cleared by BS ──

    [TestMethod]
    public void PendingWrap_ClearedByBackspace_MovesBack()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill row 0 to trigger pending wrap
        writer.Feed("ABCDE");
        Assert.IsTrue(writer.State.PendingWrap, "Pending wrap should be set");

        // Backspace should clear pending wrap and move cursor back
        writer.Feed("\x08");  // BS

        Assert.IsFalse(writer.State.PendingWrap, "BS should clear pending wrap");
        Assert.AreEqual(3, writer.State.CursorColumn, "Cursor should move back one column");
        Assert.AreEqual(0, writer.State.CursorRow, "Cursor should stay on same row");
    }

    [TestMethod]
    public void PendingWrap_ClearedByBackspace_NextCharOverwrites()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");  // fills row, pending wrap
        writer.Feed("\x08");   // BS — back to col 3
        writer.Feed("Z");      // should overwrite col 3

        Assert.AreEqual((int)'Z', surface[3, 0].Glyph, "BS+print should overwrite at backed-up position");
        Assert.AreEqual(0, writer.State.CursorRow, "Should still be on row 0");
    }

    // ── Pending wrap cleared by tab ──

    [TestMethod]
    public void PendingWrap_ClearedByTab_MovesToNextTabStop()
    {
        // Use a 20-wide surface. Fill first row to trigger pending wrap.
        var surface = new SadConsole.CellSurface(20, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill 20 characters to trigger pending wrap
        writer.Feed("01234567890123456789");
        Assert.IsTrue(writer.State.PendingWrap, "Pending wrap should be set after filling row");

        // Tab should clear pending wrap. Since tabs are C0 controls,
        // OnC0Control sets PendingWrap = false and moves to next tab stop.
        writer.Feed("\t");

        Assert.IsFalse(writer.State.PendingWrap, "Tab should clear pending wrap");
    }

    // ── Pending wrap cleared by CR ──

    [TestMethod]
    public void PendingWrap_ClearedByCr_MovesToCol0()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");  // fills row, pending wrap
        writer.Feed("\r");     // CR

        Assert.IsFalse(writer.State.PendingWrap, "CR should clear pending wrap");
        Assert.AreEqual(0, writer.State.CursorColumn, "CR should move cursor to col 0");
        Assert.AreEqual(0, writer.State.CursorRow, "CR should stay on same row");
    }

    // ── Pending wrap cleared by LF ──

    [TestMethod]
    public void PendingWrap_ClearedByLf_MovesToNextLine()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");  // fills row, pending wrap
        writer.Feed("\n");     // LF (implicit mode = CR+LF)

        Assert.IsFalse(writer.State.PendingWrap, "LF should clear pending wrap");
        Assert.AreEqual(1, writer.State.CursorRow, "LF should move to next row");
        Assert.AreEqual(0, writer.State.CursorColumn, "LF (implicit) should move to col 0");
    }

    // ── Pending wrap cleared by cursor movement sequences ──

    [TestMethod]
    public void PendingWrap_ClearedByCursorUp()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Move to row 1 first, then fill to trigger pending wrap
        writer.Feed("\x1b[2;1H");    // row 1, col 0
        writer.Feed("ABCDE");        // fills row 1, pending wrap

        writer.Feed("\x1b[1A");      // CUU 1 — cursor up

        Assert.IsFalse(writer.State.PendingWrap, "CUU should clear pending wrap");
    }

    [TestMethod]
    public void PendingWrap_ClearedByCursorForward()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");        // fills row, pending wrap
        writer.Feed("\x1b[1C");      // CUF 1 — cursor forward (will clamp)

        Assert.IsFalse(writer.State.PendingWrap, "CUF should clear pending wrap");
    }

    [TestMethod]
    public void PendingWrap_ClearedByCha()
    {
        var surface = new SadConsole.CellSurface(5, 3);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCDE");        // fills row, pending wrap
        writer.Feed("\x1b[1G");      // CHA — cursor to col 0

        Assert.IsFalse(writer.State.PendingWrap, "CHA should clear pending wrap");
        Assert.AreEqual(0, writer.State.CursorColumn, "CHA 1 should move to col 0");
    }

    // ── ED with custom background ──

    [TestMethod]
    public void Ed2_UsesCurrentBackground_ForClearedCells()
    {
        // Set background to blue, then ED 2
        _writer.Feed("\x1b[44m");    // bg=blue
        _writer.Feed("\x1b[2J");     // ED 2

        Assert.AreEqual(AnsiBlue, _surface[0, 0].Background,
            "Cleared cells should use current background color");
        Assert.AreEqual(AnsiBlue, _surface[79, 24].Background,
            "Last cell should also use current background color");
    }

    // ── ED 0 boundary: cursor at end of line ──

    [TestMethod]
    public void Ed0_CursorAtEndOfLine_ClearsRestOfScreen()
    {
        // Write two rows
        _writer.Feed("Line 1 content here\n");
        _writer.Feed("Line 2 content here");

        // Move to end of row 0
        _writer.Feed("\x1b[1;80H");  // row 0, col 79
        _writer.Feed("\x1b[0J");     // ED 0

        // Col 79 row 0 should be cleared
        Assert.AreEqual(' ', (char)_surface[79, 0].Glyph);
        // Row 1 should be fully cleared
        Assert.AreEqual(' ', (char)_surface[0, 1].Glyph);
    }

    // ── ED 1 boundary: cursor at start of line ──

    [TestMethod]
    public void Ed1_CursorAtStartOfLine_ClearsAboveRows()
    {
        _writer.Feed("Row zero text");
        _writer.Feed("\x1b[2;1H");
        _writer.Feed("Row one text");

        // Move to start of row 1
        _writer.Feed("\x1b[2;1H");
        _writer.Feed("\x1b[1J");     // ED 1

        // Row 0 should be fully cleared
        Assert.AreEqual(' ', (char)_surface[0, 0].Glyph);
        Assert.AreEqual(' ', (char)_surface[5, 0].Glyph);

        // Row 1 col 0 should be cleared (cursor is at col 0, ED 1 clears up to and including cursor)
        Assert.AreEqual(' ', (char)_surface[0, 1].Glyph);

        // Row 1 remaining content should be preserved
        Assert.AreEqual((int)'o', _surface[1, 1].Glyph);
    }

    // ── Multiple attributes cleared by SGR 0 ──

    [TestMethod]
    public void Sgr0_ResetsAllSgrAttributes()
    {
        // Set everything
        _writer.Feed("\x1b[1;3;4;5;7;9m");  // bold, italic, underline, blink, reverse, strikethrough

        // Verify all set
        Assert.IsTrue(_writer.State.Bold);
        Assert.IsTrue(_writer.State.Italic);
        Assert.IsTrue(_writer.State.Underline);
        Assert.IsTrue(_writer.State.Blink);
        Assert.IsTrue(_writer.State.Reverse);
        Assert.IsTrue(_writer.State.Strikethrough);

        // Reset
        _writer.Feed("\x1b[0m");

        // Verify all cleared
        Assert.IsFalse(_writer.State.Bold, "Bold should be off after SGR 0");
        Assert.IsFalse(_writer.State.Italic, "Italic should be off after SGR 0");
        Assert.IsFalse(_writer.State.Underline, "Underline should be off after SGR 0");
        Assert.IsFalse(_writer.State.Blink, "Blink should be off after SGR 0");
        Assert.IsFalse(_writer.State.Reverse, "Reverse should be off after SGR 0");
        Assert.IsFalse(_writer.State.Strikethrough, "Strikethrough should be off after SGR 0");
    }

    // ── Concealed text (SGR 8) ──

    [TestMethod]
    public void Sgr8_Concealed_ForegroundEqualsBackground()
    {
        _writer.Feed("\x1b[44m");    // bg=blue
        _writer.Feed("\x1b[8mH");    // concealed + print

        var cell = _surface[0, 0];
        Assert.AreEqual(cell.Background, cell.Foreground,
            "Concealed text: foreground should equal background");
    }

    [TestMethod]
    public void Sgr28_Concealed_Off_RestoredNormalColors()
    {
        _writer.Feed("\x1b[31;44m");   // fg=red, bg=blue
        _writer.Feed("\x1b[8m");       // conceal on
        _writer.Feed("A");
        _writer.Feed("\x1b[28m");      // conceal off
        _writer.Feed("B");

        var cellB = _surface[1, 0];
        Assert.AreEqual(AnsiRed, cellB.Foreground, "After conceal off, fg should be red again");
        Assert.AreEqual(AnsiBlue, cellB.Background, "After conceal off, bg should be blue");
    }

    // ══════════════════════════════════════════════════════════════
    //  PendingWrap Opt-In Clearing Model — Regression Tests
    //  Per ECMA-48 §7.1: Only cursor-moving sequences clear PendingWrap.
    //  Non-cursor-moving sequences (SGR, erase, DEC modes) must NOT.
    // ══════════════════════════════════════════════════════════════

    // ── SGR at column boundary does NOT clear PendingWrap (the reported bug) ──

    [TestMethod]
    public void PendingWrap_SgrAtColumnBoundary_DoesNotClear()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        // Fill row 0 completely with 80 characters — sets PendingWrap
        writer.Feed(new string('A', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set after filling 80-wide row");

        // SGR (set red foreground) — must NOT clear PendingWrap
        writer.Feed("\x1b[31m");
        Assert.IsTrue(writer.State.PendingWrap, "SGR must NOT clear PendingWrap (ECMA-48)");

        // Next printable character should wrap to row 1, col 0
        writer.Feed("X");
        Assert.AreEqual(1, writer.State.CursorRow, "After wrap, cursor should be on row 1");
        Assert.AreEqual(1, writer.State.CursorColumn, "After printing X at col 0, cursor advances to col 1");
        Assert.AreEqual((int)'X', surface[0, 1].Glyph, "X should appear at row 1, col 0 (wrapped)");
    }

    // ── Cursor movement (CUF) DOES clear PendingWrap ──

    [TestMethod]
    public void PendingWrap_CufClearsAtBoundary_80Wide()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('B', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // CUF — cursor forward — must clear PendingWrap
        writer.Feed("\x1b[C");
        Assert.IsFalse(writer.State.PendingWrap, "CUF must clear PendingWrap");
    }

    // ── CUP clears PendingWrap ──

    [TestMethod]
    public void PendingWrap_CupClearsAtBoundary_80Wide()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('C', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // CUP to row 1, col 1 — must clear PendingWrap
        writer.Feed("\x1b[1;1H");
        Assert.IsFalse(writer.State.PendingWrap, "CUP must clear PendingWrap");
        Assert.AreEqual(0, writer.State.CursorRow, "Cursor should be at row 0");
        Assert.AreEqual(0, writer.State.CursorColumn, "Cursor should be at col 0");
    }

    // ── DECTCEM (cursor visibility) does NOT clear PendingWrap ──

    [TestMethod]
    public void PendingWrap_Dectcem_DoesNotClear()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('D', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // DECTCEM — hide cursor — must NOT clear PendingWrap
        writer.Feed("\x1b[?25l");
        Assert.IsTrue(writer.State.PendingWrap, "DECTCEM must NOT clear PendingWrap");
    }

    // ── DECAWM mode toggle does NOT clear PendingWrap ──

    [TestMethod]
    public void PendingWrap_Decawm_DoesNotClear()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('E', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // DECAWM — enable auto-wrap — must NOT clear PendingWrap
        writer.Feed("\x1b[?7h");
        Assert.IsTrue(writer.State.PendingWrap, "DECAWM must NOT clear PendingWrap");
    }

    // ── ECH (erase character) does NOT clear PendingWrap ──

    [TestMethod]
    public void PendingWrap_Ech_DoesNotClear()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('F', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // ECH — erase 1 character — must NOT clear PendingWrap
        writer.Feed("\x1b[1X");
        Assert.IsTrue(writer.State.PendingWrap, "ECH must NOT clear PendingWrap");
    }

    // ── Multiple SGR sequences at boundary preserve PendingWrap ──

    [TestMethod]
    public void PendingWrap_MultipleSgrSequences_PreservesThenWraps()
    {
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('G', 80));
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set");

        // Send multiple SGR sequences — PendingWrap must persist through all
        writer.Feed("\x1b[1m");     // bold
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap must persist after bold SGR");

        writer.Feed("\x1b[31m");    // red foreground
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap must persist after fg SGR");

        writer.Feed("\x1b[44m");    // blue background
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap must persist after bg SGR");

        // Next printable char should wrap to row 1
        writer.Feed("Z");
        Assert.AreEqual(1, writer.State.CursorRow, "After multiple SGRs, char should wrap to row 1");
        Assert.AreEqual((int)'Z', surface[0, 1].Glyph, "Z should be at row 1, col 0");
    }

    // ── b5-ans01.ans integration test — real ANSI art that exposed the bug ──

    [TestMethod]
    public void PendingWrap_B5Ans01_RendersCorrectly()
    {
        // Load the real ANSI art file that exposed the PendingWrap clearing bug.
        // In this file, an SGR sequence appears immediately after the 80th printable
        // character on row 0. Under the old blanket-clearing model, the SGR would
        // clear PendingWrap, causing the 81st character to overwrite col 79 instead
        // of wrapping to row 1.
        string testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "b5-ans01.ans");
        byte[] fileBytes = File.ReadAllBytes(testDataPath);

        // 80-wide surface with generous height for the full art (108 CRLFs + wraps)
        var surface = new SadConsole.CellSurface(80, 200);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.Feed(fileBytes.AsSpan());

        // Anchor 1: The 80th printable character lands at [79, 0].
        // It's byte 0xDF = CP437 glyph 223 (▀, upper half block).
        // Under the bug, the 81st char (glyph 219) would overwrite this cell.
        Assert.AreEqual(223, surface[79, 0].Glyph,
            "Row 0 col 79 should be glyph 223 (upper half block), not overwritten by next char");

        // Anchor 2: The 81st printable character should wrap to [0, 1].
        // It's byte 0xDB = CP437 glyph 219 (█, full block).
        // Under the bug, this char would land at col 79 row 0 instead.
        Assert.AreEqual(219, surface[0, 1].Glyph,
            "Row 1 col 0 should be glyph 219 (full block), wrapped correctly from row 0");

        // Anchor 3: The second logical line (after CRLF) starts at row 2.
        // Its first printable char is byte 0xB2 = CP437 glyph 178 (▓, medium shade).
        // Row 0's 148 printable chars fill rows 0-1 (80+68), then CRLF advances to row 2.
        // Under the bug, accumulated line drift would shift this to a wrong row.
        Assert.AreEqual(178, surface[0, 2].Glyph,
            "Row 2 col 0 should be glyph 178 (medium shade), proving no line drift");
    }

    // ══════════════════════════════════════════════════════════════
    //  CUF resolves PendingWrap at right margin
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void CufAtPendingWrap_ResolvesWrapThenMovesForward()
    {
        // When CUF arrives while PendingWrap is true, the wrap should be
        // resolved first (advance to next row col 0), then CUF moves forward.
        // Without this fix, CUF from col 79 with PendingWrap is a no-op
        // (clamped at right margin), breaking ANSI art that relies on
        // immediate-wrap semantics after filling 80 columns.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('A', 80));   // fills row 0, PendingWrap = true
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set after 80 chars");
        Assert.AreEqual(0, writer.State.CursorRow);

        writer.Feed("\x1b[6C");             // CUF 6

        Assert.IsFalse(writer.State.PendingWrap, "CUF must clear PendingWrap");
        Assert.AreEqual(1, writer.State.CursorRow, "CUF should resolve wrap to next row");
        Assert.AreEqual(6, writer.State.CursorColumn, "CUF 6 should land at col 6 after wrap");
    }

    [TestMethod]
    public void CufAtPendingWrap_PrintsAtCorrectPosition()
    {
        // Verify that characters printed after CUF-during-PendingWrap land
        // at the correct surface cell (not stuck at the right margin).
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('X', 80));   // fills row 0, PendingWrap = true
        writer.Feed("\x1b[6C");             // CUF 6: resolve wrap → row 1, col 6
        writer.Feed("Q");                   // print at row 1, col 6

        Assert.AreEqual((int)'Q', surface[6, 1].Glyph,
            "Character after CUF-during-PendingWrap should land at row 1, col 6");
        Assert.AreEqual(7, writer.State.CursorColumn, "Cursor should advance to col 7");
    }

    [TestMethod]
    public void CufAtPendingWrap_B5Ans01_LineAfterMessagesAligned()
    {
        // Regression test for b5-ans01.ans: after the "Messages" line fills
        // 80 columns (ending at col 79 row 13), a CSI 6C (CUF 6) should
        // resolve the wrap and place subsequent characters starting at col 6
        // on row 14. Without the fix, CUF is clamped at col 79 on row 13.
        string testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData", "b5-ans01.ans");
        byte[] fileBytes = File.ReadAllBytes(testDataPath);

        var surface = new SadConsole.CellSurface(80, 200);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.Feed(fileBytes.AsSpan());

        // After the CUF 6 on row 14, the first printable chars are
        // 0xB1 (177, light shade) at col 6, 0xB2 (178, medium shade) at col 7.
        Assert.AreEqual(177, surface[6, 14].Glyph,
            "Row 14 col 6 should be glyph 177 (light shade) — CUF 6 after wrap");
        Assert.AreEqual(178, surface[7, 14].Glyph,
            "Row 14 col 7 should be glyph 178 (medium shade) — next char after CUF");

        // Columns 0-5 on row 14 should not contain block drawing glyphs since CUF skipped them.
        // (Untouched cells default to glyph 0; erased cells may be space.)
        for (int c = 0; c < 6; c++)
        {
            Assert.IsTrue(surface[c, 14].Glyph < 128,
                $"Row 14 col {c} should not be a high-byte block glyph (CUF 6 skipped these columns)");
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  CHT (CSI I) resolves PendingWrap at right margin
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void ChtAtPendingWrap_ResolvesWrapThenTabsForward()
    {
        // CHT at right margin with PendingWrap: same bug pattern as CUF.
        // NextTabStop(79) returns 79 (no tab stop beyond margin) → no-op.
        // Fix: resolve wrap first (next line col 0), then tab forward.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('A', 80));   // fills row 0, PendingWrap = true
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set after 80 chars");
        Assert.AreEqual(0, writer.State.CursorRow);

        writer.Feed("\x1b[I");              // CHT 1 (one forward tab)

        Assert.IsFalse(writer.State.PendingWrap, "CHT must clear PendingWrap");
        Assert.AreEqual(1, writer.State.CursorRow, "CHT should resolve wrap to next row");
        Assert.AreEqual(8, writer.State.CursorColumn, "CHT 1 from col 0 should land at first tab stop (col 8)");
    }

    [TestMethod]
    public void ChtAtPendingWrap_PrintsAtCorrectPosition()
    {
        // Verify printing after CHT-during-PendingWrap lands at correct cell.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('X', 80));   // fills row 0, PendingWrap = true
        writer.Feed("\x1b[I");              // CHT 1: resolve wrap → row 1, col 8
        writer.Feed("Q");                   // print at row 1, col 8

        Assert.AreEqual((int)'Q', surface[8, 1].Glyph,
            "Character after CHT-during-PendingWrap should land at row 1, col 8");
        Assert.AreEqual(9, writer.State.CursorColumn, "Cursor should advance to col 9");
    }

    [TestMethod]
    public void ChtWithMultipleTabs_ResolvesWrapThenAdvances()
    {
        // CHT 2 at right margin: resolve wrap, then advance 2 tab stops.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('A', 80));   // fills row 0, PendingWrap = true
        writer.Feed("\x1b[2I");             // CHT 2 (two forward tabs)

        Assert.AreEqual(1, writer.State.CursorRow, "CHT should resolve wrap to next row");
        Assert.AreEqual(16, writer.State.CursorColumn, "CHT 2 from col 0 should land at second tab stop (col 16)");
    }

    // ══════════════════════════════════════════════════════════════
    //  C0 HT (0x09) resolves PendingWrap at right margin
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void C0HtAtPendingWrap_ResolvesWrapThenTabsForward()
    {
        // C0 HT (tab character) at right margin with PendingWrap: same pattern.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('A', 80));   // fills row 0, PendingWrap = true
        Assert.IsTrue(writer.State.PendingWrap, "PendingWrap should be set after 80 chars");

        writer.Feed("\t");                  // C0 HT (0x09)

        Assert.IsFalse(writer.State.PendingWrap, "HT must clear PendingWrap");
        Assert.AreEqual(1, writer.State.CursorRow, "HT should resolve wrap to next row");
        Assert.AreEqual(8, writer.State.CursorColumn, "HT from col 0 should land at first tab stop (col 8)");
    }

    [TestMethod]
    public void C0HtAtPendingWrap_PrintsAtCorrectPosition()
    {
        // Verify printing after C0 HT-during-PendingWrap.
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed(new string('X', 80));   // fills row 0, PendingWrap = true
        writer.Feed("\t");                  // HT: resolve wrap → row 1, col 8
        writer.Feed("Z");                   // print at row 1, col 8

        Assert.AreEqual((int)'Z', surface[8, 1].Glyph,
            "Character after HT-during-PendingWrap should land at row 1, col 8");
        Assert.AreEqual(9, writer.State.CursorColumn, "Cursor should advance to col 9");
    }

    [TestMethod]
    public void ChtWithoutPendingWrap_NormalBehavior()
    {
        // CHT without PendingWrap should work normally (no wrap resolution).
        var surface = new SadConsole.CellSurface(80, 5);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        writer.Feed("ABCD");               // cursor at col 4, row 0
        writer.Feed("\x1b[I");              // CHT 1

        Assert.AreEqual(0, writer.State.CursorRow, "Cursor should stay on row 0");
        Assert.AreEqual(8, writer.State.CursorColumn, "CHT 1 from col 4 should land at col 8");
    }

    // ══════════════════════════════════════════════════════════════
    //  Bold + Default Foreground = Bright White (CGA convention)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void BoldWithDefaultForeground_ResolvesBrightWhite()
    {
        // CGA: SGR 0 (reset) + SGR 1 (bold) with no explicit foreground
        // should resolve to bright white (palette 15 = 255,255,255), not gray.
        _writer.Feed("\x1b[0m\x1b[1mA");

        var cell = _surface[0, 0];
        Color brightWhite = new Color(255, 255, 255);
        Assert.AreEqual(brightWhite, cell.Foreground,
            "Bold + default foreground should be bright white (palette 15), not gray");
    }

    [TestMethod]
    public void BoldWithDefaultForeground_B5Ans01Sequence()
    {
        // Exact sequence from b5-ans01.ans after "Your stats":
        // ESC[0m (reset all) → ESC[1;46m (bold + bg cyan) → print
        // Foreground should be bright white, not gray.
        _writer.Feed("\x1b[0m\x1b[1;46mX");

        var cell = _surface[0, 0];
        Color brightWhite = new Color(255, 255, 255);
        Color bgCyan = new Color(0, 170, 170);
        Assert.AreEqual(brightWhite, cell.Foreground,
            "After ESC[0m ESC[1;46m], foreground should be bright white (bold + default)");
        Assert.AreEqual(bgCyan, cell.Background,
            "Background should be cyan (palette 6)");
    }

    [TestMethod]
    public void NoBoldWithDefaultForeground_StaysGray()
    {
        // Without bold, default foreground stays gray (170,170,170).
        _writer.Feed("\x1b[0mA");

        var cell = _surface[0, 0];
        Assert.AreEqual(DefaultFg, cell.Foreground,
            "Default foreground without bold should be gray (170,170,170)");
    }
}
