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
            
            SFML.Graphics.Texture texture = new SFML.Graphics.Texture("IBM8x16.png");

            SFML.Graphics.Sprite sprite = new SFML.Graphics.Sprite(texture);
            sprite.Position = new SFML.System.Vector2f(32f, 1f);
            sprite.TextureRect = new SFML.Graphics.IntRect(10, 0, 8, 16);
            sprite.Color = new SFML.Graphics.Color(255, 255, 0, 100);
            
            while (window.IsOpen)
            {
                window.Clear(SFML.Graphics.Color.Black);

                sprite.Draw(window, SFML.Graphics.RenderStates.Default);

                window.Display();

                window.DispatchEvents();
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            ((SFML.Window.Window)sender).Close();
        }
    }
}
