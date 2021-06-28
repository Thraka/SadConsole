using System;
using Console = SadConsole.Console;
using SadRogue.Primitives;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Draws a string to a console as if someone was typing.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Instruction: Draw string")]
    public class DrawString : InstructionBase
    {
        private ColoredString _text;
        private Components.Cursor _privateCursor;

        /// <summary>
        /// Gets or sets the text to print.
        /// </summary>
        public ColoredString Text
        {
            get => _text;
            set => _text = value ?? throw new Exception($"{nameof(Text)} can't be null.");
        }

        /// <summary>
        /// Gets or sets the total time to take to write the string. Use zero for instant.
        /// </summary>
        public float TotalTimeToPrint { get; set; }

        /// <summary>
        /// Gets or sets the position on the console to write the text.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Represents the cursor used in printing. Use this for styling and printing behavior.
        /// </summary>
        public Components.Cursor Cursor { get; set; }

        private double _timeElapsed = 0d;
        private double _timePerCharacter = 0d;
        private string _textCopy;
        private short _textIndex;
        private bool _started = false;
        private Point _tempLocation;

        /// <summary>
        /// Creates a new instance of the object with the specified text.
        /// </summary>
        /// <param name="text"></param>
        public DrawString(ColoredString text) =>
            Text = text;

        /// <summary>
        /// Creates a new instance of the object. <see cref="Text"/> must be set manually.
        /// </summary>
        public DrawString(): this(new ColoredString()) { }

        /// <inheritdoc />
        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            var surface = componentHost as IScreenSurface;

            if (Cursor == null && surface == null)
                throw new ArgumentException($"For {nameof(DrawString)} either the {nameof(Cursor)} must be set or run with a {nameof(IScreenSurface)}.");

            else if (surface != null && _privateCursor == null)
                SetPrivateCursor(surface);

            var cursor = Cursor ?? _privateCursor;

            if (!_started)
            {
                _started = true;
                _textCopy = Text.ToString();
                _textIndex = 0;

                cursor.DisableWordBreak = true;

                if (_textCopy.Length == 0)
                {
                    IsFinished = true;
                    base.Update(componentHost, delta);
                    return;
                }

                _tempLocation = Position;

                if (TotalTimeToPrint > 0f)
                {
                    _timePerCharacter = TotalTimeToPrint / _textCopy.Length;
                }
            }


            if (TotalTimeToPrint == 0f)
            {
                cursor.Position = Position;
                cursor.Print(Text);
                IsFinished = true;
            }
            else
            {
                _timeElapsed += delta.TotalSeconds;
                if (_timeElapsed >= _timePerCharacter)
                {
                    int charCount = (int)(_timeElapsed / _timePerCharacter);
                    int charsLeft = _textCopy.Length - _textIndex;
                    _timeElapsed = 0d;

                    cursor.Position = _tempLocation;

                    if (charCount >= charsLeft)
                    {
                        charCount = charsLeft;
                        IsFinished = true;
                    }
                    else
                    {
                        // TODO: Change drawstring to look for \r or \n and when found, loop and consume all of them until something else is found.

                        // If we happen to end near a \r\n sequence, consume both.
                        if (Text[_textIndex + charCount].GlyphCharacter == '\r' && charsLeft > 2 && Text[_textIndex + charCount + 1].GlyphCharacter == '\n')
                        {
                            charCount += 2;

                            if (charCount >= charsLeft)
                                IsFinished = true;
                        }
                        else if (Text[_textIndex + charCount - 1].GlyphCharacter == '\r' && Text[_textIndex + charCount].GlyphCharacter == '\n')
                        {
                            charCount++;

                            if (charCount >= charsLeft)
                                IsFinished = true;
                        }
                    }

                    ColoredString textToPrint = Text.SubString(_textIndex, charCount);
                    cursor.Print(textToPrint);
                    _textIndex += (short)charCount;

                    _tempLocation = cursor.Position;
                }
            }

            base.Update(componentHost, delta);
        }

        /// <inheritdoc />
        public override void Repeat()
        {
            _started = false;
            _textIndex = 0;

            base.Repeat();
        }

        /// <summary>
        /// Creates an invisible cursor that prints on the target surface.
        /// </summary>
        /// <param name="host">The host this instruction is added to.</param>
        public override void OnAdded(IScreenObject host)
        {
            if (host is IScreenSurface surface)
            {
                if (Cursor == null)
                    SetPrivateCursor(surface);

                return;
            }

            throw new ArgumentException($"This component can only bedded to a type that implements {nameof(IScreenSurface)}.");
        }

        private void SetPrivateCursor(IScreenSurface surface) =>
            _privateCursor = new Components.Cursor(surface.Surface) { IsVisible = false };
    }
}
