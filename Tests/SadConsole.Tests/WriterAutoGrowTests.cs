using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadConsole.Tests;

/// <summary>
/// Tests for Writer auto-grow functionality when surface implements ICellSurfaceResize.
/// </summary>
[TestClass]
public class WriterAutoGrowTests
{
    private SadConsole.CellSurface _surface;
    private Writer _writer;

    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
        _surface = new SadConsole.CellSurface(80, 5); // small surface to trigger scrolling quickly
        _writer = new Writer(_surface, GameHost.Instance.EmbeddedFont);
        _writer.AutoGrow = true;
    }

    // ──────────────────────────────────────────────
    // 1. Auto-grow basics
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AutoGrow_SurfaceGrowsWhenScrollWouldOccur()
    {
        // 5-row surface. Write 6 lines → should grow to 6 rows.
        _writer.Feed("1\n2\n3\n4\n5\n6");

        Assert.AreEqual(6, _surface.Height, "Surface should have grown to 6 rows");
    }

    [TestMethod]
    public void AutoGrow_ContentPreservedAfterGrow()
    {
        _writer.Feed("A\nB\nC\nD\nE\nF");

        // All lines should be intact — no scrolling
        Assert.AreEqual((int)'A', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', _surface[0, 1].Glyph);
        Assert.AreEqual((int)'C', _surface[0, 2].Glyph);
        Assert.AreEqual((int)'D', _surface[0, 3].Glyph);
        Assert.AreEqual((int)'E', _surface[0, 4].Glyph);
        Assert.AreEqual((int)'F', _surface[0, 5].Glyph);
    }

    [TestMethod]
    public void AutoGrow_CursorPositionCorrectAfterGrow()
    {
        _writer.Feed("1\n2\n3\n4\n5\n6");

        Assert.AreEqual(5, _writer.State.CursorRow); // Row 5 (0-based, in 6-row surface)
        Assert.AreEqual(1, _writer.State.CursorColumn);
    }

    [TestMethod]
    public void AutoGrow_ViewSizeUnchanged()
    {
        int originalViewHeight = _surface.ViewHeight;
        int originalViewWidth = _surface.ViewWidth;

        _writer.Feed("1\n2\n3\n4\n5\n6\n7\n8");

        Assert.AreEqual(originalViewWidth, _surface.ViewWidth);
        Assert.AreEqual(originalViewHeight, _surface.ViewHeight);
    }

    [TestMethod]
    public void AutoGrow_MultipleGrows()
    {
        // Write 10 lines on a 5-row surface
        _writer.Feed("1\n2\n3\n4\n5\n6\n7\n8\n9\nA");

        Assert.AreEqual(10, _surface.Height, "Surface should have grown to 10 rows");
        Assert.AreEqual((int)'1', _surface[0, 0].Glyph);
        Assert.AreEqual(9, _writer.State.CursorRow);
    }

    // ──────────────────────────────────────────────
    // 2. Auto-grow disabled
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AutoGrow_Disabled_SurfaceDoesNotGrow()
    {
        _writer.AutoGrow = false;
        _writer.Feed("1\n2\n3\n4\n5\n6");

        Assert.AreEqual(5, _surface.Height, "Surface should NOT grow when AutoGrow is false");
    }

    [TestMethod]
    public void AutoGrow_Disabled_ContentScrollsNormally()
    {
        _writer.AutoGrow = false;
        _writer.Feed("A\nB\nC\nD\nE\nF");

        // Line 'A' should have scrolled off; 'B' should now be at row 0
        Assert.AreEqual((int)'B', _surface[0, 0].Glyph);
        Assert.AreEqual((int)'F', _surface[0, 4].Glyph);
    }

    // ──────────────────────────────────────────────
    // 3. State consistency after grow
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AutoGrow_ScrollBottomUpdated()
    {
        _writer.Feed("1\n2\n3\n4\n5\n6");

        Assert.AreEqual(5, _writer.State.ScrollBottom, "ScrollBottom should track the new last row");
    }

    [TestMethod]
    public void AutoGrow_HeightUpdated()
    {
        _writer.Feed("1\n2\n3\n4\n5\n6\n7");

        Assert.AreEqual(7, _writer.State.Height);
    }

    // ──────────────────────────────────────────────
    // 4. Line wrap triggers auto-grow
    // ──────────────────────────────────────────────

    [TestMethod]
    public void AutoGrow_WrapInducedScroll_GrowsSurface()
    {
        var surface = new SadConsole.CellSurface(5, 2);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.AutoGrow = true;

        // Fill row 0 (5 chars), wrap to row 1 (5 chars), wrap again → would scroll
        writer.Feed("AAAAABBBBBCCCCC");

        Assert.IsTrue(surface.Height >= 3, $"Surface should have grown to at least 3 rows, was {surface.Height}");
    }
}
