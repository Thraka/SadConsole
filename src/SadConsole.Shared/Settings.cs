using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public static class Settings
    {
        /// <summary>
        /// The color to automatically clear the device with.
        /// </summary>
        public static Color ClearColor = Color.Black;

        /// <summary>
        /// The type of resizing options for the window.
        /// </summary>
        public static WindowResizeOptions ResizeMode = WindowResizeOptions.Scale;

        /// <summary>
        /// Allow the user to resize the window. Must be set before the game is created.
        /// </summary>
        public static bool AllowWindowResize = true;

        /// <summary>
        /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
        /// </summary>
        public static bool UnlimitedFPS = false;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Draw(GameTime)"/> will run.
        /// </summary>
        public static bool DoDraw = true;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Draw(GameTime)"/> will render to the screen at the end.
        /// </summary>
        public static bool DoFinalDraw = true;

        /// <summary>
        /// When true, indicates that <see cref="SadConsole.Game.SadConsoleGameComponent.Update(GameTime)"/> will run.
        /// </summary>
        public static bool DoUpdate = true;

        internal static bool IsExitingFullscreen = false;

        /// <summary>
        /// Tells MonoGame to use a full screen resolution change instead of soft (quick) full screen. Must be set before the game is created.
        /// </summary>
        public static bool UseHardwareFullScreen { get; set; } = false;

        public static void ToggleFullScreen()
        {
#if !WPF
            Global.GraphicsDeviceManager.ApplyChanges();
            
            // Coming back from fullscreen
            if (Global.GraphicsDeviceManager.IsFullScreen)
            {
                Global.GraphicsDeviceManager.IsFullScreen = !Global.GraphicsDeviceManager.IsFullScreen;

                Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.WindowWidth;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.WindowHeight;
                Global.GraphicsDeviceManager.ApplyChanges();

                Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.WindowWidth;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.WindowHeight;
                Global.GraphicsDeviceManager.ApplyChanges();
            }

            // Going full screen
            else
            {
                Global.WindowWidth = Global.GraphicsDeviceManager.PreferredBackBufferWidth;
                Global.WindowHeight = Global.GraphicsDeviceManager.PreferredBackBufferHeight;

                Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

                Global.GraphicsDeviceManager.IsFullScreen = !Global.GraphicsDeviceManager.IsFullScreen;
                Global.GraphicsDeviceManager.ApplyChanges();
            }
#endif
        }

        /// <summary>
        /// Returns the width of the MonoGame drawing space.
        /// </summary>
        public static int PreferredBackBufferWidth
        {
            get
            {
#if WPF
                return (int)SadConsole.Game.Instance.RenderSize.Width;
#else
                return SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth;
#endif
            }
        }

        /// <summary>
        /// Returns the height of the MonoGame drawing space.
        /// </summary>
        public static int PreferredBackBufferHeight
        {
            get
            {
#if WPF
                //return (int)Height;
                return (int)SadConsole.Game.Instance.RenderSize.Height;
#else
                return SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight;
#endif
            }
        }

        /// <summary>
        /// Resizes the MonoGame drawing space, also resizes the window.
        /// </summary>
        public static void ResizeBackbuffer(int width, int height)
        {
#if WPF
            //SadConsole.Game.Instance.RenderSize = new System.Windows.Size(width, height);
            SadConsole.Global.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth = width;
            SadConsole.Global.GraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
            SadConsole.Game.Instance.Width = width;
            SadConsole.Game.Instance.Height = height;
#else
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();
#endif
        }

        public static string WindowTitle
        {
            get
            {
#if WPF || FORMS
                return "SadConsole";
#else
                return SadConsole.Game.Instance.Window.Title;
#endif
            }
            set
            {
#if !WPF && !FORMS
                SadConsole.Game.Instance.Window.Title = value;
#endif
            }
        }

        public static class Input
        {
            public static bool ProcessMouseOffscreen = false;

            public static bool DoMouse = true;

            public static bool DoKeyboard = true;
        }

        public enum WindowResizeOptions
        {
            Stretch,
            Center,
            Scale,
        }
    }
}
