#if XNA
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole.StringParser
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Sets the mirror of a glyph.
    /// </summary>
    public sealed class ParseCommandMirror : ParseCommandBase
    {
        public SpriteEffects Mirror;
        public int Counter;

        public ParseCommandMirror(string parameters)
        {
            var badCommandException = new ArgumentException("command is invalid for mirror: " + parameters);

            string[] paramArray = parameters.Split(':');

            if (paramArray.Length == 2)
            {
                Counter = int.Parse(paramArray[1], CultureInfo.InvariantCulture);
            }
            else
            {
                Counter = -1;
            }

            if (Enum.TryParse(paramArray[0], out Mirror))
            {
                CommandType = CommandTypes.Mirror;
            }
            else
            {
                throw badCommandException;
            }
        }

        public ParseCommandMirror()
        {

        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex,
            CellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.Mirror = Mirror;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                {
                    commandStack.RemoveSafe(this);
                }
            }
        }
    }
}
