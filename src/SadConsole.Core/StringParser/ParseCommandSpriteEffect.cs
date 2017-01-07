using Microsoft.Xna.Framework.Graphics;

using SadConsole.Consoles;
using System;

namespace SadConsole.StringParser
{

    /// <summary>
    /// Sets the <see cref="SpriteEffects"/> of a glyph.
    /// </summary>
    public sealed class ParseCommandSpriteEffect : ParseCommandBase
    {
        public SpriteEffects Effect;
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

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ITextSurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
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
}
