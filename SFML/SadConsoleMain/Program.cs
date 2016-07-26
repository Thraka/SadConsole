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


            SFML.Graphics.Sprite sprite = new SFML.Graphics.Sprite(font.FontImage);

            var surface = new SadConsole.Consoles.TextSurface(10, 10, font);
            surface[50].GlyphIndex = 1;
            surface[10].GlyphIndex = 1;
            surface[1].GlyphIndex = 2;
            surface[11].GlyphIndex = 3;
            surface[2].GlyphIndex = 4;
            surface[12].GlyphIndex = 5;

            SFML.Graphics.SpriteBatch batch = new SFML.Graphics.SpriteBatch();
            batch.Update(surface);

            sprite.Position = new SFML.System.Vector2f(32f, 1f);
            sprite.TextureRect = new SFML.Graphics.IntRect(10, 0, 8, 16);
            sprite.Color = new SFML.Graphics.Color(255, 255, 0, 100);
            
            while (window.IsOpen)
            {
                window.Clear(SFML.Graphics.Color.Black);
                

                window.Draw(batch);




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
