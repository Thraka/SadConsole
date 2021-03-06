using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Glyph_SetForeground()
        {
            new SadConsole.Tests.BasicGameHost();
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
            new SadConsole.Tests.BasicGameHost();
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
            new SadConsole.Tests.BasicGameHost();
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
            new SadConsole.Tests.BasicGameHost();
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

        public void Surface_Equals(ICellSurface surface1, ICellSurface surface2)
        {
            Assert.AreEqual(surface1.View, surface2.View);
            Assert.AreEqual(surface1.Area, surface2.Area);
            Assert.AreEqual(surface1.Count, surface2.Count);
            Assert.AreEqual(surface1.DefaultBackground, surface2.DefaultBackground);
            Assert.AreEqual(surface1.DefaultForeground, surface2.DefaultForeground);
            Assert.AreEqual(surface1.DefaultGlyph, surface2.DefaultGlyph);
            Assert.AreEqual(surface1.IsScrollable, surface2.IsScrollable);
            Assert.AreEqual(surface1.TimesShiftedDown, surface2.TimesShiftedDown);
            Assert.AreEqual(surface1.TimesShiftedLeft, surface2.TimesShiftedLeft);
            Assert.AreEqual(surface1.TimesShiftedRight, surface2.TimesShiftedRight);
            Assert.AreEqual(surface1.TimesShiftedUp, surface2.TimesShiftedUp);
            Assert.AreEqual(surface1.UsePrintProcessor, surface2.UsePrintProcessor);

            for (int i = 0; i < surface1.Count; i++)
                Assert.IsTrue(surface1[i].Matches(surface2[i]));
        }
    }
}
