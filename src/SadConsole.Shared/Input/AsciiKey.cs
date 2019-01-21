using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using scases=System.Tuple<char, char>;
using ncases = System.Tuple<char, Microsoft.Xna.Framework.Input.Keys>;

namespace SadConsole.Input
{
    /// <summary>
    /// Represents the state of a single key.
    /// </summary>
    public struct AsciiKey
    {
        private const int VK_NUMLOCK = 0x90;
        private const int VK_SCROLLLOCK = 0x91;     // Don't need it now, but who knows what tomorrow brings?
        private const int VK_CAPSLOCK = 0x14;

        const int capOffset = (int)'A' - (int)Keys.A;
        const int lowerOffset = (int)'a' - (int)Keys.A;

		[DllImport("user32.dll")]
        static extern short GetKeyState(int keyCode);
        
        // It will be nice when we can use modern Tuples here.
        static readonly Dictionary<Keys, scases> shiftKeyMappings = new Dictionary<Keys, scases>
        {
            {Keys.OemComma,             new scases(',', '<') },
            {Keys.OemMinus,             new scases('-', '_') },
            {Keys.OemOpenBrackets,      new scases('[', '{') },
            {Keys.OemCloseBrackets,     new scases(']', '}') },
            {Keys.OemPeriod,            new scases('.', '>') },
			{Keys.OemBackslash,         new scases('\\', '|') },
			{Keys.OemPipe,              new scases('\\', '|') },
			{Keys.OemPlus,              new scases('=', '+') },
			{Keys.OemQuestion,          new scases('/', '?') },
			{Keys.OemQuotes,            new scases('\'', '"') },
			{Keys.OemSemicolon,         new scases(';', ':') },
			{Keys.OemTilde,             new scases('`', '~') },
			{Keys.Space,                new scases(' ', ' ') },
            {Keys.Divide,               new scases('/', '/') },
			{Keys.Multiply,             new scases('*', '*') },
			{Keys.Subtract,             new scases('-', '-') },
			{Keys.Add,                  new scases('+', '+') },
            {Keys.D0,                   new scases('0', ')') },
            {Keys.D1,                   new scases('1', '!') },
            {Keys.D2,                   new scases('2', '@') },
            {Keys.D3,                   new scases('3', '#') },
            {Keys.D4,                   new scases('4', '$') },
            {Keys.D5,                   new scases('5', '%') },
            {Keys.D6,                   new scases('6', '^') },
            {Keys.D7,                   new scases('7', '&') },
            {Keys.D8,                   new scases('8', '*') },
            {Keys.D9,                   new scases('9', '(') },
        };

        private static readonly Dictionary<Keys, ncases> numKeyMappings = new Dictionary<Keys, ncases>
        {
			{Keys.Decimal,              new ncases('.', Keys.Delete) },
			{Keys.NumPad0,              new ncases('0', Keys.Insert) },
            {Keys.NumPad1,              new ncases('1', Keys.End) },
			{Keys.NumPad2,              new ncases('2', Keys.Down) },
			{Keys.NumPad3,              new ncases('3', Keys.PageDown) },
			{Keys.NumPad4,              new ncases('4', Keys.Left) },
			{Keys.NumPad5,              new ncases('5', Keys.D5) },
			{Keys.NumPad6,              new ncases('6', Keys.Right) },
			{Keys.NumPad7,              new ncases('7', Keys.Home) },
			{Keys.NumPad8,              new ncases('8', Keys.Up) },
            {Keys.NumPad9,              new ncases('9', Keys.PageUp) },
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>	Remap virtual keys. </summary>
        ///
        /// <remarks>	Does any necessary remapping for virtual key.  Currently, changes keypad virtual keys
        /// 			to their non-numeric interpretations is numlock is turned off.
        /// 			Darrell Plank, 1/21/2019. </remarks>
        ///
        /// <param name="key">	The key. </param>
        ///
        /// <returns>	The Keys. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Keys RemapVirtualKeys(Keys key)
        {
            var numLock = (((ushort)GetKeyState(VK_NUMLOCK)) & 0xffff) != 0;
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
		public void Fill(Keys key, bool shiftPressed)
        {
            Key = key;
            shiftPressed |= (((ushort)GetKeyState(VK_CAPSLOCK)) & 0xffff) != 0;
            var numLock = (((ushort)GetKeyState(VK_NUMLOCK)) & 0xffff) != 0;

			if (key >= Keys.A && key <= Keys.Z)
            {
                Character = (char)(Key + (shiftPressed ? capOffset : lowerOffset));
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
                Key = RemapVirtualKeys(Key);
                return;
            }
            Character = (char) 0;
        }

        /// <summary>
        /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type. Shift is considered not pressed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
        public static AsciiKey Get(Keys key)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, false);
            return asciiKey;
        }

        /// <summary>
        /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">If shift should be considered pressed or not.</param>
        /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
        public static AsciiKey Get(Keys key, bool shiftPressed)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, shiftPressed);
            return asciiKey;
        }

        /// <summary>
        /// Checks if the two <see cref="AsciiKey"/> types use the same <see cref="Key"/> if the <see cref="Character"/> is 0. If the <see cref="Character"/> is not 0, the <see cref="Character"/> is compared.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AsciiKey left, AsciiKey right)
        {
            if (left.Character == (char)0 && left.Character == right.Character)
                return left.Key == right.Key;
            return left.Character == right.Character;
        }

        /// <summary>
        /// Compares if the <see cref="Character"/> field of two <see cref="AsciiKey"/> instances are the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(AsciiKey left, AsciiKey right)
        {
            return left.Character != right.Character;
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
