using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public abstract class DrawCall
    {
        public abstract void Draw();
    }

    public class DrawCallSurface : DrawCall
    {
        public ISurface Surface;
        public Vector2 Position;

        public DrawCallSurface(ISurface surface, Vector2 position)
        {
            Surface = surface;
            Position = position;
        }

        public override void Draw()
        {
            Global.SpriteBatch.Draw(Surface.LastRenderResult, Position, Color.White);
        }
    }

    public class DrawCallCursor : DrawCall
    {
        public Console Console;
        public Vector2 Position;

        public DrawCallCursor(Console console)
        {
            Console = console;
        }

        public override void Draw()
        {
            int virtualCursorLocationIndex = BasicSurface.GetIndexFromPoint(
                new Point(Console.VirtualCursor.Position.X - Console.TextSurface.RenderArea.Left,
                          Console.VirtualCursor.Position.Y - Console.TextSurface.RenderArea.Top), Console.TextSurface.RenderArea.Width);

            if (virtualCursorLocationIndex >= 0 && virtualCursorLocationIndex < Console.TextSurface.RenderRects.Length)
            {
                var rect = Console.TextSurface.RenderRects[virtualCursorLocationIndex];
                rect.Offset(Console.Position.ConsoleLocationToWorld(Console.TextSurface.Font.Size.X, Console.TextSurface.Font.Size.Y));
                Console.VirtualCursor.Render(Global.SpriteBatch, Console.TextSurface.Font, rect);
            }
        }
    }
}
