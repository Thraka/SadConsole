using System.Text;
using SadConsole;
using SadConsole.Input;
using SadConsole.Terminal;
using SadRogue.Primitives;

namespace SadBBSClient;

/// <summary>
/// Main screen for the BBS client. Contains a <see cref="TerminalConsole"/> for terminal display
/// and a <see cref="TelnetClient"/> for network communication.
/// Keyboard input is intercepted and sent to the remote BBS rather than local echo.
/// </summary>
public class BbsScreen : ScreenObject
{
    private const int TermWidth = 80;
    private const int TermHeight = 25;

    private readonly TerminalConsole _terminal;
    private readonly KeyboardEncoder _encoder;
    private TelnetClient? _telnet;

    private bool _connected;
    private bool _awaitingInput;
    private readonly StringBuilder _inputBuffer = new();

    // Well-known public BBSes for the default menu
    private static readonly (string Name, string Host, int Port)[] DefaultBbses =
    {
        ("Alterant BBS",     "alterant.ca",         23),
        ("Level 29",         "bbs.fozztexx.com",    23),
        ("Black Flag",       "blackflagbbs.com",    23),
        ("Capitol Shrill",   "capitolshrill.com",  6502),
        ("Deadline",         "deadline.aegis-corp.org", 23),
    };

    public BbsScreen()
    {
        _terminal = new TerminalConsole(TermWidth, TermHeight);
        _terminal.UseKeyboard = false; // We handle keyboard ourselves
        _terminal.Position = new Point(0, 0);

        _encoder = _terminal.KeyboardEncoder;

        Children.Add(_terminal);
        _terminal.IsFocused = true;

        UseKeyboard = true;

        ShowConnectionMenu();
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (_awaitingInput)
            return HandleInputMode(keyboard);

        if (_connected)
            return HandleTerminalMode(keyboard);

        // Not connected and not awaiting input — "press any key" state
        if (keyboard.KeysPressed.Count > 0)
        {
            ShowConnectionMenu();
            return true;
        }

        return base.ProcessKeyboard(keyboard);
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
        if (_connected && _telnet != null && !_telnet.IsConnected)
        {
            _connected = false;
            _telnet.Dispose();
            _telnet = null;

            _terminal.Feed("\r\n\r\n\x1b[1;31m--- Disconnected ---\x1b[0m\r\n");
            _terminal.Feed("\r\nPress any key to return to menu...\r\n");
        }
    }

    private void ShowConnectionMenu()
    {
        _terminal.Feed("\x1b[2J\x1b[H"); // Clear screen, home cursor
        _terminal.Feed("\x1b[1;36m+==================================================================+\x1b[0m\r\n");
        _terminal.Feed("\x1b[1;36m|\x1b[0m          \x1b[1;33mSadBBS Client\x1b[0m - SadConsole Terminal Demo          \x1b[1;36m|\x1b[0m\r\n");
        _terminal.Feed("\x1b[1;36m+==================================================================+\x1b[0m\r\n");
        _terminal.Feed("\r\n");
        _terminal.Feed("  \x1b[1;37mSelect a BBS to connect to:\x1b[0m\r\n\r\n");

        for (int i = 0; i < DefaultBbses.Length; i++)
        {
            var bbs = DefaultBbses[i];
            _terminal.Feed($"  \x1b[1;32m{i + 1}\x1b[0m) {bbs.Name,-22} \x1b[90m{bbs.Host}:{bbs.Port}\x1b[0m\r\n");
        }

        _terminal.Feed($"\r\n  \x1b[1;32mC\x1b[0m) Custom address (host:port)\r\n");
        _terminal.Feed($"  \x1b[1;32mQ\x1b[0m) Quit\r\n");
        _terminal.Feed("\r\n  \x1b[1;37mChoice: \x1b[0m");

        _awaitingInput = true;
        _inputBuffer.Clear();
        _connected = false;
    }

    private bool HandleInputMode(Keyboard keyboard)
    {
        foreach (AsciiKey key in keyboard.KeysPressed)
        {
            if (key.Key == Keys.Escape)
            {
                if (_connected || _telnet != null)
                {
                    _telnet?.Dispose();
                    _telnet = null;
                    _connected = false;
                }
                ShowConnectionMenu();
                return true;
            }

            if (key.Key == Keys.Enter)
            {
                string input = _inputBuffer.ToString().Trim();
                _terminal.Feed("\r\n");
                ProcessMenuInput(input);
                return true;
            }

            if (key.Key == Keys.Back)
            {
                if (_inputBuffer.Length > 0)
                {
                    _inputBuffer.Remove(_inputBuffer.Length - 1, 1);
                    _terminal.Feed("\x08 \x08"); // Backspace, space, backspace
                }
                return true;
            }

            if (key.Character != '\0')
            {
                _inputBuffer.Append(key.Character);
                _terminal.Feed(key.Character.ToString());
            }
        }

        return true;
    }

    private void ProcessMenuInput(string input)
    {
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
        {
            SadConsole.Game.Instance.MonoGameInstance.Exit();
            return;
        }

        if (string.Equals(input, "c", StringComparison.OrdinalIgnoreCase))
        {
            _terminal.Feed("\r\n  Enter address (host:port): ");
            _inputBuffer.Clear();
            // Stay in input mode — next Enter will parse as host:port
            _awaitingInput = true;
            return;
        }

        // Try numeric selection
        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= DefaultBbses.Length)
        {
            var bbs = DefaultBbses[choice - 1];
            ConnectTo(bbs.Host, bbs.Port, bbs.Name);
            return;
        }

        // Try host:port format
        if (TryParseAddress(input, out string? host, out int port))
        {
            ConnectTo(host!, port, $"{host}:{port}");
            return;
        }

        _terminal.Feed("\x1b[1;31m  Invalid input. Try again.\x1b[0m\r\n\r\n  \x1b[1;37mChoice: \x1b[0m");
        _inputBuffer.Clear();
    }

    private void ConnectTo(string host, int port, string displayName)
    {
        _awaitingInput = false;

        _terminal.Feed($"\r\n  \x1b[1;33mConnecting to {displayName}...\x1b[0m\r\n");

        try
        {
            _telnet = new TelnetClient(TermWidth, TermHeight);
            _telnet.Disconnected += OnTelnetDisconnected;
            _telnet.Connect(host, port);

            // Wire the telnet client as the terminal output channel
            // so DA/DSR responses go back to the BBS
            _terminal.Output = _telnet;

            _connected = true;
            _terminal.Feed("\x1b[2J\x1b[H"); // Clear for BBS content
        }
        catch (Exception ex)
        {
            _terminal.Feed($"\r\n  \x1b[1;31mConnection failed: {EscapeForTerminal(ex.Message)}\x1b[0m\r\n");
            _terminal.Feed("\r\n  Press any key to return to menu...\r\n");
            _telnet?.Dispose();
            _telnet = null;
            _connected = false;
            _awaitingInput = false;
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
}
