using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace SadConsole.Input
{
    /// <summary>
    /// Represents the state of a single key.
    /// </summary>
    public struct AsciiKey
    {
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
            this.Key = key;

            switch (key)
            {
                case Keys.A:
                    if (shiftPressed)
                        this.Character = (char)65;
                    else
                        this.Character = (char)97;
                    break;
                case Keys.B:
                    if (shiftPressed)
                        this.Character = (char)66;
                    else
                        this.Character = (char)98;
                    break;
                case Keys.C:
                    if (shiftPressed)
                        this.Character = (char)67;
                    else
                        this.Character = (char)99;
                    break;
                case Keys.D:
                    if (shiftPressed)
                        this.Character = (char)68;
                    else
                        this.Character = (char)100;
                    break;
                case Keys.E:
                    if (shiftPressed)
                        this.Character = (char)69;
                    else
                        this.Character = (char)101;
                    break;
                case Keys.F:
                    if (shiftPressed)
                        this.Character = (char)70;
                    else
                        this.Character = (char)102;
                    break;
                case Keys.G:
                    if (shiftPressed)
                        this.Character = (char)71;
                    else
                        this.Character = (char)103;
                    break;
                case Keys.H:
                    if (shiftPressed)
                        this.Character = (char)72;
                    else
                        this.Character = (char)104;
                    break;
                case Keys.I:
                    if (shiftPressed)
                        this.Character = (char)73;
                    else
                        this.Character = (char)105;
                    break;
                case Keys.J:
                    if (shiftPressed)
                        this.Character = (char)74;
                    else
                        this.Character = (char)106;
                    break;
                case Keys.K:
                    if (shiftPressed)
                        this.Character = (char)75;
                    else
                        this.Character = (char)107;
                    break;
                case Keys.L:
                    if (shiftPressed)
                        this.Character = (char)76;
                    else
                        this.Character = (char)108;
                    break;
                case Keys.M:
                    if (shiftPressed)
                        this.Character = (char)77;
                    else
                        this.Character = (char)109;
                    break;
                case Keys.N:
                    if (shiftPressed)
                        this.Character = (char)78;
                    else
                        this.Character = (char)110;
                    break;
                case Keys.O:
                    if (shiftPressed)
                        this.Character = (char)79;
                    else
                        this.Character = (char)111;
                    break;
                case Keys.P:
                    if (shiftPressed)
                        this.Character = (char)80;
                    else
                        this.Character = (char)112;
                    break;
                case Keys.Q:
                    if (shiftPressed)
                        this.Character = (char)81;
                    else
                        this.Character = (char)113;
                    break;
                case Keys.R:
                    if (shiftPressed)
                        this.Character = (char)82;
                    else
                        this.Character = (char)114;
                    break;
                case Keys.S:
                    if (shiftPressed)
                        this.Character = (char)83;
                    else
                        this.Character = (char)115;
                    break;
                case Keys.T:
                    if (shiftPressed)
                        this.Character = (char)84;
                    else
                        this.Character = (char)116;
                    break;
                case Keys.U:
                    if (shiftPressed)
                        this.Character = (char)85;
                    else
                        this.Character = (char)117;
                    break;
                case Keys.V:
                    if (shiftPressed)
                        this.Character = (char)86;
                    else
                        this.Character = (char)118;
                    break;
                case Keys.W:
                    if (shiftPressed)
                        this.Character = (char)87;
                    else
                        this.Character = (char)119;
                    break;
                case Keys.X:
                    if (shiftPressed)
                        this.Character = (char)88;
                    else
                        this.Character = (char)120;
                    break;
                case Keys.Y:
                    if (shiftPressed)
                        this.Character = (char)89;
                    else
                        this.Character = (char)121;
                    break;
                case Keys.Z:
                    if (shiftPressed)
                        this.Character = (char)90;
                    else
                        this.Character = (char)122;
                    break;
                case Keys.OemComma:
                    if (shiftPressed)
                        this.Character = (char)60;
                    else
                        this.Character = (char)44;
                    break;
                case Keys.OemMinus:
                    if (shiftPressed)
                        this.Character = (char)95;
                    else
                        this.Character = (char)45;
                    break;
                case Keys.OemOpenBrackets:
                    if (shiftPressed)
                        this.Character = (char)91;
                    else
                        this.Character = (char)123;
                    break;
                case Keys.OemCloseBrackets:
                    if (shiftPressed)
                        this.Character = (char)93;
                    else
                        this.Character = (char)125;
                    break;
                case Keys.OemPeriod:
                    if (shiftPressed)
                        this.Character = (char)62;
                    else
                        this.Character = (char)46;
                    break;
                case Keys.OemBackslash:
                case Keys.OemPipe:
                    if (shiftPressed)
                        this.Character = (char)124;
                    else
                        this.Character = (char)92;
                    break;
                case Keys.OemPlus:
                    if (shiftPressed)
                        this.Character = (char)43;
                    else
                        this.Character = (char)61;
                    break;
                case Keys.OemQuestion:
                    if (shiftPressed)
                        this.Character = (char)63;
                    else
                        this.Character = (char)47;
                    break;
                case Keys.OemQuotes:
                    if (shiftPressed)
                        this.Character = (char)34;
                    else
                        this.Character = (char)39;
                    break;
                case Keys.OemSemicolon:
                    if (shiftPressed)
                        this.Character = (char)58;
                    else
                        this.Character = (char)59;
                    break;
                case Keys.OemTilde:
                    if (shiftPressed)
                        this.Character = (char)126;
                    else
                        this.Character = (char)96;
                    break;

                case Keys.Space:
                    this.Character = ' ';
                    break;
                case Keys.Decimal:
                    this.Character = (char)46;
                    break;
                case Keys.Divide:
                    this.Character = (char)47;
                    break;
                case Keys.Multiply:
                    this.Character = (char)42;
                    break;
                case Keys.Subtract:
                    this.Character = (char)45;
                    break;
                case Keys.Add:
                    this.Character = (char)43;
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                    if (shiftPressed)
                        this.Character = (char)41;
                    else
                        this.Character = (char)48;
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    if (shiftPressed)
                        this.Character = (char)33;
                    else
                        this.Character = (char)49;
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    if (shiftPressed)
                        this.Character = (char)64;
                    else
                        this.Character = (char)50;
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    if (shiftPressed)
                        this.Character = (char)35;
                    else
                        this.Character = (char)51;
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    if (shiftPressed)
                        this.Character = (char)36;
                    else
                        this.Character = (char)52;
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    if (shiftPressed)
                        this.Character = (char)37;
                    else
                        this.Character = (char)53;
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    if (shiftPressed)
                        this.Character = (char)94;
                    else
                        this.Character = (char)54;
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    if (shiftPressed)
                        this.Character = (char)38;
                    else
                        this.Character = (char)55;
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    if (shiftPressed)
                        this.Character = (char)42;
                    else
                        this.Character = (char)56;
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    if (shiftPressed)
                        this.Character = (char)40;
                    else
                        this.Character = (char)57;
                    break;
                default:
                    this.Character = (char)0;
                    break;
            }
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
            else
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
            if (obj is AsciiKey)
                return ((AsciiKey)obj) == this;

            return base.Equals(obj);
        }
    }
}
