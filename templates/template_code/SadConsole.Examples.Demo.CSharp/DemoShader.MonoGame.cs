#if MONOGAME && !FNA
using Microsoft.Xna.Framework.Graphics;
using SadConsole.DrawCalls;
using SadConsole.Renderers;
using SadConsole.UI;
using SadConsole.UI.Controls;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace SadConsole.Examples;

internal class DemoShader : IDemo
{
    public string Title => "Using Shaders";

    public string Description => "This demo provides the option to apply a shader to a specific surface or all " +
                                 "of SadConsole.\r\n";

    public string CodeFile => "DemoShader.MonoGame.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ShaderController();

    public override string ToString() =>
        Title;
}

internal class ShaderController : ControlsConsole
{
    private static bool _isGlobal = false;
    private static bool _isLoaded = false;
    private static SadConsole.Host.PostProcessingFX _monoGameComponent;
    private readonly ScreenSurface _otherSurface;
    private static Effect Effect;

    public ShaderController() : base(28, 4)
    {
        // Create the other console where the keyboard handler will be set
        _otherSurface = new ScreenSurface(GameSettings.SCREEN_DEMO_WIDTH - 8, GameSettings.SCREEN_DEMO_HEIGHT - this.Height - 3)
        {
            Position = (8, this.Height + 3),
        };
        _otherSurface.FillWithRandomGarbage(_otherSurface.Font);
        Border.CreateForSurface(_otherSurface, "");

        Children.Add(_otherSurface);

        // Load the shader once
        //if (Effect == null)
        {

        }

        // Create the controls
        RadioButton radSurfaceOnly = new("Apply to surface");
        radSurfaceOnly.Position = (1, 1);
        radSurfaceOnly.IsSelectedChanged += Surface_IsSelectedChanged;

        RadioButton radGlobal = new("Apply globally");
        radGlobal.Position = (1, 2);
        radGlobal.IsSelectedChanged += Global_IsSelectedChanged;

        if (_isGlobal)
            radGlobal.IsSelected = true;

        Controls.Add(radSurfaceOnly);
        Controls.Add(radGlobal);
    }

    private void Surface_IsSelectedChanged(object? sender, EventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender!;

        // Surface-only on
        if (radioButton.IsSelected)
        {
            OutputSurfaceRenderStep step = _otherSurface.Renderer!.Steps.OfType<OutputSurfaceRenderStep>().First();

            Effect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("crt-lottes-mg");
            Effect.Parameters["hardScan"]?.SetValue(-8.0f);
            Effect.Parameters["hardPix"]?.SetValue(-3.0f);
            Effect.Parameters["warpX"]?.SetValue(.031f);
            Effect.Parameters["warpY"]?.SetValue(0.041f);
            Effect.Parameters["maskDark"]?.SetValue(0.5f);
            Effect.Parameters["maskLight"]?.SetValue(1.5f);
            Effect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
            Effect.Parameters["shadowMask"]?.SetValue(3.0f);
            Effect.Parameters["brightboost"]?.SetValue(1.0f);
            Effect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
            Effect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
            Effect.Parameters["bloomAmount"]?.SetValue(2f);
            Effect.Parameters["shape"]?.SetValue(5.0f);
            Effect.Parameters["brightboost"].SetValue(0.5f);
            Effect.Parameters["textureSize"].SetValue(new Vector2(_otherSurface.AbsoluteArea.Width, _otherSurface.AbsoluteArea.Height));
            Effect.Parameters["outputSize"].SetValue(new Vector2(_otherSurface.AbsoluteArea.Width, _otherSurface.AbsoluteArea.Height));
            Effect.Parameters["videoSize"].SetValue(new Vector2(_otherSurface.AbsoluteArea.Width, _otherSurface.AbsoluteArea.Height));

            step.ShaderEffect = Effect;
        }

        // Surface-only off
        else
        {
            OutputSurfaceRenderStep step = _otherSurface.Renderer!.Steps.OfType<OutputSurfaceRenderStep>().First();
            step.ShaderEffect = null;
        }
    }

    private void Global_IsSelectedChanged(object? sender, EventArgs e)
    {
        RadioButton radioButton = (RadioButton)sender!;

        // Global
        if (radioButton.IsSelected)
        {
            _isGlobal = true;

            if (_monoGameComponent == null)
            {
                // Create the component
                _monoGameComponent = new();
                SadConsole.Game.Instance.MonoGameInstance.Components.Add(_monoGameComponent);
            }

            _monoGameComponent.Visible = true;
            SadConsole.Settings.DoFinalDraw = false;
        }
        // Surface-only
        else
        {
            _isGlobal = false;

            // Turn off the global one if it's there
            if (_monoGameComponent != null)
            {
                _monoGameComponent.Visible = false;
                SadConsole.Settings.DoFinalDraw = true;
            }
        }
    }
}

#endif
