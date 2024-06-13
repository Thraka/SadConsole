using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Configuration;
using SadConsole.Host;

namespace SadConsole.Host;

/// <summary>
/// A MonoGame <see cref="Microsoft.Xna.Framework.Game"/> instance that runs SadConsole.
/// </summary>
public partial class Game : Microsoft.Xna.Framework.Game
{
    internal bool _resizeBusy = false;
    internal Action<Game> _initCallback;

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
    public Game()
    {
        Global.GraphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = Settings.GraphicsProfile
        };

        Content.RootDirectory = "Content";

#if !FNA
        Global.GraphicsDeviceManager.HardwareModeSwitch = Settings.UseHardwareFullScreen;
#endif

        MonoGameCallbackConfig config = SadConsole.Game.Instance._configuration.Configs.OfType<MonoGameCallbackConfig>().FirstOrDefault();
        config?.MonoGameCtorCallback?.Invoke(this);
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
                    _resizeBusy = false;
                }
            }
        }
        else
        {
            _resizeBusy = false;
        }

        Global.ResetRendering();

        if (!_resizeBusy)
        {
            WindowResized?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        // Window title
        Window.Title = SadConsole.Settings.WindowTitle;

        // Let the XNA framework show the mouse.
        IsMouseVisible = true;

        // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
        Global.SadConsoleComponent = new SadConsoleGameComponent(this);
        Global.ClearScreenComponent = new ClearScreenGameComponent(this);

        Components.Add(Global.SadConsoleComponent);
        Components.Add(Global.ClearScreenComponent);

        // Initializes the components and loads content
        base.Initialize();

        // Hook window change for resolution fixes
        Window.ClientSizeChanged += Window_ClientSizeChanged;
        Window.AllowUserResizing = SadConsole.Settings.AllowWindowResize;

        // After we've init, clear the graphics device so everything is ready to start
        GraphicsDevice.SetRenderTarget(null);
    }
}
