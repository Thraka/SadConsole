using System.Text;
using SadConsole.Coroutine;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.Terminal;

namespace SadBBSClient;

/// <summary>
/// Main screen for the BBS client. Contains a <see cref="TerminalConsole"/> for terminal display
/// and a <see cref="TelnetClient"/> for network communication.
/// Keyboard input is intercepted and sent to the remote BBS rather than local echo.
/// </summary>
public partial class BbsScreen : ScreenObject
{
    private readonly Phonebook _phonebook;
    private readonly TerminalConsole _terminal;
    private readonly KeyboardEncoder _encoder;
    private TelnetClient? _telnet;

    private Queue<AppState> _appStatesQueue;

    private AppState _appState = AppState.MainScreen;
    private ConnectionState _connectionState = ConnectionState.Disconnected;
    private readonly StringBuilder _inputBuffer = new();
    private readonly ScreenKeyBindings _specialKeyBindings = new();

    private CoroutineHandlerComponent _coroutineConnection;
    private MessageWindow? _messageWindow;

    private Event EVENT_MESSAGEWINDOW_CLOSED = new();
    private Event EVENT_MESSAGEWINDOW_OPENED = new();

    private AppState CurrentState
    {
        get => CurrentState;
        set
        {
            if (_appState == value) return;

            //AppState oldstate = _appState;
            //_appState = value;

            // Transition rules
            if (value == AppState.Message)
                PushState(AppState.Message);
            else
                MoveState(value);
        }
    }

    // Well-known public BBSes for the default menu
    private static readonly (string Name, string Host, int Port)[] DefaultBbses =
    {
        ("SadLogic",         "192.168.1.237",           7243),
        ("Alterant BBS",     "alterant.ca",             23),
        ("Level 29",         "bbs.fozztexx.com",        23),
        ("Black Flag",       "blackflagbbs.com",        23),
        ("Capitol Shrill",   "capitolshrill.com",       6502),
        ("Deadline",         "deadline.aegis-corp.org", 23),
    };



    public BbsScreen()
    {
         _terminal = new TerminalConsole(AppSettings.Instance.Width, AppSettings.Instance.Height);
        _terminal.UseKeyboard = false; // We handle keyboard ourselves
        _terminal.Position = new Point(0, 0);
        _terminal.Mode = TerminalMode.AnsiBbs;
        _terminal.TerminalCursor.Shape = CursorShape.BlinkingUnderline;
        _terminal.TerminalCursor.UpdateGlyphForShape();
        _encoder = _terminal.KeyboardEncoder;

        _phonebook = new() { IsVisible = false };
        _phonebook.IsVisibleChanged += _phonebook_IsVisibleChanged;

        _specialKeyBindings.Add(true, Keys.D, AppAction_ShowPhonebook);
        _specialKeyBindings.Add(true, Keys.H, AppAction_Disconnect);
        _specialKeyBindings.Add(true, Keys.M, ShowTestMessage);

        Children.Add(_terminal);
        Children.Add(_phonebook);

        UseKeyboard = true;

        HideTerminalCursor();

        _coroutineConnection = new();
        SadComponents.Add(_coroutineConnection);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (_messageWindow != null && _messageWindow.IsVisible)
            return _messageWindow!.ProcessKeyboard(keyboard);

        else if (_phonebook != null && _phonebook.IsVisible)
            return _phonebook.ProcessKeyboard(keyboard);

        // Sniff for system shortcuts
        else //if (CurrentState == AppState.MainScreen || CurrentState == AppState.Terminal)
        {
            // Check for shortcuts like the phonebook or app settings
            if (_specialKeyBindings.TryMatch(keyboard, out var keyBinding))
            {
                keyBinding.Value.OnPressed();
                return true;
            }

            else if (_connectionState == ConnectionState.Connected)
                return HandleTerminalMode(keyboard);
        }
        
        return false;
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // Drain received data from the telnet client onto the terminal (game thread safe)
        _telnet?.DrainReceived(data =>
        {
            _terminal.Feed(data.AsSpan());
        });

        // Check for disconnection
        if (_connectionState == ConnectionState.Connected && _telnet != null && !_telnet.IsConnected)
        {
            AppAction_Disconnect();
        }
    }

    private void ConnectTo(string host, int port, string displayName)
    {
        if (_connectionState != ConnectionState.Disconnected)
            AppAction_Disconnect();

        _terminal.Feed($"\r\n  \x1b[1;33mConnecting to {displayName}...\x1b[0m\r\n");

        try
        {
            _connectionState = ConnectionState.Connecting;
            _telnet = new TelnetClient(AppSettings.Instance.Width, AppSettings.Instance.Height);
            _telnet.Disconnected += OnTelnetDisconnected;
            _telnet.Connect(host, port);

            // Wire the telnet client as the terminal output channel
            // so DA/DSR responses go back to the BBS
            _terminal.Output = _telnet;

            _connectionState = ConnectionState.Connected;
            _terminal.Feed("\x1b[2J\x1b[H"); // Clear for BBS content
            ShowTerminalCursor();
        }
        catch (Exception ex)
        {
            _terminal.Feed($"\r\n  \x1b[1;31mConnection failed: {EscapeForTerminal(ex.Message)}\x1b[0m\r\n");
            AppAction_Disconnect();
        }
    }

    private void OnTelnetDisconnected()
    {
        // This fires on the read thread — the Update loop will detect _telnet.IsConnected == false
    }

    private bool HandleTerminalMode(Keyboard keyboard)
    {
        if (keyboard.KeysPressed.Count == 0)
            return true;

        // Check for Escape-Escape (double-escape) to return to menu
        // Single Escape is sent to BBS as normal

        // Sync DECCKM state
        _encoder.ApplicationCursorKeys = _terminal.Writer.State.CursorKeyMode;

        byte[] encoded = _encoder.Encode(keyboard);

        if (encoded.Length > 0 && _telnet != null)
            _telnet.SendKeyData(encoded);

        return true;
    }

    private static bool TryParseAddress(string input, out string? host, out int port)
    {
        host = null;
        port = 23;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        int colonIdx = input.LastIndexOf(':');
        if (colonIdx > 0 && int.TryParse(input.AsSpan(colonIdx + 1), out int p))
        {
            host = input[..colonIdx].Trim();
            port = p;
            return host.Length > 0;
        }

        // No port specified — default to 23
        host = input.Trim();
        return host.Length > 0;
    }

    private static string EscapeForTerminal(string text)
    {
        // Strip any control chars from exception messages to avoid corrupting terminal
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c >= ' ' && c < 127)
                sb.Append(c);
            else
                sb.Append('?');
        }
        return sb.ToString();
    }

    private void AppAction_DisconnectWithMessage()
    {
        ShowMessage("Disconnected");
    }

    //private IEnumerable<Wait> WaitForMessage()
    //{
    //    Children.Add(_messageWindow);
    //    yield return new Wait(EVENT_MESSAGEWINDOWCLOSED);

    //    _messageWindow = null;
    //}

    private void ShowTestMessage()
    {
        ShowMessage("Testing 1..2...3..4!");
    }

    private void ShowMessage(string message)
    {
        _messageWindow = new(message);
        Children.Add(_messageWindow);
        _messageWindow.IsVisible = true;
        _coroutineConnection.RaiseEvent(EVENT_MESSAGEWINDOW_OPENED);
        _messageWindow.IsVisibleChanged += _messageWindow_IsVisibleChanged;
    }

    private void _messageWindow_IsVisibleChanged(object? sender, EventArgs e)
    {
        if (!_messageWindow!.IsVisible)
        {
            _messageWindow.IsVisibleChanged -= _messageWindow_IsVisibleChanged;
            Children.Remove(_messageWindow);
            _messageWindow = null;
            _coroutineConnection.RaiseEvent(EVENT_MESSAGEWINDOW_CLOSED);
        }
    }

    private void AppAction_Disconnect()
    {
        // Alert user to hangup
        if (_connectionState == ConnectionState.Connected)
            _terminal.Feed("\r\n\r\n\x1b[1;31m--- Disconnected ---\x1b[0m\r\n");

        HideTerminalCursor();

        // Regardless, dispose and reset
        _telnet?.Dispose();
        _telnet = null;
        _connectionState = ConnectionState.Disconnected;
        if (CurrentState == AppState.Terminal)
            CurrentState = AppState.MainScreen;
    }

    private void AppAction_ShowPhonebook()
    {
        _phonebook.IsVisible = true;
    }

    private void AppAction_Exit()
    {
        if (_connectionState != ConnectionState.Disconnected)
        {
            _telnet?.Dispose();
            _telnet = null;
            _connectionState = ConnectionState.Disconnected;
        }

        SadConsole.Game.Instance.MonoGameInstance.Exit();
    }

    private void HideTerminalCursor() =>
        _terminal.TerminalCursor.IsVisible = false;

    private void ShowTerminalCursor() =>
        _terminal.TerminalCursor.IsVisible = true;

    private void PushState(AppState newState)
    {
        if (CurrentState == newState)
            return;

        _appStatesQueue.Enqueue(CurrentState);
        _appState = newState;
    }

    private void PopState()
    {
        if (_appStatesQueue.Count > 1)
        {
            _appStatesQueue.Dequeue();
            _appState = _appStatesQueue.Peek();
        }
        else
            _appState = AppState.MainScreen;
    }

    private void MoveState(AppState newState)
    {
        _appStatesQueue.Clear();
        _appState = newState;
    }

    private void _phonebook_IsVisibleChanged(object? sender, EventArgs e)
    {
        // Shown
        if (_phonebook.IsVisible)
            PushState(AppState.Phonebook);

        // Hidden
        else
        {
            CurrentState = AppState.Terminal;

            if (_phonebook.SelectedEntry is not null)
            {
                var entry = _phonebook.SelectedEntry;
                ConnectTo(entry.Value.Address, entry.Value.Port, entry.Value.Name);
            }
        }
    }
}
