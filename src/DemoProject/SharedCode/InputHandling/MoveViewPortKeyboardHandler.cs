using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole.Components;
using ScrollingConsole = SadConsole.ScrollingConsole;

namespace StarterProject.InputHandling
{
    internal class MoveViewPortKeyboardHandler : KeyboardConsoleComponent
    {
        private int _originalWidth;
        private int _originalHeight;

        public override void OnAdded(SadConsole.Console console)
        {
            if (console is ScrollingConsole con)
            {
                _originalWidth = con.ViewPort.Width;
                _originalHeight = con.ViewPort.Height;
            }
            else
            {
                throw new Exception($"{nameof(MoveViewPortKeyboardHandler)} can only be used on {nameof(ScrollingConsole)}");
            }
        }

        public override void ProcessKeyboard(SadConsole.Console consoleObject, SadConsole.Input.Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (ScrollingConsole)consoleObject;

            if (info.IsKeyDown(Keys.Left))
            {
                console.ViewPort = new Rectangle(console.ViewPort.Left - 1, console.ViewPort.Top, _originalWidth, _originalHeight);
            }

            if (info.IsKeyDown(Keys.Right))
            {
                console.ViewPort = new Rectangle(console.ViewPort.Left + 1, console.ViewPort.Top, _originalWidth, _originalHeight);
            }

            if (info.IsKeyDown(Keys.Up))
            {
                console.ViewPort = new Rectangle(console.ViewPort.Left, console.ViewPort.Top - 1, _originalWidth, _originalHeight);
            }

            if (info.IsKeyDown(Keys.Down))
            {
                console.ViewPort = new Rectangle(console.ViewPort.Left, console.ViewPort.Top + 1, _originalWidth, _originalHeight);
            }

            handled = true;
        }

    }
}
