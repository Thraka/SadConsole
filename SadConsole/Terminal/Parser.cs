using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SadConsole.Terminal;

/// <summary>
/// Determines how the parser interprets bytes above 0x7F in ground state.
/// </summary>
public enum ParserEncoding
{
    /// <summary>Bytes 0x80-0xFF are treated as single-byte glyph indices (CP437). Default for BBS/ANSI art.</summary>
    Codepage437,
    /// <summary>Bytes 0x80-0xFF may be UTF-8 multibyte lead/continuation bytes.</summary>
    Utf8
}

/// <summary>
/// ECMA-48 state machine parser for terminal byte streams.
/// </summary>
public sealed class Parser
{
    private const int MaxParameters = 16;
    private const int MaxIntermediates = 2;
    private const byte Escape = 0x1B;
    private const byte Cancel = 0x18;
    private const byte Substitute = 0x1A;
    private const byte StringTerminator = 0x9C;

    private readonly ITerminalHandler _handler;
    private readonly int[] _parameters;
    private readonly byte[] _intermediates;
    private readonly List<byte> _stringBuffer;
    private readonly Decoder _utf8Decoder;
    private readonly char[] _charBuffer;

    private State _state;
    private int _paramCount;
    private int _intermediateCount;
    private int _currentParam;
    private bool _paramHasValue;
    private bool _paramSeen;
    private byte? _privatePrefix;
    private byte _escIntermediate;
    private bool _escIntermediateSet;
    private byte _dcsFinal;

    /// <summary>
    /// How bytes above 0x7F are interpreted in ground state. Default is <see cref="ParserEncoding.Codepage437"/>.
    /// </summary>
    public ParserEncoding Encoding { get; set; } = ParserEncoding.Codepage437;

    /// <summary>
    /// Creates a new parser with the provided handler.
    /// </summary>
    /// <param name="handler">Handler receiving parsed events.</param>
    public Parser(ITerminalHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _parameters = new int[MaxParameters];
        _intermediates = new byte[MaxIntermediates];
        _stringBuffer = new List<byte>(64);
        _utf8Decoder = System.Text.Encoding.UTF8.GetDecoder();
        _charBuffer = new char[2];
        _state = State.Ground;
    }

    /// <summary>
    /// Feeds a span of bytes into the parser.
    /// </summary>
    /// <param name="data">Bytes to parse.</param>
    public void Feed(ReadOnlySpan<byte> data)
    {
        foreach (byte b in data)
        {
            Feed(b);
        }
    }

    /// <summary>
    /// Feeds a single byte into the parser.
    /// </summary>
    /// <param name="b">Byte to parse.</param>
    public void Feed(byte b)
    {
        switch (_state)
        {
            case State.Ground:
                HandleGround(b);
                break;
            case State.Escape:
                HandleEscape(b);
                break;
            case State.EscapeIntermediate:
                HandleEscapeIntermediate(b);
                break;
            case State.CsiEntry:
            case State.CsiParam:
                HandleCsiParam(b);
                break;
            case State.CsiIntermediate:
                HandleCsiIntermediate(b);
                break;
            case State.DcsEntry:
            case State.DcsParam:
                HandleDcsParam(b);
                break;
            case State.DcsPassthrough:
                HandleDcsPassthrough(b);
                break;
            case State.DcsStringEscape:
                HandleDcsStringEscape(b);
                break;
            case State.OscString:
                HandleOscString(b);
                break;
            case State.OscStringEscape:
                HandleOscStringEscape(b);
                break;
            default:
                _state = State.Ground;
                break;
        }
    }

    /// <summary>
    /// Resets the parser to its initial state.
    /// </summary>
    public void Reset()
    {
        if (Encoding == ParserEncoding.Utf8)
        {
            _utf8Decoder.Reset();
        }

        ResetSequenceData();
        _state = State.Ground;
    }

    private void HandleGround(byte b)
    {
        if (b == Escape)
        {
            StartEscape();
            return;
        }

        if (b <= 0x1F)
        {
            _handler.OnC0Control(b);
            return;
        }

        if (b <= 0x7E)
        {
            _handler.OnPrint((char)b);
            return;
        }

        if (b == 0x7F)
        {
            return;
        }

        // Bytes > 0x7F: encoding-dependent
        if (Encoding == ParserEncoding.Utf8)
        {
            DecodeUtf8(b);
        }
        else
        {
            // CP437: byte IS the glyph index
            _handler.OnPrint((char)b);
        }
    }

    private void HandleEscape(byte b)
    {
        if (HandleC0InSequence(b))
        {
            return;
        }

        if (b == Escape)
        {
            StartEscape();
            return;
        }

        if (IsAbortControl(b))
        {
            ResetToGround();
            return;
        }

        if (b >= 0x20 && b <= 0x2F)
        {
            _escIntermediate = b;
            _escIntermediateSet = true;
            _state = State.EscapeIntermediate;
            return;
        }

        if (b >= 0x30 && b <= 0x3F)
        {
            DispatchEsc(b);
            return;
        }

        if (b >= 0x40 && b <= 0x5F)
        {
            switch (b)
            {
                case (byte)'[':
                    EnterCsiEntry();
                    break;
                case (byte)']':
                    EnterOscString();
                    break;
                case (byte)'P':
                    EnterDcsEntry();
                    break;
                default:
                    DispatchEsc(b);
                    break;
            }

            return;
        }

        if (b >= 0x60 && b <= 0x7E)
        {
            DispatchEsc(b);
            return;
        }

        // Unrecognized byte — reset to ground
        ResetToGround();
    }

    private void HandleEscapeIntermediate(byte b)
    {
        if (HandleC0InSequence(b))
        {
            return;
        }

        if (b == Escape)
        {
            StartEscape();
            return;
        }

        if (IsAbortControl(b))
        {
            ResetToGround();
            return;
        }

        if (b >= 0x20 && b <= 0x2F)
        {
            _escIntermediate = b;
            _escIntermediateSet = true;
            return;
        }

        if (b >= 0x30 && b <= 0x7E)
        {
            DispatchEsc(b);
        }
    }

    private void HandleCsiParam(byte b)
    {
        if (HandleSequenceControl(b))
        {
            return;
        }

        if (IsCsiPrivatePrefix(b))
        {
            if (!_paramSeen && !_paramHasValue && _paramCount == 0 && _privatePrefix is null)
            {
                _privatePrefix = b;
                _state = State.CsiParam;
            }

            return;
        }

        if (b >= (byte)'0' && b <= (byte)'9')
        {
            _currentParam = (_currentParam * 10) + (b - (byte)'0');
            _paramHasValue = true;
            _paramSeen = true;
            _state = State.CsiParam;
            return;
        }

        if (b == (byte)';')
        {
            AddParameter(_paramHasValue ? _currentParam : 0);
            _currentParam = 0;
            _paramHasValue = false;
            _paramSeen = true;
            _state = State.CsiParam;
            return;
        }

        if (b >= 0x20 && b <= 0x2F)
        {
            _state = State.CsiIntermediate;
            AddIntermediate(b);
            return;
        }

        if (b >= 0x40 && b <= 0x7E)
        {
            DispatchCsi(b);
        }
    }

    private void HandleCsiIntermediate(byte b)
    {
        if (HandleSequenceControl(b))
        {
            return;
        }

        if (b >= 0x20 && b <= 0x2F)
        {
            AddIntermediate(b);
            return;
        }

        if (b >= 0x40 && b <= 0x7E)
        {
            DispatchCsi(b);
        }
    }

    private void HandleDcsParam(byte b)
    {
        if (HandleSequenceControl(b))
        {
            return;
        }

        if (b >= (byte)'0' && b <= (byte)'9')
        {
            _currentParam = (_currentParam * 10) + (b - (byte)'0');
            _paramHasValue = true;
            _paramSeen = true;
            _state = State.DcsParam;
            return;
        }

        if (b == (byte)';')
        {
            AddParameter(_paramHasValue ? _currentParam : 0);
            _currentParam = 0;
            _paramHasValue = false;
            _paramSeen = true;
            _state = State.DcsParam;
            return;
        }

        if (b >= 0x20 && b <= 0x2F)
        {
            AddIntermediate(b);
            _state = State.DcsParam;
            return;
        }

        if (b >= 0x40 && b <= 0x7E)
        {
            _dcsFinal = b;
            _stringBuffer.Clear();
            _state = State.DcsPassthrough;
        }
    }

    private void HandleDcsPassthrough(byte b)
    {
        if (b == StringTerminator)
        {
            DispatchDcs();
            return;
        }

        if (b == Escape)
        {
            _state = State.DcsStringEscape;
            return;
        }

        if (IsAbortControl(b))
        {
            ResetToGround();
            return;
        }

        if (HandleC0InSequence(b))
        {
            return;
        }

        _stringBuffer.Add(b);
    }

    private void HandleDcsStringEscape(byte b)
    {
        if (b == (byte)'\\')
        {
            DispatchDcs();
            return;
        }

        StartEscape();
        HandleEscape(b);
    }

    private void HandleOscString(byte b)
    {
        if (b == StringTerminator)
        {
            DispatchOsc();
            return;
        }

        // BEL (0x07) terminates OSC strings (xterm convention)
        if (b == 0x07)
        {
            DispatchOsc();
            return;
        }

        if (b == Escape)
        {
            _state = State.OscStringEscape;
            return;
        }

        if (IsAbortControl(b))
        {
            ResetToGround();
            return;
        }

        if (HandleC0InSequence(b))
        {
            return;
        }

        _stringBuffer.Add(b);
    }

    private void HandleOscStringEscape(byte b)
    {
        if (b == (byte)'\\')
        {
            DispatchOsc();
            return;
        }

        StartEscape();
        HandleEscape(b);
    }

    private void DispatchEsc(byte final)
    {
        byte intermediate = _escIntermediateSet ? _escIntermediate : (byte)0;
        _handler.OnEscDispatch(intermediate, final);
        ResetToGround();
    }

    private void DispatchCsi(byte final)
    {
        FinalizeParameters();
        _handler.OnCsiDispatch(_parameters.AsSpan(0, _paramCount), _intermediates.AsSpan(0, _intermediateCount), final, _privatePrefix);
        ResetToGround();
    }

    private void DispatchOsc()
    {
        _handler.OnOscDispatch(CollectionsMarshal.AsSpan(_stringBuffer));
        ResetToGround();
    }

    private void DispatchDcs()
    {
        FinalizeParameters();
        _handler.OnDcsDispatch(_parameters.AsSpan(0, _paramCount), _intermediates.AsSpan(0, _intermediateCount), _dcsFinal, CollectionsMarshal.AsSpan(_stringBuffer));
        ResetToGround();
    }

    private void DecodeUtf8(byte b)
    {
        Span<byte> bytes = stackalloc byte[1];
        bytes[0] = b;
        _utf8Decoder.Convert(bytes, _charBuffer, false, out _, out int charsUsed, out _);

        for (int i = 0; i < charsUsed; i++)
        {
            _handler.OnPrint(_charBuffer[i]);
        }
    }

    private bool HandleSequenceControl(byte b)
    {
        if (HandleC0InSequence(b))
        {
            return true;
        }

        if (b == Escape)
        {
            StartEscape();
            return true;
        }

        if (IsAbortControl(b))
        {
            ResetToGround();
            return true;
        }

        return false;
    }

    private bool HandleC0InSequence(byte b)
    {
        if (!IsSequenceC0(b))
        {
            return false;
        }

        _handler.OnC0Control(b);
        return true;
    }

    private static bool IsSequenceC0(byte b) => b is 0x07 or 0x08 or 0x09 or 0x0A or 0x0D;

    private static bool IsAbortControl(byte b) => b is Cancel or Substitute;

    private static bool IsCsiPrivatePrefix(byte b) => b is (byte)'?' or (byte)'=' or (byte)'<' or (byte)'>';

    private void StartEscape()
    {
        ResetSequenceData();
        _state = State.Escape;
    }

    private void ResetToGround()
    {
        ResetSequenceData();
        _state = State.Ground;
    }

    private void EnterCsiEntry()
    {
        ResetSequenceData();
        _state = State.CsiEntry;
    }

    private void EnterDcsEntry()
    {
        ResetSequenceData();
        _state = State.DcsEntry;
    }

    private void EnterOscString()
    {
        ResetSequenceData();
        _state = State.OscString;
    }

    private void ResetSequenceData()
    {
        _paramCount = 0;
        _intermediateCount = 0;
        _currentParam = 0;
        _paramHasValue = false;
        _paramSeen = false;
        _privatePrefix = null;
        _escIntermediate = 0;
        _escIntermediateSet = false;
        _dcsFinal = 0;
        _stringBuffer.Clear();
    }

    private void FinalizeParameters()
    {
        if (_paramHasValue || _paramSeen)
        {
            AddParameter(_paramHasValue ? _currentParam : 0);
        }
        // If nothing was seen, leave _paramCount at 0 — empty params
    }

    private void AddParameter(int value)
    {
        if (_paramCount >= MaxParameters)
        {
            return;
        }

        _parameters[_paramCount++] = value;
    }

    private void AddIntermediate(byte value)
    {
        if (_intermediateCount >= MaxIntermediates)
        {
            return;
        }

        _intermediates[_intermediateCount++] = value;
    }

    private enum State
    {
        Ground,
        Escape,
        EscapeIntermediate,
        CsiEntry,
        CsiParam,
        CsiIntermediate,
        DcsEntry,
        DcsParam,
        DcsPassthrough,
        DcsStringEscape,
        OscString,
        OscStringEscape
    }
}
