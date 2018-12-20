using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using Console = SadConsole.Console;
using SadConsole;
using System;
using System.Linq;
using SadConsole.Input;
using SadConsole.Surfaces;

namespace StarterProject.CustomConsoles
{
    class TextCursorConsole : Console
    {
        ScreenObject mouseCursor;
        
        public TextCursorConsole()
            : base(80, 23)
        {
            mouseCursor = new ScreenObject(1, 1);
            mouseCursor.SetGlyph(0,0, 178);

            Children.Add(mouseCursor);
        }

        public override void Draw(TimeSpan delta)
        {
            // First draw the console
            base.Draw(delta);

            //mouseCursor.Draw(delta);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            mouseCursor.Position = state.ConsolePosition;

            return base.ProcessMouse(state);
        }
    } 
}
