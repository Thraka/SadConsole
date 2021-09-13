using System;
using System.Collections.Generic;
using SadConsole.StringParser;

namespace SadConsole
{
    public partial class ColoredString
    {
        /// <summary>
        /// Custom processor called if any built in command is not triggerd. Signature is ("command", "sub command", existing glyphs, text surface, associated editor, command stacks).
        /// </summary>
        public static Func<string, string, ColoredGlyphEffect[], ICellSurface, ParseCommandStacks, ParseCommandBase> CustomProcessor;

        /// <summary>
        /// Creates a colored string by parsing commands embedded in the string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="surfaceIndex">Index of where this string will be printed.</param>
        /// <param name="surface">The surface the string will be printed to.</param>
        /// <param name="initialBehaviors">Any initial defaults.</param>
        /// <returns>The finalized string.</returns>
        public static ColoredString Parse(string value, int surfaceIndex = -1, ICellSurface surface = null, ParseCommandStacks initialBehaviors = null)
        {
            ParseCommandStacks commandStacks = initialBehaviors ?? new ParseCommandStacks();
            List<ColoredGlyphEffect> glyphs = new List<ColoredGlyphEffect>(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                ColoredGlyphEffect[] existingGlyphs = glyphs.ToArray();

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
                        if (i + 4 < value.Length && value[i + 1] == 'c' && value[i + 2] == ':' && value.IndexOf(']', i + 2) != -1)
                        {
                            // Pull the contents after [c: and before ]
                            int commandExitIndex = value.IndexOf(']', i + 2);
                            string command = value.Substring(i + 3, commandExitIndex - (i + 3));
                            string commandParams = "";

                            // If the command text contains a space, it contains a parameter
                            if (command.Contains(" "))
                            {
                                string[] commandSections = command.Split(new char[] { ' ' }, 2);
                                command = commandSections[0].ToLower();
                                commandParams = commandSections[1];
                            }

                            // Check for custom command
                            // Send the method the command name, the parameters, the current glyphs that have been created, the surface (if it's being actively printed), and the existing commands
                            ParseCommandBase commandObject = CustomProcessor?.Invoke(command, commandParams, existingGlyphs, surface, commandStacks);

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
                    catch (Exception e1)
                    {
#if DEBUG
                        // Helps track down parsing bugs
                        System.Diagnostics.Debugger.Break();
#endif
                    }
                }

                int fixedSurfaceIndex;

                if (surfaceIndex == -1 || surface == null)
                    fixedSurfaceIndex = -1;
                else
                    fixedSurfaceIndex = i + surfaceIndex < surface.Count ? i + surfaceIndex : -1;

                ColoredGlyphEffect newGlyph;

                if (fixedSurfaceIndex != -1)
                {
                    newGlyph = new ColoredGlyphEffect();
                    surface[i + surfaceIndex].CopyAppearanceTo(newGlyph);
                    newGlyph.Glyph = value[i];
                }
                else
                {
                    newGlyph = new ColoredGlyphEffect()
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

                // Effect
                if (commandStacks.Effect.Count != 0)
                    commandStacks.Effect.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                glyphs.Add(newGlyph);
            }

            return new ColoredString(glyphs.ToArray()) { IgnoreEffect = !commandStacks.TurnOnEffects };
        }
    }
}
