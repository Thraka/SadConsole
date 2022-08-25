
using System.Collections.Generic;
using System.Globalization;

namespace SadConsole.StringParser;
/// <summary>
/// Blinks characters.
/// </summary>
public sealed class ParseCommandBlink : ParseCommandBase
{
    private int _counter;

    /// <summary>
    /// The speed of the blink.
    /// </summary>
    public System.TimeSpan Speed = System.TimeSpan.FromSeconds(0.35d);

    private readonly CustomBlinkEffect _blinkEffect;

    /// <summary>
    /// Creates a new instance of this command.
    /// </summary>
    /// <param name="parameters">The string to parse for parameters.</param>
    /// <param name="glyphString">The string that has been processed so far.</param>
    /// <param name="commandStack">The current commands for the string.</param>
    /// <param name="surface">The surface hosting the string.</param>
    public ParseCommandBlink(string parameters, ColoredGlyph[] glyphString, ParseCommandStacks commandStack, ICellSurface? surface)
    {
        string[] parametersArray = parameters.Split(':');

        if (parametersArray.Length == 2)
            Speed = System.TimeSpan.FromSeconds(double.Parse(parametersArray[1], CultureInfo.InvariantCulture));

        if (parametersArray.Length >= 1 && parametersArray[0] != "")
            _counter = int.Parse(parametersArray[0], CultureInfo.InvariantCulture);
        else
            _counter = -1;

        // Try sync with surface editor
        if (surface != null)
        {
            IEnumerable<Effects.ICellEffect>? effects = surface.Effects.GetEffects();
            if (effects != null)
            {
                var existingBlinks = new List<Effects.ICellEffect>(effects);

                foreach (Effects.ICellEffect item in existingBlinks)
                {
                    if (item is CustomBlinkEffect)
                    {
                        if (Speed == ((CustomBlinkEffect)item).BlinkSpeed)
                            _blinkEffect = (CustomBlinkEffect)item;

                        break;
                    }
                }
            }
        }

        // Failed, look within this parse for existing
        if (_blinkEffect == null)
        {
            foreach (ColoredGlyphAndEffect item in glyphString)
            {
                if (item.Effect != null && item.Effect is CustomBlinkEffect)
                {
                    if (Speed == ((CustomBlinkEffect)item.Effect).BlinkSpeed)
                        _blinkEffect = (CustomBlinkEffect)item.Effect;
                }
            }
        }


        if (_blinkEffect == null)
            _blinkEffect = new CustomBlinkEffect() { BlinkSpeed = Speed };

        // No exceptions, set the type
        CommandType = CommandTypes.Effect;
    }

    /// <inheritdoc />
    public override void Build(ref ColoredGlyphAndEffect glyphState, ColoredGlyphAndEffect[] glyphString, int surfaceIndex,
        ICellSurface? surface, ref int stringIndex, System.ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
    {
        glyphState.Effect = _blinkEffect;

        if (_counter != -1)
        {
            _counter--;

            if (_counter == 0)
                commandStack.RemoveSafe(this);
        }
    }

    private class CustomBlinkEffect : Effects.Blink
    {

    }
}
