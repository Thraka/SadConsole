namespace SadConsole.Instructions
{
    using Microsoft.Xna.Framework;
    using SadConsole.Effects;
    using System.Runtime.Serialization;

    /// <summary>
    /// Draws a string to a console as if someone was typing.
    /// </summary>
    [DataContract]
    public class DrawString : InstructionBase<Consoles.IConsole>
    {
        #region Settings
        /// <summary>
        /// Gets or sets the text to print.
        /// </summary>
        [DataMember]
        public ColoredString Text { get; set; }

        /// <summary>
        /// Gets or sets the total time to take to write the string. Use zero for instant.
        /// </summary>
        [DataMember]
        public float TotalTimeToPrint { get; set; }

        /// <summary>
        /// Gets or sets the position on the console to write the text.
        /// </summary>
        [DataMember]
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets whether or not to use the virtual cursor to write the text.
        /// </summary>
        [DataMember]
        public bool UseConsolesCursorToPrint { get; set; }
        #endregion

        private double _timeElapsed = 0d;
        private double _timePerCharacter = 0d;
        private string _textCopy;
        private short _textIndex;
        private bool _started = false;
        private Point _tempLocation;

        public DrawString(Consoles.IConsole target)
            : base(target)
        {
        }

        public override void Run()
        {
            if (!_started)
            {
                _started = true;
                _textCopy = Text.ToString();
                _textIndex = 0;

                if (_textCopy.Length == 0)
                {
                    IsFinished = true;
                    base.Run();
                    return;
                }

                _tempLocation = Position;

                if (TotalTimeToPrint > 0f)
                    _timePerCharacter = TotalTimeToPrint / _textCopy.Length;
            }


            if (TotalTimeToPrint == 0f)
            {
                Target.Data.Print(Position.X, Position.Y, Text);
                IsFinished = true;
            }
            else
            {
                _timeElapsed += SadConsole.Engine.GameTimeElapsedUpdate;
                if (_timeElapsed >= _timePerCharacter)
                {
                    int charCount = (int)(_timeElapsed / _timePerCharacter);
                    _timeElapsed = 0d;

                    SadConsole.Consoles.Cursor cur;

                    if (UseConsolesCursorToPrint)
                        cur = Target.VirtualCursor;
                    else
                        cur = new Consoles.Cursor(Target);

                    cur.Position = _tempLocation;

                    if (charCount >= _textCopy.Length - _textIndex)
                    {
                        charCount = _textCopy.Length - _textIndex;
                        IsFinished = true;
                    }

                    var textToPrint = Text.SubString(_textIndex, charCount);
                    cur.Print(textToPrint);
                    _textIndex += (short)charCount;

                    _tempLocation = cur.Position;
                }
            }

            base.Run();
        }

        public override void Repeat()
        {
            _started = false;
            _textIndex = 0;

            base.Repeat();
        }
    }
}