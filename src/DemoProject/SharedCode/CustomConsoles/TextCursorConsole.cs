using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using ScrollingConsole = SadConsole.ScrollingConsole;
using SadConsole;
using System;
using System.Linq;
using SadConsole.Input;


namespace StarterProject.CustomConsoles
{
    class TextCursorConsole : ScrollingConsole
    {
        SadConsole.Console mouseCursor;
        
        public TextCursorConsole()
            : base(80, 23)
        {
            mouseCursor = new SadConsole.Console(1, 1);
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
