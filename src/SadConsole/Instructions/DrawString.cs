#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.Instructions
{
    using System;
    using Console = SadConsole.Console;

    /// <summary>
    /// Draws a string to a console as if someone was typing.
    /// </summary>
    public class DrawString : InstructionBase
    {
        private ColoredString _text;

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
        public Cursor Cursor { get; set; }

        private CellSurface _target;
        private double _timeElapsed = 0d;
        private double _timePerCharacter = 0d;
        private string _textCopy;
        private short _textIndex;
        private bool _started = false;
        private Point _tempLocation;

        /// <summary>
        /// Draws a string on the specified surface.
        /// </summary>
        /// <param name="target">The target surface to use.</param>
        /// <param name="text">The text to print.</param>
        public DrawString(CellSurface target, ColoredString text)
        {
            _target = target;
            Cursor = new Cursor();
            Text = text;
        }

        /// <summary>
        /// Draws a string on the surface passed to <see cref="Update(Console, TimeSpan)"/>.
        /// </summary>
        /// <param name="text"></param>
        public DrawString(ColoredString text)
        {
            Cursor = new Cursor();
            Text = text;
        }

        /// <summary>
        /// Draws a string on the surface passed to <see cref="Update(Console, TimeSpan)"/>. <see cref="Text"/> must be set manually.
        /// </summary>
        public DrawString()
        {
            Cursor = new Cursor();
            Text = new ColoredString();
        }

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            if (!_started)
            {
                _started = true;
                _textCopy = Text.ToString();
                _textIndex = 0;

                if (_target == null)
                {
                    _target = console;
                }

                Cursor.AttachSurface(_target);
                Cursor.DisableWordBreak = true;

                if (_textCopy.Length == 0)
                {
                    IsFinished = true;
                    base.Update(console, delta);
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
                Cursor.Position = Position;
                Cursor.Print(Text);
                IsFinished = true;
            }
            else
            {
                _timeElapsed += Global.GameTimeElapsedUpdate;
                if (_timeElapsed >= _timePerCharacter)
                {
                    int charCount = (int)(_timeElapsed / _timePerCharacter);
                    _timeElapsed = 0d;

                    Cursor.Position = _tempLocation;

                    if (charCount >= _textCopy.Length - _textIndex)
                    {
                        charCount = _textCopy.Length - _textIndex;
                        IsFinished = true;
                    }

                    ColoredString textToPrint = Text.SubString(_textIndex, charCount);
                    Cursor.Print(textToPrint);
                    _textIndex += (short)charCount;

                    _tempLocation = Cursor.Position;
                }
            }

            base.Update(console, delta);
        }

        /// <inheritdoc />
        public override void Repeat()
        {
            _started = false;
            _textIndex = 0;

            base.Repeat();
        }
    }
}
