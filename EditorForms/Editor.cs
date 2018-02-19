using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace SadConsole.Editor
{
    public static class Editor
    {
        public static void StartApp(string font, int width, int height, Action init, Action<GameTime> drawFrame, Action<GameTime> update)
        {
            SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Center;
            SadConsole.Game.WpfFont = font;
            SadConsole.Game.WpfConsoleWidth = width;
            SadConsole.Game.WpfConsoleHeight = height;
            SadConsole.Game.OnInitialize = init;
            SadConsole.Game.OnUpdate = update;
            SadConsole.Game.OnDraw = drawFrame;

            //SadConsole.Editor.App.Main();
            System.Windows.Forms.Application.EnableVisualStyles();
            
            Form1 form = new Form1();
            form.ShowDialog();
            form.Dispose();
        }


        static Editor()
        {
        }
    }

    internal class DataContext: System.ComponentModel.INotifyPropertyChanged
    {
        public static DataContext Instance = new DataContext();
        public bool IsEditMode;
        public bool PauseEditMode;

        private Tools.ITool selectedTool;
        private IScreen selectedScreen;
        private System.Windows.Forms.Control selectedToolPanel;
        private Dictionary<Font, Surfaces.BasicSurface> FontSurfaces = new Dictionary<Font, Surfaces.BasicSurface>();

        private IScreen OriginalScreen;
        private IScreen oldParent;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Tools.ITool> Tools { get; set; }

        public Tools.ITool SelectedTool
        {
            get => selectedTool;
            set
            {
                selectedTool = value;
                selectedToolPanel = selectedTool.GetUI();
                NotifyProperty();
            }
        }

        public Font SelectedFont = Global.FontDefault;

        public System.Windows.Forms.Control SelectedToolPanel => selectedToolPanel;

        public IScreen Screen
        {
            get => selectedScreen;
            set
            {
                if (selectedScreen != value || !SadConsole.Global.CurrentScreen.Children.Contains(value))
                {
                    selectedScreen = value;

                    if (IsEditMode && value != null)
                    {
                        if (SadConsole.Global.CurrentScreen.Children.Count != 0)
                        {
                            var removed = SadConsole.Global.CurrentScreen.Children[0];
                            removed.Parent = oldParent;
                        }
                        SadConsole.Global.CurrentScreen.Children.Clear();
                        oldParent = value.Parent;
                        SadConsole.Global.CurrentScreen.Children.Add(value);
                    }
                }
            }
        }
        
        private void NotifyProperty([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Surfaces.BasicSurface GetFontSurface(Font font)
        {
            if (!FontSurfaces.ContainsKey(font))
            {
                var surface = new Surfaces.BasicSurface(16, font.Rows, font);
                for (int i = 0; i < surface.Cells.Length; i++)
                {
                    surface.Cells[i].Glyph = i;
                    surface.Cells[i].Background = Color.Transparent;
                }
                FontSurfaces[font] = surface;
                return surface;
            }
            else
                return FontSurfaces[font];
        }

        public DataContext()
        {
            Tools = new List<Tools.ITool>() { new Tools.Pencil(), new Tools.Box(), new Tools.Recolor() };

            selectedTool = Tools[2];
        }



        public void EnableEditMode()
        {
            SadConsole.Settings.DoUpdate = false;
            OriginalScreen = SadConsole.Global.CurrentScreen;
            SadConsole.Global.CurrentScreen = new Screen();
            IsEditMode = true;
        }

        public void DisableEditMode()
        {
            SadConsole.Settings.DoUpdate = true;
            SadConsole.Global.CurrentScreen.Children.Clear();
            SadConsole.Global.CurrentScreen = OriginalScreen;
            IsEditMode = false;
        }

        public void Update(GameTime gameTime)
        {
            // Process our editor events instead
            Global.MouseState.Update(gameTime);
            MouseConsoleState mouseConsoleState = new MouseConsoleState(null, Global.MouseState);

            // Scan through each "console" in the current screen, including children.
            if (Global.CurrentScreen != null)
            {
                // Build a list of all consoles
                var consoles = new List<IConsole>();

                // Inline code for GetConsoles
                void GetConsoles(IScreen screen, ref List<IConsole> list)
                {
                    if (screen is IConsole)
                        list.Add((IConsole)screen);

                    foreach (var child in screen.Children)
                    {
                        GetConsoles(child, ref list);
                    }
                }

                GetConsoles(Global.CurrentScreen, ref consoles);

                // Process top-most consoles first.
                consoles.Reverse();

                for (int i = 0; i < consoles.Count; i++)
                {
                    mouseConsoleState = new MouseConsoleState(consoles[i], Global.MouseState);

                    if (mouseConsoleState.IsOnConsole)
                        break;
                }
            }

            // Process tool
            SelectedTool.OnUpdate(mouseConsoleState);
        }
    }

}
