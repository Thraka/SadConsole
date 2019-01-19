using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Input;
using ScrollingConsole = SadConsole.ScrollingConsole;
using SadConsole;
using System;


namespace StarterProject.InputHandling
{
    class ClassicConsoleKeyboardHandler: KeyboardConsoleComponent
    {
        // This holds the row that the virtual cursor is starting from when someone is typing.
        public int CursorLastY;

        // this is a callback for the owner of this keyboard handler. It is called when the user presses ENTER.
        public Action<string> EnterPressedAction = (s) => { int i = s.Length; };

        public override void ProcessKeyboard(SadConsole.Console consoleObject, SadConsole.Input.Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (ScrollingConsole)consoleObject;

            // Check each key pressed.
            foreach (var key in info.KeysPressed)
            {
                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                    console.Cursor.Print(key.Character.ToString());

                // Special character - BACKSPACE
                else if (key.Key == Keys.Back)
                {
                    // Get the prompt that the console has.
                    string prompt = ((CustomConsoles.DOSConsole)console).Prompt;

                    // If the console has scrolled since the user started typing, adjust the starting row of the virtual cursor by that much.
                    if (console.TimesShiftedUp != 0)
                    {
                        CursorLastY -= console.TimesShiftedUp;
                        console.TimesShiftedUp = 0;
                    }

                    // Do not let them backspace into the prompt
                    if (console.Cursor.Position.Y != CursorLastY || console.Cursor.Position.X > prompt.Length)
                        console.Cursor.LeftWrap(1).Print(" ").LeftWrap(1);
                }

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    // If the console has scrolled since the user started typing, adjust the starting row of the virtual cursor by that much.
                    if (console.TimesShiftedUp != 0)
                    {
                        CursorLastY -= console.TimesShiftedUp;
                        console.TimesShiftedUp = 0;
                    }

                    // Get the prompt to exclude it in determining the total length of the string the user has typed.
                    string prompt = ((CustomConsoles.DOSConsole)console).Prompt;
                    int startingIndex = console.GetIndexFromPoint(new Point(prompt.Length, CursorLastY));
                    string data = ((ScrollingConsole)console).GetString(startingIndex, console.GetIndexFromPoint(console.Cursor.Position) - startingIndex);

                    // Move the cursor to the next line before we send the string data to the processor
                    console.Cursor.CarriageReturn().LineFeed();

                    // Send the string data
                    EnterPressedAction(data);

                    // After they have processed the string, we will create a new line and display the prompt.
                    console.Cursor.CarriageReturn().LineFeed();
                    console.Cursor.DisableWordBreak = true;
                    console.Cursor.Print(((CustomConsoles.DOSConsole)console).Prompt);
                    console.Cursor.DisableWordBreak = false;
                    CursorLastY = console.Cursor.Position.Y;

                    // Preparing the next lines could have scrolled the console, reset the counter
                    console.TimesShiftedUp = 0;
                }
            }

            handled = true;
        }
        
    }
}
