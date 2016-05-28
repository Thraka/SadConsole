using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Represents all the basic information about console text and methods to manipulate that text.
    /// </summary>
    public class TextSurface : TextSurfaceBase
    {
        protected Rectangle area;

        #region Properties
        public Rectangle ViewArea
        {
            get { return area; }
            set
            {
                //if (area.Width > data.ViewArea.Width)
                //    throw new ArgumentOutOfRangeException("area", "The area is too wide for the surface.");
                //if (area.Height > data.ViewArea.Height)
                //    throw new ArgumentOutOfRangeException("area", "The area is too tall for the surface.");

                //if (area.X < 0)
                //    throw new ArgumentOutOfRangeException("area", "The left of the area cannot be less than 0.");
                //if (area.Y < 0)
                //    throw new ArgumentOutOfRangeException("area", "The top of the area cannot be less than 0.");

                //if (area.X + area.Width > data.ViewArea.Width)
                //    throw new ArgumentOutOfRangeException("area", "The area x + width is too wide for the surface.");
                //if (area.Y + area.Height > data.ViewArea.Height)
                //    throw new ArgumentOutOfRangeException("area", "The area y + height is too tal for the surface.");

                area = value;

                if (area.Width > base.Width)
                    area.Width = base.Width;
                if (area.Height > base.Height)
                    area.Height = base.Height;

                if (area.X < 0)
                    area.X = 0;
                if (area.Y < 0)
                    area.Y = 0;

                if (area.X + area.Width > base.Width)
                    area.X = base.Width - area.Width;
                if (area.Y + area.Height > base.Height)
                    area.Y = base.Height - area.Height;

                ResetArea();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new cell surface that can be resized and also have its cells resized.
        /// </summary>
        /// <remarks>You must set the Font property before rendering this cell surface.</remarks>
        //public ConsoleData() : this(1, 1, Engine.DefaultFont) { }

        public TextSurface(int width, int height) : this(width, height, Engine.DefaultFont) { }

        public TextSurface(int width, int height, Font font)
        {
            DefaultBackground = Color.Transparent;
            DefaultForeground = Color.White;
            base.font = font;
            base.width = width;
            base.height = height;
            area = new Rectangle(0, 0, width, height);

            InitializeCells();
        }

        protected TextSurface() { }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the cells. This method caches all of the rendering points and rectangles and initializes each cell.
        /// </summary>
        /// <param name="oldWidth">The old size of the surface in width. Used when resizing to preserve existing cells.</param>
        protected virtual void InitializeCells()
        {
            base.cells = new Cell[base.width * base.height];
            base.renderCells = cells;

            for (int i = 0; i < base.cells.Length; i++)
            {
                base.cells[i] = new Cell();
                base.cells[i].Foreground = this.DefaultForeground;
                base.cells[i].Background = this.DefaultBackground;
                base.cells[i].OnCreated();
            }

            // Setup the new render area
            ResetArea();
        }

        /// <summary>
        /// Keeps the text view data in sync with this surface.
        /// </summary>
        protected virtual void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            renderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    renderCells[index] = base.cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets each background of a cell to the array of colors. Must be the same length as this cell surface.
        /// </summary>
        /// <param name="pixels">The colors to place.</param>
        public void SetPixels(Color[] pixels)
        {
            if (pixels.Length != cells.Length)
                throw new ArgumentOutOfRangeException("pixels", "The amount of colors do not match the size of this cell surface.");

            for (int i = 0; i < pixels.Length; i++)
                cells[i].Background = pixels[i];
        }

        #region IsValidCell
        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < this.width && y >= 0 && y < this.height;
        }

        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <param name="index">If the cell is valid, the index of the cell when found.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public bool IsValidCell(int x, int y, out int index)
        {
            if (x >= 0 && x < this.width && y >= 0 && y < this.height)
            {
                index = y * width + x;
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        /// <summary>
        /// Tests if a cell is valid based on its index.
        /// </summary>
        /// <param name="index">The index to test.</param>
        /// <returns>A true value indicating the cell index is in this cell surface.</returns>
        public bool IsValidCell(int index)
        {
            return index >= 0 && index < cells.Length;
        }
        #endregion

        #region Copy
        /// <summary>
        /// Copies the contents of this cell surface to the destination.
        /// </summary>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="destination">The destination surface.</param>
        public void Copy(TextSurface destination)
        {
            int maxX = this.width >= destination.width ? destination.width : this.width;
            int maxY = this.height >= destination.height ? destination.height : this.height;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (IsValidCell(x, y) && destination.IsValidCell(x, y))
                    {
                        var sourceCell = this[x, y];
                        var desCell = destination[x, y];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface to the destination at the specified x,y.
        /// </summary>
        /// <param name="x">The x coordinate of the destination.</param>
        /// <param name="y">The y coordinate of the destination.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(TextSurface destination, int x, int y)
        {
            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    var sourceCell = this[curx, cury];

                    if (destination.IsValidCell(x + curx, y + cury))
                    {
                        var desCell = destination[x + curx, y + cury];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(int x, int y, TextSurface destination)
        {
            int maxX = this.width - x >= destination.width ? destination.width : this.width;
            int maxY = this.height - y >= destination.height ? destination.height : this.height;

            int destX = 0;
            int destY = 0;

            for (int curx = x; curx < maxX; curx++)
            {
                for (int cury = y; cury < maxY; cury++)
                {
                    if (IsValidCell(curx, cury) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx, cury];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                        destY++;
                    }
                }
                destY = 0;
                destX++;
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified width and height.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The width to copy from.</param>
        /// <param name="height">The height to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(int x, int y, int width, int height, TextSurface destination)
        {
            int maxX = width > destination.width ? destination.width : x + width;
            int maxY = height > destination.height ? destination.height : y + height;

            int destX = 0;
            int destY = 0;

            for (int curx = 0; curx < maxX; curx++)
            {
                for (int cury = 0; cury < maxY; cury++)
                {
                    if (IsValidCell(curx + x, cury + y) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx + x, cury + y];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                        destY++;
                    }
                }
                destY = 0;
                destX++;
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination, only with the specified width and height, and copies it to the specified <paramref name="destinationX"/> and <paramref name="destinationY"/> position.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="width">The width to copy from.</param>
        /// <param name="height">The height to copy from.</param>
        /// <param name="destination">The destination surface.</param>
        /// <param name="destinationX">The x coordinate to copy to.</param>
        /// <param name="destinationY">The y coordinate to copy to.</param>
        public void Copy(int x, int y, int width, int height, TextSurface destination, int destinationX, int destinationY)
        {
            int destX = destinationX;
            int destY = destinationY;

            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    if (IsValidCell(curx + x, cury + y) && destination.IsValidCell(destX, destY))
                    {
                        var sourceCell = this[curx + x, cury + y];
                        var desCell = destination[destX, destY];
                        sourceCell.CopyAppearanceTo(desCell);
                        desCell.Effect = sourceCell.Effect;
                    }
                    destY++;
                }
                destY = destinationY;
                destX++;
            }
        }
        #endregion

        #region Cell Manipulation

        #region Cell Specifics
        /// <summary>
        /// Changes the character of a specified cell to a new value.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="character">The desired character of the cell.</param>
        public void SetCharacter(int x, int y, int character)
        {
            cells[y * width + x].CharacterIndex = character;
        }
        /// <summary>
        /// Changes the character, foreground, and background of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="character">The desired character.</param>
        /// <param name="foreground">The desired foreground.</param>
        public void SetCharacter(int x, int y, int character, Color foreground)
        {
            int index = y * width + x;

            cells[index].Foreground = foreground;
            cells[index].CharacterIndex = character;
        }
        /// <summary>
        /// Changes the character, foreground, and background of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="character">The desired character.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        public void SetCharacter(int x, int y, int character, Color foreground, Color background)
        {
            int index = y * width + x;

            cells[index].Background = background;
            cells[index].Foreground = foreground;
            cells[index].CharacterIndex = character;
        }

        /// <summary>
        /// Changes the character, foreground, background, and effect of a cell.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="character"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="effect"></param>
        public void SetCharacter(int x, int y, int character, Color foreground, Color background, ICellEffect effect)
        {
            int index = y * width + x;

            cells[index].Background = background;
            cells[index].Foreground = foreground;
            cells[index].CharacterIndex = character;
            cells[index].Effect = effect;
        }

        /// <summary>
        /// Gets the character of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The character.</returns>
        public int GetCharacter(int x, int y)
        {
            return cells[y * width + x].CharacterIndex;
        }

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetForeground(int x, int y, Color color)
        {
            cells[y * width + x].Foreground = color;
        }
        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetForeground(int x, int y)
        {
            return cells[y * width + x].Foreground;
        }

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetBackground(int x, int y, Color color)
        {
            cells[y * width + x].Background = color;
        }
        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetBackground(int x, int y)
        {
            return cells[y * width + x].Background;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect. The effect provided will be cloned and then set to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int x, int y, Effects.ICellEffect effect)
        {

            cells[y * width + x].Effect = effect;
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(int x, int y)
        {
            return cells[y * width + x].Effect;
        }

        /// <summary>
        /// Changes the appearance of the cell. The appearance represents the look of a cell and will first be cloned, then applied to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="appearance">The desired appearance of the cell. A null value cannot be passed.</param>
        public void SetCellAppearance(int x, int y, ICellAppearance appearance)
        {
            if (appearance == null)
                throw new NullReferenceException("Appearance may not be null.");

            appearance.CopyAppearanceTo(cells[y * width + x]);
        }
        /// <summary>
        /// Gets the appearance of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The appearance.</returns>
        public ICellAppearance GetCellAppearance(int x, int y)
        {
            CellAppearance appearance = new CellAppearance();
            cells[y * width + x].CopyAppearanceTo(appearance);
            return appearance;
        }

        /// <summary>
        /// Gets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public SpriteEffects GetSpriteEffect(int x, int y)
        {
            return cells[y * width + x].SpriteEffect;
        }

        /// <summary>
        /// Sets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public void SetSpriteEffect(int x, int y, SpriteEffects spriteEffect)
        {
            cells[y * width + x].SpriteEffect = spriteEffect;
        }
        #endregion

        #region Print
        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public void Print(int x, int y, string text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Length > cells.Length ? cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                cells[index].CharacterIndex = text[charIndex];
                charIndex++;
            }
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="appearance">The appearance of the cell</param>
        /// <param name="effect">An optional effect to apply to the printed cells.</param>
        public void Print(int x, int y, string text, ICellAppearance appearance, ICellEffect effect = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Length > cells.Length ? cells.Length - index : index + text.Length;
            int charIndex = 0;

            for (; index < total; index++)
            {
                Cell cell = cells[index];
                appearance.CopyAppearanceTo(cell);
                cell.CharacterIndex = text[charIndex];
                cell.Effect = effect;
                charIndex++;
            }
        }

        /// <summary>
        /// Draws the string on the console at the specified location and color, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        public void Print(int x, int y, string text, Color foreground)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Length > cells.Length ? cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                cells[index].CharacterIndex = text[charIndex];
                cells[index].Foreground = foreground;
                charIndex++;
            }
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified foreground and background color, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        public void Print(int x, int y, string text, Color foreground, Color background)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Length > cells.Length ? cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                cells[index].CharacterIndex = text[charIndex];
                cells[index].Background = background;
                cells[index].Foreground = foreground;
                charIndex++;
            }
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public void Print(int x, int y, ColoredString text)
        {
            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Count > cells.Length ? cells.Length : index + text.Count;
            int charIndex = 0;

            for (; index < total; index++)
            {
                if (!text.IgnoreCharacter)
                    cells[index].CharacterIndex = text[charIndex].Character;
                if (!text.IgnoreBackground)
                    cells[index].Background = text[charIndex].Background;
                if (!text.IgnoreForeground)
                    cells[index].Foreground = text[charIndex].Foreground;
                if (!text.IgnoreEffect)
                    cells[index].Effect = text[charIndex].Effect;
                charIndex++;
            }
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        /// <param name="spriteEffect">The sprite effect to set on the cell.</param>
        public void Print(int x, int y, string text, Color? foreground = null, Color? background = null, SpriteEffects? spriteEffect = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= width || x < 0 || y >= height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * width + x;
            int total = index + text.Length > cells.Length ? cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                cells[index].CharacterIndex = text[charIndex];

                if (background.HasValue)
                    cells[index].Background = background.Value;
                if (foreground.HasValue)
                    cells[index].Foreground = foreground.Value;
                if (spriteEffect.HasValue)
                    cells[index].SpriteEffect = spriteEffect.Value;

                charIndex++;
            }
        }

        #endregion

        #region Get String
        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public string GetString(int x, int y, int length)
        {
            return GetString(y * width + x, length);
        }

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public string GetString(int index, int length)
        {
            if (index >= 0 && index < this.cells.Length)
            {
                StringBuilder sb = new StringBuilder(length);
                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < this.cells.Length)
                        sb.Append((char)this.cells[tempIndex].CharacterIndex);
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Builds a string from the text surface from the specified coordinates.
        /// </summary>
        /// <param name="x">The x position of the surface to start at.</param>
        /// <param name="y">The y position of the surface to start at.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public ColoredString GetStringColored(int x, int y, int length)
        {
            return GetStringColored(y * width + x, length);
        }

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public ColoredString GetStringColored(int index, int length)
        {
            if (index >= 0 && index < this.cells.Length)
            {
                ColoredString sb = new ColoredString(length);

                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < this.cells.Length)
                        this.cells[tempIndex].CopyAppearanceTo(sb[i]);
                }

                return sb;
            }

            return new ColoredString(string.Empty);
        }
        #endregion

        #region Clear
        /// <summary>
        /// Clears the console data. Characters are reset to 0, the forground and background are set to default, and effect set to none.
        /// </summary>
        public void Clear()
        {
            Fill(DefaultForeground, DefaultBackground, 0, null);
        }

        /// <summary>
        /// Clears a cell. Character is reset to 0, the forground and background is set to default, and effect is set to none.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        public void Clear(int x, int y)
        {
            var cell = cells[y * width + x];
            cell.Reset();
            cell.Foreground = DefaultForeground;
            cell.Background = DefaultBackground;
            cell.Effect = null;
        }
        #endregion

        #region Shifting Rows
        /// <summary>
        /// Scrolls all the console data up by one.
        /// </summary>
        public void ShiftUp()
        {
            ShiftUp(1);
        }

        /// <summary>
        /// Scrolls all the console data up by the specified amount of rows.
        /// </summary>
        /// <param name="amount">How many rows to shift.</param>
        public void ShiftUp(int amount, bool wrap = false)
        {
            if (amount == 0)
                return;
            else if (amount < 0)
            {
                ShiftDown(Math.Abs(amount), wrap);
                return;
            }

            List<Tuple<Cell, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<Cell, int>>(height * amount);

                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var tempCell = new Cell();
                        cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (height - amount + y) * width + x));
                    }
                }
            }

            for (int y = amount; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell destination = cells[(y - amount) * width + x];
                    Cell source = cells[y * width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }


            if (!wrap)
                for (int y = height - amount; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Clear(x, y);
                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = cells[wrappedCells[i].Item2];

                    destination.Background = wrappedCells[i].Item1.Background;
                    destination.Foreground = wrappedCells[i].Item1.Foreground;
                    destination.CharacterIndex = wrappedCells[i].Item1.CharacterIndex;
                    destination.Effect = wrappedCells[i].Item1.Effect;
                }
        }

        /// <summary>
        /// Scrolls all the console data down by one.
        /// </summary>
        public void ShiftDown()
        {
            ShiftDown(1);
        }

        /// <summary>
        /// Scrolls all the console data down by the specified amount of rows.
        /// </summary>
        /// <param name="amount">How many rows to shift.</param>
        public void ShiftDown(int amount, bool wrap = false)
        {
            if (amount == 0)
                return;
            else if (amount < 0)
            {
                ShiftUp(Math.Abs(amount), wrap);
                return;
            }


            List<Tuple<Cell, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<Cell, int>>(height * amount);

                for (int y = height - amount; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var tempCell = new Cell();
                        cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (amount - (height - y)) * width + x));
                    }
                }
            }

            for (int y = (height - 1) - amount; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell destination = cells[(y + amount) * width + x];
                    Cell source = cells[y * width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }

            if (!wrap)
                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Cell source = cells[y * width + x];
                        source.Reset();
                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = cells[wrappedCells[i].Item2];

                    destination.Background = wrappedCells[i].Item1.Background;
                    destination.Foreground = wrappedCells[i].Item1.Foreground;
                    destination.CharacterIndex = wrappedCells[i].Item1.CharacterIndex;
                    destination.Effect = wrappedCells[i].Item1.Effect;
                }
        }

        /// <summary>
        /// Scrolls all the console data right by one.
        /// </summary>
        public void ShiftRight()
        {
            ShiftRight(1);
        }

        /// <summary>
        /// Scrolls all the console data right by the specified amount.
        /// </summary>
        /// <param name="amount">How much to scroll.</param>
        public void ShiftRight(int amount, bool wrap = false)
        {
            if (amount == 0)
                return;
            else if (amount < 0)
            {
                ShiftLeft(Math.Abs(amount), wrap);
                return;
            }


            List<Tuple<Cell, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<Cell, int>>(height * amount);

                for (int x = width - amount; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var tempCell = new Cell();
                        cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * width + amount - (width - x)));
                    }
                }
            }


            for (int x = width - 1 - amount; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell destination = cells[y * width + (x + amount)];
                    Cell source = cells[y * width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }

            if (!wrap)
                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Clear(x, y);

                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = cells[wrappedCells[i].Item2];

                    destination.Background = wrappedCells[i].Item1.Background;
                    destination.Foreground = wrappedCells[i].Item1.Foreground;
                    destination.CharacterIndex = wrappedCells[i].Item1.CharacterIndex;
                    destination.Effect = wrappedCells[i].Item1.Effect;
                }
        }

        /// <summary>
        /// Scrolls all the console data left by one.
        /// </summary>
        public void ShiftLeft()
        {
            ShiftLeft(1);
        }

        /// <summary>
        /// Scrolls all the console data left by the specified amount.
        /// </summary>
        /// <param name="amount">How much to scroll.</param>
        public void ShiftLeft(int amount, bool wrap = false)
        {
            if (amount == 0)
                return;
            else if (amount < 0)
            {
                ShiftRight(Math.Abs(amount), wrap);
                return;
            }


            List<Tuple<Cell, int>> wrappedCells = null;

            // Handle all the wrapped ones first
            if (wrap)
            {
                wrappedCells = new List<Tuple<Cell, int>>(height * amount);

                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var tempCell = new Cell();
                        cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * width + (width - amount + x)));
                    }
                }
            }

            for (int x = amount; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell destination = cells[y * width + (x - amount)];
                    Cell source = cells[y * width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }

            if (!wrap)
                for (int x = width - amount; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Clear(x, y);
                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = cells[wrappedCells[i].Item2];

                    destination.Background = wrappedCells[i].Item1.Background;
                    destination.Foreground = wrappedCells[i].Item1.Foreground;
                    destination.CharacterIndex = wrappedCells[i].Item1.CharacterIndex;
                    destination.Effect = wrappedCells[i].Item1.Effect;
                }
        }
        
        #endregion

        #region Fill
        /// <summary>
        /// Fills the console.
        /// </summary>
        /// <param name="foreground">Foregorund of every cell.</param>
        /// <param name="background">Foregorund of every cell.</param>
        /// <param name="character">Character of every cell.</param>
        /// <param name="effect">Effect of every cell.</param>
        public void Fill(Color foreground, Color background, int character, Effects.ICellEffect effect)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Foreground = foreground;
                cells[i].Background = background;
                cells[i].CharacterIndex = character;
                cells[i].Effect = effect;
            }
        }

        /// <summary>
        /// Fills the console.
        /// </summary>
        /// <param name="foreground">Foregorund of every cell.</param>
        /// <param name="background">Foregorund of every cell.</param>
        /// <param name="character">Character of every cell.</param>
        /// <param name="effect">Effect of every cell.</param>
        /// <param name="spriteEffect">Sprite effect of every cell.</param>
        public void Fill(Color foreground, Color background, int character, Effects.ICellEffect effect, SpriteEffects spriteEffect)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Foreground = foreground;
                cells[i].Background = background;
                cells[i].CharacterIndex = character;
                cells[i].SpriteEffect = spriteEffect;
                cells[i].Effect = effect;
            }
        }

        /// <summary>
        /// Fills the specified area.
        /// </summary>
        /// <param name="area">The area to fill.</param>
        /// <param name="foreground">Foregorund of every cell.</param>
        /// <param name="background">Foregorund of every cell.</param>
        /// <param name="character">Character of every cell.</param>
        /// <param name="effect">Effect of every cell.</param>
        public void FillArea(Rectangle area, Color foreground, Color background, int character, Effects.ICellEffect effect)
        {
            // Check for valid rect
            Rectangle consoleArea = new Rectangle(0, 0, width, height);
            if (consoleArea.Contains(area))
            {
                for (int x = area.X; x < area.Right; x++)
                {
                    for (int y = area.Y; y < area.Bottom; y++)
                    {
                        Cell cell = this.cells[y * width + x];
                        cell.Foreground = foreground;
                        cell.Background = background;
                        cell.CharacterIndex = character;
                        cell.Effect = effect;
                    }
                }
            }
        }

        /// <summary>
        /// Fills the specified area.
        /// </summary>
        /// <param name="area">The area to fill.</param>
        /// <param name="foreground">Foregorund of every cell.</param>
        /// <param name="background">Foregorund of every cell.</param>
        /// <param name="character">Character of every cell.</param>
        /// <param name="effect">Effect of every cell.</param>
        /// <param name="spriteEffect">Sprite effect of every cell.</param>
        public void FillArea(Rectangle area, Color foreground, Color background, int character, Effects.ICellEffect effect, SpriteEffects spriteEffect)
        {
            List<Cell> cells = new List<Cell>(area.Width * area.Height);

            // Check for valid rect
            Rectangle consoleArea = new Rectangle(0, 0, width, height);
            if (consoleArea.Contains(area))
            {
                for (int x = area.X; x < area.Right; x++)
                {
                    for (int y = area.Y; y < area.Bottom; y++)
                    {
                        Cell cell = this.cells[y * width + x];
                        cell.Foreground = foreground;
                        cell.Background = background;
                        cell.CharacterIndex = character;
                        cell.SpriteEffect = spriteEffect;
                        cell.Effect= effect;
                    }
                }
            }
        }
        #endregion

        #endregion
        #endregion

        
        /// <summary>
        /// Saves this TextSurface.
        /// </summary>
        /// <param name="file">The file to save the TextSurface too.</param>
        public void Save(string file)
        {
            TextSurfaceSerialized.Save(this, file);
        }

        /// <summary>
        /// Loads a TextSurface.
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <returns></returns>
        public static TextSurface Load(string file)
        {
            return TextSurfaceSerialized.Load(file);
        }

        [DataContract]
        internal class TextSurfaceSerialized
        {
            [DataMember]
            Cell[] Cells;

            [DataMember]
            Rectangle ViewArea;

            [DataMember]
            string FontName;

            [DataMember]
            int FontMultiple;

            [DataMember]
            int Width;

            [DataMember]
            int Height;

            [DataMember]
            Color Tint;

            public static void Save(TextSurface surfaceBase, string file)
            {
                TextSurfaceSerialized data = new TextSurfaceSerialized();
                data.Cells = surfaceBase.cells;
                data.ViewArea = surfaceBase.ViewArea;
                data.FontName = surfaceBase.font.Name;
                data.FontMultiple = surfaceBase.font.SizeMultiple;
                data.Width = surfaceBase.width;
                data.Height = surfaceBase.height;
                data.Tint = surfaceBase.Tint;

                SadConsole.Serializer.Save(data, file);
            }

            public static TextSurface Load(string file)
            {
                TextSurfaceSerialized data = Serializer.Load<TextSurfaceSerialized>(file);
                TextSurface newSurface = new TextSurface();

                newSurface.width = data.Width;
                newSurface.height = data.Height;
                newSurface.cells = newSurface.renderCells = data.Cells;

                // Try to find font
                if (Engine.Fonts.ContainsKey(data.FontName))
                    newSurface.font = Engine.Fonts[data.FontName].GetFont(data.FontMultiple);
                else
                    newSurface.font = Engine.DefaultFont;

                newSurface.ViewArea = data.ViewArea;
                newSurface.Tint = data.Tint;

                return newSurface;
            }
        }
    }
}
