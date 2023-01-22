using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using SadRogue.Primitives.GridViews;
using SadConsole.Effects;
using System.Collections;
using SadConsole.Quick;
using SadConsole.Readers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SadConsole.Editor
{
    internal class Program
    {
        public static int MainWidth = 80;
        public static int MainHeight = 23;
        public static int HeaderWidth = 80;
        public static int HeaderHeight = 2;

        private static void Main(string[] args)
        {
            Settings.WindowTitle = "Feature Demo (MonoGame)";
            Settings.CreateStartingConsole = false;

            Game.Configuration config =
                new Game.Configuration()
                    .SetScreenSize(130, 50)
                    .OnStart(Init)
                    .SetStartingScreen<ScreenObject>();

            SadConsole.Game.Create(config);
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            // Register the types provided by the SadConsole.Extended library
            SadConsole.UI.RegistrarExtended.Register();

            ImGuiCore.Start();
        }
    }
}
