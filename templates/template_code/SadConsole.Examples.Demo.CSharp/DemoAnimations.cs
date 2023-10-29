using SadConsole.Input;

namespace SadConsole.Examples;

internal class DemoAnimations : IDemo
{
    public string Title => "Animations";

    public string Description => "This demo shows how to read a PNG sprite sheet and load each sprite frame into an animated object.\r\n\r\nPress the [c:r f:Red:10]Left Arrow or [c:r f:Red:11]Right Arrow keys to change the active animation.";

    public string CodeFile => "DemoAnimation.cs";

    public IScreenSurface CreateDemoScreen() =>
        new Animations();

    public override string ToString() =>
        Title;
}

internal class Animations : ScreenSurface
{
    private readonly AnimationSlide[] _animations;

    public Animations() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        IsFocused = true;
        UseKeyboard = true;
        Game.Instance.LoadFont("Res/Fonts/thick_square_8x8.font");

        _animations = new AnimationSlide[]
        {
            new AnimatedGlobe(),
            new AnimatedFlip(),
            new AnimatedSkater(),
        };

        Children.Add(_animations[0]);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.Right))
            {
                NextAnimation();
                return true;
            }

            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                PrevAnimation();
                return true;
            }
        }

        return false;
    }

    private void NextAnimation() =>
        ChangeAnimation(_animations.Length - 1, 1, _animations[0]);

    private void PrevAnimation() =>
        ChangeAnimation(0, -1, _animations.Last());

    private void ChangeAnimation(int testIndex, int step, AnimationSlide overlappingAnim)
    {
        int currentAnimationIndex = Array.IndexOf(_animations, Children[0]);

        if (currentAnimationIndex == testIndex) Children[0] = overlappingAnim;
        else Children[0] = _animations[currentAnimationIndex + step];
    }
}

class AnimationSlide : ScreenSurface
{
    private string[] _info = {"Name", "Font", "Size", "Frames"};
    private ColoredGlyph _appearance = new(Color.Red, Color.White, 0, Mirror.None, true, new() { new CellDecorator(Color.Green, 95, Mirror.None) });

    public AnimationSlide() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Surface.DefaultBackground = Color.White;
        Surface.Clear();
    }

    public void Add(AnimatedScreenObject animation, int row = -1)
    {
        float fontSizeRatioX = (float)Font.GetFontSize(IFont.Sizes.One).X / animation.Font.GetFontSize(IFont.Sizes.One).X;
        float fontSizeRatioY = (float)Font.GetFontSize(IFont.Sizes.One).Y / animation.Font.GetFontSize(IFont.Sizes.One).Y;

        int x = Convert.ToInt32((Surface.Width * fontSizeRatioX - animation.CurrentFrame.Width) / 2);
        int y = Convert.ToInt32((Surface.Height * fontSizeRatioY - animation.CurrentFrame.Height) / 2);

        animation.Position = (x, row != -1 ? row : y);
        animation.Repeat = true;
        animation.Start();

        Children.Add(animation);
    }

    public void Add(ScreenSurface surface) =>
        Children.Add(surface);

    protected void PrintInfo(string fontSize, IScreenObject screen)
    {
        var board = new ScreenSurface(19, 15) { Parent = this };

        for (int i = 0, y = 1, length = _info.Length; i < length; i++, y += 2)
        {
            board.Surface.Print(1, y + i, $"{_info[i]}:", _appearance);
            board.Surface.Print(3, y + i + 1, i switch {
                0 => ((AnimatedScreenObject)screen).Name,
                1 => fontSize,
                2 => GetSize((AnimatedScreenObject)screen),
                _ => ((AnimatedScreenObject)screen).Frames.Count.ToString()
            }, Color.Black, Color.White);
        }
    }

    private static string GetSize(AnimatedScreenObject screenObject) =>
        $"{screenObject.Width} x {screenObject.Height}";
}

class AnimatedFlip : AnimationSlide
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

        Add(AnimatedScreenObject.FromImage("Acrobatic Flip", "Res/Images/Animations/flip_anim.png", (9, 3), TimeSpan.FromSeconds(0.1d),
            pixelPadding: (1, 1), frameStartAndFinish: (0, 23), font: Game.Instance.Fonts["ThickSquare8"]));

        PrintInfo("Square 8 x 8", Children[1]);
    }
}

class AnimatedSkater : AnimationSlide
{
    public AnimatedSkater() : base()
    {
        Add(AnimatedScreenObject.FromImage("Clumsy Skater", "Res/Images/Animations/skater_anim.png", (6, 3), TimeSpan.FromSeconds(0.15d),
            pixelPadding: (1, 1), frameStartAndFinish: (0, 15), font: Game.Instance.Fonts["ThickSquare8"]));

        PrintInfo("Square 8 x 8", Children[0]);
    }
}

/// <summary>
/// Animated globe that has a fading introduction
/// </summary>
class AnimatedGlobe : AnimationSlide
{
    readonly AnimatedScreenObject _clip;

    public AnimatedGlobe() : base()
    {
        // random noise simulating stars
        AnimatedScreenObject staticNoise = AnimatedScreenObject.CreateStatic(Width, Height, 48, 0.9d);
        staticNoise.AnimationDuration = TimeSpan.FromSeconds(48 * 0.1d);

        // globe animation
        _clip = AnimatedScreenObject.FromImage("Globe", "Res/Images/Animations/globe_anim.png", (48, 1), TimeSpan.FromSeconds(0.17d),
                    action: (c) => { if (c.Foreground.GetHSLLightness() < 1f) c.Background = c.Background.FillAlpha(); });

        _clip.Position = ((Width / 2) - (_clip.Width / 2), (Height / 2) - (_clip.Height / 2));
        _clip.Repeat = true;

        staticNoise.Start();
        _clip.Start();

        Children.Add(staticNoise);
        Children.Add(_clip);
        
        PrintInfo("IBM 8x16", Children[0]);
    }
}
