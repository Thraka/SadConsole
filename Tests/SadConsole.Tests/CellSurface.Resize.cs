using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Resize_Smaller()
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = 16;
            int newHeight = 16;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, false);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsTrue(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        public void Resize_Smaller_MatchingWidth()
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = 16;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, false);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsTrue(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        public void Resize_Bigger()
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = 30;
            int newHeight = 30;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);
            Point point4 = new(newWidth - 3, newHeight - 3);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsFalse(surface1.IsValidCell(point4));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, false);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsTrue(surface1[point1].Matches(glyph1));
            Assert.IsTrue(surface1[point2].Matches(glyph2));
            Assert.IsTrue(surface1[point3].Matches(glyph3));

            ColoredGlyph glyph4 = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);
            Assert.IsTrue(surface1[point4].Matches(glyph4));
        }

        [TestMethod]
        public void Resize_Bigger_MatchingWidth()
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = 30;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);
            Point point4 = new(10, newHeight - 3);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsFalse(surface1.IsValidCell(point4));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, false);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
            
            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsTrue(surface1[point1].Matches(glyph1));
            Assert.IsTrue(surface1[point2].Matches(glyph2));
            Assert.IsTrue(surface1[point3].Matches(glyph3));

            ColoredGlyph glyph4 = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);
            Assert.IsTrue(surface1[point4].Matches(glyph4));
        }

        [TestMethod]
        public void Resize_Clear()
        {
            // This test only does smaller resize

            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            ColoredGlyph clearCell = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);

            int newWidth = 16;
            int newHeight = 16;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            Assert.IsFalse(surface1[point3].Matches(glyph3));
            Assert.IsTrue(surface1[point3].Matches(clearCell));
        }

        [TestMethod]
        public void Resize_Clear_MatchingWidth()
        {
            // This test only does bigger resize

            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            ColoredGlyph clearCell = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);

            int newWidth = width;
            int newHeight = 30;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);
            Point point4 = new(10, newHeight - 3);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            Assert.IsFalse(surface1.IsValidCell(point4));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            Assert.IsTrue(surface1[point1].Matches(clearCell));
            Assert.IsTrue(surface1[point2].Matches(clearCell));
            Assert.IsTrue(surface1[point3].Matches(clearCell));
            Assert.IsTrue(surface1[point4].Matches(clearCell));
        }

        [TestMethod]
        public void Resize_SameSize()
        {
            // This test only does bigger resize

            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = height;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, false);

            Assert.AreEqual(width, surface1.Width);
            Assert.AreEqual(height, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            Assert.IsTrue(surface1[point1].Matches(glyph1));
            Assert.IsTrue(surface1[point2].Matches(glyph2));
            Assert.IsTrue(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        public void Resize_SameSize_Clear()
        {
            // This test only does bigger resize

            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            ColoredGlyph clearCell = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);

            int newWidth = width;
            int newHeight = height;

            Point point1 = new(20, 20);
            Point point2 = new(18, 18);
            Point point3 = new(15, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, true);

            Assert.AreEqual(width, surface1.Width);
            Assert.AreEqual(height, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            Assert.IsTrue(surface1[point1].Matches(clearCell));
            Assert.IsTrue(surface1[point2].Matches(clearCell));
            Assert.IsTrue(surface1[point3].Matches(clearCell));
        }

        [TestMethod]
        public void Resize_BiggerHeight_To_SmallerHeight_Clear_NoException()
        {
            // This test does only big height to small height with clear
            new SadConsole.Tests.BasicGameHost();

            int width = 20;
            int height = 25;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = height - 5;

            // Should not throw index outside of bounds
            surface1.Resize(newWidth, newHeight, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
        }

        [TestMethod]
        public void Resize_SmallerHeight_To_BiggerHeight_Clear_NoException()
        {
            // This test does only small height to big height with clear
            new SadConsole.Tests.BasicGameHost();

            int width = 20;
            int height = 20;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = height + 5;

            // Should not throw index outside of bounds
            surface1.Resize(newWidth, newHeight, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
        }

        [TestMethod]
        public void Resize_BiggerWidth_To_SmallerWidth_Clear_NoException()
        {
            // This test does only big width to small width with clear
            new SadConsole.Tests.BasicGameHost();

            int width = 25;
            int height = 20;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width - 5;
            int newHeight = height;

            // Should not throw index outside of bounds
            surface1.Resize(newWidth, newHeight, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
        }

        [TestMethod]
        public void Resize_SmallerWidth_To_BiggerWidth_Clear_NoException()
        {
            // This test does only small width to big width with clear
            new SadConsole.Tests.BasicGameHost();

            int width = 20;
            int height = 20;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width + 5;
            int newHeight = height;

            // Should not throw index outside of bounds
            surface1.Resize(newWidth, newHeight, newWidth, newHeight, true);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
        }
    }
}
