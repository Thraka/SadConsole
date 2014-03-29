namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;

    class DOSConsole: Console
    {
        public string Prompt { get; set; }

        private InputHandling.ClassicConsoleKeyboardHandler _keyboardHandlerObject;

        // This console domonstrates a classic MS-DOS or Windows Command Prompt style console.
        public DOSConsole()
            : base(80, 25)
        {
            this.IsVisible = false;

            // This is our cusotmer keyboard handler we'll be using to process the cursor on this console.
            _keyboardHandlerObject = new InputHandling.ClassicConsoleKeyboardHandler();

            // Assign our custom handler method from our handler object to this consoles keyboard handler.
            // We could have overridden the ProcessKeyboard method, but I wanted to demonstrate how you
            // can use your own handler on any console type.
            KeyboardHandler = _keyboardHandlerObject.HandleKeyboard;

            // Our custom handler has a call back for processing the commands the user types. We could handle
            // this in any method object anywhere, but we've implemented it on this console directly.
            _keyboardHandlerObject.EnterPressedAction = EnterPressedActionHandler;

            // Enable the keyboard and setup the prompt.
            CanUseKeyboard = true;
            VirtualCursor.IsVisible = true;
            Prompt = "DOS Prompt> ";
            Clear();
        }

        public void Clear()
        {
            CellData.Clear();
            VirtualCursor.Position = new Point(0, 24);
            _keyboardHandlerObject.VirtualCursorLastY = 24;
            VirtualCursor.Print(Prompt);
        }

        private void EnterPressedActionHandler(string value)
        {
            if (value.ToLower() == "help")
                VirtualCursor.Print("  There is no help available right now, sorry.").NewLine();

            else if (value.ToLower() == "ver")
                VirtualCursor.Print("  SadConsole for MonoGame and XNA 4.0").NewLine();

            else if (value.ToLower() == "exit" || value.ToLower() == "quit")
                Program.Game.Exit();

            else
                VirtualCursor.Print("  Unknown command").NewLine();
        }
    }
}
