namespace SadConsole.StringParser
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Blinks characters.
    /// </summary>
    public sealed class ParseCommandBlink : ParseCommandBase
    {
        public int Counter;
        public double Speed = 0.35d;

        private readonly CustomBlinkEffect BlinkEffect;

        public ParseCommandBlink(string parameters, ColoredGlyph[] glyphString, ParseCommandStacks commandStack, CellSurface surfaceEditor)
        {
            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length == 2)
            {
                Speed = double.Parse(parametersArray[1], CultureInfo.InvariantCulture);
            }

            if (parametersArray.Length >= 1 && parametersArray[0] != "")
            {
                Counter = int.Parse(parametersArray[0], CultureInfo.InvariantCulture);
            }
            else
            {
                Counter = -1;
            }

            // Try sync with surface editor
            if (surfaceEditor != null)
            {
                IEnumerable<Effects.ICellEffect> effects = surfaceEditor.Effects.GetEffects();
                if (effects != null)
                {
                    var existingBlinks = new List<Effects.ICellEffect>(effects);

                    foreach (Effects.ICellEffect item in existingBlinks)
                    {
                        if (item is CustomBlinkEffect)
                        {
                            if (Speed == ((CustomBlinkEffect)item).BlinkSpeed)
                            {
                                BlinkEffect = (CustomBlinkEffect)item;
                            }

                            break;
                        }
                    }
                }
            }

            // Failed, look within this parse for existing
            if (BlinkEffect == null)
            {
                foreach (ColoredGlyph item in glyphString)
                {
                    if (item.Effect != null && item.Effect is CustomBlinkEffect)
                    {
                        if (Speed == ((CustomBlinkEffect)item.Effect).BlinkSpeed)
                        {
                            BlinkEffect = (CustomBlinkEffect)item.Effect;
                        }
                    }
                }
            }


            if (BlinkEffect == null)
            {
                BlinkEffect = new CustomBlinkEffect() { BlinkSpeed = Speed };
            }

            commandStack.TurnOnEffects = true;

            // No exceptions, set the type
            CommandType = CommandTypes.Effect;
        }

        public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex,
            CellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {
            glyphState.Effect = BlinkEffect;

            if (Counter != -1)
            {
                Counter--;

                if (Counter == 0)
                {
                    commandStack.RemoveSafe(this);
                }
            }
        }

        private class CustomBlinkEffect : Effects.Blink
        {

        }
    }
}
