using System;
using System.Globalization;
using SadRogue.Primitives;

namespace SadConsole.StringParser
{
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
            {
                Counter = int.Parse(parametersArray[2], CultureInfo.InvariantCulture);
            }
            else
            {
                Counter = -1;
            }

            if (parametersArray.Length >= 2)
            {
                CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
                Color color = Color.White.FromParser(parametersArray[1], out KeepRed, out KeepGreen, out KeepBlue, out KeepAlpha, out Default);

                R = color.R;
                G = color.G;
                B = color.B;
                A = color.A;
            }
            else
            {
                throw badCommandException;
            }
        }

        public ParseCommandRecolor()
        {

        }

        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
            ICellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            byte a = 0;

            if (Default)
            {
                if (CommandType == CommandTypes.Background)
                {
                    (surface != null ? surface.DefaultBackground : Color.Transparent).Deconstruct(out r, out g, out b, out a);
                }
                else
                {
                    (surface != null ? surface.DefaultForeground : Color.White).Deconstruct(out r, out g, out b, out a);
                }
            }
            else
            {
                if (CommandType == CommandTypes.Background)
                {
                    glyphState.Background.Deconstruct(out r, out g, out b, out a);
                }
                else
                {
                    glyphState.Foreground.Deconstruct(out r, out g, out b, out a);
                }

                if (!KeepRed)
                {
                    r = R;
                }

                if (!KeepGreen)
                {
                    g = G;
                }

                if (!KeepBlue)
                {
                    b = B;
                }

                if (!KeepAlpha)
                {
                    a = A;
                }
            }

            if (CommandType == CommandTypes.Background)
            {
                glyphState.Background = new Color(r, g, b, a);
            }
            else
            {
                glyphState.Foreground = new Color(r, g, b, a);
            }

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
