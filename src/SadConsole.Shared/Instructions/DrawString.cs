#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.Instructions
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Draws a string to a console as if someone was typing.
    /// </summary>
    [DataContract]
    public class DrawString : InstructionBase<CellSurface>
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
        /// Represents the cursor used in printing. Use this for styling and printing behavior.
        /// </summary>
        [DataMember]
        public Cursor Cursor { get => _cursor; set => _cursor = value; }

        #endregion

        private double _timeElapsed = 0d;
        private double _timePerCharacter = 0d;
        private string _textCopy;
        private short _textIndex;
        private bool _started = false;
        private Point _tempLocation;
        private Cursor _cursor;
        

        public DrawString(CellSurface target)
            : base(target)
        {
            _cursor = new Cursor(target);
            _cursor.DisableWordBreak = true;
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

                    _cursor.Position = _tempLocation;

                    if (charCount >= _textCopy.Length - _textIndex)
                    {
                        charCount = _textCopy.Length - _textIndex;
                        IsFinished = true;
                    }

                    var textToPrint = Text.SubString(_textIndex, charCount);
                    _cursor.Print(textToPrint);
                    _textIndex += (short)charCount;

                    _tempLocation = _cursor.Position;
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