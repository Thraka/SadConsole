using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Copy_SameSizeRegion_Exact()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);
            var surface2 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);
            surface1.Copy(surface2);

            for (int i = 0; i < surface1.Cells.Length; i++)
            {
                Assert.IsTrue(surface1[i].Equals(surface2[i]));
            }
        }

        [TestMethod]
        public void Copy_SameSizeRegion_IntoBigger()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);
            var surface2 = new SadConsole.CellSurface(22, 22);

            ColoredGlyph defaultCell = new ColoredGlyph();
            surface2[0].CopyAppearanceTo(defaultCell);

            surface1.FillWithRandomGarbage(255);
            surface1.Copy(surface2);

            for (int y = 0; y < surface2.BufferHeight; y++)
            {
                for (int x = 0; x < surface2.BufferWidth; x++)
                {
                    if (x > surface1.BufferWidth - 1 || y > surface1.BufferHeight - 1)
                        Assert.IsTrue(surface2[x, y].Equals(defaultCell));
                    else
                        Assert.IsTrue(surface1[x, y].Equals(surface2[x, y]));
                }
            }
        }
    }
}
