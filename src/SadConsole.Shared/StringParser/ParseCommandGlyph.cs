using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Prints a glyph.
    /// </summary>
    class ParseCommandSetGlyph : ParseCommandBase
    {
        public int Counter;
        public char Glyph;

        public ParseCommandSetGlyph(string parameters)
        {
            string[] parts = parameters.Split(new char[] { ':' }, 2);

            // Count and glyph type provided
            if (parts.Length == 2)
            {
                if (parts[1] == "*")
                    Counter = -1;
                else
                    Counter = int.Parse(parts[1]);
            }
            else
                Counter = 1;

            // Get character
            Glyph = (char)int.Parse(parts[0]);

            // No exceptions, set the type
            CommandType = CommandTypes.Glyph;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.GlyphCharacter = Glyph;

            if (Counter != -1)
            {
                Counter--;
            }

            if (Counter == 0)
                commandStack.RemoveSafe(this);
        }
    }
}
