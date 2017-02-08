using Microsoft.Xna.Framework;
using SadConsole.Surfaces;
using System;

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
                Counter = int.Parse(parametersArray[2]);
            else
                Counter = -1;

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
                throw badCommandException;


        }

        public ParseCommandRecolor()
        {

        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            Color newColor;

            if (Default)
            {
                if (CommandType == CommandTypes.Background)
                    newColor = surface != null ? surface.DefaultBackground : Color.Transparent;
                else
                    newColor = surface != null ? surface.DefaultForeground : Color.White;
            }
            else
            {
                if (CommandType == CommandTypes.Background)
                    newColor = glyphState.Background;
                else
                    newColor = glyphState.Foreground;

                if (!KeepRed)
                    newColor.R = R;
                if (!KeepGreen)
                    newColor.G = G;
                if (!KeepBlue)
                    newColor.B = B;
                if (!KeepAlpha)
                    newColor.A = A;
            }

            if (CommandType == CommandTypes.Background)
                glyphState.Background = newColor;
            else
                glyphState.Foreground = newColor;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }
    }
}
