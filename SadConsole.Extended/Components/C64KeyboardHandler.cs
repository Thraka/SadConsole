using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace SadConsole.Components
{
    /// <summary>
    /// A console prompt keyboard handler that acts like the text editor on the Commodore 64 and VIC-20 computers.
    /// </summary>
    /// <remarks>
    /// This handler lets the user move the cursor with the keyboard arrow keys. When the <kbd>ENTER</kbd> key is pressed, the current line is sent to the <see cref="EnterPressedAction"/> callback. All empty characters are trimmed from the start and end of the string.
    /// </remarks>
    public class C64KeyboardHandler : KeyboardConsoleComponent
    {
        private Cursor _attachedCursor;
        private IScreenSurface _surface;
        private bool _insertMode;

        /// <summary>
        /// The prompt to display to the user.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// The glyph to print when erasing a character, such as when the backspace key is pressed to erase a character.
        /// </summary>
        public char EraseGlyph { get; set; } = ' ';

        /// <summary>
        /// The glyph used to replace the empty characters that may be in the string sent to <see cref="EnterPressedAction"/>.
        /// </summary>
        public char ReplaceEmptyGlyph { get; set; } = ' ';

        /// <summary>
        /// This is a callback for the owner of this keyboard handler. When it returns <see langword="true"/>, the handler will print the prompt. The parameters are the component itself, the <see cref="Cursor"/> used by the handler, and the string input by the user. It's called when the user presses ENTER.
        /// </summary>
        public Func<C64KeyboardHandler, Cursor, string, bool> EnterPressedAction = (h, c, s) => { return true; };

        /// <summary>
        /// Creates a new keyboard handler with the specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        public C64KeyboardHandler(string prompt) =>
            Prompt = prompt;

        /// <summary>
        /// Called when this component is added to an object with a cursor; caches reference to the cursor.
        /// </summary>
        /// <param name="host">The host object for the component.</param>
        /// <remarks>
        /// Makes the cursor visible, disables word breaks and the string parser, and prints the prompt.
        ///
        /// If the host is a <see cref="Console"/>, it uses the <see cref="_attachedCursor"/> property, caching reference to it. You can't change the cursor reference unless you re-add this component. If the host is a <see cref="IScreenObject"/>, the first instance of a cursor in the <see cref="IScreenObject.SadComponents"/> collection is used. If there is no cursor, an exception is thrown.
        /// </remarks>
        public override void OnAdded(IScreenObject host)
        {
            if (host is Console c)
            {
                _attachedCursor = c.Cursor;
                _surface = c;
            }
            else if (host is IScreenSurface surface)
            {
                _attachedCursor = host.GetSadComponent<Cursor>();

                if (_attachedCursor == null) throw new ArgumentException("Host doesn't contain a cursor component");

                _surface = surface;
            }
            else
                throw new ArgumentException($"Host must implement {typeof(IScreenSurface)}.");

            _attachedCursor.IsVisible = true;
            _attachedCursor.DisableWordBreak = true;
            _attachedCursor.UseStringParser = false;
            _attachedCursor.CarriageReturn();
            PrintPrompt();
        }

        /// <summary>
        /// Prints the prompt at the cursors current position.
        /// </summary>
        public void PrintPrompt() =>
            _attachedCursor.Print(Prompt);

        /// <inheritdoc/>
        public override void ProcessKeyboard(IScreenObject consoleObject, Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (Console)consoleObject;

            // Check each key pressed.
            foreach (AsciiKey key in info.KeysPressed)
            {
                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                {
                    //if (_insertMode)
                    // console.ShiftRight(console.Cursor.Column, console.Cursor.Row, 1);

                    console.Cursor.Print(key.Character.ToString());
                }

                else if (key.Key == Keys.Insert)
                    _insertMode = !_insertMode;

                // Special character - BACKSPACE
                else if (key.Key == Keys.Back)
                    console.Cursor.LeftWrap(1).Print(new string(EraseGlyph, 1)).LeftWrap(1);

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    // Get the prompt to exclude it in determining the total length of the string the user has typed.
                    int startingIndex = new Point(0, console.Cursor.Row).ToIndex(console.Width);
                    string data = console.GetString(startingIndex, console.Width).TrimEnd('\0').Replace('\0', ReplaceEmptyGlyph);

                    // Move the cursor to the next line before we send the string data to the processor
                    console.Cursor.NewLine();

                    // Send the string data
                    if (EnterPressedAction(this, console.Cursor, data))
                    {
                        // After they have processed the string, we will create a new line and display the prompt.
                        console.Cursor.NewLine();
                        PrintPrompt();
                    }

                    // Preparing the next lines could have scrolled the console, reset the counter
                    console.TimesShiftedUp = 0;
                }

                else if (key.Key == Keys.Down)
                {
                    if (console.Cursor.Row == console.Height - 1)
                        console.ShiftUp();
                    else
                        console.Cursor.Down(1);
                }

                else if (key.Key == Keys.Right)
                {
                    if (console.Cursor.Row == console.Height - 1 && console.Cursor.Column == console.Width - 1)
                    {
                        console.ShiftUp();
                        console.Cursor.CarriageReturn();
                    }
                    else
                        console.Cursor.RightWrap(1);
                }

                else if (key.Key == Keys.Left)
                    console.Cursor.LeftWrap(1);

                else if (key.Key == Keys.Up)
                    console.Cursor.Up(1);
            }

            handled = true;
        }

    }
}
