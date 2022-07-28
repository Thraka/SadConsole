using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Effects;

/// <summary>
/// Switches between the glyph of a cell and a specified glyph for an amount of time, and then repeats.
/// </summary>
[DataContract]
public class BlinkGlyph : CellEffectBase
{
    private bool _isOn;
    private int _blinkCounter = 0;
    private System.TimeSpan _duration = System.TimeSpan.Zero;

    /// <summary>
    /// In seconds, how fast the fade in and fade out each are
    /// </summary>
    [DataMember]
    public System.TimeSpan BlinkSpeed { get; set; }

    /// <summary>
    /// The glyph index to blink into.
    /// </summary>
    [DataMember]
    public int GlyphIndex { get; set; }

    /// <summary>
    /// How many times to blink. The value of -1 represents forever.
    /// </summary>
    [DataMember]
    public int BlinkCount { get; set; }

    /// <summary>
    /// The total duraction this effect will run for, before being flagged as finished. <see cref="System.TimeSpan.MaxValue"/> represents forever.
    /// </summary>
    [DataMember]
    public System.TimeSpan Duration { get; set; }

    /// <summary>
    /// Creates an instance of the blink glyph effect.
    /// </summary>
    public BlinkGlyph()
    {
        Duration = System.TimeSpan.MaxValue;
        BlinkCount = -1;
        BlinkSpeed = System.TimeSpan.FromSeconds(1);
        GlyphIndex = 0;
        _isOn = true;
    }

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
    {
        int oldGlyph = cell.Glyph;

        if (!_isOn)
            cell.Glyph = GlyphIndex;
        else
            cell.Glyph = originalState.Glyph;

        return cell.Glyph != oldGlyph;
    }

    /// <inheritdoc />
    public override void Update(System.TimeSpan delta)
    {
        base.Update(delta);

        if (_delayFinished && !IsFinished)
        {
            if (Duration != System.TimeSpan.MaxValue)
            {
                _duration += delta;
                if (_duration >= Duration)
                {
                    IsFinished = true;
                    return;
                }
            }

            if (_timeElapsed >= BlinkSpeed)
            {
                _isOn = !_isOn;
                _timeElapsed = System.TimeSpan.Zero;

                if (BlinkCount != -1)
                {
                    _blinkCounter += 1;

                    if (BlinkCount != -1 && _blinkCounter > (BlinkCount * 2))
                        IsFinished = true;
                }
            }

            if (_timeElapsed >= BlinkSpeed)
            {
                _isOn = !_isOn;
                _timeElapsed = System.TimeSpan.Zero;
            }
        }
    }

    /// <summary>
    /// Restarts the cell effect but does not reset it.
    /// </summary>
    public override void Restart()
    {
        _isOn = true;
        _blinkCounter = 0;
        _duration = System.TimeSpan.Zero;

        base.Restart();
    }

    /// <inheritdoc />
    public override ICellEffect Clone() => new BlinkGlyph()
    {
        _isOn = _isOn,
        BlinkSpeed = BlinkSpeed,
        GlyphIndex = GlyphIndex,

        IsFinished = IsFinished,
        StartDelay = StartDelay,
        CloneOnAdd = CloneOnAdd,
        RemoveOnFinished = RemoveOnFinished,
        RestoreCellOnRemoved = RestoreCellOnRemoved,
        _timeElapsed = _timeElapsed,
    };

    //public override bool Equals(ICellEffect effect)
    //{
    //    if (effect is BlinkGlyph)
    //    {
    //        if (base.Equals(effect))
    //        {
    //            var effect2 = (BlinkGlyph)effect;

    //            return GlyphIndex == effect2.GlyphIndex &&
    //                   BlinkSpeed == effect2.BlinkSpeed &&
    //                   RemoveOnFinished == effect2.RemoveOnFinished &&
    //                   StartDelay == effect2.StartDelay;
    //        }
    //    }

    //    return false;
    //}

    /// <inheritdoc />
    public override string ToString() =>
        string.Format("BLINKCHAR-{0}-{1}-{2}-{3}", GlyphIndex, BlinkSpeed, StartDelay, RemoveOnFinished);
}
