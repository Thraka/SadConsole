using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Host;

/// <summary>
/// A MonoGame <see cref="Microsoft.Xna.Framework.Game"/> instance that runs SadConsole.
/// </summary>
public partial class Game : Microsoft.Xna.Framework.Game
{
    internal bool _resizeBusy = false;
    internal Action<Game> _initCallback;

    /// <summary>
    /// The game component to control SadConsole updates, input, and rendering.
    /// </summary>
    public SadConsoleGameComponent SadConsoleComponent;

    /// <summary>
    /// The game component that clears the render output before each frame draw.
    /// </summary>
    public ClearScreenGameComponent ClearScreenComponent;

    /// <summary>
    /// The current game window width.
    /// </summary>
    public int WindowWidth => GraphicsDevice.PresentationParameters.BackBufferWidth;

    /// <summary>
    /// The current game window height.
    /// </summary>
    public int WindowHeight => GraphicsDevice.PresentationParameters.BackBufferHeight;

    /// <summary>
    /// Raised when the window is resized and the render area has been calculated.
    /// </summary>
    public event EventHandler WindowResized;

    /// <summary>
    /// Creates the new MonoGame game object.
    /// </summary>
    internal Game()
    {
        Global.GraphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = Settings.GraphicsProfile
        };

        Content.RootDirectory = "Content";
        
        Global.GraphicsDeviceManager.HardwareModeSwitch = Settings.UseHardwareFullScreen;

        SadConsole.Game.Instance._configuration.MonoGameCtorCallback?.Invoke(this);
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        if (!_resizeBusy)
        {
            if (!Global.GraphicsDeviceManager.IsFullScreen && SadConsole.Settings.WindowMinimumSize != Point.Zero.ToPoint())
            {
                if (GraphicsDevice.PresentationParameters.BackBufferWidth < SadConsole.Settings.WindowMinimumSize.X
                    || GraphicsDevice.PresentationParameters.BackBufferHeight < SadConsole.Settings.WindowMinimumSize.Y)
                {
                    _resizeBusy = true;
                    Global.GraphicsDeviceManager.PreferredBackBufferWidth = SadConsole.Settings.WindowMinimumSize.X;
                    Global.GraphicsDeviceManager.PreferredBackBufferHeight = SadConsole.Settings.WindowMinimumSize.Y;
                    Global.GraphicsDeviceManager.ApplyChanges();
                }
            }
        }
        else
        {
            _resizeBusy = false;
        }

        //if (!resizeBusy && Settings.IsExitingFullscreen)
        //{
        //    GraphicsDeviceManager.PreferredBackBufferWidth = Global.WindowWidth;
        //    GraphicsDeviceManager.PreferredBackBufferHeight = Global.WindowHeight;

        //    resizeBusy = true;
        //    GraphicsDeviceManager.ApplyChanges();
        //    resizeBusy = false;
        //    Settings.IsExitingFullscreen = false;
        //}

        //Global.WindowWidth = GraphicsDeviceManager.PreferredBackBufferWidth;
        //Global.WindowHeight = GraphicsDeviceManager.PreferredBackBufferHeight;
        //Global.WindowWidth = Global.RenderWidth = GraphicsDeviceManager.PreferredBackBufferWidth;
        //Global.WindowHeight = Global.RenderHeight = GraphicsDeviceManager.PreferredBackBufferHeight;
        ResetRendering();

        if (!_resizeBusy)
        {
            WindowResized?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Global.GraphicsDevice = GraphicsDevice;
        if (SadConsole.Settings.UnlimitedFPS)
        {
            Global.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }
        Window.Title = SadConsole.Settings.WindowTitle;

        // Let the XNA framework show the mouse.
        IsMouseVisible = true;

        // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
        SadConsoleComponent = new SadConsoleGameComponent(this);
        ClearScreenComponent = new ClearScreenGameComponent(this);

        Components.Add(SadConsoleComponent);
        Components.Add(ClearScreenComponent);

        // Call the default initialize of the base class.
        base.Initialize();

        // Hook window change for resolution fixes
        Window.ClientSizeChanged += Window_ClientSizeChanged;
        Window.AllowUserResizing = SadConsole.Settings.AllowWindowResize;

        Global.SharedSpriteBatch = new SpriteBatch(GraphicsDevice, 5000);

        SadConsole.Game.Instance._configuration.MonoGameInitCallback?.Invoke(this);

        ResetRendering();

        // After we've init, clear the graphics device so everything is ready to start
        GraphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Resizes the <see cref="Global.GraphicsDeviceManager"/> by the specified font size.
    /// </summary>
    /// <param name="fontSize">The size of the font to base the final values on.</param>
    /// <param name="width">The count of glyphs along the X-axis.</param>
    /// <param name="height">The count of glyphs along the Y-axis.</param>
    /// <param name="additionalWidth">Additional pixel width to add to the resize.</param>
    /// <param name="additionalHeight">Additional pixel height to add to the resize.</param>
    public void ResizeGraphicsDeviceManager(Point fontSize, int width, int height, int additionalWidth, int additionalHeight)
    {
        Global.GraphicsDeviceManager.PreferredBackBufferWidth = (fontSize.X * width) + additionalWidth;
        Global.GraphicsDeviceManager.PreferredBackBufferHeight = (fontSize.Y * height) + additionalHeight;

        SadConsole.Settings.Rendering.RenderWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
        SadConsole.Settings.Rendering.RenderHeight = Global.GraphicsDeviceManager.PreferredBackBufferHeight;

        Global.GraphicsDeviceManager.ApplyChanges();
        
    }

    /// <summary>
    /// Regenerates the <see cref="Global.RenderOutput"/> if the desired size doesn't match the current size.
    /// </summary>
    /// <param name="width">The width of the render output.</param>
    /// <param name="height">The height of the render output.</param>
    protected virtual void RecreateRenderOutput(int width, int height)
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
    public virtual void ResetRendering()
    {
        if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Center)
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            SadConsole.Settings.Rendering.RenderRect = new Rectangle(
                                                        (GraphicsDevice.PresentationParameters.BackBufferWidth - SadConsole.Settings.Rendering.RenderWidth) / 2,
                                                        (GraphicsDevice.PresentationParameters.BackBufferHeight - SadConsole.Settings.Rendering.RenderHeight) / 2,
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

            SadConsole.Settings.Rendering.RenderRect = new Rectangle((GraphicsDevice.PresentationParameters.BackBufferWidth - (SadConsole.Settings.Rendering.RenderWidth * multiple)) / 2,
                                                                     (GraphicsDevice.PresentationParameters.BackBufferHeight - (SadConsole.Settings.Rendering.RenderHeight * multiple)) / 2,
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
                                                                        (int)((GraphicsDevice.PresentationParameters.BackBufferHeight - fitHeight) / 2),
                                                                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                                        (int)fitHeight).ToRectangle();

                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / fitHeight);
            }
            else
            {
                // Render height = window height, pad left and right

                SadConsole.Settings.Rendering.RenderRect = new Rectangle((int)((GraphicsDevice.PresentationParameters.BackBufferWidth - fitWidth) / 2),
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
        else
        {
            RecreateRenderOutput(SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
            SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
            SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)GraphicsDevice.PresentationParameters.BackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / (float)GraphicsDevice.PresentationParameters.BackBufferHeight);
        }
    }
}
