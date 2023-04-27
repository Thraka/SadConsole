#if MONOGAME
using Microsoft.Xna.Framework.Graphics;
using SadConsole.DrawCalls;
using SadConsole.Renderers;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace SadConsole.Examples;

internal class DemoRotatedSurface : IDemo
{
    public string Title => "Rotated Surface";

    public string Description => "This demo creates a custom rendering step and draw call which can rotate consoles.\r\n\r\n" +
                                 "As SadConsole renders objects, it builds a list of 'DrawCalls' that compose the console or surface. Such as, [c:r f:violet:3](1) clear the output texture [c:r f:violet:3](2) draw all the glyphs [c:r f:violet:3](3) apply tinting [c:r f:violet:3](4) render to the screen.\r\n";

    public string CodeFile => "DemoShapes.MonoGame.cs";

    public IScreenSurface CreateDemoScreen() =>
        new RotatedSurface();

    public override string ToString() =>
        Title;

    void IDemo.PostCreateDemoScreen(SadConsole.IScreenSurface demoScreen)
    {
        demoScreen.Children.Clear();
        demoScreen.Position += (-5, 1);
    }
}


class RotatedSurface : ScreenSurface
{
    public RotatedSurface() : base(GameSettings.ScreenDemoBounds.Height, GameSettings.ScreenDemoBounds.Height / 2)
    {
        UseKeyboard = false;
        UseMouse = true;

        FontSize = Font.GetFontSize(IFont.Sizes.Two);

        Surface.FillWithRandomGarbage(Font);
        for (int i = 0; i < Renderer!.Steps.Count; i++)
        {
            if (Renderer!.Steps[i] is OutputSurfaceRenderStep)
            {
                Renderer!.Steps.RemoveAt(i);
                break;
            }
        }

        Renderer!.Steps.Add(new RotatedOutputSurfaceRenderStep() { Rotation = 45d });
    }
}

class RotatedOutputSurfaceRenderStep: IRenderStep
{
    public string Name => SadConsole.Renderers.Constants.RenderStepNames.Output;
    public uint SortOrder { get; set; } = SadConsole.Renderers.Constants.RenderStepSortValues.Output;

    public double Rotation { get; set; }

    public void SetData(object data) { }

    public void Reset() { }

    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
        false;

    public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        var monoRenderer = (ScreenSurfaceRenderer)renderer;

        if (screenObject.Tint.A != 255)
            GameHost.Instance.DrawCalls.Enqueue(new DrawCallTextureRotation(monoRenderer._backingTexture,
                                                                            new Vector2(screenObject.AbsoluteArea.Position.X, screenObject.AbsoluteArea.Position.Y),
                                                                            (float)SadRogue.Primitives.MathHelpers.ToRadian(Rotation),
                                                                            monoRenderer._finalDrawColor));
    }

    public void Dispose() =>
        Reset();

    class DrawCallTextureRotation : IDrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;
        public XnaColor Tint;
        public float Rotation;

        public DrawCallTextureRotation(Texture2D texture, Vector2 position, float rotation, XnaColor? tint = null)
        {
            if (texture == null) throw new System.NullReferenceException($"{nameof(texture)} cannot be null.");

            Texture = texture;
            Position = position;
            Rotation = rotation;

            if (tint.HasValue)
                Tint = tint.Value;
            else
                Tint = XnaColor.White;

        }

        public void Draw() =>
            Host.Global.SharedSpriteBatch.Draw(Texture,
                                               Position + new Vector2(Texture.Width / 2, Texture.Height / 2),
                                               null,
                                               Tint,
                                               Rotation,
                                               new Vector2(Texture.Width / 2, Texture.Height / 2),
                                               Vector2.One,
                                               SpriteEffects.None,
                                               0f);
    }
}
#endif
