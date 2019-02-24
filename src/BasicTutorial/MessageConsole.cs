using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;
using Console = SadConsole.Console;

namespace BasicTutorial
{
    class MessageConsole : Console
    {
        Color _semiTransparentBlack;

        public enum MessageTypes
        {
            Warning,
            Status,
            Problem,
            Battle
        }

        public MessageConsole(int width, int height) : base(width, height)
        {
            IsCursorDisabled = true;
            Cursor.IsVisible = false;
            UseKeyboard = false;

            _semiTransparentBlack = Color.Black;
            _semiTransparentBlack.A = 128;

            DefaultBackground = _semiTransparentBlack;

            Fill(Color.White, _semiTransparentBlack, 0);
            this[0].CopyAppearanceTo(Cursor.PrintAppearance);
        }

        public void Print(string text, MessageTypes type)
        {
            Color color;

            switch (type)
            {
                case MessageTypes.Warning:
                    color = Color.PaleVioletRed;
                    break;
                case MessageTypes.Problem:
                    color = Color.Orange;
                    break;
                case MessageTypes.Battle:
                    color = Color.LawnGreen;
                    break;
                case MessageTypes.Status:
                default:
                    color = Color.LightGray;
                    break;
            }

            Cursor.NewLine().Print(new ColoredString("* " + text, color, Color.Transparent) { IgnoreBackground = true });
        }
    }
}
