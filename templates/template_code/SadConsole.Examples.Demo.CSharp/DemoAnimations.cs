using SadConsole.Input;
using SadConsole.Instructions;
using SadConsole.Readers;

namespace SadConsole.Examples;

internal class DemoAnimations : IDemo
{
    public string Title => "Animations";

    public string Description => "This demo ... [c:r f:yellow]Instructions.DrawString[c:u]";

    public string CodeFile => "DemoAnimation.cs";

    public IScreenSurface CreateDemoScreen() =>
        new Animations();

    public override string ToString() =>
        Title;
}
internal class Animations : Console
{
    AnimationDemo[] _animations;

    public Animations() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        IsFocused = true;
        UseKeyboard = true;
        Game.Instance.LoadFont("Res/Fonts/thick_square_8x8.font");

        _animations = new AnimationDemo[]
        {
            new AnimatedGlobe(),
            new AnimatedFlip(),
            new AnimatedSkater(),
        };

        Restart();
    }

    public void Restart()
    {
        Children.Clear();
        Children.Add(_animations[0]);
        (Children[0] as AnimatedGlobe).Restart();
    } 

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.Right))
                return NextAnimation();
            else if (keyboard.IsKeyPressed(Keys.Left))
                return PrevAnimation();
        }
        return false;
    }

    bool NextAnimation() => ChangeAnimation(_animations.Length - 1, 1, _animations[0]);

    bool PrevAnimation() => ChangeAnimation(0, -1, _animations.Last());

    bool ChangeAnimation(int testIndex, int step, AnimationDemo overlappingAnim)
    {
        if (_animations.Length <= 1) return true;
        var currentAnimationIndex = Array.IndexOf(_animations, Children[0]);
        Children.Clear();
        if (currentAnimationIndex == testIndex) Children.Add(overlappingAnim);
        else Children.Add(_animations[currentAnimationIndex + step]);
        return true;
    }
}

class AnimationDemo : ScreenSurface
{
    string[] _info = {"Name", "Font", "Size", "Frames"};
    ColoredGlyph _appearance = new(Color.Red, Color.Transparent, 0, Mirror.None, true, new() { new CellDecorator(Color.Green, 95, Mirror.None) });

    public AnimationDemo() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Surface.DefaultBackground = Color.White;
        Surface.Clear();
    }

    public void Add(AnimatedScreenObject a, int row = -1)
    {
        float fontSizeRatioX = (float)Font.GetFontSize(IFont.Sizes.One).X / a.Font.GetFontSize(IFont.Sizes.One).X,
            fontSizeRatioY = (float)Font.GetFontSize(IFont.Sizes.One).Y / a.Font.GetFontSize(IFont.Sizes.One).Y;
        int x = Convert.ToInt32((Surface.Width * fontSizeRatioX - a.CurrentFrame.Width) / 2),
            y = Convert.ToInt32((Surface.Height * fontSizeRatioY - a.CurrentFrame.Height) / 2);
        a.Position = (x, row != -1 ? row : y);
        a.Repeat = true;
        Children.Add(a);
        a.Start();
    }

    public void Add(ScreenSurface s)
    {
        Children.Add(s);
    }

    protected void PrintInfo(string fontSize, IScreenObject s)
    {
        var board = new ScreenSurface(19, 15) { Parent = this };
        for (int i = 0, y = 1, length = _info.Length; i < length; i++, y += 2)
        {
            board.Surface.Print(1, y + i, $"{_info[i]}:", _appearance);
            board.Surface.Print(3, y + i + 1, i switch {
                0 => ((AnimatedScreenObject)s).Name,
                1 => fontSize,
                2 => GetSize((AnimatedScreenObject)s),
                _ => ((AnimatedScreenObject)s).Frames.Count.ToString()
            }, Color.Black);
        }
    }

    protected string GetSize(AnimatedScreenObject s) => $"{s.Width} x {s.Height}";
}

class AnimatedFlip : AnimationDemo
{
    public AnimatedFlip() : base()
    {
        Surface.DefaultBackground = new Color(227, 227, 227);
        Surface.Clear();
        var floor = new ScreenSurface(Surface.Width, 5)
        {
            Parent = this,
            Position = (0, Surface.Height - 5)
        };
        floor.Surface.DefaultBackground = new Color(235, 235, 235);
        floor.Surface.Clear();
        Add(AnimatedScreenObject.FromImage("Acrobatic Flip", "Res/Images/Animations/flip_anim.png", (9, 3), TimeSpan.FromMilliseconds(0.1d),
            pixelPadding: (1, 1), frameStartAndFinish: (0, 23), font: Game.Instance.Fonts["ThickSquare8"]));
        PrintInfo("Square 8 x 8", Children[1]);
    }
}

class AnimatedSkater : AnimationDemo
{
    public AnimatedSkater() : base()
    {
        Add(AnimatedScreenObject.FromImage("Clumsy Skater", "Res/Images/Animations/skater_anim.png", (6, 3), TimeSpan.FromMilliseconds(0.15d),
            pixelPadding: (1, 1), frameStartAndFinish: (0, 15), font: Game.Instance.Fonts["ThickSquare8"]));
        PrintInfo("Square 8 x 8", Children[0]);
    }
}

class AnimatedGlobe : AnimationDemo
{
    readonly TheDrawFont _drawFont;
    readonly AnimationDemo _animationScreen;
    readonly AnimatedScreenObject _clip;
    readonly ScreenSurface _titleScreen;

    public AnimatedGlobe() : base()
    {
        // draw font
        string fontFileName = "DESTRUCX.TDF";
        var fontEnumerable = TheDrawFont.ReadFonts($"./Res/TheDraw/{fontFileName}");
        if (fontEnumerable is null) throw new ArgumentException();
        _drawFont = fontEnumerable.ToArray()[0];

        // animation screen
        _animationScreen = new();

        // random noise simulating stars
        var staticNoise = AnimatedScreenObject.CreateStatic(Width, Height, 48, 0.9d);
        staticNoise.AnimationDuration = TimeSpan.FromMilliseconds(48 * 0.1d);
        _animationScreen.Children.Add(staticNoise);

        // globe animation
        _clip = AnimatedScreenObject.FromImage("Globe", "Res/Images/Animations/globe_anim.png", (48, 1), TimeSpan.FromMilliseconds(0.17d),
            action: (c) => { if (c.Foreground.GetHSLLightness() < 1f) c.Background = c.Background.FillAlpha(); });
        _animationScreen.Add(_clip);

        // title screen
        _titleScreen = new ScreenSurface(Width, Height);
        _titleScreen.Surface.DefaultBackground = Color.Black;
    }

    public void Restart()
    {
        Children.Clear();
        SadComponents.Clear();
        Start();
    }

    void Start()
    {
        _clip.Stop();
        Children.Add(_animationScreen);
        Children.Add(_titleScreen);

        // reset title screen
        byte opacity = 255;
        _titleScreen.Surface.Clear();  /* to make the whole surface opaque */
        _titleScreen.Surface.PrintTheDraw(3, "globe", _drawFont, HorizontalAlignment.Center);
        _titleScreen.Surface.Print(0, 20, "Use arrow keys Left and Right to switch animations".Align(HorizontalAlignment.Center, _titleScreen.Surface.Width));
        ((SadConsole.Renderers.ScreenSurfaceRenderer)_titleScreen.Renderer).Opacity = opacity;
        _titleScreen.Tint = Color.Black;

        // instructions

        var animationInstructions = new InstructionSet() { RemoveOnFinished = true }
            .Instruct(new FadeTextSurfaceTint(_titleScreen, new Gradient(Color.Black, Color.Transparent), TimeSpan.FromSeconds(1)))
            .Code(() =>
            {
                Children.Add(new SadConsole.SplashScreens.Simple());
            })
            .Wait(TimeSpan.FromSeconds(4))
            .Instruct(new FadeTextSurfaceTint(_titleScreen, new Gradient(Color.Transparent, Color.Black), TimeSpan.FromSeconds(1)))
            .Code(() =>
            {
                _clip.Start();
                _titleScreen.Surface.Clear();
                _titleScreen.Tint = Color.Transparent;
            })
            .Code((s, d) =>
            {
                ((SadConsole.Renderers.ScreenSurfaceRenderer)_titleScreen.Renderer).Opacity = opacity--;
                return opacity == 0 ? true : false;
            });
        SadComponents.Add(animationInstructions);
    }
}
