using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;

namespace StarterProject.CustomConsoles
{
    class WorldGenerationConsole : Console
    {
        //private CellSurface mainData;
        private CellsRenderer messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        // Zoom calculation for the width/height of a cell. Zooms out to make it look like square pixels.
        private Point ZoomLevel = new Point(2, 4);

        private SadConsole.GameHelpers.WorldGeneration.WrappingWorldGenerator<SadConsole.GameHelpers.WorldGeneration.CellSurfaceMap, CellSurface> generator;

        public WorldGenerationConsole() : base(80, 25)
        {
            messageData = new Console(1, 1);
            messageData.CellData = CellData;
            IsVisible = false;
            
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X - 1, cons.ViewArea.Y, 80 * ZoomLevel.X, 25 * ZoomLevel.Y);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X + 1, cons.ViewArea.Y, 80 * ZoomLevel.X, 25 * ZoomLevel.Y);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X, cons.ViewArea.Y - 1, 80 * ZoomLevel.X, 25 * ZoomLevel.Y);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X, cons.ViewArea.Y + 1, 80 * ZoomLevel.X, 25 * ZoomLevel.Y);
                
                if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    messageData.CellData.Fill(Color.White, Color.Black, 0, null);
                    messageData.CellData.Print(0, 0, "Generating map, please wait...");
                    initialized = true;
                    initializedStep2 = false;
                    initializedStep3 = false;
                }

                if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    if (CellData == generator.HeightMapRenderer)
                    {
                        CellData = generator.HeatMapRenderer;
                        messageData.CellData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Heat    ", Color.White, Color.Black);
                    }
                    else if (CellData == generator.HeatMapRenderer)
                    {
                        CellData = generator.MoistureMapRenderer;
                        messageData.CellData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Moisture", Color.White, Color.Black);
                    }
                    else if (CellData == generator.MoistureMapRenderer)
                    {
                        CellData = generator.BiomeMapRenderer;
                        messageData.CellData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Biome   ", Color.White, Color.Black);
                    }
                    else
                    {
                        CellData = generator.HeightMapRenderer;
                        messageData.CellData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Height  ", Color.White, Color.Black);
                    }
                }

                return true;
            };

        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                CellData.Clear();
                CellData.Print(0, 0, "Generating map, please wait...");
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
                messageData.CellData.Fill(Color.White, Color.Transparent, 0, null);

                generator = new SadConsole.GameHelpers.WorldGeneration.WrappingWorldGenerator<SadConsole.GameHelpers.WorldGeneration.CellSurfaceMap, CellSurface>();
                generator.Start(256, 256);
                CellData = generator.BiomeMapRenderer;
                messageData.CellData.Print(0, 0, $"[SPACE] Change Map Info [ENTER] New Map -- Biome   ", Color.White, Color.Black);
                this.CellSize = new Point(this.Font.CellWidth / ZoomLevel.X, this.Font.CellHeight / ZoomLevel.Y);

                initializedStep3 = true;
            }

            else
            {
                // Set message data information about where the viewport is located
                //messageData.CellData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Y}            ", Color.White, Color.Black);

                // Create a faux layering system.
                base.Render();

                if (IsVisible)
                    messageData.Render();
            }
        }
    }
}
