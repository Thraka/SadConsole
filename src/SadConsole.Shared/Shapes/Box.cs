using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Surfaces;

namespace SadConsole.Shapes
{
    public struct Box: IShape
    {
        public int TopLeftCharacter;
        public int TopRightCharacter;
        public int BottomRightCharacter;
        public int BottomLeftCharacter;
        public int LeftSideCharacter;
        public int RightSideCharacter;
        public int TopSideCharacter;
        public int BottomSideCharacter;

        public Color Foreground;
        public Color BorderBackground;
        public Color FillColor;

        public int Width;
        public int Height;
        public Point Position;
        public bool Fill;
        public bool DrawTransparency;

        public void Draw(SurfaceEditor surface)
        {
            for (int x = Position.X; x < Position.X + Width; x++)
            {
                for (int y = Position.Y; y < Position.Y + Height; y++)
                {
                    // Top row
                    if (y == Position.Y)
                    {
                        if (x == Position.X)
                            PlotCell(surface, x, y, TopLeftCharacter);
                        else if (x == Position.X + Width - 1)
                            PlotCell(surface, x, y, TopRightCharacter);
                        else
                            PlotCell(surface, x, y, TopSideCharacter);
                    }

                    // Bottom row
                    else if (y == Position.Y + Height - 1)
                    {
                        if (x == Position.X)
                            PlotCell(surface, x, y, BottomLeftCharacter);
                        else if (x == Position.X + Width - 1)
                            PlotCell(surface, x, y, BottomRightCharacter);
                        else
                            PlotCell(surface, x, y, BottomSideCharacter);
                    }

                    // Other rows
                    else
                    {
                        if (x == Position.X)
                            PlotCell(surface, x, y, LeftSideCharacter);
                        else if (x == Position.X + Width - 1)
                            PlotCell(surface, x, y, RightSideCharacter);
                        else if (Fill)
                            PlotCell(surface, x, y, 0, Fill);
                    }
                    
                }
            }

            surface.TextSurface.IsDirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        public void SetRegion(Rectangle rectangle)
        {
            Position = rectangle.Location;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public Rectangle GetRegion()
        {
            return new Rectangle(Position.X, Position.Y, Width, Height);
        }

        private void PlotCell(SurfaceEditor surface, int x, int y, int glyph, bool fillMe = false)
        {
            if (surface.IsValidCell(x,y))
            {
                var cell = surface[x, y];

                if (fillMe)
                {
                    cell.Background = FillColor;
                    cell.Foreground = Foreground;
                    cell.Glyph = glyph;
                    return;
                }

                if (Foreground != Color.Transparent || DrawTransparency)
                    cell.Foreground = Foreground;

                if (BorderBackground != Color.Transparent || DrawTransparency)
                    cell.Background = BorderBackground;

                cell.Glyph = glyph;
            }
        }

        /// <summary>
        /// Draws a box from the specified <paramref name="lineStyle"/>.
        /// </summary>
        /// <param name="lineStyle">The array of line styles indexed by <see cref="SurfaceEditor.LineRoadIndex"/>.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        /// <param name="foreground">The foreground color of the box edges.</param>
        /// <param name="background">The background color of the box edges.</param>
        /// <param name="fill">When true fills the inside of the box with the <paramref name="foreground"/> and <paramref name="fillColor"/> as the background.</param>
        /// <param name="fillColor">The background color to fill the box with.</param>
        public Box(int[] lineStyle, int width, int height, Color foreground, Color background, bool fill, Color fillColor):
            this(lineStyle[(int)SurfaceEditor.LineRoadIndex.Left], 
                 lineStyle[(int)SurfaceEditor.LineRoadIndex.Top], 
                 lineStyle[(int)SurfaceEditor.LineRoadIndex.TopLeft], 
                 lineStyle[(int)SurfaceEditor.LineRoadIndex.TopRight],
                 lineStyle[(int)SurfaceEditor.LineRoadIndex.BottomRight],
                 lineStyle[(int)SurfaceEditor.LineRoadIndex.BottomLeft],
                 width, height, foreground, background, fill, fillColor)
        {

        }

        public Box(int sidesCharacter, int topBottomCharacter, int topLeftCharacter, int topRightCharacter, int bottomRightCharacter, int bottomLeftCharacter, int width, int height):
            this(sidesCharacter, topBottomCharacter, topLeftCharacter, topRightCharacter, bottomRightCharacter, bottomLeftCharacter, width, height, Color.White, Color.Transparent, false, Color.Transparent)
        {

        }

        public Box(int sidesCharacter, int topBottomCharacter, int topLeftCharacter, int topRightCharacter, int bottomRightCharacter, int bottomLeftCharacter, int width, int height, Color foreground, Color background, bool fill, Color fillColor)
        {
            TopLeftCharacter = topLeftCharacter;
            TopRightCharacter = topRightCharacter;
            BottomRightCharacter = bottomRightCharacter;
            BottomLeftCharacter = bottomLeftCharacter;
            LeftSideCharacter = sidesCharacter;
            RightSideCharacter = sidesCharacter;
            TopSideCharacter = topBottomCharacter;
            BottomSideCharacter = topBottomCharacter;

            Foreground = foreground;
            BorderBackground = background;

            Width = width;
            Height = height;

            Position = new Point(0, 0);
            Fill = fill;
            FillColor = fillColor;
            DrawTransparency = false;
        }

        [System.Obsolete("Use Thick() instead")]
        public static Box GetDefaultBox()
        {
            Box box = new Box(186, 205, 201, 187, 188, 200, 4, 4);

            return box;
        }

        /// <summary>
        /// Returns a box made up of double lines. Glyphs: 186, 205, 201, 187, 188, 200.
        /// </summary>
        /// <returns>A new 4x4 box object.</returns>
        public static Box Thick()
        {
            return new Box(186, 205, 201, 187, 188, 200, 4, 4);
        }

        /// <summary>
        /// Returns a box made up of single lines. Glyphs: 179, 196, 218, 191, 217, 192.
        /// </summary>
        /// <returns>A new 4x4 box object.</returns>
        public static Box Thin()
        {
            return new Box(179, 196, 218, 191, 217, 192, 4, 4);
        }
    }
}
