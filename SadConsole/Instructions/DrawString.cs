using System;
using Coroutine;
using SadRogue.Primitives;

namespace SadConsole.Instructions;

/// <summary>
/// Draws a string to a console as if someone was typing.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Instruction: Draw string")]
public class DrawString : InstructionBase
{
    private ColoredString _text;
    private Components.Cursor? _privateCursor;
    CoroutineHandlerInstance? _coroutineHandler;
    ActiveCoroutine? _runningCoroutine;

    /// <summary>
    /// Gets or sets the text to print.
    /// </summary>
    public ColoredString Text
    {
        get => _text;
        set => _text = value ?? throw new Exception($"{nameof(Text)} can't be null.");
    }

    /// <summary>
    /// Gets or sets the total time to take to write the string. Use <see cref="TimeSpan.Zero"/> for no duration.
    /// </summary>
    public TimeSpan TotalTimeToPrint { get; set; }

    /// <summary>
    /// Gets or sets the position on the console to write the text.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Represents the cursor used in printing. Use this for styling and printing behavior.
    /// </summary>
    public Components.Cursor? Cursor { get; set; }

    private TimeSpan _timeElapsed;
    private TimeSpan _timePerCharacter;
    private string _textCopy = string.Empty;
    private short _textIndex;
    private bool _started = false;
    private Point _tempLocation;

    /// <summary>
    /// Creates a new instance of the object with the specified text.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="duration">The time to print the entire text.</param>
    public DrawString(ColoredString text, TimeSpan duration)
    {
        _text = text;
        TotalTimeToPrint = duration;
    }

    /// <summary>
    /// Creates a new instance of the object with the specified text. Prints the text in one second.
    /// </summary>
    /// <param name="text"></param>
    public DrawString(ColoredString text): this(text, TimeSpan.FromSeconds(1)) { }
        

    /// <summary>
    /// Creates a new instance of the object. <see cref="Text"/> must be set manually.
    /// </summary>
    public DrawString() : this(new ColoredString(), TimeSpan.FromSeconds(1)) { }

    /// <inheritdoc />
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        var surface = componentHost as IScreenSurface;

        if (Cursor == null && surface == null)
            throw new ArgumentException($"For {nameof(DrawString)} either the {nameof(Cursor)} must be set or run with a {nameof(IScreenSurface)}.");

        else if (surface != null && _privateCursor == null && Cursor == null)
            SetPrivateCursor(surface);

        var cursor = Cursor ?? _privateCursor ?? throw new Exception("Cursor is null");

        if (!_started)
        {
            _started = true;
            _textCopy = Text.ToString();
            _textIndex = 0;

            if (_textCopy.Length == 0)
            {
                IsFinished = true;
                base.Update(componentHost, delta);
                return;
            }

            cursor.Position = Position;

            if (TotalTimeToPrint > TimeSpan.Zero)
            {
                _timePerCharacter = TotalTimeToPrint / _textCopy.Length;
            }
        }


        if (TotalTimeToPrint == TimeSpan.Zero)
        {
            cursor.Print(Text);
            IsFinished = true;
        }
        else
        {
            _timeElapsed += delta;
            if (_timeElapsed >= _timePerCharacter)
            {
                int charCount = (int)(_timeElapsed / _timePerCharacter);
                int charsLeft = _textCopy.Length - _textIndex;
                _timeElapsed = TimeSpan.FromTicks(_timeElapsed.Ticks - _timePerCharacter.Ticks * charCount);

                if (charCount >= charsLeft)
                {
                    charCount = charsLeft;
                    IsFinished = true;
                }

                for (int i = 0; i < charCount; i++)
                {
                    if (_coroutineHandler == null)
                    {
                        _coroutineHandler = new CoroutineHandlerInstance();
                        _runningCoroutine = _coroutineHandler.Start(cursor.PrintCoroutine(Text));
                    }
                    else
                    {
                        _coroutineHandler.Tick(delta);
                    }
                }

                _textIndex += (short)charCount;

                if (IsFinished)
                {
                    _coroutineHandler = null;
                    _runningCoroutine = null;
                }
            }
        }

        base.Update(componentHost, delta);
    }

    /// <inheritdoc />
    public override void Reset()
    {
        _started = false;
        _textIndex = 0;
        _coroutineHandler = null;
        _runningCoroutine = null;

        base.Reset();
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
        _privateCursor = new Components.Cursor(surface.Surface) { IsVisible = false, PrintAppearanceMatchesHost = false };
}
