using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_WidthTo1(bool clear)
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = 1;
            int newHeight = height;

            Point point1 = new(2, 20);
            Point point2 = new(5, 18);
            Point point3 = new(0, 15);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            if (!clear)
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            else
                Assert.IsFalse(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_HeightTo1(bool clear)
        {
            new SadConsole.Tests.BasicGameHost();

            int width = 21;
            int height = 21;

            var surface1 = new SadConsole.CellSurface(width, height);
            surface1.FillWithRandomGarbage(255);

            int newWidth = width;
            int newHeight = 1;

            Point point1 = new(20, 2);
            Point point2 = new(18, 5);
            Point point3 = new(15, 0);

            ColoredGlyph glyph1 = new();
            ColoredGlyph glyph2 = new();
            ColoredGlyph glyph3 = new();
            surface1[point1].CopyAppearanceTo(glyph1);
            surface1[point2].CopyAppearanceTo(glyph2);
            surface1[point3].CopyAppearanceTo(glyph3);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            surface1.Resize(newWidth - 5, newHeight, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            if (!clear)
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            else
                Assert.IsFalse(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_Smaller(bool clear)
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

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            if (!clear)
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            else
                Assert.IsFalse(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_Smaller_MatchingWidth(bool clear)
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

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsFalse(surface1.IsValidCell(point1));
            Assert.IsFalse(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            if (!clear)
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            else
                Assert.IsFalse(surface1[point3].Matches(glyph3));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_Bigger(bool clear)
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

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));
            if (!clear)
            {
                Assert.IsTrue(surface1[point1].Matches(glyph1));
                Assert.IsTrue(surface1[point2].Matches(glyph2));
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            }
            else
            {
                Assert.IsFalse(surface1[point1].Matches(glyph1));
                Assert.IsFalse(surface1[point2].Matches(glyph2));
                Assert.IsFalse(surface1[point3].Matches(glyph3));
            }

                ColoredGlyph glyph4 = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);
            Assert.IsTrue(surface1[point4].Matches(glyph4));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_Bigger_MatchingWidth(bool clear)
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

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(newWidth, surface1.Width);
            Assert.AreEqual(newHeight, surface1.Height);
            
            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            if (!clear)
            {
                Assert.IsTrue(surface1[point1].Matches(glyph1));
                Assert.IsTrue(surface1[point2].Matches(glyph2));
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            }
            else
            {
                Assert.IsFalse(surface1[point1].Matches(glyph1));
                Assert.IsFalse(surface1[point2].Matches(glyph2));
                Assert.IsFalse(surface1[point3].Matches(glyph3));
            }

                ColoredGlyph glyph4 = new(surface1.DefaultForeground, surface1.DefaultBackground, 0, Mirror.None);
            Assert.IsTrue(surface1[point4].Matches(glyph4));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Resize_SameSize(bool clear)
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

            surface1.Resize(newWidth - 5, newHeight - 5, newWidth, newHeight, clear);

            Assert.AreEqual(width, surface1.Width);
            Assert.AreEqual(height, surface1.Height);

            Assert.IsTrue(surface1.IsValidCell(point1));
            Assert.IsTrue(surface1.IsValidCell(point2));
            Assert.IsTrue(surface1.IsValidCell(point3));

            if (!clear)
            {
                Assert.IsTrue(surface1[point1].Matches(glyph1));
                Assert.IsTrue(surface1[point2].Matches(glyph2));
                Assert.IsTrue(surface1[point3].Matches(glyph3));
            }
            else
            {
                Assert.IsFalse(surface1[point1].Matches(glyph1));
                Assert.IsFalse(surface1[point2].Matches(glyph2));
                Assert.IsFalse(surface1[point3].Matches(glyph3));
            }
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
