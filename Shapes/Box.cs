using Microsoft.Xna.Framework;

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

        public void Draw(CellSurface surface)
        {
            // box is on the console area
            if (Location.X < surface.Width && Location.Y < surface.Height)
            {
                // box is on the console area
                if (Location.X + Width > 0 && Location.Y + Height > 0)
                {
                    int workingWidth;
                    int workingHeight;
                    int workingX;
                    int workingY;

                    // Flags for drawing sides of the box
                    bool drawLeft = Location.X >= 0 && Location.X < surface.Width;
                    bool drawRight = Location.X + Width >= 0 && Location.X + Width <= surface.Width;
                    bool drawTop = Location.Y >= 0 && Location.Y < surface.Height;
                    bool drawBottom = Location.Y + Height >= 0 && Location.Y + Height <= surface.Height;

                    // Predict X and Y start.
                    if (drawLeft)
                        workingX = Location.X;
                    else
                        workingX = 0;

                    if (drawTop)
                        workingY = Location.Y;
                    else
                        workingY = 0;

                    // Predict the width and height.
                    if (workingX + Width <= surface.Width)
                        workingWidth = Width;
                    else
                        workingWidth = surface.Width - workingX;

                    if (workingY + Height <= surface.Height)
                        workingHeight = Height;
                    else
                        workingHeight = surface.Height - workingY;

                    // Fill
                    if (Fill)
                        surface.FillArea(new Rectangle(workingX, workingY, workingWidth, workingHeight), Foreground, FillColor, 0, null);

                    // Drawing top\bottom
                    if (drawTop && drawBottom)
                    {
                        int topStartingIndex = surface.GetIndexFromPoint(workingX, workingY);
                        int bottomStartingIndex = topStartingIndex + (surface.Width * (workingHeight - 1));

                        for (int x = 0; x < workingWidth; x++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                            {
                                surface[topStartingIndex + x].Foreground = Foreground;
                                surface[bottomStartingIndex + x].Foreground = Foreground;
                            }
                            if (BorderBackground != Color.Transparent || DrawTransparency)
                            {
                                surface[topStartingIndex + x].Background = BorderBackground;
                                surface[bottomStartingIndex + x].Background = BorderBackground;
                            }

                            surface[topStartingIndex + x].CharacterIndex = TopSideCharacter;
                            surface[bottomStartingIndex + x].CharacterIndex = BottomSideCharacter;
                        }
                    }
                    else if (drawTop)
                    {
                        int topStartingIndex = surface.GetIndexFromPoint(workingX, workingY);

                        for (int x = 0; x < workingWidth; x++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                                surface[topStartingIndex + x].Foreground = Foreground;

                            if (BorderBackground != Color.Transparent || DrawTransparency)
                                surface[topStartingIndex + x].Background = BorderBackground;

                            surface[topStartingIndex + x].CharacterIndex = TopSideCharacter;
                        }
                    }
                    else
                    {
                        int bottomStartingIndex = surface.GetIndexFromPoint(workingX, workingY + workingHeight - 1);

                        for (int x = 0; x < workingWidth; x++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                                surface[bottomStartingIndex + x].Foreground = Foreground;

                            if (BorderBackground != Color.Transparent || DrawTransparency)
                                surface[bottomStartingIndex + x].Background = BorderBackground;

                            surface[bottomStartingIndex + x].CharacterIndex = BottomSideCharacter;
                        }
                    }

                    // Draw left\right
                    if (drawLeft && drawRight)
                    {
                        int leftStartingIndex = surface.GetIndexFromPoint(workingX, workingY);
                        int rightStartingIndex = leftStartingIndex + workingWidth - 1;

                        for (int y = 0; y < workingHeight; y++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                            {
                                surface[leftStartingIndex + (y * surface.Width)].Foreground = Foreground;
                                surface[rightStartingIndex + (y * surface.Width)].Foreground = Foreground;
                            }
                            if (BorderBackground != Color.Transparent || DrawTransparency)
                            {
                                surface[leftStartingIndex + (y * surface.Width)].Background = BorderBackground;
                                surface[rightStartingIndex + (y * surface.Width)].Background = BorderBackground;
                            }

                            surface[leftStartingIndex + (y * surface.Width)].CharacterIndex = LeftSideCharacter;
                            surface[rightStartingIndex + (y * surface.Width)].CharacterIndex = RightSideCharacter;
                        }
                    }
                    else if (drawLeft)
                    {
                        int leftStartingIndex = surface.GetIndexFromPoint(workingX, workingY);

                        for (int y = 0; y < workingHeight; y++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                                surface[leftStartingIndex + (y * surface.Width)].Foreground = Foreground;

                            if (BorderBackground != Color.Transparent || DrawTransparency)
                                surface[leftStartingIndex + (y * surface.Width)].Background = BorderBackground;

                            surface[leftStartingIndex + (y * surface.Width)].CharacterIndex = LeftSideCharacter;
                        }
                    }
                    else
                    {
                        int rightStartingIndex = surface.GetIndexFromPoint(workingX + workingWidth - 1, workingY);

                        for (int y = 0; y < workingHeight; y++)
                        {
                            if (Foreground != Color.Transparent || DrawTransparency)
                                surface[rightStartingIndex + (y * surface.Width)].Foreground = Foreground;

                            if (BorderBackground != Color.Transparent || DrawTransparency)
                                surface[rightStartingIndex + (y * surface.Width)].Background = BorderBackground;

                            surface[rightStartingIndex + (y * surface.Width)].CharacterIndex = RightSideCharacter;
                        }
                    }

                    // Corners
                    if (drawTop && drawLeft)
                    {
                        int index = surface.GetIndexFromPoint(workingX, workingY);

                        if (Foreground != Color.Transparent || DrawTransparency)
                            surface[index].Foreground = Foreground;

                        if (BorderBackground != Color.Transparent || DrawTransparency)
                            surface[index].Background = BorderBackground;

                        surface[index].CharacterIndex = TopLeftCharacter;
                    }

                    if (drawTop && drawRight)
                    {
                        int index = surface.GetIndexFromPoint(workingX + workingWidth - 1, workingY);

                        if (Foreground != Color.Transparent || DrawTransparency)
                            surface[index].Foreground = Foreground;

                        if (BorderBackground != Color.Transparent || DrawTransparency)
                            surface[index].Background = BorderBackground;

                        surface[index].CharacterIndex = TopRightCharacter;
                    }

                    if (drawBottom && drawLeft)
                    {
                        int index = surface.GetIndexFromPoint(workingX, workingY + workingHeight - 1);

                        if (Foreground != Color.Transparent || DrawTransparency)
                            surface[index].Foreground = Foreground;

                        if (BorderBackground != Color.Transparent || DrawTransparency)
                            surface[index].Background = BorderBackground;

                        surface[index].CharacterIndex = BottomLeftCharacter;
                    }

                    if (drawBottom && drawRight)
                    {
                        int index = surface.GetIndexFromPoint(workingX + workingWidth - 1, workingY + workingHeight - 1);

                        if (Foreground != Color.Transparent || DrawTransparency)
                            surface[index].Foreground = Foreground;

                        if (BorderBackground != Color.Transparent || DrawTransparency)
                            surface[index].Background = BorderBackground;

                        surface[index].CharacterIndex = BottomRightCharacter;
                    }
                }
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
