#if MONOGAME
using Microsoft.Xna.Framework;
#elif SFML
using SFML.Graphics;
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using SFML.System;
#endif

using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterProject.CustomConsoles
{
    class HexConsole : SadConsole.Consoles.Console
    {
        int lastCell = -1;
        bool lastCellHexRow = false;

        public HexConsole(int width, int height) : base(new HexTextSurface(width, height))
        {
            Position = new Point(3, 2);
            textSurface.Font = Engine.LoadFont("Cheepicus12.font").GetFont(Font.FontSizes.One);

            Fill(Color.Red, null, glyph: 176);

            for (int x = 0; x < width; x++)
            {
                bool isHexColumn = x % 2 == 1;

                for (int y = 0; y < height; y++)
                {
                    bool isHexRow = y % 2 == 1;

                    if (isHexColumn && isHexRow)
                    {
                        SetForeground(x, y, Color.Blue * 0.6f);
                    }
                    else if (isHexRow)
                    {
                        SetForeground(x, y, Color.AliceBlue * 0.6f);
                    }
                    else if (isHexColumn)
                    {
                        SetForeground(x, y, Color.DarkRed);
                    }
                }
            }
        }

        public override bool ProcessMouse(MouseInfo info)
        {
            var worldLocation = info.ScreenLocation.WorldLocationToConsole(textSurface.Font.Size.X, textSurface.Font.Size.Y);
            var consoleLocation = new Point(worldLocation.X - _position.X, worldLocation.Y - _position.Y);

            // Check if mouse is within the upper/lower bounds of the console
            if (info.ConsoleLocation.Y >= 0 && info.ConsoleLocation.Y <= textSurface.RenderArea.Height - 1)
            {
                bool isHexRow = info.ConsoleLocation.Y % 2 == 1;
                // Check if mouse is on an alternating row
                if (isHexRow)
                    info.ScreenLocation.X -= textSurface.Font.Size.X / 2;

                info.Fill(this);

                if (info.Console == this)
                {
                    FillHexes(lastCell, 176, lastCellHexRow);
                    lastCell = info.ConsoleLocation.ToIndex(textSurface.Width);
                    lastCellHexRow = isHexRow;
                    FillHexes(lastCell, 45, isHexRow);

                    return true;
                }
            }

            //base.ProcessMouse(info);

            FillHexes(lastCell, 176, lastCellHexRow);

            return true;
        }

        public override void Update()
        {
            base.Update();
        }

        private void FillHexes(int index, int glyphIndex, bool isHexRow)
        {
            SetGlyph(GetHexCellIndex(HexDirection.TopLeft, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.TopRight, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.Left, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.Right, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.BottomLeft, index, isHexRow), glyphIndex);
            SetGlyph(GetHexCellIndex(HexDirection.BottomRight, index, isHexRow), glyphIndex);

            if (index >= 0 && index < textSurface.Cells.Length)
                SetGlyph(index, glyphIndex);
        }

        private void SetGlyph(int cellIndex, int glyphIndex)
        {
            if (cellIndex != -1)
                textSurface[cellIndex].GlyphIndex = glyphIndex;
        }

        public int GetHexCellIndex(HexDirection direction, int sourceHex, bool isHexRow)
        {
            int returnHex = -1;

            switch (direction)
            {
                case HexDirection.TopLeft:
                    returnHex = isHexRow ? sourceHex - textSurface.Width : sourceHex - textSurface.Width - 1;
                    break;
                case HexDirection.TopRight:
                    returnHex = isHexRow ? sourceHex - textSurface.Width + 1 : sourceHex - textSurface.Width ;
                    break;
                case HexDirection.Left:
                    returnHex = sourceHex - 1;
                    break;
                case HexDirection.Right:
                    returnHex = sourceHex + 1;
                    break;
                case HexDirection.BottomLeft:
                    returnHex = isHexRow ? sourceHex + textSurface.Width : sourceHex + textSurface.Width - 1;
                    break;
                case HexDirection.BottomRight:
                    returnHex = isHexRow ? sourceHex + textSurface.Width + 1 : sourceHex + textSurface.Width;
                    break;
                default:
                    break;
            }

            if (returnHex < 0 || returnHex > textSurface.Cells.Length - 1)
                returnHex = -1;

            return returnHex;
        }

        public enum HexDirection
        {
            TopLeft,
            TopRight,
            Left,
            Right,
            BottomLeft,
            BottomRight
        }

    }

    class HexTextSurface : SadConsole.Consoles.TextSurface
    {
        public HexTextSurface(int width, int height) : base(width, height) { }

        protected override void ResetArea()
        {
            base.ResetArea();

            // After the render rects are calculated, shift every other row
            for (int y = 1; y < area.Height; y += 2)
            {
                for (int x = 0; x < area.Width; x++)
                {
#if MONOGAME
                    RenderRects[GetIndexFromPoint(x, y, area.Width)].Offset(font.Size.X / 2, 0);
#elif SFML
                    RenderRects[GetIndexFromPoint(x, y, area.Width)].Left += font.Size.X / 2;
                    RenderRects[GetIndexFromPoint(x, y, area.Width)].Width += font.Size.X / 2;
#endif
                }
            }
        }
    }
}
