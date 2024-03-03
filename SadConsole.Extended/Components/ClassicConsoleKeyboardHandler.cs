using System;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// A classic console/terminal prompt keyboard handler.
    /// </summary>
    public class ClassicConsoleKeyboardHandler : KeyboardConsoleComponent
    {
        private Cursor _attachedCursor;
        private IScreenSurface _surface;

        private bool _isReady = true;

        /// <summary>
        /// A flag that when set to true prints a new line and the prompt.
        /// </summary>
        public bool IsReady
        {
            get => _isReady;
            set
            {
                // Changing to true
                if (_isReady == false && value)
                {
                    // After they have processed the string, we will create a new line and display the prompt.
                    _attachedCursor.NewLine();
                    PrintPrompt();

                    // Preparing the next lines could have scrolled the console, reset the counter
                    _surface.Surface.TimesShiftedUp = 0;
                }

                _isReady = value;
            }
        }

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
        /// This holds the row that the virtual cursor is starting from when someone is typing.
        /// </summary>
        public int CursorLastY { get; set; }

        /// <summary>
        /// This is a callback for the owner of this keyboard handler. The parameters are the component itself, the <see cref="Cursor"/> used by the handler, and the string input by the user. It's called when the user presses ENTER.
        /// </summary>
        public Action<ClassicConsoleKeyboardHandler, Cursor, string> EnterPressedAction = (h, c, s) => { };

        /// <summary>
        /// Creates the handler with the specified prompt.
        /// </summary>
        /// <param name="prompt">The prompt to display to the user.</param>
        public ClassicConsoleKeyboardHandler(string prompt = "> ") =>
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
        /// Prints the prompt and locks the cursor to the column that ends at the prompt.
        /// </summary>
        public void PrintPrompt()
        {
            _attachedCursor.Print(Prompt);
            CursorLastY = _attachedCursor.Position.Y;
        }

        /// <inheritdoc/>
        public override void ProcessKeyboard(IScreenObject consoleObject, Keyboard info, out bool handled)
        {
            if (!_isReady) { handled = false; return; }

            // Check each key pressed.
            for (int i = 0; i < info.KeysPressed.Count; i++)
            {
                AsciiKey key = info.KeysPressed[i];

                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                    _attachedCursor.Print(key.Character.ToString());

                // Special character - BACKSPACE
                else if (key.Key == Keys.Back)
                {
                    // If the console has scrolled since the user started typing, adjust the starting row of the virtual cursor by that much.
                    if (_surface.Surface.TimesShiftedUp != 0)
                    {
                        CursorLastY -= _surface.Surface.TimesShiftedUp;
                        _surface.Surface.TimesShiftedUp = 0;
                    }

                    // Do not let them backspace into the prompt
                    if (_attachedCursor.Position.Y != CursorLastY || _attachedCursor.Position.X > Prompt.Length)
                        _attachedCursor.LeftWrap(1).Print(new string(EraseGlyph, 1)).LeftWrap(1);
                }

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    // If the console has scrolled since the user started typing, adjust the starting row of the virtual cursor by that much.
                    if (_surface.Surface.TimesShiftedUp != 0)
                    {
                        CursorLastY -= _surface.Surface.TimesShiftedUp;
                        _surface.Surface.TimesShiftedUp = 0;
                    }

                    // Get the prompt to exclude it in determining the total length of the string the user has typed.
                    int startingIndex =  new Point(Prompt.Length, CursorLastY).ToIndex(_surface.Surface.Width);
                    string data = _surface.Surface.GetString(startingIndex, _attachedCursor.Position.ToIndex(_surface.Surface.Width) - startingIndex).Replace('\0', ReplaceEmptyGlyph);
                    
                    // Move the cursor to the next line before we send the string data to the processor
                    _attachedCursor.NewLine();

                    // Send the string data
                    EnterPressedAction(this, _attachedCursor, data);

                    if (_isReady)
                    {
                        // After they have processed the string, we will create a new line and display the prompt.
                        _attachedCursor.NewLine();
                        PrintPrompt();

                        // Preparing the next lines could have scrolled the console, reset the counter
                        _surface.Surface.TimesShiftedUp = 0;
                    }
                }
            }

            handled = true;
        }

    }
}
