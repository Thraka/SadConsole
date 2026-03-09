#nullable enable

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SadConsole.Terminal;

namespace SadBBSClient;

/// <summary>
/// A telnet client that connects to remote BBSes via raw TCP.
/// Implements <see cref="ITerminalOutput"/> so DA/DSR responses and user keystrokes
/// are sent back to the BBS through the same socket.
/// </summary>
public class TelnetClient : ITerminalOutput, IDisposable
{
    // Telnet protocol constants
    private const byte IAC  = 0xFF;
    private const byte WILL = 0xFB;
    private const byte WONT = 0xFC;
    private const byte DO   = 0xFD;
    private const byte DONT = 0xFE;
    private const byte SB   = 0xFA;
    private const byte SE   = 0xF0;

    // Telnet option codes
    private const byte OPT_ECHO  = 1;
    private const byte OPT_SGA   = 3;
    private const byte OPT_TTYPE = 24;
    private const byte OPT_NAWS  = 31;

    private TcpClient? _tcp;
    private NetworkStream? _stream;
    private Thread? _readThread;
    private volatile bool _running;

    private readonly int _termWidth;
    private readonly int _termHeight;
    private readonly ConcurrentQueue<byte[]> _receiveQueue = new();

    /// <summary>
    /// Raised when the connection is lost (remotely or due to error).
    /// </summary>
    public event Action? Disconnected;

    /// <summary>
    /// Whether the client is currently connected.
    /// </summary>
    public bool IsConnected => _tcp?.Connected == true && _running;

    /// <summary>
    /// Creates a new telnet client for the given terminal dimensions.
    /// </summary>
    public TelnetClient(int termWidth = 80, int termHeight = 25)
    {
        _termWidth = termWidth;
        _termHeight = termHeight;
    }

    /// <summary>
    /// Connects to the specified host and port.
    /// </summary>
    public void Connect(string host, int port)
    {
        Disconnect();

        _tcp = new TcpClient();
        _tcp.Connect(host, port);
        _stream = _tcp.GetStream();
        _running = true;

        _readThread = new Thread(ReadLoop)
        {
            IsBackground = true,
            Name = "TelnetReadLoop"
        };
        _readThread.Start();
    }

    /// <summary>
    /// Disconnects from the remote host.
    /// </summary>
    public void Disconnect()
    {
        _running = false;
        _stream?.Close();
        _tcp?.Close();
        _stream = null;
        _tcp = null;
    }

    /// <summary>
    /// Drains all received data chunks from the thread-safe queue.
    /// Call this from the game thread (Update loop) to safely retrieve data.
    /// </summary>
    /// <param name="action">Callback invoked for each received chunk.</param>
    public void DrainReceived(Action<byte[]> action)
    {
        while (_receiveQueue.TryDequeue(out byte[]? data))
            action(data);
    }

    // --- ITerminalOutput implementation ---

    /// <summary>
    /// Writes raw bytes to the remote BBS (DA/DSR responses, user input).
    /// </summary>
    public void Write(byte[] data)
    {
        try
        {
            _stream?.Write(data, 0, data.Length);
            _stream?.Flush();
        }
        catch (Exception)
        {
            // Connection lost during write — will be caught by read loop
        }
    }

    /// <summary>
    /// Writes a string to the remote BBS, encoded as UTF-8.
    /// </summary>
    public void Write(string text) => Write(Encoding.UTF8.GetBytes(text));

    /// <summary>
    /// Sends raw user keystroke bytes to the remote BBS.
    /// </summary>
    public void SendKeyData(byte[] data) => Write(data);

    public void Dispose()
    {
        Disconnect();
        GC.SuppressFinalize(this);
    }

    private void ReadLoop()
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (_running && _stream != null)
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break; // Remote closed

                // Process telnet IAC commands in-band, enqueue clean data
                byte[] clean = ProcessTelnet(buffer, bytesRead);
                if (clean.Length > 0)
                    _receiveQueue.Enqueue(clean);
            }
        }
        catch (Exception)
        {
            // Socket closed or error
        }
        finally
        {
            _running = false;
            Disconnected?.Invoke();
        }
    }

    /// <summary>
    /// Strips telnet IAC sequences from raw data, handling negotiation in-line.
    /// Returns the cleaned data with telnet commands removed.
    /// </summary>
    private byte[] ProcessTelnet(byte[] buffer, int length)
    {
        var clean = new byte[length];
        int cleanIdx = 0;
        int i = 0;

        while (i < length)
        {
            if (buffer[i] == IAC && i + 1 < length)
            {
                byte cmd = buffer[i + 1];

                switch (cmd)
                {
                    case WILL:
                        if (i + 2 < length)
                        {
                            HandleWill(buffer[i + 2]);
                            i += 3;
                        }
                        else i = length;
                        continue;

                    case WONT:
                        if (i + 2 < length)
                        {
                            HandleWont(buffer[i + 2]);
                            i += 3;
                        }
                        else i = length;
                        continue;

                    case DO:
                        if (i + 2 < length)
                        {
                            HandleDo(buffer[i + 2]);
                            i += 3;
                        }
                        else i = length;
                        continue;

                    case DONT:
                        // Accept DONT for any option
                        i += (i + 2 < length) ? 3 : length;
                        continue;

                    case SB:
                        i = HandleSubnegotiation(buffer, i, length);
                        continue;

                    case IAC:
                        // Escaped 0xFF — pass single 0xFF through
                        clean[cleanIdx++] = IAC;
                        i += 2;
                        continue;

                    default:
                        // Skip unknown 2-byte command
                        i += 2;
                        continue;
                }
            }

            clean[cleanIdx++] = buffer[i];
            i++;
        }

        if (cleanIdx == 0) return Array.Empty<byte>();

        var result = new byte[cleanIdx];
        Array.Copy(clean, result, cleanIdx);
        return result;
    }

    private void HandleWill(byte option)
    {
        switch (option)
        {
            case OPT_ECHO:
            case OPT_SGA:
                // Accept: BBS wants to handle echo / suppress go-ahead
                SendCommand(DO, option);
                break;
            default:
                SendCommand(DONT, option);
                break;
        }
    }

    private void HandleWont(byte option)
    {
        // Acknowledge with DONT
        SendCommand(DONT, option);
    }

    private void HandleDo(byte option)
    {
        switch (option)
        {
            case OPT_TTYPE:
                // We support terminal type negotiation
                SendCommand(WILL, option);
                break;
            case OPT_NAWS:
                // We support window size negotiation
                SendCommand(WILL, option);
                SendNaws();
                break;
            case OPT_SGA:
                SendCommand(WILL, option);
                break;
            default:
                SendCommand(WONT, option);
                break;
        }
    }

    private int HandleSubnegotiation(byte[] buffer, int start, int length)
    {
        // Find IAC SE to delimit the subnegotiation
        int end = start + 2;
        while (end < length - 1)
        {
            if (buffer[end] == IAC && buffer[end + 1] == SE)
            {
                // Process the subnegotiation content
                ProcessSubnegotiation(buffer, start + 2, end - start - 2);
                return end + 2;
            }
            end++;
        }

        // Incomplete subneg — skip to end
        return length;
    }

    private void ProcessSubnegotiation(byte[] buffer, int offset, int subLength)
    {
        if (subLength < 1) return;

        byte option = buffer[offset];

        switch (option)
        {
            case OPT_TTYPE:
                // BBS asks: IAC SB 24 1 IAC SE  (SEND = 1)
                if (subLength >= 2 && buffer[offset + 1] == 1)
                    SendTerminalType();
                break;
        }
    }

    private void SendCommand(byte cmd, byte option)
    {
        Write(new byte[] { IAC, cmd, option });
    }

    private void SendNaws()
    {
        // IAC SB NAWS <width-hi> <width-lo> <height-hi> <height-lo> IAC SE
        byte wHi = (byte)((_termWidth >> 8) & 0xFF);
        byte wLo = (byte)(_termWidth & 0xFF);
        byte hHi = (byte)((_termHeight >> 8) & 0xFF);
        byte hLo = (byte)(_termHeight & 0xFF);

        Write(new byte[] { IAC, SB, OPT_NAWS, wHi, wLo, hHi, hLo, IAC, SE });
    }

    private void SendTerminalType()
    {
        // IAC SB TTYPE IS "ANSI" IAC SE  (IS = 0)
        byte[] termType = Encoding.ASCII.GetBytes("ANSI");
        byte[] response = new byte[4 + termType.Length + 2];
        response[0] = IAC;
        response[1] = SB;
        response[2] = OPT_TTYPE;
        response[3] = 0; // IS
        Array.Copy(termType, 0, response, 4, termType.Length);
        response[response.Length - 2] = IAC;
        response[response.Length - 1] = SE;

        Write(response);
    }
}
