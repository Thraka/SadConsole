#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.StringParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Recolors a glyph.
    /// </summary>
    public sealed class ParseCommandGradient : ParseCommandBase
    {
        public ColoredString GradientString;
        public int Length;
        public int Counter;

        public ParseCommandGradient(string parameters)
        {

            var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length > 3)
            {
                CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
                Counter = Length = int.Parse(parametersArray[parametersArray.Length - 1], CultureInfo.InvariantCulture);


                List<Color> steps = new List<Color>();

                for (int i = 1; i < parametersArray.Length - 1; i++)
                {
                    steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));
                }

                GradientString = new ColorGradient(steps.ToArray()).ToColoredString(new string(' ', Length));
            }

            else
            {
                throw badCommandException;
            }
        }

        public ParseCommandGradient()
        {

        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex,
            CellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            if (CommandType == CommandTypes.Background)
            {
                glyphState.Background = GradientString[Length - Counter].Foreground;
            }
            else
            {
                glyphState.Foreground = GradientString[Length - Counter].Foreground;
            }

            Counter--;

            if (Counter == 0)
            {
                commandStack.RemoveSafe(this);
            }
        }
    }
}
