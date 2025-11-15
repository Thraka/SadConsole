using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// A cursor that is attached to a <see cref="Console"/> used for printing.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Cursor")]
[DataContract]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class Cursor : IComponent, IDisposable
{
    private ColoredGlyphBase _cursorCellOriginal;
    private ICellEffect? _cursorCellEffect;
    private ColoredGlyphBase _cursorCell;

    private ICellSurface? _editor;
    private Point _position = new();
    [DataMember]
    private bool _applyCursorEffect = true;
    private Renderers.IRenderStep? _cursorRenderStep;
    private bool _disposedValue;

    /// <summary>
    /// The default glyph used for a new cursor. Value 219.
    /// </summary>
    public static readonly int DefaultCursorGlyph = 219;

    /// <summary>
    /// Raised when the keyboard is processing keys. Each key is sent to this event and can be cancelled
    /// </summary>
    public event EventHandler<KeyboardHandledKeyEventArgs>? KeyboardPreview;

    /// <summary>
    /// Cell used to render the cursor on the screen.
    /// </summary>
    [DataMember(Order = 0)]
    public ColoredGlyphBase CursorRenderCell
    {
        get => _cursorCellOriginal;
        set
        {
            _cursorCellOriginal = value;
            _cursorCell = value.Clone();

            _cursorCellEffect?.Restart();
        }
    }

    /// <summary>
    /// Used in rendering. The cell after the effect has been applied.
    /// </summary>
    public ColoredGlyphBase CursorRenderCellActiveState => _cursorCell;

    /// <summary>
    /// Appearance used when printing text. <see cref="PrintOnlyCharacterData"/> must be set to <see langword="false"/> for this to apply.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase PrintAppearance { get; set; }

    /// <summary>
    /// When <see langword="true"/>, printing will use the host's <see cref="ICellSurface.DefaultForeground"/> and <see cref="ICellSurface.DefaultBackground"/> color properties.
    /// </summary>
    [DataMember]
    public bool PrintAppearanceMatchesHost { get; set; } = true;

    /// <summary>
    /// This effect is applied to each cell printed by the cursor.
    /// </summary>
    [DataMember]
    public ICellEffect? PrintEffect { get; set; }

    /// <summary>
    /// This is the cursor visible effect, like blinking.
    /// </summary>
    [DataMember]
    public ICellEffect? CursorRenderEffect
    {
        get => _cursorCellEffect;
        set
        {
            _cursorCellEffect = value;

            if (_cursorCell != null)
                _cursorCellOriginal?.CopyAppearanceTo(_cursorCell, true);
        }
    }

    /// <summary>
    /// Sets the glyph used in rendering. A shortcut to <see cref="CursorRenderCell"/>.
    /// </summary>
    public int CursorGlyph
    {
        get => _cursorCellOriginal.Glyph;
        set => CursorRenderCell = new ColoredGlyph(_cursorCellOriginal.Foreground, _cursorCellOriginal.Background, value);
    }

    /// <summary>
    /// When <see langword="true"/>, indicates that the cursor, when printing, should not use the <see cref="PrintAppearance"/> property in determining the color/effect of the cell, but keep the cell the same as it was.
    /// </summary>
    [DataMember]
    public bool PrintOnlyCharacterData { get; set; }

    /// <summary>
    /// When <see langword="true"/>, left-clicking on the host surface will reposition the cursor to the clicked position.
    /// </summary>
    [DataMember]
    public bool MouseClickReposition { get; set; }

    /// <summary>
    /// When <see langword="true"/>, returns a handled status from the mouse processor when the <see cref="MouseClickReposition"/> is enabled. This prevents further left-mouse processing on the host.
    /// </summary>
    [DataMember]
    public bool MouseClickRepositionHandlesMouse { get; set; } = true;

    /// <summary>
    /// Shows or hides the cursor. This does not affect how the cursor operates.
    /// </summary>
    [DataMember]
    public bool IsVisible { get; set; }

    /// <summary>
    /// When <see langword="false"/>, prevents the cursor from running on the host.
    /// </summary>
    [DataMember]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// When <see langword="false"/>, prevents the <see cref="CursorRenderEffect"/> from being applied.
    /// </summary>
    public bool ApplyCursorEffect
    {
        get => _applyCursorEffect;
        set
        {
            _applyCursorEffect = value;

            // If this is disabled, restore cell state
            if (!value)
                _cursorCellOriginal.CopyAppearanceTo(_cursorCell);
            else
            {
                _cursorCellEffect?.Restart();
                _cursorCellEffect?.ApplyToCell(_cursorCell, _cursorCellOriginal);
            }
        }
    }

    /// <summary>
    /// When <see langword="true"/>, applies the <see cref="PrintEffect"/> to the cursor when it prints.
    /// </summary>
    [DataMember]
    public bool UsePrintEffect { get; set; } = true;

    /// <summary>
    /// Gets or sets the location of the cursor on the console.
    /// </summary>
    [DataMember]
    public Point Position
    {
        get => _position;
        set
        {
            if (_editor != null)
            {
                _position = new Point(Math.Clamp(value.X, 0, _editor.Width - 1), Math.Clamp(value.Y, 0, _editor.Height - 1));
                RestartCursorEffect();
            }
        }
    }

    /// <summary>
    /// When true, prevents any print method from breaking words up by spaces when wrapping lines.
    /// </summary>
    [DataMember]
    public bool DisableWordBreak { get; set; } = false;

    /// <summary>
    /// When <see langword="true"/>, printing characters to the end of a line won't wrap to the next line. Defaults to <see langword="false"/>, which moves the cursor to the next line if printing goes past the end of the line.
    /// </summary>
    public bool DisablePrintAutomaticLineFeed { get; set; } = false;

    /// <summary>
    /// Enables Linux-like string parsing where a \n behaves like a \r\n.
    /// </summary>
    [DataMember]
    public bool UseLinuxLineEndings { get; set; } = false;

    /// <summary>
    /// Indicates this cursor should process te string through the <see cref="StringParser.IParser.Parse"/> method from <see cref="ColoredString.Parser"/> to create a colored string when using <see cref="Print(string)"/> or <see cref="Print(string, ColoredGlyphBase, ICellEffect)"/>.
    /// </summary>
    [DataMember]
    public bool UseStringParser { get; set; } = false;

    /// <summary>
    /// Gets or sets the row of the cursor position.
    /// </summary>
    public int Row
    {
        get => _position.Y;
        set => Position = new Point(_position.X, value);
    }

    /// <summary>
    /// Gets or sets the column of the cursor position.
    /// </summary>
    public int Column
    {
        get => _position.X;
        set => Position = new Point(value, _position.Y);
    }

    /// <summary>
    /// Indicates that when the cursor goes past the last cell of the console, that the rows should be shifted up when the cursor is automatically reset to the next line.
    /// </summary>
    [DataMember]
    public bool AutomaticallyShiftRowsUp { get; set; }

    /// <summary>
    /// Sets the sort order of this component within the host.
    /// </summary>
    [DataMember]
    public uint SortOrder { get; set; }

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

        // CursorRenderCell property
        _cursorCellOriginal = new ColoredGlyph(Color.White, Color.Transparent, DefaultCursorGlyph);
        _cursorCell = _cursorCellOriginal.Clone();

        ApplyDefaultCursorEffect();
    }

    /// <summary>
    /// Creates a new instance of the cursor that works with the specified surface.
    /// </summary>
    /// <param name="surface"></param>
    public Cursor(ICellSurface surface) : this() =>
        _editor = surface;

    /// <summary>
    /// Resets the <see cref="CursorRenderEffect"/> back to the default.
    /// </summary>
    public Cursor ApplyDefaultCursorEffect()
    {
        if (_cursorCellEffect != null)
            _cursorCellOriginal.CopyAppearanceTo(_cursorCell);

        CursorRenderEffect = new Effects.Blink
        {
            BlinkSpeed = System.TimeSpan.FromSeconds(0.35d)
        };
        CursorRenderEffect.ApplyToCell(_cursorCell, _cursorCellOriginal);

        return this;
    }

    /// <summary>
    /// Clones and reassigns <see cref="CursorRenderEffect"/> to restart it.
    /// </summary>
    /// <returns></returns>
    public Cursor RestartCursorEffect()
    {
        if (_cursorCellEffect == null) return this;

        _cursorCellOriginal.CopyAppearanceTo(_cursorCell);

        _cursorCellEffect.Restart();
        _cursorCellEffect.ApplyToCell(_cursorCell, _cursorCellOriginal);

        return this;
    }

    /// <summary>
    /// Sets the cursor appearance to the console's default foreground and background.
    /// </summary>
    /// <returns>This cursor object.</returns>
    /// <exception cref="Exception">Thrown when the cursor is not attached to any surface.</exception>
    public Cursor SetPrintAppearanceToHost()
    {
        if (_editor == null) throw new Exception("A host is not attached, cannot reset appearance.");

        PrintAppearance = new ColoredGlyph(_editor.DefaultForeground, _editor.DefaultBackground, 0);

        return this;
    }

    /// <summary>
    /// Sets <see cref="PrintAppearance"/>.
    /// </summary>
    /// <param name="appearance">The appearance to set.</param>
    /// <returns>This cursor object.</returns>
    public Cursor SetPrintAppearance(ColoredGlyphBase appearance)
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

    private void PrintGlyph(ColoredGlyphAndEffect glyph, ColoredString settings)
    {
        if (_editor == null) throw new Exception("A host is not attached, cannot print.");

        ColoredGlyphBase cell = _editor[_position.Y * _editor.Width + _position.X];

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

            if (!settings.IgnoreDecorators)
                cell.Decorators = CellDecoratorHelpers.CloneDecorators(glyph);

            if (!settings.IgnoreEffect)
               _editor.SetEffect(_position.Y * _editor.Width + _position.X, glyph.Effect);
        }
        else if (!settings.IgnoreGlyph)
            cell.Glyph = glyph.GlyphCharacter;

        (int x, int y) = _position;
        x += 1;

        if (!DisablePrintAutomaticLineFeed && x >= _editor.Width)
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
    /// <param name="templateEffect">Effect to apply to the text as its printed. Can be <see langword="null"/>.</param>
    /// <returns>Returns this cursor object.</returns>
    public Cursor Print(string text, ColoredGlyphBase template, Effects.ICellEffect? templateEffect)
    {
        if (_editor == null) throw new Exception("A host is not attached, cannot print.");

        ColoredString coloredString;

        if (UseStringParser)
            coloredString = ColoredString.Parser.Parse(text, _position.Y * _editor.Width + _position.X, _editor);

        else
        {
            if (PrintAppearanceMatchesHost)
                coloredString = text.CreateColored(_editor.DefaultForeground, _editor.DefaultBackground, Mirror.None);
            else
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
    /// Prints text to the console using the appearance of the colored string, but with coroutine.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <returns>An enumerator that returns at each character printed.</returns>
    public IEnumerator<Coroutine.Wait> PrintCoroutine(ColoredString text)
    {
        // This is a copy of the code in Print. Updates in one should happen in the other.
        // TODO: Find a better way to do this and share code.
        if (_editor == null) throw new Exception("A host is not attached, cannot print.");

        if (text.Length == 0) yield break;

        _cursorCellEffect?.Restart();
        _cursorCellEffect?.ApplyToCell(_cursorCell, _cursorCellOriginal);

        // If we don't want the pretty print, or we're printing a single character (for example, from keyboard input)
        // Then use the pretty print system.
        if (!DisableWordBreak && text.String.Length != 1)
        {
            // Prep
            ColoredGlyphAndEffect glyph;
            ColoredGlyphAndEffect spaceGlyph = (ColoredGlyphAndEffect)text[0].Clone();

            spaceGlyph.GlyphCharacter = ' ';
            string stringText = text.String.TrimEnd(' ');

            // Pull any starting spaces off
            string newStringText = stringText.TrimStart(' ');
            int spaceCount = stringText.Length - newStringText.Length;

            for (int i = 0; i < spaceCount; i++)
            {
                PrintGlyph(spaceGlyph, text);
                yield return new();
            }

            if (spaceCount != 0)
                text = text.SubString(spaceCount, text.Length - spaceCount);

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

                                    // Update the space glyph with the last character printed.
                                    glyph.CopyAppearanceTo(spaceGlyph);
                                    spaceGlyph.GlyphCharacter = ' ';

                                    c++;
                                    yield return new();
                                }

                                // If we had a \r in the string, handle it by going back
                                if (returnParts.Length != 1 && indexR != returnParts.Length - 1)
                                {
                                    // Wrapped to a new line through print glyph, which triggered \r\n. We don't want the \n so return back.
                                    if (_position.X == 0 && _position.Y != currentLine)
                                        _position = new Point(_position.X, _position.Y - 1);
                                    else
                                        CarriageReturn();

                                    c++;
                                    yield return new();
                                }
                            }
                        }

                        // We had \n in the string, handle them.
                        if (newlineParts.Length != 1 && indexNL != newlineParts.Length - 1)
                        {
                            if (!UseLinuxLineEndings)
                                LineFeed();
                            else
                                NewLine();

                            c++;
                            yield return new();
                        }
                    }
                }

                // Not last part
                if (wordMajor != parts.Length - 1 && _position.X != 0)
                {
                    PrintGlyph(spaceGlyph, text);
                    yield return new();
                    c++;
                }
                else
                    c++;
            }
        }
        else
        {
            bool movedLines = false;
            int oldLine = _position.Y;

            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                ColoredGlyphAndEffect glyph = text[i];
                // Check if the previous print moved us down a line (from print at end of the line) and move us back for the \r
                if (movedLines)
                {
                    if (_position.X == 0 && glyph.GlyphCharacter == '\r')
                    {
                        _position = new Point(_position.X, _position.Y - 1);
                        continue;
                    }
                    else
                        movedLines = false;
                }

                if (glyph.GlyphCharacter == '\r')
                    CarriageReturn();

                else if (glyph.GlyphCharacter == '\n')
                {
                    if (!UseLinuxLineEndings)
                        LineFeed();
                    else
                        NewLine();
                }
                else
                {
                    PrintGlyph(glyph, text);

                    // Lines changed and it wasn't a \n that caused it, so it was a print that did it.
                    movedLines = _position.Y != oldLine;
                    yield return new();
                }
            }
        }
    }

    /// <summary>
    /// Prints text to the console using the appearance of the colored string.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <returns>Returns this cursor object.</returns>
    public Cursor Print(ColoredString text)
    {
        // This is a copy of the code in PrintCoroutine. Updates in one should happen in the other.
        // TODO: Find a better way to do this and share code.
        if (_editor == null) throw new Exception("A host is not attached, cannot print.");

        if (text.Length == 0) return this;

        _cursorCellEffect?.Restart();
        _cursorCellEffect?.ApplyToCell(_cursorCell, _cursorCellOriginal);

        // If we don't want the pretty print, or we're printing a single character (for example, from keyboard input)
        // Then use the pretty print system.
        if (!DisableWordBreak && text.String.Length != 1)
        {
            // Prep
            ColoredGlyphAndEffect glyph;
            ColoredGlyphAndEffect spaceGlyph = (ColoredGlyphAndEffect)text[0].Clone();

            spaceGlyph.GlyphCharacter = ' ';
            string stringText = text.String.TrimEnd(' ');

            // Pull any starting spaces off
            string newStringText = stringText.TrimStart(' ');
            int spaceCount = stringText.Length - newStringText.Length;

            for (int i = 0; i < spaceCount; i++)
                PrintGlyph(spaceGlyph, text);

            if (spaceCount != 0)
                text = text.SubString(spaceCount, text.Length - spaceCount);

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
                                        PrintGlyph(spaceGlyph, text);
                                }

                                // Print the rest of the text as normal.
                                for (int i = 0; i < returnParts[indexR].Length; i++)
                                {
                                    glyph = text[c];

                                    PrintGlyph(glyph, text);

                                    // Update the space glyph with the last character printed.
                                    glyph.CopyAppearanceTo(spaceGlyph);
                                    spaceGlyph.GlyphCharacter = ' ';

                                    c++;
                                }

                                // If we had a \r in the string, handle it by going back
                                if (returnParts.Length != 1 && indexR != returnParts.Length - 1)
                                {
                                    // Wrapped to a new line through print glyph, which triggered \r\n. We don't want the \n so return back.
                                    if (_position.X == 0 && _position.Y != currentLine)
                                        _position = new Point(_position.X, _position.Y - 1);
                                    else
                                        CarriageReturn();

                                    c++;
                                }
                            }
                        }

                        // We had \n in the string, handle them.
                        if (newlineParts.Length != 1 && indexNL != newlineParts.Length - 1)
                        {
                            if (!UseLinuxLineEndings)
                                LineFeed();
                            else
                                NewLine();

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
                    c++;
            }
        }
        else
        {
            bool movedLines = false;
            int oldLine = _position.Y;

            int count = text.Length;
            for (int i = 0; i < count; i++)
            {
                ColoredGlyphAndEffect glyph = text[i];
                // Check if the previous print moved us down a line (from print at end of the line) and move use back for the \r
                if (movedLines)
                {
                    if (_position.X == 0 && glyph.GlyphCharacter == '\r')
                    {
                        _position = new Point(_position.X, _position.Y - 1);
                        continue;
                    }
                    else
                        movedLines = false;
                }

                if (glyph.GlyphCharacter == '\r')
                    CarriageReturn();

                else if (glyph.GlyphCharacter == '\n')
                {
                    if (!UseLinuxLineEndings)
                        LineFeed();
                    else
                        NewLine();
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
        if (_editor == null) throw new Exception("A host is not attached, cannot use line feed.");

        if (_position.Y == _editor.Height - 1)
        {
            if (AutomaticallyShiftRowsUp)
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
    /// Moves the cursor up by the specified amount of lines.
    /// </summary>
    /// <param name="amount">The amount of lines to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor Up(int amount)
    {
        Position = new Point(_position.X, _position.Y - amount);
        return this;
    }

    /// <summary>
    /// Moves the cursor down by the specified amount of lines.
    /// </summary>
    /// <param name="amount">The amount of lines to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor Down(int amount)
    {
        Position = new Point(_position.X, _position.Y + amount);
        return this;
    }

    /// <summary>
    /// Moves the cursor left by the specified amount of columns.
    /// </summary>
    /// <param name="amount">The amount of columns to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor Left(int amount)
    {
        Position = new Point(_position.X - amount, _position.Y);
        return this;
    }

    /// <summary>
    /// Moves the cursor left by the specified amount of columns, wrapping the cursor if needed.
    /// </summary>
    /// <param name="amount">The amount of columns to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor LeftWrap(int amount)
    {
        if (_editor == null) throw new Exception("A host is not attached, cannot move left.");

        int index = Point.ToIndex(_position.X, _position.Y, _editor.Width) - amount;

        if (index < 0)
        {
            index = 0;
        }

        _position = Point.FromIndex(index, _editor.Width);

        return this;
    }

    /// <summary>
    /// Moves the cursor right by the specified amount of columns.
    /// </summary>
    /// <param name="amount">The amount of columns to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor Right(int amount)
    {
        Position = new Point(_position.X + amount, _position.Y);

        return this;
    }

    /// <summary>
    /// Moves the cursor right by the specified amount of columns, wrapping the cursor if needed.
    /// </summary>
    /// <param name="amount">The amount of columns to move the cursor</param>
    /// <returns>This cursor object.</returns>
    public Cursor RightWrap(int amount)
    {
        if (_editor == null) throw new Exception("A host is not attached, cannot move right.");

        int index = Point.ToIndex(_position.X, _position.Y, _editor.Width) + amount;

        if (index > _editor.Count)
        {
            index = _editor.Count - 1;
        }

        _position = Point.FromIndex(index, _editor.Width);

        return this;
    }

    private ColoredGlyphAndEffect CreateEmptyGlyph(int glyph = 0)
    {
        ColoredGlyphAndEffect glyphObj;

        if (PrintAppearanceMatchesHost)
        {
            glyphObj = new ColoredGlyphAndEffect()
            {
                Foreground = _editor!.DefaultForeground,
                Background = _editor!.DefaultBackground,
            };
        }
        else
            glyphObj = ColoredGlyphAndEffect.FromColoredGlyph(PrintAppearance);

        if (UsePrintEffect)
            glyphObj.Effect = PrintEffect;

        return glyphObj;
    }

    /// <summary>
    /// Clears the cell at the position of the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cell. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor Erase(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        CreateEmptyGlyph(glyph).CopyAppearanceTo(_editor[_position]);

        return this;
    }

    /// <summary>
    /// Clears the cells in the same column as the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseColumn(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int y = 0; y < _editor.Height; y++)
            glyphObj.CopyAppearanceTo(_editor[_position.X, y]);

        return this;
    }

    /// <summary>
    /// Clears the cells in the same row as the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseRow(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int x = 0; x < _editor.Width; x++)
            glyphObj.CopyAppearanceTo(_editor[x, _position.Y]);

        return this;
    }

    /// <summary>
    /// Clears the cells above the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseUp(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int y = _position.Y - 1; y >= 0; y--)
            glyphObj.CopyAppearanceTo(_editor[_position.X, y]);

        return this;
    }

    /// <summary>
    /// Clears the cells below the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseDown(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int y = _position.Y + 1; y < _editor.Height; y++)
            glyphObj.CopyAppearanceTo(_editor[_position.X, y]);

        return this;
    }

    /// <summary>
    /// Clears the cells to the left of the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseLeft(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int x = _position.X - 1; x >= 0; x--)
            glyphObj.CopyAppearanceTo(_editor[x, _position.Y]);

        return this;
    }

    /// <summary>
    /// Clears the cells to the right of the cursor.
    /// </summary>
    /// <param name="glyph">The glyph to set on the erased cells. Defaults to glyph index 0.</param>
    /// <returns>The cursor object.</returns>
    /// <exception cref="Exception">Thrown when a host isn't attached to the cursor.</exception>
    public Cursor EraseRight(int glyph = 0)
    {
        if (_editor == null) throw new Exception("A host isn't attached to this cursor.");

        ColoredGlyphAndEffect glyphObj = CreateEmptyGlyph(glyph);

        for (int x = _position.X + 1; x < _editor.Width; x++)
            glyphObj.CopyAppearanceTo(_editor[x, _position.Y]);

        return this;
    }

    void IComponent.Update(IScreenObject host, TimeSpan delta)
    {
        if (IsVisible && _applyCursorEffect && _cursorCellEffect != null)
        {
            _cursorCellEffect.Update(delta);
            _cursorCellEffect.ApplyToCell(_cursorCell, _cursorCellOriginal);
        }
    }

    void IComponent.Render(IScreenObject host, TimeSpan delta)
    {
        throw new NotImplementedException();
    }

    void IComponent.ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
    {
        handled = false;

        if (MouseClickReposition && state.IsOnScreenObject && state.Mouse.LeftClicked)
        {
            Position = state.CellPosition;
            handled = MouseClickRepositionHandlesMouse;
        }
    }

    void IComponent.ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
    {
        handled = false;

        if (!IsEnabled) return;

        System.Collections.ObjectModel.ReadOnlyCollection<AsciiKey> keysPressed = keyboard.KeysPressed;
        int count = keysPressed.Count;
        for (int i = 0; i < count; i++)
        {
            Input.AsciiKey key = keysPressed[i];
            // If someone attached an event to KeyboardPreview, process it
            if (KeyboardPreview != null)
            {
                KeyboardHandledKeyEventArgs args = new();
                args.Key = key;
                KeyboardPreview.Invoke(this, args);
                if (args.IsHandled)
                    continue;
            }

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

    void IComponent.OnAdded(IScreenObject host)
    {
        if (host is IScreenSurface surface)
            _editor = surface.Surface;
        else
            throw new ArgumentException($"This component can only be added to a type that implements {nameof(IScreenSurface)}.");

        _cursorRenderStep?.Dispose();
        _cursorRenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.Cursor);
        _cursorRenderStep.SetData(this);
        surface.Renderer?.Steps.Add(_cursorRenderStep);
        surface.Renderer?.Steps.Sort(RenderStepComparer.Instance);
    }

    void IComponent.OnRemoved(IScreenObject host)
    {
        _editor = null;

        if (_cursorRenderStep != null)
        {
            ((IScreenSurface)host).Renderer?.Steps.Remove(_cursorRenderStep);
            ((IScreenSurface)host).IsDirty = true;
            _cursorRenderStep.Dispose();
            _cursorRenderStep = null;
        }
    }

    void IComponent.OnHostUpdated(IScreenObject host)
    {
        if (host is IScreenSurface surface)
            _editor = surface.Surface;
        else
            throw new ArgumentException($"Host must be a {nameof(IScreenSurface)}", nameof(host));

        if (!_editor.IsValidCell(_position.X, _position.Y))
            Position = (0, 0);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _cursorCellOriginal = null;
                _cursorCellEffect = null;
                _cursorCell = null;
                _editor = null;
                PrintAppearance = null;
                PrintEffect = null;
                CursorRenderEffect = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            _cursorRenderStep?.Dispose();
            _cursorRenderStep = null;

            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    ~Cursor()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
