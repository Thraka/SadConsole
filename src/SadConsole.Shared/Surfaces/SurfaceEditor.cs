using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using SpriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects;

using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Effects;
using System.Runtime.Serialization;
using SadConsole.StringParser;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// Provides methods to manipulate a <see cref="ISurfaceRendered"/>.
    /// </summary>
    [DataContract]
    public class SurfaceEditor
    {
        protected ISurface textSurface;
        
        public int TimesShiftedDown;
        public int TimesShiftedRight;
        public int TimesShiftedLeft;
        public int TimesShiftedUp;

        /// <summary>
        /// The width of the text surface being edited.
        /// </summary>
        public int Width { get { return textSurface.Width; } }

        /// <summary>
        /// The height of the text surface being edited.
        /// </summary>
        public int Height { get { return textSurface.Height; } }

        /// <summary>
        /// When true, the <see cref="ColoredString.Parse(string, int, ISurface, ParseCommandStacks)"/> command is used to print strings.
        /// </summary>
        public bool UsePrintProcessor = true;

        /// <summary>
        /// The text surface being changed.
        /// </summary>
        [IgnoreDataMember]
        public ISurface TextSurface
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
        /// The effects manager associated with the <see cref="TextSurface"/>.
        /// </summary>
        /// <remarks>
        /// When the <see cref="TextSurface"/> property is set, a new <see cref="EffectsManager"/> instance is created.
        /// </remarks>
        [IgnoreDataMember]
        public EffectsManager Effects { get; set; }

        /// <summary>
        /// Gets a cell based on it's coordinates on the surface.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The indicated cell.</returns>
        public Cell this[int x, int y]
        {
            get { return textSurface.Cells[y * textSurface.Width + x]; }
            protected set { textSurface.Cells[y * textSurface.Width + x] = value; }
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
        public SurfaceEditor(ISurface surface)
        {
            TextSurface = surface;
        }
#endregion

        /// <summary>
        /// Called when the <see cref="TextSurface"/> property is changed. Sets <see cref="Effects"/> to a new instance of <see cref="EffectsManager"/>.
        /// </summary>
        /// <param name="oldSurface">The previous text surface.</param>
        /// <param name="newSurface">The new text surface.</param>
        protected virtual void OnSurfaceChanged(ISurface oldSurface, ISurface newSurface)
        {
            Effects = new EffectsManager(newSurface);
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

            textSurface.IsDirty = true;
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
            return x >= 0 && x < textSurface.Width && y >= 0 && y < textSurface.Height;
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
            if (x >= 0 && x < textSurface.Width && y >= 0 && y < textSurface.Height)
            {
                index = y * textSurface.Width + x;
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


#region Cell Manipulation

#region Cell Specifics
        /// <summary>
        /// Changes the glyph of a specified cell to a new value.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph of the cell.</param>
        public void SetGlyph(int x, int y, int glyph)
        {
            textSurface.Cells[y * textSurface.Width + x].Glyph = glyph;
            textSurface.IsDirty = true;
        }
        /// <summary>
        /// Changes the glyph, foreground, and background of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground)
        {
            int index = y * textSurface.Width + x;

            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].Glyph = glyph;
            textSurface.IsDirty = true;
        }
        /// <summary>
        /// Changes the glyph, foreground, and background of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground, Color background)
        {
            int index = y * textSurface.Width + x;

            textSurface.Cells[index].Background = background;
            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].Glyph = glyph;
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Changes the glyph, foreground, background, and effect of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="glyph">The desired glyph.</param>
        /// <param name="foreground">The desired foreground.</param>
        /// <param name="background">The desired background.</param>
        /// <param name="effect">Sets the effect of the cell</param>
        public void SetGlyph(int x, int y, int glyph, Color foreground, Color background, SpriteEffects mirror)
        {
            int index = y * textSurface.Width + x;

            textSurface.Cells[index].Background = background;
            textSurface.Cells[index].Foreground = foreground;
            textSurface.Cells[index].Glyph = glyph;
            textSurface.Cells[index].Mirror = mirror;

            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Gets the glyph of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The glyph index.</returns>
        public int GetGlyph(int x, int y)
        {
            return textSurface.Cells[y * textSurface.Width + x].Glyph;
        }

        /// <summary>
        /// Changes the foreground of a specified cell to a new color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetForeground(int x, int y, Color color)
        {
            textSurface.Cells[y * textSurface.Width + x].Foreground = color;
            textSurface.IsDirty = true;
        }
        /// <summary>
        /// Gets the foreground of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetForeground(int x, int y)
        {
            return textSurface.Cells[y * textSurface.Width + x].Foreground;
        }

        /// <summary>
        /// Changes the background of a cell to the specified color.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="color">The desired color of the cell.</param>
        public void SetBackground(int x, int y, Color color)
        {
            textSurface.Cells[y * textSurface.Width + x].Background = color;
            textSurface.IsDirty = true;
        }
        /// <summary>
        /// Gets the background of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public Color GetBackground(int x, int y)
        {
            return textSurface.Cells[y * textSurface.Width + x].Background;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int x, int y, Effects.ICellEffect effect)
        {
            Effects.SetEffect(textSurface[y * textSurface.Width + x], effect);
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a cell to the specified effect.
        /// </summary>
        /// <param name="index">Index of the cell.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(int index, ICellEffect effect)
        {
            Effects.SetEffect(textSurface[index], effect);
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Changes the effect of a list of cells to the specified effect.
        /// </summary>
        /// <param name="cells">The cells for the effect.</param>
        /// <param name="effect">The desired effect.</param>
        public void SetEffect(IEnumerable<Cell> cells, Effects.ICellEffect effect)
        {
            Effects.SetEffect(cells, effect);
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Gets the effect of the specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The effect.</returns>
        public Effects.ICellEffect GetEffect(int x, int y)
        {
            return Effects.GetEffect(textSurface[x, y]);
        }

        /// <summary>
        /// Changes the appearance of the cell. The appearance represents the look of a cell and will first be cloned, then applied to the cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="appearance">The desired appearance of the cell. A null value cannot be passed.</param>
        public void SetCell(int x, int y, Cell appearance)
        {
            if (appearance == null)
                throw new NullReferenceException("Appearance may not be null.");

            appearance.CopyAppearanceTo(textSurface.Cells[y * textSurface.Width + x]);
            textSurface.IsDirty = true;
        }
        /// <summary>
        /// Gets the appearance of a cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The appearance.</returns>
        public Cell GetCell(int x, int y)
        {
            Cell appearance = new Cell();
            textSurface.Cells[y * textSurface.Width + x].CopyAppearanceTo(appearance);
            return appearance;
        }

        /// <summary>
        /// Gets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <returns>The color.</returns>
        public SpriteEffects GetMirror(int x, int y)
        {
            return textSurface.Cells[y * textSurface.Width + x].Mirror;
        }

        /// <summary>
        /// Sets the sprite effect of a specified cell.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        /// <param name="mirror">The mirror of the cell.</param>
        public void SetMirror(int x, int y, SpriteEffects mirror)
        {
            textSurface.Cells[y * textSurface.Width + x].Mirror = mirror;
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Fills a console with random colors and glyphs.
        /// </summary>
        public void FillWithRandomGarbage(bool useEffect = false)
        {
            //pulse.Reset();
            int charCounter = 0;
            for (int y = 0; y < textSurface.Height; y++)
            {
                for (int x = 0; x < textSurface.Width; x++)
                {
                    SetGlyph(x, y, charCounter);
                    SetForeground(x, y, new Color((byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)255));
                    SetBackground(x, y, textSurface.DefaultBackground);
                    SetBackground(x, y, new Color((byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)Global.Random.Next(0, 256), (byte)255));
                    SetMirror(x, y, (SpriteEffects)Global.Random.Next(0, 4));
                    charCounter++;
                    if (charCounter > 255)
                        charCounter = 0;
                }
            }
            textSurface.IsDirty = true;
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

            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;

            if (!UsePrintProcessor)
            {
                int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    textSurface.Cells[index].Glyph = text[charIndex];
                    charIndex++;
                }
            }
            else
                PrintNoCheck(index, ColoredString.Parse(text, index, textSurface, this));

            textSurface.IsDirty = true;
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

            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;

            if (!UsePrintProcessor)
            {
                int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    textSurface.Cells[index].Glyph = text[charIndex];
                    textSurface.Cells[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behavior = new ParseCommandRecolor() { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behavior);
                PrintNoCheck(index, ColoredString.Parse(text, index, textSurface, this, stacks));
            }
            textSurface.IsDirty = true;
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

            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;

            if (!UsePrintProcessor)
            {
                int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    textSurface.Cells[index].Glyph = text[charIndex];
                    textSurface.Cells[index].Background = background;
                    textSurface.Cells[index].Foreground = foreground;
                    charIndex++;
                }
            }
            else
            {
                var behaviorFore = new ParseCommandRecolor() { R = foreground.R, G = foreground.G, B = foreground.B, A = foreground.A, CommandType = CommandTypes.Foreground };
                var behaviorBack = new ParseCommandRecolor() { R = background.R, G = background.G, B = background.B, A = background.A, CommandType = CommandTypes.Background };
                var stacks = new ParseCommandStacks();
                stacks.AddSafe(behaviorFore);
                stacks.AddSafe(behaviorBack);
                PrintNoCheck(index, ColoredString.Parse(text, index, textSurface, this, stacks));
            }
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location with the specified settings. 
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="foreground">Sets the foreground of all characters in the text.</param>
        /// <param name="background">Sets the background of all characters in the text.</param>
        /// <param name="mirror">The mirror to set on all characters in the text.</param>
        public void Print(int x, int y, string text, Color? foreground = null, Color? background = null, SpriteEffects? mirror = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;

            if (!UsePrintProcessor)
            {
                int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
                int charIndex = 0;
                for (; index < total; index++)
                {
                    textSurface.Cells[index].Glyph = text[charIndex];

                    if (background.HasValue)
                        textSurface.Cells[index].Background = background.Value;
                    if (foreground.HasValue)
                        textSurface.Cells[index].Foreground = foreground.Value;
                    if (mirror.HasValue)
                        textSurface.Cells[index].Mirror = mirror.Value;

                    charIndex++;
                }
            }
            else
            {
                var stacks = new ParseCommandStacks();

                if (foreground.HasValue)
                    stacks.AddSafe(new ParseCommandRecolor() { R = foreground.Value.R, G = foreground.Value.G, B = foreground.Value.B, A = foreground.Value.A, CommandType = CommandTypes.Foreground });

                if (background.HasValue)
                    stacks.AddSafe(new ParseCommandRecolor() { R = background.Value.R, G = background.Value.G, B = background.Value.B, A = background.Value.A, CommandType = CommandTypes.Background });

                if (mirror.HasValue)
                    stacks.AddSafe(new ParseCommandMirror() { Mirror = mirror.Value, CommandType = CommandTypes.Mirror });

                PrintNoCheck(index, ColoredString.Parse(text, index, textSurface, this, stacks));
            }
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        /// <param name="appearance">The appearance of the cell</param>
        /// <param name="effect">An optional effect to apply to the printed cells.</param>
        public void Print(int x, int y, string text, Cell appearance, ICellEffect effect = null)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;
            int total = index + text.Length > textSurface.Cells.Length ? textSurface.Cells.Length - index : index + text.Length;
            int charIndex = 0;

            for (; index < total; index++)
            {
                Cell cell = textSurface.Cells[index];
                appearance.CopyAppearanceTo(cell);
                cell.Glyph = text[charIndex];
                Effects.SetEffect(cell, effect);
                charIndex++;
            }
            textSurface.IsDirty = true;
        }

        /// <summary>
        /// Draws the string on the console at the specified location, wrapping if needed.
        /// </summary>
        /// <param name="x">X location of the text.</param>
        /// <param name="y">Y location of the text.</param>
        /// <param name="text">The string to display.</param>
        public void Print(int x, int y, ColoredString text)
        {
            if (x >= textSurface.Width || x < 0 || y >= textSurface.Height || y < 0)
                throw new Exception("X,Y is out of range for Print");

            int index = y * textSurface.Width + x;
            PrintNoCheck(index, text);
            textSurface.IsDirty = true;
        }


        private void PrintNoCheck(int index, ColoredString text)
        {
            int total = index + text.Count > textSurface.Cells.Length ? textSurface.Cells.Length : index + text.Count;
            int charIndex = 0;
            
            for (; index < total; index++)
            {
                if (!text.IgnoreGlyph)
                    textSurface.Cells[index].Glyph = text[charIndex].GlyphCharacter;
                if (!text.IgnoreBackground)
                    textSurface.Cells[index].Background = text[charIndex].Background;
                if (!text.IgnoreForeground)
                    textSurface.Cells[index].Foreground = text[charIndex].Foreground;
                if (!text.IgnoreMirror)
                    textSurface.Cells[index].Mirror = text[charIndex].Mirror;
                if (!text.IgnoreEffect)
                    SetEffect(index, text[charIndex].Effect);
                charIndex++;
            }
            textSurface.IsDirty = true;
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
            return GetString(y * textSurface.Width + x, length);
        }

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public string GetString(int index, int length)
        {
            if (index >= 0 && index < textSurface.Cells.Length)
            {
                StringBuilder sb = new StringBuilder(length);
                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;

                    if (tempIndex < textSurface.Cells.Length)
                        sb.Append((char)textSurface.Cells[tempIndex].Glyph);
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
            return GetStringColored(y * textSurface.Width + x, length);
        }

        /// <summary>
        /// Builds a string from the text surface.
        /// </summary>
        /// <param name="index">Where to start getting characters from.</param>
        /// <param name="length">How many characters to fill the string with.</param>
        /// <returns>A string built from the text surface data.</returns>
        public ColoredString GetStringColored(int index, int length)
        {
            if (index >= 0 && index < textSurface.Cells.Length)
            {
                ColoredString sb = new ColoredString(length);

                int tempIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    tempIndex = i + index;
                    var cell = (Cell)sb[i];
                    if (tempIndex < textSurface.Cells.Length)
                        textSurface.Cells[tempIndex].CopyAppearanceTo(cell);
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
            Fill(textSurface.DefaultForeground, textSurface.DefaultBackground, 0, SpriteEffects.None);
        }

        /// <summary>
        /// Clears a cell. Character is reset to 0, the forground and background is set to default, and effect is set to none.
        /// </summary>
        /// <param name="x">The x location of the cell.</param>
        /// <param name="y">The y location of the cell.</param>
        public void Clear(int x, int y)
        {
            var cell = textSurface.Cells[y * textSurface.Width + x];
            cell.Clear();
            cell.Foreground = textSurface.DefaultForeground;
            cell.Background = textSurface.DefaultBackground;
            cell.Mirror = SpriteEffects.None;
            textSurface.IsDirty = true;
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
                wrappedCells = new List<Tuple<Cell, int>>(textSurface.Height * amount);

                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < textSurface.Width; x++)
                    {
                        var tempCell = new Cell();
                        textSurface.Cells[y * textSurface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (textSurface.Height - amount + y) * textSurface.Width + x));
                    }
                }
            }

            for (int y = amount; y < textSurface.Height; y++)
            {
                for (int x = 0; x < textSurface.Width; x++)
                {
                    Cell destination = textSurface.Cells[(y - amount) * textSurface.Width + x];
                    Cell source = textSurface.Cells[y * textSurface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }


            if (!wrap)
                for (int y = textSurface.Height - amount; y < textSurface.Height; y++)
                {
                    for (int x = 0; x < textSurface.Width; x++)
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
                    destination.Glyph = wrappedCells[i].Item1.Glyph;
                    destination.Mirror = wrappedCells[i].Item1.Mirror;
                }
            textSurface.IsDirty = true;
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
                wrappedCells = new List<Tuple<Cell, int>>(textSurface.Height * amount);

                for (int y = textSurface.Height - amount; y < textSurface.Height; y++)
                {
                    for (int x = 0; x < textSurface.Width; x++)
                    {
                        var tempCell = new Cell();
                        textSurface.Cells[y * textSurface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, (amount - (textSurface.Height - y)) * textSurface.Width + x));
                    }
                }
            }

            for (int y = (textSurface.Height - 1) - amount; y >= 0; y--)
            {
                for (int x = 0; x < textSurface.Width; x++)
                {
                    Cell destination = textSurface.Cells[(y + amount) * textSurface.Width + x];
                    Cell source = textSurface.Cells[y * textSurface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
                for (int y = 0; y < amount; y++)
                {
                    for (int x = 0; x < textSurface.Width; x++)
                    {
                        Cell source = textSurface.Cells[y * textSurface.Width + x];
                        source.Clear();
                    }
                }
            else
                for (int i = 0; i < wrappedCells.Count; i++)
                {
                    Cell destination = textSurface.Cells[wrappedCells[i].Item2];

                    destination.Background = wrappedCells[i].Item1.Background;
                    destination.Foreground = wrappedCells[i].Item1.Foreground;
                    destination.Glyph = wrappedCells[i].Item1.Glyph;
                    destination.Mirror = wrappedCells[i].Item1.Mirror;
                }
            textSurface.IsDirty = true;
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
                wrappedCells = new List<Tuple<Cell, int>>(textSurface.Height * amount);

                for (int x = textSurface.Width - amount; x < textSurface.Width; x++)
                {
                    for (int y = 0; y < textSurface.Height; y++)
                    {
                        var tempCell = new Cell();
                        textSurface.Cells[y * textSurface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * textSurface.Width + amount - (textSurface.Width - x)));
                    }
                }
            }


            for (int x = textSurface.Width - 1 - amount; x >= 0; x--)
            {
                for (int y = 0; y < textSurface.Height; y++)
                {
                    Cell destination = textSurface.Cells[y * textSurface.Width + (x + amount)];
                    Cell source = textSurface.Cells[y * textSurface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < textSurface.Height; y++)
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
                    destination.Glyph = wrappedCells[i].Item1.Glyph;
                    destination.Mirror = wrappedCells[i].Item1.Mirror;
                }
            textSurface.IsDirty = true;
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
                wrappedCells = new List<Tuple<Cell, int>>(textSurface.Height * amount);

                for (int x = 0; x < amount; x++)
                {
                    for (int y = 0; y < textSurface.Height; y++)
                    {
                        var tempCell = new Cell();
                        textSurface.Cells[y * textSurface.Width + x].CopyAppearanceTo(tempCell);

                        wrappedCells.Add(new Tuple<Cell, int>(tempCell, y * textSurface.Width + (textSurface.Width - amount + x)));
                    }
                }
            }

            for (int x = amount; x < textSurface.Width; x++)
            {
                for (int y = 0; y < textSurface.Height; y++)
                {
                    Cell destination = textSurface.Cells[y * textSurface.Width + (x - amount)];
                    Cell source = textSurface.Cells[y * textSurface.Width + x];

                    destination.Background = source.Background;
                    destination.Foreground = source.Foreground;
                    destination.Glyph = source.Glyph;
                    destination.Mirror = source.Mirror;
                }
            }

            if (!wrap)
                for (int x = textSurface.Width - amount; x < textSurface.Width; x++)
                {
                    for (int y = 0; y < textSurface.Height; y++)
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
                    destination.Glyph = wrappedCells[i].Item1.Glyph;
                    destination.Mirror = wrappedCells[i].Item1.Mirror;
                }
            textSurface.IsDirty = true;
        }

        #endregion

        #region Fill

        /// <summary>
        /// Fills the console.
        /// </summary>
        /// <param name="foreground">Foregorund of every cell. If null, skips.</param>
        /// <param name="background">Foregorund of every cell. If null, skips.</param>
        /// <param name="glyph">Glyph of every cell. If null, skips.</param>
        /// <param name="spriteEffect">Sprite effect of every cell. If null, skips.</param>
        public Cell[] Fill(Color? foreground, Color? background, int? glyph, SpriteEffects? spriteEffect = null)
        {
            for (int i = 0; i < textSurface.Cells.Length; i++)
            {
                if (glyph.HasValue)
                    textSurface.Cells[i].Glyph = glyph.Value;
                if (background.HasValue)
                    textSurface.Cells[i].Background = background.Value;
                if (foreground.HasValue)
                    textSurface.Cells[i].Foreground = foreground.Value;
                if (spriteEffect.HasValue)
                    textSurface.Cells[i].Mirror = spriteEffect.Value;
            }

            textSurface.IsDirty = true;
            return textSurface.Cells;
        }

        /// <summary>
        /// Fills the specified area.
        /// </summary>
        /// <param name="area">The area to fill.</param>
        /// <param name="foreground">Foregorund of every cell. If null, skips.</param>
        /// <param name="background">Foregorund of every cell. If null, skips.</param>
        /// <param name="glyph">Glyph of every cell. If null, skips.</param>
        /// <param name="spriteEffect">Sprite effect of every cell. If null, skips.</param>
        public Cell[] Fill(Rectangle area, Color? foreground, Color? background, int? glyph, SpriteEffects? spriteEffect = null)
        {
            // Check for valid rect
            Rectangle consoleArea = new Rectangle(0, 0, textSurface.Width, textSurface.Height);

            if (consoleArea.Contains(area))
            {
                var cells = new Cell[consoleArea.Width * consoleArea.Height];
                int cellIndex = 0;

                for (int x = area.Left; x < area.Left + area.Width; x++)
                {
                    for (int y = area.Top; y < area.Top + area.Height; y++)
                    {
                        Cell cell = textSurface[y * textSurface.Width + x];

                        if (glyph.HasValue)
                            cell.Glyph = glyph.Value;
                        if (background.HasValue)
                            cell.Background = background.Value;
                        if (foreground.HasValue)
                            cell.Foreground = foreground.Value;
                        if (spriteEffect.HasValue)
                            cell.Mirror = spriteEffect.Value;

                        cells[cellIndex] = cell;
                        cellIndex++;
                    }
                }

                textSurface.IsDirty = true;
                return cells;
            }

            return new Cell[] { };
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
