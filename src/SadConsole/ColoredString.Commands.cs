namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using SadConsole.StringParser;

    public partial class ColoredString
    {
        /// <summary>
        /// Custom processor called if any built in command is not triggerd. Signature is ("command", "sub command", existing glyphs, text surface, associated editor, command stacks).
        /// </summary>
        public static Func<string, string, ColoredGlyph[], CellSurface, ParseCommandStacks, ParseCommandBase> CustomProcessor;

        /// <summary>
        /// Creates a colored string by parsing commands embedded in the string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="surfaceIndex">Index of where this string will be printed.</param>
        /// <param name="surface">The surface the string will be printed to.</param>
        /// <param name="initialBehaviors">Any initial defaults.</param>
        /// <returns></returns>
        public static ColoredString Parse(string value, int surfaceIndex = -1, CellSurface surface = null, ParseCommandStacks initialBehaviors = null)
        {
            ParseCommandStacks commandStacks = initialBehaviors ?? new ParseCommandStacks();
            List<ColoredGlyph> glyphs = new List<ColoredGlyph>(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                ColoredGlyph[] existingGlyphs = glyphs.ToArray();

                if (value[i] == '`' && i + 1 < value.Length && value[i + 1] == '[')
                {
                    continue;
                }

                if (value[i] == '[' && (i == 0 || value[i - 1] != '`'))
                {
                    try
                    {
                        if (i + 4 < value.Length && value[i + 1] == 'c' && value[i + 2] == ':' && value.IndexOf(']', i + 2) != -1)
                        {
                            int commandExitIndex = value.IndexOf(']', i + 2);
                            string command = value.Substring(i + 3, commandExitIndex - (i + 3));
                            string commandParams = "";

                            if (command.Contains(" "))
                            {
                                string[] commandSections = command.Split(new char[] { ' ' }, 2);
                                command = commandSections[0].ToLower();
                                commandParams = commandSections[1];
                            }

                            // Check for custom command
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
                        throw e1;
#endif
                    }
                }

                int fixedSurfaceIndex;

                if (surfaceIndex == -1 || surface == null)
                {
                    fixedSurfaceIndex = -1;
                }
                else
                {
                    fixedSurfaceIndex = i + surfaceIndex < surface.Cells.Length ? i + surfaceIndex : -1;
                }

                ColoredGlyph newGlyph;

                if (fixedSurfaceIndex != -1)
                {
                    newGlyph = new ColoredGlyph(surface[i + surfaceIndex]) { Glyph = value[i] };
                }
                else
                {
                    newGlyph = new ColoredGlyph(new Cell()) { Glyph = value[i] };
                }


                // Foreground
                if (commandStacks.Foreground.Count != 0)
                {
                    commandStacks.Foreground.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                }

                // Background
                if (commandStacks.Background.Count != 0)
                {
                    commandStacks.Background.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                }

                if (commandStacks.Glyph.Count != 0)
                {
                    commandStacks.Glyph.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                }

                // SpriteEffect
                if (commandStacks.Mirror.Count != 0)
                {
                    commandStacks.Mirror.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                }

                // Effect
                if (commandStacks.Effect.Count != 0)
                {
                    commandStacks.Effect.Peek().Build(ref newGlyph, existingGlyphs, fixedSurfaceIndex, surface, ref i, value, commandStacks);
                }

                glyphs.Add(newGlyph);
            }

            return new ColoredString(glyphs.ToArray()) { IgnoreEffect = !commandStacks.TurnOnEffects };
        }
    }
}
