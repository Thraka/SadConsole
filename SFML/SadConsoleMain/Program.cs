using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Rectangle = SFML.Graphics.IntRect;

namespace SadConsoleMain
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new SFML.Graphics.RenderWindow(new SFML.Window.VideoMode(400, 500), "Test", SFML.Window.Styles.Default);

            window.Closed += Window_Closed;
            //window.SetFramerateLimit(60);

            

            var surface = SadConsole.Engine.Initialize(window, "IBM.font", 80, 25);
            //Engine.DefaultFont = Engine.Fonts["IBM"].GetFont(Font.FontSizes.Two);
            //Engine.DefaultFont.ResizeGraphicsDeviceManager(window, 80, 25, 0, 0);

            //surface.TextSurface.DefaultBackground = SFML.Graphics.Color.Yellow;
            //surface.Fill(SFML.Graphics.Color.Green, null, 1, null);
            //surface.TextSurface.Font = SadConsole.Engine.Fonts["IBM"].GetFont(SadConsole.Font.FontSizes.Two);
            //SadConsole.Consoles.TextSurfaceRenderer renderer = new SadConsole.Consoles.TextSurfaceRenderer();
            //surface.TextSurface.Font.ResizeGraphicsDeviceManager(window, 80, 25, 0, 0);
            ////batch.Update(surface);
            //surface.Print(2, 2, "LL[c:g f:White:Black:11]LLLLLLLLLLLLL");
            //surface.Print(5, 5, "P ", spriteEffect: SFML.Graphics.SpriteEffects.FlipHorizontally);
            //surface.Print(7, 5, "P ", spriteEffect: SFML.Graphics.SpriteEffects.FlipVertically);
            //surface.Print(9, 5, "P ", spriteEffect: SFML.Graphics.SpriteEffects.FlipVertically | SFML.Graphics.SpriteEffects.FlipHorizontally);
            ////surface.Renderer = new SadConsole.Consoles.CachedTextSurfaceRenderer(surface.TextSurface);

            var fps = new FPSCounterComponent();

            for (int i = 0; i < 5; i++)
            {
                //SadConsole.Engine.ConsoleRenderStack.Add(new SadConsole.Consoles.Console(surface.TextSurface));
            }

            RandomScrollingConsole surface1 = new RandomScrollingConsole();
            surface1.IsVisible = true;
            Engine.ConsoleRenderStack.Clear();
            Engine.ConsoleRenderStack.Add(surface1);

            Engine.ActiveConsole = surface1;

            while (window.IsOpen)
            {
                window.Clear(SFML.Graphics.Color.Black);

                SadConsole.Engine.Update(window.HasFocus());
                SadConsole.Engine.Draw();

                fps.Update();
                fps.Draw();

                window.Display();

                window.DispatchEvents();
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            ((SFML.Window.Window)sender).Close();
        }
        public class FPSCounterComponent
        {
            SadConsole.Consoles.TextSurfaceRenderer consoleRender;
            SadConsole.Consoles.TextSurface console;
            SadConsole.Consoles.SurfaceEditor editor;

            int frameRate = 0;
            int frameCounter = 0;
            TimeSpan elapsedTime = TimeSpan.Zero;


            public FPSCounterComponent()
            {
                console = new SadConsole.Consoles.TextSurface(10, 1, SadConsole.Engine.DefaultFont);
                editor = new SadConsole.Consoles.SurfaceEditor(console);
                console.DefaultBackground = SFML.Graphics.Color.Black;
                editor.Clear();
                consoleRender = new SadConsole.Consoles.TextSurfaceRenderer();
            }


            public void Update()
            {
                elapsedTime += SadConsole.Engine.GameTimeUpdate.ElapsedGameTime;

                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }


            public void Draw()
            {
                frameCounter++;

                string fps = string.Format("fps: {0}", frameRate);
                editor.Clear();
                editor.Print(0, 0, fps);
                consoleRender.Render(console, new SFML.System.Vector2i(0, 0));
            }
        }

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

                    if (info.IsKeyDown(SFML.Window.Keyboard.Key.Left))
                        cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left - 1, cons.TextSurface.RenderArea.Top, 80, 25);

                    if (info.IsKeyDown(SFML.Window.Keyboard.Key.Right))
                        cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left + 1, cons.TextSurface.RenderArea.Top, 80, 25);

                    if (info.IsKeyDown(SFML.Window.Keyboard.Key.Up))
                        cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top - 1, 80, 25);

                    if (info.IsKeyDown(SFML.Window.Keyboard.Key.Down))
                        cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.Left, cons.TextSurface.RenderArea.Top + 1, 80, 25);

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
                        TextSurface.RenderArea = new Rectangle(0, 0, 80, 25);

                        // Clear message data and make it transparent so that it acts as a layer
                        messageData.Fill(SFML.Graphics.Color.White, SFML.Graphics.Color.Transparent, 0, null);

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


}
