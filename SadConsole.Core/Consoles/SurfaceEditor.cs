using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using SpriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects;


using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Effects;

namespace SadConsole.Consoles
{
    public class SurfaceEditor
    {
        protected ITextSurface textSurface;
        protected int width;
        protected int height;

        public int TimesShiftedDown;
        public int TimesShiftedRight;
        public int TimesShiftedLeft;
        public int TimesShiftedUp;
        
        public ITextSurface TextSurface
        {
            get { return textSurface; }
            set
            {
                if (value == null)
                    throw new NullReferenceException();

                var old = textSurface;
                textSurface = value;
                
                OnSurfaceChanged(old, value);
            }
        }

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get { return textSurface.Cells[y * width + x]; }
            protected set { textSurface.Cells[y * width + x] = value; }
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get { return textSurface.Cells[index]; }
            protected set { textSurface.Cells[index] = value; }
        }

        #region Constructors
        /// <summary>
        /// Creates a new cell surface that can be resized and also have its textSurface.Cells resized.
        /// </summary>
        /// <remarks>You must set the Font property before rendering this cell surface.</remarks>
        //public ConsoleData() : this(1, 1, Engine.DefaultFont) { }

        public SurfaceEditor(ITextSurface surface)
        {
            width = surface.Width;
            height = surface.Height;
            textSurface = surface;
        }
        #endregion

        protected virtual void OnSurfaceChanged(ITextSurface oldSurface, ITextSurface newSurface)
        {
            width = textSurface.Width;
            height = textSurface.Height;
        }

        #region Public Methods

        /// <summary>
        /// Sets each background of a cell to the array of colors. Must be the same length as this cell surface.
        /// </summary>
        /// <param name="pixels">The colors to place.</param>
        public void SetPixels(Color[] pixels)
        {
            if (pixels.Length != textSurface.Cells.Length)
                throw new ArgumentOutOfRangeException("pixels", "The amount of colors do not match the size of this cell surface.");

            for (int i = 0; i < pixels.Length; i++)
                textSurface.Cells[i].Background = pixels[i];
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
            return index >= 0 && index < textSurface.Cells.Length;
        }
        #endregion

        #region Copy
        




































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
            textSurface.Cells[y * width + x].CharacterIndex = character;
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

            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].CharacterIndex = character;
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

            textSurface.Cells[index].Background = background;
            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].CharacterIndex = character;
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

            textSurface.Cells[index].Background = background;
            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].CharacterIndex = character;
            textSurface.Cells[index].Effect = effect;
        }

        /// <summary>
        /// Gets the character of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The character.</returns>
        public int GetCharacter(int x, int y)
        {
            return textSurface.Cells[y * width + x].CharacterIndex;
        }

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetForeground(int x, int y, Color color)
        {
            textSurface.Cells[y * width + x].Foreground = color;
        }
        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetForeground(int x, int y)
        {
            return textSurface.Cells[y * width + x].Foreground;
        }

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetBackground(int x, int y, Color color)
        {
            textSurface.Cells[y * width + x].Background = color;
        }
        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetBackground(int x, int y)
        {
            return textSurface.Cells[y * width + x].Background;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect. The effect provided will be cloned and then set to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int x, int y, Effects.ICellEffect effect)
        {

            textSurface.Cells[y * width + x].Effect = effect;
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(int x, int y)
        {
            return textSurface.Cells[y * width + x].Effect;
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

            appearance.CopyAppearanceTo(textSurface.Cells[y * width + x]);
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
            textSurface.Cells[y * width + x].CopyAppearanceTo(appearance);
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
            return textSurface.Cells[y * width + x].SpriteEffect;
        }

        /// <summary>
        /// Sets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="spriteEffect">The sprite effect of the cell.</param>
        public void SetSpriteEffect(int x, int y, SpriteEffects spriteEffect)
        {
            textSurface.Cells[y * width + x].SpriteEffect = spriteEffect;
        }

        /// <summary>
        /// Fills a console with random colors and characters.
        /// </summary>
        public void FillWithRandomGarbage(bool useEffect = false)
        {
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetCharacter(x, y, charCounter);
                    SetForeground(x, y, new Color(Engine.Random.Next(0, 256), Engine.Random.Next(0, 256), Engine.Random.Next(0, 256), 255));
                    SetBackground(x, y, textSurface.DefaultBackground);
                    SetBackground(x, y, new Color(Engine.Random.Next(0, 256), Engine.Random.Next(0, 256), Engine.Random.Next(0, 256), 255));
                    SetSpriteEffect(x, y, (SpriteEffects)Engine.Random.Next(0, 4));
                    charCounter++;
                    if (charCounter > 255)
                        charCounter = 0;
                }
            }
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
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                textSurface.Cells[index].CharacterIndex = text[charIndex];
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
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;

            for (; index < total; index++)
            {
                Cell cell = textSurface.Cells[index];
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
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                textSurface.Cells[index].CharacterIndex = text[charIndex];
                textSurface.Cells[index].Foreground = foreground;
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
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                textSurface.Cells[index].CharacterIndex = text[charIndex];
                textSurface.Cells[index].Background = background;
                textSurface.Cells[index].Foreground = foreground;
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
            int total = index + text.Count > textSurface.Cells.Length ? textSurface.Cells.Length : index + text.Count;
            int charIndex = 0;

            for (; index < total; index++)
            {
                if (!text.IgnoreCharacter)
                    textSurface.Cells[index].CharacterIndex = text[charIndex].Character;
                if (!text.IgnoreBackground)
                    textSurface.Cells[index].Background = text[charIndex].Background;
                if (!text.IgnoreForeground)
                    textSurface.Cells[index].Foreground = text[charIndex].Foreground;
                if (!text.IgnoreEffect)
                    textSurface.Cells[index].Effect = text[charIndex].Effect;
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
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                textSurface.Cells[index].CharacterIndex = text[charIndex];

                if (background.HasValue)
                    textSurface.Cells[index].Background = background.Value;
                if (foreground.HasValue)
                    textSurface.Cells[index].Foreground = foreground.Value;
                if (spriteEffect.HasValue)
                    textSurface.Cells[index].SpriteEffect = spriteEffect.Value;

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
            if (index >= 0 && index < this.textSurface.Cells.Length)
            {
                StringBuilder sb = new StringBuilder(length);
                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < this.textSurface.Cells.Length)
                        sb.Append((char)this.textSurface.Cells[tempIndex].CharacterIndex);
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
            if (index >= 0 && index < this.textSurface.Cells.Length)
            {
                ColoredString sb = new ColoredString(length);

                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < this.textSurface.Cells.Length)
                        this.textSurface.Cells[tempIndex].CopyAppearanceTo(sb[i]);
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
            Fill(textSurface.DefaultForeground, textSurface.DefaultBackground, 0, null);
        }

        /// <summary>
        /// Clears a cell. Character is reset to 0, the forground and background is set to default, and effect is set to none.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        public void Clear(int x, int y)
        {
            var cell = textSurface.Cells[y * width + x];
            cell.Reset();
            cell.Foreground = textSurface.DefaultForeground;
            cell.Background = textSurface.DefaultBackground;
        }
        #endregion

        #region Shifting Rows

        public void ClearShiftValues()
        {
            TimesShiftedDown = 0;
            TimesShiftedUp = 0;
            TimesShiftedLeft = 0;
            TimesShiftedRight = 0;
        }

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

            TimesShiftedUp += amount;

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
                        textSurface.Cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (height - amount + y) * width + x));
                    }
                }
            }

            for (int y = amount; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell destination = textSurface.Cells[(y - amount) * width + x];
                    Cell source = textSurface.Cells[y * width + x];

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
                    Cell destination = textSurface.Cells[wrappedCells[i].Item2];

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

            TimesShiftedDown += amount;

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
                        textSurface.Cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (amount - (height - y)) * width + x));
                    }
                }
            }

            for (int y = (height - 1) - amount; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell destination = textSurface.Cells[(y + amount) * width + x];
                    Cell source = textSurface.Cells[y * width + x];

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
                        Cell source = textSurface.Cells[y * width + x];
                        source.Reset();
                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = textSurface.Cells[wrappedCells[i].Item2];

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

            TimesShiftedRight += amount;

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
                        textSurface.Cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * width + amount - (width - x)));
                    }
                }
            }


            for (int x = width - 1 - amount; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell destination = textSurface.Cells[y * width + (x + amount)];
                    Cell source = textSurface.Cells[y * width + x];

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
                    Cell destination = textSurface.Cells[wrappedCells[i].Item2];

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

            TimesShiftedLeft += amount;

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
                        textSurface.Cells[y * width + x].Copy(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * width + (width - amount + x)));
                    }
                }
            }

            for (int x = amount; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell destination = textSurface.Cells[y * width + (x - amount)];
                    Cell source = textSurface.Cells[y * width + x];

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
                    Cell destination = textSurface.Cells[wrappedCells[i].Item2];

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
            for (int i = 0; i < textSurface.Cells.Length; i++)
            {
                textSurface.Cells[i].Foreground = foreground;
                textSurface.Cells[i].Background = background;
                textSurface.Cells[i].CharacterIndex = character;
                textSurface.Cells[i].Effect = effect;
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
            for (int i = 0; i < textSurface.Cells.Length; i++)
            {
                textSurface.Cells[i].Foreground = foreground;
                textSurface.Cells[i].Background = background;
                textSurface.Cells[i].CharacterIndex = character;
                textSurface.Cells[i].SpriteEffect = spriteEffect;
                textSurface.Cells[i].Effect = effect;
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
                        Cell cell = this.textSurface.Cells[y * width + x];
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
            // Check for valid rect
            Rectangle consoleArea = new Rectangle(0, 0, width, height);
            if (consoleArea.Contains(area))
            {
                for (int x = area.X; x < area.Right; x++)
                {
                    for (int y = area.Y; y < area.Bottom; y++)
                    {
                        Cell cell = this.textSurface.Cells[y * width + x];
                        cell.Foreground = foreground;
                        cell.Background = background;
                        cell.CharacterIndex = character;
                        cell.SpriteEffect = spriteEffect;
                        cell.Effect = effect;
                    }
                }
            }
        }
        #endregion

        #endregion
        #endregion


        ///// <summary>
        ///// Saves this TextSurface.
        ///// </summary>
        ///// <param name="file">The file to save the TextSurface too.</param>
        //public void Save(string file)
        //{
        //    TextSurfaceSerialized.Save(this, file);
        //}

        ///// <summary>
        ///// Loads a TextSurface.
        ///// </summary>
        ///// <param name="file">The file to load.</param>
        ///// <returns></returns>
        //public static TextSurface Load(string file)
        //{
        //    return TextSurfaceSerialized.Load(file);
        //}
    }


}
