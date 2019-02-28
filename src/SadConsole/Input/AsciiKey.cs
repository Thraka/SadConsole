using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using scases = System.Tuple<char, char>;
using ncases = System.Tuple<char, Microsoft.Xna.Framework.Input.Keys>;

namespace SadConsole.Input
{
    /// <summary>
    /// Represents the state of a single key.
    /// </summary>
    public struct AsciiKey
    {
        private const int CapOffset = (int) 'A' - (int) Keys.A;
        private const int LowerOffset = (int) 'a' - (int) Keys.A;


        // It will be nice when we can use modern Tuples here.
        static readonly Dictionary<Keys, scases> shiftKeyMappings = new Dictionary<Keys, scases>
        {
            {Keys.OemComma, new scases(',', '<')},
            {Keys.OemMinus, new scases('-', '_')},
            {Keys.OemOpenBrackets, new scases('[', '{')},
            {Keys.OemCloseBrackets, new scases(']', '}')},
            {Keys.OemPeriod, new scases('.', '>')},
            {Keys.OemBackslash, new scases('\\', '|')},
            {Keys.OemPipe, new scases('\\', '|')},
            {Keys.OemPlus, new scases('=', '+')},
            {Keys.OemQuestion, new scases('/', '?')},
            {Keys.OemQuotes, new scases('\'', '"')},
            {Keys.OemSemicolon, new scases(';', ':')},
            {Keys.OemTilde, new scases('`', '~')},
            {Keys.Space, new scases(' ', ' ')},
            {Keys.Divide, new scases('/', '/')},
            {Keys.Multiply, new scases('*', '*')},
            {Keys.Subtract, new scases('-', '-')},
            {Keys.Add, new scases('+', '+')},
            {Keys.D0, new scases('0', ')')},
            {Keys.D1, new scases('1', '!')},
            {Keys.D2, new scases('2', '@')},
            {Keys.D3, new scases('3', '#')},
            {Keys.D4, new scases('4', '$')},
            {Keys.D5, new scases('5', '%')},
            {Keys.D6, new scases('6', '^')},
            {Keys.D7, new scases('7', '&')},
            {Keys.D8, new scases('8', '*')},
            {Keys.D9, new scases('9', '(')},
        };

        private static readonly Dictionary<Keys, ncases> numKeyMappings = new Dictionary<Keys, ncases>
        {
            {Keys.Decimal, new ncases('.', Keys.Delete)},
            {Keys.NumPad0, new ncases('0', Keys.Insert)},
            {Keys.NumPad1, new ncases('1', Keys.End)},
            {Keys.NumPad2, new ncases('2', Keys.Down)},
            {Keys.NumPad3, new ncases('3', Keys.PageDown)},
            {Keys.NumPad4, new ncases('4', Keys.Left)},
            {Keys.NumPad5, new ncases('5', Keys.D5)},
            {Keys.NumPad6, new ncases('6', Keys.Right)},
            {Keys.NumPad7, new ncases('7', Keys.Home)},
            {Keys.NumPad8, new ncases('8', Keys.Up)},
            {Keys.NumPad9, new ncases('9', Keys.PageUp)},
        };

        /// <summary>
        /// The key from MonoGame or XNA.
        /// </summary>
        public Keys Key;

        /// <summary>
        /// The keyboard character of the key.
        /// </summary>
        public char Character;

        /// <summary>
        /// Total time the key has been held.
        /// </summary>
        public float TimeHeld;

        /// <summary>
        /// Tracks if the key was previously held when calculating the <see cref="Keyboard.InitialRepeatDelay"/>.
        /// </summary>
        public bool PostInitialDelay;

		/// <summary>
		///  Does any necessary remapping for virtual keys.
		/// </summary>
		/// <param name="key"> The key to be remapped. </param>
		/// <returns> The remapped key. </returns>
		public static Keys RemapVirtualKeys(Keys key, KeyboardState state)
        {
            var numLock = state.NumLock;
            if (numLock)
            {
                return key;
            }

            if (numKeyMappings.ContainsKey(key))
            {
                return numKeyMappings[key].Item2;
            }

            return key;
        }

        /// <summary>
        /// Fills out the fields based on the MonoGame/XNA key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">Helps identify which <see cref="Character"/> to use while the key is pressed. For example, if <see cref="Keys.A"/> is used the <see cref="Character"/> field will be either 'A' if <paramref name="shiftPressed"/> is true or 'a' if false.</param>
        public void Fill(Keys key, bool shiftPressed, KeyboardState state)
        {
            Key = key;
            var numLock = state.NumLock;

            if (key >= Keys.A && key <= Keys.Z)
            {
                var capsLock = state.CapsLock;
				Character = (char) (Key + (shiftPressed || capsLock ? CapOffset : LowerOffset));
                return;
            }

            if (shiftKeyMappings.ContainsKey(Key))
            {
                var casesCur = shiftKeyMappings[Key];
                Character = shiftPressed ? casesCur.Item2 : casesCur.Item1;
                return;
            }

            if (numKeyMappings.ContainsKey(Key))
            {
                var casesCur = numKeyMappings[Key];
                Character = numLock ? casesCur.Item1 : (char) 0;
                Key = RemapVirtualKeys(Key, state);
                return;
            }

            Character = (char) 0;
        }

        /// <summary>
        /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type. Shift is considered not pressed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
        public static AsciiKey Get(Keys key, KeyboardState state)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, false, state);
            return asciiKey;
        }

        /// <summary>
        /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">If shift should be considered pressed or not.</param>
        /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
        public static AsciiKey Get(Keys key, bool shiftPressed, KeyboardState state)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, shiftPressed, state);
            return asciiKey;
        }

        /// <summary>
        /// Checks if the two <see cref="AsciiKey"/> types use the same <see cref="Key"/> if the <see cref="Character"/> is 0. If the <see cref="Character"/> is not 0, the <see cref="Character"/> is compared.
        /// </summary>
        /// <param name="left">First item to compare.</param>
        /// <param name="right">Second item to compare.</param>
        /// <returns></returns>
        public static bool operator ==(AsciiKey left, AsciiKey right)
        {
            if (left.Character == (char) 0 && left.Character == right.Character)
                return left.Key == right.Key;
            return left.Character == right.Character;
        }

        /// <summary>
        /// Compares if the <see cref="Character"/> field of two <see cref="AsciiKey"/> instances are the same.
        /// </summary>
        /// <param name="left">First item to compare.</param>
        /// <param name="right">Second item to compare.</param>
        /// <returns></returns>
        public static bool operator !=(AsciiKey left, AsciiKey right)
        {
            return left.Character != right.Character;
        }


        /// <summary>
        /// Checks if the a <see cref="AsciiKey"/> type uses the indicated <see cref="Key"/>.
        /// </summary>
        /// <param name="left">The <see cref="AsciiKey"/> to compare.</param>
        /// <param name="right">The <see cref="Key"/> to compare.</param>
        /// <returns>True when <see cref="AsciiKey.Key"/> matches.</returns>
        public static bool operator ==(AsciiKey left, Keys right)
        {
            return left.Key == right;
        }

        /// <summary>
        /// Checks if the a <see cref="AsciiKey"/> type does not use the indicated <see cref="Key"/>.
        /// </summary>
        /// <param name="left">The <see cref="AsciiKey"/> to compare.</param>
        /// <param name="right">The <see cref="Key"/> to compare.</param>
        /// <returns>True when <see cref="AsciiKey.Key"/> does not match.</returns>
        public static bool operator !=(AsciiKey left, Keys right)
        {
            return left.Key != right;
        }

        /// <summary>
        /// Checks if the a <see cref="AsciiKey"/> type uses the indicated <see cref="Key"/>.
        /// </summary>
        /// <param name="left">The <see cref="Key"/> to compare.</param>
        /// <param name="right">The <see cref="AsciiKey"/> to compare.</param>
        /// <returns>True when <see cref="AsciiKey.Key"/> matches.</returns>
        public static bool operator ==(Keys left, AsciiKey right)
        {
            return left == right.Key;
        }

        /// <summary>
        /// Checks if the a <see cref="AsciiKey"/> type does not use the indicated <see cref="Key"/>.
        /// </summary>
        /// <param name="left">The <see cref="Key"/> to compare.</param>
        /// <param name="right">The <see cref="AsciiKey"/> to compare.</param>
        /// <returns>True when <see cref="AsciiKey.Key"/> does not match.</returns>
        public static bool operator !=(Keys left, AsciiKey right)
        {
            return left != right.Key;
        }

        /// <summary>
        /// Compares references.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is AsciiKey key)
                return key == this;

            return base.Equals(obj);
        }
    }
}
