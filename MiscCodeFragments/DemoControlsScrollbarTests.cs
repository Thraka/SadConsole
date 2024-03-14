using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoControlsScrollbarTests : IDemo
{
    public string Title => "Controls (Scrollbar)";

    public string Description => "Test area";

    public string CodeFile => "DemoControlsScrollbarTests.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ControlsTest3();

    public override string ToString() =>
        Title;
}

class ControlsTest3 : SadConsole.UI.ControlsConsole
{
    public ControlsTest3() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Controls.ThemeColors = GameSettings.ControlColorScheme;

        CreateBox((1, 1), 10, 10);
        CreateBox((3, 3), 5, 10);
        CreateBox((6, 5), 10, 5);

        CreateBox2((20, 1), 10, 1);
        CreateBox2((22, 3), 10, 2);
        CreateBox2((24, 5), 10, 5);
        CreateBox2((26, 7), 10, 10);
        CreateBox2((28, 9), 10, 15);
        CreateBox2((30, 11), 10, 7);
        CreateBox2((32, 13), 2, 7);
        CreateBox2((34, 15), 3, 7);
        CreateBox2((36, 17), 4, 7);
        CreateBox2((38, 19), 5, 7);

        CreateBox3((54, 2), 10, 1);
        CreateBox3((54, 5), 10, 2);
        CreateBox3((54, 8), 10, 5);
        CreateBox3((54, 11), 10, 10);
        CreateBox3((54, 14), 10, 15);
        CreateBox3((54, 17), 10, 7);
        CreateBox3((54, 20), 2, 7);
        CreateBox3((54, 23), 3, 1);
        CreateBox3((65, 2), 4, 1);
        CreateBox3((65, 5), 5, 1);
        CreateBox3((65, 8), 6, 1);
    }

    private (ScrollBar bar, Label label) CreateBox3(Point position, int height, int max)
    {
        ScrollBar bar1 = new(Orientation.Horizontal, height);
        Label label1 = new(10);
        bar1.Position = position;
        bar1.MaximumValue = max;
        bar1.ArrowsMoveGrip = true;
        bar1.Tag = label1;
        label1.PlaceRelativeTo(bar1, Direction.Types.Up, 0);
        label1.DisplayText = "Label";

        bar1.ValueChanged += Bar2_ValueChanged;

        Controls.Add(bar1);
        Controls.Add(label1);

        Bar2_ValueChanged(bar1, EventArgs.Empty);

        return (bar1, label1);
    }

    private (ScrollBar bar, Label label) CreateBox2(Point position, int height, int max)
    {
        ScrollBar bar1 = new(Orientation.Vertical, height);
        Label label1 = new(10);
        bar1.Position = position;
        bar1.MaximumValue = max;
        bar1.MouseWheelMovesGrip = true;
        bar1.Tag = label1;
        label1.PlaceRelativeTo(bar1, Direction.Types.Right);
        label1.DisplayText = "Label";

        bar1.ValueChanged += Bar2_ValueChanged;

        Controls.Add(bar1);
        Controls.Add(label1);

        Bar2_ValueChanged(bar1, EventArgs.Empty);

        return (bar1, label1);
    }

    private (ScrollBar bar, Label label) CreateBox(Point position, int height, int max)
    {
        ScrollBar bar1 = new(Orientation.Vertical, height);
        Label label1 = new(10);
        bar1.Position = position;
        bar1.MaximumValue = max;
        bar1.Tag = label1;
        label1.PlaceRelativeTo(bar1, Direction.Types.Right);
        label1.DisplayText = "Label";

        bar1.ValueChanged += Bar1_ValueChanged;

        Controls.Add(bar1);
        Controls.Add(label1);

        Bar1_ValueChanged(bar1, EventArgs.Empty);

        return (bar1, label1);
    }

    private void Bar1_ValueChanged(object? sender, EventArgs e)
    {
        ScrollBar control = (ScrollBar)sender!;
        ((Label)control.Tag!).DisplayText = $"{control.Value}/{control.MaximumValue} {control.Style.GripStart}";
    }

    private void Bar2_ValueChanged(object? sender, EventArgs e)
    {
        ScrollBar control = (ScrollBar)sender!;
        ((Label)control.Tag!).DisplayText = $"{control.Value}/{control.MaximumValue} {control.Style.GripStart}";
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        return base.ProcessMouse(state);
    }
}




