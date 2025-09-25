namespace SadConsole;

/// <summary>
/// The mirroring mode
/// </summary>
[System.Flags]
public enum Mirror
{
    /// <summary>
    /// No mirroring set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Mirror vertically.
    /// </summary>
    Vertical = 1,

    /// <summary>
    /// Mirror horizontally.
    /// </summary>
    Horizontal = 2,
}
