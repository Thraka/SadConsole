using System;
using System.Collections.Generic;
using System.Globalization;
using SadRogue.Primitives;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Recolors a glyph.
    /// </summary>
    public sealed class ParseCommandGradient : ParseCommandBase
    {
        private int _counter;

        /// <summary>
        /// The string to apply to the characters.
        /// </summary>
        public ColoredString GradientString;

        /// <summary>
        /// The length to apply.
        /// </summary>
        public int Length;

        /// <summary>
        /// Creates a new instance of this command.
        /// </summary>
        /// <param name="parameters">The string to parse for parameters.</param>
        public ParseCommandGradient(string parameters)
        {

            var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length > 3)
            {
                CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
                _counter = Length = int.Parse(parametersArray[parametersArray.Length - 1], CultureInfo.InvariantCulture);


                List<Color> steps = new List<Color>();

                for (int i = 1; i < parametersArray.Length - 1; i++)
                    steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));

                GradientString = new Gradient(steps.ToArray()).ToColoredString(new string(' ', Length));
            }

            else
                throw badCommandException;
        }

        /// <summary>
        /// Creates a new instance of this command.
        /// </summary>
        public ParseCommandGradient()
        {

        }

        /// <inheritdoc />
        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
            ICellSurface surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
        {
            if (CommandType == CommandTypes.Background)
                glyphState.Background = GradientString[Length - _counter].Foreground;
            else
                glyphState.Foreground = GradientString[Length - _counter].Foreground;

            _counter--;

            if (_counter == 0)
                commandStack.RemoveSafe(this);
        }
    }
}
