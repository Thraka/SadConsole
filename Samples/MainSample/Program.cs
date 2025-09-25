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

        public static int MainWidth = 80;
        public static int MainHeight = 23;
        public static int HeaderWidth = 80;
        public static int HeaderHeight = 2;

        private static void Main(string[] args)
        {
            //SadConsole.Settings.UseDefaultExtendedFont = true;
            //SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

            Game.Configuration configuration = new Game.Configuration()
                .ConfigureFonts(loader => loader.UseBuiltinFontExtended())
                .SetScreenSize(80, 25)
                .SetStartingScreen<Container>()
                .OnStart(Init)
                .UseFrameUpdateEvent(Instance_FrameUpdate)
                ;

#if MONOGAME
            Settings.WindowTitle = "Feature Demo (MonoGame)";
            configuration
                .ShowMonoGameFPS()
                .UseUnlimitedFPS();
#elif SFML
            Settings.WindowTitle = "Feature Demo (SFML)";
            //configuration.UseUnlimitedFPS();
#endif
            Game.Create(configuration);
            Game.Instance.Run();
            Game.Instance.Dispose();
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
                    ((Container)Game.Instance.Screen).MoveNextConsole();

                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F2))
                    _characterWindow.Show(true);
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F3))
                {
#if MONOGAME
                    SadConsole.Host.Global.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
                    SadConsole.Game.Instance.MonoGameInstance.IsFixedTimeStep = true;
#endif
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F10))
                {
                    SadConsole.Debug.Screen.Show();
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F9))
                {
#if MONOGAME
                    if (!SadConsole.Debug.MonoGame.Debugger.IsOpened)
                        SadConsole.Debug.MonoGame.Debugger.Start();
                    else
                        SadConsole.Debug.MonoGame.Debugger.Stop();
#endif
                }
                else if (SadConsole.GameHost.Instance.Keyboard.IsKeyReleased(Keys.F5))
                    SadConsole.Game.Instance.ToggleFullScreen();

            }
        }

        /// <summary>
        /// <c>test</c>
        /// </summary>
        private static void Init()
        {
            // Register the types provided by the SadConsole.Extended library
            SadConsole.UI.RegistrarExtended.Register();

            // Splash screens show up at the start of the game.
            //SadConsole.Game.Instance.SetSplashScreens(new SadConsole.SplashScreens.Ansi1());

            // Initialize the windows used by the global keyboard handler in Instance_FrameUpdate
            _characterWindow = new Windows.CharacterViewer(0);

            // The demo screen 
            //Game.Instance.Screen = new Container();

            //return;
            //SadConsole.Settings.ClearColor = Color.White;

            return;
            var panel1 = new SadConsole.UI.ControlsConsole(20, 10);
            panel1.Surface.DefaultBackground = Color.LightGray;
            panel1.Surface.DefaultForeground = Color.Black;
            panel1.Surface.Clear();
            panel1.Surface.Print(2, 2, "hello");
            panel1.Position = (2, 5);
            panel1.FocusOnMouseClick = true;

            var borderParams = SadConsole.UI.Border.BorderParameters.GetDefault()
                .ChangeBorderGlyph(ICellSurface.ConnectedLineThin)
                .ChangeBorderColors(panel1.Surface.DefaultForeground, panel1.Surface.DefaultBackground)
                .AddTitle("Panel 1", panel1.Surface.DefaultBackground, panel1.Surface.DefaultForeground)
                .AddShadow(177, Color.Black, Color.White);

            var border = new SadConsole.UI.Border(panel1, borderParams);

            var label = new SadConsole.UI.Controls.Label(3);
            label.Position = new Point(2, 4);

            var scroll = new SadConsole.UI.Controls.ScrollBar(Orientation.Vertical, 2);
            scroll.Position = (1, 4);
            scroll.Maximum = 10;
            scroll.ValueChanged += (s,e) => { label.DisplayText = scroll.Value.ToString(); };

            panel1.Controls.Add(label);
            panel1.Controls.Add(scroll);

            Game.Instance.Screen = new ScreenObject();
            Game.Instance.Screen.Children.Add(panel1);





            panel1 = new SadConsole.UI.ControlsConsole(20, 10);
            panel1.Surface.DefaultBackground = Color.AnsiWhite;
            panel1.Surface.DefaultForeground = Color.AnsiBlue;
            panel1.Surface.Clear();
            panel1.Surface.Print(2, 2, "hello");
            panel1.Position = (27, 5);
            panel1.FocusOnMouseClick = true;

            borderParams = SadConsole.UI.Border.BorderParameters.GetDefault()
                .ChangeBorderGlyph(ICellSurface.Connected3dBox)
                .ChangeBorderColors(Color.AnsiWhiteBright, panel1.Surface.DefaultBackground)
                .AddTitle("Panel 2", Color.AnsiWhiteBright, Color.AnsiBlue)
                ;
            

            border = new SadConsole.UI.Border(panel1, borderParams);
            for (int i = 1; i < border.Surface.Width; i++)
                border.Surface[i, border.Surface.Height - 1].Foreground = Color.AnsiBlackBright;
            for (int i = 0; i < border.Surface.Height; i++)
                border.Surface[border.Surface.Width - 1, i].Foreground = Color.AnsiBlackBright;
            border.Surface.IsDirty = true;

            label = new SadConsole.UI.Controls.Label(3);
            label.Position = new Point(2, 4);

            scroll = new SadConsole.UI.Controls.ScrollBar(Orientation.Vertical, 2);
            scroll.Position = (1, 4);
            scroll.Maximum = 10;
            scroll.ValueChanged += (s, e) => { label.DisplayText = scroll.Value.ToString(); };

            panel1.Controls.Add(label);
            panel1.Controls.Add(scroll);

            Game.Instance.Screen.Children.Add(panel1);


            panel1 = new SadConsole.UI.ControlsConsole(20, 10);
            panel1.Surface.DefaultBackground = Color.AnsiWhite;
            panel1.Surface.DefaultForeground = Color.AnsiBlue;
            panel1.Surface.Clear();
            panel1.Surface.Print(2, 2, "hello");
            panel1.Position = (49, 5);
            panel1.FocusOnMouseClick = true;

            SadConsole.UI.Border.Create3DForSurface(panel1, "Panel 3");


            label = new SadConsole.UI.Controls.Label(3);
            label.Position = new Point(2, 4);

            scroll = new SadConsole.UI.Controls.ScrollBar(Orientation.Vertical, 2);
            scroll.Position = (1, 4);
            scroll.Maximum = 10;
            scroll.ValueChanged += (s, e) => { label.DisplayText = scroll.Value.ToString(); };

            panel1.Controls.Add(label);
            panel1.Controls.Add(scroll);

            Game.Instance.Screen.Children.Add(panel1);
        }
    }

    // Playing around code, ignore this
    /*
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
    */
}
