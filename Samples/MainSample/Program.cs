using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using SadRogue.Primitives.GridViews;
using SadConsole.Effects;
using System.Collections;
using SadConsole.Quick;
using SadConsole.Readers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FeatureDemo
{
    internal class Program
    {
        private static Windows.CharacterViewer _characterWindow;
        private static Container MainConsole;

        public static int MainWidth = 80;
        public static int MainHeight = 23;
        public static int HeaderWidth = 80;
        public static int HeaderHeight = 2;

        private static void Main(string[] args)
        {
            SadConsole.Settings.UnlimitedFPS = true;
            //SadConsole.Settings.UseDefaultExtendedFont = true;
            //SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

#if MONOGAME
            Settings.WindowTitle = "Feature Demo (MonoGame)";
#elif SFML
            Settings.WindowTitle = "Feature Demo (SFML)";
#endif

            SadConsole.Game.Create(80, 25); //, "Res/Fonts/C64.font");
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.FrameUpdate += Instance_FrameUpdate;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
            
        }

        private static void Instance_FrameUpdate(object sender, GameHost e)
        {
            // Called each logic update.
            //if (!_characterWindow.IsVisible)
            {
                // This block of code cycles through the consoles in the SadConsole.Engine.ConsoleRenderStack, showing only a single console
                // at a time. This code is provided to support the custom consoles demo. If you want to enable the demo, uncomment one of the lines
                // in the Initialize method above.
                if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F1))
                {
                    MainConsole.MoveNextConsole();
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F2))
                {
                    _characterWindow.Show(true);
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F9))
                {
                    //SadConsole.Debug.Screen.Show();
#if MONOGAME
                    if (!SadConsole.Debug.MonoGame.Debugger.IsOpened)
                        SadConsole.Debug.MonoGame.Debugger.Start();
                    else
                        SadConsole.Debug.MonoGame.Debugger.Stop();
#endif
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F5))
                {
                    SadConsole.Game.Instance.ToggleFullScreen();

                }

            }
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            if (Settings.UnlimitedFPS)
              SadConsole.Game.Instance.MonoGameInstance.Components.Add(new SadConsole.Host.Game.FPSCounterComponent(SadConsole.Game.Instance.MonoGameInstance));
            
            // Register the types provided by the SadConsole.Extended library
            SadConsole.UI.RegistrarExtended.Register();

            // Splash screens show up at the start of the game.
            //SadConsole.Game.Instance.SetSplashScreens(new SadConsole.SplashScreens.PCBoot());

            // Initialize the windows used by the global keyboard handler in Instance_FrameUpdate
            _characterWindow = new Windows.CharacterViewer(0);

            // The demo screen 
            MainConsole = new Container();

            // By default SadConsole adds a blank ready-to-go console to the rendering system. 
            // We don't want to use that for the sample project so we'll remove and then destroy it.
            Game.Instance.Screen = MainConsole;
        }

        // this is test code for things not yet here...
        /*
        static void Init2()
        {
            // This stuff needs full tests written for all scenarios
            var console = new Console(10, 20);
            //console.PrintFadingText("This is a fade!", (2, 2), TimeSpan.FromSeconds(1));
            console.Print(0, 0, "0123456789", Color.AnsiBlueBright);
            console.Print(0, 1, "0123456789", Color.AnsiRedBright);
            console.Print(0, 2, "0123456789", Color.AnsiYellowBright);
            console.Print(0, 3, "0123456789", Color.AnsiGreenBright);
            console.Print(0, 4, "0123456789", Color.AnsiGreenBright);

            console.ShiftRow(0, 4, -3, false);

            console.Cursor.UseStringParser = true;
            console.Cursor.Move(3, 15).Print("[c:r f:DodgerBlue]4.[c:r f:White] You [c:b 3]can apply [c:b 5:0.17]blink effects.");

            EffectSet set = new EffectSet() { new BlinkGlyph() { BlinkCount = 2 }, new BlinkGlyph() { Duration = System.TimeSpan.FromSeconds(5), GlyphIndex = 2 } };
            set.Repeat = true;
            console.SetEffect(3, 3, set.Clone());
            //console.ShiftRow(0, 0, 3, false);
            //console.ShiftRow(1, 1, 3, false);
            //console.ShiftRow(2, 2, 3, false);
            //console.ShiftRow(3, 2, 10, false);
            ////console.ShiftRow(4, 20, 10, false);


            //console.Print(0, 6, "0123456789", Color.AnsiBlueBright);
            //console.Print(0, 7, "0123456789", Color.AnsiRedBright);
            //console.Print(0, 8, "0123456789", Color.AnsiYellowBright);
            //console.Print(0, 9, "0123456789", Color.AnsiGreenBright);
            //console.Print(0, 10, "0123456789", Color.AnsiGreenBright);

            //console.ShiftRow(6, 9, -3, false);
            //console.ShiftRow(7, 8, -3, false);
            //console.ShiftRow(8, 7, -3, false);
            //console.ShiftRow(9, 6, -10, false);
            ////console.ShiftRow(10, 20, -10, false);


            Game.Instance.Screen = console;

            Game.Instance.DestroyDefaultStartingConsole();
        }
        */

    }


    class CellSurface<TGlyph> : ICellSurface
        where TGlyph : ColoredGlyph
    {
        private TGlyph[] _cells;

        public ColoredGlyph this[Point pos] => throw new NotImplementedException();

        public ColoredGlyph this[int index1D] => throw new NotImplementedException();

        public ColoredGlyph this[int x, int y] => throw new NotImplementedException();

        public int TimesShiftedDown { get; set; }
        public int TimesShiftedRight { get; set; }
        public int TimesShiftedLeft { get; set; }
        public int TimesShiftedUp { get; set; }
        public bool UsePrintProcessor { get; set; }

        public EffectsManager Effects { get; }

        public Rectangle Area => throw new NotImplementedException();

        public Color DefaultBackground { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color DefaultForeground { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DefaultGlyph { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDirty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsScrollable => throw new NotImplementedException();

        public Rectangle View { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ViewHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point ViewPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ViewWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Height => throw new NotImplementedException();

        public int Width => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public event EventHandler IsDirtyChanged;

        public IEnumerator<ColoredGlyph> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }


    class FunGlyph : ColoredGlyph
    {
        public FunGlyph()
        {
        }

        public FunGlyph(Color foreground) : base(foreground)
        {
        }

        public FunGlyph(Color foreground, Color background) : base(foreground, background)
        {
        }

        public FunGlyph(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {
        }

        public FunGlyph(Color foreground, Color background, int glyph, Mirror mirror) : base(foreground, background, glyph, mirror)
        {
        }

        public FunGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible) : base(foreground, background, glyph, mirror, isVisible)
        {
        }

        public FunGlyph(Color foreground, Color background, int glyph, Mirror mirror, bool isVisible, CellDecorator[] decorators) : base(foreground, background, glyph, mirror, isVisible, decorators)
        {
        }

        public void Randomize()
        {
            Foreground = Foreground.GetRandomColor(SadConsole.Game.Instance.Random);
            Background = Foreground.GetRandomColor(SadConsole.Game.Instance.Random);
        }
    }

}
