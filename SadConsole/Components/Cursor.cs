using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using SadConsole.Input;
using SadConsole.Effects;

namespace SadConsole.Components
{
    /// <summary>
    /// A cursor that is attached to a <see cref="Console"/> used for printing.
    /// </summary>
    public class Cursor: IComponent
    {
        private ICellSurface _editor;
        private Point _position = new Point();
        private EffectsManager.ColoredGlyphState _cursorRenderCellState;
        private ColoredGlyph _cursorRenderCell;
        private bool _applyCursorEffect = true;
        private ICellEffect _cursorEffect;
        private Renderers.IRenderStep _cursorRenderStep;

        /// <summary>
        /// The default glyph used for a new cursor. Value 219.
        /// </summary>
        public static readonly int DefaultCursorGlyph = 219;

        /// <summary>
        /// Cell used to render the cursor on the screen.
        /// </summary>
        public ColoredGlyph CursorRenderCell
        {
            get => _cursorRenderCell;
            set
            {
                _cursorRenderCell = value ?? throw new NullReferenceException("The render cell cannot be null. To hide the cursor, use the IsVisible property.");
                _cursorRenderCellState = new EffectsManager.ColoredGlyphState(_cursorRenderCell);
            }
        }

        /// <summary>
        /// Appearance used when printing text. <see cref="PrintOnlyCharacterData"/> must be set to <see langword="false"/> for this to apply.
        /// </summary>
        public ColoredGlyph PrintAppearance { get; set; }

        /// <summary>
        /// This effect is applied to each cell printed by the cursor.
        /// </summary>
        public ICellEffect PrintEffect { get; set; }

        /// <summary>
        /// This is the cursor visible effect, like blinking.
        /// </summary>
        public ICellEffect CursorEffect
        {
            get => _cursorEffect;
            set
            {
                _cursorEffect = value;
                _cursorRenderCellState.RestoreState(ref _cursorRenderCell);
            }
        }


        /// <summary>
        /// Sets the glyph used in rendering. A shortcut to <see cref="CursorRenderCell"/>.
        /// </summary>
        public int CursorGlyph
        {
            get => _cursorRenderCell.Glyph;
            set => CursorRenderCell = new ColoredGlyph(_cursorRenderCell.Foreground, _cursorRenderCell.Background, value);
        }

        /// <summary>
        /// When <see langword="true"/>, indicates that the cursor, when printing, should not use the <see cref="PrintAppearance"/> property in determining the color/effect of the cell, but keep the cell the same as it was.
        /// </summary>
        public bool PrintOnlyCharacterData { get; set; }

        /// <summary>
        /// When <see langword="true"/>, left-clicking on the host surface will reposition the cursor to the clicked position.
        /// </summary>
        public bool MouseClickReposition { get; set; }

        /// <summary>
        /// Shows or hides the cursor. This does not affect how the cursor operates.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// When <see langword="false"/>, prevents the cursor from running on the host.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// When <see langword="false"/>, prevents the <see cref="CursorEffect"/> from being applied.
        /// </summary>
        public bool ApplyCursorEffect
        {
            get => _applyCursorEffect;
            set
            {
                _applyCursorEffect = value;

                // If this is disabled, restore cell state
                if (!_applyCursorEffect)
                    _cursorRenderCellState.RestoreState(ref _cursorRenderCell);
            }
        }

        /// <summary>
        /// When <see langword="true"/>, applies the <see cref="PrintEffect"/> to the cursor when it prints.
        /// </summary>
        public bool UsePrintEffect { get; set; } = true;

        /// <summary>
        /// Gets or sets the location of the cursor on the console.
        /// </summary>
        public Point Position
        {
            get => _position;
            set
            {
                if (_editor != null)
                {
                    if (!(value.X < 0 || value.X >= _editor.Width))
                        _position = new Point(value.X, _position.Y);

                    if (!(value.Y < 0 || value.Y >= _editor.Height))
                        _position = new Point(_position.X, value.Y);
                }
            }
        }

        /// <summary>
        /// When true, prevents the any print method from breaking words up by spaces when wrapping lines.
        /// </summary>
        public bool DisableWordBreak { get; set; } = false;

        /// <summary>
        /// Enables linux-like string parsing where a \n behaves like a \r\n.
        /// </summary>
        public bool UseLinuxLineEndings { get; set; } = false;

        /// <summary>
        /// Calls <see cref="ColoredString.Parse"/> to create a colored string when using <see cref="Print(string)"/> or <see cref="Print(string, ColoredGlyph, ICellEffect)"/>.
        /// </summary>
        public bool UseStringParser { get; set; } = false;

        /// <summary>
        /// Gets or sets the row of the cursor postion.
        /// </summary>
        public int Row
        {
            get => _position.Y;
            set => Position = new Point(_position.X, value);
        }

        /// <summary>
        /// Gets or sets the column of the cursor postion.
        /// </summary>
        public int Column
        {
            get => _position.X;
            set => Position = new Point(value, _position.Y);
        }

        /// <summary>
        /// Indicates that the when the cursor goes past the last cell of the console, that the rows should be shifted up when the cursor is automatically reset to the next line.
        /// </summary>
        public bool AutomaticallyShiftRowsUp { get; set; }

        /// <summary>
        /// Sets the sort order of this component within the host.
        /// </summary>
        public int SortOrder { get; set; }

        bool IComponent.IsUpdate => true;

        bool IComponent.IsRender => false;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => true;

        /// <summary>
        /// Creates a new instance of the cursor as a component.
        /// </summary>
        public Cursor()
        {
            IsEnabled = true;
            IsVisible = true;
            AutomaticallyShiftRowsUp = true;

            PrintAppearance = new ColoredGlyph(Color.White, Color.Black, 0);

            CursorRenderCell = new ColoredGlyph(Color.White, Color.Transparent, DefaultCursorGlyph);

            ApplyDefaultCursorEffect();
        }

        /// <summary>
        /// Creates a new instance of the cursor that works with the specified surface.
        /// </summary>
        /// <param name="surface"></param>
        public Cursor(ICellSurface surface): this() =>
            _editor = surface;

        /// <summary>
        /// Resets the <see cref="CursorRenderCell"/> back to the default.
        /// </summary>
        public Cursor ApplyDefaultCursorEffect()
        {
            if (CursorEffect != null)
                _cursorRenderCellState.RestoreState(ref _cursorRenderCell);

            SadConsole.Effects.Blink blinkEffect = new Effects.Blink
            {
                BlinkSpeed = 0.35f
            };
            CursorEffect = blinkEffect;
            CursorEffect.ApplyToCell(_cursorRenderCell, _cursorRenderCellState);

            return this;
        }

        /// <summary>
        /// Clones and reassigns <see cref="CursorEffect"/> to restart it.
        /// </summary>
        /// <returns></returns>
        public Cursor RestartCursorEffect()
        {
            if (CursorEffect == null) return this;

            _cursorRenderCellState.RestoreState(ref _cursorRenderCell);

            CursorEffect = CursorEffect.Clone();
            CursorEffect.ApplyToCell(_cursorRenderCell, _cursorRenderCellState);

            return this;
        }

        /// <summary>
        /// Sets the cursor appearance to the console's default foreground and background.
        /// </summary>
        /// <returns>This cursor object.</returns>
        /// <exception cref="Exception">Thrown when the cursor is not attached to any surface.</exception>
        public Cursor SetPrintAppearanceToHost()
        {
            if (_editor != null)
            {
                PrintAppearance = new ColoredGlyph(_editor.DefaultForeground, _editor.DefaultBackground, 0);
            }
            else
            {
                throw new Exception("A host is not attached, cannot reset appearance.");
            }

            return this;
        }

        /// <summary>
        /// Sets <see cref="PrintAppearance"/>.
        /// </summary>
        /// <param name="appearance">The appearance to set.</param>
        /// <returns>This cursor object.</returns>
        public Cursor SetPrintAppearance(ColoredGlyph appearance)
        {
            PrintAppearance = appearance;
            return this;
        }

        /// <summary>
        /// Sets <see cref="PrintAppearance"/>.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color</param>
        /// <returns>This cursor object.</returns>
        public Cursor SetPrintAppearance(Color foreground, Color background)
        {
            PrintAppearance = new ColoredGlyph(foreground, background);
            return this;
        }

        /// <summary>
        /// Sets <see cref="PrintAppearance"/>, only changing the foreground color.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <returns>This cursor object.</returns>
        public Cursor SetPrintAppearance(Color foreground)
        {
            PrintAppearance = new ColoredGlyph(foreground, PrintAppearance.Background);
            return this;
        }

        private void PrintGlyph(ColoredString.ColoredGlyphEffect glyph, ColoredString settings)
        {
            ColoredGlyph cell = _editor[_position.Y * _editor.Width + _position.X];

            if (!PrintOnlyCharacterData)
            {
                if (!settings.IgnoreGlyph)
                    cell.Glyph = glyph.GlyphCharacter;

                if (!settings.IgnoreBackground)
                    cell.Background = glyph.Background;

                if (!settings.IgnoreForeground)
                    cell.Foreground = glyph.Foreground;

                if (!settings.IgnoreMirror)
                    cell.Mirror = glyph.Mirror;

                if (!settings.IgnoreEffect)
                    _editor.SetEffect(_position.Y * _editor.Width + _position.X, glyph.Effect);
            }
            else if (!settings.IgnoreGlyph)
                cell.Glyph = glyph.GlyphCharacter;

            (int x, int y) = _position;
            x += 1;

            if (x >= _editor.Width)
            {
                x = 0;
                y += 1;

                if (y >= _editor.Height)
                {
                    y -= 1;

                    if (AutomaticallyShiftRowsUp)
                        _editor.ShiftUp();
                }
            }

            _position = (x, y);

            _editor.IsDirty = true;
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
        public Cursor Print(string text, ColoredGlyph template, Effects.ICellEffect templateEffect)
        {
            ColoredString coloredString;

            if (UseStringParser)
            {
                coloredString = ColoredString.Parse(text, _position.Y * _editor.Width + _position.X, _editor, new StringParser.ParseCommandStacks());
            }
            else
            {
                coloredString = text.CreateColored(template.Foreground, template.Background, template.Mirror);

                if (UsePrintEffect)
                {
                    coloredString.IgnoreEffect = false;
                    coloredString.SetEffect(templateEffect);
                }
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
            if (text.Length == 0)
            {
                return this;
            }

            CursorEffect?.Restart();

            // If we don't want the pretty print, or we're printing a single character (for example, from keyboard input)
            // Then use the pretty print system.
            if (!DisableWordBreak && text.String.Length != 1)
            {
                // Prep
                ColoredString.ColoredGlyphEffect glyph;
                ColoredString.ColoredGlyphEffect spaceGlyph = text[0].Clone();
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
                    text = text.SubString(spaceCount, text.Length - spaceCount);
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
                                int currentLine = _position.Y;

                                // New line parts broken up by carriage returns = returnParts
                                string[] returnParts = newlineParts[indexNL].Split('\r');

                                for (int indexR = 0; indexR < returnParts.Length; indexR++)
                                {
                                    // If the text we'll print will move off the edge, fill with spaces to get a fresh line
                                    if (returnParts[indexR].Length > _editor.Width - _position.X && _position.X != 0)
                                    {
                                        int spaces = _editor.Width - _position.X;

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
                                        if (_position.X == 0 && _position.Y != currentLine)
                                        {
                                            _position = new Point(_position.X, _position.Y - 1);
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
                    if (wordMajor != parts.Length - 1 && _position.X != 0)
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
                int oldLine = _position.Y;

                foreach (ColoredString.ColoredGlyphEffect glyph in text)
                {
                    // Check if the previous print moved us down a line (from print at end of the line) and move use back for the \r
                    if (movedLines)
                    {
                        if (_position.X == 0 && glyph.GlyphCharacter == '\r')
                        {
                            _position = new Point(_position.X, _position.Y - 1);
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
                        movedLines = _position.Y != oldLine;
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
            _position = new Point(0, _position.Y);
            return this;
        }

        /// <summary>
        /// Moves the cursor down a line.
        /// </summary>
        /// <returns>The current cursor object.</returns>
        public Cursor LineFeed()
        {
            if (_position.Y == _editor.Height - 1)
            {
                _editor.ShiftUp();
                //if (((CustomConsole)_console.Target).Data.ResizeOnShift)
                //    _position.Y++;
            }
            else
            {
                _position = new Point(_position.X, _position.Y + 1);
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
            int newY = _position.Y - amount;

            if (newY < 0)
            {
                newY = 0;
            }

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

            if (newY >= _editor.Height)
            {
                newY = _editor.Height - 1;
            }

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
            {
                newX = 0;
            }

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
            int index = Point.ToIndex(_position.X, _position.Y, _editor.Width) - amount;

            if (index < 0)
            {
                index = 0;
            }

            _position = Point.FromIndex(index, _editor.Width);

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

            if (newX >= _editor.Width)
            {
                newX = _editor.Width - 1;
            }

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
            int index = Point.ToIndex(_position.X, _position.Y, _editor.Width) + amount;

            if (index > _editor.Count)
            {
                index = _editor.Count - 1;
            }

            _position = Point.FromIndex(index, _editor.Width);

            return this;
        }

        void IComponent.Update(IScreenObject host, TimeSpan delta)
        {
            if (IsVisible && _applyCursorEffect && CursorEffect != null)
            {
                CursorEffect.Update(delta.TotalSeconds);
                CursorEffect.ApplyToCell(_cursorRenderCell, _cursorRenderCellState);
            }
        }

        void IComponent.Render(IScreenObject host, TimeSpan delta)
        {
            throw new NotImplementedException();
        }

        void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;

            if (MouseClickReposition && state.IsOnScreenObject && state.ScreenObject == host)
            {
                Position = state.CellPosition;
                handled = true;
            }
        }

        void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = false;

            if (!IsEnabled) return;

            foreach (Input.AsciiKey key in keyboard.KeysPressed)
            {
                if (key.Character == '\0')
                {
                    switch (key.Key)
                    {
                        case Keys.Space:
                            Print(key.Character.ToString());
                            handled = true;
                            break;
                        case Keys.Enter:
                            CarriageReturn().LineFeed();
                            handled = true;
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
                            handled = true;
                            break;
                        case Keys.Left:
                            Left(1);
                            handled = true;
                            break;
                        case Keys.Right:
                            Right(1);
                            handled = true;
                            break;
                        case Keys.Down:
                            Down(1);
                            handled = true;
                            break;
                        case Keys.None:
                            break;
                        case Keys.Back:
                            Left(1).Print(" ").Left(1);
                            handled = true;
                            break;
                        default:
                            Print(key.Character.ToString());
                            handled = true;
                            break;
                    }
                }
                else
                {
                    Print(key.Character.ToString());
                    handled = true;
                }
            }

            if (handled)
                RestartCursorEffect();
        }

        /// <summary>
        /// Changes the target of the cursor. Careful, may desync the host if this component is added to one.
        /// </summary>
        /// <param name="surface">The target surface</param>
        public void ChangeTarget(IScreenSurface surface) =>
            _editor = surface.Surface;

        void IComponent.OnAdded(IScreenObject host)
        {
            if (host is IScreenSurface surface)
                _editor = surface.Surface;
            else
                throw new ArgumentException($"This component can only be added to a type that implements {nameof(IScreenSurface)}.");

            _cursorRenderStep?.Dispose();
            _cursorRenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Cursor);

            var existingSteps = surface.Renderer.GetRenderSteps();
            Type renderType = _cursorRenderStep.GetType();
            bool containsRenderStep = false;
            foreach (var item in existingSteps)
            {
                if (item.GetType() == renderType)
                    containsRenderStep = true;
            }

            if (!containsRenderStep)
                surface.Renderer.AddRenderStep(_cursorRenderStep);
            else
                _cursorRenderStep = null;
        }

        void IComponent.OnRemoved(IScreenObject host)
        {
            _editor = null;

            if (_cursorRenderStep != null)
            {
                ((IScreenSurface)host).Renderer.RemoveRenderStep(_cursorRenderStep);
                _cursorRenderStep = null;
            }
        }
    }
}
