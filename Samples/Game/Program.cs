using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Game
{
    internal class Program
    {
        public static ClassicBasic.Interpreter.Interpreter Interpreter;
        public static ClassicBasic.Interpreter.Executor Executor;

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

            //Global.Screen.Renderer = null;
            Global.Screen.DrawBox(new Rectangle(1, 1, 10, 10), new ColoredGlyph(Color.AliceBlue), null, ICellSurface.CreateLine(ICellSurface.ConnectedLineThin[2]));
            Global.Screen.DrawBox(new Rectangle(4, 4, 10, 10), new ColoredGlyph(Color.Magenta), null, ICellSurface.CreateLine(ICellSurface.ConnectedLineThin[5]));
            Global.Screen.DrawBox(new Rectangle(8, 8, 10, 10), new ColoredGlyph(Color.PapayaWhip), null, ICellSurface.CreateLine(ICellSurface.ConnectedLineThin[7]));

            Global.Screen.DrawLine((6, 0), (6, 20), Color.IndianRed, null, ICellSurface.ConnectedLineThin[0]);
            Global.Screen.Components.Add(new keyboardComponent());

            Global.Screen.IsFocused = true;
        }

        class keyboardComponent: SadConsole.Components.KeyboardConsoleComponent
        {
            public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
            {
                if (keyboard.IsKeyReleased(Keys.Space))
                    ((ICellSurface)host).ConnectLines();

                handled = true;
            }
        }
    }
}
