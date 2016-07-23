using Microsoft.Xna.Framework;
using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public partial class ColoredString
    {
        /// <summary>
        /// Custom processor called if any built in command is not triggerd. Signature is ("command", "sub command", text surface, command stacks).
        /// </summary>
        public static Func<string, string, ITextSurface, ParseCommandStacks, ParseCommandBase> CustomProcessor;

        /// <summary>
        /// Creates a colored string by parsing commands embedded in the string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="surfaceIndex">Index of where this string will be printed.</param>
        /// <param name="surface">The surface the string will be printed to.</param>
        /// <param name="initialBehaviors">Any initial defaults.</param>
        /// <returns></returns>
        public static ColoredString Parse(string value, int surfaceIndex = -1, Consoles.ITextSurface surface = null, ParseCommandStacks initialBehaviors = null)
        {
            var commandStacks = initialBehaviors != null ? initialBehaviors : new ParseCommandStacks();
            List<ColoredGlyph> glyphs = new List<ColoredGlyph>(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '`' && i + 1 < value.Length && value[i + 1] == '[')
                    continue;

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
                                var commandSections = command.Split(new char[] { ' ' }, 2);
                                command = commandSections[0].ToLower();
                                commandParams = commandSections[1];
                            }

                            // Check for custom command
                            ParseCommandBase commandObject = CustomProcessor != null ? CustomProcessor(command, commandParams, surface, commandStacks) : null;

                            // No custom command found, run build in ones
                            if (commandObject == null)
                                switch (command)
                                {
                                    case "recolor":
                                    case "r":
                                        commandObject = new ParseCommandRecolor(commandParams);
                                        break;
                                    case "mirror":
                                    case "m":
                                        commandObject = new ParseCommandSpriteEffect(commandParams);
                                        break;
                                    case "undo":
                                    case "u":
                                        commandObject = new ParseCommandUndo(commandParams, commandStacks);
                                        break;
                                    default:
                                        break;
                                }

                            if (commandObject != null && commandObject.CommandType != ParseCommandBase.CommandTypes.Invalid)
                            {
                                commandStacks.AddSafe(commandObject);

                                i = commandExitIndex;
                                continue;
                            }
                        }
                        
                    }
                    catch (System.Exception)
                    {
                        // bad parsing, just skip it then
                    }
                }

                int fixedSurfaceIndex;

                if (surfaceIndex == -1 || surface == null)
                    fixedSurfaceIndex = -1;
                else
                    fixedSurfaceIndex = i + surfaceIndex < surface.Cells.Length ? i + surfaceIndex : -1;


                ColoredGlyph newGlyph;

                if (fixedSurfaceIndex != -1)
                    newGlyph = new ColoredGlyph(surface[i + surfaceIndex]) { Glyph = value[i] };
                else
                    newGlyph = new ColoredGlyph(new Cell()) { Glyph = value[i] };

                // Foreground
                if (commandStacks.Foreground.Count != 0)
                    commandStacks.Foreground.Peek().Build(ref newGlyph, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                // Background
                if (commandStacks.Background.Count != 0)
                    commandStacks.Background.Peek().Build(ref newGlyph, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                if (commandStacks.Glyph.Count != 0)
                    commandStacks.Glyph.Peek().Build(ref newGlyph, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                // SpriteEffect
                if (commandStacks.SpriteEffect.Count != 0)
                    commandStacks.SpriteEffect.Peek().Build(ref newGlyph, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                // Effect
                if (commandStacks.Effect.Count != 0)
                    commandStacks.Effect.Peek().Build(ref newGlyph, fixedSurfaceIndex, surface, ref i, value, commandStacks);

                glyphs.Add(newGlyph);
            }

            return new ColoredString(glyphs.ToArray());
        }

        /// <summary>
        /// A list of behaviors applied as a string is processed.
        /// </summary>
        public class ParseCommandStacks
        {
            public Stack<ParseCommandBase> Foreground;
            public Stack<ParseCommandBase> Background;
            public Stack<ParseCommandBase> Glyph;
            public Stack<ParseCommandBase> SpriteEffect;
            public Stack<ParseCommandBase> Effect;
            public Stack<ParseCommandBase> All;

            public ParseCommandStacks()
            {
                Foreground = new Stack<ParseCommandBase>(4);
                Background = new Stack<ParseCommandBase>(4);
                Glyph = new Stack<ParseCommandBase>(4);
                SpriteEffect = new Stack<ParseCommandBase>(4);
                Effect = new Stack<ParseCommandBase>(4);
                All = new Stack<ParseCommandBase>(10);
            }

            /// <summary>
            /// Adds a behavior to the <see cref="All"/> collection and the collection based on the <see cref="ParseCommandBase.CommandType"/> type.
            /// </summary>
            /// <param name="command"></param>
            public void AddSafe(ParseCommandBase command)
            {
                switch (command.CommandType)
                {
                    case ParseCommandBase.CommandTypes.Foreground:
                        Foreground.Push(command);
                        All.Push(command);
                        break;
                    case ParseCommandBase.CommandTypes.Background:
                        Background.Push(command);
                        All.Push(command);
                        break;
                    case ParseCommandBase.CommandTypes.SpriteEffect:
                        SpriteEffect.Push(command);
                        All.Push(command);
                        break;
                    case ParseCommandBase.CommandTypes.Effect:
                        Effect.Push(command);
                        All.Push(command);
                        break;
                    case ParseCommandBase.CommandTypes.Glyph:
                        Glyph.Push(command);
                        All.Push(command);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// Removes a command from the appropriate command stack and from the <see cref="All"/> stack.
            /// </summary>
            /// <param name="command">The command to remove</param>
            public void RemoveSafe(ParseCommandBase command)
            {
                List<ParseCommandBase> commands = null;

                // Get the stack we need to remove from
                switch (command.CommandType)
                {
                    case ParseCommandBase.CommandTypes.Foreground:
                        if (Foreground.Count != 0)
                            commands = new List<ParseCommandBase>(Foreground);
                        break;
                    case ParseCommandBase.CommandTypes.Background:
                        if (Background.Count != 0)
                            commands = new List<ParseCommandBase>(Background);
                        break;
                    case ParseCommandBase.CommandTypes.SpriteEffect:
                        if (SpriteEffect.Count != 0)
                            commands = new List<ParseCommandBase>(SpriteEffect);
                        break;
                    case ParseCommandBase.CommandTypes.Effect:
                        if (Effect.Count != 0)
                            commands = new List<ParseCommandBase>(Effect);
                        break;
                    case ParseCommandBase.CommandTypes.Glyph:
                        if (Glyph.Count != 0)
                            commands = new List<ParseCommandBase>(Glyph);
                        break;
                    default:
                        return;
                }

                // If we have one, remove and restore stack
                if (commands != null && commands.Contains(command))
                {
                    commands.Remove(command);

                    switch (command.CommandType)
                    {
                        case ParseCommandBase.CommandTypes.Foreground:
                            Foreground = new Stack<ParseCommandBase>(commands);
                            break;
                        case ParseCommandBase.CommandTypes.Background:
                            Background = new Stack<ParseCommandBase>(commands);
                            break;
                        case ParseCommandBase.CommandTypes.SpriteEffect:
                            SpriteEffect = new Stack<ParseCommandBase>(commands);
                            break;
                        case ParseCommandBase.CommandTypes.Effect:
                            Effect = new Stack<ParseCommandBase>(commands);
                            break;
                        case ParseCommandBase.CommandTypes.Glyph:
                            Glyph = new Stack<ParseCommandBase>(commands);
                            break;
                        default:
                            return;
                    }
                }

                List<ParseCommandBase> all = new List<ParseCommandBase>(All);

                if (all.Contains(command))
                {
                    all.Remove(command);
                    All = new Stack<ParseCommandBase>(all);
                }
            }
        }
        
        /// <summary>
        /// Recolors a glyph.
        /// </summary>
        public sealed class ParseCommandRecolor : ParseCommandBase
        {
            public bool Default;
            public bool KeepRed;
            public bool KeepGreen;
            public bool KeepBlue;
            public bool KeepAlpha;

            public byte R;
            public byte G;
            public byte B;
            public byte A;

            public int Counter;

            public ParseCommandRecolor(string parameters)
            {
                var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

                string[] parametersArray = parameters.Split(':');

                if (parametersArray.Length == 3)
                    Counter = int.Parse(parametersArray[2]);
                else
                    Counter = -1;

                if (parametersArray.Length >= 2)
                {
                    CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
                    string colorString = parametersArray[1];

                    if (colorString.Contains(","))
                    {
                        string[] channels = colorString.Trim(' ').Split(',');

                        if (channels.Length >= 3)
                        {

                            byte colorValue;

                            // Red
                            if (channels[0] == "x")
                                KeepRed = true;
                            else if (byte.TryParse(channels[0], out colorValue))
                                R = colorValue;
                            else
                                throw badCommandException;

                            // Green
                            if (channels[1] == "x")
                                KeepGreen = true;
                            else if (byte.TryParse(channels[1], out colorValue))
                                G = colorValue;
                            else
                                throw badCommandException;

                            // Blue
                            if (channels[2] == "x")
                                KeepBlue = true;
                            else if (byte.TryParse(channels[2], out colorValue))
                                B = colorValue;
                            else
                                throw badCommandException;

                            if (channels.Length == 4)
                            {
                                // Alpha
                                if (channels[3] == "x")
                                    KeepAlpha = true;
                                else if (byte.TryParse(channels[3], out colorValue))
                                    A = colorValue;
                                else
                                    throw badCommandException;
                            }
                            else
                                A = 255;
                        }
                    }
                    else if (colorString == "default")
                    {
                        Default = true;
                    }
                    else
                    {
                        // Lookup color in framework
                        Color testColor = Color.AliceBlue;
                        Type colorType = testColor.GetType();
                        if (null != colorType)
                        {
                            System.Reflection.PropertyInfo[] propInfoList =
                             colorType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly
                                | System.Reflection.BindingFlags.Public);
                            int nNumProps = propInfoList.Length;

                            bool found = false;

                            for (int i = 0; i < nNumProps; i++)
                            {
                                if (propInfoList[i].Name.ToLower() == colorString)
                                {
                                    Color color = (Color)propInfoList[i].GetValue(null, null);
                                    R = color.R;
                                    G = color.G;
                                    B = color.B;
                                    A = color.A;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                CommandType = CommandTypes.Invalid;
                        }

                    }
                }
                else
                    throw badCommandException;


            }

            public ParseCommandRecolor()
            {

            }

            public override void Build(ref ColoredGlyph glyphState, int surfaceIndex, ITextSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
            {
                Color newColor;

                if (Default)
                {
                    if (CommandType == CommandTypes.Background)
                        newColor = surface != null ? surface.DefaultBackground : Color.Transparent;
                    else
                        newColor = surface != null ? surface.DefaultForeground : Color.White;
                }
                else
                {
                    if (CommandType == CommandTypes.Background)
                        newColor = glyphState.Background;
                    else
                        newColor = glyphState.Foreground;

                    if (!KeepRed)
                        newColor.R = R;
                    if (!KeepGreen)
                        newColor.G = G;
                    if (!KeepBlue)
                        newColor.B = B;
                    if (!KeepAlpha)
                        newColor.A = A;
                }

                if (CommandType == CommandTypes.Background)
                    glyphState.Background = newColor;
                else
                    glyphState.Foreground = newColor;

                if (Counter != -1)
                {
                    Counter--;

                    if (Counter == 0)
                        commandStack.RemoveSafe(this);
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="Microsoft.Xna.Framework.Graphics.SpriteEffects"/> of a glyph.
        /// </summary>
        public sealed class ParseCommandSpriteEffect : ParseCommandBase
        {
            public Microsoft.Xna.Framework.Graphics.SpriteEffects Effect;
            public int Counter;

            public ParseCommandSpriteEffect(string parameters)
            {
                var badCommandException = new ArgumentException("command is invalid for SpriteEffect: " + parameters);

                string[] paramArray = parameters.Split(':');

                if (paramArray.Length == 2)
                    Counter = int.Parse(paramArray[1]);
                else
                    Counter = -1;

                if (Enum.TryParse(paramArray[0], out Effect))
                    CommandType = CommandTypes.SpriteEffect;
                else
                    throw badCommandException;
            }

            public ParseCommandSpriteEffect()
            {

            }

            public override void Build(ref ColoredGlyph glyphState, int surfaceIndex, ITextSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
            {
                glyphState.SpriteEffect = Effect;

                if (Counter != -1)
                {
                    Counter--;

                    if (Counter == 0)
                        commandStack.RemoveSafe(this);
                }
            }
        }

        /// <summary>
        /// Pops a behavior off of a <see cref="ParseCommandStacks"/>.
        /// </summary>
        public sealed class ParseCommandUndo : ParseCommandBase
        {
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
                            stackType = CommandTypes.SpriteEffect;
                            break;
                        case "a":
                            isSpecificStack = false;
                            break;
                        default:
                            throw badCommandException;
                    }
                }

                if (parts.Length >= 1 && parts[0] != "")
                    times = int.Parse(parts[0]);


                for (int i = 0; i < times; i++)
                {
                    ParseCommandBase behavior = null;

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
                                case CommandTypes.SpriteEffect:
                                    stacks.SpriteEffect.Pop();
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
                            case CommandTypes.SpriteEffect:
                                if (stacks.SpriteEffect.Count != 0)
                                    behavior = stacks.SpriteEffect.Pop();
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

            public override void Build(ref ColoredGlyph glyphState, int surfaceIndex, ITextSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
            {
                
            }
        }

        /// <summary>
        /// Base class for a string processor behavior.
        /// </summary>
        public abstract class ParseCommandBase
        {
            public enum CommandTypes
            {
                Foreground,
                Background,
                Glyph,
                SpriteEffect,
                Effect,
                PureCommand,
                Invalid
            }

            public CommandTypes CommandType = CommandTypes.Invalid;


            public abstract void Build(ref ColoredGlyph glyphState, int surfaceIndex, ITextSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack);
        }
    }
}
