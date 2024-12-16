namespace SadConsole.ImGuiTypes;

/// <summary>
/// The mirroring mode
/// </summary>
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

    /// <summary>
    /// Mirror vertically and horizontally.
    /// </summary>
    Both = 3,
}

public static class MirrorConverter
{
    public static Mirror FromSadConsoleMirror(SadConsole.Mirror mirror) =>
        mirror switch
        {
            SadConsole.Mirror.Horizontal | SadConsole.Mirror.Vertical => Mirror.Both,
            SadConsole.Mirror.Horizontal => Mirror.Horizontal,
            SadConsole.Mirror.Vertical => Mirror.Vertical,
            _ => Mirror.None,
        };

    public static SadConsole.Mirror ToSadConsoleMirror(this Mirror mirror) =>
        mirror switch
        {
            Mirror.Both => SadConsole.Mirror.Vertical | SadConsole.Mirror.Horizontal,
            Mirror.Horizontal => SadConsole.Mirror.Horizontal,
            Mirror.Vertical => SadConsole.Mirror.Vertical,
            _ => SadConsole.Mirror.None,
        };
}
