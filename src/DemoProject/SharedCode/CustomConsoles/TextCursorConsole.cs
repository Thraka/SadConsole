using Microsoft.Xna.Framework;
using ColorHelper = Microsoft.Xna.Framework.Color;

using Console = SadConsole.Console;
using SadConsole;
using System;
using System.Linq;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class TextCursorConsole : Console, IConsoleMetadata
    {
        SadConsole.GameHelpers.GameObject mouseCursor;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Text Mouse Cursor", Summary = "Draws a game object where ever the mouse cursor is." };
            }
        }

        public TextCursorConsole()
            : base(80, 23)
        {
            mouseCursor = new SadConsole.GameHelpers.GameObject();
            mouseCursor.Animation = new SadConsole.Surfaces.AnimatedSurface("default", 1, 1);
            mouseCursor.Animation.CreateFrame();
            mouseCursor.Animation.CurrentFrame.Cells[0].Glyph = 178;
        }

        public override void Draw(TimeSpan delta)
        {
            // First draw the console
            base.Draw(delta);

            mouseCursor.Draw(delta);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            mouseCursor.Position = state.WorldPosition;

            return base.ProcessMouse(state);
        }
    } 
}
