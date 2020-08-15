using System.Globalization;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Clears the cell effect for the glyph.
    /// </summary>
    public sealed class ParseCommandClearEffect : ParseCommandBase
    {
        public int Counter;

        public ParseCommandClearEffect(string parameters, ParseCommandStacks commandStack)
        {
            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length == 1 && parametersArray[0] != "")
                Counter = int.Parse(parametersArray[0], CultureInfo.InvariantCulture);
            else
                Counter = -1;

            commandStack.TurnOnEffects = true;

            // No exceptions, set the type
            CommandType = CommandTypes.Effect;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex,
            CellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.Effect = null;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }
    }
}
