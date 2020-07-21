using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Glyph_SetForeground()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            Color cellForeground = surface1[0].Foreground;
            Color newForeground = Color.Blue.GetRandomColor(new System.Random());

            Assert.IsFalse(newForeground == surface1[0, 0].Foreground);

            surface1.SetForeground(0, 0, newForeground);

            Assert.IsTrue(newForeground == surface1[0, 0].Foreground);
        }

        [TestMethod]
        public void Glyph_SetBackground()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            Color newBackground = Color.Blue.GetRandomColor(new System.Random());

            Assert.IsFalse(newBackground == surface1[0, 0].Background);

            surface1.SetBackground(0, 0, newBackground);

            Assert.IsTrue(newBackground == surface1[0, 0].Background);
        }

        [TestMethod]
        public void Glyph_SetGlyph()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            int newGlyph = new System.Random().Next(0, 256);

            Assert.IsFalse(newGlyph == surface1[0, 0].Glyph);

            surface1.SetGlyph(0, 0, newGlyph);

            Assert.IsTrue(newGlyph == surface1[0, 0].Glyph);
        }

        [TestMethod]
        public void Glyph_SetMirror()
        {
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            Mirror cellMirror = surface1[0].Mirror;
            Mirror newMirror = cellMirror switch
            {
                Mirror.None => Mirror.Horizontal,
                Mirror.Horizontal => Mirror.Vertical,
                _ => Mirror.None
            };

            Assert.IsFalse(newMirror == surface1[0, 0].Mirror);

            surface1.SetMirror(0, 0, newMirror);

            Assert.IsTrue(newMirror == surface1[0, 0].Mirror);
        }
    }
}
