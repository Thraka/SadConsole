using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public interface IDrawCall
    {
        void Draw();
    }

    public class DrawCallSurface : IDrawCall
    {
        public ISurface Surface;
        public Vector2 Position;

        public DrawCallSurface(ISurface surface, Point position, bool pixelPositioned)
        {
            if (pixelPositioned)
                Position = position.ToVector2();
            else
                Position = surface.Font.GetWorldPosition(position).ToVector2();

            Surface = surface;
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Surface.LastRenderResult, Position, Color.White);
        }
    }

    public class DrawCallTexture : IDrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;

        public DrawCallTexture(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Texture, Position, Color.White);
        }
    }

    public class DrawCallCustom : IDrawCall
    {
        public Action DrawCallback;

        public DrawCallCustom(Action draw) { DrawCallback = draw; }

        public void Draw()
        {
            DrawCallback();
        }
    }

    public class DrawCallColoredRect : IDrawCall
    {
        public Rectangle Rectangle;
        public Color Shade;

        public DrawCallColoredRect(Rectangle rectangle, Color shade)
        {
            Rectangle = rectangle;
            Shade = shade;
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Global.FontDefault.FontImage, Rectangle, Global.FontDefault.SolidGlyphRectangle, Shade);
        }
    }

    public class DrawCallCursor : IDrawCall
    {
        public Console Console;
        public Vector2 Position;

        public DrawCallCursor(Console console)
        {
            Console = console;
        }

        public void Draw()
        {
            int virtualCursorLocationIndex = BasicSurface.GetIndexFromPoint(
                new Point(Console.VirtualCursor.Position.X - Console.TextSurface.RenderArea.Left,
                          Console.VirtualCursor.Position.Y - Console.TextSurface.RenderArea.Top), Console.TextSurface.RenderArea.Width);

            if (virtualCursorLocationIndex >= 0 && virtualCursorLocationIndex < Console.TextSurface.RenderRects.Length)
            {
                var rect = Console.TextSurface.RenderRects[virtualCursorLocationIndex];
                rect.Offset(Console.Position.ConsoleLocationToPixel(Console.TextSurface.Font.Size.X, Console.TextSurface.Font.Size.Y));
                Console.VirtualCursor.Render(Global.SpriteBatch, Console.TextSurface.Font, rect);
            }
        }
    }
}
