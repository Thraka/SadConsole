using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ColorHelper = Microsoft.Xna.Framework.Color;

using SadConsole;

using ScrollingConsole = SadConsole.ScrollingConsole;
using System;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class RandomScrollingConsole : ScrollingConsole
    {
        private ScrollingConsole mainData;
        private ScrollingConsole messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;
        
        public RandomScrollingConsole() : base(80, 23)
        {
            messageData = new ScrollingConsole(80, 1);
            messageData.Print(0, 0, "Generating random console data, please wait...");
            mainData = new ScrollingConsole(1, 1);
            IsVisible = false;
            mainData.IsVisible = false;

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

                    mainData.Components.RemoveAll();
                    mainData.Components.Add(new InputHandling.MoveViewPortKeyboardHandler());

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

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            return mainData.ProcessKeyboard(info);
        }
    }
}
