using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;

namespace StarterProject.CustomConsoles
{
    class RandomScrollingConsole : Console
    {
        private CellSurface mainData;
        private CellsRenderer messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        public RandomScrollingConsole() : base(80, 25)
        {
            messageData = new Console(1, 1);
            messageData.CellData = CellData;
            IsVisible = false;

            
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X - 1, cons.ViewArea.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X + 1, cons.ViewArea.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X, cons.ViewArea.Y - 1, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    cons.ViewArea = new Rectangle(cons.ViewArea.X, cons.ViewArea.Y + 1, 80, 25);

                return true;
            };

        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                CellData.Print(0, 0, "Generating random console data, please wait...");
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
            }
            else if (!initializedStep3)
            {
                base.Render();

                // Generate the content
                mainData = new CellSurface(2000, 2000);

                // Clear message data and make it transparent so that it acts as a layer
                messageData.CellData.Fill(Color.White, Color.Transparent, 0, null);

                // We need to set celldata to the big console data so we can use the FillWithRandom method.
                CellData = mainData;
                ViewArea = new Rectangle(0, 0, 80, 25);
                FillWithRandomGarbage();
                initializedStep3 = true;
            }

            else
            {
                // Set message data information about where the viewport is located
                messageData.CellData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Y}            ", Color.White, Color.Black);

                // Create a faux layering system.
                base.Render();
                messageData.Render();
            }
        }
    }
}
