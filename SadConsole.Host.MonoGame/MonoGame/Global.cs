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
    /// Resizes the <see cref="Global.GraphicsDeviceManager"/> by the specified font size.
    /// </summary>
    /// <param name="fontSize">The size of the font to base the final values on.</param>
    /// <param name="width">The count of glyphs along the X-axis.</param>
    /// <param name="height">The count of glyphs along the Y-axis.</param>
    /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
    /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
    public delegate void ResizeGraphicsDeviceManagerDelegate(Point fontSize, int width, int height, int additionalWidth, int additionalHeight);

    /// <summary>
    /// Regenerates the <see cref="Global.RenderOutput"/> if the desired size doesn't match the current size.
    /// </summary>
    /// <param name="width">The width of the render output.</param>
    /// <param name="height">The height of the render output.</param>
    public delegate void RecreateRenderOutputDelegate(int width, int height);

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

    /// <summary>
    /// Regenerates the <see cref="Global.RenderOutput"/> if the desired size doesn't match the current size.
    /// </summary>
    public static RecreateRenderOutputDelegate RecreateRenderOutput { get; set; } = RecreateRenderOutputHandler;

    /// <summary>
    /// Resizes the <see cref="Global.GraphicsDeviceManager"/> by the specified font size.
    /// </summary>
    public static ResizeGraphicsDeviceManagerDelegate ResizeGraphicsDeviceManager { get; set; } = ResizeGraphicsDeviceManagerHandler;

    /// <summary>
    /// Resets the <see cref="Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
    /// </summary>
    public static Action ResetRendering { get; set; } = ResetRenderingHandler;

    /// <summary>
    /// The game component to control SadConsole updates, input, and rendering.
    /// </summary>
    public static SadConsoleGameComponent SadConsoleComponent { get; set; }

    /// <summary>
    /// The game component that clears the render output before each frame draw.
    /// </summary>
    public static ClearScreenGameComponent ClearScreenComponent { get; set; }

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

    /// <summary>
    /// Resizes the <see cref="Global.GraphicsDeviceManager"/> by the specified font size.
    /// </summary>
    /// <param name="fontSize">The size of the font to base the final values on.</param>
    /// <param name="width">The count of glyphs along the X-axis.</param>
    /// <param name="height">The count of glyphs along the Y-axis.</param>
    /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
    /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
    public static void ResizeGraphicsDeviceManagerHandler(Point fontSize, int width, int height, int additionalWidth, int additionalHeight)
    {
#if !WPF
        GraphicsDeviceManager.PreferredBackBufferWidth = (fontSize.X * width) + additionalWidth;
        GraphicsDeviceManager.PreferredBackBufferHeight = (fontSize.Y * height) + additionalHeight;
#endif
        SadConsole.Settings.Rendering.RenderWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
        SadConsole.Settings.Rendering.RenderHeight = Global.GraphicsDeviceManager.PreferredBackBufferHeight;

        Global.GraphicsDeviceManager.ApplyChanges();
    }
    /// <summary>
    /// Regenerates the <see cref="Global.RenderOutput"/> if the desired size doesn't match the current size.
    /// </summary>
    /// <param name="width">The width of the render output.</param>
    /// <param name="height">The height of the render output.</param>

    public static void RecreateRenderOutputHandler(int width, int height)
    {
        if (Global.RenderOutput == null || Global.RenderOutput.Width != width || Global.RenderOutput.Height != height)
        {
            Global.RenderOutput?.Dispose();
            Global.RenderOutput = new RenderTarget2D(GraphicsDevice, width, height);
        }
    }

    /// <summary>
    /// Resets the <see cref="Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
    /// </summary>
    public static void ResetRenderingHandler()
    {
        if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Center)
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            SadConsole.Settings.Rendering.RenderRect = new Rectangle(
                                                        Math.Max(0, (GraphicsDevice.PresentationParameters.BackBufferWidth - SadConsole.Settings.Rendering.RenderWidth) / 2),
                                                        Math.Max(0, (GraphicsDevice.PresentationParameters.BackBufferHeight - SadConsole.Settings.Rendering.RenderHeight) / 2),
                                                        SadConsole.Settings.Rendering.RenderWidth,
                                                        SadConsole.Settings.Rendering.RenderHeight).ToRectangle();

            SadConsole.Settings.Rendering.RenderScale = (1, 1);
        }
        else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Scale)
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            int multiple = 2;

            // Find the bounds
            while (true)
            {
                if (SadConsole.Settings.Rendering.RenderWidth * multiple > GraphicsDevice.PresentationParameters.BackBufferWidth || SadConsole.Settings.Rendering.RenderHeight * multiple > GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    multiple--;
                    break;
                }

                multiple++;
            }

            SadConsole.Settings.Rendering.RenderRect = new Rectangle(Math.Max(0, (GraphicsDevice.PresentationParameters.BackBufferWidth - (SadConsole.Settings.Rendering.RenderWidth * multiple)) / 2),
                                                                     Math.Max(0, (GraphicsDevice.PresentationParameters.BackBufferHeight - (SadConsole.Settings.Rendering.RenderHeight * multiple)) / 2),
                                                                     SadConsole.Settings.Rendering.RenderWidth * multiple,
                                                                     SadConsole.Settings.Rendering.RenderHeight * multiple).ToRectangle();

            SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / ((float)SadConsole.Settings.Rendering.RenderWidth * multiple), SadConsole.Settings.Rendering.RenderHeight / (float)(SadConsole.Settings.Rendering.RenderHeight * multiple));
        }
        else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Fit)
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            float heightRatio = GraphicsDevice.PresentationParameters.BackBufferHeight / (float)SadConsole.Settings.Rendering.RenderHeight;
            float widthRatio = GraphicsDevice.PresentationParameters.BackBufferWidth / (float)SadConsole.Settings.Rendering.RenderWidth;

            float fitHeight = SadConsole.Settings.Rendering.RenderHeight * widthRatio;
            float fitWidth = SadConsole.Settings.Rendering.RenderWidth * heightRatio;

            if (fitHeight <= GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                // Render width = window width, pad top and bottom

                SadConsole.Settings.Rendering.RenderRect = new Rectangle(0,
                                                                        Math.Max(0, (int)((GraphicsDevice.PresentationParameters.BackBufferHeight - fitHeight) / 2)),
                                                                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                                        (int)fitHeight).ToRectangle();

                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / fitHeight);
            }
            else
            {
                // Render height = window height, pad left and right

                SadConsole.Settings.Rendering.RenderRect = new Rectangle(Math.Max(0, (int)((GraphicsDevice.PresentationParameters.BackBufferWidth - fitWidth) / 2)),
                                                                         0,
                                                                         (int)fitWidth,
                                                                         GraphicsDevice.PresentationParameters.BackBufferHeight).ToRectangle();

                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / fitWidth, SadConsole.Settings.Rendering.RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
            }
        }
        else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.None)
        {
            SadConsole.Settings.Rendering.RenderWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            SadConsole.Settings.Rendering.RenderHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
            SadConsole.Settings.Rendering.RenderScale = (1, 1);
        }
        else // Stretch
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
            SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
        }
    }
}
