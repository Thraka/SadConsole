using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using SadConsole;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System.Collections.Generic;
using SadConsole.Instructions;
using SadConsole.Effects;

namespace StarterProject.CustomConsoles
{
    class SplashScreen : Console, IConsoleMetadata
    {
        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Text Mouse Cursor", Summary = "Draws a game object where ever the mouse cursor is." };
            }
        }

        public Action SplashCompleted { get; set; }

        private InstructionSet _animation;
        private BasicSurface _consoleImage;
        private Point _consoleImagePosition;
        private EffectsManager effectsManager;
        int _x = -50;

        List<int> cellindexes = new List<int>();

        public SplashScreen()
            : base(80, 23)
        {
            IsVisible = false;
            effectsManager = new EffectsManager(this.TextSurface);
            // Setup the console text background
            string textTemplate = "sole SadCon";
            System.Text.StringBuilder text = new System.Text.StringBuilder(2200);

            for (int i = 0; i < TextSurface.Width * TextSurface.Height; i++)
            {
                text.Append(textTemplate);
            }
            Print(0, 0, text.ToString(), Color.Black, Color.Transparent);

            // Load the logo
            System.IO.Stream imageStream = Microsoft.Xna.Framework.TitleContainer.OpenStream("sad.png");
            var image = Texture2D.FromStream(Global.GraphicsDevice, imageStream);
            imageStream.Dispose();

            // Configure the logo
            _consoleImage = image.ToSurface(Global.FontDefault, false);
            _consoleImagePosition = new Point(TextSurface.Width / 2 - _consoleImage.Width / 2, -1);
            _consoleImage.Tint = Color.Black;

            // Configure the animations
            _animation = new InstructionSet();
            _animation.Instructions.AddLast(new Wait() { Duration = 0.3f });

            // Animation to move the angled gradient spotlight effect.
            var moveGradientInstruction = new CodeInstruction();
            moveGradientInstruction.CodeCallback = (inst) =>
            {
                _x += 1;

                if (_x > TextSurface.Width + 50)
                {
                    inst.IsFinished = true;
                }

                Color[] colors = new Color[] { Color.Black, Color.Blue, Color.White, Color.Blue, Color.Black };
                float[] colorStops = new float[] { 0f, 0.2f, 0.5f, 0.8f, 1f };

                Algorithms.GradientFill(TextSurface.Font.Size, new Point(_x, 12), 10, 45, new Rectangle(0, 0, TextSurface.Width, TextSurface.Height), new ColorGradient(colors, colorStops), SetForeground);
            };
            _animation.Instructions.AddLast(moveGradientInstruction);

            // Animation to clear the SadConsole text.
            _animation.Instructions.AddLast(new CodeInstruction() { CodeCallback = (i) => { Fill(Color.Black, Color.Transparent, 0, null); i.IsFinished = true; } });

            // Animation for the logo text.
            var logoText = new ColorGradient(new Color[] { Color.Magenta, Color.Yellow }, new float[] { 0.0f, 1f }).ToColoredString("[| Powered by SadConsole |]");
            logoText.SetEffect(new SadConsole.Effects.Fade() { DestinationForeground = Color.Blue, FadeForeground = true, FadeDuration = 1f, Repeat = false, RemoveOnFinished = true, Permanent = true, CloneOnApply = true });
            _animation.Instructions.AddLast(new DrawString(this) { Position = new Point(26, this.TextSurface.Height - 1), Text = logoText, TotalTimeToPrint = 1f });

            // Animation for fading in the logo picture.
            _animation.Instructions.AddLast(new FadeTextSurfaceTint(_consoleImage, new ColorGradient(Color.Black, Color.Transparent), new TimeSpan(0, 0, 0, 0, 2000)));

            // Animation to blink SadConsole in the logo text
            _animation.Instructions.AddLast(new CodeInstruction()
            {
                CodeCallback = (i) =>
                {
                    SadConsole.Effects.Fade fadeEffect = new SadConsole.Effects.Fade();
                    fadeEffect.AutoReverse = true;
                    fadeEffect.DestinationForeground = new ColorGradient(Color.Blue, Color.Yellow);
                    fadeEffect.FadeForeground = true;
                    fadeEffect.UseCellForeground = false;
                    fadeEffect.Repeat = true;
                    fadeEffect.FadeDuration = 0.7f;
                    fadeEffect.RemoveOnFinished = true;

                    List<Cell> cells = new List<Cell>();
                    for (int index = 0; index < 10; index++)
                    {
                        var point = new Point(26, this.TextSurface.Height - 1).ToIndex(this.TextSurface.Width) + 14 + index;
                        cells.Add(textSurface.Cells[point]);
                        cellindexes.Add(point);
                    }

                    effectsManager.SetEffect(cells, fadeEffect);
                    i.IsFinished = true;
                }
            });

            // Animation to delay, keeping the logo and all on there for 2 seconds, then destroy itself.
            _animation.Instructions.AddLast(new Wait() { Duration = 2.5f });
            _animation.Instructions.AddLast(new FadeTextSurfaceTint(_consoleImage, new ColorGradient(Color.Transparent, Color.Black), new TimeSpan(0, 0, 0, 0, 2000)));
            _animation.Instructions.AddLast(new CodeInstruction()
            {
                CodeCallback = (i) =>
                {
                    SplashCompleted?.Invoke();

                    i.IsFinished = true;
                }
            });
        }

        public override void Update(TimeSpan delta)
        {
            if (!IsVisible)
                return;

            base.Update(delta);
            effectsManager.UpdateEffects(Global.GameTimeElapsedUpdate);

            _animation.Run();
        }

        public override void Draw(TimeSpan delta)
        {
            // Draw the logo console...
            if (IsVisible)
            {
                Renderer.Render(_consoleImage);
                Global.DrawCalls.Add(new DrawCallSurface(_consoleImage, _consoleImagePosition, false));

                base.Draw(delta);
            }
        }
    }
}

