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
            _attachedCursor.NewLine();
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
            // Check each key pressed.
            for (int i = 0; i < info.KeysPressed.Count; i++)
            {
                AsciiKey key = info.KeysPressed[i];

                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                {
                    //if (_insertMode)
                    // console.ShiftRight(_attachedCursor.Column, _attachedCursor.Row, 1);

                    _attachedCursor.Print(key.Character.ToString());
                }

                else if (key.Key == Keys.Insert)
                    _insertMode = !_insertMode;

                // Special character - BACKSPACE
                else if (key.Key == Keys.Back)
                    _attachedCursor.LeftWrap(1).Print(new string(EraseGlyph, 1)).LeftWrap(1);

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    // Get the prompt to exclude it in determining the total length of the string the user has typed.
                    int startingIndex = new Point(0, _attachedCursor.Row).ToIndex(_surface.Surface.Width);
                    string data = _surface.Surface.GetString(startingIndex, _surface.Surface.Width).TrimEnd('\0').Replace('\0', ReplaceEmptyGlyph);

                    // Move the cursor to the next line before we send the string data to the processor
                    _attachedCursor.NewLine();

                    // Send the string data
                    if (EnterPressedAction(this, _attachedCursor, data))
                    {
                        // After they have processed the string, we will create a new line and display the prompt.
                        _attachedCursor.NewLine();
                        PrintPrompt();
                    }

                    // Preparing the next lines could have scrolled the console, reset the counter
                    _surface.Surface.TimesShiftedUp = 0;
                }

                else if (key.Key == Keys.Down)
                {
                    if (_attachedCursor.Row == _surface.Surface.Height - 1)
                        _surface.Surface.ShiftUp();
                    else
                        _attachedCursor.Down(1);
                }

                else if (key.Key == Keys.Right)
                {
                    if (_attachedCursor.Row == _surface.Surface.Height - 1 && _attachedCursor.Column == _surface.Surface.Width - 1)
                    {
                        _surface.Surface.ShiftUp();
                        _attachedCursor.CarriageReturn();
                    }
                    else
                        _attachedCursor.RightWrap(1);
                }

                else if (key.Key == Keys.Left)
                    _attachedCursor.LeftWrap(1);

                else if (key.Key == Keys.Up)
                    _attachedCursor.Up(1);
            }

            handled = true;
        }

    }
}
