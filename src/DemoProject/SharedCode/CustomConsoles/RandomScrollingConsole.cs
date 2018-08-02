using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System;

namespace StarterProject.CustomConsoles
{
    class RandomScrollingConsole : Console
    {
        private Basic mainData;
        private Basic messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;
        
        public RandomScrollingConsole() : base(80, 23)
        {
            messageData = new Basic(80, 1);
            messageData.Print(0, 0, "Generating random console data, please wait...");
            mainData = new Basic(1, 1);
            IsVisible = false;
            mainData.IsVisible = false;
            
            KeyboardHandler = (cons, info) =>
            {
                if (!mainData.IsVisible)
                    return false; 

                if (info.IsKeyDown(Keys.Left))
                    mainData.ViewPort = new Rectangle(mainData.ViewPort.Left - 1, mainData.ViewPort.Top, 80, 23);

                if (info.IsKeyDown(Keys.Right))
                    mainData.ViewPort = new Rectangle(mainData.ViewPort.Left + 1, mainData.ViewPort.Top, 80, 23);

                if (info.IsKeyDown(Keys.Up))
                    mainData.ViewPort = new Rectangle(mainData.ViewPort.Left, mainData.ViewPort.Top - 1, 80, 23);

                if (info.IsKeyDown(Keys.Down))
                    mainData.ViewPort = new Rectangle(mainData.ViewPort.Left, mainData.ViewPort.Top + 1, 80, 23);

                return true;
            };

            Children.Add(mainData);
            Children.Add(messageData);
        }

        protected override void OnVisibleChanged()
        {
            if (IsVisible && !initialized)
            {
                // Write to the message layer
                initialized = true;
            }
        }

        public override void Draw(TimeSpan delta)
        {
            // These 3 render calls are a hack to get the console data generated and display a message to the user
            // Should add in async calls that let us generate these in the background... That would be cool.
            if (IsVisible)
            {
                if (!initialized)
                    base.Draw(delta);

                else if (!initializedStep2)
                {
                    initializedStep2 = true;
                    base.Draw(delta);
                }
                else if (!initializedStep3)
                {
                    base.Draw(delta);

                    // Generate the content
                    mainData.Resize(2000, 2000, false, new Rectangle(0, 0, 80, 23));
                    mainData.FillWithRandomGarbage();
                    mainData.IsVisible = true;

                    // Clear message data and make it transparent so that it acts as a layer
                    messageData.IsVisible = false;

                    // We need to set celldata to the big console data so we can use the FillWithRandom method.
                    initializedStep3 = true;
                }

                else
                {
                    // Set message data information about where the viewport is located
                    //messageData.Print(0, 0, $"{ViewArea.X} , {ViewArea.Y}            ", Color.White, Color.Black);

                    // Create a faux layering system.
                    base.Draw(delta);

                    //Renderer.Render(messageData.TextSurface, new Point(0, 0));
                }
            }
        }
    }
}
