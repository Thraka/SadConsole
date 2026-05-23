using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    [TestClass]
    public class StringParserDefault
    {
        /// <summary>
        /// Regression test for issue #370: fixedSurfaceIndex uses input string index
        /// instead of output glyph index, causing wrong surface cell colors to be used
        /// as fallback when the string contains parser commands.
        /// </summary>
        [TestMethod]
        public void Parse_SurfaceColorFallback_UsesCorrectSurfaceIndex()
        {
            new BasicGameHost();

            // Create a 20-wide surface and set distinct background colors on cells 0-9
            var surface = new SadConsole.CellSurface(20, 1);
            for (int i = 0; i < 10; i++)
            {
                surface[i, 0].Background = new Color((byte)100, (byte)(50 + i * 20), (byte)200, (byte)255);
            }

            // Parse a string with a foreground command prefix followed by text "Hello".
            // The command "[c:r f:255,0,0]" is 15 chars in the input string.
            // Without the bug fix, when processing character 'H' at input index 15,
            // fixedSurfaceIndex would be 15 (wrong). It should be 0 (the first output glyph).
            var parser = new SadConsole.StringParser.Default();
            SadConsole.ColoredString result = parser.Parse("[c:r f:255,0,0]Hello", surfaceIndex: 0, surface: surface);

            // The result should have 5 glyphs: H, e, l, l, o
            Assert.AreEqual(5, result.Length, "Parsed string should have 5 glyphs");

            // Each glyph's background should come from the CORRECT surface cell (index 0-4),
            // not from the wrong offset (index 15-19 which would be default/transparent).
            for (int i = 0; i < 5; i++)
            {
                Color expectedBackground = new Color((byte)100, (byte)(50 + i * 20), (byte)200, (byte)255);
                Assert.AreEqual(expectedBackground, result[i].Background,
                    $"Glyph at position {i} should have background from surface cell {i}, " +
                    $"expected {expectedBackground} but got {result[i].Background}");
            }
        }
    }
}
