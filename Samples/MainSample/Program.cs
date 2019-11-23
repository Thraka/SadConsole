using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace ConsoleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!")
            //Console.ReadKey();

            SadConsole.Game.Create(80, 25);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            //Game.Instance.MonoGameInstance.Components.Add(new SadConsole.MonoGame.Game.FPSCounterComponent(Game.Instance.MonoGameInstance));
            //Global.Screen.Surface.Print(1, 1, "Hello from SadConsole 9.0");
            //Global.Screen.Surface.Print(10, 15, "Hello from SadConsole 9.0", Color.AnsiCyan);
            //Global.Screen.Surface.Print(5, 18, new ColorGradient(Color.AliceBlue, Color.DarkOrange, Color.LightPink, Color.Red.GetRandomColor(Global.Random)).ToColoredString("Some color is fun to play with when you got it!"));

            //var screen = new ScreenObjectSurface(new CellSurface(5, 5, 10, 10));
            //screen.Surface.DefaultBackground = Color.Green;
            //screen.Surface.Print(0, 0, "0123456789");
            //screen.Surface.Print(0, 9, "0123456789");
            //for (int i = 0; i < 10; i++)
            //{
            //    char value = Convert.ToString(i + 1)[0];
            //    screen.Surface.SetGlyph(0, i + 1, value);
            //    screen.Surface.SetGlyph(9, i + 1, value);
            //    screen.Surface.SetGlyph(i + 1, i + 1, value);
            //}
            //screen.Surface.BufferPosition = new Point(5,5);
            //screen.Surface.ViewWidth = 8;
            //screen.Surface.ViewHeight = 8;
            //screen.Parent = Global.Screen;
            //screen.Position = new Point(1, 1);

            //var surface = screen.Surface.GetSubSurface(new Rectangle(0, 0, 10, 10));
            //screen.Surface.Copy(surface);
            //surface.DefaultBackground = Color.Blue;
            //var screen2 = new ScreenObjectSurface(surface);
            //screen2.Position = new Point(1, 11);
            //screen2.Parent = Global.Screen;
            //screen2.Surface.SetGlyph(5, 4, 'a');
            //screen2.Surface.SetBackground(5, 4, Color.Purple);
            //screen2.Surface.SetForeground(5, 4, Color.Black);
            //Global.Screen.Renderer = null;

            var layeredSurface = new LayeredCellSurface(20, 10, 3);

            AddStars(layeredSurface.Layers[0].Surface, Color.DarkOrange);
            AddStars(layeredSurface.Layers[1].Surface, Color.Green);
            AddStars(layeredSurface.Layers[2].Surface, Color.GreenYellow);

            static void AddStars(CellSurface surface, Color starColor)
            {
                int count = surface.BufferWidth * surface.BufferHeight / 10;

                for (int i = 0; i < count; i++)
                {
                    if (Global.Random.Next(0, 10) > 5)
                    {
                        int x = Global.Random.Next(0, surface.BufferWidth);
                        int y = Global.Random.Next(0, surface.BufferHeight);

                        surface.SetGlyph(x, y, 7, starColor);
                    }
                }
            }

            var con = new Console(layeredSurface);
            con.Renderer = new SadConsole.Renderers.LayeredConsole();
            con.Parent = Global.Screen;
            con.Position = new Point(10, 14);
            con.Cursor.IsEnabled = false;

            var set = new SadConsole.Instructions.InstructionSet()
                .InstructConcurrent(
                    new SadConsole.Instructions.InstructionSet() { RepeatCount = -1 }
                        .Wait(new TimeSpan(0, 0, 1))
                        .Code((s) => { layeredSurface.Layers[0].Surface.ShiftRight(1, true); layeredSurface.IsDirty = true; return true; }),

                    new SadConsole.Instructions.InstructionSet() { RepeatCount = -1 }
                        .Wait(new TimeSpan(0, 0, 0, 0, 800))
                        .Code((s) => { layeredSurface.Layers[1].Surface.ShiftRight(1, true); layeredSurface.IsDirty = true; return true; }),

                    new SadConsole.Instructions.InstructionSet() { RepeatCount = -1 }
                        .Wait(new TimeSpan(0, 0, 0, 0, 400))
                        .Code((s) => { layeredSurface.Layers[2].Surface.ShiftRight(1, true); layeredSurface.IsDirty = true; return true; })
                );

            con.Components.Add(set);

            var obj2 = new ScreenObjectSurface(layeredSurface);
            obj2.Renderer = new SadConsole.Renderers.LayeredScreenObject();
            obj2.Position = (5, 1);
            obj2.Parent = Global.Screen;

            layeredSurface.IsDirtyChanged += (o, e) => { obj2.ForceRendererRefresh = true; };


            var ent = new SadConsole.Entities.Entity(3, 3);
            ent.Animation.CurrentFrame.Fill(Color.AliceBlue, Color.DarkBlue, '.');
            ent.Animation.Center = (1, 1);
            ent.UseKeyboard = true;
            ent.Components.Add(new SadConsole.Components.MoveObject());
            Global.Screen.Children.Add(ent);
            ent.IsFocused = true;

            //con.Components.Add(
            //    new SadConsole.Instructions.InstructionSet().Wait(TimeSpan.FromSeconds(1)).Instruct(

            //    new SadConsole.Instructions.FadeTextSurfaceTint(
            //                    new SadRogue.Primitives.ColorGradient(Color.Purple.SetAlpha(0), Color.Purple.FillAlpha(), Color.Purple.SetAlpha(0)),
            //                    TimeSpan.FromSeconds(5)
            //                  )
            //    { RepeatCount = -1 })
            //    );

            //con.Components.Add(new MouseTest());

            //var animation = AnimatedScreenObject.CreateStatic(20, 10, 20, 0.5d);
            //animation.Parent = Global.Screen;
            //animation.Position = (22, 2);
        }

        class MouseTest : SadConsole.Components.MouseConsoleComponent
        {
            public override void ProcessMouse(ScreenObject host, MouseScreenObjectState state, out bool handled)
            {
                if (state.Mouse.IsOnScreen)
                {
                    host.Position = state.WorldCellPosition;
                    handled = true;
                }
                else
                    handled = false;
            }
        }
    }
}
