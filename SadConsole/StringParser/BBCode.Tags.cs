using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.StringParser.BBCode;

public class Strikethrough : BBCodeCommandBase
{
    /// <summary>
    /// The glyph used for the strikethrough decorator.
    /// </summary>
    public static int Glyph = 196;

    public Strikethrough() =>
        CommandType = CommandTypes.Decorator;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        CellDecorator decorator = new CellDecorator(Color.AnsiWhite, Glyph, Mirror.None);

        // Create decorator list if needed
        glyphState.Decorators ??= new();

        // If decorator isn't already in the glyph state decorators, add it
        if (glyphState.Decorators.IndexOf(decorator) == -1)
            glyphState.Decorators.Add(decorator);
    }
}

public class Underline : BBCodeCommandBase
{
    /// <summary>
    /// The glyph used for the underline decorator.
    /// </summary>
    public static int Glyph = 95;

    public Underline() =>
        CommandType = CommandTypes.Decorator;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        CellDecorator decorator = new CellDecorator(Color.AnsiWhite, Glyph, Mirror.None);

        // Create decorator list if needed
        glyphState.Decorators ??= new();

        // If decorator isn't already in the glyph state decorators, add it
        if (glyphState.Decorators.IndexOf(decorator) == -1)
            glyphState.Decorators.Add(decorator);
    }
}

public class Recolor : BBCodeCommandBase
{
    public Color Color { get; set; }

    public Recolor() =>
        CommandType = CommandTypes.Foreground;

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

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Foreground = Color;
    }
}

public class Bold : BBCodeCommandBase
{
    /// <summary>
    /// The offset applied to the glyph to get the emboldened variant.
    /// </summary>
    public static int GlyphOffset = 1;

    public Bold() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Glyph += GlyphOffset;
    }
}

public class Italic : BBCodeCommandBase
{
    /// <summary>
    /// The offset applied to the glyph to get the italicized variant.
    /// </summary>
    public static int GlyphOffset = -1;

    public Italic() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Glyph += GlyphOffset;
    }
}

public class Upper : BBCodeCommandBase
{
    public Upper() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.GlyphCharacter = char.ToUpper(glyphState.GlyphCharacter);
    }
}

public class Lower : BBCodeCommandBase
{
    public Lower() =>
        CommandType = CommandTypes.Glyph;

    /// <inheritdoc />
    public override void SetBBCode(string tag, string? value, Dictionary<string, string>? parameters)
    {
        Tag = tag;
    }

    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex, ICellSurface? surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.GlyphCharacter = char.ToLower(glyphState.GlyphCharacter);
    }
}
