using SadConsole.StringParser;

namespace SadConsole.Examples;

internal class DemoStringParsing : IDemo
{
    public string Title => "String Parser";

    public string Description => "SadConsole has a proprietary string-based command parsing system. The string parser uses a simple syntax and supports commands like:\r\n\r\n" +
                                 "[c:r f:violet]*[c:u] Recolor\r\n" +
                                 "[c:r f:violet]*[c:u] Gradient\r\n" +
                                 "[c:r f:violet]*[c:u] Mirror\r\n" +
                                 "[c:r f:violet]*[c:u] Blink\r\n";



    public string CodeFile => "DemoStringParsing.cs";

    public IScreenSurface CreateDemoScreen() =>
        new StringParsingConsole();

    public override string ToString() =>
        Title;
}

class StringParsingConsole : ScreenSurface
{
    public StringParsingConsole() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Surface.UsePrintProcessor = true;
        UseKeyboard = false;
        Surface.DefaultForeground = Color.AnsiWhite;
        Surface.Clear();
        int c = 59;
        int r = 1;

        ((Default)ColoredString.Parser).CustomProcessor = CustomParseCommand;

        ((Default)ColoredString.Parser).Variables["first_name"] = () => "John";
        ((Default)ColoredString.Parser).Variables["last_name"] = () => "Smith";

        Surface.Print(1, r, "[c:r f:ansibluebright][c:r b:ansiblue]String parsing supports...                                                    ");
        Surface.SetGlyph(0, r, 221, Color.Black, Color.AnsiBlue);

        r = 2;
        Color a = Color.Green;
        Surface.Print(1, ++r, "[c:r f:DodgerBlue]1.[c:r f:White] Multi-colored [c:r f:yellow]str[c:r f:red]ings");
        r++;
        Surface.Print(1, ++r, "[c:r f:DodgerBlue]2.[c:r f:White] You color the backgrounds: [c:r f:black][c:r b:Purple] Purple [c:u] [c:r b:Cyan] Cyan ");
        r++;
        Surface.Print(1, ++r, "[c:r f:DodgerBlue]3.[c:r f:White] [c:g f:LimeGreen:Orange:9]Gradients are [c:g b:Black:Red:Yellow:Red:Black:15]easily supported");
        r++;
        Surface.Print(1, ++r, "[c:r f:DodgerBlue]4.[c:r f:White] You [c:b 3]can apply [c:b 5:0.17]blink effects.");
        r++;
        Surface.Print(1, ++r, "[c:r f:DodgerBlue]5.[c:r f:White] You set [c:r f:greenyellow:9]mirroring [c:m 1]on any [c:m 2]text [c:u][c:u]you want.");

        r += 2;
        Surface.Print(1, r, "[c:r f:ansibluebright][c:r b:ansiblue]Examples                                                                      ");
        Surface.SetGlyph(0, r, 221, Color.Black, Color.AnsiBlue);

        //var temp = Font.Master.GetDecorator("underline", Color.White);
        //SetDecorator(1, 3, 24, new[] { temp });
        //temp = Font.Master.GetDecorator("strikethrough", Color.White);
        //SetDecorator(1, 5, 24, new[] { temp });

        //temp = Font.Master.GetDecorator("box-edge-left-top-bottom", Color.White);
        //SetDecorator(1, 7, 1, new[] { temp });
        //var doubletemp = new CellDecorator[] { Font.Master.GetDecorator("box-edge-top", Color.White),
        //                                       Font.Master.GetDecorator("box-edge-bottom", Color.White) };
        //SetDecorator(2, 7, 31, doubletemp);
        //temp = Font.Master.GetDecorator("box-edge-top-right-bottom", Color.White);
        //SetDecorator(33, 7, 1, new[] { temp });


        r += 2;

        //SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
        //line.StartingLocation = new Point(c, r);
        //line.EndingLocation = new Point(c, r + 5);
        //line.UseEndingCell = false;
        //line.UseStartingCell = false;
        //line.Cell.Glyph = 179;
        //line.Draw(this);

        Surface.Print(1, r, "Some `[c:r f:red]text`[c:u] to print                       ");
        Surface.Print(c + 2, r, $"Some [c:r f:{Color.Red.ToParser()}]text[c:u] to print");



        Surface.Print(1, ++r, "Some `[c:r f:100,100,33]text`[c:u] to print");
        Surface.Print(c + 2, r, "Some [c:r f:255,255,0]text[c:u] to print");

        Surface.Print(1, ++r, "Some `[c:r b:ansiblackbright]text to `[c:r f:white]print");
        Surface.Print(c + 2, r, "Some [c:r b:ansiblackbright]text to [c:r f:white]print");

        Surface.Print(1, ++r, "`[c:m 1]Some `[c:r f:purple]text to`[c:u] pr`[c:u]int");
        Surface.Print(c + 2, r, "[c:m 1]Some [c:r f:purple]text to[c:u] pr[c:u]int");

        Surface.Print(1, ++r, "[c:r f:default]`[c:g f:red:green:18]`[c:g b:green:red:18]Some text to print");
        Surface.Print(c + 2, r, "[c:g f:red:green:18][c:g b:green:red:18]Some text to print");

        Surface.Print(1, ++r, "[c:r f:default]`[c:r f:BurlyWood]$first_name`[c:u] `[c:r f:Lav...");
        Surface.Print(c + 2, r, "[c:r f:BurlyWood]$$first_name[c:u] [c:r f:LavenderBlush]$$last_name[c:u]");
    }

    private ParseCommandBase? CustomParseCommand(string command, string parameters, ColoredGlyphBase[] glyphString,
                                                      ICellSurface? surface, ParseCommandStacks? commandStacks)
    {
        return command switch
        {
            "retext" => new ParseCommandRetext(parameters),
            "t" => new ParseCommandRetext(parameters),
            _ => null,
        };
    }

    private class ParseCommandRetext : ParseCommandBase
    {
        public int Counter;
        public char Glyph;

        public ParseCommandRetext(string parameters)
        {
            string[] parts = parameters.Split(new char[] { ':' }, 2);

            // Count and glyph type provided
            if (parts.Length == 2)
                Counter = int.Parse(parts[1]);
            else
                Counter = -1;

            // Get character
            Glyph = parts[0][0];

            // No exceptions, set the type
            CommandType = CommandTypes.Glyph;
        }
        public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
        {
            glyphState.Glyph = Glyph;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }
    }
}
