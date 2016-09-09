#if SFML
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

using SadConsole.Effects;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;
using SadConsole.StringParser;

namespace SadConsole
{
    /// <summary>
    /// Represents a string that has foreground and background colors for each character in the string.
    /// </summary>
    [DataContract]
    public partial class ColoredString : IEnumerable<ColoredGlyph>
    {
        [DataMember(Name = "Characters")]
        private List<ColoredGlyph> _characters;

        public ColoredGlyph this[int index]
        {
            get { return _characters[index]; }
            set { _characters[index] = value; }
        }

        /// <summary>
        /// Gets or sets the string. When Set, the colors for each character default to the <see cref="SadConsole.ColoredString.Foreground"/> and <see cref="SadConsole.ColoredString.Background"/> property values.
        /// </summary>
        public string String
        {
            get
            {
                if (_characters.Count == 0)
                    return "";

                System.Text.StringBuilder sb = new System.Text.StringBuilder(_characters.Count);

                for (int i = 0; i < _characters.Count; i++)
                    sb.Append(_characters[i].Glyph);

                return sb.ToString();
            }
            set
            {
                _characters = ColoredString.Parse(value)._characters;
            }
        }

        public int Count { get { return _characters.Count; } }
        
        /// <summary>
        /// When true, instructs a caller to not render the glyphs of the string.
        /// </summary>
        [DataMember]
        public bool IgnoreGlyph;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="Foreground"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreForeground;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="Background"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreBackground;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="Effect"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreEffect = true;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="SpriteEffects"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreSpriteEffect;

        /// <summary>
        /// Default contructor.
        /// </summary>
        public ColoredString() { }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified blank characters.
        /// </summary>
        /// <param name="capacity">The number of blank characters.</param>
        public ColoredString(int capacity)
        {
            _characters = new List<ColoredGlyph>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                _characters.Add(new ColoredGlyph() { Glyph = ' ' });
            }
        }
        
        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value.
        /// </summary>
        /// <param name="value">The backing string.</param>
        public ColoredString(string value) { String = value; }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value, foreground and background colors, and a cell effect.
        /// </summary>
        /// <param name="value">The backing string.</param>
        /// <param name="foreground">The foreground color for each character.</param>
        /// <param name="background">The background color for each character.</param>
        /// <param name="spriteEffect">The sprite effects for each character.</param>
        public ColoredString(string value, Color foreground, Color background, SpriteEffects spriteEffect = SpriteEffects.None)
        {
            var stacks = new ParseCommandStacks();
            stacks.AddSafe(new ParseCommandRecolor() { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground });
            stacks.AddSafe(new ParseCommandRecolor() { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background });
            stacks.AddSafe(new ParseCommandSpriteEffect() { Effect = spriteEffect, CommandType = CommandTypes.SpriteEffect });
            _characters = ColoredString.Parse(value, initialBehaviors: stacks)._characters;
        }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value, foreground and background colors, and a cell effect.
        /// </summary>
        /// <param name="value">The backing string.</param>
        /// <param name="appearance">The appearance to use for each character.</param>
        public ColoredString(string value, CellAppearance appearance) : this(value, appearance.Foreground, appearance.Background, appearance.SpriteEffect) { }

        /// <summary>
        /// Combines a <see cref="ColoredGlyph"/> array into a <see cref="ColoredString"/>.
        /// </summary>
        /// <param name="glyphs">The glyphs to combine.</param>
        public ColoredString(ColoredGlyph[] glyphs)
        {
            _characters = new List<ColoredGlyph>(glyphs);
        }

        /// <summary>
        /// Returns a new <see cref="ColoredString"/> object using a substring of this instance.
        /// </summary>
        /// <param name="index">The index to copy the contents from.</param>
        /// <param name="count">The count of <see cref="ColoredGlyph"/> objects to copy.</param>
        /// <returns>A new <see cref="ColoredString"/> object.</returns>
        public ColoredString SubString(int index, int count)
        {
            if (index + count > _characters.Count)
                throw new System.IndexOutOfRangeException();

            ColoredString returnObject = new ColoredString(count);

            returnObject.IgnoreBackground = this.IgnoreBackground;
            returnObject.IgnoreForeground = this.IgnoreForeground;
            returnObject.IgnoreGlyph = this.IgnoreGlyph;
            returnObject.IgnoreEffect = this.IgnoreEffect;

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
            for (int i = 0; i < _characters.Count; i++)
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
            for (int i = 0; i < _characters.Count; i++)
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
            for (int i = 0; i < _characters.Count; i++)
            {
                _characters[i].Background = color;
            }
        }

        public override string ToString()
        {
            return this.String;
        }


        public IEnumerator<ColoredGlyph> GetEnumerator()
        {
            return ((IEnumerable<ColoredGlyph>)_characters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ColoredGlyph>)_characters).GetEnumerator();
        }


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
                returnString._characters[i] = string1._characters[i].Clone();

            for (int i = 0; i < string2.Count; i++)
                returnString._characters[i + string1.Count] = string2._characters[i].Clone();

            returnString.IgnoreGlyph = string1.IgnoreGlyph && string1.IgnoreGlyph;
            returnString.IgnoreForeground = string1.IgnoreForeground && string1.IgnoreForeground;
            returnString.IgnoreBackground = string1.IgnoreBackground && string1.IgnoreBackground;
            returnString.IgnoreEffect = string1.IgnoreEffect && string1.IgnoreEffect;

            return returnString;
        }
        
    }

    /// <summary>
    /// Represents a single character that has a foreground and background color.
    /// </summary>
    [DataContract]
    public class ColoredGlyph: CellAppearance
    {
        [DataMember(Name = "Glyph")]
        private char _character;

        /// <summary>
        /// The glyph.
        /// </summary>
        public char Glyph
        {
            get { return _character; }
            set
            {
                _character = value;
                base.GlyphIndex = _character;
            }
        }

        /// <summary>
        /// Sets the glyph by index.
        /// </summary>
        public new int GlyphIndex
        {
            get { return base.GlyphIndex; }
            set { Glyph = (char)value; }
        }

        /// <summary>
        /// The effect for the glyph.
        /// </summary>
        [DataMember]
        public ICellEffect Effect;

        /// <summary>
        /// Creates a new colored glyph with a white foreground, transparent background, and a glyph index of 0.
        /// </summary>
        public ColoredGlyph() { }

        /// <summary>
        /// Creates a new colored glyph based on the provided cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public ColoredGlyph(Cell cell)
        {
            cell.CopyAppearanceTo(this);
            Glyph = (char)cell.GlyphIndex;
        }

        /// <summary>
        /// Creates a new copy of this cell appearance.
        /// </summary>
        /// <returns>The cloned cell appearance.</returns>
        public new ColoredGlyph Clone()
        {
            return new ColoredGlyph() { Foreground = this.Foreground, Background = this.Background, Effect = this.Effect != null ? this.Effect.Clone() : null, Glyph = this.Glyph , SpriteEffect = this.SpriteEffect };
        }
    }
}
