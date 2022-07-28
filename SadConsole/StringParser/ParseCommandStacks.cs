
using System.Collections.Generic;

namespace SadConsole.StringParser;
/// <summary>
/// A list of behaviors applied as a string is processed.
/// </summary>
public class ParseCommandStacks
{
    /// <summary>
    /// The commands that affect the foreground.
    /// </summary>
    public Stack<ParseCommandBase> Foreground { get; set; }

    /// <summary>
    /// The commands that affect the background.
    /// </summary>
    public Stack<ParseCommandBase> Background { get; set; }

    /// <summary>
    /// The commands that affect the glyph.
    /// </summary>
    public Stack<ParseCommandBase> Glyph { get; set; }

    /// <summary>
    /// The commands that affect the mirror setting.
    /// </summary>
    public Stack<ParseCommandBase> Mirror { get; set; }

    /// <summary>
    /// The commands that affect the glyph effect.
    /// </summary>
    public Stack<ParseCommandBase> Effect { get; set; }


    /// <summary>
    /// The commands that affect the glyph decorators.
    /// </summary>
    public Stack<ParseCommandBase> Decorator { get; set; }

    /// <summary>
    /// All commands.
    /// </summary>
    public Stack<ParseCommandBase> All { get; set; }

    /// <summary>
    /// Returns the active array that is calculated from the <see cref="Decorator"/> stack.
    /// </summary>
    public CellDecorator[] CalculatedDecorators { get; set; } = System.Array.Empty<CellDecorator>();

    /// <summary>
    /// Creates new stacks of commands.
    /// </summary>
    public ParseCommandStacks()
    {
        Foreground = new Stack<ParseCommandBase>(4);
        Background = new Stack<ParseCommandBase>(4);
        Glyph = new Stack<ParseCommandBase>(4);
        Mirror = new Stack<ParseCommandBase>(4);
        Effect = new Stack<ParseCommandBase>(4);
        Decorator = new Stack<ParseCommandBase>(4);
        All = new Stack<ParseCommandBase>(10);
    }

    /// <summary>
    /// Adds a behavior to the <see cref="All"/> collection and the collection 
    /// based on the <see cref="ParseCommandBase.CommandType"/> type.
    /// </summary>
    /// <param name="command"></param>
    public void AddSafe(ParseCommandBase command)
    {
        switch (command.CommandType)
        {
            case CommandTypes.Foreground:
                Foreground.Push(command);
                All.Push(command);
                break;
            case CommandTypes.Background:
                Background.Push(command);
                All.Push(command);
                break;
            case CommandTypes.Mirror:
                Mirror.Push(command);
                All.Push(command);
                break;
            case CommandTypes.Effect:
                Effect.Push(command);
                All.Push(command);
                break;
            case CommandTypes.Glyph:
                Glyph.Push(command);
                All.Push(command);
                break;
            case CommandTypes.Decorator:
                Decorator.Push(command);
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
            case CommandTypes.Foreground:
                if (Foreground.Count != 0)
                    commands = new List<ParseCommandBase>(Foreground);

                break;
            case CommandTypes.Background:
                if (Background.Count != 0)
                    commands = new List<ParseCommandBase>(Background);

                break;
            case CommandTypes.Mirror:
                if (Mirror.Count != 0)
                    commands = new List<ParseCommandBase>(Mirror);

                break;
            case CommandTypes.Effect:
                if (Effect.Count != 0)
                    commands = new List<ParseCommandBase>(Effect);

                break;
            case CommandTypes.Glyph:
                if (Glyph.Count != 0)
                    commands = new List<ParseCommandBase>(Glyph);

                break;
            case CommandTypes.Decorator:
                if (Decorator.Count != 0)
                    commands = new List<ParseCommandBase>(Decorator);

                break;
            default:
                return;
        }

        // If we have one, remove and restore stack
        if (commands != null && commands.Contains(command))
        {
            commands.Remove(command);
            commands.Reverse();

            switch (command.CommandType)
            {
                case CommandTypes.Foreground:
                    Foreground = new Stack<ParseCommandBase>(commands);
                    break;
                case CommandTypes.Background:
                    Background = new Stack<ParseCommandBase>(commands);
                    break;
                case CommandTypes.Mirror:
                    Mirror = new Stack<ParseCommandBase>(commands);
                    break;
                case CommandTypes.Effect:
                    Effect = new Stack<ParseCommandBase>(commands);
                    break;
                case CommandTypes.Glyph:
                    Glyph = new Stack<ParseCommandBase>(commands);
                    break;
                case CommandTypes.Decorator:
                    Decorator = new Stack<ParseCommandBase>(commands);
                    break;
                default:
                    return;
            }
        }

        var all = new List<ParseCommandBase>(All);
        if (all.Contains(command))
        {
            all.Remove(command);
            all.Reverse();
            All = new Stack<ParseCommandBase>(all);
        }
    }
}
