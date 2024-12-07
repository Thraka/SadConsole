using SadConsole.Ansi;
using SadConsole.Input;
using SadConsole.Readers;
using SadConsole.UI;
using SadConsole.UI.Controls;
using static SadConsole.Examples.RootScreen;

namespace SadConsole.Examples;

internal class DemoPlayground : IDemo
{
    public string Title => "Playground";

    public string Description => "This demo exists as a place to mess with SadConsole features. By default, it's empty, but add code to the [c:r f:yellow:w]Playground class hosted in this code file.";

    public string CodeFile => "DemoPlayground.cs";

    public IScreenSurface CreateDemoScreen() =>
        new Playground();

    public override string ToString() =>
        Title;
}

internal class Playground : ControlsConsole
{
    public Playground() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Controls.Add(new TextBox(15) { Tag = "First name", Position = (5, 5) });
        Controls.Add(new TextBox(15) { Tag = "Last name", Position = (5, 9) });
        Controls.Add(new TextBox(15) { Tag = "User name", Position = (5, 13) });

        FrameTextBoxes();
    }

    void FrameTextBoxes()
    {
        foreach (TextBox control in Controls.OfType<TextBox>())
        {
            if (control.Tag is string value)
            {
                Surface.DrawBox(control.Bounds.Expand(1, 1), ShapeParameters.CreateStyledBoxThin(Color.White));
                Point stringPos = control.Position + (0, -1);
                Surface.Print(stringPos.X, stringPos.Y, $" {value} ");
            }
        }
    }
}
