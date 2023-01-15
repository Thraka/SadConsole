using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace SadConsole.Tests;

public partial class CellSurface
{
    #region Test Data
    // Want to test shift values both < axis / 2 and > axis / 2, to ensure no wrapping/optimization issues.
    // Also should be composed of at least one odd number.  We also want to test one value that is exactly shift
    // count.
    //
    // The values here are derived from the values specified in CellSurface.Editor.ShiftRows, so if those tests
    // meet the criteria, so too should these.

    public static IEnumerable<(int shiftAmount, bool wrap)> ShiftInputs
        => s_shiftValuesCol
            .Combinate(new[] { true, false });

    public static IEnumerable<(int shiftAmount, bool wrap)> ShiftInputsWithNeg
        => s_shiftValuesColWithNeg
            .Combinate(new[] { true, false });
    #endregion

    #region Shift Surface Vertically One Direction
    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputs))]
    public void ShiftConsoleDown(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftDown(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (shiftAmount != surface.Height)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedVertically(surface, shiftAmount, wrap);
    }

    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputs))]
    public void ShiftConsoleUp(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftUp(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (shiftAmount != surface.Height)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedVertically(surface, -shiftAmount, wrap);
    }
    #endregion

    #region Shift Surface Vertically Both Directions
    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputsWithNeg))]
    public void ShiftConsoleDownBothDirs(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftDown(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (Math.Abs(shiftAmount) != surface.Height)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedVertically(surface, shiftAmount, wrap);
    }

    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputsWithNeg))]
    public void ShiftConsoleUpBothDirs(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftUp(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (Math.Abs(shiftAmount) != surface.Height)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedVertically(surface, -shiftAmount, wrap);
    }
    #endregion

    #region Shift Surface Horizontally One Direction
    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputs))]
    public void ShiftConsoleRight(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftRight(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (shiftAmount != surface.Width)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedHorizontally(surface, shiftAmount, wrap);
    }

    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputs))]
    public void ShiftConsoleLeft(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftLeft(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (shiftAmount != surface.Width)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedHorizontally(surface, -shiftAmount, wrap);
    }

    #endregion

    #region Shift Surface Horizontally Both Directions
    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputsWithNeg))]
    public void ShiftConsoleRightBothDirs(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftRight(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (Math.Abs(shiftAmount) != surface.Width)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedHorizontally(surface, shiftAmount, wrap);
    }

    [TestMethod]
    [BetterDynamicData(nameof(ShiftInputsWithNeg))]
    public void ShiftConsoleLeftBothDirs(int shiftAmount, bool wrap)
    {
        // Create test surface
        var surface = CreateShiftableCellSurfaceForEntireSurface();

        // Shift with some helpful before/after output
        PrintSurfaceGlyphs(surface, "Before:");
        surface.ShiftLeft(shiftAmount, wrap);
        PrintSurfaceGlyphs(surface, "After:");

        // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
        // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
        if (Math.Abs(shiftAmount) != surface.Width)
            Assert.IsTrue(surface.IsDirty);

        // Verify shift result
        AssertHasShiftedHorizontally(surface, -shiftAmount, wrap);
    }
    #endregion

    #region Shift Test Helpers
    private static SadConsole.CellSurface CreateShiftableCellSurfaceForEntireSurface()
    {
        // Create surface and set cells for testing
        var surface = new SadConsole.CellSurface(SurfaceWidth, SurfaceHeight);

        // Number it with first cell in col glyph 1, second glyph 2, etc.
        foreach (var pos in surface.Positions())
            surface[pos].CopyAppearanceFrom(GetShiftCellForEntireSurface(pos.ToIndex(surface.Width)));

        // Set IsDirty to false because we want to test that certain functions set it to true, and we're not
        // actually rendering it so it won't matter
        surface.IsDirty = false;

        return surface;
    }

    // Computes a ColoredGlyph with unique values for its appearance-related fields, given an ID. This ID
    // should be the "index" value of that position, eg. the result of point.ToIndex(surface.Width).
    private static ColoredGlyph GetShiftCellForEntireSurface(int positionId)
    {
        int id = positionId + 1;
        var colorId = new Color(id, id, id);
        var mirrorId = s_mirrorValues[positionId % s_mirrorValues.Length];
        var glyph = new ColoredGlyph
        {
            Glyph = id,
            Background = colorId,
            Foreground = colorId,
            Decorators = new[] { new CellDecorator(colorId, id, mirrorId) },
            Mirror = mirrorId,
        };
        return glyph;
    }

    // Checks that a surface has been shifted as specified, assuming the cells were generated via GetShiftCellFor.
    // Positive values check for shift down, negative values shift up
    private static void AssertHasShiftedVertically(ICellSurface surface, int shiftAmount, bool wrap)
    {
        // Generate blank glyph appropriate for the surface we're checking
        ColoredGlyph blankGlyph = new ColoredGlyph
        {
            Glyph = surface.DefaultGlyph,
            Background = surface.DefaultBackground,
            Foreground = surface.DefaultForeground,
            Decorators = Array.Empty<CellDecorator>(),
            Mirror = Mirror.None
        };

        Assert.AreEqual(Math.Abs(shiftAmount), shiftAmount > 0 ? surface.TimesShiftedDown : surface.TimesShiftedUp);
        Assert.AreEqual(0, shiftAmount > 0 ? surface.TimesShiftedUp : surface.TimesShiftedDown);

        foreach (var (x, y) in surface.Positions())
        {
            int oldY = y - shiftAmount;
            if (wrap) oldY = WrapAround(oldY, surface.Height);
            var expectedGlyph = oldY >= 0 && oldY < surface.Height || wrap ? GetShiftCellForEntireSurface(Point.ToIndex(x, oldY, surface.Width)) : blankGlyph;
            Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
        }
    }

    // Checks that a surface has been shifted as specified, assuming the cells were generated via GetShiftCellFor.
    // Positive values check for shift right, negative values shift left
    private static void AssertHasShiftedHorizontally(ICellSurface surface, int shiftAmount, bool wrap)
    {
        // Generate blank glyph appropriate for the surface we're checking
        ColoredGlyph blankGlyph = new ColoredGlyph
        {
            Glyph = surface.DefaultGlyph,
            Background = surface.DefaultBackground,
            Foreground = surface.DefaultForeground,
            Decorators = Array.Empty<CellDecorator>(),
            Mirror = Mirror.None
        };

        Assert.AreEqual(Math.Abs(shiftAmount), shiftAmount > 0 ? surface.TimesShiftedRight : surface.TimesShiftedLeft);
        Assert.AreEqual(0, shiftAmount > 0 ? surface.TimesShiftedLeft : surface.TimesShiftedRight);

        foreach (var (x, y) in surface.Positions())
        {
            int oldX = x - shiftAmount;
            if (wrap) oldX = WrapAround(oldX, surface.Width);
            var expectedGlyph = oldX >= 0 && oldX < surface.Width || wrap ? GetShiftCellForEntireSurface(Point.ToIndex(oldX, y, surface.Width)) : blankGlyph;
            Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
        }
    }
    #endregion
}
