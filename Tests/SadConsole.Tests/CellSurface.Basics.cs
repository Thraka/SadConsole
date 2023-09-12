using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace SadConsole.Tests
{
    public partial class CellSurface
    {
        [TestMethod]
        public void Print_DecoratorsClear()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            Color foreground = Color.AliceBlue;
            Color background = Color.Purple;
            Mirror mirror = Mirror.Vertical;

            surface1.Print(1, 1, "Test me!", foreground, background, mirror);
            surface1.SetDecorator(2, 1, 1, new CellDecorator(Color.Yellow, 22, Mirror.None));

            Assert.IsTrue(surface1[1, 1].Foreground == foreground);
            Assert.IsTrue(surface1[1, 1].Background == background);
            Assert.IsTrue(surface1[1, 1].Glyph == (int)'T');
            Assert.IsTrue(surface1[1, 1].Mirror == mirror);
            Assert.IsNull(surface1[1, 1].Decorators);

            Assert.IsTrue(surface1[2, 1].Glyph == (int)'e');
            Assert.IsTrue(surface1[2, 1].Decorators.Count == 1);

            surface1.Print(1, 1, "Test me!", foreground, background, mirror);
            Assert.IsTrue(surface1[2, 1].Glyph == (int)'e');
            Assert.IsTrue(surface1[2, 1].Decorators.Count == 1);

            CellDecorator dec1 = new CellDecorator(Color.Wheat, 34, Mirror.None);
            CellDecorator dec2 = new CellDecorator(Color.Purple, 21, Mirror.Vertical);
            CellDecorator dec3 = new CellDecorator(Color.PapayaWhip, 85, Mirror.Horizontal);

            surface1.Print(1, 1, "dON'T!pl", foreground, background, mirror, new[] { dec1, dec2, dec3 });
            Assert.IsTrue(surface1[(4, 1)].Glyph == (int)'\'');
            Assert.IsTrue(surface1[(4, 1)].Decorators.Count == 3);

            surface1.Print(1, 1, "dON'T!pl", foreground, background, mirror, null);
            Assert.IsTrue(surface1[(4, 1)].Glyph == (int)'\'');
            Assert.IsNull(surface1[(4, 1)].Decorators);
        }

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

        [TestMethod]
        public void Glyph_SetDecoratorSingle()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                          GameHost.Instance.Random.Next(255),
                                          s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.SetDecorator((5, 5), decorator);

            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator));
        }

        [TestMethod]
        public void Glyph_SetDecoratorMultiple()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator1 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator2 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator3 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.SetDecorator((5, 5), decorator1, decorator2, decorator3);

            Assert.IsTrue(surface1[5, 5].Decorators.Count == 3);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator3));
        }

        [TestMethod]
        public void Glyph_SetDecoratorNull()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator1 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator2 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator3 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.SetDecorator((5, 5), decorator1, decorator2, decorator3);

            Assert.IsTrue(surface1[5, 5].Decorators.Count == 3);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator3));

            surface1.SetDecorator((5, 5), null);
            Assert.IsNull(surface1[5, 5].Decorators);
        }

        [TestMethod]
        public void Glyph_AddDecoratorSingle()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                          GameHost.Instance.Random.Next(255),
                                          s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.AddDecorator((5, 5), decorator);

            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator));
        }

        [TestMethod]
        public void Glyph_AddDecoratorMultiple()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator1 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator2 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator3 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.AddDecorator((5, 5), decorator1, decorator2);

            Assert.IsTrue(surface1[5, 5].Decorators.Count == 2);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));
            Assert.IsFalse(surface1[5, 5].Decorators.Contains(decorator3));

            surface1.AddDecorator((5, 5), decorator3);
            Assert.IsTrue(surface1[5, 5].Decorators.Count == 3);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator3));
        }

        [TestMethod]
        public void Glyph_AddDecoratorNoDupes()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator1 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator2 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator3 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.AddDecorator((5, 5), decorator1, decorator2);

            Assert.IsTrue(surface1[5, 5].Decorators.Count == 2);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));
            Assert.IsFalse(surface1[5, 5].Decorators.Contains(decorator3));

            surface1.AddDecorator((5, 5), decorator3);
            Assert.IsTrue(surface1[5, 5].Decorators.Count == 3);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator3));

            surface1.AddDecorator((5, 5), decorator2);
            Assert.IsTrue(surface1[5, 5].Decorators.Count == 3);
        }

        [TestMethod]
        public void Glyph_RemoveDecorator()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(20, 20);

            surface1.FillWithRandomGarbage(255);

            CellDecorator decorator1 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            CellDecorator decorator2 = new(Color.AliceBlue.GetRandomColor(GameHost.Instance.Random),
                                           GameHost.Instance.Random.Next(255),
                                           s_mirrorValues[GameHost.Instance.Random.Next(100) % s_mirrorValues.Length]);

            Assert.IsNull(surface1[5, 5].Decorators);

            surface1.AddDecorator((5, 5), decorator1, decorator2);

            Assert.IsTrue(surface1[5, 5].Decorators.Count == 2);
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));

            surface1.RemoveDecorator((5, 5), decorator1);
            Assert.IsTrue(surface1[5, 5].Decorators.Count == 1);
            Assert.IsFalse(surface1[5, 5].Decorators.Contains(decorator1));
            Assert.IsTrue(surface1[5, 5].Decorators.Contains(decorator2));

            surface1.RemoveDecorator((5, 5), decorator2);
            Assert.IsNull(surface1[5, 5].Decorators);
        }

        [TestMethod]
        [DataRow(20, 20, 4, 4, 7, 7)]
        [DataRow(20, 20, -20, -20, 50, 50)]
        [DataRow(20, 20, 30, 30, 10, 10)]
        public void Clear_Rect(int width, int height, int regionX, int regionY, int regionWidth, int regionHeight)
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(width, height);
            var region = new Rectangle(regionX, regionY, regionWidth, regionHeight);

            surface1.FillWithRandomGarbage(255);
            PrintSurfaceGlyphs(surface1, "Before:");
            surface1.Clear(region);
            PrintSurfaceGlyphs(surface1, "After:");

            ColoredGlyphBase defaultGlyph = new ColoredGlyph(surface1.DefaultForeground, surface1.DefaultBackground, surface1.DefaultGlyph);

            foreach (var item in Rectangle.GetIntersection(region, new Rectangle(0, 0, surface1.Width, surface1.Height)).Positions())
                Assert.IsTrue(defaultGlyph.Matches(surface1[item]));
        }

        [TestMethod]
        [DataRow(5, 5, 0, 1)]
        [DataRow(5, 5, 4, 4)]
        [DataRow(5, 5, 3, 3)]
        [DataRow(5, 5, -1, -1)]
        public void Clear_XY(int width, int height, int clearX, int clearY)
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(width, height);

            surface1.FillWithRandomGarbage(255);
            PrintSurfaceGlyphs(surface1, "Before:");
            surface1.Clear(clearX, clearY);
            PrintSurfaceGlyphs(surface1, "After:");

            if (surface1.IsValidCell(clearX, clearY))
            {
                ColoredGlyphBase defaultGlyph = new ColoredGlyph(surface1.DefaultForeground, surface1.DefaultBackground, surface1.DefaultGlyph);
                Assert.IsTrue(defaultGlyph.Matches(surface1[clearX, clearY]));
            }
        }

        [TestMethod]
        [DataRow(5, 5, 0, 1, 3)]
        [DataRow(5, 5, 4, 4, 6)]
        [DataRow(5, 5, 3, 3, 8)]
        [DataRow(5, 5, -1, -1, 8)]
        public void Clear_Length(int width, int height, int clearX, int clearY, int length)
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(width, height);

            surface1.FillWithRandomGarbage(255);
            PrintSurfaceGlyphs(surface1, "Before:");
            surface1.Clear(clearX, clearY, length);
            PrintSurfaceGlyphs(surface1, "After:");

            if (surface1.IsValidCell(clearX, clearY))
            {
                ColoredGlyphBase defaultGlyph = new ColoredGlyph(surface1.DefaultForeground, surface1.DefaultBackground, surface1.DefaultGlyph);
                int startingIndex = new Point(clearX, clearY).ToIndex(surface1.Width);
                for (int i = 0; i < length; i++)
                {
                    if (surface1.IsValidCell(startingIndex + i))
                        Assert.IsTrue(defaultGlyph.Matches(surface1[startingIndex + i]));
                }
            }
        }

        [TestMethod]
        public void Clear_All()
        {
            new SadConsole.Tests.BasicGameHost();
            var surface1 = new SadConsole.CellSurface(7, 10);

            surface1.FillWithRandomGarbage(255);
            PrintSurfaceGlyphs(surface1, "Before:");
            surface1.Clear();
            PrintSurfaceGlyphs(surface1, "After:");

            ColoredGlyphBase defaultGlyph = new ColoredGlyph(surface1.DefaultForeground, surface1.DefaultBackground, surface1.DefaultGlyph);
            for (int i = 0; i < surface1.Count; i++)
                Assert.IsTrue(defaultGlyph.Matches(surface1[i]));
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
