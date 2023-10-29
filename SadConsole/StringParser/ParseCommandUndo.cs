﻿
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SadConsole.StringParser;
/// <summary>
/// Pops a behavior off of a <see cref="ParseCommandStacks"/>.
/// </summary>
public sealed class ParseCommandUndo : ParseCommandBase
{
    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    /// <param name="stacks">The current commands for the string.</param>
    public ParseCommandUndo(string parameters, ParseCommandStacks stacks)
    {
        var badCommandException = new ArgumentException("command is invalid for Undo: " + parameters);
        string[] parts = parameters.Split(new char[] { ':' }, 3);
        int times = 1;
        bool isSpecificStack = false;
        CommandTypes stackType = CommandTypes.Invalid;

        if (parts.Length > 1)
        {
            isSpecificStack = true;

            switch (parts[1])
            {
                case "f":
                    stackType = CommandTypes.Foreground;
                    break;
                case "b":
                    stackType = CommandTypes.Background;
                    break;
                case "g":
                    stackType = CommandTypes.Glyph;
                    break;
                case "e":
                    stackType = CommandTypes.Effect;
                    break;
                case "m":
                    stackType = CommandTypes.Mirror;
                    break;
                case "a":
                    isSpecificStack = false;
                    break;
                default:
                    throw badCommandException;
            }
        }

        if (parts.Length >= 1 && parts[0] != "")
            times = int.Parse(parts[0], CultureInfo.InvariantCulture);

        for (int i = 0; i < times; i++)
        {
            ParseCommandBase? behavior = null;

            if (!isSpecificStack)
            {
                if (stacks.All.Count != 0)
                {
                    behavior = stacks.All.Pop();

                    switch (behavior.CommandType)
                    {
                        case CommandTypes.Foreground:
                            stacks.Foreground.Pop();
                            break;
                        case CommandTypes.Background:
                            stacks.Background.Pop();
                            break;
                        case CommandTypes.Glyph:
                            stacks.Glyph.Pop();
                            break;
                        case CommandTypes.Mirror:
                            stacks.Mirror.Pop();
                            break;
                        case CommandTypes.Effect:
                            stacks.Effect.Pop();
                            break;
                        default:
                            break;
                    }
                }
                else
                    break;
            }
            else
            {
                switch (stackType)
                {
                    case CommandTypes.Foreground:
                        if (stacks.Foreground.Count != 0)
                            behavior = stacks.Foreground.Pop();

                        break;
                    case CommandTypes.Background:
                        if (stacks.Background.Count != 0)
                            behavior = stacks.Background.Pop();

                        break;
                    case CommandTypes.Glyph:
                        if (stacks.Glyph.Count != 0)
                            behavior = stacks.Glyph.Pop();

                        break;
                    case CommandTypes.Mirror:
                        if (stacks.Mirror.Count != 0)
                            behavior = stacks.Mirror.Pop();

                        break;
                    case CommandTypes.Effect:
                        if (stacks.Effect.Count != 0)
                            behavior = stacks.Effect.Pop();

                        break;
                    default:
                        break;
                }

                if (behavior != null)
                {
                    List<ParseCommandBase> all = new List<ParseCommandBase>(stacks.All);
                    all.Remove(behavior);
                    stacks.All = new Stack<ParseCommandBase>(all);
                }
            }
        }

        CommandType = CommandTypes.PureCommand;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {

    }
}
