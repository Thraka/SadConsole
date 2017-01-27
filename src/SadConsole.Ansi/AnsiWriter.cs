using Microsoft.Xna.Framework;

using SadConsole.Surfaces;
using SadConsole.Effects;
using System;
using System.Linq;
using System.Text;
using Console = SadConsole.Console;

namespace SadConsole.Ansi
{

    public class AnsiWriter
    {
        private string ValidAnsiCodes = "HhfFAaBbCcDdJjKkMmSsUu";
        
        private bool _inEscapeCode;
        private StringBuilder _ansiCodeBuilder;
        private StringBuilder _ansiStringBuilder;
        private Cursor _cursor;
        private Document _ansiDoc;
        private double _totalTime;
        private double _timePerCharacter;
        private int _charsPerSecond;
        private int _readerIndex;
        private byte[] _bytes;
        private SurfaceEditor _editor;
        private State _ansiState;
        private Point _storedCursorLocation;

        public ICellEffect BlinkEffect { get; set; }

        public Cursor Cursor { get { return _cursor; } }

        public int CharactersPerSecond
        {
            get { return _charsPerSecond; }
            set
            {
                if (value != 0)
                    _timePerCharacter = 1d / value;

                _charsPerSecond = value;
            }
        }

        public AnsiWriter(Document ansiDocument, SurfaceEditor editor)
        {
            _ansiDoc = ansiDocument;
            _editor = editor;
            _cursor = new Cursor(editor);
            _cursor.UseStringParser = false;
            _cursor.DisableWordBreak = true;
                        
            CharactersPerSecond = 800;

            _bytes = ansiDocument.AnsiBytes;
            _ansiState = new State();

            _ansiCodeBuilder = new StringBuilder(5);
            _ansiStringBuilder = new StringBuilder(40);

            BlinkEffect = new Blink() { BlinkSpeed = 0.35f };
        }

        public void Process(double timeElapsed)
        {
            if (_readerIndex != _bytes.Length - 1)
            {
                if (_charsPerSecond == 0)
                {
                    ReadEntireDocument();
                    _readerIndex = _bytes.Length - 1;
                }
                else
                {
                    _totalTime += timeElapsed;

                    // Process a character
                    if (_totalTime >= _timePerCharacter)
                    {
                        int charCount = (int)(_totalTime / _timePerCharacter);
                        _totalTime -= _timePerCharacter * charCount;

                        if (_readerIndex + charCount > _bytes.Length - 1)
                            charCount = _bytes.Length - _readerIndex - 1;

                        for (int i = 0; i < charCount; i++)
                        {
                            char character = (char)_bytes[_readerIndex];
                            _readerIndex++;

                            if (_inEscapeCode)
                            {
                                if (ValidAnsiCodes.Contains(character))
                                {
                                    _ansiCodeBuilder.Append(character);
                                    AnsiInterpret(_ansiCodeBuilder.ToString());
                                    _inEscapeCode = false;
                                }
                                else
                                    _ansiCodeBuilder.Append(character);
                            }

                            else if (character == (char)26)
                            { }

                            else if (character == (char)27)
                            {
                                _ansiCodeBuilder.Clear();
                                _ansiCodeBuilder.Append(character);
                                _inEscapeCode = true;
                            }
                            else
                            {
                                _cursor.PrintAppearance = new Cell(_ansiState.Foreground, _ansiState.Background);
                                _cursor.Print(character.ToString());
                            }
                        }
                    }
                    
                    //if (_totalTime >= 1d )
                    //{
                    //    _totalTime = 0d;
                    //}
                    else
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Interprets an individual ansi code.
        /// </summary>
        /// <param name="code">The ANSI.SYS code to read.</param>
        public void AnsiInterpret(string code)
        {
            if (code[0] == (char)27 && code[1] == '[')
            {
                string data = code.Substring(2, code.Length - 3);
                string[] values = data.Split(';');

                switch (code[code.Length - 1])
                {
                    case 'H':
                    case 'h':
                        if (values.Length == 2)
                        {
                            if (values[1] == "")
                                _cursor.Position = new Point(0, _cursor.Position.Y);
                            else
                                _cursor.Position = new Point(Convert.ToInt32(values[1]) - 1, _cursor.Position.Y);

                            if (values[0] == "")
                                _cursor.Position = new Point(_cursor.Position.X, 0);
                            else
                                _cursor.Position = new Point(_cursor.Position.X, Convert.ToInt32(values[0]) - 1);
                        }
                        //else
                        //    System.Diagnostics.Debugger.Break();
                        break;
                    case 'F':
                    case 'f':
                        break;
                    case 'A':
                    case 'a':
                        if (data.Length == 0)
                            _cursor.Up(1);
                        else
                            _cursor.Up(Convert.ToInt32(data));
                        break;
                    case 'B':
                    case 'b':
                        if (data.Length == 0)
                            _cursor.Down(1);
                        else
                            _cursor.Down(Convert.ToInt32(data));
                        break;
                    case 'C':
                    case 'c':
                        if (data.Length == 0)
                            _cursor.Right(1);
                        else
                            _cursor.Right(Convert.ToInt32(data));
                        break;
                    case 'D':
                    case 'd':
                        if (data.Length == 0)
                            _cursor.Left(1);
                        else
                            _cursor.Left(Convert.ToInt32(data));
                        break;
                    case 'J':
                    case 'j':
                        if (data == "" || data == "0")
                            for (int i = _cursor.Position.X; i < _editor.TextSurface.Width; i++)
                                _editor.Clear(i, _cursor.Position.Y);

                        else if (data == "1")
                            for (int i = _cursor.Position.X; i >= 0; i--)
                                _editor.Clear(i, _cursor.Position.Y);

                        else if (data == "2")
                        {
                            _editor.Clear();
                            _cursor.Position = new Point(0, 0);
                        }
                        break;
                    case 'K':
                    case 'k':
                        if (data == "" || data == "0")
                            for (int i = _cursor.Position.X; i < _editor.TextSurface.Width; i++)
                                _editor.Clear(i, _cursor.Position.Y);

                        else if (data == "1")
                            for (int i = _cursor.Position.X; i >= 0; i--)
                                _editor.Clear(i, _cursor.Position.Y);

                        else if (data == "2")
                        {
                            for (int i = 0; i < _editor.TextSurface.Width; i++)
                                _editor.Clear(i, _cursor.Position.Y);
                        }
                        break;

                    case 'S':
                    case 's':
                        _storedCursorLocation = _cursor.Position;
                        break;
                    case 'U':
                    case 'u':
                        _cursor.Position = _storedCursorLocation;
                        break;
                    case 'M':
                    case 'm':

                        if (data == "")
                            _ansiState.AnsiResetVideo();

                        else
                        {
                            for (int i = 0; i < values.Length; i++)
                            {
                                int value = Convert.ToInt32(values[i]);
                                switch (value)
                                {
                                    case 0:
                                        _ansiState.AnsiResetVideo();
                                        break;
                                    case 1:
                                        _ansiState.Bold = true;
                                        _ansiState.AnsiCorrectPrintColor();
                                        break;
                                    case 5:
                                        //Appearance.Effect = BlinkEffect;
                                        break;
                                    case 7:
                                        Color tempFore = _ansiState.Foreground;
                                        _ansiState.Foreground = Helpers.AnsiAdjustColor(_ansiState.Background, _ansiState.Bold);
                                        _ansiState.Background = Helpers.AnsiJustNormalColor(tempFore);
                                        break;
                                    case 30:
                                    case 31:
                                    case 32:
                                    case 33:
                                    case 34:
                                    case 35:
                                    case 36:
                                    case 37:
                                        Helpers.AnsiConfigurePrintColor(false, value - 30, _ansiState);
                                        break;
                                    case 40:
                                    case 41:
                                    case 42:
                                    case 43:
                                    case 44:
                                    case 45:
                                    case 46:
                                    case 47:
                                        Helpers.AnsiConfigurePrintColor(true, value - 40, _ansiState);
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        System.Diagnostics.Debugger.Break();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a line of ANSI.SYS code.
        /// </summary>
        /// <param name="line">The line to read.</param>
        /// <returns>Returns false when character 26 is encountered; otherwise true.</returns>
        public bool AnsiReadLine(string line, bool moreLines = false)
        {
            bool inEscape = false;
            StringBuilder stringValue = new StringBuilder();
            StringBuilder stringEscape = new StringBuilder(5);

            if (line == "")
            {
                _cursor.CarriageReturn();
                _cursor.LineFeed();
                return true;
            }

            bool onLastLine = _cursor.Position.Y == _editor.TextSurface.Height - 1;

            foreach (var item in line)
            {
                if (inEscape)
                {
                    if (ValidAnsiCodes.Contains(item))
                    {
                        stringEscape.Append(item);
                        AnsiInterpret(stringEscape.ToString());
                        inEscape = false;
                    }
                    else
                        stringEscape.Append(item);
                }

                else if (item == (char)26)
                    return false;


                else if (item == (char)27)
                {
                    if (stringValue.Length != 0)
                    {
                        _cursor.PrintAppearance = new Cell(_ansiState.Foreground, _ansiState.Background);
                        _cursor.Print(stringValue.ToString());
                        stringValue.Clear();
                    }

                    stringEscape.Clear();
                    stringEscape.Append(item);
                    inEscape = true;

                }
                else
                {
                    stringValue.Append(item);
                }
            }

            if (stringValue.Length != 0)
            {
                _cursor.PrintAppearance = new Cell(_ansiState.Foreground, _ansiState.Background);
                _cursor.Print(stringValue.ToString());
            }

            _cursor.CarriageReturn();

            if ((onLastLine && moreLines) || !onLastLine)
                _cursor.LineFeed();

            return true;
        }

        /// <summary>
        /// Loads an ansi file and parses it.
        /// </summary>
        /// <param name="path">The paath to the ansi file.</param>
        public void ReadEntireDocument()
        {
            string[] lines = _ansiDoc.AnsiString.Split('\n');
            _ansiState.AnsiResetVideo();
            int counter = 0;

            foreach (var line in lines)
            {
                counter++;
                bool onLastLine = _cursor.Position.Y == _editor.TextSurface.Height - 1;

                if (AnsiReadLine(line, counter != lines.Length) == false)
                    return;
            }
        }

        public void Restart()
        {
            _readerIndex = 0;
        }
    }
}
