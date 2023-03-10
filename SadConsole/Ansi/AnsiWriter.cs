﻿using System;
using System.Text;
using SadConsole.Effects;
using SadRogue.Primitives;

namespace SadConsole.Ansi;

/// <summary>
/// Writes a <see cref="Document"/> to a <see cref="ICellSurface"/>.
/// </summary>
public class AnsiWriter
{
    private const string ValidAnsiCodes = "HhfFAaBbCcDdJjKkMmSsUu";

    private bool _inEscapeCode;
    private readonly StringBuilder _ansiCodeBuilder;
    private double _totalTime;
    private double _timePerCharacter;
    private int _charsPerSecond;
    private int _readerIndex;
    private readonly byte[] _bytes;
    private readonly ICellSurface _editor;
    private readonly State _ansiState;
    private Point _storedCursorLocation;

    /// <summary>
    /// The document used to create this writer.
    /// </summary>
    public Document AnsiDocument { get; }

    /// <summary>
    /// The blink effect to apply when the ansi document provides the ANSI blink command.
    /// </summary>
    public ICellEffect BlinkEffect { get; set; }

    /// <summary>
    /// The cursor used to write to the target <see cref="ICellSurface"/>.
    /// </summary>
    public Components.Cursor Cursor { get; }

    /// <summary>
    /// How many characters to process a second. When set to 0 reads the entire document at once.
    /// </summary>
    public int CharactersPerSecond
    {
        get => _charsPerSecond;
        set
        {
            if (value != 0)
            {
                _timePerCharacter = 1d / value;
            }

            _charsPerSecond = value;
        }
    }

    /// <summary>
    /// Creates a new instance with the source document and target surface.
    /// </summary>
    /// <param name="ansiDocument">The source document containing ANSI commands.</param>
    /// <param name="editor">The surface to parse the ANSI to.</param>
    public AnsiWriter(Document ansiDocument, ICellSurface editor)
    {
        AnsiDocument = ansiDocument;
        _editor = editor;
        Cursor = new Components.Cursor(editor)
        {
            UseStringParser = false,
            DisableWordBreak = true,
            PrintOnlyCharacterData = false,
            PrintAppearanceMatchesHost = false,
            UseLinuxLineEndings = true,
            DisablePrintAutomaticLineFeed = true
        };

        CharactersPerSecond = 800;

        _bytes = ansiDocument.AnsiBytes;
        _ansiState = new State();

        _ansiCodeBuilder = new StringBuilder(5);

        BlinkEffect = new Blink() { BlinkSpeed = System.TimeSpan.FromSeconds(0.35d) };
    }

    /// <summary>
    /// Processes the document by the amount of time that has elapsed. If <see cref="CharactersPerSecond"/> is 0, time elapsed has no affect.
    /// </summary>
    /// <param name="timeElapsed">The time in seconds.</param>
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
                    {
                        charCount = _bytes.Length - _readerIndex - 1;
                    }

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
                            {
                                _ansiCodeBuilder.Append(character);
                            }
                        }

                        // CTRL-Z
                        else if (character == (char)26)
                        {
                            _readerIndex = _bytes.Length - 1;
                            return;
                        }

                        else if (character == (char)27)
                        {
                            _ansiCodeBuilder.Clear();
                            _ansiCodeBuilder.Append(character);
                            _inEscapeCode = true;
                        }
                        else
                        {
                            Cursor.PrintAppearance = new ColoredGlyph(_ansiState.Foreground, _ansiState.Background);
                            Cursor.Print(character.ToString());
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
                        Cursor.Position = new Point(
                            values[1] == ""
                                ? 0
                                : Convert.ToInt32(values[1]) - 1,

                            values[0] == ""
                                ? 0
                                : Convert.ToInt32(values[0]) - 1
                        );
                    }

                    //else
                    //    System.Diagnostics.Debugger.Break();
                    break;
                case 'F':
                case 'f':
                    break;
                case 'A':
                case 'a':
                    Cursor.Up(data.Length == 0
                        ? 1
                        : Convert.ToInt32(data)
                    );
                    break;
                case 'B':
                case 'b':
                    Cursor.Down(data.Length == 0
                        ? 1
                        : Convert.ToInt32(data)
                    );
                    break;
                case 'C':
                case 'c':
                    Cursor.Right(data.Length == 0
                        ? 1
                        : Convert.ToInt32(data)
                    );
                    break;
                case 'D':
                case 'd':
                    Cursor.Left(data.Length == 0
                        ? 1
                        : Convert.ToInt32(data)
                    );
                    break;
                case 'J':
                case 'j':
                    if (data == "" || data == "0")
                    {
                        for (int i = Cursor.Position.X; i < _editor.Width; i++)
                        {
                            _editor.Clear(i, Cursor.Position.Y);
                        }
                    }
                    else if (data == "1")
                    {
                        for (int i = Cursor.Position.X; i >= 0; i--)
                        {
                            _editor.Clear(i, Cursor.Position.Y);
                        }
                    }
                    else if (data == "2")
                    {
                        _editor.Clear();
                        Cursor.Position = Point.Zero;
                    }

                    break;
                case 'K':
                case 'k':
                    if (data == "" || data == "0")
                    {
                        for (int i = Cursor.Position.X; i < _editor.Width; i++)
                        {
                            _editor.Clear(i, Cursor.Position.Y);
                        }
                    }
                    else if (data == "1")
                    {
                        for (int i = Cursor.Position.X; i >= 0; i--)
                        {
                            _editor.Clear(i, Cursor.Position.Y);
                        }
                    }
                    else if (data == "2")
                    {
                        for (int i = 0; i < _editor.Width; i++)
                        {
                            _editor.Clear(i, Cursor.Position.Y);
                        }
                    }

                    break;

                case 'S':
                case 's':
                    _storedCursorLocation = Cursor.Position;
                    break;
                case 'U':
                case 'u':
                    Cursor.Position = _storedCursorLocation;
                    break;
                case 'M':
                case 'm':

                    if (data == "")
                    {
                        _ansiState.AnsiResetVideo();
                    }
                    else
                    {
                        int count = values.Length;
                        for (int i = 0; i < count; i++)
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
    /// <param name="moreLines">When <see langword="true"/>, calls <see cref="Components.Cursor.LineFeed"/>; otherwise does nothing.</param>
    /// <returns>Returns false when character 26 is encountered; otherwise true.</returns>
    public bool AnsiReadLine(string line, bool moreLines = false)
    {
        bool inEscape = false;
        var stringValue = new StringBuilder();
        var stringEscape = new StringBuilder(5);

        if (line == "")
        {
            Cursor.CarriageReturn();
            Cursor.LineFeed();
            return true;
        }

        bool onLastLine = Cursor.Position.Y == _editor.Height - 1;

        int count = line.Length;
        for (int i = 0; i < count; i++)
        {
            char item = line[i];
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
            {
                return false;
            }

            else if (item == (char)27)
            {
                if (stringValue.Length != 0)
                {
                    Cursor.PrintAppearance = new ColoredGlyph(_ansiState.Foreground, _ansiState.Background);
                    Cursor.Print(stringValue.ToString());
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
            Cursor.PrintAppearance = new ColoredGlyph(_ansiState.Foreground, _ansiState.Background);
            Cursor.Print(stringValue.ToString());
        }

        Cursor.CarriageReturn();

        if ((onLastLine && moreLines) || !onLastLine)
            Cursor.LineFeed();

        return true;
    }

    /// <summary>
    /// Loads an ansi file and parses it.
    /// </summary>
    public void ReadEntireDocument()
    {
        _ansiState.AnsiResetVideo();

        for (int i = 0; i < _bytes.Length; i++)
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
                {
                    _ansiCodeBuilder.Append(character);
                }
            }

            // CTRL-Z
            else if (character == (char)26)
            {
                break;
            }

            else if (character == (char)27)
            {
                _ansiCodeBuilder.Clear();
                _ansiCodeBuilder.Append(character);
                _inEscapeCode = true;
            }
            else if (_readerIndex - 1 < _bytes.Length || (_readerIndex - 1 == _bytes.Length && character != '\n'))
            {
                Cursor.PrintAppearance = new ColoredGlyph(_ansiState.Foreground, _ansiState.Background);
                Cursor.Print(character.ToString());
            }
        }
    }

    /// <summary>
    /// Moves the reader back to the start of the file so that the source can .
    /// </summary>
    public void Restart() => _readerIndex = 0;
}
