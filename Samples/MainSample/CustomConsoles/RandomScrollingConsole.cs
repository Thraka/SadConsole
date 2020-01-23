using System;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace FeatureDemo.CustomConsoles
{
    internal class RandomScrollingConsole : Console
    {
        private readonly Console mainData;
        private readonly Console messageData;
        private bool initialized;
        private bool initializedStep2;
        private bool initializedStep3;

        public RandomScrollingConsole() : base(80, 23)
        {
            messageData = new Console(80, 1);
            messageData.Print(0, 0, "Generating random console data, please wait...");
            mainData = new Console(1, 1);
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

        public override void Draw()
        {
            // These 3 render calls are a hack to get the console data generated and display a message to the user
            // Should add in async calls that let us generate these in the background... That would be cool.
            if (IsVisible)
            {
                if (!initialized)
                {
                    base.Draw();
                }
                else if (!initializedStep2)
                {
                    initializedStep2 = true;
                    base.Draw();
                }
                else if (!initializedStep3)
                {
                    base.Draw();

                    mainData.SadComponents.Clear();

                    // Generate the content
                    mainData.Resize(80, 23, 2000, 2000, false);
                    mainData.SadComponents.Add(new InputHandling.MoveViewPortKeyboardHandler());
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
                    base.Draw();

                    //Renderer.Render(messageData.TextSurface, new Point(0, 0));
                }
            }
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info) => mainData.ProcessKeyboard(info);
    }
}
