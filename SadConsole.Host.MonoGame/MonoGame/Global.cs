using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WPF
using MonoGame.Framework.WpfInterop;
#endif

namespace SadConsole.Host;

/// <summary>
/// Global variables used by the MonoGame host.
/// </summary>
public static class Global
{
    /// <summary>
    /// When <see langword="true"/>, prevents the keyboard and mouse logic from running.
    /// </summary>
    public static bool BlockSadConsoleInput { get; set; }

    /// <summary>
    /// The graphics device created by MonoGame.
    /// </summary>
    public static GraphicsDevice GraphicsDevice { get; set; }

    /// <summary>
    /// A sprite batch used by all of SadConsole to render objects.
    /// </summary>
    public static SpriteBatch SharedSpriteBatch { get; set; }

    /// <summary>
    /// The output texture. After each screen in SadConsole is drawn, they're then drawn on this output texture to compose the final scene.
    /// </summary>
    public static RenderTarget2D RenderOutput { get; set; }

    /// <summary>
    /// Reference to the game timer used in the MonoGame update loop.
    /// </summary>
    public static GameTime UpdateLoopGameTime { get; internal set; }

    /// <summary>
    /// Reference to the game timer used in the MonoGame render loop.
    /// </summary>
    public static GameTime RenderLoopGameTime { get; internal set; }

#if WPF

    /// <summary>
    /// The WPF control used in drawing the screen.
    /// </summary>
    public static RenderTarget2D GraphicsDeviceWpfControl { get; set; }

    /// <summary>
    /// A WPF-specific graphics device service.
    /// </summary>
    public static WpfGraphicsDeviceService GraphicsDeviceManager { get; set; }

    /// <summary>
    /// Sets the <see cref="GraphicsDevice"/> render target to the <see cref="GraphicsDeviceWpfControl"/> control.
    /// </summary>
    public static void ResetGraphicsDevice() =>
        GraphicsDevice.SetRenderTarget(GraphicsDeviceWpfControl);

#else
    /// <summary>
    /// The graphics device manager created by MonoGame.
    /// </summary>
    public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

    /// <summary>
    /// Sets the <see cref="GraphicsDevice"/> render target to <see langword="null"/>, targeting the app window.
    /// </summary>
    public static void ResetGraphicsDevice() =>
        GraphicsDevice.SetRenderTarget(null);
#endif

}
