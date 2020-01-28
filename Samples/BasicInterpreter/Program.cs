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
            //Teletype keyboardComponent = new Teletype();

            //var runtimeEnv = new ClassicBasic.Interpreter.RunEnvironment();
            //var programRep = new ClassicBasic.Interpreter.ProgramRepository();
            //var tokensProvider = new new ClassicBasic.Interpreter.TokensProvider(new ClassicBasic.Interpreter.IToken[] { });
            //var tokenizer = new ClassicBasic.Interpreter.Tokeniser(tokensProvider);

            //Executor = new ClassicBasic.Interpreter.Executor(keyboardComponent, runtimeEnv, programRep, tokensProvider, tokenizer);


            //Interpreter = new ClassicBasic.Interpreter.Interpreter(keyboardComponent, tokenizer, runtimeEnv, programRep, Executor);
            Global.Screen.SadComponents.Add(new ConsoleBASICInterpreter());
            Global.Screen.Cursor.IsVisible = true;
            Global.Screen.Cursor.IsEnabled = true;
            Global.Screen.IsFocused = true;
        }
    }
}
