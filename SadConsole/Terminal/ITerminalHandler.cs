using System;

namespace SadConsole.Terminal;

/// <summary>
/// Receives parsed terminal events from <see cref="Parser"/>.
/// </summary>
public interface ITerminalHandler
{
    /// <summary>
    /// Called for printable characters.
    /// </summary>
    void OnPrint(char ch);

    /// <summary>
    /// Called for C0 control codes.
    /// </summary>
    void OnC0Control(byte controlCode);

    /// <summary>
    /// Called for ESC dispatch sequences.
    /// </summary>
    void OnEscDispatch(byte intermediate, byte final);

    /// <summary>
    /// Called for CSI dispatch sequences.
    /// </summary>
    void OnCsiDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, byte? privatePrefix);

    /// <summary>
    /// Called for OSC string dispatch.
    /// </summary>
    void OnOscDispatch(ReadOnlySpan<byte> payload);

    /// <summary>
    /// Called for DCS string dispatch.
    /// </summary>
    void OnDcsDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, ReadOnlySpan<byte> payload);
}
