using System;
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
    /// Helpers for generic libraries that need to access the render output texture in a standard property.
    /// </summary>
    /// <remarks></remarks>
    public static Texture RenderOutputTexture => RenderOutput.Texture;

    /// <summary>
    /// Gets the width, in pixels, of the rendered output.
    /// </summary>
    /// <remarks>This property retrieves the width from the underlying texture used for rendering. It is
    /// useful for determining the horizontal dimension of the output when performing layout calculations or rendering
    /// operations.</remarks>
    public static int RenderOutputWidth => (int)RenderOutput.Texture.Size.X;

    /// <summary>
    /// Gets the height, in pixels, of the texture used for rendering output.
    /// </summary>
    /// <remarks>This property provides the vertical dimension of the rendering surface. Use it to determine
    /// the required display space or to align graphical elements based on the output's height.</remarks>
    public static int RenderOutputHeight => (int)RenderOutput.Texture.Size.Y;

    /// <summary>
    /// Reference to the game timer used in the update loop.
    /// </summary>
    public static SFML.System.Clock UpdateTimer { get; set; }

    /// <summary>
    /// Reference to the game timer used in the render loop.
    /// </summary>
    public static SFML.System.Clock DrawTimer { get; set; }

    /// <summary>
    /// A callback invoked after the final draw phase, before presenting to the screen.
    /// Used by overlay systems like ImGui to render on top of SadConsole.
    /// </summary>
    public static Action? DrawOverlay { get; set; }

    /// <summary>
    /// A callback invoked before the update phase, used by overlay systems like ImGui to update their state before SadConsole updates.
    /// </summary>
    public static Action<TimeSpan>? UpdateOverlay { get; set; }
}
