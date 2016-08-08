using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using static SFML.Window.Keyboard;
using SFML.Graphics;

namespace StarterProject.CustomConsoles
{
    class RandomScrollingConsole : Console
    {
        private TextSurface mainData;
        private SurfaceEditor messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        public RandomScrollingConsole() : base(80, 25)
        {
            messageData = new SurfaceEditor(new TextSurface(10, 1, Engine.DefaultFont));
            IsVisible = false;

            
            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Key.Left))
                    cons.TextSurface.RenderArea = new IntRect(cons.TextSurface.RenderArea.Left - 1, cons.TextSurface.RenderArea.Top, 80, 25);

                if (info.IsKeyDown(Key.Right))
                    cons.TextSurface.RenderArea = new IntRect(cons.TextSurface.RenderArea.Left + 1, cons.TextSurface.RenderArea.Top, 80, 25);

                if (info.IsKeyDown(Key.Up))
                    cons.TextSurface.RenderArea = new IntRect(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top - 1, 80, 25);

                if (info.IsKeyDown(Key.Down))
                    cons.TextSurface.RenderArea = new IntRect(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top + 1, 80, 25);

                return true;
            };

        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                Print(0, 0, "Generating random console data, please wait...");
                initialized = true;
            }
        }

        public override void Render()
        {
            // These 3 render calls are a hack to get the console data generated and display a message to the user
            // Should add in async calls that let us generate these in the background... That would be cool.
            if (IsVisible)
            {
                if (!initialized)
                    base.Render();

                else if (!initializedStep2)
                {
                    initializedStep2 = true;
                    base.Render();
                }
                else if (!initializedStep3)
                {
                    base.Render();

                    // Generate the content
                    TextSurface = new TextSurface(2000, 2000, Engine.DefaultFont); //500mb ?? why?
                    //Data = new TextSurface(2000, 2000);
                    //DataViewport = new Rectangle(0, 0, 80, 25);
                    TextSurface.RenderArea = new IntRect(0, 0, 80, 25);

                    // Clear message data and make it transparent so that it acts as a layer
                    messageData.Fill(Color.White, Color.Transparent, 0, null);

                    // We need to set celldata to the big console data so we can use the FillWithRandom method.
                    FillWithRandomGarbage();
                    initializedStep3 = true;
                }

                else
                {
                    // Set message data information about where the viewport is located
                    //messageData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Y}            ", Color.White, Color.Black);

                    // Create a faux layering system.
                    base.Render();

                    //Renderer.Render(messageData.TextSurface, new Point(0, 0));
                }
            }
        }
    }
}
