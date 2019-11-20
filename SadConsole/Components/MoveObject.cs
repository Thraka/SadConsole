using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

namespace SadConsole.Components
{
    public class MoveObject : KeyboardConsoleComponent
    {
        public override void ProcessKeyboard(ScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = false;

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                host.Position -= (1, 0);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                host.Position += (1, 0);
                handled = true;
            }

            if (keyboard.IsKeyPressed(Keys.Up))
            {
                host.Position -= (0, 1);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                host.Position += (0, 1);
                handled = true;
            }
        }
    }
}
