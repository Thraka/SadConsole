using System.Diagnostics;
using Microsoft.Scripting.Hosting;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.Configuration;

Dictionary<string, object> options = new Dictionary<string, object>();
options["Debug"] = true;

ScriptEngine engine = IronPython.Hosting.Python.CreateEngine(options);
Builder config;

try
{
    ScriptSource script = engine.CreateScriptSourceFromFile("game.py");
    CompiledCode compiledScript = script.Compile();


    compiledScript.Execute();

    config = compiledScript.DefaultScope.GetVariable("getGameConfig")();


    Game.Create(config);
    Game.Instance.Run();
    Game.Instance.Dispose();
}
catch (Exception e1)
{
    System.Console.WriteLine(engine.GetService<ExceptionOperations>().FormatException(e1));
}




//Game.Configuration gameStartup = new Game.Configuration()
//    .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
//    .SetStartingScreen<multilayers>()
//    .SetStartingScreen<othertest>()
//    .SetStartingScreen<RootScene>()
//    .IsStartingScreenFocused(false)
//    .ConfigureFonts((f) => f.UseBuiltinFontExtended())
//    ;

//Game.Create(gameStartup);
//Game.Instance.Run();
//Game.Instance.Dispose();

