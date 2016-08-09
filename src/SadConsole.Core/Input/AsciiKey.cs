#if SFML
using Keys = SFML.Window.Keyboard.Key;
#elif MONOGAME
using Microsoft.Xna.Framework.Input;
#endif
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
        public Keys XnaKey;

        /// <summary>
        /// The keyboard character of the key.
        /// </summary>
        public char Character;
        
        /// <summary>
        /// Total time the key has been held.
        /// </summary>
        public float TimeHeld;

        /// <summary>
        /// Tracks if the key was previously held when calcualting the <see cref="KeyboardInfo.InitialRepeatDelay"/>.
        /// </summary>
        public bool PreviouslyPressed;

        /// <summary>
        /// Fills out the fields based on the MonoGame/XNA key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">Helps identify which <see cref="Character"/> to use while the key is pressed. For example, if <see cref="Keys.A"/> is used the <see cref="Character"/> field will be either 'A' if <paramref name="shiftPressed"/> is true or 'a' if false.</param>
        public void Fill(Keys key, bool shiftPressed)
        {
            this.XnaKey = key;

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
#if MONOGAME
                case Keys.OemComma:
#elif SFML
                case Keys.Comma:
#endif
                    if (shiftPressed)
                        this.Character = (char)60;
                    else
                        this.Character = (char)44;
                    break;
#if MONOGAME
                case Keys.OemMinus:
#elif SFML
                case Keys.Dash:
#endif
                    if (shiftPressed)
                        this.Character = (char)95;
                    else
                        this.Character = (char)45;
                    break;
#if MONOGAME
                case Keys.OemOpenBrackets:
#elif SFML
                case Keys.LBracket:
#endif
                    if (shiftPressed)
                        this.Character = (char)91;
                    else
                        this.Character = (char)123;
                    break;
#if MONOGAME
                case Keys.OemCloseBrackets:
#elif SFML
                case Keys.RBracket:
#endif
                    if (shiftPressed)
                        this.Character = (char)93;
                    else
                        this.Character = (char)125;
                    break;
#if MONOGAME
                case Keys.OemPeriod:
#elif SFML
                case Keys.Period:
#endif
                    if (shiftPressed)
                        this.Character = (char)62;
                    else
                        this.Character = (char)46;
                    break;
#if MONOGAME
                case Keys.OemBackslash:
                case Keys.OemPipe:
#elif SFML
                case Keys.BackSlash:
#endif
                    if (shiftPressed)
                        this.Character = (char)124;
                    else
                        this.Character = (char)92;
                    break;
#if MONOGAME
                case Keys.OemPlus:
#elif SFML
                case Keys.Equal:
#endif
                    if (shiftPressed)
                        this.Character = (char)43;
                    else
                        this.Character = (char)61;
                    break;
#if MONOGAME
                case Keys.OemQuestion:
#elif SFML
                case Keys.Slash:
#endif
                    if (shiftPressed)
                        this.Character = (char)63;
                    else
                        this.Character = (char)47;
                    break;
#if MONOGAME
                case Keys.OemQuotes:
#elif SFML
                case Keys.Quote:
#endif
                    if (shiftPressed)
                        this.Character = (char)34;
                    else
                        this.Character = (char)39;
                    break;
#if MONOGAME
                case Keys.OemSemicolon:
#elif SFML
                case Keys.SemiColon:
#endif
                    if (shiftPressed)
                        this.Character = (char)58;
                    else
                        this.Character = (char)59;
                    break;
#if MONOGAME
                case Keys.OemTilde:
#elif SFML
                case Keys.Tilde:
#endif
                    if (shiftPressed)
                        this.Character = (char)126;
                    else
                        this.Character = (char)96;
                    break;

                case Keys.Space:
                    this.Character = ' ';
                    break;
#if MONOGAME
                case Keys.Decimal:
                    this.Character = (char)46;
                    break;
#endif
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
#if MONOGAME
                case Keys.D0:
                case Keys.NumPad0:
#elif SFML
                case Keys.Num0:
                case Keys.Numpad0:
#endif

                    if (shiftPressed)
                        this.Character = (char)41;
                    else
                        this.Character = (char)48;
                    break;
#if MONOGAME
                case Keys.D1:
                case Keys.NumPad1:
#elif SFML
                case Keys.Num1:
                case Keys.Numpad1:
#endif
                    if (shiftPressed)
                        this.Character = (char)33;
                    else
                        this.Character = (char)49;
                    break;
#if MONOGAME
                case Keys.D2:
                case Keys.NumPad2:
#elif SFML
                case Keys.Num2:
                case Keys.Numpad2:
#endif
                    if (shiftPressed)
                        this.Character = (char)64;
                    else
                        this.Character = (char)50;
                    break;
#if MONOGAME
                case Keys.D3:
                case Keys.NumPad3:
#elif SFML
                case Keys.Num3:
                case Keys.Numpad3:
#endif
                    if (shiftPressed)
                        this.Character = (char)35;
                    else
                        this.Character = (char)51;
                    break;
#if MONOGAME
                case Keys.D4:
                case Keys.NumPad4:
#elif SFML
                case Keys.Num4:
                case Keys.Numpad4:
#endif
                    if (shiftPressed)
                        this.Character = (char)36;
                    else
                        this.Character = (char)52;
                    break;
#if MONOGAME
                case Keys.D5:
                case Keys.NumPad5:
#elif SFML
                case Keys.Num5:
                case Keys.Numpad5:
#endif
                    if (shiftPressed)
                        this.Character = (char)37;
                    else
                        this.Character = (char)53;
                    break;
#if MONOGAME
                case Keys.D6:
                case Keys.NumPad6:
#elif SFML
                case Keys.Num6:
                case Keys.Numpad6:
#endif
                    if (shiftPressed)
                        this.Character = (char)94;
                    else
                        this.Character = (char)54;
                    break;
#if MONOGAME
                case Keys.D7:
                case Keys.NumPad7:
#elif SFML
                case Keys.Num7:
                case Keys.Numpad7:
#endif
                    if (shiftPressed)
                        this.Character = (char)38;
                    else
                        this.Character = (char)55;
                    break;
#if MONOGAME
                case Keys.D8:
                case Keys.NumPad8:
#elif SFML
                case Keys.Num8:
                case Keys.Numpad8:
#endif
                    if (shiftPressed)
                        this.Character = (char)42;
                    else
                        this.Character = (char)56;
                    break;
#if MONOGAME
                case Keys.D9:
                case Keys.NumPad9:
#elif SFML
                case Keys.Num9:
                case Keys.Numpad9:
#endif
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
        /// Checks if the two <see cref="AsciiKey"/> types use the same <see cref="XnaKey"/> if the <see cref="Character"/> is 0. If the <see cref="Character"/> is not 0, the <see cref="Character"/> is compared.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AsciiKey left, AsciiKey right)
        {
            if (left.Character == (char)0 && left.Character == right.Character)
                return left.XnaKey == right.XnaKey;
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
