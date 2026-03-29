using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Effects;
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

        // ── Construction ────────────────────────────────────────────────────────

        [TestMethod]
        public void Constructor_Default_IsEmpty()
        {
            var cs = new SadConsole.ColoredString();
            Assert.AreEqual(0, cs.Length);
            Assert.AreEqual("", cs.String);
        }

        [TestMethod]
        public void Constructor_Capacity_CreatesSpaceGlyphs()
        {
            var cs = new SadConsole.ColoredString(5);
            Assert.AreEqual(5, cs.Length);
            for (int i = 0; i < 5; i++)
                Assert.AreEqual(' ', cs[i].GlyphCharacter, $"Character at {i} should be space");
        }

        [TestMethod]
        public void Constructor_StringOnly_SetsCharacters()
        {
            var cs = new SadConsole.ColoredString("hello");
            Assert.AreEqual(5, cs.Length);
            Assert.AreEqual("hello", cs.String);
        }

        [TestMethod]
        public void Constructor_StringOnly_EmptyString_IsEmpty()
        {
            var cs = new SadConsole.ColoredString("");
            Assert.AreEqual(0, cs.Length);
        }

        [TestMethod]
        public void Constructor_TreatAsString_SetsAllIgnoreFlags()
        {
            var cs = new SadConsole.ColoredString("test", treatAsString: true);
            Assert.IsTrue(cs.IgnoreBackground, "IgnoreBackground should be true");
            Assert.IsTrue(cs.IgnoreDecorators, "IgnoreDecorators should be true");
            Assert.IsTrue(cs.IgnoreEffect, "IgnoreEffect should be true");
            Assert.IsTrue(cs.IgnoreForeground, "IgnoreForeground should be true");
            Assert.IsTrue(cs.IgnoreGlyph, "IgnoreGlyph should be true");
            Assert.IsTrue(cs.IgnoreMirror, "IgnoreMirror should be true");
        }

        [TestMethod]
        public void Constructor_WithColors_AppliesColorsToAll()
        {
            var cs = new SadConsole.ColoredString("abc", Color.Red, Color.Blue);
            Assert.AreEqual(3, cs.Length);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(Color.Red, cs[i].Foreground, $"Foreground at {i}");
                Assert.AreEqual(Color.Blue, cs[i].Background, $"Background at {i}");
            }
        }

        [TestMethod]
        public void Constructor_WithMirror_AppliesMirrorToAll()
        {
            var cs = new SadConsole.ColoredString("ab", Color.White, Color.Black, Mirror.Vertical);
            Assert.AreEqual(Mirror.Vertical, cs[0].Mirror);
            Assert.AreEqual(Mirror.Vertical, cs[1].Mirror);
        }

        [TestMethod]
        public void Constructor_FromColoredGlyphAndEffectArray_SetsCharacters()
        {
            var g1 = new ColoredGlyphAndEffect { GlyphCharacter = 'X', Foreground = Color.Cyan };
            var g2 = new ColoredGlyphAndEffect { GlyphCharacter = 'Y', Foreground = Color.Magenta };
            var cs = new SadConsole.ColoredString(g1, g2);
            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual('X', cs[0].GlyphCharacter);
            Assert.AreEqual('Y', cs[1].GlyphCharacter);
            Assert.AreEqual(Color.Cyan, cs[0].Foreground);
        }

        [TestMethod]
        public void Constructor_FromAppearance_UsesColorsFromAppearance()
        {
            var appearance = new ColoredGlyphAndEffect { Foreground = Color.Orange, Background = Color.Purple };
            var cs = new SadConsole.ColoredString("hi", appearance);
            Assert.AreEqual(Color.Orange, cs[0].Foreground);
            Assert.AreEqual(Color.Purple, cs[0].Background);
            Assert.AreEqual(Color.Orange, cs[1].Foreground);
        }

        // ── Properties ──────────────────────────────────────────────────────────

        [TestMethod]
        public void String_Setter_Expand_ClonesLastCharacter()
        {
            // Start with 2-char string, expand to 4
            var cs = new SadConsole.ColoredString("ab", Color.Red, Color.Blue);
            cs.String = "abcd";
            Assert.AreEqual(4, cs.Length);
            // Newly added chars should inherit color from the last original character
            Assert.AreEqual(Color.Red, cs[2].Foreground, "Expanded char should inherit foreground");
            Assert.AreEqual(Color.Blue, cs[2].Background, "Expanded char should inherit background");
        }

        [TestMethod]
        public void String_Setter_Shrink_ReducesLength()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            cs.String = "hi";
            Assert.AreEqual(2, cs.Length);
            Assert.AreEqual("hi", cs.String);
        }

        [TestMethod]
        public void String_Setter_NullOrEmpty_ClearsCharacters()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            cs.String = "";
            Assert.AreEqual(0, cs.Length);
        }

        [TestMethod]
        public void IgnoreDecorators_DefaultIsTrue()
        {
            var cs = new SadConsole.ColoredString("x");
            Assert.IsTrue(cs.IgnoreDecorators, "IgnoreDecorators should default to true");
        }

        [TestMethod]
        public void IgnoreFlags_DefaultAreFalse()
        {
            var cs = new SadConsole.ColoredString("x");
            Assert.IsFalse(cs.IgnoreGlyph, "IgnoreGlyph should default to false");
            Assert.IsFalse(cs.IgnoreForeground, "IgnoreForeground should default to false");
            Assert.IsFalse(cs.IgnoreBackground, "IgnoreBackground should default to false");
            Assert.IsFalse(cs.IgnoreEffect, "IgnoreEffect should default to false");
            Assert.IsFalse(cs.IgnoreMirror, "IgnoreMirror should default to false");
        }

        // ── Clone ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Clone_PreservesContent()
        {
            var original = new SadConsole.ColoredString("test", Color.Green, Color.Yellow);
            var clone = original.Clone();
            Assert.AreEqual(original.Length, clone.Length);
            Assert.AreEqual(original.String, clone.String);
            Assert.AreEqual(original[0].Foreground, clone[0].Foreground);
            Assert.AreEqual(original[0].Background, clone[0].Background);
        }

        [TestMethod]
        public void Clone_PreservesIgnoreFlags()
        {
            var original = new SadConsole.ColoredString("test") { IgnoreForeground = true, IgnoreBackground = true };
            var clone = original.Clone();
            Assert.IsTrue(clone.IgnoreForeground);
            Assert.IsTrue(clone.IgnoreBackground);
        }

        [TestMethod]
        public void Clone_IsDeepCopy_ChangingCloneDoesNotAffectOriginal()
        {
            var original = new SadConsole.ColoredString("test", Color.Green, Color.Yellow);
            var clone = original.Clone();
            clone[0].Foreground = Color.Red;
            Assert.AreEqual(Color.Green, original[0].Foreground, "Original should be unchanged after modifying clone");
        }

        // ── SubString ────────────────────────────────────────────────────────────

        [TestMethod]
        public void SubString_FromIndex_ReturnsRemainder()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            var sub = cs.SubString(2);
            Assert.AreEqual(3, sub.Length);
            Assert.AreEqual("llo", sub.String);
        }

        [TestMethod]
        public void SubString_Range_ReturnsCorrectSlice()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            var sub = cs.SubString(1, 3);
            Assert.AreEqual(3, sub.Length);
            Assert.AreEqual("ell", sub.String);
        }

        [TestMethod]
        public void SubString_PreservesColors()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            var sub = cs.SubString(0, 2);
            Assert.AreEqual(Color.Red, sub[0].Foreground);
            Assert.AreEqual(Color.Blue, sub[0].Background);
        }

        [TestMethod]
        public void SubString_PreservesIgnoreFlags()
        {
            var cs = new SadConsole.ColoredString("hello") { IgnoreForeground = true };
            var sub = cs.SubString(1, 2);
            Assert.IsTrue(sub.IgnoreForeground);
        }

        [TestMethod]
        public void SubString_OutOfRange_Throws()
        {
            var cs = new SadConsole.ColoredString("hi");
            bool threw = false;
            try { cs.SubString(0, 10); }
            catch (System.IndexOutOfRangeException) { threw = true; }
            Assert.IsTrue(threw, "Expected IndexOutOfRangeException");
        }

        // ── Set* methods ─────────────────────────────────────────────────────────

        [TestMethod]
        public void SetForeground_AppliesColorToAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc", Color.White, Color.Black);
            cs.SetForeground(Color.Cyan);
            for (int i = 0; i < cs.Length; i++)
                Assert.AreEqual(Color.Cyan, cs[i].Foreground, $"Foreground at {i}");
        }

        [TestMethod]
        public void SetBackground_AppliesColorToAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc", Color.White, Color.Black);
            cs.SetBackground(Color.Purple);
            for (int i = 0; i < cs.Length; i++)
                Assert.AreEqual(Color.Purple, cs[i].Background, $"Background at {i}");
        }

        [TestMethod]
        public void SetGlyph_AppliesGlyphToAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc");
            cs.SetGlyph(42);
            for (int i = 0; i < cs.Length; i++)
                Assert.AreEqual(42, cs[i].Glyph, $"Glyph at {i}");
        }

        [TestMethod]
        public void SetMirror_AppliesMirrorToAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc");
            cs.SetMirror(Mirror.Horizontal);
            for (int i = 0; i < cs.Length; i++)
                Assert.AreEqual(Mirror.Horizontal, cs[i].Mirror, $"Mirror at {i}");
        }

        [TestMethod]
        public void SetEffect_AppliesEffectToAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc");
            var effect = new SadConsole.Effects.Blink();
            cs.SetEffect(effect);
            for (int i = 0; i < cs.Length; i++)
                Assert.AreSame(effect, cs[i].Effect, $"Effect at {i}");
        }

        [TestMethod]
        public void SetEffect_Null_ClearsEffectOnAllCharacters()
        {
            var cs = new SadConsole.ColoredString("ab");
            cs.SetEffect(new SadConsole.Effects.Blink());
            cs.SetEffect(null);
            for (int i = 0; i < cs.Length; i++)
                Assert.IsNull(cs[i].Effect, $"Effect at {i} should be null");
        }

        // ── Operator ColoredString + ColoredString ───────────────────────────────

        [TestMethod]
        public void Operator_ColoredPlusColored_CombinesContent()
        {
            var str1 = new SadConsole.ColoredString("foo", Color.Red, Color.Black);
            var str2 = new SadConsole.ColoredString("bar", Color.Blue, Color.White);
            var result = str1 + str2;
            Assert.AreEqual(6, result.Length);
            Assert.AreEqual("foobar", result.String);
            Assert.AreEqual(Color.Red, result[0].Foreground, "First segment keeps its foreground");
            Assert.AreEqual(Color.Blue, result[3].Foreground, "Second segment keeps its foreground");
        }

        [TestMethod]
        public void Operator_ColoredPlusColored_IgnoreFlags_OnlyTrueWhenBothTrue()
        {
            var str1 = new SadConsole.ColoredString("a") { IgnoreForeground = true, IgnoreBackground = false };
            var str2 = new SadConsole.ColoredString("b") { IgnoreForeground = true, IgnoreBackground = true };
            var result = str1 + str2;
            Assert.IsTrue(result.IgnoreForeground, "IgnoreForeground should be true when both are true");
            Assert.IsFalse(result.IgnoreBackground, "IgnoreBackground should be false when one is false");
        }

        // ── Indexer ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void Indexer_Read_ReturnsCorrectGlyph()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Green, Color.Yellow);
            Assert.AreEqual('e', cs[1].GlyphCharacter);
        }

        [TestMethod]
        public void Indexer_Write_UpdatesGlyph()
        {
            var cs = new SadConsole.ColoredString("hello");
            var replacement = new ColoredGlyphAndEffect { GlyphCharacter = 'Z', Foreground = Color.Red };
            cs[2] = replacement;
            Assert.AreEqual('Z', cs[2].GlyphCharacter);
            Assert.AreEqual(Color.Red, cs[2].Foreground);
        }

        // ── Enumeration ──────────────────────────────────────────────────────────

        [TestMethod]
        public void Enumeration_IteratesAllCharacters()
        {
            var cs = new SadConsole.ColoredString("abc", Color.Red, Color.Blue);
            var collected = new List<ColoredGlyphAndEffect>();
            foreach (var g in cs)
                collected.Add(g);
            Assert.AreEqual(3, collected.Count);
            Assert.AreEqual('a', collected[0].GlyphCharacter);
            Assert.AreEqual('b', collected[1].GlyphCharacter);
            Assert.AreEqual('c', collected[2].GlyphCharacter);
        }

        // ── ToString ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void ToString_MatchesStringProperty()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Green, Color.Yellow);
            Assert.AreEqual(cs.String, cs.ToString());
        }

        [TestMethod]
        public void ToString_Empty_ReturnsEmptyString()
        {
            var cs = new SadConsole.ColoredString();
            Assert.AreEqual("", cs.ToString());
        }

        // ── FromGradient ─────────────────────────────────────────────────────────

        [TestMethod]
        public void FromGradient_CreatesCorrectLength()
        {
            var gradient = new Gradient(Color.Red, Color.Blue);
            var cs = SadConsole.ColoredString.FromGradient(gradient, "hello");
            Assert.AreEqual(5, cs.Length);
        }

        [TestMethod]
        public void FromGradient_SetsGlyphCharacters()
        {
            var gradient = new Gradient(Color.Red, Color.Blue);
            var cs = SadConsole.ColoredString.FromGradient(gradient, "abc");
            Assert.AreEqual("abc", cs.String);
        }

        [TestMethod]
        public void FromGradient_AppliesForegroundColors()
        {
            var gradient = new Gradient(Color.Red, Color.Blue);
            var cs = SadConsole.ColoredString.FromGradient(gradient, "ab");
            // First character should be closer to Red, last closer to Blue
            Assert.AreNotEqual(cs[0].Foreground, cs[1].Foreground, "Gradient should produce different foreground colors");
        }

        // ── Edge cases ───────────────────────────────────────────────────────────

        [TestMethod]
        public void SingleCharacter_LengthIsOne()
        {
            var cs = new SadConsole.ColoredString("x", Color.Red, Color.Blue);
            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual('x', cs[0].GlyphCharacter);
        }

        [TestMethod]
        public void Operator_ColoredPlusString_SingleCharacter()
        {
            var cs = new SadConsole.ColoredString("a", Color.Green, Color.Yellow);
            var result = cs + "b";
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("ab", result.String);
            Assert.AreEqual(Color.Green, result[1].Foreground, "Appended char inherits foreground of last colored char");
        }

        [TestMethod]
        public void SubString_EntireString_MatchesOriginal()
        {
            var cs = new SadConsole.ColoredString("hello", Color.Red, Color.Blue);
            var sub = cs.SubString(0, cs.Length);
            Assert.AreEqual(cs.Length, sub.Length);
            Assert.AreEqual(cs.String, sub.String);
        }

        [TestMethod]
        public void SubString_ZeroCount_ReturnsEmptyString()
        {
            var cs = new SadConsole.ColoredString("hello");
            var sub = cs.SubString(2, 0);
            Assert.AreEqual(0, sub.Length);
        }
    }
}
