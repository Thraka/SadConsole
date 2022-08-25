using System;
using System.Collections.Generic;
using SadConsole.Extensions;
using static SadConsole.ColoredString;

namespace SadConsole.StringParser;

/// <summary>
/// The default string parser.
/// </summary>
public class Default : IParser
{
    /// <summary>
    /// Custom processor called if any built in command is not triggerd. Signature is ("command", "parameters", existing glyphs, text surface, associated editor, command stacks).
    /// </summary>
    public Func<string, string, ColoredGlyphAndEffect[], ICellSurface?, ParseCommandStacks?, ParseCommandBase>? CustomProcessor;

    /// <summary>
    /// Creates a colored string by parsing commands embedded in the string.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="surfaceIndex">Index of where this string will be printed.</param>
    /// <param name="surface">The surface the string will be printed to.</param>
    /// <param name="initialBehaviors">Any initial defaults.</param>
    /// <returns>The finalized string.</returns>
    public ColoredString Parse(ReadOnlySpan<char> value, int surfaceIndex = -1, ICellSurface? surface = null, ParseCommandStacks? initialBehaviors = null)
    {
        ParseCommandStacks commandStacks = initialBehaviors ?? new ParseCommandStacks();
        List<ColoredGlyphAndEffect> glyphs = new List<ColoredGlyphAndEffect>(value.Length);

        for (int i = 0; i < value.Length; i++)
        {
            ColoredGlyphAndEffect[] existingGlyphs = glyphs.ToArray();

            // Check for an escaped `[ sequence and ignore the escape character `
            if (value[i] == '`' && i + 1 < value.Length && value[i + 1] == '[')
                continue;

            // If [ is encountered, and the previous character isn't an escape character ` try to process the command
            if (value[i] == '[' && (i == 0 || value[i - 1] != '`'))
            {
                try
                {
                    // Check for the next part of the magic sequence
                    // If the rest of the string is long enough to contain the start of a command, check
                    // Check for c: and then a ] character for the end of the command.
                    if (i + 4 < value.Length && value[i + 1] == 'c' && value[i + 2] == ':' && value.Slice(i).Next(']', out int commandExitIndex))
                    {
                        // Pull the contents after [c: and before ]
                        commandExitIndex += i;

                        string commandParams;
                        string command;
                        ReadOnlySpan<char> fullCommand = value.Slice(i + 3, commandExitIndex - (i + 3));
                        int splitPos = fullCommand.IndexOf(' ');

                        if (splitPos != -1)
                        {
                            commandParams = fullCommand.Slice(splitPos + 1).ToString();
                            command = fullCommand.Slice(0, splitPos).ToString();
                        }
                        else
                        {
                            commandParams = String.Empty;
                            command = fullCommand.ToString();
                        }
                        // Check for custom command
                        // Send the method the command name, the parameters, the current glyphs that have been created, the surface (if it's being actively printed), and the existing commands
                        ParseCommandBase? commandObject = CustomProcessor?.Invoke(command, commandParams, existingGlyphs, surface, commandStacks);

                        // No custom command found, run build in ones
                        if (commandObject == null)
                        {
                            switch (command)
                            {
                                case "recolor":
                                case "r":
                                    commandObject = new ParseCommandRecolor(commandParams);
                                    break;
                                case "mirror":
                                case "m":
                                    commandObject = new ParseCommandMirror(commandParams);
                                    break;
                                case "d":
                                    commandObject = new ParseCommandDecorator(commandParams);
                                    break;
                                case "undo":
                                case "u":
                                    commandObject = new ParseCommandUndo(commandParams, commandStacks);
                                    break;
                                case "grad":
                                case "g":
                                    commandObject = new ParseCommandGradient(commandParams);
                                    break;
                                case "blink":
                                case "b":
                                    commandObject = new ParseCommandBlink(commandParams, existingGlyphs, commandStacks, surface);
                                    break;
                                case "sglyph":
                                case "sg":
                                    commandObject = new ParseCommandSetGlyph(commandParams);
                                    break;
                                case "ceffect":
                                case "ce":
                                    commandObject = new ParseCommandClearEffect(commandParams, commandStacks);
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (commandObject != null && commandObject.CommandType != CommandTypes.Invalid)
                        {
                            commandStacks.AddSafe(commandObject);

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
                catch () { }
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
                commandStacks.Decorator.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            // Effect
            if (commandStacks.Effect.Count != 0)
                commandStacks.Effect.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

            glyphs.Add(newGlyph);
        }

        return new ColoredString(glyphs.ToArray());
    }


}
