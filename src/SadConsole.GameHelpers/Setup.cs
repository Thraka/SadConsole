using SadConsole.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Libraries
{
    /// <summary>
    /// Represents the GameHelpers library.
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        /// Should be called when you first use the library so that things hook up to SadConsole.Core correctly.
        /// </summary>
        public static void Initialize()
        {
            GameObject.EnsureMapping();
        }
    }
}
