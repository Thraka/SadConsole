using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace FeatureDemo.InputHandling
{
    internal class MoveViewPortKeyboardHandler : KeyboardConsoleComponent
    {
        private int _originalWidth;
        private int _originalHeight;

        public override void OnAdded(IScreenObject console)
        {
            if (console is Console con)
            {
                _originalWidth = con.Width;
                _originalHeight = con.Height;
            }
            else
            {
                throw new Exception($"{nameof(MoveViewPortKeyboardHandler)} can only be used on {nameof(Console)}");
            }
        }

        public override void ProcessKeyboard(IScreenObject consoleObject, SadConsole.Input.Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (Console)consoleObject;

            if (info.IsKeyDown(Keys.Left))
            {
                console.ViewPosition = console.ViewPosition.Translate((-1, 0));
            }

            if (info.IsKeyDown(Keys.Right))
            {
                console.ViewPosition = console.ViewPosition.Translate((1, 0));
            }

            if (info.IsKeyDown(Keys.Up))
            {
                console.ViewPosition = console.ViewPosition.Translate((0, -1));
            }

            if (info.IsKeyDown(Keys.Down))
            {
                console.ViewPosition = console.ViewPosition.Translate((0, +1));
            }

            handled = true;
        }
    }
}
