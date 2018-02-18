using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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



        private Tools.ITool selectedTool;
        private IScreen selectedScreen;
        private System.Windows.Forms.Control selectedToolPanel;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Tools.ITool> Tools { get; set; }

        public Tools.ITool SelectedTool
        {
            get => selectedTool;
            set
            {
                selectedTool = value;
                selectedToolPanel = selectedTool.GetUI();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedToolPanel"));
            }
        }

        public System.Windows.Forms.Control SelectedToolPanel => selectedToolPanel;

        public IScreen Screen
        {
            get => selectedScreen;
            set
            {
                if (selectedScreen != value)
                {
                    selectedScreen = value;

                    // REFRESH
                }
            }
        }
        
        public DataContext()
        {
            Tools = new List<Tools.ITool>() { new Tools.Pencil(), new Tools.Box(), new Tools.Recolor() };

            selectedTool = Tools[2];
        }
    }

}
