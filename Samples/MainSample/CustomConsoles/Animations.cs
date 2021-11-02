using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Input;
using SadConsole.Instructions;
using SadConsole.Readers;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class Animations : SadConsole.Console
    {
        public const int W = 80, H = 23;
        AnimationDemo[] _animations;

        public Animations() : base(W, H)
        {
            IsFocused = true;
            UseKeyboard = true;

            _animations = new AnimationDemo[]
            {
                new AnimatedGlobe(),
                new AnimatedBoy(),
                new AnimatedSanta()
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
                if (keyboard.IsKeyPressed(Keys.Right)) return NextAnimation();
                else if (keyboard.IsKeyPressed(Keys.Left)) return PrevAnimation();
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
        public AnimationDemo() : base(Animations.W, Animations.H) { }

        public void Add(AnimatedScreenSurface a, int row = -1)
        {
            int x = (Surface.Width - a.Surface.Width) / 2,
                y = (Surface.Height - a.Surface.Height) / 2;
            a.Position = (x, row != -1 ? row : y);
            a.Repeat = true;
            Children.Add(a);
            a.Start();
        }
    }

    class AnimatedBoy : AnimationDemo
    {
        public AnimatedBoy() : base()
        {
            var boyAnim = AnimatedScreenSurface.ConvertImageFile("Res/Images/boy_anim.jpg", (7, 2), (6, 31), 0.2f, Game.Instance.DefaultFont);
            Add(boyAnim);
        }
    }

    class AnimatedSanta : AnimationDemo
    {
        public AnimatedSanta() : base()
        {
            var santaAnim = AnimatedScreenSurface.ConvertImageFile("Res/Images/santa_anim.jpg", (6, 2), (3, 5), 0.2f, Game.Instance.DefaultFont);
            Add(santaAnim);
        }
    }

    class AnimatedGlobe : AnimationDemo
    {
        readonly TheDrawFont _drawFont;
        readonly AnimationDemo _animationScreen;
        readonly AnimatedScreenSurface _clip;
        readonly ScreenSurface _titleScreen;

        public AnimatedGlobe() : base()
        {
            // draw font
            string fontFileName = "DESTRUCX.TDF";
            var fontEnumerable = TheDrawFont.ReadFonts($"./TheDraw/{fontFileName}");
            if (fontEnumerable is null) throw new ArgumentException();
            _drawFont = fontEnumerable.ToArray()[0];

            // animation screen
            _animationScreen = new();

            // random noise simulating stars
            var staticNoise = AnimatedScreenSurface.CreateStatic(Animations.W, Animations.H, 48, 0.9d);
            staticNoise.AnimationDuration = 48 * 0.1f;
            _animationScreen.Children.Add(staticNoise);

            // globe animation
            Action<ColoredGlyph> callback = (c) => { if (c.Foreground.GetBrightness() < 1) c.Background = c.Background.FillAlpha(); };
            _clip = AnimatedScreenSurface.ConvertImageFile("Res/Images/globe_anim.png", (48, 1), (0, 0), 0.17f, Game.Instance.DefaultFont, callback);
            _animationScreen.Add(_clip);

            // title screen
            _titleScreen = new ScreenSurface(Animations.W, Animations.H);
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
                .Instruct(new FadeTextSurfaceTint(_titleScreen, new ColorGradient(Color.Black, Color.Transparent), TimeSpan.FromSeconds(1)))
                .Code(() =>
                {
                    Children.Add(new SadConsole.SplashScreens.Simple());
                })
                .Wait(TimeSpan.FromSeconds(4))
                .Instruct(new FadeTextSurfaceTint(_titleScreen, new ColorGradient(Color.Transparent, Color.Black), TimeSpan.FromSeconds(1)))
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
}
