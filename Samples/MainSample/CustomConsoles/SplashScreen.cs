using System;
using System.Collections.Generic;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.Effects;
using SadConsole.Instructions;
using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    internal class SplashScreen : SadConsole.Console
    {
        public Action SplashCompleted { get; set; }

        private readonly Console _consoleImage;
        private readonly Point _consoleImagePosition;
        private int _gradientPositionX = -50;

        public SplashScreen()
            : base(80, 23)
        {
            Cursor.IsEnabled = false;
            IsVisible = false;

            // Setup the console text background pattern, which is hidden via colors
            // Print the text template on all of the console surface
            const string textTemplate = "sole SadCon";
            var text = new System.Text.StringBuilder(Width * Height);

            for (int i = 0; i < Width * Height; i++)
            {
                text.Append(textTemplate);
            }
            
            this.Print(0, 0, text.ToString(), Color.Black, Color.Transparent);

            using ITexture sadImage = GameHost.Instance.GetTexture("Res/Images/sad.png");

            var defaultFontSize = SadConsole.Game.Instance.DefaultFont.GetFontSize(SadConsole.Game.Instance.DefaultFontSize);
            var defaultFontSizeRatio = SadConsole.Game.Instance.DefaultFont.GetGlyphRatio(defaultFontSize);

            // Load the logo and convert to a console
            ICellSurface logo;// = sadImage.ToSurface(TextureConvertMode.Background, sadImage.Width / (int)(defaultFontSize.X * defaultFontSizeRatio.Y), sadImage.Height / (int)(defaultFontSize.Y * defaultFontSizeRatio.X));

            if (defaultFontSizeRatio.X == 0 && defaultFontSizeRatio.Y == 0)
                logo = sadImage.ToSurface(TextureConvertMode.Foreground, Width, Height - 1, foregroundStyle: TextureConvertForegroundStyle.Block);
            else if (defaultFontSizeRatio.Y > defaultFontSizeRatio.X)
                logo = sadImage.ToSurface(TextureConvertMode.Foreground, (int)((Height - 1)* defaultFontSizeRatio.Y), Height - 1, foregroundStyle: TextureConvertForegroundStyle.Block);
            else
                logo = sadImage.ToSurface(TextureConvertMode.Foreground, Width, (int)(Width * defaultFontSize.X), foregroundStyle: TextureConvertForegroundStyle.Block);

            _consoleImage = new Console(logo);
            _consoleImage.Position =
                    _consoleImagePosition = new Point(Width / 2 - _consoleImage.Width / 2, 0);
            _consoleImage.Tint = Color.Black;

            // Animation for the logo text.
            var logoText = new ColorGradient(new[] { Color.Magenta, Color.Yellow }, new[] { 0.0f, 1f })
                               .ToColoredString("[| Powered by SadConsole |]");
            logoText.SetEffect(new Fade()
            {
                DestinationForeground = Color.Blue,
                FadeForeground = true,
                FadeDuration = 1f,
                Repeat = false,
                RemoveOnFinished = true,
                CloneOnApply = true
            });

            // Configure the animation
            InstructionSet animation = new InstructionSet()

                    .Wait(TimeSpan.FromSeconds(0.3d))

                    // Animation to move the angled gradient spotlight effect
                    .Code(MoveGradient)

                    // Clear the background text so new printing doesn't look bad
                    .Code((host, delta) =>
                        {
                            ((IScreenSurface)host).Surface.Fill(Color.Black, Color.Transparent, 0);
                            return true;
                        })

                    // Draw the SadConsole text at the bottom
                    .Instruct(new DrawString(logoText)
                    {
                        Position = new Point(26, Height - 1),
                        TotalTimeToPrint = 1f
                    })

                    // Add the logo to the console children
                    .Code((o, delta) => { Children.Add(_consoleImage); return true; })

                    // Fade in the logo
                    .Instruct(new FadeTextSurfaceTint(_consoleImage,
                                                      new ColorGradient(Color.Black, Color.Transparent),
                                                      TimeSpan.FromSeconds(2)))

                    // Blink SadConsole text at the bottom
                    .Code(SetBlinkOnLogoText)

                    // Delay so blinking effect is seen
                    .Wait(TimeSpan.FromSeconds(2.5d))

                    // Fade out main console and logo console.
                    .InstructConcurrent(new FadeTextSurfaceTint(_consoleImage,
                                                      new ColorGradient(Color.Transparent, Color.Black),
                                                      TimeSpan.FromSeconds(2)),

                                        new FadeTextSurfaceTint(this,
                                                      new ColorGradient(Color.Transparent, Color.Black),
                                                      TimeSpan.FromSeconds(1.0d)))

                    // Animation has completed, call the callback this console uses to indicate it's complete
                    .Code((host, delta) => { SplashCompleted?.Invoke(); return true; })
                ;

            animation.RemoveOnFinished = true;

            SadComponents.Add(animation);
        }

        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
                base.Update(delta);
        }


        public override void Render(TimeSpan delta)
        {
            // Draw the logo console...
            if (IsVisible)
                base.Render(delta);
        }

        private bool MoveGradient(IScreenObject console, TimeSpan delta)
        {
            _gradientPositionX += 1;

            if (_gradientPositionX > Width + 50)
            {
                return true;
            }

            Color[] colors = new[] { Color.Black, Color.Blue, Color.White, Color.Blue, Color.Black };
            float[] colorStops = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };

            Algorithms.GradientFill(FontSize, new Point(_gradientPositionX, 12), 10, 45, new Rectangle(0, 0, Width, Height), new ColorGradient(colors, colorStops), (f,b,c) => ((IScreenSurface)console).Surface.SetForeground(f,b,c));

            return false;
        }

        private bool SetBlinkOnLogoText(IScreenObject console, TimeSpan delta)
        {
            var fadeEffect = new Fade
            {
                AutoReverse = true,
                DestinationForeground = new ColorGradient(Color.Blue, Color.Yellow),
                FadeForeground = true,
                UseCellForeground = false,
                Repeat = true,
                FadeDuration = 0.7f,
                RemoveOnFinished = true
            };

            var cells = new List<ColoredGlyph>();
            for (int index = 0; index < 10; index++)
            {
                int point = new Point(26, Height - 1).ToIndex(Width) + 14 + index;
                cells.Add(this[point]);
            }

            this.SetEffect(cells, fadeEffect);
            return true;
        }
    }
}

