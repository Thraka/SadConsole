#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endif

namespace SadConsole
{
    using System;
    using SadConsole.Effects;

    /// <summary>
    /// A cursor that is attached to a <see cref="Console"/> used for printing.
    /// </summary>
    public class Cursor
    {
        private CellSurface editor;
        private Cell _cursorRenderCell;
        private Point position = new Point();

        private readonly int cursorCharacter = 219;

        /// <summary>
        /// Cell used to render the cursor on the screen.
        /// </summary>
        public Cell CursorRenderCell
        {
            get => _cursorRenderCell;
            set
            {
                CursorEffect?.ClearCell(_cursorRenderCell);

                _cursorRenderCell = value ?? throw new NullReferenceException("The render cell cannot be null. To hide the cursor, use the IsVisible property.");

                CursorEffect?.AddCell(_cursorRenderCell);
            }
        }

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
        /// When true, allows the <see cref="ProcessKeyboard(Input.Keyboard)"/> method to run.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the location of the cursor on the console.
        /// </summary>
        public Point Position
        {
            get => position;
            set
            {
                if (editor != null)
                {
                    Point old = position;

                    if (!(value.X < 0 || value.X >= editor.Width))
                    {
                        position.X = value.X;
                    }

                    if (!(value.Y < 0 || value.Y >= editor.Height))
                    {
                        position.Y = value.Y;
                    }

                    if (position != null)
                    {
                        editor.IsDirty = true;
                    }
                }
            }
        }

        /// <summary>
        /// When true, prevents the any print method from breaking words up by spaces when wrapping lines.
        /// </summary>
        public bool DisableWordBreak = false;

        /// <summary>
        /// Enables linux-like string parsing where a \n behaves like a \r\n.
        /// </summary>
        public bool UseLinuxLineEndings = false;

        /// <summary>
        /// Calls <see cref="ColoredString.Parse"/> to create a colored string when using <see cref="Print(string)"/> or <see cref="Print(string, Cell, ICellEffect)"/>
        /// </summary>
        public bool UseStringParser = false;

        /// <summary>
        /// Gets or sets the row of the cursor postion.
        /// </summary>
        public int Row
        {
            get => position.Y;
            set => position.Y = value;
        }

        /// <summary>
        /// Gets or sets the column of the cursor postion.
        /// </summary>
        public int Column
        {
            get => position.X;
            set => position.X = value;
        }

        /// <summary>
        /// Indicates that the when the cursor goes past the last cell of the console, that the rows should be shifted up when the cursor is automatically reset to the next line.
        /// </summary>
        public bool AutomaticallyShiftRowsUp { get; set; }

        /// <summary>
        /// Creates a new instance of the cursor class that will work with the specified console.
        /// </summary>
        /// <param name="console">The console this cursor will print on.</param>
        public Cursor(CellSurface console)
        {
            editor = console;

            Constructor();
        }

        private void Constructor()
        {
            IsEnabled = true;
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
        public void AttachSurface(CellSurface console)
        {
            editor = console;
            Position = Position;
        }

        /// <summary>
        /// Resets the <see cref="CursorRenderCell"/> back to the default.
        /// </summary>
        public Cursor ResetCursorEffect()
        {
            SadConsole.Effects.Blink blinkEffect = new Effects.Blink
            {
                BlinkSpeed = 0.35f
            };
            CursorEffect = blinkEffect;
            CursorEffect.AddCell(CursorRenderCell);
            return this;
        }

        /// <summary>
        /// Resets the cursor appearance to the console's default foreground and background.
        /// </summary>
        /// <returns>This cursor object.</returns>
        /// <exception cref="Exception">Thrown when the backing console's CellData is null.</exception>
        public Cursor ResetAppearanceToConsole()
        {
            if (editor != null)
            {
                PrintAppearance = new Cell(editor.DefaultForeground, editor.DefaultBackground, 0);
            }
            else
            {
                throw new Exception("Attached console is null. Cannot reset appearance.");
            }

            return this;
        }

        /// <summary>
        /// Sets <see cref="PrintAppearance"/>.
        /// </summary>
        /// <param name="appearance">The appearance to set.</param>
        /// <returns>This cursor object.</returns>
        public Cursor SetPrintAppearance(Cell appearance)
        {
            PrintAppearance = appearance;
            return this;
        }

        private void PrintGlyph(ColoredGlyph glyph, ColoredString settings)
        {
            Cell cell = editor.Cells[position.Y * editor.Width + position.X];

            if (!PrintOnlyCharacterData)
            {
                if (!settings.IgnoreGlyph)
                {
                    cell.Glyph = glyph.GlyphCharacter;
                }

                if (!settings.IgnoreBackground)
                {
                    cell.Background = glyph.Background;
                }

                if (!settings.IgnoreForeground)
                {
                    cell.Foreground = glyph.Foreground;
                }

                if (!settings.IgnoreMirror)
                {
                    cell.Mirror = glyph.Mirror;
                }

                if (!settings.IgnoreEffect)
                {
                    editor.SetEffect(cell, glyph.Effect);
                }
            }
            else if (!settings.IgnoreGlyph)
            {
                cell.Glyph = glyph.GlyphCharacter;
            }

            position.X += 1;
            if (position.X >= editor.Width)
            {
                position.X = 0;
                position.Y += 1;

                if (position.Y >= editor.Height)
                {
                    position.Y -= 1;

                    if (AutomaticallyShiftRowsUp)
                    {
                        editor.ShiftUp();
                    }
                }
            }

            editor.IsDirty = true;
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
                coloredString = ColoredString.Parse(text, position.Y * editor.Width + position.X, editor, new StringParser.ParseCommandStacks());
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
            if (text.Count == 0)
            {
                return this;
            }

            CursorEffect?.Restart();

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
                string newStringText = stringText.TrimStart(' ');
                int spaceCount = stringText.Length - newStringText.Length;

                for (int i = 0; i < spaceCount; i++)
                {
                    PrintGlyph(spaceGlyph, text);
                }

                if (spaceCount != 0)
                {
                    text = text.SubString(spaceCount, text.Count - spaceCount);
                }

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
                                        int spaces = editor.Width - position.X;

                                        // Fill rest of line with spaces
                                        for (int i = 0; i < spaces; i++)
                                        {
                                            PrintGlyph(spaceGlyph, text);
                                        }
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
                                        {
                                            position.Y -= 1;
                                        }
                                        else
                                        {
                                            CarriageReturn();
                                        }

                                        c++;
                                    }
                                }
                            }

                            // We had \n in the string, handle them.
                            if (newlineParts.Length != 1 && indexNL != newlineParts.Length - 1)
                            {
                                if (!UseLinuxLineEndings)
                                {
                                    LineFeed();
                                }
                                else
                                {
                                    NewLine();
                                }

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
                    {
                        c++;
                    }
                }
            }
            else
            {
                bool movedLines = false;
                int oldLine = position.Y;

                foreach (ColoredGlyph glyph in text)
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
                        {
                            movedLines = false;
                        }
                    }

                    if (glyph.GlyphCharacter == '\r')
                    {
                        CarriageReturn();
                    }
                    else if (glyph.GlyphCharacter == '\n')
                    {
                        if (!UseLinuxLineEndings)
                        {
                            LineFeed();
                        }
                        else
                        {
                            NewLine();
                        }
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
            if (position.Y == editor.Height - 1)
            {
                editor.ShiftUp();
                //if (((CustomConsole)_console.Target).Data.ResizeOnShift)
                //    _position.Y++;
            }
            else
            {
                position.Y++;
            }

            return this;
        }

        /// <summary>
        /// Calls the <see cref="CarriageReturn"/> and <see cref="LineFeed"/> methods in a single call.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor NewLine() => CarriageReturn().LineFeed();

        /// <summary>
        /// Moves the cursor to the specified position.
        /// </summary>
        /// <param name="position">The destination of the cursor.</param>
        /// <returns>This cursor object.</returns>
        public Cursor Move(Point position)
        {
            Position = position;
            return this;
        }


        /// <summary>
        /// Moves the cursor to the specified position.
        /// </summary>
        /// <param name="x">The x (horizontal) of the position.</param>
        /// <param name="y">The x (vertical) of the position.</param>
        /// <returns></returns>
        public Cursor Move(int x, int y)
        {
            Position = new Point(x, y);
            return this;
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
            {
                newY = 0;
            }

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

            if (newY >= editor.Height)
            {
                newY = editor.Height - 1;
            }

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
            {
                newX = 0;
            }

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
            int index = editor.GetIndexFromPoint(position) - amount;

            if (index < 0)
            {
                index = 0;
            }

            position = editor.GetPointFromIndex(index);

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

            if (newX >= editor.Width)
            {
                newX = editor.Width - 1;
            }

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
            int index = editor.GetIndexFromPoint(position) + amount;

            if (index > editor.Cells.Length)
            {
                index = editor.Cells.Length - 1;
            }

            position = editor.GetPointFromIndex(index);

            return this;
        }

        /// <inheritdoc />
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

                if (CursorEffect.UpdateCell(CursorRenderCell))
                {
                    editor.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Automates the cursor based on keyboard input.
        /// </summary>
        /// <param name="info">The state of the keyboard</param>
        /// <returns>Returns true when the keyboard caused the cursor to do something.</returns>
        public virtual bool ProcessKeyboard(Input.Keyboard info)
        {
            if (!IsEnabled)
            {
                return false;
            }

            bool didSomething = false;

            foreach (Input.AsciiKey key in info.KeysPressed)
            {
                if (key.Character == '\0')
                {
                    switch (key.Key)
                    {
                        case Keys.Space:
                            Print(key.Character.ToString());
                            didSomething = true;
                            break;
                        case Keys.Enter:
                            CarriageReturn().LineFeed();
                            didSomething = true;
                            break;

                        case Keys.Pause:
                        case Keys.Escape:
                        case Keys.F1:
                        case Keys.F2:
                        case Keys.F3:
                        case Keys.F4:
                        case Keys.F5:
                        case Keys.F6:
                        case Keys.F7:
                        case Keys.F8:
                        case Keys.F9:
                        case Keys.F10:
                        case Keys.F11:
                        case Keys.F12:
                        case Keys.CapsLock:
                        case Keys.NumLock:
                        case Keys.PageUp:
                        case Keys.PageDown:
                        case Keys.Home:
                        case Keys.End:
                        case Keys.LeftShift:
                        case Keys.RightShift:
                        case Keys.LeftAlt:
                        case Keys.RightAlt:
                        case Keys.LeftControl:
                        case Keys.RightControl:
                        case Keys.LeftWindows:
                        case Keys.RightWindows:
                        case Keys.F13:
                        case Keys.F14:
                        case Keys.F15:
                        case Keys.F16:
                        case Keys.F17:
                        case Keys.F18:
                        case Keys.F19:
                        case Keys.F20:
                        case Keys.F21:
                        case Keys.F22:
                        case Keys.F23:
                        case Keys.F24:
                            //this._virtualCursor.Print(key.Character.ToString());
                            break;
                        case Keys.Up:
                            Up(1);
                            didSomething = true;
                            break;
                        case Keys.Left:
                            Left(1);
                            didSomething = true;
                            break;
                        case Keys.Right:
                            Right(1);
                            didSomething = true;
                            break;
                        case Keys.Down:
                            Down(1);
                            didSomething = true;
                            break;
                        case Keys.None:
                            break;
                        case Keys.Back:
                            Left(1).Print(" ").Left(1);
                            didSomething = true;
                            break;
                        default:
                            Print(key.Character.ToString());
                            didSomething = true;
                            break;
                    }
                }
                else
                {
                    Print(key.Character.ToString());
                    didSomething = true;
                }
            }

            return didSomething;
        }
    }
}
