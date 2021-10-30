using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        // Want to test shift values both < axis / 2 and > axis / 2, to ensure no wrapping/optimization issues
        public static IEnumerable<int> ShiftValues => new[] { 2, SurfaceWidth - 5 };

        #endregion

        #region ShiftRowRight
        [TestMethod]
        [BetterDynamicDataAttribute(nameof(ShiftValues))]
        public void ShiftRowRight_NoWrap(int shiftAmount)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, shiftAmount, false);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set
            Assert.IsTrue(surface.IsDirty);

            // Verify result row
            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x < shiftAmount ? 0 : x + 1 - shiftAmount, surface.GetGlyph(x, ShiftRow));

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }

        [TestMethod]
        public void ShiftRowRight_NoWrap_ShiftsAll()
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, surface.Width, false);
            PrintSurfaceGlyphs(surface, "After:");

            // IsDirty can be set or not; doesn't matter since the operation didn't actually change anything,
            // so we won't check it.

            // Verify results; everything was shifted off the end
            foreach (var pos in surface.Positions())
                Assert.AreEqual(0, surface.GetGlyph(pos.X, pos.Y));
        }

        [TestMethod]
        [BetterDynamicDataAttribute(nameof(ShiftValues))]
        public void ShiftRowRight_WithWrap(int shiftAmount)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, shiftAmount, true);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set
            Assert.IsTrue(surface.IsDirty);

            // Instead of checking the position we have an x value for, we will check the position
            // it should have been shifted to, since this is much easier to compute
            for (int x = 0; x < surface.Width; x++)
            {
                // Old value at x
                int oldValue = x + 1;
                // New location for old value that was at x
                int newLocationX = (x + shiftAmount) % surface.Width;

                // Check
                Assert.AreEqual(oldValue, surface.GetGlyph(newLocationX, ShiftRow));
            }

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }

        [TestMethod]
        public void ShiftRowRight_WithWrap_ShiftsAll()
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(ShiftRow, surface.Width, true);
            PrintSurfaceGlyphs(surface, "After:");

            // IsDirty can be set or not; doesn't matter since the operation didn't actually change anything,
            // so we won't check it.

            // Verify results for shifted row; everything ends up back where it started
            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x + 1, surface.GetGlyph(x, ShiftRow));

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }
        #endregion

        #region ShiftRowLeft
        [TestMethod]
        [BetterDynamicDataAttribute(nameof(ShiftValues))]
        public void ShiftRowLeft_NoWrap(int shiftAmount)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, shiftAmount, false);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set
            Assert.IsTrue(surface.IsDirty);

            // Verify results for shifted row
            for (int x = 0; x < surface.Width; x++)
            {
                int shiftedFromIdx = x + shiftAmount;
                int shiftedFromValue = shiftedFromIdx + 1;
                Assert.AreEqual(shiftedFromIdx >= surface.Width ? 0 : shiftedFromValue, surface.GetGlyph(x, ShiftRow));
            }

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }

        [TestMethod]
        public void ShiftRowLeft_NoWrap_ShiftsAll()
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, surface.Width, false);
            PrintSurfaceGlyphs(surface, "After:");

            // IsDirty can be set or not; doesn't matter since the operation didn't actually change anything,
            // so we won't check it.

            // Verify results; everything is shifted off the edge
            foreach (var pos in surface.Positions())
                Assert.AreEqual(0, surface.GetGlyph(pos.X, pos.Y));
        }

        [TestMethod]
        [BetterDynamicDataAttribute(nameof(ShiftValues))]
        public void ShiftRowLeft_WithWrap(int shiftAmount)
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, shiftAmount, true);
            PrintSurfaceGlyphs(surface, "After:");

            // Verify IsDirty is set
            Assert.IsTrue(surface.IsDirty);

            // Instead of checking the position we have an x value for, we will check the position
            // it should have been shifted to, since this is much easier to compute
            for (int x = 0; x < surface.Width; x++)
            {
                // Old value at x
                int oldValue = x + 1;
                // New location for old value that was at x
                int newLocationX = WrapAround(x - shiftAmount, surface.Width);

                // Check
                Assert.AreEqual(oldValue, surface.GetGlyph(newLocationX, ShiftRow));
            }

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }

        [TestMethod]
        public void ShiftRowLeft_WithWrap_ShiftsAll()
        {
            // Create test surface
            var surface = CreateShiftableCellSurface();

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(ShiftRow, surface.Width, true);
            PrintSurfaceGlyphs(surface, "After:");

            // IsDirty can be set or not; doesn't matter since the operation didn't actually change anything,
            // so we won't check it.

            // Verify results for shifted row; everything ends up back where it started
            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x + 1, surface.GetGlyph(x, ShiftRow));

            // Verify no other row was changed
            foreach (var (x, y) in surface.Positions().Where(pos => pos.Y != ShiftRow))
                Assert.AreEqual(0, surface.GetGlyph(x, y));
        }
        #endregion

        #region Shift Test Helpers

        private static SadConsole.CellSurface CreateShiftableCellSurface()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, ShiftRow, x + 1);

            // Set IsDirty to false because we want to test that certain functions set it to true, and we're not
            // actually rendering it so it won't matter
            surface.IsDirty = false;

            return surface;
        }

        // Wrapping function that wraps -1 around to wrapTo - 1
        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
        #endregion
    }
}
