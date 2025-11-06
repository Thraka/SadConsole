using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public class ColoredString
    {
        [TestMethod]
        public void CombineStringNormal()
        {
            var str1 = new SadConsole.ColoredString("string 1", Color.Green, Color.Yellow);
            var str2 = "string 2";
            SadConsole.ColoredString result = str1 + str2;
            Assert.IsTrue(result[7].Matches(str1[7]), "ColoredGlyph should match");
            Assert.AreEqual(str2[3], result[str1.Length + 3].Glyph, "Glyph should match");
            Assert.IsTrue(result[str1.Length - 1].Background == result[str1.Length + 3].Background, "Background should match");
        }

        [TestMethod]
        public void CombineStringReversed()
        {
            var str1 = new SadConsole.ColoredString("string 1", Color.Green, Color.Yellow);
            var str2 = "string 2";
            SadConsole.ColoredString result = str2 + str1;
            Assert.AreEqual(result[7].Glyph, str2[7], "Glyph should match");
            Assert.IsTrue(result[str2.Length + 3].Matches(str1[3]), "ColoredGlyph should match");
            Assert.IsFalse(result[str1.Length - 1].Background == result[str2.Length + 3].Background, "Background shouldn't match");
        }

        [TestMethod]
        public void EmptyCombine()
        {
            var str1 = new SadConsole.ColoredString("", Color.Green, Color.Yellow);
            var str2 = "";

            SadConsole.ColoredString result = str2 + str1;

            Assert.AreEqual(0, result.Length, "empty + empty should have 0 length");

            str2 = "test";
            result = str2 + str1;
            Assert.AreEqual(str2.Length, result.Length, "empty + Colored should have correct length");

            str1 = new SadConsole.ColoredString("test2", Color.Green, Color.Yellow);
            str2 = "";
            result = str1 + str2;
            Assert.AreEqual(str1.Length, result.Length, "Colored + empty should have correct length");
            Assert.IsTrue(result[2].Matches(str1[2]), "Colored + empty should match on glyph");
        }
    }
}
