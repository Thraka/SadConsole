using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.StringParser.BBCode;

/// <summary>
/// Implements a BBCode tag for strikethrough text formatting.
/// </summary>
public class Strikethrough : BBCodeCommandBase
{
    /// <summary>
    /// The glyph used for the strikethrough decorator.
    /// </summary>
    public static int Glyph = 196;

    /// <summary>
    /// Initializes a new instance of the <see cref="Strikethrough"/> class.
    /// </summary>
    public Strikethrough() =>
        CommandType = CommandTypes.Decorator;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        CellDecorator decorator = new(Color.AnsiWhite, Glyph, Mirror.None);

        // Create decorator list if needed
        glyphState.Decorators ??= new();

        // If decorator isn't already in the glyph state decorators, add it
        if (glyphState.Decorators.IndexOf(decorator) == -1)
            glyphState.Decorators.Add(decorator);
    }
}

/// <summary>
/// Implements a BBCode tag for underlined text formatting.
/// </summary>
public class Underline : BBCodeCommandBase
{
    /// <summary>
    /// The glyph used for the underline decorator.
    /// </summary>
    public static int Glyph = 95;

    /// <summary>
    /// Initializes a new instance of the <see cref="Underline"/> class.
    /// </summary>
    public Underline() =>
        CommandType = CommandTypes.Decorator;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        CellDecorator decorator = new(Color.AnsiWhite, Glyph, Mirror.None);

        // Create decorator list if needed
        glyphState.Decorators ??= new();

        // If decorator isn't already in the glyph state decorators, add it
        if (glyphState.Decorators.IndexOf(decorator) == -1)
            glyphState.Decorators.Add(decorator);
    }
}

/// <summary>
/// Implements a BBCode tag for changing text color.
/// </summary>
public class Recolor : BBCodeCommandBase
{
    /// <summary>
    /// Gets or sets the color to apply to the text.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Recolor"/> class.
    /// </summary>
    public Recolor() =>
        CommandType = CommandTypes.Foreground;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;

        string valueToParse;

        if (value != null)
            valueToParse = value!;
        else if (parameters != null)
        {
            if (!parameters.ContainsKey("color")) throw new Exception("BBCode is missing parameter 'color'.");

            valueToParse = parameters["color"];
        }
        else
            throw new Exception("BBCode condition is invalid. Expected complex or parameterized.");

        Color = Color.White.FromParser(valueToParse, out _, out _, out _, out _, out _);
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Foreground = Color;
    }
}

/// <summary>
/// Implements a BBCode tag for bold text formatting.
/// </summary>
public class Bold : BBCodeCommandBase
{
    /// <summary>
    /// The offset applied to the glyph to get the emboldened variant.
    /// </summary>
    public static int GlyphOffset = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bold"/> class.
    /// </summary>
    public Bold() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Glyph += GlyphOffset;
    }
}

/// <summary>
/// Implements a BBCode tag for italic text formatting.
/// </summary>
public class Italic : BBCodeCommandBase
{
    /// <summary>
    /// The offset applied to the glyph to get the italicized variant.
    /// </summary>
    public static int GlyphOffset = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Italic"/> class.
    /// </summary>
    public Italic() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Glyph += GlyphOffset;
    }
}

/// <summary>
/// Implements a BBCode tag for converting text to uppercase.
/// </summary>
public class Upper : BBCodeCommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Upper"/> class.
    /// </summary>
    public Upper() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.GlyphCharacter = char.ToUpper(glyphState.GlyphCharacter);
    }
}

/// <summary>
/// Implements a BBCode tag for converting text to lowercase.
/// </summary>
public class Lower : BBCodeCommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Lower"/> class.
    /// </summary>
    public Lower() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.GlyphCharacter = char.ToLower(glyphState.GlyphCharacter);
    }
}
