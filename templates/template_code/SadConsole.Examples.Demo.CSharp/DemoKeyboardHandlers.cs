using SadConsole.Components;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;
internal class DemoKeyboardHandlers : IDemo
{
    public string Title => "Keyboard Handlers and Cursors";

    public string Description => "The [c:r f:ansibluebright]SadConsole.Extended[c:u] NuGet library contains 2 keyboard " +
                                 "components which change how the keyboard interacts with a console." +
                                 "\r\n\r\n" +
                                 "[c:r f:AnsiGreenBright]ClassicConsoleKeyboardHandler[c:u]: Command prompt style handler. Cursor movement is limited.\r\n" +
                                 "\r\n" +
                                 "[c:r f:AnsiGreenBright]C64KeyboardHandler[c:u]: Commdore computer style handler. Cursor can be moved around.\r\n";

    public string CodeFile => "DemoKeyboardHandlers.cs";

    public IScreenSurface CreateDemoScreen() =>
        new KeyboardHandlers();

    public override string ToString() =>
        Title;
}


internal class KeyboardHandlers : ControlsConsole
{
    private ScreenSurface _promptScreen;

    private readonly ClassicConsoleKeyboardHandler _keyboardHandlerDOS;
    private readonly C64KeyboardHandler _keyboardHandlerC64;

    // This console domonstrates a classic MS-DOS or Windows Command Prompt style console.
    public KeyboardHandlers() : base(28, 4)
    {
        // This is our custom keyboard handlers we'll be using to process the cursor on this console.
        _keyboardHandlerDOS = new ClassicConsoleKeyboardHandler("Prompt> ");
        _keyboardHandlerC64 = new C64KeyboardHandler("READY.\r\n");

        // Create the other console where the keyboard handler will be set
        _promptScreen = new ScreenSurface(GameSettings.SCREEN_DEMO_WIDTH - 8, GameSettings.SCREEN_DEMO_HEIGHT - this.Height - 3)
        {
            Position = (8, this.Height + 3),
            UseKeyboard = true
        };

        // The keyboard handlers from the SadConsole.Extended library require a cursor to exist on
        // the object they're added to. Console and ControlsConsole always have a cursor. A standard
        // ScreenSurface doesn't. Also, this cursor is disabled, since we don't want it to handle
        // the keyboard and we want the handlers to do it.
        _promptScreen.SadComponents.Add(new Cursor() { IsEnabled = false });

        Border.CreateForSurface(_promptScreen, "");

        Children.Add(_promptScreen);

        // Create the controls
        RadioButton buttonDos = new RadioButton(28, 1)
        {
            Text = "Classic keyboard handler",
            Position = (1, 1),
            Tag = 0
        };

        buttonDos.IsSelectedChanged += ButtonHandler_IsSelectedChanged;
        Controls.Add(buttonDos);

        RadioButton buttonC64 = new RadioButton(24, 1)
        {
            Text = "C64 keyboard handler",
            Tag = 1
        };

        // Place this control at the same location as the other button
        buttonC64.Position = buttonDos.Position;

        // Then move this control under it.
        buttonC64.PlaceRelativeTo(buttonDos, Direction.Types.Down, 0);

        buttonC64.IsSelectedChanged += ButtonHandler_IsSelectedChanged;
        Controls.Add(buttonC64);

        // Now that everything is created and ready, configure the handlers and prep the screens
        SetupHandlers();

        // Select the first button to enable a handler
        buttonDos.IsSelected = true;
    }

    private void ButtonHandler_IsSelectedChanged(object? sender, EventArgs e)
    {
        // Only operate on a button that is selected.
        // This event handler is used by both radio buttons, and will be called twice,
        // once for the button that is deselected, and once for the one that is selected.
        if (sender is RadioButton button && button.IsSelected)
        {
            if (button.Tag is not null)
            {
                // DOS handler selected
                if (button.Tag.Equals(0))
                {
                    _promptScreen.SadComponents.Remove(_keyboardHandlerC64);
                    _promptScreen.SadComponents.Add(_keyboardHandlerDOS);
                }

                // Commodore 64 handler selected
                else
                {
                    _promptScreen.SadComponents.Add(_keyboardHandlerC64);
                    _promptScreen.SadComponents.Remove(_keyboardHandlerDOS);
                }
            }
        }
    }


    public override void OnFocused()
    {
        // If this object is focused, move focus to the child object: _promptScreen.
        // This makes sure that _promptScreen receives keyboard input and not this
        // object
        _promptScreen.IsFocused = true;
    }

    private void SetupHandlers()
    {
        // Our custom handler has a call back for processing the commands the user types. We could handle
        // this in any method object anywhere, but we've implemented it on this console directly.
        _keyboardHandlerDOS.EnterPressedAction = DOSHandlerEnterPressed;
        _keyboardHandlerC64.EnterPressedAction = C64HandlerEnterPressed;

        // Disable the cursor since our keyboard handler will do the work.
        Cursor cursor = _promptScreen.GetSadComponent<Cursor>()!;
        cursor.Print("Try typing in the following commands: help, ver, cls, look. If you type exit or quit, the program will end.").NewLine().NewLine();
        
        _promptScreen.Surface.TimesShiftedUp = 0;
    }

    private void DOSHandlerEnterPressed(ClassicConsoleKeyboardHandler keyboardComponent, Cursor cursor, string value)
    {
        value = value.ToLower().Trim();

        if (value == "help")
        {
            cursor.NewLine().
                          Print("  Advanced Example: Command Prompt - HELP").NewLine().
                          Print("  =======================================").NewLine().NewLine().
                          Print("  help      - Display this help info").NewLine().
                          Print("  ver       - Display version info").NewLine().
                          Print("  cls       - Clear the screen").NewLine().
                          Print("  look      - Example adventure game command").NewLine().
                          Print("  ").NewLine();
        }
        else if (value == "ver")
        {
            cursor.Print("  SadConsole for MonoGame").NewLine();
        }
        else if (value == "cls")
        {
            _promptScreen.Clear();
            cursor.Position = new Point(0, 0);

            // The DOS keyboard handler wants to track which row the cursor is on. Since we moved it in the previous
            // statement, we want to update it here.
            _keyboardHandlerDOS.CursorLastY = cursor.Position.Y;
        }
        else if (value == "look")
        {
            // In this case we want word breaks to be nice when the cursor prints the next string.
            cursor.DisableWordBreak = false;
            cursor.Print("  Looking around you discover that you are in a dark and empty room. To your left there is a computer monitor in front of you and Visual Studio is opened, waiting for your next command.").NewLine();
            cursor.DisableWordBreak = true;
        }
        else
        {
            cursor.Print("  Unknown command").NewLine();
        }
    }

    private bool C64HandlerEnterPressed(C64KeyboardHandler keyboardComponent, Cursor cursor, string value)
    {
        value = value.ToLower().Trim();

        if (value == "help")
        {
            cursor.NewLine().
                          Print("  Advanced Example: Command Prompt - HELP").NewLine().
                          Print("  =======================================").NewLine().NewLine().
                          Print("  help      - Display this help info").NewLine().
                          Print("  ver       - Display version info").NewLine().
                          Print("  cls       - Clear the screen").NewLine().
                          Print("  look      - Example adventure game cmd").NewLine().
                          Print("  exit,quit - Quit the program").NewLine().
                          Print("  ").NewLine();
        }
        else if (value == "ver")
        {
            cursor.Print("  SadConsole for MonoGame").NewLine();
        }
        else if (value == "cls")
        {
            _promptScreen.Clear();
            cursor.Position = new Point(0, 0);
        }
        else if (value == "look")
        {
            // In this case we want word breaks to be nice when the cursor prints the next string.
            cursor.DisableWordBreak = false;
            cursor.Print("  Looking around you discover that you are in a dark and empty room. To your left there is a computer monitor in front of you and Visual Studio is opened, waiting for your next command.").NewLine();
            cursor.DisableWordBreak = true;
        }
        else if (value.StartsWith("clear "))
        {
            if (value.StartsWith("clear right"))
            {
                cursor.Up(1).Right("clear right".Length).EraseRight().NewLine();
            }
            else if (value.StartsWith("clear left"))
            {
                cursor.Up(1).Right("clear left".Length).EraseLeft().NewLine();
            }
            else if (value.StartsWith("clear up"))
            {
                cursor.Up(1).Right("clear up".Length).EraseUp().NewLine();
            }
            else if (value.StartsWith("clear down"))
            {
                cursor.Up(1).Right("clear down".Length).EraseDown().NewLine();
            }
            else if (value.StartsWith("clear row"))
            {
                cursor.Up(1).Right("clear row".Length).EraseRow().NewLine();
            }
            else if (value.StartsWith("clear col"))
            {
                cursor.Up(1).Right("clear col".Length).EraseColumn().NewLine();
            }
        }
        else
        {
            cursor.Print("  Unknown command").NewLine();
        }

        return true;
    }
}
