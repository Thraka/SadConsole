using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives.GridViews;
using Diag = System.Diagnostics;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        #region ShiftRowRight
        [TestMethod]
        public void ShiftRowRight_NoWrap()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;
            int shiftAmount = 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                 surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(centerRow, shiftAmount, false);
            PrintSurfaceGlyphs(surface, "After:");

            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x < shiftAmount ? 0 : x + 1 - shiftAmount, surface.GetGlyph(x, centerRow));
        }

        [TestMethod]
        public void ShiftRowRight_NoWrap_ShiftsAll()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(centerRow, surface.Width, false);
            PrintSurfaceGlyphs(surface, "After:");

            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(0, surface.GetGlyph(x, centerRow));
        }

        [TestMethod]
        public void ShiftRowRight_WithWrap()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;
            int shiftAmount = 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(centerRow, shiftAmount, true);
            PrintSurfaceGlyphs(surface, "After:");

            // Instead of checking the position we have an x value for, we will check the position
            // it should have been shifted to, since this is much easier to compute
            for (int x = 0; x < surface.Width; x++)
            {
                // Old value at x
                int oldValue = x + 1;
                // New location for old value that was at x
                int newLocationX = (x + shiftAmount) % surface.Width;

                // Check
                Assert.AreEqual(oldValue, surface.GetGlyph(newLocationX, centerRow));
            }
        }

        [TestMethod]
        public void ShiftRowRight_WithWrap_ShiftsAll()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowRight(centerRow, surface.Width, true);
            PrintSurfaceGlyphs(surface, "After:");

            // If we shifted width, everything ends up back where it started if we wrap properly
            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x + 1, surface.GetGlyph(x, centerRow));
        }
        #endregion

        #region ShiftRowLeft
        [TestMethod]
        public void ShiftRowLeft_NoWrap()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;
            int shiftAmount = 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                 surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(centerRow, shiftAmount, false);
            PrintSurfaceGlyphs(surface, "After:");

            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x >= shiftAmount ? 0 : x + 1 + shiftAmount, surface.GetGlyph(x, centerRow));
        }

        [TestMethod]
        public void ShiftRowLeft_NoWrap_ShiftsAll()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(centerRow, surface.Width, false);
            PrintSurfaceGlyphs(surface, "After:");

            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(0, surface.GetGlyph(x, centerRow));
        }

        [TestMethod]
        public void ShiftRowLeft_WithWrap()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;
            int shiftAmount = 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(centerRow, shiftAmount, true);
            PrintSurfaceGlyphs(surface, "After:");

            // Instead of checking the position we have an x value for, we will check the position
            // it should have been shifted to, since this is much easier to compute
            for (int x = 0; x < surface.Width; x++)
            {
                // Old value at x
                int oldValue = x + 1;
                // New location for old value that was at x
                int newLocationX = WrapAround(x + shiftAmount, surface.Width);

                // Check
                Assert.AreEqual(oldValue, surface.GetGlyph(newLocationX, centerRow));
            }
        }

        [TestMethod]
        public void ShiftRowLeft_WithWrap_ShiftsAll()
        {
            // Create surface and set cells of one row only
            var surface = new SadConsole.CellSurface(20, 10);
            int centerRow = surface.Height / 2;

            // Number it with first cell in row glyph 1, second glyph 2, etc.
            for (int x = 0; x < surface.Width; x++)
                surface.SetGlyph(x, centerRow, x + 1);

            // Shift with some helpful before/after output
            PrintSurfaceGlyphs(surface, "Before:");
            surface.ShiftRowLeft(centerRow, surface.Width, true);
            PrintSurfaceGlyphs(surface, "After:");

            // If we shifted width, everything ends up back where it started if we wrap properly
            for (int x = 0; x < surface.Width; x++)
                Assert.AreEqual(x + 1, surface.GetGlyph(x, centerRow));
        }
        #endregion

        private void PrintSurfaceGlyphs(ICellSurface surface, string header)
        {
            Diag.Debug.WriteLine(header);
            Diag.Debug.WriteLine(surface.ExtendToString(2, elementStringifier: g => g.Glyph.ToString()));
        }

        // Wrapping function that wraps -1 around to wrapTo - 1
        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
    }
}
