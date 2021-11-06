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


        private static readonly IEnumerable<int> s_shiftValues = new[] { 2, SurfaceWidth - 5, SurfaceWidth };
        // Want to test shift values both < axis / 2 and > axis / 2, to ensure no wrapping/optimization issues.
        // Also should be composed of at least one odd number.  We also want to test one value that is exactly shift
        // width
        public static IEnumerable<(int shift, bool wrap)> ShiftInputs => s_shiftValues.Combinate(new[] { true, false });

        #endregion

        private static readonly Mirror[] s_mirrorValues = LinqExtensions.GetEnumValues<Mirror>().ToArray();

        // A "unit test" that validates that the test data is sufficient to cover some corner cases.
        #region Test Data Validation

        [TestMethod]
        public void TestDataIsValid()
        {
            // Need both halves of the list to be in test cases
            int min = ShiftInputs.Min(v => v.shift);
            int max = ShiftInputs.Min(v => v.shift);
            Assert.IsTrue(ShiftInputs.Any(v => v.shift < SurfaceWidth / 2));
            Assert.IsTrue(ShiftInputs.Any(v => v.shift > SurfaceWidth / 2 && v.shift != SurfaceWidth));

            // Also need to test shift of width (everything ends up back where it starts)
            Assert.IsTrue(ShiftInputs.Max(v => v.shift) == SurfaceWidth);

            // Need at least one odd shift number because of how we generate test data.  Every other field in a row has
            // a unique value for IsVisible, so we want to ensure at least one shift is odd so that each cell gets
            // fully new values.
            Assert.IsTrue(ShiftInputs.Any(v => v.shift % 2 == 1));
        }
        #endregion

        #region ShiftRowRight

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputs))]
        public void ShiftRowRight(int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != surface.Width)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, shiftAmount, wrap);
        }

        [TestMethod]
        [BetterDynamicData(nameof(ShiftInputs))]
        public void ShiftRowLeft(int shiftAmount, bool wrap)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, shiftAmount, wrap);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set if cells changed.  If nothing changed, the implementation doesn't _have_ to set
            // IsDirty to be correct, but setting it harms nothing but efficiency; so we just won't check it
            if (shiftAmount != surface.Width)
                Assert.IsTrue(surface.IsDirty);

            // Verify shift result
            AssertRowHasShifted(surface, ShiftRow, -shiftAmount, wrap);
        }
        #endregion

        #region Shift Test Helpers

        private static SadConsole.CellSurface CreateShiftableCellSurface()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface[x, ShiftRow].CopyAppearanceFrom(GetShiftCellFor(x));

            // Set IsDirty to false because we want to test that certain functions set it to true, and we're not
            // actually rendering it so it won't matter
            surface.IsDirty = false;

            return surface;
        }

        // Computes a ColoredGlyph with unique values for its appearance-related fields, given an ID. This ID
        // should be either the X-value or the Y-value for a position on a surface.
        private static ColoredGlyph GetShiftCellFor(int positionId)
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
        private static void AssertRowHasShifted(ICellSurface surface, int row, int shiftAmount, bool wrap)
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
                if (y == ShiftRow)
                {
                    int oldX = x - shiftAmount;
                    if (wrap) oldX = WrapAround(oldX, surface.Width);
                    var expectedGlyph = surface.Contains(oldX, row) || wrap ? GetShiftCellFor(oldX) : blankGlyph;

                    Assert.IsTrue(CheckAppearancesEqual(expectedGlyph, surface[x, y]));
                }
                // Row never had anything in it so nothing should have been added.
                else
                    Assert.IsTrue(CheckAppearancesEqual(blankGlyph, surface[x, y]));
            }
        }

        // Wrapping function that wraps -1 around to wrapTo - 1
        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
        #endregion
    }
}
