using System.Globalization;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Clears the cell effect for the glyph.
    /// </summary>
    public sealed class ParseCommandClearEffect : ParseCommandBase
    {
        private int _counter;

        /// <summary>
        /// Creates a new instance of this command.
        /// </summary>
        /// <param name="parameters">The string to parse for parameters.</param>
        /// <param name="commandStack">The current commands for the string.</param>
        public ParseCommandClearEffect(string parameters, ParseCommandStacks commandStack)
        {
            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length == 1 && parametersArray[0] != "")
                _counter = int.Parse(parametersArray[0], CultureInfo.InvariantCulture);
            else
                _counter = -1;

            commandStack.TurnOnEffects = true;

            // No exceptions, set the type
            CommandType = CommandTypes.Effect;
        }

        /// <inheritdoc />
        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
            ICellSurface surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
        {
            glyphState.Effect = null;

            if (_counter != -1)
            {
                _counter--;

                if (_counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }
    }
}
