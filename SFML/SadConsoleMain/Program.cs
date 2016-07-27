using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleMain
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new SFML.Graphics.RenderWindow(new SFML.Window.VideoMode(400, 500), "Test", SFML.Window.Styles.Default);

            window.Closed += Window_Closed;
            window.SetFramerateLimit(10);


            var fontMaster = LoadFont("IBM.font");
            var font = fontMaster.GetFont(SadConsole.Font.FontSizes.One);
            var surface = SadConsole.Engine.Initialize(window, "IBM.font", 20, 20);
            surface.TextSurface.DefaultBackground = SFML.Graphics.Color.Yellow;
            surface.Fill(SFML.Graphics.Color.Green, null, 1, null);
            surface.TextSurface.Font = font = fontMaster.GetFont(SadConsole.Font.FontSizes.Two);
            SadConsole.Consoles.TextSurfaceRenderer renderer = new SadConsole.Consoles.TextSurfaceRenderer();
            surface.Position = new SFML.System.Vector2i(1, 1);
            font.ResizeGraphicsDeviceManager(window, 20, 20, 0, 0);
            //batch.Update(surface);
            
            while (window.IsOpen)
            {
                window.Clear(SFML.Graphics.Color.Black);

                //renderer.Render(surface.TextSurface, new SFML.System.Vector2i(2, 2));
                SadConsole.Engine.Draw(new SadConsole.GameTime());

                window.Display();

                window.DispatchEvents();
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            ((SFML.Window.Window)sender).Close();
        }

        public static SadConsole.FontMaster LoadFont(string font)
        {
            if (!System.IO.File.Exists(font))
                throw new Exception($"Font does not exist: {font}");

            using (var stream = System.IO.File.OpenRead(font))
            {
                var masterFont = SadConsole.Serializer.Deserialize<SadConsole.FontMaster>(stream);

                return masterFont;
            }
        }
    }

    
}
