namespace SadConsole.Terminal;

/// <summary>
/// Provides a response channel for a terminal emulator.
/// When terminal sequences produce responses (DA, DSR, DECRQM), the terminal
/// sends them through this interface. When <see langword="null"/> on the
/// <see cref="Writer"/>, the terminal operates in silent data-stream mode.
/// </summary>
public interface ITerminalOutput
{
    /// <summary>
    /// Writes raw response bytes to the output channel.
    /// </summary>
    /// <param name="data">The bytes to send.</param>
    void Write(byte[] data);

    /// <summary>
    /// Writes a string response to the output channel (encoded as UTF-8).
    /// </summary>
    /// <param name="text">The text to send.</param>
    void Write(string text);
}
