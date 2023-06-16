using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoSurfaceOpacity : IDemo
{
    public string Title => "Fading Surface";

    public string Description => "This demo's code demonstrates a few different concepts:\r\n" +
                                 "[c:r f:violet]*[c:u] Buttons use a different color palette.\r\n" +
                                 "[c:r f:violet]*[c:u] Both the buttons and the scrollbar control\r\n" +
                                 "customize the theme object.\r\n" +
                                 "[c:r f:violet]*[c:u] The surface renderer from the host library can have the surfaces [c:r f:yellow]Opacity[c:u] changed.\r\n" +
                                 "[c:r f:violet]*[c:u] An [c:r f:yellow]Instructions.AnimatedValue[c:u] animates the opacity of the renderer.";

    public string CodeFile => "DemoSurfaceOpacity.cs";

    public IScreenSurface CreateDemoScreen() =>
        new SurfaceOpacity();

    public override string ToString() =>
        Title;
}


internal class SurfaceOpacity : ControlsConsole
{
    private readonly ScreenSurface _otherParentSurface;
    private readonly ScreenSurface _fadingSurface;
    private SadConsole.Instructions.AnimatedValue _animatedOpacity;
    private readonly ScrollBar _opacitySlider;

    public SurfaceOpacity() : base(28, 4)
    {
        // Create the other console where the keyboard handler will be set
        _otherParentSurface = new ScreenSurface(GameSettings.SCREEN_DEMO_WIDTH - 8, GameSettings.SCREEN_DEMO_HEIGHT - this.Height - 3)
        {
            Position = (8, this.Height + 3),
        };
        _otherParentSurface.FillWithRandomGarbage(_otherParentSurface.Font);
        Border.CreateForSurface(_otherParentSurface, "");

        Children.Add(_otherParentSurface);

        // Create the fading surface
        _fadingSurface = new ScreenSurface(30, 13);
        _fadingSurface.Position = (_otherParentSurface.ViewWidth / 2- _fadingSurface.ViewWidth / 2, _otherParentSurface.ViewHeight / 2 - _fadingSurface.ViewHeight / 2);
        _fadingSurface.Parent = _otherParentSurface;

        Color[] colorsFadingSurface = new[] { Color.LightGreen, Color.Coral, Color.CornflowerBlue, Color.DarkGreen };
        float[] colorStopsFadingSurface = new[] { 0f, 0.35f, 0.75f, 1f };

        Algorithms.GradientFill(_fadingSurface.FontSize,
                                _fadingSurface.Surface.Area.Center,
                                _fadingSurface.Width / 3,
                                45,
                                _fadingSurface.Surface.Area,
                                new Gradient(colorsFadingSurface, colorStopsFadingSurface),
                                (x, y, color) => _fadingSurface.Surface[x, y].Background = color);

        _fadingSurface.DrawBox(_fadingSurface.Surface.Area, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.White, Color.Black)));

        // Print the 0 .. 255
        Surface.Print(2, Height - 1, "0");
        Surface.Print(Width - 5, Height - 1, "255");

        Colors buttonColors = Colors.CreateSadConsoleBlue();

        // Create the controls
        Button buttonIn = new Button(11);
        buttonIn.Text = "Fade In";
        buttonIn.ShowEnds = false;
        buttonIn.Position = (Width - 13, 0);
        buttonIn.SetThemeColors(buttonColors);
        buttonIn.Click += ButtonIn_Click;
        Controls.Add(buttonIn);

        Button buttonOut = new Button(12);
        buttonOut.Text = "Fade Out";
        buttonOut.ShowEnds = false;
        buttonOut.Position = (1, 0);
        buttonOut.SetThemeColors(buttonColors);
        buttonOut.Click += ButtonOut_Click;
        Controls.Add(buttonOut);

        _opacitySlider = new ScrollBar(Orientation.Horizontal, Width - 4);
        _opacitySlider.Position = (2, Height - 2);
        _opacitySlider.BarGlyph = ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.Top];
        _opacitySlider.StartButtonHorizontalGlyph = '<';
        _opacitySlider.EndButtonHorizontalGlyph = '>';
        _opacitySlider.Maximum = 255;
        _opacitySlider.Value = 255;
        _opacitySlider.ValueChanged += OpacitySlider_ValueChanged;
        Controls.Add(_opacitySlider);
    }

    private void ButtonOut_Click(object? sender, EventArgs e)
    {
        if (_animatedOpacity != null)
        {
            if (SadComponents.Contains(_animatedOpacity))
                SadComponents.Remove(_animatedOpacity);
        }

        _animatedOpacity = new Instructions.AnimatedValue(TimeSpan.FromSeconds(1.5), 255, 0, new EasingFunctions.Linear());
        _animatedOpacity.RemoveOnFinished = true;
        _animatedOpacity.ValueChanged += AnimatedOpacity_ValueChanged;
        SadComponents.Add(_animatedOpacity);
    }

    private void ButtonIn_Click(object? sender, EventArgs e)
    {
        if (_animatedOpacity != null)
        {
            if (SadComponents.Contains(_animatedOpacity))
                SadComponents.Remove(_animatedOpacity);
        }

        _animatedOpacity = new Instructions.AnimatedValue(TimeSpan.FromSeconds(1.5), 0, 255, new EasingFunctions.Linear());
        _animatedOpacity.RemoveOnFinished = true;
        _animatedOpacity.ValueChanged += AnimatedOpacity_ValueChanged;
        SadComponents.Add(_animatedOpacity);
    }

    private void AnimatedOpacity_ValueChanged(object? sender, double e) =>
        _opacitySlider.Value = (int)e;

    private void OpacitySlider_ValueChanged(object? sender, EventArgs e) =>
        ((Renderers.ScreenSurfaceRenderer)_fadingSurface.Renderer!).Opacity = (byte)_opacitySlider.Value;
}
