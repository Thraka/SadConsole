using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System;
using System.Runtime.Serialization;
using SadConsole.Surfaces;
using SadConsole.Effects;
using SadConsole;
//using SadConsole.Effects;

namespace SadConsole
{
    //TODO: Cursor should have option to not use PrintAppearance but just place the character using existing appearance of cell
    public class Cursor
    {
        private SurfaceEditor editor;
        private Point position = new Point();

        private int cursorCharacter = 95;

        /// <summary>
        /// Cell used to render the cursor on the screen.
        /// </summary>
        public Cell CursorRenderCell { get; set; }

        /// <summary>
        /// Appearance used when printing text.
        /// </summary>
        public Cell PrintAppearance { get; set; }

        /// <summary>
        /// This effect is applied to each cell printed by the cursor.
        /// </summary>
        public ICellEffect PrintEffect { get; set; }

        /// <summary>
        /// This is the cursor visible effect, like blinking.
        /// </summary>
        public ICellEffect CursorEffect { get; set; }

        /// <summary>
        /// When true, indicates that the cursor, when printing, should not use the <see cref="PrintAppearance"/> property in determining the color/effect of the cell, but keep the cell the same as it was.
        /// </summary>
        public bool PrintOnlyCharacterData { get; set; }

        /// <summary>
        /// Shows or hides the cursor. This does not affect how the cursor operates.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the location of the cursor on the console.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set
            {
                if (editor != null)
                {
                    var old = position;

                    if (!(value.X < 0 || value.X >= editor.TextSurface.Width))
                        position.X = value.X;
                    if (!(value.Y < 0 || value.Y >= editor.TextSurface.Height))
                        position.Y = value.Y;

                    if (position != null)
                        editor.TextSurface.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// When true, prevents the <see cref="Print"/> method from breaking words up by spaces when wrapping lines.
        /// </summary>
        public bool DisableWordBreak = false;

        /// <summary>
        /// Enables linux-like string parsing where a \n behaves like a \r\n.
        /// </summary>
        public bool UseLinuxLineEndings = false;

        public bool UseStringParser = false;

        /// <summary>
        /// Gets or sets the row of the cursor postion.
        /// </summary>
        public int Row
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        /// <summary>
        /// Gets or sets the column of the cursor postion.
        /// </summary>
        public int Column
        {
            get { return position.X; }
            set { position.X = value; }
        }

        /// <summary>
        /// Indicates that the when the cursor goes past the last cell of the console, that the rows should be shifted up when the cursor is automatically reset to the next line.
        /// </summary>
        public bool AutomaticallyShiftRowsUp { get; set; }

        /// <summary>
        /// Creates a new instance of the cursor class that will work with the specified console.
        /// </summary>
        /// <param name="console">The console this cursor will print on.</param>
        public Cursor(SurfaceEditor console)
        {
            editor = console;

            Constructor();
        }

        public Cursor(ISurface surface)
        {
            editor = new SurfaceEditor(surface);

            Constructor();
        }

        private void Constructor()
        {
            IsVisible = false;
            AutomaticallyShiftRowsUp = true;

            PrintAppearance = new Cell(Color.White, Color.Black, 0);

            CursorRenderCell = new Cell(Color.White, Color.Transparent, cursorCharacter);

            ResetCursorEffect();
        }

        internal Cursor()
        {

        }

        /// <summary>
        /// Sets the console this cursor is targetting.
        /// </summary>
        /// <param name="console">The console the cursor works with.</param>
        internal void AttachConsole(SurfaceEditor console)
        {
            editor = console;
        }

        /// <summary>
        /// Resets the <see cref="CursorRenderCell"/> back to the default.
        /// </summary>
        public void ResetCursorEffect()
        {
            SadConsole.Effects.Blink blinkEffect = new Effects.Blink();
            blinkEffect.BlinkSpeed = 0.35f;
            CursorEffect = blinkEffect;
        }

        /// <summary>
        /// Resets the cursor appearance to the console's default foreground and background.
        /// </summary>
        /// <returns>This cursor object.</returns>
        /// <exception cref="Exception">Thrown when the backing console's CellData is null.</exception>
        public Cursor ResetAppearanceToConsole()
        {
            if (editor.TextSurface != null)
                PrintAppearance = new Cell(editor.TextSurface.DefaultForeground, editor.TextSurface.DefaultBackground, 0);
            else
                throw new Exception("CellData of the attached console is null. Cannot reset appearance.");

            return this;
        }
        

        private void PrintGlyph(ColoredGlyph glyph, ColoredString settings)
        {
            var cell = editor.TextSurface.Cells[position.Y * editor.TextSurface.Width + position.X];

            if (!PrintOnlyCharacterData)
            {
                if (!settings.IgnoreBackground)
                    cell.Background = glyph.Background;
                if (!settings.IgnoreForeground)
                    cell.Foreground = glyph.Foreground;
                //if (!settings.IgnoreEffect)
                //    cell.Effect = glyph.Effect;
                if (!settings.IgnoreMirror)
                    cell.Mirror = glyph.Mirror;
            }

            if (!settings.IgnoreGlyph)
                cell.Glyph = glyph.GlyphCharacter;

            position.X += 1;
            if (position.X >= editor.TextSurface.Width)
            {
                position.X = 0;
                position.Y += 1;

                if (position.Y >= editor.TextSurface.Height)
                {
                    position.Y -= 1;

                    if (AutomaticallyShiftRowsUp)
                    {
                        editor.ShiftUp();
                    }
                }
            }

            editor.TextSurface.IsDirty = true;
        }

        /// <summary>
        /// Prints text to the console using the default print appearance.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <returns>Returns this cursor object.</returns>
        public Cursor Print(string text)
        {
            Print(text, PrintAppearance, PrintEffect);
            return this;
        }

        /// <summary>
        /// Prints text on the console.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <param name="template">The way the text will look when it is printed.</param>
        /// <param name="templateEffect">Effect to apply to the text as its printed.</param>
        /// <returns>Returns this cursor object.</returns>
        public Cursor Print(string text, Cell template, Effects.ICellEffect templateEffect)
        {
            ColoredString coloredString;

            if (UseStringParser)
            {
                coloredString = ColoredString.Parse(text, position.Y * editor.TextSurface.Width + position.X, editor.TextSurface, editor, new StringParser.ParseCommandStacks());
            }
            else
            {
                coloredString = text.CreateColored(template.Foreground, template.Background, template.Mirror);
                coloredString.SetEffect(templateEffect);
            }

            return Print(coloredString);
        }

        /// <summary>
        /// Prints text to the console using the appearance of the colored string.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <returns>Returns this cursor object.</returns>
        public Cursor Print(ColoredString text)
        {
            // If we don't want the pretty print, or we're printing a single character (for example, from keyboard input)
            // Then use the pretty print system.
            if (!DisableWordBreak && text.String.Length != 1)
            {
                // Prep
                ColoredGlyph glyph;
                ColoredGlyph spaceGlyph = text[0].Clone();
                spaceGlyph.GlyphCharacter = ' ';
                string stringText = text.String.TrimEnd(' ');

                // Pull any starting spaces off
                var newStringText = stringText.TrimStart(' ');
                int spaceCount = stringText.Length - newStringText.Length;

                for (int i = 0; i < spaceCount; i++)
                    PrintGlyph(spaceGlyph, text);
                    
                if (spaceCount != 0)
                    text = text.SubString(spaceCount, text.Count - spaceCount);

                stringText = newStringText;
                string[] parts = stringText.Split(' ');
                
                // Start processing the string
                int c = 0;

                for (int wordMajor = 0; wordMajor < parts.Length; wordMajor++)
                {
                    // Words broken up by spaces = parts
                    if (parts[wordMajor].Length != 0)
                    {
                        // Parts broken by new lines = newLineParts
                        string[] newlineParts = parts[wordMajor].Split('\n');

                        for (int indexNL = 0; indexNL < newlineParts.Length; indexNL++)
                        {
                            if (newlineParts[indexNL].Length != 0)
                            {
                                int currentLine = position.Y;

                                // New line parts broken up by carriage returns = returnParts
                                string[] returnParts = newlineParts[indexNL].Split('\r');

                                for (int indexR = 0; indexR < returnParts.Length; indexR++)
                                {
                                    // If the text we'll print will move off the edge, fill with spaces to get a fresh line
                                    if (returnParts[indexR].Length > editor.Width - position.X && position.X != 0)
                                    {
                                        var spaces = editor.Width - position.X;

                                        // Fill rest of line with spaces
                                        for (int i = 0; i < spaces; i++)
                                            PrintGlyph(spaceGlyph, text);
                                    }

                                    // Print the rest of the text as normal.
                                    for (int i = 0; i < returnParts[indexR].Length; i++)
                                    {
                                        glyph = text[c];

                                        PrintGlyph(glyph, text);

                                        c++;
                                    }

                                    // If we had a \r in the string, handle it by going back
                                    if (returnParts.Length != 1 && indexR != returnParts.Length - 1)
                                    {
                                        // Wrapped to a new line through print glyph, which triggerd \r\n. We don't want the \n so return back.
                                        if (position.X == 0 && position.Y != currentLine)
                                            position.Y -= 1;
                                        else
                                            CarriageReturn();
                                        c++;
                                    }
                                }
                            }

                            // We had \n in the string, handle them.
                            if (newlineParts.Length != 1 && indexNL != newlineParts.Length - 1)
                            {
                                if (!UseLinuxLineEndings)
                                    LineFeed();
                                else
                                    NewLine();
                                c++;
                            }
                        }
                    }

                    // Not last part
                    if (wordMajor != parts.Length - 1 && position.X != 0)
                    {
                        PrintGlyph(spaceGlyph, text);
                        c++;
                    }
                    else
                        c++;
                }
            }
            else
            {
                bool movedLines = false;
                int oldLine = position.Y;

                foreach (var glyph in text)
                {
                    // Check if the previous print moved us down a line (from print at end of the line) and move use back for the \r
                    if (movedLines)
                    {
                        if (position.X == 0 && glyph.GlyphCharacter == '\r')
                        {
                            position.Y -= 1;
                            continue;
                        }
                        else
                            movedLines = false;
                    }

                    if (glyph.GlyphCharacter == '\r')
                        CarriageReturn();

                    else if (glyph.GlyphCharacter == '\n')
                    {
                        if (!UseLinuxLineEndings)
                            LineFeed();
                        else
                            NewLine();
                    }
                    else
                    {
                        PrintGlyph(glyph, text);

                        // Lines changed and it wasn't a \n that caused it, so it was a print that did it.
                        movedLines = position.Y != oldLine;
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Returns the cursor to the start of the current line.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor CarriageReturn()
        {
            position.X = 0;
            return this;
        }

        /// <summary>
        /// Moves the cursor down a line.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor LineFeed()
        {
            if (position.Y == editor.TextSurface.Height - 1)
            {
                editor.ShiftUp();
                //if (((CustomConsole)_console.Target).Data.ResizeOnShift)
                //    _position.Y++;
            }
            else
                position.Y++;

            return this;
        }

        /// <summary>
        /// Calls the <see cref="M:CarriageReturn()"/> and <see cref="M:LineFeed()"/> methods in a single call.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor NewLine()
        {
            return CarriageReturn().LineFeed();
        }

        /// <summary>
        /// Moves the cusor up by the specified amount of lines.
        /// </summary>
        /// <param name="amount">The amount of lines to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Up(int amount)
        {
            int newY = position.Y - amount;

            if (newY < 0)
                newY = 0;

            Position = new Point(position.X, newY);
            return this;
        }

        /// <summary>
        /// Moves the cusor down by the specified amount of lines.
        /// </summary>
        /// <param name="amount">The amount of lines to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Down(int amount)
        {
            int newY = position.Y + amount;

            if (newY >= editor.TextSurface.Height)
                newY = editor.TextSurface.Height - 1;

            Position = new Point(position.X, newY);
            return this;
        }

        /// <summary>
        /// Moves the cusor left by the specified amount of columns.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Left(int amount)
        {
            int newX = position.X - amount;

            if (newX < 0)
                newX = 0;

            Position = new Point(newX, position.Y);
            return this;
        }

        /// <summary>
        /// Moves the cusor left by the specified amount of columns, wrapping the cursor if needed.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor LeftWrap(int amount)
        {
            int index = editor.TextSurface.GetIndexFromPoint(this.position) - amount;

            if (index < 0)
                index = 0;

            this.position = editor.TextSurface.GetPointFromIndex(index);

            return this;
        }

        /// <summary>
        /// Moves the cusor right by the specified amount of columns.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Right(int amount)
        {
            int newX = position.X + amount;

            if (newX >= editor.TextSurface.Width)
                newX = editor.TextSurface.Width - 1;

            Position = new Point(newX, position.Y);
            return this;
        }

        /// <summary>
        /// Moves the cusor right by the specified amount of columns, wrapping the cursor if needed.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor RightWrap(int amount)
        {
            int index = editor.TextSurface.GetIndexFromPoint(this.position) + amount;

            if (index > editor.TextSurface.Cells.Length)
                index = editor.TextSurface.Cells.Length - 1;

            this.position = editor.TextSurface.GetPointFromIndex(index);

            return this;
        }
        
        public virtual void Render(SpriteBatch batch, Font font, Rectangle renderArea)
        {
            batch.Draw(font.FontImage, renderArea, font.GlyphRects[font.SolidGlyphIndex], CursorRenderCell.Background, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
            batch.Draw(font.FontImage, renderArea, font.GlyphRects[CursorRenderCell.Glyph], CursorRenderCell.Foreground, 0f, Vector2.Zero, SpriteEffects.None, 0.7f);
        }

        internal void Update(TimeSpan elapsed)
        {
            if (CursorEffect != null)
            {
                CursorEffect.Update(elapsed.TotalSeconds);

                if (CursorEffect.Apply(CursorRenderCell))
                    editor.TextSurface.IsDirty = true;
            }
        }
    }
}
