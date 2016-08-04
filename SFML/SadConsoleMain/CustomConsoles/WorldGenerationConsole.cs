#if SFML
using Keys = SFML.Window.Keyboard.Key;
using Rectangle = SFML.Graphics.IntRect;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using SFML.Graphics;

namespace StarterProject.CustomConsoles
{
    class WorldGenerationConsole : Console
    {
        //private CellSurface mainData;
        private Console messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        // Zoom calculation for the width/height of a cell. Zooms out to make it look like square pixels.
        //private Point ZoomLevel = new Point(2, 4);

        private SadConsole.Game.WorldGeneration.WrappingWorldGenerator<SadConsole.Game.WorldGeneration.TextSurfaceMap, TextSurface> generator;

        public WorldGenerationConsole() : base(80, 25)
        {
            messageData = new Console(1, 1);
            messageData.TextSurface = textSurface;
            IsVisible = false;
            
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Keys.Left))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left - 1, cons.TextSurface.RenderArea.Top, 80 * 2, 25 * 2);

                if (info.IsKeyDown(Keys.Right))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left + 1, cons.TextSurface.RenderArea.Top, 80 * 2, 25 * 2);

                if (info.IsKeyDown(Keys.Up))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top - 1, 80 * 2, 25 * 2);

                if (info.IsKeyDown(Keys.Down))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top + 1, 80 * 2, 25 * 2);

                if (info.IsKeyReleased(Keys.Return))
                {
                    messageData.Fill(Color.White, Color.Black, 0, null);
                    messageData.Print(0, 0, "Generating map, please wait...");
                    initialized = true;
                    initializedStep2 = false;
                    initializedStep3 = false;
                }

                if (info.IsKeyReleased(Keys.Space))
                {
                    var oldRenderArea = cons.TextSurface.RenderArea;
                    var oldFont = cons.TextSurface.Font;

                    if (textSurface == generator.HeightMapRenderer)
                    {
                        textSurface = generator.HeatMapRenderer;
                        messageData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Heat    ", Color.White, Color.Black);
                    }
                    else if (textSurface == generator.HeatMapRenderer)
                    {
                        textSurface = generator.MoistureMapRenderer;
                        messageData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Moisture", Color.White, Color.Black);
                    }
                    else if (textSurface == generator.MoistureMapRenderer)
                    {
                        textSurface = generator.BiomeMapRenderer;
                        messageData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Biome   ", Color.White, Color.Black);
                    }
                    else
                    {
                        textSurface = generator.HeightMapRenderer;
                        messageData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Height  ", Color.White, Color.Black);
                    }

                    cons.TextSurface.RenderArea = oldRenderArea;
                    cons.TextSurface.Font = oldFont;
                }

                return true;
            };

        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                Clear();
                Print(0, 0, "Generating map, please wait...");
                initialized = true;
            }
        }

        public override void Render()
        {
            // These 3 render calls are a hack to get the console data generated and display a message to the user
            // Should add in async calls that let us generate these in the background... That would be cool.

            if (!initialized)
                base.Render();

            else if(!initializedStep2)
            {
                initializedStep2 = true;
                base.Render();
                messageData.Render();
            }
            else if (!initializedStep3)
            {
                base.Render();

                // Generate the content
                //mainData = new CellSurface(2000, 2000);

                // Clear message data and make it transparent so that it acts as a layer
                messageData.Fill(Color.White, Color.Transparent, 0, null);

                generator = new SadConsole.Game.WorldGeneration.WrappingWorldGenerator<SadConsole.Game.WorldGeneration.TextSurfaceMap, TextSurface>();
                generator.Start(512, 256);
                textSurface = generator.BiomeMapRenderer;
                messageData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Biome   ", Color.White, Color.Black);
                textSurface.Font = Engine.Fonts[Engine.DefaultFont.Name].GetFont(SadConsole.Font.FontSizes.Half);
                
                initializedStep3 = true;
            }

            else
            {
                // Set message data information about where the viewport is located
                //messageData.CellData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Top}            ", Color.White, Color.Black);

                // Create a faux layering system.
                base.Render();

                if (IsVisible)
                    messageData.Render();
            }
        }
    }
}
