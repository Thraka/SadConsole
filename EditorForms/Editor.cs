using System;
using System.Collections.Generic;
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
    }
}
