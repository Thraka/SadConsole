using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MonoGame.Framework.WpfInterop;
using SadConsole;
using Console = SadConsole.Console;

namespace WPFSadConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.None;

            InitializeComponent();
        }

        private void Game_SadConsoleStarted(object sender, EventArgs e)
        {
            SadConsole.Game.Instance.StartingConsole.DrawBox(new SadRogue.Primitives.Rectangle(2, 1, 28, 3),
                                                             ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick,
                                                                new ColoredGlyph(SadRogue.Primitives.Color.AnsiCyanBright, SadRogue.Primitives.Color.AnsiCyan),
                                                                new ColoredGlyph(SadRogue.Primitives.Color.AnsiCyanBright, SadRogue.Primitives.Color.AnsiCyan)
                                                             ));

            SadConsole.Game.Instance.StartingConsole.Print(4, 2, "Welcome to SadConsole WPF");


            lstScreenObjects.Items.Add(SadConsole.Game.Instance.StartingConsole.ToString());
        }


        private void Game_WindowResized(object sender, EventArgs e)
        {
            WpfMonoGameControl.ResetRenderingNextFrame = true;
        }

        public void RedrawInfo()
        {
            SadConsole.Game.Instance.StartingConsole.Clear();
            SadConsole.Game.Instance.StartingConsole.Print(0, 0, $"RenderRect: {Settings.Rendering.RenderRect}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 1, $"RenderWidth: {Settings.Rendering.RenderWidth}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 2, $"RenderHeight: {Settings.Rendering.RenderHeight}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 3, $"BackbufferWidth: {SadConsole.Host.Global.GraphicsDevice.PresentationParameters.BackBufferWidth}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 4, $"BackbufferHeight: {SadConsole.Host.Global.GraphicsDevice.PresentationParameters.BackBufferHeight}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 5, $"ViewPort: {SadConsole.Host.Global.GraphicsDevice.Viewport.Bounds}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 6, $"ControlSize: {WpfMonoGameControl.ActualWidth},{WpfMonoGameControl.ActualHeight}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 7, $"PreferredBackBufferWidth: {SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferWidth}");
            SadConsole.Game.Instance.StartingConsole.Print(0, 8, $"PreferredBackBufferHeight: {SadConsole.Host.Global.GraphicsDeviceManager.PreferredBackBufferHeight}");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SadConsole.Host.Global.RenderOutput != null)
            {
                WpfMonoGameControl.ResetRenderingNextFrame = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var console = new Console(SadConsole.Host.Global.RenderOutput.Width / Game.Instance.DefaultFont.GlyphWidth, SadConsole.Host.Global.RenderOutput.Height / Game.Instance.DefaultFont.GlyphHeight);
            console.FillWithRandomGarbage(console.Font);

            console.DrawBox(new SadRogue.Primitives.Rectangle(2, 1, 28, 3),
                                                 ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick,
                                                    new ColoredGlyph(SadRogue.Primitives.Color.AnsiCyanBright, SadRogue.Primitives.Color.AnsiCyan),
                                                    new ColoredGlyph(SadRogue.Primitives.Color.AnsiCyanBright, SadRogue.Primitives.Color.AnsiCyan)
                                                 ));

            console.Print(4, 2, "UPDATED");
            SadConsole.Game.Instance.Screen = console;
        }
    }
}
