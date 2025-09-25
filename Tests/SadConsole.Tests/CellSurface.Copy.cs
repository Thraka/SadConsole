using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Copy_SameSizeRegion_Exact()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);
            var surface2 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);
            surface1.Copy(surface2);

            for (int i = 0; i < surface1.Cells.Length; i++)
            {
                Assert.IsTrue(surface1[i].Matches(surface2[i]));
            }
        }

        [TestMethod]
        public void Copy_SameSizeRegion_IntoBigger()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);
            var surface2 = new SadConsole.CellSurface(22, 22);

            ColoredGlyphBase defaultCell = new ColoredGlyph();
            surface2[0].CopyAppearanceTo(defaultCell);

            surface1.FillWithRandomGarbage(255);
            surface1.Copy(surface2);

            for (int y = 0; y < surface2.Height; y++)
            {
                for (int x = 0; x < surface2.Width; x++)
                {
                    if (x > surface1.Width - 1 || y > surface1.Height - 1)
                        Assert.IsTrue(surface2[x, y].Matches(defaultCell));
                    else
                        Assert.IsTrue(surface1[x, y].Matches(surface2[x, y]));
                }
            }
        }
    }
}
