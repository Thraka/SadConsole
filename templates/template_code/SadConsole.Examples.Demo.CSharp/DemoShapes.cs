using SadConsole.Input;
using SadConsole.StringParser;

namespace SadConsole.Examples;

internal class DemoShapes : IDemo
{
    public string Title => "Shapes & Mouse Cursor";

    public string Description => "Examples of drawing shapes and displaying a mouse cursor\r\n\r\n" +
                                 "SadConsole has built in shape drawing for [c:r f:AnsiGreen]Circles[c:u], [c:r f:AnsiGreen]Boxes[c:u], and [c:r f:AnsiGreen]Lines[c:u].\r\n\r\n" +
                                 "The block mouse cursor is simply a 1x1 child surface that follows the mouse.";

    public string CodeFile => "DemoShapes.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ShapesSurface();

    public override string ToString() =>
        Title;
}

class ShapesSurface : ScreenSurface
{
    private readonly ScreenSurface _mouseCursor;

    public ShapesSurface() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        UseKeyboard = false;
        UseMouse = true;

        Surface.DrawLine(new Point(2, 2), new Point(Surface.ViewWidth - 4, 2), (int)'=', Color.Yellow);
        Surface.DrawBox(new Rectangle(2, 4, 6, 6), ShapeParameters.CreateBorder(new ColoredGlyph(Color.Yellow, Color.Black, '=')));
        Surface.DrawBox(new Rectangle(9, 4, 6, 6), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black, '=')));
        Surface.DrawBox(new Rectangle(16, 4, 6, 6), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, new ColoredGlyph(Color.Yellow, Color.Black, '=')));
        Surface.DrawBox(new Rectangle(23, 4, 6, 6), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick, new ColoredGlyph(Color.Black, Color.Yellow, '='), new ColoredGlyph(Color.Black, Color.Yellow, 0)));

        Surface.DrawCircle(new Rectangle(2, 12, 16, 10), ShapeParameters.CreateBorder(new ColoredGlyph(Color.White, Color.Black, 176)));
        Surface.DrawCircle(new Rectangle(19, 12, 16, 10), ShapeParameters.CreateFilled(new ColoredGlyph(Color.White, Color.Black, 176), new ColoredGlyph(Color.Green, Color.Black, 178)));

        Surface.DrawLine(new Point(55, 4), new Point(38, 20), 176);
        Surface.DrawLine(new Point(55, 4), new Point(45, 22), 176);
        Surface.DrawLine(new Point(55, 4), new Point(55, 22), 176);
        Surface.DrawLine(new Point(55, 4), new Point(64, 22), 176);
        Surface.DrawLine(new Point(55, 4), new Point(76, 20), 176);

        _mouseCursor = new SadConsole.ScreenSurface(1, 1);
        _mouseCursor.Surface.SetGlyph(0, 0, 178);
        _mouseCursor.UseMouse = false;

        Children.Add(_mouseCursor);
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        _mouseCursor.IsVisible = state.IsOnScreenObject;
        _mouseCursor.Position = state.CellPosition;

        return base.ProcessMouse(state);
    }
}
