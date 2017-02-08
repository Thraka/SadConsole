using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.StringParser
{
    /// <summary>
    /// Blinks characters.
    /// </summary>
    public sealed class ParseCommandBlink : ParseCommandBase
    {
        public int Counter;
        public double Speed = 0.35d;
        CustomBlinkEffect BlinkEffect;

        public ParseCommandBlink(string parameters, ColoredGlyph[] glyphString, ParseCommandStacks commandStack, SurfaceEditor surfaceEditor)
        {
            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length == 2)
                Speed = double.Parse(parametersArray[1]);

            if (parametersArray.Length >= 1 && parametersArray[0] != "")
                Counter = int.Parse(parametersArray[0]);
            else
                Counter = -1;

            // Try sync with surface editor
            if (surfaceEditor != null)
            {
                var effects = surfaceEditor.Effects.GetEffects();
                if (effects != null)
                {
                    var existingBlinks = new List<SadConsole.Effects.ICellEffect>(effects);

                    foreach (var item in existingBlinks)
                    {
                        if (item is CustomBlinkEffect)
                        {
                            if (Speed == ((CustomBlinkEffect)item).BlinkSpeed)
                                BlinkEffect = (CustomBlinkEffect)item;

                            break;
                        }
                    }
                }
            }

            // Failed, look within this parse for existing
            if (BlinkEffect == null)
            {
                foreach (var item in glyphString)
                {
                    if (item.Effect != null && item.Effect is CustomBlinkEffect)
                        if (Speed == ((CustomBlinkEffect)item.Effect).BlinkSpeed)
                            BlinkEffect = (CustomBlinkEffect)item.Effect;
                }
            }


            if (BlinkEffect == null)
                BlinkEffect = new CustomBlinkEffect() { BlinkSpeed = Speed };

            commandStack.TurnOnEffects = true;

            // No exceptions, set the type
            CommandType = CommandTypes.Effect;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.Effect = BlinkEffect;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                    commandStack.RemoveSafe(this);
            }
        }

        private class CustomBlinkEffect : SadConsole.Effects.Blink
        {

        }
    }
}
