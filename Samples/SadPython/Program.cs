using Microsoft.Scripting.Hosting;
using SadConsole.Configuration;

// Python config options
Dictionary<string, object> options = new();
options["Debug"] = true;

// Create the python engine
ScriptEngine engine = IronPython.Hosting.Python.CreateEngine(options);

try
{
    // Load game.py and compile it
    ScriptSource script = engine.CreateScriptSourceFromFile("game.py");
    CompiledCode compiledScript = script.Compile();

    // Run the script
    compiledScript.Execute();

    // Run the getGameConfig method and get its return value, which should be a SadConsole.Configuration.Builder type
    Builder config = compiledScript.DefaultScope.GetVariable("getGameConfig")();

    // Boot up SadConsole
    Game.Create(config);
    Game.Instance.Run();
    Game.Instance.Dispose();
}
catch (Exception e1)
{
    // The game crashed. To help debug Python, this exception catch formats
    // the exception text and recreates SadConsole to print out the error

    string errorMessage = engine.GetService<ExceptionOperations>().FormatException(e1);

    System.Console.WriteLine(errorMessage);

    ColorExtensions2.ColorMappings?.Clear();

    if (Game.Instance != null && Game.Instance.MonoGameInstance != null)
        Game.Instance.MonoGameInstance.Dispose();

    Game.Instance = null!;
    Game.Create(80, 20, new(StartupCrash));
    Game.Instance!.Run();
    Game.Instance.Dispose();
    
    void StartupCrash(object? sender, GameHost gameHost)
    {
        gameHost.StartingConsole!.Clear();
        gameHost.StartingConsole!.Cursor.DisableWordBreak = true;
        gameHost.StartingConsole!.Cursor.Move(0, 1)
                                        .Print("Error starting SadConsole with Python")
                                        .NewLine()
                                        .Print(errorMessage);
    }
}
