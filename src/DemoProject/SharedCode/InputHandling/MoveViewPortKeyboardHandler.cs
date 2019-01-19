using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Input;
using ScrollingConsole = SadConsole.ScrollingConsole;
using SadConsole;
using System;


namespace StarterProject.InputHandling
{
    class MoveViewPortKeyboardHandler : KeyboardConsoleComponent
    {
        public override void ProcessKeyboard(SadConsole.Console consoleObject, SadConsole.Input.Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (ScrollingConsole)consoleObject;

            if (info.IsKeyDown(Keys.Left))
                console.ViewPort = new Rectangle(console.ViewPort.Left - 1, console.ViewPort.Top, 80, 23);

            if (info.IsKeyDown(Keys.Right))
                console.ViewPort = new Rectangle(console.ViewPort.Left + 1, console.ViewPort.Top, 80, 23);

            if (info.IsKeyDown(Keys.Up))
                console.ViewPort = new Rectangle(console.ViewPort.Left, console.ViewPort.Top - 1, 80, 23);

            if (info.IsKeyDown(Keys.Down))
                console.ViewPort = new Rectangle(console.ViewPort.Left, console.ViewPort.Top + 1, 80, 23);


            handled = true;
        }
        
    }
}
