using SFML.Graphics;

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
    /// The render window created by SFML.
    /// </summary>
    public static RenderWindow GraphicsDevice { get; set; }

    /// <summary>
    /// A sprite batch used by all of SadConsole to render objects.
    /// </summary>
    public static SpriteBatch SharedSpriteBatch { get; set; }

    /// <summary>
    /// The output texture. After each screen in SadConsole is drawn, they're then drawn on this output texture to compose the final scene.
    /// </summary>
    public static RenderTexture RenderOutput { get; set; }

    /// <summary>
    /// Reference to the game timer used in the update loop.
    /// </summary>
    public static SFML.System.Clock UpdateTimer { get; set; }

    /// <summary>
    /// Reference to the game timer used in the render loop.
    /// </summary>
    public static SFML.System.Clock DrawTimer { get; set; }
}
