using System;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;


namespace SadConsole.Host
{
    /// <summary>
    /// A MonoGame <see cref="Microsoft.Xna.Framework.Game"/> instance that runs SadConsole.
    /// </summary>
    public sealed partial class Game : WpfGame
    {
        public WpfKeyboard Keyboard;
        public WpfMouse Mouse;
        public RenderTarget2D WpfRenderTarget;

        public event EventHandler SadConsoleStarted;

        private bool _initFirstSize = false;
        private bool _initMain = false;

        public string FontPath
        {
            get { return (string)GetValue(FontPathProperty); }
            set { SetValue(FontPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontPathProperty =
            DependencyProperty.Register("FontPath", typeof(string), typeof(Game), new PropertyMetadata(""));

        public bool ResetRenderingNextFrame = false;

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
        public int WindowWidth => Global.GraphicsDeviceManager.PreferredBackBufferWidth;

        /// <summary>
        /// The current game window height.
        /// </summary>
        public int WindowHeight => Global.GraphicsDeviceManager.PreferredBackBufferHeight;

        /// <summary>
        /// Raised when the window is resized and the render area has been calculated.
        /// </summary>
        public event EventHandler WindowResized;

        public Game()
        {
            SadConsole.Game.Instance = new SadConsole.Game();
            SadConsole.Game.Instance.MonoGameInstance = this;
        }

        /// <inheritdoc/>
        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            Global.GraphicsDeviceManager = new WpfGraphicsDeviceService(this);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            Keyboard = new WpfKeyboard(this);
            Mouse = new WpfMouse(this);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            Global.GraphicsDevice = GraphicsDevice;

            _initMain = true;

            if (_initMain && _initFirstSize)
                SadConsoleStarted?.Invoke(this, EventArgs.Empty);
        }


        public void ActivateRenderSizeChanged(SizeChangedInfo sizeInfo) =>
            OnRenderSizeChanged(sizeInfo);

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!_initFirstSize)
            {
                _initFirstSize = true;
                SadConsole.Game.Instance.MonoGameInit((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height, (string)GetValue(FontPathProperty));

                // Initialize the SadConsole engine with a font, and a screen size that mirrors MS-DOS.
                SadConsoleComponent = new SadConsoleGameComponent(this);
                ClearScreenComponent = new ClearScreenGameComponent(this);

                Components.Add(SadConsoleComponent);
                Components.Add(ClearScreenComponent);

                Global.SharedSpriteBatch = new SpriteBatch(GraphicsDevice, 5000);

                if (_initMain && _initFirstSize)
                    SadConsoleStarted?.Invoke(this, EventArgs.Empty);
            }

            ResetRendering();
            WindowResized?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Resets the <see cref="Global.RenderOutput"/> target and determines the appropriate <see cref="SadConsole.Settings.Rendering.RenderRect"/> and <see cref="SadConsole.Settings.Rendering.RenderScale"/> based on the window or fullscreen state.
        /// </summary>
        public void ResetRendering()
        {
            Global.RenderOutput?.Dispose();

            if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Center)
            {
                Global.RenderOutput = new RenderTarget2D(GraphicsDevice, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                SadConsole.Settings.Rendering.RenderRect = new Rectangle(
                                                            (Global.GraphicsDeviceManager.PreferredBackBufferWidth - SadConsole.Settings.Rendering.RenderWidth) / 2,
                                                            (Global.GraphicsDeviceManager.PreferredBackBufferHeight - SadConsole.Settings.Rendering.RenderHeight) / 2,
                                                            SadConsole.Settings.Rendering.RenderWidth,
                                                            SadConsole.Settings.Rendering.RenderHeight).ToRectangle();

                SadConsole.Settings.Rendering.RenderScale = (1, 1);
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Scale)
            {
                Global.RenderOutput = new RenderTarget2D(GraphicsDevice, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                int multiple = 2;

                // Find the bounds
                while (true)
                {
                    if (SadConsole.Settings.Rendering.RenderWidth * multiple > Global.GraphicsDeviceManager.PreferredBackBufferWidth || SadConsole.Settings.Rendering.RenderHeight * multiple > Global.GraphicsDeviceManager.PreferredBackBufferHeight)
                    {
                        multiple--;
                        break;
                    }

                    multiple++;
                }

                SadConsole.Settings.Rendering.RenderRect = new Rectangle((Global.GraphicsDeviceManager.PreferredBackBufferWidth - (SadConsole.Settings.Rendering.RenderWidth * multiple)) / 2,
                                                                         (Global.GraphicsDeviceManager.PreferredBackBufferHeight - (SadConsole.Settings.Rendering.RenderHeight * multiple)) / 2,
                                                                         SadConsole.Settings.Rendering.RenderWidth * multiple,
                                                                         SadConsole.Settings.Rendering.RenderHeight * multiple).ToRectangle();
                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / ((float)SadConsole.Settings.Rendering.RenderWidth * multiple), SadConsole.Settings.Rendering.RenderHeight / (float)(SadConsole.Settings.Rendering.RenderHeight * multiple));
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.Fit)
            {
                Global.RenderOutput = new RenderTarget2D(GraphicsDevice, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                float heightRatio = Global.GraphicsDeviceManager.PreferredBackBufferHeight / (float)SadConsole.Settings.Rendering.RenderHeight;
                float widthRatio = Global.GraphicsDeviceManager.PreferredBackBufferWidth / (float)SadConsole.Settings.Rendering.RenderWidth;

                float fitHeight = SadConsole.Settings.Rendering.RenderHeight * widthRatio;
                float fitWidth = SadConsole.Settings.Rendering.RenderWidth * heightRatio;

                if (fitHeight <= Global.GraphicsDeviceManager.PreferredBackBufferHeight)
                {
                    // Render width = window width, pad top and bottom

                    SadConsole.Settings.Rendering.RenderRect = new Rectangle(0,
                                                                            (int)((Global.GraphicsDeviceManager.PreferredBackBufferHeight - fitHeight) / 2),
                                                                            Global.GraphicsDeviceManager.PreferredBackBufferWidth,
                                                                            (int)fitHeight).ToRectangle();

                    SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)Global.GraphicsDeviceManager.PreferredBackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / fitHeight);
                }
                else
                {
                    // Render height = window height, pad left and right

                    SadConsole.Settings.Rendering.RenderRect = new Rectangle((int)((Global.GraphicsDeviceManager.PreferredBackBufferWidth - fitWidth) / 2),
                                                                             0,
                                                                             (int)fitWidth,
                                                                             Global.GraphicsDeviceManager.PreferredBackBufferHeight).ToRectangle();

                    SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / fitWidth, SadConsole.Settings.Rendering.RenderHeight / (float)Global.GraphicsDeviceManager.PreferredBackBufferHeight);
                }
            }
            else if (SadConsole.Settings.ResizeMode == SadConsole.Settings.WindowResizeOptions.None)
            {
                SadConsole.Settings.Rendering.RenderWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
                SadConsole.Settings.Rendering.RenderHeight = Global.GraphicsDeviceManager.PreferredBackBufferHeight;
                Global.RenderOutput = new RenderTarget2D(GraphicsDevice, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
                SadConsole.Settings.Rendering.RenderScale = (1, 1);
            }
            else
            {
                Global.RenderOutput = new RenderTarget2D(GraphicsDevice, SadConsole.Settings.Rendering.RenderWidth, SadConsole.Settings.Rendering.RenderHeight);
                SadConsole.Settings.Rendering.RenderRect = GraphicsDevice.Viewport.Bounds.ToRectangle();
                SadConsole.Settings.Rendering.RenderScale = (SadConsole.Settings.Rendering.RenderWidth / (float)Global.GraphicsDeviceManager.PreferredBackBufferWidth, SadConsole.Settings.Rendering.RenderHeight / (float)Global.GraphicsDeviceManager.PreferredBackBufferHeight);
            }
        }

    }
}
