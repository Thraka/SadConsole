using Microsoft.Xna.Framework;
using SadConsole.Effects;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;

namespace SadConsole
{
    /// <summary>
    /// Represents a string that has foreground and background colors for each character in the string.
    /// </summary>
    [DataContract]
    public class ColoredString : IEnumerable<ColoredCharacter>
    {
        private List<ColoredCharacter> _characters;

        public ColoredCharacter this[int index]
        {
            get { return _characters[index]; }
            set { _characters[index] = value; }
        }

        /// <summary>
        /// Gets or sets the string. When Set, the colors for each character default to the <see cref="SadConsole.ColoredString.Foreground"/> and <see cref="SadConsole.ColoredString.Background"/> property values.
        /// </summary>
        [DataMember]
        public string String
        {
            get
            {
                if (_characters.Count == 0)
                    return "";

                System.Text.StringBuilder sb = new System.Text.StringBuilder(_characters.Count);

                for (int i = 0; i < _characters.Count; i++)
                    sb.Append(_characters[i].Character);

                return sb.ToString();
            }
            set
            {
                _characters.Clear();
                _characters.Capacity = value.Length;

                for (int i = 0; i < value.Length; i++)
                {
                    var character = new ColoredCharacter();
                    character.Character = value[i];
                    character.Foreground = this.Foreground;
                    character.Background = this.Background;
                    character.Effect = this.Effect;
                    _characters.Add(character);
                }
            }
        }

        public int Count { get { return _characters.Count; } }

        /// <summary>
        /// The default foreground color for a new string.
        /// </summary>
        [DataMember]
        public Color Foreground = Color.White;

        /// <summary>
        /// The default background color for a new string.
        /// </summary>
        [DataMember]
        public Color Background = Color.Black;

        /// <summary>
        /// The default cell effect for the new string.
        /// </summary>
        [DataMember]
        public ICellEffect Effect;

        /// <summary>
        /// When true, instructs a caller to not render the <see cref="Character"/>.
        /// </summary>
        [DataMember]
        public bool IgnoreCharacter;

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
        public bool IgnoreEffect;

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
            _characters = new List<ColoredCharacter>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                _characters.Add(new ColoredCharacter() { Character = ' ', Background = this.Background, Foreground = this.Foreground, Effect = this.Effect });
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
        /// <param name="effect">The cell effect for each character.</param>
        public ColoredString(string value, Color foreground, Color background, ICellEffect effect)
        {
            this.Foreground = foreground;
            this.Background = background;
            this.Effect = effect;
            this.String = value;
        }

        /// <summary>
        /// Creates a new instance of the ColoredString class with the specified string value, foreground and background colors, and a cell effect.
        /// </summary>
        /// <param name="value">The backing string.</param>
        /// <param name="appearance">The appearance to use for each character.</param>
        public ColoredString(string value, CellAppearance appearance) : this(value, appearance.Foreground, appearance.Background, null) { }

        /// <summary>
        /// Updates the backing string foreground, background, and cell effect based on the current defaults.
        /// </summary>
        public void UpdateWithDefaults()
        {
            for (int i = 0; i < _characters.Count; i++)
            {
                var character = _characters[i];
                character.Foreground = this.Foreground;
                character.Background = this.Background;
                character.Effect = this.Effect;
            }
        }

        /// <summary>
        /// Returns a new <see cref="ColoredString"/> object using a substring of this instance.
        /// </summary>
        /// <param name="index">The index to copy the contents from.</param>
        /// <param name="count">The count of <see cref="ColoredCharacter"/> objects to copy.</param>
        /// <returns>A new <see cref="ColoredString"/> object.</returns>
        public ColoredString SubString(int index, int count)
        {
            if (index + count > _characters.Count)
                throw new System.IndexOutOfRangeException();

            ColoredString returnObject = new ColoredString(count);

            returnObject.IgnoreBackground = this.IgnoreBackground;
            returnObject.IgnoreForeground = this.IgnoreForeground;
            returnObject.IgnoreCharacter = this.IgnoreCharacter;
            returnObject.IgnoreEffect = this.IgnoreEffect;
            returnObject.Foreground = this.Foreground;
            returnObject.Background = this.Background;
            returnObject.Effect = this.Effect;

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

        public IEnumerator<ColoredCharacter> GetEnumerator()
        {
            return ((IEnumerable<ColoredCharacter>)_characters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ColoredCharacter>)_characters).GetEnumerator();
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

            returnString.IgnoreCharacter = string1.IgnoreCharacter && string1.IgnoreCharacter;
            returnString.IgnoreForeground = string1.IgnoreForeground && string1.IgnoreForeground;
            returnString.IgnoreBackground = string1.IgnoreBackground && string1.IgnoreBackground;
            returnString.IgnoreEffect = string1.IgnoreEffect && string1.IgnoreEffect;

            return returnString;
        }
    }

    /// <summary>
    /// Represents a single character that has a foreground and background color.
    /// </summary>
    public class ColoredCharacter: CellAppearance
    {
        private char _character;

        /// <summary>
        /// The character.
        /// </summary>
        public char Character
        {
            get { return _character; }
            set
            {
                _character = value;
                base.CharacterIndex = _character;
            }
        }

        /// <summary>
        /// The effect for the character.
        /// </summary>
        public ICellEffect Effect;

        /// <summary>
        /// Creates a new copy of this cell appearance.
        /// </summary>
        /// <returns>The cloned cell appearance.</returns>
        public new ColoredCharacter Clone()
        {
            return new ColoredCharacter() { Foreground = this.Foreground, Background = this.Background, Effect = this.Effect != null ? this.Effect.Clone() : null, Character = this.Character };
        }
    }
}
