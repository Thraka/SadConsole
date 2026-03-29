using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Tests for the SadConsole.TerminalConsole class.
/// TerminalConsole : ScreenSurface — a high-level terminal emulator surface
/// that bundles a Writer, TerminalCursor, and TerminalCursorRenderStep.
/// Written against the contract while Roy implements in parallel.
/// </summary>
[TestClass]
public class TerminalConsoleTests
{
    // Default terminal colors — CGA palette
    private static readonly Color DefaultFg = new Color(170, 170, 170); // palette index 7
    private static readonly Color DefaultBg = new Color(0, 0, 0);       // palette index 0

    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
    }

    // ══════════════════════════════════════════════════════════════
    //  1. Construction
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Construction_WidthHeight_Creates()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNotNull(terminal);
    }

    [TestMethod]
    public void Construction_SurfaceWidth_MatchesConstructorArg()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreEqual(80, terminal.Surface.Width);
    }

    [TestMethod]
    public void Construction_SurfaceHeight_MatchesConstructorArg()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreEqual(25, terminal.Surface.Height);
    }

    [TestMethod]
    public void Construction_SmallSize_Creates()
    {
        var terminal = new TerminalConsole(10, 5);
        Assert.AreEqual(10, terminal.Surface.Width);
        Assert.AreEqual(5, terminal.Surface.Height);
    }

    [TestMethod]
    public void Construction_Writer_IsNotNull()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNotNull(terminal.Writer);
    }

    [TestMethod]
    public void Construction_TerminalCursor_IsNotNull()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNotNull(terminal.TerminalCursor);
    }

    [TestMethod]
    public void Construction_WriterCursor_IsSameInstanceAsTerminalCursor()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreSame(terminal.TerminalCursor, terminal.Writer.Cursor);
    }

    [TestMethod]
    public void Construction_WithFont_UsesProvidedFont()
    {
        var font = GameHost.Instance.EmbeddedFont;
        var terminal = new TerminalConsole(80, 25, font);
        Assert.AreSame(font, terminal.Font);
    }

    [TestMethod]
    public void Construction_WithoutFont_UsesDefaultFont()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreSame(GameHost.Instance.DefaultFont, terminal.Font);
    }

    // ══════════════════════════════════════════════════════════════
    //  2. Inheritance
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Inheritance_IsScreenSurface()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsInstanceOfType(terminal, typeof(SadConsole.ScreenSurface));
    }

    [TestMethod]
    public void Inheritance_IsNotConsole()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNotInstanceOfType(terminal, typeof(SadConsole.Console));
    }

    [TestMethod]
    public void Inheritance_IsScreenObject()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsInstanceOfType(terminal, typeof(SadConsole.IScreenObject));
    }

    // ══════════════════════════════════════════════════════════════
    //  3. Feed Convenience Methods
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Feed_String_WritesGlyphsToSurface()
    {
        var terminal = new TerminalConsole(80, 25);
        terminal.Feed("Hello");

        Assert.AreEqual((int)'H', terminal.Surface[0, 0].Glyph);
        Assert.AreEqual((int)'e', terminal.Surface[1, 0].Glyph);
        Assert.AreEqual((int)'l', terminal.Surface[2, 0].Glyph);
        Assert.AreEqual((int)'l', terminal.Surface[3, 0].Glyph);
        Assert.AreEqual((int)'o', terminal.Surface[4, 0].Glyph);
    }

    [TestMethod]
    public void Feed_Bytes_WritesGlyphsToSurface()
    {
        var terminal = new TerminalConsole(80, 25);
        byte[] data = Encoding.UTF8.GetBytes("AB");
        terminal.Feed(data);

        Assert.AreEqual((int)'A', terminal.Surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', terminal.Surface[1, 0].Glyph);
    }

    [TestMethod]
    public void Feed_AnsiColor_AppliesForeground()
    {
        var terminal = new TerminalConsole(80, 25);
        // SGR 31 = red foreground, then print 'X'
        terminal.Feed("\x1b[31mX");

        var cell = terminal.Surface[0, 0];
        Assert.AreEqual((int)'X', cell.Glyph);
        Assert.AreEqual(new Color(170, 0, 0), cell.Foreground); // CGA red
    }

    [TestMethod]
    public void Feed_AnsiCursorMovement_UpdatesTerminalCursorPosition()
    {
        var terminal = new TerminalConsole(80, 25);
        // CUP: move cursor to row 5, col 10 (1-based in ANSI)
        terminal.Feed("\x1b[5;10H");

        Assert.AreEqual(new Point(9, 4), terminal.TerminalCursor.Position);
    }

    [TestMethod]
    public void Feed_Text_AdvancesCursorPosition()
    {
        var terminal = new TerminalConsole(80, 25);
        terminal.Feed("ABC");

        // After printing 3 chars, cursor should be at column 3
        Assert.AreEqual(3, terminal.TerminalCursor.Position.X);
        Assert.AreEqual(0, terminal.TerminalCursor.Position.Y);
    }

    [TestMethod]
    public void Feed_NewLine_MovesCursorDown()
    {
        var terminal = new TerminalConsole(80, 25);
        terminal.Feed("AB\nCD");

        Assert.AreEqual((int)'C', terminal.Surface[0, 1].Glyph);
        Assert.AreEqual((int)'D', terminal.Surface[1, 1].Glyph);
    }

    // ══════════════════════════════════════════════════════════════
    //  4. Cursor Integration
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Cursor_DECTCEM_Show_SetsIsVisibleTrue()
    {
        var terminal = new TerminalConsole(80, 25);
        // First hide, then show to verify the toggle works
        terminal.Feed("\x1b[?25l"); // hide
        Assert.IsFalse(terminal.TerminalCursor.IsVisible);

        terminal.Feed("\x1b[?25h"); // show
        Assert.IsTrue(terminal.TerminalCursor.IsVisible);
    }

    [TestMethod]
    public void Cursor_DECTCEM_Hide_SetsIsVisibleFalse()
    {
        var terminal = new TerminalConsole(80, 25);
        // Cursor starts visible by default
        Assert.IsTrue(terminal.TerminalCursor.IsVisible);

        terminal.Feed("\x1b[?25l"); // hide
        Assert.IsFalse(terminal.TerminalCursor.IsVisible);
    }

    [TestMethod]
    public void Cursor_DECSCUSR_SteadyBlock_SetsShape()
    {
        var terminal = new TerminalConsole(80, 25);
        // DECSCUSR: CSI 2 SP q → steady block
        terminal.Feed("\x1b[2 q");
        Assert.AreEqual(CursorShape.SteadyBlock, terminal.TerminalCursor.Shape);
    }

    [TestMethod]
    public void Cursor_DECSCUSR_BlinkingUnderline_SetsShape()
    {
        var terminal = new TerminalConsole(80, 25);
        // DECSCUSR: CSI 3 SP q → blinking underline
        terminal.Feed("\x1b[3 q");
        Assert.AreEqual(CursorShape.BlinkingUnderline, terminal.TerminalCursor.Shape);
    }

    [TestMethod]
    public void Cursor_DECSCUSR_SteadyBar_SetsShape()
    {
        var terminal = new TerminalConsole(80, 25);
        // DECSCUSR: CSI 6 SP q → steady bar
        terminal.Feed("\x1b[6 q");
        Assert.AreEqual(CursorShape.SteadyBar, terminal.TerminalCursor.Shape);
    }

    [TestMethod]
    public void Cursor_DefaultShape_IsBlinkingBlock()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreEqual(CursorShape.BlinkingBlock, terminal.TerminalCursor.Shape);
    }

    [TestMethod]
    public void Cursor_DefaultVisibility_IsTrue()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsTrue(terminal.TerminalCursor.IsVisible);
    }

    [TestMethod]
    public void Cursor_DefaultPosition_IsOrigin()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreEqual(new Point(0, 0), terminal.TerminalCursor.Position);
    }

    // ══════════════════════════════════════════════════════════════
    //  5. Multiple Instances
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void MultipleInstances_IndependentWriters()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        Assert.AreNotSame(t1.Writer, t2.Writer);
    }

    [TestMethod]
    public void MultipleInstances_IndependentCursors()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        Assert.AreNotSame(t1.TerminalCursor, t2.TerminalCursor);
    }

    [TestMethod]
    public void MultipleInstances_IndependentContent()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        t1.Feed("Hello");
        t2.Feed("World");

        Assert.AreEqual((int)'H', t1.Surface[0, 0].Glyph);
        Assert.AreEqual((int)'W', t2.Surface[0, 0].Glyph);
    }

    [TestMethod]
    public void MultipleInstances_IndependentCursorPosition()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        t1.Feed("\x1b[5;10H"); // move t1 cursor to row 5, col 10
        t2.Feed("\x1b[3;4H");  // move t2 cursor to row 3, col 4

        Assert.AreEqual(new Point(9, 4), t1.TerminalCursor.Position);
        Assert.AreEqual(new Point(3, 2), t2.TerminalCursor.Position);
    }

    // ══════════════════════════════════════════════════════════════
    //  6. Renderer Integration
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Renderer_HasTerminalCursorRenderStep()
    {
        var terminal = new TerminalConsole(80, 25);

        // The renderer should contain a TerminalCursor render step
        Assert.IsTrue(
            terminal.Renderer!.Steps.Any(s => s.Name == Renderers.Constants.RenderStepNames.TerminalCursor),
            "Renderer should contain a TerminalCursorRenderStep");
    }

    // ══════════════════════════════════════════════════════════════
    //  7. Keyboard Focus
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Focus_UseKeyboard_IsTrue()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsTrue(terminal.UseKeyboard, "TerminalConsole should accept keyboard input");
    }

    [TestMethod]
    public void Focus_FocusedMode_IsNotNone()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.AreNotEqual(FocusBehavior.None, terminal.FocusedMode,
            "TerminalConsole should be focusable");
    }
}
