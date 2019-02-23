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
            mouseCursor.UseMouse = false;

            Children.Add(mouseCursor);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            mouseCursor.IsVisible = state.IsOnConsole;
            mouseCursor.Position = state.ConsoleCellPosition;

            return base.ProcessMouse(state);
        }
    } 
}
