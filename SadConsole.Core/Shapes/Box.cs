using Microsoft.Xna.Framework;
using SadConsole.Consoles;

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
        public Point Location;
        public bool Fill;
        public bool DrawTransparency;

        public void Draw(SurfaceEditor surface)
        {
            for (int x = Location.X; x < Location.X + Width; x++)
            {
                for (int y = Location.Y; y < Location.Y + Height; y++)
                {
                    // Top row
                    if (y == Location.Y)
                    {
                        if (x == Location.X)
                            PlotCell(surface, x, y, TopLeftCharacter);
                        else if (x == Location.X + Width - 1)
                            PlotCell(surface, x, y, TopRightCharacter);
                        else
                            PlotCell(surface, x, y, TopSideCharacter);
                    }

                    // Bottom row
                    else if (y == Location.Y + Height - 1)
                    {
                        if (x == Location.X)
                            PlotCell(surface, x, y, BottomLeftCharacter);
                        else if (x == Location.X + Width - 1)
                            PlotCell(surface, x, y, BottomRightCharacter);
                        else
                            PlotCell(surface, x, y, BottomSideCharacter);
                    }

                    // Other rows
                    else
                    {
                        if (x == Location.X)
                            PlotCell(surface, x, y, LeftSideCharacter);
                        else if (x == Location.X + Width - 1)
                            PlotCell(surface, x, y, RightSideCharacter);
                        else if (Fill)
                            PlotCell(surface, x, y, 0, Fill);
                    }
                    
                }
            }

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
                    cell.GlyphIndex = glyph;
                    return;
                }

                if (Foreground != Color.Transparent || DrawTransparency)
                    cell.Foreground = Foreground;

                if (BorderBackground != Color.Transparent || DrawTransparency)
                    cell.Background = BorderBackground;

                cell.GlyphIndex = glyph;
            }
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

            Foreground = Color.White;
            BorderBackground = Color.Transparent;

            Width = width;
            Height = height;

            Location = new Point(0, 0);
            Fill = fill;
            FillColor = fillColor;
            DrawTransparency = false;
        }

        public static Box GetDefaultBox()
        {
            Box box = new Box(186, 205, 201, 187, 188, 200, 4, 4);

            return box;
        }
    }
}
