namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Text;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Graphics;
    using SadConsole.Effects;

    //TODO: Cursor should have option to not use PrintAppearance but just place the character using existing appearance of cell
    [DataContract]
    public class Cursor
    {
        private WeakReference _console;
        private Point _position = Point.Zero;

        private int _cursorCharacter = 95;

        /// <summary>
        /// Cell used to render the cursor on the screen.
        /// </summary>
        [DataMember]
        public Cell CursorRenderCell { get; set; }

        /// <summary>
        /// Appearance used when printing text.
        /// </summary>
        [DataMember]
        public ICellAppearance PrintAppearance { get; set; }

        /// <summary>
        /// This effect is applied to each cell printed by the cursor.
        /// </summary>
        [DataMember]
        public ICellEffect PrintEffect { get; set; }

        /// <summary>
        /// When true, indicates that the cursor, when printing, should not use the <see cref="PrintAppearance"/> property in determining the color/effect of the cell, but keep the cell the same as it was.
        /// </summary>
        [DataMember]
        public bool PrintOnlyCharacterData { get; set; }

        /// <summary>
        /// Shows or hides the cursor. This does not affect how the cursor operates.
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the location of the cursor on the console.
        /// </summary>
        [DataMember]
        public Point Position
        {
            get { return _position; }
            set
            {
                if (_console != null)
                {
                    IConsole console = (IConsole)_console.Target;

                    if (!(value.X < 0 || value.X >= console.Data.Width))
                        _position.X = value.X;
                    if (!(value.Y < 0 || value.Y >= console.Data.Height))
                        _position.Y = value.Y;
                }
            }
        }

        /// <summary>
        /// Gets or sets the row of the cursor postion.
        /// </summary>
        public int Row
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        /// <summary>
        /// Gets or sets the column of the cursor postion.
        /// </summary>
        public int Column
        {
            get { return _position.X; }
            set { _position.X = value; }
        }

        /// <summary>
        /// Indicates that the when the cursor goes past the last cell of the console, that the rows should be shifted up when the cursor is automatically reset to the next line.
        /// </summary>
        [DataMember]
        public bool AutomaticallyShiftRowsUp { get; set; }

        /// <summary>
        /// Creates a new instance of the cursor class that will work with the specified console.
        /// </summary>
        /// <param name="console">The console this cursor will print on.</param>
        public Cursor(Console console)
        {
            _console = new WeakReference(console);
            IsVisible = false;

            SadConsole.Effects.Blink blinkEffect = new Effects.Blink();
            blinkEffect.BlinkSpeed = 0.35f;

            PrintAppearance = new CellAppearance(Color.White, Color.Black);
            AutomaticallyShiftRowsUp = true;

            CursorRenderCell = new Cell();
            CursorRenderCell.CharacterIndex = _cursorCharacter;
            CursorRenderCell.Effect = blinkEffect;
            CursorRenderCell.Foreground = Color.White;
            CursorRenderCell.Background = Color.Transparent;
        }

        internal Cursor()
        {

        }

        internal void AttachConsole(Console console)
        {
            _console = new WeakReference(console);
        }

        /// <summary>
        /// Resets the cursor appearance to the console's default foreground and background.
        /// </summary>
        /// <returns>This cursor object.</returns>
        /// <exception cref="Exception">Thrown when the backing console's CellData is null.</exception>
        public Cursor ResetAppearanceToConsole()
        {
            IConsole console = ((IConsole)_console.Target);

            if (console.Data != null)
                PrintAppearance = new CellAppearance(console.Data.DefaultForeground, console.Data.DefaultBackground);
            else
                throw new Exception("CellData of the attached console is null. Cannot reset appearance.");

            return this;
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
        /// Prints text to the console using the appearance of the colored string.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <returns>Returns this cursor object.</returns>
        public Cursor Print(ColoredString text)
        {
            var console = (IConsole)_console.Target;

            foreach (var character in text)
            {
                if (character.Character == '\r')
                    CarriageReturn();

                else if (character.Character == '\n')
                    LineFeed();

                else
                {
                    var cell = console.Data[_position.X, _position.Y];

                    if (!PrintOnlyCharacterData)
                    {
                        if (!text.IgnoreCharacter)
                            cell.CharacterIndex = character.Character;
                        if (!text.IgnoreBackground)
                            cell.Background = character.Background;
                        if (!text.IgnoreForeground)
                            cell.Foreground = character.Foreground;
                        if (!text.IgnoreEffect)
                            console.Data.SetEffect(cell, character.Effect);
                    }

                    _position.X += 1;
                    if (_position.X >= console.Data.Width)
                    {
                        _position.X = 0;
                        _position.Y += 1;

                        if (_position.Y >= console.Data.Height)
                        {
                            _position.Y -= 1;

                            if (AutomaticallyShiftRowsUp)
                            {
                                console.Data.ShiftUp();

                                if (console.Data.ResizeOnShift)
                                    _position.Y++;
                            }
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Prints text on the console.
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <param name="template">The way the text will look when it is printed.</param>
        /// <returns>Returns this cursor object.</returns>
        public Cursor Print(string text, ICellAppearance template, ICellEffect templateEffect)
        {
            // TODO: Look for the flag 7 on settings. This means allow word wrap. So without it we need to test if the text will reach the end of the screen and cut it off.
            //((Console)_console.Target).DrawString(_location.X, _location.Y, text, PrintAppearance.Foreground, PrintAppearance.Background, PrintAppearance.Effect);

            var console = (IConsole)_console.Target;

            foreach (var character in text)
            {
                if (character == '\r')
                {
                    CarriageReturn();
                }
                else if (character == '\n')
                {
                    LineFeed();
                }
                else
                {
                    var cell = console.Data[_position.X, _position.Y];

                    if (!PrintOnlyCharacterData)
                    {
                        template.CopyAppearanceTo(cell);
                        console.Data.SetEffect(cell, templateEffect);
                    }

                    cell.CharacterIndex = character;

                    _position.X += 1;
                    if (_position.X >= console.Data.Width)
                    {
                        _position.X = 0;
                        _position.Y += 1;

                        if (_position.Y >= console.Data.Height)
                        {
                            _position.Y -= 1;

                            if (AutomaticallyShiftRowsUp)
                            {
                                console.Data.ShiftUp();

                                if (console.Data.ResizeOnShift)
                                    _position.Y++;
                            }
                        }
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
            _position.X = 0;
            return this;
        }

        /// <summary>
        /// Moves the cursor down a line.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor LineFeed()
        {
            if (_position.Y == ((IConsole)_console.Target).Data.Height - 1)
            {
                ((IConsole)_console.Target).Data.ShiftUp();
                if (((IConsole)_console.Target).Data.ResizeOnShift)
                    _position.Y++;
            }
            else
                _position.Y++;

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
            int newY = _position.Y - amount;

            if (newY < 0)
                newY = 0;

            Position = new Point(_position.X, newY);
            return this;
        }

        /// <summary>
        /// Moves the cusor down by the specified amount of lines.
        /// </summary>
        /// <param name="amount">The amount of lines to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Down(int amount)
        {
            int newY = _position.Y + amount;

            if (newY >= ((IConsole)_console.Target).Data.Height)
                newY = ((IConsole)_console.Target).Data.Height - 1;

            Position = new Point(_position.X, newY);
            return this;
        }

        /// <summary>
        /// Moves the cusor left by the specified amount of columns.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Left(int amount)
        {
            int newX = _position.X - amount;

            if (newX < 0)
                newX = 0;

            Position = new Point(newX, _position.Y);
            return this;
        }

        /// <summary>
        /// Moves the cusor left by the specified amount of columns, wrapping the cursor if needed.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor LeftWrap(int amount)
        {
            var console = ((IConsole)_console.Target);

            int index = console.Data.GetIndexFromPoint(this._position) - amount;

            if (index < 0)
                index = 0;

            this._position = console.Data.GetPointFromIndex(index);

            return this;
        }

        /// <summary>
        /// Moves the cusor right by the specified amount of columns.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor Right(int amount)
        {
            int newX = _position.X + amount;

            if (newX >= ((IConsole)_console.Target).Data.Width)
                newX = ((IConsole)_console.Target).Data.Width - 1;

            Position = new Point(newX, _position.Y);
            return this;
        }

        /// <summary>
        /// Moves the cusor right by the specified amount of columns, wrapping the cursor if needed.
        /// </summary>
        /// <param name="amount">The amount of columns to move the cursor</param>
        /// <returns>This cursor object.</returns>
        public Cursor RightWrap(int amount)
        {
            var console = ((IConsole)_console.Target);

            int index = console.Data.GetIndexFromPoint(this._position) + amount;

            if (index > console.Data.CellCount)
                index = console.Data.CellCount - 1;

            this._position = console.Data.GetPointFromIndex(index);

            return this;
        }

        public virtual void Render(SpriteBatch batch, Font font, Rectangle renderArea)
        {
            batch.Draw(font.FontImage, renderArea, font.CharacterIndexRects[font.SolidCharacterIndex], CursorRenderCell.ActualBackground, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
            batch.Draw(font.FontImage, renderArea, font.CharacterIndexRects[CursorRenderCell.ActualCharacterIndex], CursorRenderCell.ActualForeground, 0f, Vector2.Zero, SpriteEffects.None, 0.7f);
        }
    }
}
