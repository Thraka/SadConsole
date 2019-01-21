using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using cases=System.Tuple<char, char>;

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

        static readonly Dictionary<Keys, cases> keyMappings = new Dictionary<Keys, cases>
        {
            {Keys.OemComma,             new cases(',', '<') },
            {Keys.OemMinus,             new cases('-', '_') },
            {Keys.OemOpenBrackets,      new cases('[', '{') },
            {Keys.OemCloseBrackets,     new cases(']', '}') },
            {Keys.OemPeriod,            new cases('.', '>') },
			{Keys.OemBackslash,         new cases('\\', '|') },
			{Keys.OemPipe,              new cases('\\', '|') },
			{Keys.OemPlus,              new cases('=', '+') },
			{Keys.OemQuestion,          new cases('/', '?') },
			{Keys.OemQuotes,            new cases('\'', '"') },
			{Keys.OemSemicolon,         new cases(';', ':') },
			{Keys.OemTilde,             new cases('`', '~') },
			{Keys.Space,                new cases(' ', ' ') },
			{Keys.Decimal,              new cases('.', '.') },
            {Keys.Divide,               new cases('/', '/') },
			{Keys.Multiply,             new cases('*', '*') },
			{Keys.Subtract,             new cases('-', '-') },
			{Keys.Add,                  new cases('+', '+') },
			{Keys.D0,                   new cases('0', ')') },
            {Keys.D1,                   new cases('1', '!') },
			{Keys.D2,                   new cases('2', '@') },
			{Keys.D3,                   new cases('3', '#') },
			{Keys.D4,                   new cases('4', '$') },
			{Keys.D5,                   new cases('5', '%') },
			{Keys.D6,                   new cases('6', '^') },
			{Keys.D7,                   new cases('7', '&') },
			{Keys.D8,                   new cases('8', '*') },
            {Keys.D9,                   new cases('9', '(') },
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
        /// Tracks if the key was previously held when calcualting the <see cref="Keyboard.InitialRepeatDelay"/>.
        /// </summary>
        public bool PreviouslyPressed;

        /// <summary>
        /// Fills out the fields based on the MonoGame/XNA key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">Helps identify which <see cref="Character"/> to use while the key is pressed. For example, if <see cref="Keys.A"/> is used the <see cref="Character"/> field will be either 'A' if <paramref name="shiftPressed"/> is true or 'a' if false.</param>
        public void Fill(Keys key, bool shiftPressed)
        {
            Key = key;
            shiftPressed |= (((ushort)GetKeyState(VK_CAPSLOCK)) & 0xffff) != 0;

			if (key >= Keys.A && key <= Keys.Z)
            {
                Character = (char)(Key + (shiftPressed ? capOffset : lowerOffset));
                return;
            }

            if (keyMappings.ContainsKey(Key))
            {
                var casesCur = keyMappings[Key];
                Character = shiftPressed ? casesCur.Item2 : casesCur.Item1;
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
