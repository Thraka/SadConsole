using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    [TestClass]
    public partial class CellSurface
    {
        [DataTestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        [DataRow(100, 100)]
        public void Create_WithSize(int width, int height)
        {
            var surface = new SadConsole.CellSurface(width, height);
            Assert.IsTrue(surface.BufferWidth == width, $"BufferWidth does not match width");
            Assert.IsTrue(surface.BufferWidth == height, $"BufferHeight does not match height");
            Assert.IsTrue(surface.ViewPosition == new SadRogue.Primitives.Point(0, 0));
        }

        [DataTestMethod]
        [DataRow(1, 1, 10, 10)]
        [DataRow(10, 10, 100, 100)]
        [DataRow(100, 100, 1000, 1000)]
        public void Create_WithViewSize(int width, int height, int bufferWidth, int bufferHeight)
        {
            var surface = new SadConsole.CellSurface(width, height, bufferWidth, bufferHeight);
            Assert.IsTrue(surface.BufferWidth == bufferWidth, $"BufferWidth does not match value");
            Assert.IsTrue(surface.BufferWidth == bufferHeight, $"BufferHeight does not match value");
            Assert.IsTrue(surface.View.Width == width, $"View.Width does not match value");
            Assert.IsTrue(surface.View.Height == height, $"View.Height does not match value");
            Assert.IsTrue(surface.ViewPosition == new SadRogue.Primitives.Point(0, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_WidthZero_IsException()
        {
            new SadConsole.CellSurface(0, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_HeightZero_IsException()
        {
            new SadConsole.CellSurface(10, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_BufferHeightZero_IsException()
        {
            new SadConsole.CellSurface(1, 1, 10, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_BufferWidthZero_IsException()
        {
            new SadConsole.CellSurface(1, 1, 0, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_ViewBiggerThanBufferWidth_IsException()
        {
            new SadConsole.CellSurface(11, 1, 10, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Create_ViewBiggerThanBufferHeight_IsException()
        {
            new SadConsole.CellSurface(1, 11, 10, 10);
        }
    }
}
