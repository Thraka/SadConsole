using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System.Threading.Tasks;

namespace StarterProject.CustomConsoles
{
    class WorldGenerationConsole : Console, IConsoleMetadata
    {
        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Random world generator", Summary = "Generates a random world, displaying it at half-font size." };
            }
        }

        private enum InitState
        {
            BeforeGeneration,
            Generating,
            AfterGeneration,
            Completed
        }

        private InitState state;

        private Console messageData;
        
        private AnimatedSurface loadingAnimation;

        private Task<BasicSurface> createRandomWorldTask;

        private SadConsole.GameHelpers.WorldGeneration.WrappingWorldGenerator<SadConsole.GameHelpers.WorldGeneration.SurfaceMap, BasicSurface> generator;

        public WorldGenerationConsole() : base(80, 23)
        {
            // Create loading animation that looks like a sequence of / - \ |
            loadingAnimation = new AnimatedSurface("default", 1, 1);
            loadingAnimation.Frames.Clear();

            var frame = loadingAnimation.CreateFrame();
            frame[0].Glyph = 92;
            frame = loadingAnimation.CreateFrame();
            frame[0].Glyph = 124;
            frame = loadingAnimation.CreateFrame();
            frame[0].Glyph = 47;
            frame = loadingAnimation.CreateFrame();
            frame[0].Glyph = 45;

            loadingAnimation.AnimationDuration = 1f;
            loadingAnimation.Repeat = true;
            
            messageData = new Console(80, 1);
            Children.Add(messageData);
            
            // Custom keyboard handler
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Keys.Left))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left - 1, cons.TextSurface.RenderArea.Top, 80 * 2, 24 * 2);

                if (info.IsKeyDown(Keys.Right))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left + 1, cons.TextSurface.RenderArea.Top, 80 * 2, 24 * 2);

                if (info.IsKeyDown(Keys.Up))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top - 1, 80 * 2, 24 * 2);

                if (info.IsKeyDown(Keys.Down))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top + 1, 80 * 2, 24 * 2);

                if (info.IsKeyReleased(Keys.Enter))
                {
                    state = InitState.BeforeGeneration;
                }

                if (info.IsKeyReleased(Keys.Space))
                {
                    var oldRenderArea = cons.TextSurface.RenderArea;
                    var oldFont = cons.TextSurface.Font;

                    if (textSurface == generator.HeightMapRenderer)
                    {
                        textSurface = generator.HeatMapRenderer;
                        SetMessage("[SPACE] Change Map Info [ENTER] New Map -- Heat");
                    }
                    else if (textSurface == generator.HeatMapRenderer)
                    {
                        textSurface = generator.MoistureMapRenderer;
                        SetMessage("[SPACE] Change Map Info [ENTER] New Map -- Moisture");
                    }
                    else if (textSurface == generator.MoistureMapRenderer)
                    {
                        textSurface = generator.BiomeMapRenderer;
                        SetMessage("[SPACE] Change Map Info [ENTER] New Map -- Biome   ");
                    }
                    else
                    {
                        textSurface = generator.HeightMapRenderer;
                        SetMessage("[SPACE] Change Map Info [ENTER] New Map -- Height  ");
                    }

                    cons.TextSurface.RenderArea = oldRenderArea;
                    cons.TextSurface.Font = oldFont;
                }

                return true;
            };

        }
        
        public override void Update(TimeSpan delta)
        {
            // If we're generating a new 
            if (state == InitState.BeforeGeneration)
            {
                // Clear out the text surface
                TextSurface = new BasicSurface(1, 1);
                state = InitState.Generating;

                SetMessage("Generating map, please wait...  ");

                // Kick off task
                loadingAnimation.Start();
                createRandomWorldTask = new Task<BasicSurface>(GenerateWorld);
                createRandomWorldTask.Start();
            }
            else if (state == InitState.Generating)
            {
                // Update the animation
                loadingAnimation.Update();

                // "Print" the animation to the message console.
                loadingAnimation.Copy(messageData.TextSurface, messageData.Width - 1, 0);

                // If task done...
                if (createRandomWorldTask.IsCompleted)
                {
                    SetMessage("[SPACE] Change Map Info [ENTER] New Map -- Biome   ");
                    this.TextSurface = createRandomWorldTask.Result;
                    state = InitState.AfterGeneration;
                }
            }
            else if (state == InitState.AfterGeneration)
            {
                loadingAnimation.Stop();
                state = InitState.Completed;
            }
            else
                base.Update(delta);
        }
        
        private void SetMessage(string text)
        {
            messageData.TextSurface = new BasicSurface(text.Length, 1) { DefaultBackground = new Color(0, 0, 0, 128) };
            messageData.Clear();
            messageData.Print(0, 0, text);

            // Center the message console
            messageData.Position = new Point(40 - text.Length / 2, 0);
        }

        private BasicSurface GenerateWorld()
        {
            generator = new SadConsole.GameHelpers.WorldGeneration.WrappingWorldGenerator<SadConsole.GameHelpers.WorldGeneration.SurfaceMap, BasicSurface>();
            generator.Start(512, 256);
            var textSurface = generator.BiomeMapRenderer;
            textSurface.Font = Global.Fonts[Global.FontDefault.Name].GetFont(SadConsole.Font.FontSizes.Half);
            return textSurface;
        }
    }
}
