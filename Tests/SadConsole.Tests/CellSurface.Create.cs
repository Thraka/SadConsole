using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    [TestClass]
    public partial class CellSurface
    {
        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(10, 10)]
        [DataRow(100, 100)]
        public void Create_WithSize(int width, int height)
        {
            var surface = new SadConsole.CellSurface(width, height);
            Assert.AreEqual(width, surface.Width, $"BufferWidth does not match width");
            Assert.AreEqual(height, surface.Height, $"BufferHeight does not match height");
            Assert.IsTrue(surface.ViewPosition == new SadRogue.Primitives.Point(0, 0));
        }

        [TestMethod]
        [DataRow(1, 1, 10, 10)]
        [DataRow(10, 10, 100, 100)]
        [DataRow(100, 100, 1000, 1000)]
        public void Create_WithViewSize(int width, int height, int bufferWidth, int bufferHeight)
        {
            var surface = new SadConsole.CellSurface(width, height, bufferWidth, bufferHeight);
            Assert.AreEqual(bufferWidth, surface.Width, $"BufferWidth does not match value");
            Assert.AreEqual(bufferHeight, surface.Height, $"BufferHeight does not match value");
            Assert.AreEqual(width, surface.View.Width, $"View.Width does not match value");
            Assert.AreEqual(height, surface.View.Height, $"View.Height does not match value");
            Assert.IsTrue(surface.ViewPosition == new SadRogue.Primitives.Point(0, 0));
        }

        [TestMethod]
        public void Create_WidthZero_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(0, 10));
        }

        [TestMethod]
        public void Create_HeightZero_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(10, 0));
        }

        [TestMethod]
        public void Create_BufferHeightZero_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(1, 1, 10, 0));
        }

        [TestMethod]
        public void Create_BufferWidthZero_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(1, 1, 0, 10));
        }

        [TestMethod]
        public void Create_ViewBiggerThanBufferWidth_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(11, 1, 10, 10));
        }

        [TestMethod]
        public void Create_ViewBiggerThanBufferHeight_IsException()
        {
            Assert.ThrowsExactly<System.ArgumentOutOfRangeException>(() => new SadConsole.CellSurface(1, 11, 10, 10));
        }
    }
}
