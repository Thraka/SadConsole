using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoSecondSurfaceRenderer : IDemo
{
    public string Title => "Renderer - Second surface";

    public string Description => "";

    public string CodeFile => "DemoSecondSurfaceRenderer.cs";

    public IScreenSurface CreateDemoScreen() =>
        new SurfaceSecondRenderer();

    public override string ToString() =>
        Title;
}

internal class SurfaceSecondRenderer : ControlsConsole
{
    private readonly ScreenSurface _otherSurface;
    private readonly ScreenSurface _secondSurface;
    private readonly Renderers.SurfaceRenderStep _extraSurfaceStep;

    public SurfaceSecondRenderer() : base(28, 4)
    {
        // Create the other console where the keyboard handler will be set
        _otherSurface = new ScreenSurface(GameSettings.SCREEN_DEMO_WIDTH - 8, GameSettings.SCREEN_DEMO_HEIGHT - this.Height - 3)
        {
            Position = (8, this.Height + 3),
        };
        _otherSurface.FillWithRandomGarbage(_otherSurface.Font);
        Border.CreateForSurface(_otherSurface, "");

        Children.Add(_otherSurface);

        // Create a 2nd surface
        _secondSurface = new ScreenSurface(_otherSurface.Width, _otherSurface.Height);
        _secondSurface.DrawBox((1, 1, 17, 3), ShapeParameters.CreateFilled(new ColoredGlyph(Color.Black, Color.Lavender), new ColoredGlyph(Color.Black, Color.Lavender)));
        _secondSurface.Print(2, 2, "Second Surface");
        _secondSurface.Position = (10, 4);

        // Create the controls
        CheckBox chkEnableSurface = new("Show Surface");
        chkEnableSurface.Position = (1, 1);
        //chkEnableSurface.SetThemeColors(buttonColors);
        chkEnableSurface.IsSelectedChanged += ChkEnableSurface_IsSelectedChanged;
        Controls.Add(chkEnableSurface);

        // Create the render step
        _extraSurfaceStep = new Renderers.SurfaceRenderStep();
        _extraSurfaceStep.SetData(_secondSurface);
        _extraSurfaceStep.SortOrder += 1;
    }

    private void ChkEnableSurface_IsSelectedChanged(object? sender, EventArgs e)
    {
        if (((CheckBox)sender!).IsSelected)
            _otherSurface.Renderer!.Steps.Add(_extraSurfaceStep);
        else
            _otherSurface.Renderer!.Steps.Remove(_extraSurfaceStep);

        _otherSurface.Renderer.Steps.Sort(Renderers.RenderStepComparer.Instance);
        _otherSurface.IsDirty = true;
    }
}
