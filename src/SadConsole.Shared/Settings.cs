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
        public static bool AllowWindowResize = false;

        /// <summary>
        /// Unlimited FPS when rendering (normally limited to 60fps). Must be set before the game is created.
        /// </summary>
        public static bool UnlimitedFPS = false;

        public static bool DoDraw = true;
        public static bool DoFinalDraw = true;
        public static bool DoUpdate = true;

        /// <summary>
        /// Tells MonoGame to use a full screen resolution change instead of soft (quick) full screen. Must be set before the game is created.
        /// </summary>
        public static bool UseHardwareFullScreen { get; set; } = false;

        public static void ToggleFullScreen()
        {
            // Coming back from fullscreen
            if (Global.GraphicsDeviceManager.IsFullScreen)
            {
                Global.GraphicsDeviceManager.PreferredBackBufferWidth = Global.RenderWidth;
                Global.GraphicsDeviceManager.PreferredBackBufferHeight = Global.RenderHeight;

                
            }

            // Going full screen
            else
            {

            }

            Global.GraphicsDeviceManager.ToggleFullScreen();
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
