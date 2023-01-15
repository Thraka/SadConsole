using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Diag = System.Diagnostics;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        #region Test Data
        // Width/height values
        private const int SurfaceWidth = 20;
        private const int SurfaceHeight = 10;

        // Test center row/column so artifacts on either side would be easily visible
        private const int ShiftRow = SurfaceHeight / 2;
        private const int ShiftCol = SurfaceWidth / 2;

        // Make sure we test shifting a subset where everything ends up back where it's started.
        private const int SubsetOffsetRow = SurfaceWidth - 3;
        private const int SubsetOffsetCol = SurfaceHeight - 3;

        private static readonly IEnumerable<int> s_shiftValuesRow = new[] { 2, SurfaceWidth - 5, 17, SurfaceWidth };
        private static readonly IEnumerable<int> s_shiftValuesRowWithNeg = new[] { -1, 2, SurfaceWidth - 5, -17};
        private static readonly IEnumerable<(int startingX, int count)> s_rowSubsetValues = new[] { (0, SurfaceWidth), (1, SubsetOffsetRow)};

        private static readonly IEnumerable<int> s_shiftValuesCol = new[] { 2, SurfaceHeight - 3, 8, SurfaceHeight };
        private static readonly IEnumerable<int> s_shiftValuesColWithNeg = new[] { -1, 2, SurfaceHeight - 3, -8 };
        private static readonly IEnumerable<(int startingY, int count)> s_colSubsetValues = new[] { (0, SurfaceHeight), (1, SubsetOffsetCol) };

        // Want to test shift values both < axis / 2 and > axis / 2, to ensure no wrapping/optimization issues.
        // Also should be composed of at least one odd number.  We also want to test one value that is exactly shift
        // count
        public static IEnumerable<(int startingX, int count, int shift, bool wrap)> ShiftInputsRow
            => s_rowSubsetValues
                .Combinate(s_shiftValuesRow)
                .Combinate(new[] { true, false });

        public static IEnumerable<(int startingX, int count, int shift, bool wrap)> ShiftInputsRowWithNeg
            => s_rowSubsetValues
                .Combinate(s_shiftValuesRowWithNeg)
                .Combinate(new[] { true, false });

        public static IEnumerable<(int shift, bool wrap)> ShiftInputsRowWholeRow
            => s_shiftValuesRowWithNeg
                .Combinate(new[] { true, false });


        public static IEnumerable<(int startingX, int count, int shift, bool wrap)> ShiftInputsCol
            => s_colSubsetValues
                .Combinate(s_shiftValuesCol)
                .Combinate(new[] { true, false });

        public static IEnumerable<(int startingX, int count, int shift, bool wrap)> ShiftInputsColWithNeg
            => s_colSubsetValues
                .Combinate(s_shiftValuesColWithNeg)
                .Combinate(new[] { true, false });

        public static IEnumerable<(int shift, bool wrap)> ShiftInputsColWholeCol
            => s_shiftValuesColWithNeg
                .Combinate(new[] { true, false });


        #endregion

        private static readonly Mirror[] s_mirrorValues = LinqExtensions.GetEnumValues<Mirror>().ToArray();

        // A "unit test" that validates that the test data is sufficient to cover some corner cases.
        #region Test Data Validation

        [TestMethod]
        public void TestDataIsValid()
        {
            // Need both halves of the list to be in test cases
            Assert.IsTrue(ShiftInputsRow.Any(v => v.shift < SurfaceWidth / 2));
            Assert.IsTrue(ShiftInputsRow.Any(v => v.shift > SurfaceWidth / 2 && v.shift != SurfaceWidth));

            Assert.IsTrue(ShiftInputsCol.Any(v => v.shift < SurfaceHeight / 2));
            Assert.IsTrue(ShiftInputsCol.Any(v => v.shift > SurfaceHeight / 2 && v.shift != SurfaceHeight));

            // Also need to test shift of width (everything ends up back where it starts)
            Assert.IsTrue(ShiftInputsRow.Max(v => v.shift) == SurfaceWidth);
            Assert.IsTrue(ShiftInputsCol.Max(v => v.shift) == SurfaceHeight);

            // Make sure that our other subset test case also tests count
            Assert.IsTrue(ShiftInputsRow.Any(v => v.shift == SubsetOffsetRow));
            Assert.IsTrue(ShiftInputsCol.Any(v => v.shift == SubsetOffsetCol));

            // Need at least one odd shift number because of how we generate test data.  Every other field in a row has
            // a unique value for IsVisible, so we want to ensure at least one shift is odd so that each cell gets
            // fully new values.
            Assert.IsTrue(ShiftInputsRow.Any(v => v.shift % 2 == 1));
            Assert.IsTrue(ShiftInputsCol.Any(v => v.shift % 2 == 1));

            // Necessary to ensure our color indexing will work properly (both here and in ShiftConsole)
            Assert.IsTrue(SurfaceWidth * SurfaceHeight <= 255);
        }
        #endregion

        #region Shift Row one direction

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsRow))]
        public void ShiftRowRight(int startingX, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForRow();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, startingX, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != count)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, startingX, count, shiftAmount, wrap);
        }

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsRow))]
        public void ShiftRowLeft(int startingX, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForRow();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, startingX, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != count)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, startingX, count, -shiftAmount, wrap);
        }
        #endregion

        #region Shift Row both directions
        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsRowWithNeg))]
        public void ShiftRowLeftOrRightSubset(int startingX, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForRow();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRow(ShiftRow, startingX, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (Math.Abs(shiftAmount) != Math.Abs(count))
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, startingX, count, shiftAmount, wrap);
        }

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsRowWholeRow))]
        public void ShiftRowWholeLeftOrRight(int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForRow();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRow(ShiftRow, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (Math.Abs(shiftAmount) != SurfaceWidth)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, 0, SurfaceWidth, shiftAmount, wrap);
        }
        #endregion

        #region Shift Col one direction

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsCol))]
        public void ShiftColDown(int startingY, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForCol();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftColumnDown(ShiftCol, startingY, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != count)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertColHasShifted(surface, ShiftCol, startingY, count, shiftAmount, wrap);
        }

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsCol))]
        public void ShiftColUp(int startingY, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForCol();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftColumnUp(ShiftCol, startingY, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != count)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertColHasShifted(surface, ShiftCol, startingY, count, -shiftAmount, wrap);
        }
        #endregion

        #region Shift Col both directions
        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsColWithNeg))]
        public void ShiftColUpOrDownSubset(int startingY, int count, int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForCol();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftColumn(ShiftCol, startingY, count, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (Math.Abs(shiftAmount) != Math.Abs(count))
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertColHasShifted(surface, ShiftCol, startingY, count, shiftAmount, wrap);
        }

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputsColWholeCol))]
        public void ShiftColWholeUpOrDown(int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurfaceForCol();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftColumn(ShiftCol, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (Math.Abs(shiftAmount) != SurfaceHeight)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertColHasShifted(surface, ShiftCol, 0, SurfaceHeight, shiftAmount, wrap);
        }
        #endregion


        #region Shift Test Helpers

        private static SadConsole.CellSurface CreateShiftableCellSurfaceForRow()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(SurfaceWidth, SurfaceHeight);

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface[x, ShiftRow].CopyAppearanceFrom(GetShiftCellForSingleRowCol(x));

            // Set IsDirty to false because we want to test that certain functions set it to true, and we're not
            // actually rendering it so it won't matter
            surface.IsDirty = false;

            return surface;
        }

        private static SadConsole.CellSurface CreateShiftableCellSurfaceForCol()
        {
            // Create surface and set cells of one col only
            var surface = new SadConsole.CellSurface(SurfaceWidth, SurfaceHeight);

            // Number it with first cell in col glyph 1, second glyph 2, etc.
            for (int y = 0; y < surface.Height; y++)
                surface[ShiftCol, y].CopyAppearanceFrom(GetShiftCellForSingleRowCol(y));

            // Set IsDirty to false because we want to test that certain functions set it to true, and we're not
            // actually rendering it so it won't matter
            surface.IsDirty = false;

            return surface;
        }

        // Computes a ColoredGlyph with unique values for its appearance-related fields, given an ID. This ID
        // should be either the X-value or the Y-value for a position on a surface.
        private static ColoredGlyph GetShiftCellForSingleRowCol(int positionId)
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

        private static bool CheckAppearancesEqual(ColoredGlyph c1, ColoredGlyph c2)
        {
            if (c1.Glyph != c2.Glyph || c1.Background != c2.Background || c1.Foreground != c2.Foreground ||
                c1.Mirror != c2.Mirror)
                return false;

            return c1.Decorators.SequenceEqual(c2.Decorators);
        }

        // Checks that a row has been shifted as specified, assuming the cells were generated via GetShiftCellFor.
        // Also asserts that no other cells (outside of that row) have changed
        private static void AssertRowHasShifted(ICellSurface surface, int row, int startingX, int count, int shiftAmount, bool wrap)
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

            foreach (var (x, y) in surface.Positions())
            {
                // Row was shifted; check that the value has been pulled from the right cell during shift
                if (y == row && x >= startingX && x < startingX + count)
                {
                    int oldX = x - shiftAmount;

                    if (wrap) oldX = WrapAround(oldX - startingX, count) + startingX;
                    var expectedGlyph = oldX >= startingX && oldX < startingX + count || wrap ? GetShiftCellForSingleRowCol(oldX) : blankGlyph;

                    Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
                }
                // Value should not have changed.
                else
                {
                    var expectedGlyph = y == ShiftRow ? GetShiftCellForSingleRowCol(x) : blankGlyph;
                    Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
                }
            }
        }

        // Checks that a col has been shifted as specified, assuming the cells were generated via GetShiftCellFor.
        // Also asserts that no other cells (outside of that col) have changed
        private static void AssertColHasShifted(ICellSurface surface, int col, int startingY, int count, int shiftAmount, bool wrap)
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

            foreach (var (x, y) in surface.Positions())
            {
                // Row was shifted; check that the value has been pulled from the right cell during shift
                if (x == col && y >= startingY && y < startingY + count)
                {
                    int oldY = y - shiftAmount;

                    if (wrap) oldY = WrapAround(oldY - startingY, count) + startingY;
                    var expectedGlyph = oldY >= startingY && oldY < startingY + count || wrap ? GetShiftCellForSingleRowCol(oldY) : blankGlyph;

                    Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
                }
                // Value should not have changed.
                else
                {
                    var expectedGlyph = x == ShiftCol ? GetShiftCellForSingleRowCol(y) : blankGlyph;
                    Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
                }
            }
        }

        // Wrapping function that wraps -1 around to wrapTo - 1
        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
        #endregion
    }
}
