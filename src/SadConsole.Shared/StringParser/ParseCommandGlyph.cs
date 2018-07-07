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
        public bool RandomGlyph;
        public int RandomGlyphMin;
        public int RandomGlyphMax;

        public ParseCommandSetGlyph(string parameters)
        {
            string[] parts = parameters.Split(new char[] { ':' }, 3);

            // Random glyph requested
            if (parts.Length == 3)
            {
                RandomGlyph = true;
                if (parts[0] == "*")
                    Counter = -1;
                else
                    Counter = int.Parse(parts[0]);

                RandomGlyphMin = int.Parse(parts[1]);
                RandomGlyphMax = int.Parse(parts[2]);
            }
            // Count and glyph type provided
            else if (parts.Length == 2)
            {
                if (parts[1] == "*")
                    Counter = -1;
                else
                    Counter = int.Parse(parts[1]);

                Glyph = (char)int.Parse(parts[0]);
            }
            else
            {
                Counter = 1;
                Glyph = (char)int.Parse(parts[0]);
            }


            // No exceptions, set the type
            CommandType = CommandTypes.Glyph;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            if (RandomGlyph)
                glyphState.GlyphCharacter = (char)SadConsole.Global.Random.Next(RandomGlyphMin, RandomGlyphMax);
            else
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
