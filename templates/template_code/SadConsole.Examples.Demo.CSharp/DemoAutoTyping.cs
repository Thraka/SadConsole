using SadConsole.Components;

namespace SadConsole.Examples;

internal class DemoAutoTyping : IDemo
{
    public string Title => "Auto Typing";

    public string Description => "This demo uses the [c:r f:yellow]Instructions.DrawString[c:u] type to draw a large string on the surface. The instruction uses a [c:r f:yellow]Cursor[c:u] to print each character over a specified amount of time.";

    public string CodeFile => "DemoAutoTyping.cs";

    public IScreenSurface CreateDemoScreen() =>
        new AutoTyping();

    public override string ToString() =>
        Title;
}

class AutoTyping : Console
{
    SadConsole.Instructions.DrawString typingInstruction;

    public AutoTyping() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {

        string[] text = new string[] {
                                           "[c:r f:ansiwhite]Welcome to the [c:g f:black:red:black:10][c:r b:White]SadConsole[c:u] starter project!",
                                           "",
                                           "Not only does the app demonstrate some of the things you can do with ",
                                           "[c:g f:black:red:black:10][c:r b:White]SadConsole[c:u], but it also provides code examples.",
                                           "",
                                           "The following items are examples of what youc an find in this sample app:",
                                           "",
                                           "[c:r f:LightGreen]01[c:r f:green])[c:u 2] Splash Screen     -- [c:r f:white]Sample splash screen advertising [c:g f:black:red:black:10][c:r b:White]SadConsole[c:u 2]",
                                           "[c:r f:LightGreen]02[c:r f:green])[c:u 2] String Parser     -- [c:r f:white]Examples of using the string parser functions[c:u]",
                                           "[c:r f:LightGreen]03[c:r f:green])[c:u 2] Controls Test     -- [c:r f:white]Displays all SadConsole.Controls for testing[c:u]",
                                           "[c:r f:LightGreen]04[c:r f:green])[c:u 2] DOS\\Terminal      -- [c:r f:white]A terminal prompt that accepts commands[c:u]",
                                           "[c:r f:LightGreen]05[c:r f:green])[c:u 2] Ascii Graphics    -- [c:r f:white]Displays Ansi, REXPaint and Playscii files.[c:u]",
                                           "[c:r f:LightGreen]06[c:r f:green])[c:u 2] Shapes            -- [c:r f:white]Draw different shapes[c:u]",
                                           "[c:r f:LightGreen]07[c:r f:green])[c:u 2] Entity demo       -- [c:r f:white]1000s of entities on the screen moving around[c:u]",
                                           "[c:r f:LightGreen]08[c:r f:green])[c:u 2] Scroll Control    -- [c:r f:white]Single surface with multiple views + scrolling[c:u]",
                                           "[c:r f:LightGreen]09[c:r f:green])[c:u 2] Stretched Console -- [c:r f:white]Shows how to print with a font that x2 in size[c:u]",
                                           "[c:r f:LightGreen]10[c:r f:green])[c:u 2] Multiple Cursors  -- [c:r f:white]Multiple cursors typing on a single console[c:u]",
                                           "[c:r f:LightGreen]11[c:r f:green])[c:u 2] Transparent fade  -- [c:r f:white]Demonstrates fading\\blending a console[c:u]",
                                           "[c:r f:LightGreen]12[c:r f:green])[c:u 2] 2000x2000 Console -- [c:r f:white]2000x2000 console that is scrollable[c:u]",
                                           "[c:r f:LightGreen]13[c:r f:green])[c:u 2] Serialization     -- [c:r f:white]Test serializing various types from SadConsole[c:u]",
                                           "[c:r f:LightGreen]14[c:r f:green])[c:u 2] Animations        -- [c:r f:white]Show use of instructions and AnimatedScreenSurface[c:u]",
                                           "[c:r f:LightGreen]15[c:r f:green])[c:u 2] And more!!",
                                         };

        // We want this to print on a sub region of the main console, so we'll create a sub view and use that
        typingInstruction = new SadConsole.Instructions.DrawString(SadConsole.ColoredString.Parser.Parse(string.Join("\r\n", text)));
        typingInstruction.TotalTimeToPrint = TimeSpan.FromSeconds(8); // 0.5 seconds per line of text

        Cursor.Position = new SadRogue.Primitives.Point(1, 1);
        Cursor.IsEnabled = false;
        Cursor.IsVisible = true;
        typingInstruction.Cursor = Cursor;

        SadComponents.Add(typingInstruction);

        //Cursor.IsVisible = true;
    }
}
