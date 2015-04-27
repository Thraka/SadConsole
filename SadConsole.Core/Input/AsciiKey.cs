#if !SHARPDX
using Microsoft.Xna.Framework.Input;
#else
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using Keys = SharpDX.DirectInput.Key;
#endif
using System.Linq;

namespace SadConsole.Input
{
    public struct AsciiKey
    {
        public Keys XnaKey;
        public char Character;
        public float TimeHeld;
        public bool PreviouslyPressed;

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
#if !SILVERLIGHT
#if !SHARPDX
                case Keys.OemComma:
#else
                case Keys.Comma:
#endif
                    if (shiftPressed)
                        this.Character = (char)60;
                    else
                        this.Character = (char)44;
                    break;
#if !SHARPDX
                case Keys.OemMinus:
#else
                case Keys.Minus:
#endif
                    if (shiftPressed)
                        this.Character = (char)95;
                    else
                        this.Character = (char)45;
                    break;
#if !SHARPDX
                case Keys.OemOpenBrackets:
#else
                case Keys.LeftBracket:
#endif
                    if (shiftPressed)
                        this.Character = (char)91;
                    else
                        this.Character = (char)123;
                    break;
#if !SHARPDX
                case Keys.OemCloseBrackets:
#else
                case Keys.RightBracket:
#endif
                    if (shiftPressed)
                        this.Character = (char)93;
                    else
                        this.Character = (char)125;
                    break;
#if !SHARPDX
                case Keys.OemPeriod:
#else
                case Keys.Period:
#endif
                    if (shiftPressed)
                        this.Character = (char)62;
                    else
                        this.Character = (char)46;
                    break;
#if !SHARPDX
                case Keys.OemBackslash:
                case Keys.OemPipe:
#else
                case Keys.Backslash:
#endif
                    if (shiftPressed)
                        this.Character = (char)124;
                    else
                        this.Character = (char)92;
                    break;
#if !SHARPDX
                case Keys.OemPlus:
#else
                case Keys.Equals:
#endif
                    if (shiftPressed)
                        this.Character = (char)43;
                    else
                        this.Character = (char)61;
                    break;
#if !SHARPDX
                case Keys.OemQuestion:
#else
                case Keys.Slash:
#endif
                    if (shiftPressed)
                        this.Character = (char)63;
                    else
                        this.Character = (char)47;
                    break;
#if !SHARPDX
                case Keys.OemQuotes:
#else
                case Keys.Apostrophe:
#endif
                    if (shiftPressed)
                        this.Character = (char)34;
                    else
                        this.Character = (char)39;
                    break;
#if !SHARPDX
                case Keys.OemSemicolon:
#else
                case Keys.Semicolon:
#endif
                    if (shiftPressed)
                        this.Character = (char)58;
                    else
                        this.Character = (char)59;
                    break;
#if !SHARPDX
                case Keys.OemTilde:
#else
                case Keys.Grave:
#endif
                    if (shiftPressed)
                        this.Character = (char)126;
                    else
                        this.Character = (char)96;
                    break;
#endif

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
#if !SHARPDX
                case Keys.NumPad0:
#else
                case Keys.NumberPad0:
#endif

                    if (shiftPressed)
                        this.Character = (char)41;
                    else
                        this.Character = (char)48;
                    break;
                case Keys.D1:
#if !SHARPDX
                case Keys.NumPad1:
#else
                case Keys.NumberPad1:
#endif
                    if (shiftPressed)
                        this.Character = (char)33;
                    else
                        this.Character = (char)49;
                    break;
                case Keys.D2:
#if !SHARPDX
                case Keys.NumPad2:
#else
                case Keys.NumberPad2:
#endif
                    if (shiftPressed)
                        this.Character = (char)64;
                    else
                        this.Character = (char)50;
                    break;
                case Keys.D3:
#if !SHARPDX
                case Keys.NumPad3:
#else
                case Keys.NumberPad3:
#endif
                    if (shiftPressed)
                        this.Character = (char)35;
                    else
                        this.Character = (char)51;
                    break;
                case Keys.D4:
#if !SHARPDX
                case Keys.NumPad4:
#else
                case Keys.NumberPad4:
#endif
                    if (shiftPressed)
                        this.Character = (char)36;
                    else
                        this.Character = (char)52;
                    break;
                case Keys.D5:
#if !SHARPDX
                case Keys.NumPad5:
#else
                case Keys.NumberPad5:
#endif
                    if (shiftPressed)
                        this.Character = (char)37;
                    else
                        this.Character = (char)53;
                    break;
                case Keys.D6:
#if !SHARPDX
                case Keys.NumPad6:
#else
                case Keys.NumberPad6:
#endif
                    if (shiftPressed)
                        this.Character = (char)94;
                    else
                        this.Character = (char)54;
                    break;
                case Keys.D7:
#if !SHARPDX
                case Keys.NumPad7:
#else
                case Keys.NumberPad7:
#endif
                    if (shiftPressed)
                        this.Character = (char)38;
                    else
                        this.Character = (char)55;
                    break;
                case Keys.D8:
#if !SHARPDX
                case Keys.NumPad8:
#else
                case Keys.NumberPad8:
#endif
                    if (shiftPressed)
                        this.Character = (char)42;
                    else
                        this.Character = (char)56;
                    break;
                case Keys.D9:
#if !SHARPDX
                case Keys.NumPad9:
#else
                case Keys.NumberPad9:
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

        public static AsciiKey Get(Keys key)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, false);
            return asciiKey;
        }

        public static AsciiKey Get(Keys key, bool shiftPressed)
        {
            AsciiKey asciiKey = new AsciiKey();
            asciiKey.Fill(key, shiftPressed);
            return asciiKey;
        }
        public static bool operator ==(AsciiKey left, AsciiKey right)
        {
            if (left.Character == (char)0 && left.Character == right.Character)
                return left.XnaKey == right.XnaKey;
            else
                return left.Character == right.Character;
        }

        public static bool operator !=(AsciiKey left, AsciiKey right)
        {
            return left.Character != right.Character;
        }

        public override bool Equals(object obj)
        {
            if (obj is AsciiKey)
                return ((AsciiKey)obj) == this;

            return base.Equals(obj);
        }
    }
}
