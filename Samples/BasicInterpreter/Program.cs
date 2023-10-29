using BasicTerminal;
using ClassicBasic.Interpreter;
using SadConsole.Configuration;

Settings.WindowTitle = "SadConsole Examples";

Builder startup = new Builder()
    .SetScreenSize(80, 25)
    .SetStartingScreen(game => {

        Console console = new(80, 25);
        console.Cursor.IsVisible = true;
        console.Cursor.IsEnabled = true;
        console.Cursor.SortOrder = 1;

        ConsoleBASICInterpreter interpreter = new ConsoleBASICInterpreter();
        interpreter.BASICTokensProvider.Tokens.Add(new ClearScreen(interpreter));
        console.SadComponents.Add(interpreter);

        console.SortComponents();

        return console;
    })
    .IsStartingScreenFocused(true)
    .ConfigureFonts()
    .SetSplashScreen<SadConsole.SplashScreens.Ansi1>()
    ;

Game.Create(startup);
Game.Instance.Run();
Game.Instance.Dispose();

class ClearScreen : Token, ICommand
{
    private readonly ConsoleBASICInterpreter _teleTypeInterpreter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Clear"/> class.
    /// </summary>
    /// <param name="runEnvironment">Run time environment.</param>
    /// <param name="variableRepository">Variable Repository.</param>
    /// <param name="dataStatementReader">Data statement reader.</param>
    public ClearScreen(ConsoleBASICInterpreter teleTypeInterpreter) : base("CLR", TokenClass.Statement)
    {
        _teleTypeInterpreter = teleTypeInterpreter;
    }

    /// <summary>
    /// Executes the CLEAR command.
    /// </summary>
    public void Execute()
    {
        _teleTypeInterpreter.ClearScreen();
    }
}
