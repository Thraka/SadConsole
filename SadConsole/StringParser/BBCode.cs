using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.Extensions;

namespace SadConsole.StringParser.BBCode;

/// <summary>
/// A BBCode string parser.
/// </summary>
public class Parser : IParser
{
    /// <summary>
    /// A collection of tags used by the parser.
    /// </summary>
    public Dictionary<string, Type> Tags { get; set; } = new();

    /// <summary>
    /// Creates a new instanace of the class.
    /// </summary>
    public Parser()
    {
        Tags.Add("color", typeof(Recolor));
        Tags.Add("u", typeof(Underline));
        Tags.Add("b", typeof(Bold));
        Tags.Add("i", typeof(Italic));
        Tags.Add("s", typeof(Strikethrough));
        Tags.Add("upper", typeof(Upper));
        Tags.Add("lower", typeof(Lower));
    }

    private BBCodeCommandBase CreateBBCodeCommand(Type type)
    {
        return Activator.CreateInstance(type) as BBCodeCommandBase
            ?? throw new NullReferenceException($"Unable to create an instance of {nameof(type.Name)}.");
    }

    /// <summary>
    /// Creates a colored string by parsing BBCode commands embedded in the string.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="surfaceIndex">Index of where this string will be printed.</param>
    /// <param name="surface">The surface the string will be printed to.</param>
    /// <param name="initialBehaviors">Any initial defaults.</param>
    /// <returns>The finalized string.</returns>
    public ColoredString Parse(ReadOnlySpan<char> value, int surfaceIndex = -1, ICellSurface? surface = null, ParseCommandStacks? initialBehaviors = null)
    {
        bool hasDecorator = false;
        ParseCommandStacks commandStacks = initialBehaviors ?? new ParseCommandStacks();
        List<ColoredGlyphAndEffect> glyphs = new(value.Length);

        for (int i = 0; i < value.Length; i++)
        {
            ColoredGlyphAndEffect[] existingGlyphs = glyphs.ToArray();

            // Check for an escaped `[ sequence and ignore the escape character `
            if (value[i] == '`' && i + 1 < value.Length && value[i + 1] == '[')
                continue;

            if (value[i] == '[' && (i == 0 || value[i - 1] != '`') && (i == value.Length - 1 || value[i + 1] == '/'))
            {
                try
                {
                    if (value.Slice(i).Next(']', out int commandExitIndex))
                    {
                        // Pull the contents after [ and before ]
                        commandExitIndex += i;

                        ReadOnlySpan<char> fullCommand = value.Slice(i + 2, commandExitIndex - i - 2);
                        string fullCommandString = fullCommand.ToString();
                        BBCodeCommandBase? foundItem = null;
                        foreach (BBCodeCommandBase item in commandStacks.All.Cast<BBCodeCommandBase>())
                        {
                            if (item.Tag.Equals(fullCommandString, StringComparison.OrdinalIgnoreCase))
                            {
                                foundItem = item;
                                break;
                            }
                        }

                        if (foundItem != null)
                        {
                            commandStacks.RemoveSafe(foundItem);
                            i = commandExitIndex;
                            continue;
                        }
                    }
                }
#if DEBUG
                catch (Exception e1)
                {
                    // Helps track down parsing bugs
                    System.Diagnostics.Debug.WriteLine(e1);
                    System.Diagnostics.Debugger.Break();
                }
#else
                catch (Exception _) { }
#endif
            }

            // If [ is encountered, and the previous character isn't an escape character ` try to process the command
            else if (value[i] == '[' && (i == 0 || value[i - 1] != '`'))
            {
                try
                {
                    if (value.Slice(i).Next(']', out int commandExitIndex))
                    {
                        // Pull the contents after [ and before ]
                        commandExitIndex += i;

                        string tag;
                        string? tagValue = null;
                        List<KeyValuePair<string, string>> complexValues = new();

                        ReadOnlySpan<char> fullCommand = value.Slice(i + 1, commandExitIndex - i - 1);

                        // Simple tag, no space no equals
                        if (!fullCommand.Contains(' ') && !fullCommand.Contains('='))
                        {
                            tag = fullCommand.ToString();
                        }

                        // Value tag, no space yes equals
                        else if (!fullCommand.Contains(' ') && fullCommand.Contains('='))
                        {
                            int equalIndex = fullCommand.IndexOf('=');
                            tag = fullCommand.Slice(0, equalIndex).ToString();
                            tagValue = fullCommand.Slice(equalIndex + 1).ToString();
                        }

                        // Complex tag with parameters
                        else
                        {
                            throw new Exception("Tag complex parameters not yet supported.");
                        }

                        // Search for command
                        if (Tags.ContainsKey(tag))
                        {
                            BBCodeCommandBase command = CreateBBCodeCommand(Tags[tag]);
                            command.SetBBCode(tag, tagValue, null);
                            if (command.CommandType != CommandTypes.Invalid)
                            {
                                commandStacks.AddSafe(command);

                                i = commandExitIndex;
                                continue;
                            }
                        }
                    }

                }
#if DEBUG
                catch (Exception e1)
                {
                    // Helps track down parsing bugs
                    System.Diagnostics.Debug.WriteLine(e1);
                    System.Diagnostics.Debugger.Break();
                }
#else
                catch (Exception _) { }
#endif
            }

            // If the string is associated with a surface, pull the glyph out and use its colors
            int fixedSurfaceIndex;

            if (surfaceIndex == -1 || surface == null)
                fixedSurfaceIndex = -1;
            else
                // If the index is within range of the surface, use it, otherwise -1
                fixedSurfaceIndex = i + surfaceIndex < surface.Count ? i + surfaceIndex : -1;

            ColoredGlyphAndEffect newGlyph;

            // If surface index is in range, copy the surface colors to the new glyph
            if (fixedSurfaceIndex != -1)
            {
                newGlyph = new ColoredGlyphAndEffect();
                surface![i + surfaceIndex].CopyAppearanceTo(newGlyph);

                Effects.ICellEffect? effect = surface.GetEffect(i + surfaceIndex);

                // Get the glyph's character from the string
                newGlyph.Glyph = value[i];
                newGlyph.Effect = effect;
            }
            else
            {
                // Create new noncolored glyph.
                newGlyph = new ColoredGlyphAndEffect()
                {
                    Glyph = value[i]
                };
            }

            // Foreground
            if (commandStacks.Foreground.Count != 0)
                commandStacks.Foreground.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            // Background
            if (commandStacks.Background.Count != 0)
                commandStacks.Background.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            // Glyph
            if (commandStacks.Glyph.Count != 0)
                commandStacks.Glyph.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            // Mirror
            if (commandStacks.Mirror.Count != 0)
                commandStacks.Mirror.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            // Decorator
            if (commandStacks.Decorator.Count != 0)
            {
                commandStacks.Decorator.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                hasDecorator = true;
            }

            // Effect
            if (commandStacks.Effect.Count != 0)
                commandStacks.Effect.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            glyphs.Add(newGlyph);
        }

        return new ColoredString(glyphs.ToArray()) { IgnoreDecorators = !hasDecorator };
    }
}
