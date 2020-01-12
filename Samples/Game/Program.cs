using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Game
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseDefaultExtendedFont = true;

            SadConsole.Game.Create(80, 25);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            //SadConsole.Settings.gam.Window.Title = "DemoProject Core";
            using var reader = System.IO.File.OpenRead("DEMO.ZZT");
            var world = ZReader.ZWorld.Load(reader);

            Global.Screen.Renderer = null;
            var worldScreen = new Screens.WorldPlay();
            Global.Screen.Children.Add(worldScreen);
            worldScreen.SadComponents.Add(new KeyboardChangeBoard(world));
            worldScreen.UseKeyboard = true;
            worldScreen.IsFocused = true;
        }


        private class KeyboardChangeBoard : SadConsole.Components.KeyboardConsoleComponent
        {
            int mapIndex = 0;
            ZReader.ZWorld world;

            public KeyboardChangeBoard(ZReader.ZWorld world) =>
                this.world = world;

            public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
            {
                if (keyboard.IsKeyPressed(Keys.Space))
                    LoadNextMap((Screens.WorldPlay)host);

                handled = true;
            }

            private void LoadNextMap(Screens.WorldPlay worldScreen)
            {
                worldScreen.SetActiveBoard(worldScreen.ImportZZTBoard(world.Boards[mapIndex]).Name);
                mapIndex++;

                if (mapIndex >= world.Boards.Length)
                    mapIndex = 0;
            }
        }
    }
}
