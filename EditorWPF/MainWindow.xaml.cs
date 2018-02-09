using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SadConsole;
using System;
using System.Collections.Generic;
using System.IO;
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
using WpfWindow = System.Windows.Window;
using PointSys = System.Windows.Point;

namespace SadConsole.Editor
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WpfWindow
    {
        EditorDataContext dataContext = new EditorDataContext();



        public SadConsole.Surfaces.ISurface SelectedSurface { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TreeScreens.Items.Add(CreateNode(SadConsole.Global.CurrentScreen));
        }

        private TreeViewItem CreateNode(IScreen screen)
        {
            TreeViewItem node = new TreeViewItem() { Header = screen.GetType().ToString(), Tag = screen };

            foreach (var child in screen.Children)
            {
                node.Items.Add(CreateNode(child));
            }

            return node;
        }

        private void TreeScreens_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeScreens.SelectedItem == null)
            {
                dataContext.Surface = null;
                //ImageSurface.Source = null;
                return;
            }

            IScreen screen = ((TreeViewItem)TreeScreens.SelectedItem).Tag as IScreen;

            if (screen is SadConsole.Console)
            {
                dataContext.Surface = ((SadConsole.Console)screen).TextSurface;
                dataContext.Editor = new SadConsole.Surfaces.SurfaceEditor(dataContext.Surface);
                RefreshImage();
            }
        }

        private void RefreshImage()
        {
            MemoryStream memoryStream = new MemoryStream();
            var texture = dataContext.Surface.LastRenderResult;
            texture.SaveAsPng(memoryStream, texture.Width, texture.Height); //Or SaveAsPng( memoryStream, texture.Width, texture.Height )
            memoryStream.Position = 0;
            BitmapImage initialImage = new BitmapImage();
            initialImage.BeginInit();
            initialImage.StreamSource = memoryStream;
            initialImage.EndInit();
            WriteableBitmap img = new WriteableBitmap(initialImage);
            //ImageSurface.Source = img;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TreeScreens.Items.Clear();
            TreeScreens.Items.Add(CreateNode(SadConsole.Global.CurrentScreen));
        }
        
        private void ImageSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var point = dataContext.MousePosition;
                Microsoft.Xna.Framework.Point sadPoint = PointExtensions.PixelLocationToConsole(new Microsoft.Xna.Framework.Point((int)point.X, (int)point.Y), dataContext.Surface.Font);
                dataContext.Editor.SetBackground(sadPoint.X, sadPoint.Y, Microsoft.Xna.Framework.Color.BlueViolet);
                //RefreshImage();
            }
        }
    }

    public static class Editor
    {
        private static MainWindow form;

        public static void ShowEditor()
        {
            if (form == null)
                form = new MainWindow();

            form.Show();
        }

        //public static void StartApp()
        //{
        //    SadConsole.Editor.App.Main();
        //}

        public static void StartApp(string font, int width, int height, Action init, Action<GameTime> drawFrame, Action<GameTime> update)
        {
            SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Center;
            SadConsole.Game.WpfFont = font;
            SadConsole.Game.WpfConsoleWidth = width;
            SadConsole.Game.WpfConsoleHeight = height;
            SadConsole.Game.OnInitialize = init;
            SadConsole.Game.OnUpdate = update;
            SadConsole.Game.OnDraw = drawFrame;

            SadConsole.Editor.App.Main();
        }

        public static void InputOverride()
        {
            //Global.KeyboardState.Update(gameTime);
            //Global.KeyboardState.Process();
            //Global.MouseState.Update(gameTime);
            //Global.MouseState.Process();
        }
    }

    
    //public class MyGame : WpfGame
    //{
    //    private IGraphicsDeviceService _graphicsDeviceManager;
    //    private WpfKeyboard _keyboard;
    //    private WpfMouse _mouse;
        
    //    protected override void Initialize()
    //    {
    //        // must be initialized. required by Content loading and rendering (will add itself to the Services)
    //        _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            
    //        // wpf and keyboard need reference to the host control in order to receive input
    //        // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
    //        _keyboard = new WpfKeyboard(this);
    //        _mouse = new WpfMouse(this);

    //        // must be called after the WpfGraphicsDeviceService instance was created
    //        base.Initialize();
            
    //        // content loading now possible
    //    }

    //    protected override void Update(GameTime time)
    //    {
    //        // every update we can now query the keyboard & mouse for our WpfGame
    //        var mouseState = _mouse.GetState();
    //        var keyboardState = _keyboard.GetState();
    //    }

    //    protected override void Draw(GameTime time)
    //    {
    //        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
    //    }
    //}
}
