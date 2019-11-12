using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Host
{
    public static class Settings
    {
        /// <summary>
        /// Tells MonoGame to use a full screen resolution change instead of soft (quick) full screen. Must be set before the game is created.
        /// </summary>
        public static bool UseHardwareFullScreen { get; set; } = false;

        /// <summary>
        /// The target frames per second for the game window. Set before calling <see cref="SadConsole.Game.Create(int, int, string)"/>.
        /// </summary>
        public static int FPS { get; set; } = 60;

        /// <summary>
        /// The game window title. Set before calling <see cref="SadConsole.Game.Create(int, int, string)"/>.
        /// </summary>
        public static string WindowTitle { get; set; } = "SadConsole Game";
    }
}
