namespace SadConsole.Terminal;

/// <summary>
/// Terminal cursor shapes per DECSCUSR (CSI Ps SP q).
/// Values map directly to the DECSCUSR parameter values.
/// </summary>
public enum CursorShape
{
    /// <summary>DECSCUSR 0 or 1 (0 = default = blinking block)</summary>
    BlinkingBlock = 1,
    /// <summary>DECSCUSR 2</summary>
    SteadyBlock = 2,
    /// <summary>DECSCUSR 3</summary>
    BlinkingUnderline = 3,
    /// <summary>DECSCUSR 4</summary>
    SteadyUnderline = 4,
    /// <summary>DECSCUSR 5</summary>
    BlinkingBar = 5,
    /// <summary>DECSCUSR 6</summary>
    SteadyBar = 6
}
