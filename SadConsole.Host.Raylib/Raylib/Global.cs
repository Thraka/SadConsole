using Raylib_cs;

namespace SadConsole.Host;

/// <summary>
/// Global variables used by the SFML host.
/// </summary>
public static class Global
{
    /// <summary>
    /// When <see langword="true"/>, prevents the keyboard and mouse logic from running.
    /// </summary>
    public static bool BlockSadConsoleInput { get; set; }

    /// <summary>
    /// The output texture. After each screen in SadConsole is drawn, they're then drawn on this output texture to compose the final scene.
    /// </summary>
    public static RenderTexture2D RenderOutput { get; set; }
}
