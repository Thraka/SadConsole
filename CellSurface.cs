using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Represents all the basic information about console text and methods to manipulate that text.
    /// </summary>
    [DataContract]
    public class CellSurface: IEnumerable<Cell>
    {
        [DataMember(Name = "Width")]
        protected int _width = 1;
        [DataMember(Name = "Height")]
        protected int _height = 1;

        private Dictionary<ICellEffect, CellEffectData> _effects;
        private Dictionary<Cell, CellEffectData> _effectCells;

        [DataMember(Name = "Effects")]
        private Dictionary<ICellEffect, Point[]> _effectSerialized;

        /// <summary>
        /// Raised when this cell surface is resized.
        /// </summary>
        public event EventHandler Resized;

        #region Properties
        /// <summary>
        /// An array of all cells in this surface.
        /// </summary>
        /// <remarks>This array is calculated internally and its size shouldn't be modified. Use the <see cref="Width"/> and <see cref="Height"/> properties instead. The cell data can be changed.</remarks>
        [DataMember]
        protected Cell[] Cells { get; set; }

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get { return Cells[y * _width + x]; }
            protected set { Cells[y * _width + x] = value; }
        }

        /// <summary>
        /// Gets a cell by index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int index]
        {
            get { return Cells[index]; }
            protected set { Cells[index] = value; }
        }

        /// <summary>
        /// The total cells for this surface.
        /// </summary>
        public int CellCount { get { return Cells.Length; } }

        /// <summary>
        /// The default foreground for characters on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultForeground { get; set; }

        /// <summary>
        /// The default background for characters on this surface.
        /// </summary>
        [DataMember]
        public Color DefaultBackground { get; set; }

        /// <summary>
        /// How many cells wide the surface is.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// How many cells high the surface is.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        public int TimesShiftedUp { get; set; }

        bool ReuseEffects { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new cell surface that can be resized and also have its cells resized.
        /// </summary>
        /// <remarks>You must set the Font property before rendering this cell surface.</remarks>
        public CellSurface() : this(1, 1) { }

        public CellSurface(int width, int height)
        {
            DefaultBackground = Color.Transparent;
            DefaultForeground = Color.White;
            ReuseEffects = true;
            Resize(width, height);

            _effects = new Dictionary<ICellEffect, CellEffectData>(20);
            _effectCells = new Dictionary<Cell,CellEffectData>(50);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the cells. This method caches all of the rendering points and rectangles and initializes each cell.
        /// </summary>
        /// <param name="oldWidth">The old size of the surface in width. Used when resizing to preserve existing cells.</param>
        protected virtual void InitializeCells(int oldWidth, int oldHeight)
        {
            bool processOldCells = Cells != null;

            Rectangle oldCellSurfaceArea = new Rectangle(0, 0, oldWidth, oldHeight);
            Cell[] existingCells = Cells;
            Cells = new Cell[Width * Height];

            for (int i = 0; i < Cells.Length; i++)
            {
                if (processOldCells)
                {
                    Point cellPosition = GetPointFromIndex(i);

                    if (oldCellSurfaceArea.Contains(cellPosition))
                    {
                        // Check if the cell position was valid in the old cell data, 
                        int oldCellIndex = CellSurface.GetIndexFromPoint(cellPosition, oldWidth);

                        // new position existed in the old cells, we should copy.
                        Cells[i] = existingCells[oldCellIndex];

                        // We've processed the old cell, we should null it out to flag as processed
                        existingCells[oldCellIndex] = null;

                        // Update the extra metadata.
                        Cells[i].Position = cellPosition;
                        Cells[i].Index = i;
                    }
                    else
                    {
                        Cells[i] = new Cell();
                        Cells[i].Foreground = this.DefaultForeground;
                        Cells[i].Background = this.DefaultBackground;
                        Cells[i].Position = cellPosition;
                        Cells[i].Index = i;
                        Cells[i].OnCreated();
                    }
                }
                else
                {
                    Cells[i] = new Cell();
                    Cells[i].Foreground = this.DefaultForeground;
                    Cells[i].Background = this.DefaultBackground;
                    Cells[i].Position = GetPointFromIndex(i);
                    Cells[i].Index = i;
                    Cells[i].OnCreated();
                }
            }

            // We had old cells. Any that are not null, haven't been copied and must check for effects to remove.
            if (processOldCells)
            {
                foreach (var cell in existingCells)
                {
                    if (cell != null)
                    {
                        ClearCellEffect(cell);
                    }
                }
            }

        }

        private bool GetKnownEffect(ICellEffect effect, out CellEffectData effectData)
        {
            if (_effects.TryGetValue(effect, out effectData))
            {
                return true;
            }
            else
            {
                effectData = new CellEffectData(effect);
                return false;
            }
        }
        #endregion

        #region Public Methods
        #region Static Methods
        public static int GetIndexFromPoint(Point location, int width)
        {
            return location.Y * width + location.X;
        }

        public int GetIndexFromPoint(Point location)
        {
            return location.Y * _width + location.X;
        }

        public static int GetIndexFromPoint(int x, int y, int width)
        {
            return y * width + x;
        }

        public int GetIndexFromPoint(int x, int y)
        {
            return y * _width + x;
        }

        public static Point GetPointFromIndex(int index, int width)
        {
            return new Point(index % width, index / width);
        }

        public Point GetPointFromIndex(int index)
        {
            return new Point(index % _width, index / _width);
        }
        #endregion

        #region Resize
        /// <summary>
        /// Resizes the cell surface and resets all data.
        /// </summary>
        /// <param name="width">The width of the console in cells.</param>
        /// <param name="height">The height of the console in cells.</param>
        public void Resize(int width, int height)
        {
            if (width <= 0)
                throw new Exception("Width of a cell surface must be greater than 0");

            if (height <= 0)
                throw new Exception("Height of a cell surface must be greater than 0");

            int oldWidth = _width;
            int oldHeight = _height;
            _width = width;
            _height = height;

            InitializeCells(oldWidth, oldHeight);

            OnResize();
        }

        /// <summary>
        /// Called when the cell surface is resized. Also raises the <see cref="Resized"/> event.
        /// </summary>
        protected virtual void OnResize()
        {
            if (Resized != null)
                Resized(this, new EventArgs());
        }
        #endregion

        #region IsValidCell
        /// <summary>
        /// Tests if a cell is valid based on its x,y position.
        /// </summary>
        /// <param name="x">The x coordinate of the cell to test.</param>
        /// <param name="y">The y coordinate of the cell to test.</param>
        /// <returns>A true value indicating the cell by x,y does exist in this cell surface.</returns>
        public bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
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
            if (x >= 0 && x < this.Width && y >= 0 && y < this.Height)
            {
                index = y * Width + x;
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
            return index >= 0 && index < Cells.Length;
        }
        #endregion

        #region Copy
        /// <summary>
        /// Copies the contents of this cell surface to the destination.
        /// </summary>
        /// <remarks>If the sizes to not match, it will always start at 0,0 and work with what it can and move on to the next row when either surface runs out of columns being processed</remarks>
        /// <param name="destination">The destination surface.</param>
        public void Copy(CellSurface destination)
        {
            int maxX = this._width >= destination._width ? destination._width : this._width;
            int maxY = this._height >= destination._height ? destination._height : this._height;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var sourceCell = this[x, y];
                    var desCell = destination[x, y];
                    sourceCell.CopyAppearanceTo(desCell);
                    destination.SetEffect(desCell, sourceCell.Effect);
                }
            }
        }

        /// <summary>
        /// Copies the contents of this cell surface at the specified x,y coordinates to the destination.
        /// </summary>
        /// <param name="x">The x coordinate to start from.</param>
        /// <param name="y">The y coordinate to start from.</param>
        /// <param name="destination">The destination surface.</param>
        public void Copy(int x, int y, CellSurface destination)
        {
            int maxX = this._width - x >= destination._width ? destination._width : this._width;
            int maxY = this._height - y >= destination._height ? destination._height : this._height;

            int destX = 0;
            int destY = 0;

            for (int curx = x; curx < maxX; curx++)
            {
                for (int cury = y; cury < maxY; cury++)
                {
                    var sourceCell = this[curx, cury];
                    var desCell = destination[destX, destY];
                    sourceCell.CopyAppearanceTo(desCell);
                    destination.SetEffect(desCell, sourceCell.Effect);
                    destY++;
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
        public void Copy(int x, int y, int width, int height, CellSurface destination)
        {
            if (x + width > this._width || y + height > this._height)
                throw new Exception("Invalid x,y,width,height combination.");

            int maxX = width > destination._width ? destination._width : x + width;
            int maxY = height > destination._height ? destination._height : y + height;

            int destX = 0;
            int destY = 0;

            for (int curx = 0; curx < maxX; curx++)
            {
                for (int cury = 0; cury < maxY; cury++)
                {
                    var sourceCell = this[curx + x, cury + y];
                    var desCell = destination[destX, destY];
                    sourceCell.CopyAppearanceTo(desCell);
                    destination.SetEffect(desCell, sourceCell.Effect);
                    destY++;
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
        public void Copy(int x, int y, int width, int height, CellSurface destination, int destinationX, int destinationY)
        {
            if (x + width > this._width || y + height > this._height)
                throw new Exception("Invalid x,y,width,height combination.");

            int destX = destinationX;
            int destY = destinationY;

            for (int curx = 0; curx < width; curx++)
            {
                for (int cury = 0; cury < height; cury++)
                {
                    if (IsValidCell(curx + x, cury + y))
                    {
                        var sourceCell = this[curx + x, cury + y];

                        if (destination.IsValidCell(destX, destY))
                        {
                            var desCell = destination[destX, destY];
                            sourceCell.CopyAppearanceTo(desCell);
                            destination.SetEffect(desCell, sourceCell.Effect);
                        }
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
            Cells[y * Width + x].CharacterIndex = character;
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
            int index = y * Width + x;

            Cells[index].Foreground = foreground;
            Cells[index].CharacterIndex = character;
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
            int index = y * Width + x;

            Cells[index].Background = background;
            Cells[index].Foreground = foreground;
            Cells[index].CharacterIndex = character;
        }

        public void SetCharacter(int x, int y, int character, Color foreground, Color background, ICellEffect effect)
        {
            int index = y * Width + x;

            Cells[index].Background = background;
            Cells[index].Foreground = foreground;
            Cells[index].CharacterIndex = character;

            SetEffect(Cells[index], effect);
        }

        /// <summary>
        /// Gets the character of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The character.</returns>
        public int GetCharacter(int x, int y)
        {
            return Cells[y * Width + x].CharacterIndex;
        }

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetForeground(int x, int y, Color color)
        {
            Cells[y * Width + x].Foreground = color;
        }
        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetForeground(int x, int y)
        {
            return Cells[y * Width + x].Foreground;
        }

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetBackground(int x, int y, Color color)
        {
            Cells[y * Width + x].Background = color;
        }
        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetBackground(int x, int y)
        {
            return Cells[y * Width + x].Background;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect. The effect provided will be cloned and then set to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int x, int y, Effects.ICellEffect effect)
        {
            SetEffect(Cells[y * Width + x], effect);
        }

        /// <summary>
        /// Changes the effect of the identified cell.
        /// </summary>
        /// <param name="cell">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(Cell cell, ICellEffect effect)
        {
            if (effect != null)
            {
                CellEffectData workingEffect;

                if (effect.CloneOnApply)
                {
                    effect = effect.Clone();
                    workingEffect = new CellEffectData(effect);
                    _effects.Add(workingEffect.Effect, workingEffect);
                }
                else
                {
                    // Is the effect unknown? Add it.
                    if (GetKnownEffect(effect, out workingEffect) == false)
                        _effects.Add(workingEffect.Effect, workingEffect);
                    else
                        if (workingEffect.Cells.Contains(cell))
                            return;
                }

                // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                ClearCellEffect(cell);

                // Add the cell to the effects by cell key and to list of known cells for the effect
                _effectCells.Add(cell, workingEffect);
                cell.Effect = workingEffect.Effect;
                workingEffect.Cells.Add(cell);
            }
            else
                ClearCellEffect(cell);
        }

        /// <summary>
        /// Changes the effect of the <paramref name="cells"/> provided.
        /// </summary>
        /// <param name="cells">Cells to change the effect on.</param>
        /// <param name="effect">The effect to associate with the cell.</param>
        public void SetEffect(IEnumerable<Cell> cells, ICellEffect effect)
        {
            if (effect != null)
            {
                CellEffectData workingEffect;

                if (effect.CloneOnApply)
                {
                    effect = effect.Clone();
                    workingEffect = new CellEffectData(effect);
                    _effects.Add(workingEffect.Effect, workingEffect);
                }
                else
                {
                    // Is the effect unknown? Add it.
                    if (GetKnownEffect(effect, out workingEffect) == false)
                        _effects.Add(workingEffect.Effect, workingEffect);
                }

                foreach (var cell in cells)
                {
                    if (!workingEffect.Cells.Contains(cell))
                    {
                        // Remove the targeted cell from the known cells list if it is already there (associated with another effect)
                        ClearCellEffect(cell);

                        // Add the cell to the effects by cell key and to list of known cells for the effect
                        _effectCells.Add(cell, workingEffect);
                        cell.Effect = workingEffect.Effect;
                        workingEffect.Cells.Add(cell);
                    }
                }
            }
            else
            {
                foreach (var cell in cells)
                    ClearCellEffect(cell);
            }
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(int x, int y)
        {
            return Cells[y * Width + x].Effect;
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

            appearance.CopyAppearanceTo(Cells[y * Width + x]);
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
            Cells[y * Width + x].CopyAppearanceTo(appearance);
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
            return Cells[y * Width + x].SpriteEffect;
        }

        /// <summary>
        /// Sets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public void SetSpriteEffect(int x, int y, SpriteEffects spriteEffect)
        {
            Cells[y * Width + x].SpriteEffect = spriteEffect;
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

            if (x >= Width || x < 0 || y >= Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * Width + x;
            int total = index + text.Length > Cells.Length ? Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                Cells[index].CharacterIndex = text[charIndex];
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
        public void Print(int x, int y, string text, ICellAppearance appearance, ICellEffect effect = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= Width || x < 0 || y >= Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * Width + x;
            int total = index + text.Length > Cells.Length ? Cells.Length - index : index + text.Length;
            int charIndex = 0;

            List<Cell> effectCells = new List<Cell>(total);

            for (; index < total; index++)
            {
                Cell cell = Cells[index];
                appearance.CopyAppearanceTo(cell);
                cell.CharacterIndex = text[charIndex];
                effectCells.Add(cell);
                charIndex++;
            }

            SetEffect(effectCells, effect);
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

            if (x >= Width || x < 0 || y >= Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * Width + x;
            int total = index + text.Length > Cells.Length ? Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                Cells[index].CharacterIndex = text[charIndex];
                Cells[index].Foreground = foreground;
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

            if (x >= Width || x < 0 || y >= Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * Width + x;
            int total = index + text.Length > Cells.Length ? Cells.Length - index : index + text.Length;
            int charIndex = 0;
            for (; index < total; index++)
            {
                Cells[index].CharacterIndex = text[charIndex];
                Cells[index].Background = background;
                Cells[index].Foreground = foreground;
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
            if (x >= Width || x < 0 || y >= Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * Width + x;
            int total = index + text.Count > Cells.Length ? Cells.Length : index + text.Count;
            int charIndex = 0;

            for (; index < total; index++)
            {
                if (!text.IgnoreCharacter)
                    Cells[index].CharacterIndex = text[charIndex].Character;
                if (!text.IgnoreBackground)
                    Cells[index].Background = text[charIndex].Background;
                if (!text.IgnoreForeground)
                    Cells[index].Foreground = text[charIndex].Foreground;
                if (!text.IgnoreEffect)
                    SetEffect(Cells[index], text[charIndex].Effect);
                charIndex++;
            }
        }

        #endregion

        #region Get String
        public string GetString(int x, int y, int length)
        {
            return GetString(y * Width + x, length);
        }

        public string GetString(int index, int length)
        {
            if (index >= 0 && index < this.Cells.Length)
            {
                StringBuilder sb = new StringBuilder(length);
                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < this.Cells.Length)
                        sb.Append((char)this.Cells[tempIndex].CharacterIndex);
                }

                return sb.ToString();
            }

            return string.Empty;
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
            var cell = Cells[y * Width + x];
            cell.Reset();
            cell.Foreground = DefaultForeground;
            cell.Background = DefaultBackground;
            ClearCellEffect(cell);
        }
        #endregion

        #region Shifting Rows
        /// <summary>
        /// Scrolls all the console data up by one.
        /// </summary>
        public void ShiftRowsUp()
        {
            ShiftRowsUp(1);
        }
        /// <summary>
        /// Scrolls all the console data up by the specified amount of rows.
        /// </summary>
        /// <param name="rowsToShift">How many rows to shift.</param>
        public void ShiftRowsUp(int rowsToShift)
        {
            for (int y = rowsToShift; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell destination = Cells[(y - rowsToShift) * Width + x];
                    Cell source = Cells[y * Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }


            for (int y = Height - rowsToShift; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Clear(x, y);
                }
            }

            TimesShiftedUp += rowsToShift;
        }

        /// <summary>
        /// Scrolls all the console data down by one.
        /// </summary>
        public void ShiftRowsDown()
        {
            ShiftRowsDown(1);
        }
        /// <summary>
        /// Scrolls all the console data down by the specified amount of rows.
        /// </summary>
        /// <param name="rowsToShift">How many rows to shift.</param>
        public void ShiftRowsDown(int rowsToShift)
        {
            for (int y = (Height - 1) - rowsToShift; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell destination = Cells[(y + rowsToShift) * Width + x];
                    Cell source = Cells[y * Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.CharacterIndex = source.CharacterIndex;
                    destination.Effect = source.Effect;
                }
            }


            for (int y = 0; y < rowsToShift; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell source = Cells[y * Width + x];
                    source.Reset();
                }
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
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Foreground = foreground;
                Cells[i].Background = background;
                Cells[i].CharacterIndex = character;
            }

            SetEffect(Cells, effect);
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
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Foreground = foreground;
                Cells[i].Background = background;
                Cells[i].CharacterIndex = character;
                Cells[i].SpriteEffect = spriteEffect;
            }

            SetEffect(Cells, effect);
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
            List<Cell> cells = new List<Cell>(area.Width * area.Height);

            // Check for valid rect
            Rectangle consoleArea = new Rectangle(0, 0, Width, Height);
            if (consoleArea.Contains(area))
            {
                for (int x = area.X; x < area.Right; x++)
                {
                    for (int y = area.Y; y < area.Bottom; y++)
                    {
                        Cell cell = Cells[y * Width + x];
                        cell.Foreground = foreground;
                        cell.Background = background;
                        cell.CharacterIndex = character;
                        cells.Add(cell);
                    }
                }
            }

            SetEffect(cells, effect);
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
            Rectangle consoleArea = new Rectangle(0, 0, Width, Height);
            if (consoleArea.Contains(area))
            {
                for (int x = area.X; x < area.Right; x++)
                {
                    for (int y = area.Y; y < area.Bottom; y++)
                    {
                        Cell cell = Cells[y * Width + x];
                        cell.Foreground = foreground;
                        cell.Background = background;
                        cell.CharacterIndex = character;
                        cell.SpriteEffect = spriteEffect;
                        cells.Add(cell);
                    }
                }
            }

            SetEffect(cells, effect);
        }
        #endregion

        #region Effect Helpers
        private void ClearCellEffect(Cell cell)
        {
            CellEffectData oldEffectData;

            if (_effectCells.TryGetValue(cell, out oldEffectData))
            {
                oldEffectData.Effect.Clear(cell);
                oldEffectData.Cells.Remove(cell);
                _effectCells.Remove(cell);

                if (oldEffectData.Cells.Count == 0)
                    _effects.Remove(oldEffectData.Effect);
            }

            cell.Effect = null;
        }

        /// <summary>
        /// Updates all known effects and applies them to their associated cells.
        /// </summary>
        /// <param name="timeElapsed">The time elapased since the last update.</param>
        public void UpdateEffects(double timeElapsed)
        {
            List<ICellEffect> effectsToRemove = new List<ICellEffect>();

            foreach (var effectData in _effects.Values)
            {
                List<Cell> cellsToRemove = new List<Cell>();
                effectData.Effect.Update(timeElapsed);

                foreach (var cell in effectData.Cells)
                {
                    effectData.Effect.Apply(cell);

                    if (effectData.Effect.IsFinished && effectData.Effect.RemoveOnFinished)
                        cellsToRemove.Add(cell);
                }

                foreach (var cell in cellsToRemove)
                {
                    effectData.Effect.Clear(cell);
                    effectData.Cells.Remove(cell);
                    _effectCells.Remove(cell);
                    cell.Effect = null;
                }

                if (effectData.Cells.Count == 0)
                    effectsToRemove.Add(effectData.Effect);
            }

            foreach (var effect in effectsToRemove)
                _effects.Remove(effect);
        }
        #endregion

        #endregion
        #endregion

        [OnSerializing]
        private void BeforeDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            _effectSerialized = new Dictionary<ICellEffect, Point[]>(_effects.Count);

            foreach (var effectData in _effects.Values)
            {
                List<Point> effectCellPositions = new List<Point>(effectData.Cells.Count);

                foreach (var cell in effectData.Cells)
                    effectCellPositions.Add(cell.Position);

                _effectSerialized.Add(effectData.Effect, effectCellPositions.ToArray());
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            // Update each cell with local information
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Position = GetPointFromIndex(i);
                Cells[i].Index = i;
            }

            _effects = new Dictionary<ICellEffect, CellEffectData>(20);
            _effectCells = new Dictionary<Cell, CellEffectData>(50);

            if (_effectSerialized != null)
            {
                foreach (var effect in _effectSerialized.Keys)
                {
                    Point[] points = _effectSerialized[effect];

                    List<Cell> cells = new List<Cell>(points.Length);

                    foreach (var position in points)
                        cells.Add(this[position.X, position.Y]);

                    SetEffect(cells, effect);
                }
            }

            _effectSerialized = null;
        }

        public static void Save(CellSurface instance, string file)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CellSurface));
            using (var stream = System.IO.File.OpenWrite(file))
            {
                serializer.WriteObject(stream, instance);
            }
        }

        public static CellSurface Load(string file)
        {
            if (System.IO.File.Exists(file))
            {
                using (var fileObject = System.IO.File.OpenRead(file))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CellSurface));

                    return serializer.ReadObject(fileObject) as CellSurface;
                }
            }

            throw new System.IO.FileNotFoundException("File not found.", file);
        }

        private class CellEffectData
        {
            public ICellEffect Effect;
            public List<Cell> Cells;

            public CellEffectData(ICellEffect effect)
            {
                Effect = effect;
                Cells = new List<Cell>();
            }
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return ((IEnumerable<Cell>)Cells).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }
    }
}
