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
        private ITool selectedTool;
        private IScreen selectedScreen;
        private System.Windows.Forms.Control selectedToolPanel;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ITool> Tools { get; set; }

        public ITool SelectedTool
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
            Tools = new List<ITool>() { new Pencil(), new Box(), new Recolor() };

            selectedTool = Tools[2];
        }
    }

    internal interface ITool
    {
        string Name { get; }

        System.Windows.Forms.Control GetUI();
    }

    internal class Pencil : ITool
    {
        public Model.GlyphItem Glyph = new Model.GlyphItem();

        public string Name => "Pencil";

        public System.Windows.Forms.Control GetUI()
        {
            var panel = new System.Windows.Forms.Panel();
            ToolControls.GlyphEditPanel.SharedInstance.DataObject = Glyph;
            panel.AddArrangeControls(ToolControls.GlyphEditPanel.SharedInstance, new System.Windows.Forms.Button(), new System.Windows.Forms.Button());
            return panel;
        }
    }

    internal class Box : ITool
    {
        public string Name => "Box";

        public System.Windows.Forms.Control GetUI()
        {
            return new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Box", Name = "ToolPanel" };
        }
    }

    internal class Recolor : ITool
    {
        public string Name => "Recolor";

        public System.Windows.Forms.Control GetUI()
        {
            return new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Recolor", Name = "ToolPanel" };
        }
    }
}
