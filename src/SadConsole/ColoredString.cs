#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using SadConsole.Effects;
    using SadConsole.StringParser;

    /// <summary>
    /// Represents a string that has foreground and background colors for each character in the string.
    /// </summary>
    public partial class ColoredString : IEnumerable<ColoredGlyph>
    {
        private ColoredGlyph[] _characters;

        /// <summary>
        /// Gets a <see cref="ColoredGlyph"/> from the string.
        /// </summary>
        /// <param name="index">The index in the string of the <see cref="ColoredGlyph"/>.</param>
        /// <returns>The colored glyph representing the character in the string.</returns>
        public ColoredGlyph this[int index]
        {
            get => _characters[index];
            set => _characters[index] = value;
        }

        /// <summary>
        /// Gets or sets the characters represneting this string. When set, first processes the string through <see cref="Parse(string, int, CellSurface, ParseCommandStacks)"/>.
        /// </summary>
        public string String
        {
            get
            {
                if (_characters.Length == 0)
                {
                    return "";
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder(_characters.Length);

                for (int i = 0; i < _characters.Length; i++)
                {
                    sb.Append(_characters[i].GlyphCharacter);
                }

                return sb.ToString();
            }

            set => _characters = ColoredString.Parse(value)._characters;
        }

        /// <summary>
        /// The total number of <see cref="ColoredGlyph"/> characters in the string.
        /// </summary>
        public int Count => _characters.Length;

        /// <summary>
        /// When true, instructs a caller to not render the glyphs of the string.
        /// </summary>
        [DataMember]
        public bool IgnoreGlyph;

        /// <summary>
        /// When true, instructs a caller to not render the foreground color.
        /// </summary>
        [DataMember]
        public bool IgnoreForeground;

        /// <summary>
        /// When true, instructs a caller to not render the background color.
        /// </summary>
        [DataMember]
        public bool IgnoreBackground;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="Effect"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreEffect = true;

        /// <summary>
        /// When true, instructs a caller to not render the mirror state.
        /// </summary>
        [DataMember]
        public bool IgnoreMirror;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColoredString() { }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified blank characters.
        /// </summary>
        /// <param name="capacity">The number of blank characters.</param>
        public ColoredString(int capacity)
        {
            _characters = new ColoredGlyph[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _characters[i] = new ColoredGlyph() { GlyphCharacter = ' ' };
            }
        }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value. Calls <see cref="Parse(string, int, CellSurface, ParseCommandStacks)"/> first to process the string.
        /// </summary>
        /// <param name="value">The backing string.</param>
        public ColoredString(string value) => String = value;

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value, foreground and background colors, and a cell effect.
        /// </summary>
        /// <param name="value">The backing string.</param>
        /// <param name="foreground">The foreground color for each character.</param>
        /// <param name="background">The background color for each character.</param>
        /// <param name="mirror">The mirror for each character.</param>
        public ColoredString(string value, Color foreground, Color background, SpriteEffects mirror = SpriteEffects.None)
        {
            var stacks = new ParseCommandStacks();
            stacks.AddSafe(new ParseCommandRecolor() { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground });
            stacks.AddSafe(new ParseCommandRecolor() { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background });
            stacks.AddSafe(new ParseCommandMirror() { Mirror = mirror, CommandType = CommandTypes.Mirror });
            _characters = Parse(value, initialBehaviors: stacks)._characters;
        }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value, foreground and background colors, and a cell effect.
        /// </summary>
        /// <param name="value">The backing string.</param>
        /// <param name="appearance">The appearance to use for each character.</param>
        public ColoredString(string value, Cell appearance) : this(value, appearance.Foreground, appearance.Background, appearance.Mirror) { }

        /// <summary>
        /// Combines a <see cref="ColoredGlyph"/> array into a <see cref="ColoredString"/>.
        /// </summary>
        /// <param name="glyphs">The glyphs to combine.</param>
        public ColoredString(params ColoredGlyph[] glyphs) => _characters = glyphs.ToArray();

        /// <summary>
        /// Returns a new <see cref="ColoredString"/> object by cloning this instance.
        /// </summary>
        /// <returns>A new <see cref="ColoredString"/> object.</returns>
        public ColoredString Clone()
        {
            ColoredString returnObject = new ColoredString(_characters.Length)
            {
                IgnoreBackground = IgnoreBackground,
                IgnoreForeground = IgnoreForeground,
                IgnoreGlyph = IgnoreGlyph,
                IgnoreEffect = IgnoreEffect
            };

            for (int i = 0; i < _characters.Length; i++)
            {
                returnObject._characters[i] = _characters[i].Clone();
            }
            return returnObject;
        }

        /// <summary>
        /// Returns a new <see cref="ColoredString"/> object using a substring of this instance from the index to the end.
        /// </summary>
        /// <param name="index">The index to copy the contents from.</param>
        /// <returns>A new <see cref="ColoredString"/> object.</returns>
        public ColoredString SubString(int index) => SubString(index, _characters.Length - index);

        /// <summary>
        /// Returns a new <see cref="ColoredString"/> object using a substring of this instance.
        /// </summary>
        /// <param name="index">The index to copy the contents from.</param>
        /// <param name="count">The count of <see cref="ColoredGlyph"/> objects to copy.</param>
        /// <returns>A new <see cref="ColoredString"/> object.</returns>
        public ColoredString SubString(int index, int count)
        {
            if (index + count > _characters.Length)
            {
                throw new System.IndexOutOfRangeException();
            }

            ColoredString returnObject = new ColoredString(count)
            {
                IgnoreBackground = IgnoreBackground,
                IgnoreForeground = IgnoreForeground,
                IgnoreGlyph = IgnoreGlyph,
                IgnoreEffect = IgnoreEffect
            };

            for (int i = 0; i < count; i++)
            {
                returnObject._characters[i] = _characters[i + index].Clone();
            }

            return returnObject;
        }

        /// <summary>
        /// Applies the referenced cell effect to every character in the colored string.
        /// </summary>
        /// <param name="effect">The effect to apply.</param>
        public void SetEffect(ICellEffect effect)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _characters[i].Effect = effect;
            }
        }

        /// <summary>
        /// Applies the referenced color to every character foreground in the colored string.
        /// </summary>
        /// <param name="color">The color to apply.</param>
        public void SetForeground(Color color)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _characters[i].Foreground = color;
            }
        }

        /// <summary>
        /// Applies the referenced color to every character background in the colored string.
        /// </summary>
        /// <param name="color">The color to apply.</param>
        public void SetBackground(Color color)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _characters[i].Background = color;
            }
        }

        /// <summary>
        /// Returns a string representing the glyphs in this object.
        /// </summary>
        /// <returns>A string composed of each glyph in this object.</returns>
        public override string ToString() => String;

        /// <summary>
        /// Gets an enumerator for the <see cref="ColoredGlyph"/> objects in this string.
        /// </summary>
        /// <returns>The enumerator in the string.</returns>
        public IEnumerator<ColoredGlyph> GetEnumerator() => ((IEnumerable<ColoredGlyph>)_characters).GetEnumerator();

        /// <summary>
        /// Gets an enumerator for the <see cref="ColoredGlyph"/> objects in this string.
        /// </summary>
        /// <returns>The enumerator in the string.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ColoredGlyph>)_characters).GetEnumerator();

        /// <summary>
        /// Combines two ColoredString objects into a single ColoredString object. Ignore* values are only copied when both strings Ignore* values match.
        /// </summary>
        /// <param name="string1">The left-side of the string.</param>
        /// <param name="string2">The right-side of the string.</param>
        /// <returns></returns>
        public static ColoredString operator +(ColoredString string1, ColoredString string2)
        {
            ColoredString returnString = new ColoredString(string1.Count + string2.Count);

            for (int i = 0; i < string1.Count; i++)
            {
                returnString._characters[i] = string1._characters[i].Clone();
            }

            for (int i = 0; i < string2.Count; i++)
            {
                returnString._characters[i + string1.Count] = string2._characters[i].Clone();
            }

            returnString.IgnoreGlyph = string1.IgnoreGlyph && string2.IgnoreGlyph;
            returnString.IgnoreForeground = string1.IgnoreForeground && string2.IgnoreForeground;
            returnString.IgnoreBackground = string1.IgnoreBackground && string2.IgnoreBackground;
            returnString.IgnoreEffect = string1.IgnoreEffect && string2.IgnoreEffect;

            return returnString;
        }

        /// <summary>
        /// Combines a colored string and string. The last colored glyph in the colored string is used for all of the characters in the added string.
        /// </summary>
        /// <param name="string1">The colored string.</param>
        /// <param name="string2">The string.</param>
        /// <returns>A new colored string instance.</returns>
        public static ColoredString operator +(ColoredString string1, string string2)
        {

            var returnString = new ColoredString(string1.Count + string2.Length);

            for (int i = 0; i < string1.Count; i++)
            {
                returnString._characters[i] = string1._characters[i].Clone();
            }

            ColoredGlyph templateCharacter = string1[string1.Count - 1];

            for (int i = 0; i < string2.Length; i++)
            {
                ColoredGlyph newChar = templateCharacter.Clone();
                newChar.GlyphCharacter = string2[i];
                returnString._characters[i + string1.Count] = newChar;
            }

            returnString.IgnoreGlyph = string1.IgnoreGlyph;
            returnString.IgnoreForeground = string1.IgnoreForeground;
            returnString.IgnoreBackground = string1.IgnoreBackground;
            returnString.IgnoreEffect = string1.IgnoreEffect;

            return returnString;
        }

        /// <summary>
        /// Combines a string and a colored string. The first colored glyph in the colored string is used for all of the characters in the added string.
        /// </summary>
        /// <param name="string1">The string.</param>
        /// <param name="string2">The colored string.</param>
        /// <returns>A new colored string instance.</returns>
        public static ColoredString operator +(string string1, ColoredString string2)
        {
            var returnString = new ColoredString(string1, string2[0])
            {
                IgnoreGlyph = string2.IgnoreGlyph,
                IgnoreForeground = string2.IgnoreForeground,
                IgnoreBackground = string2.IgnoreBackground,
                IgnoreEffect = string2.IgnoreEffect
            };

            return returnString + string2;
        }
    }
}
