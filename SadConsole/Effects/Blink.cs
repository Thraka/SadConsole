using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Effects;

/// <summary>
/// Switches between the normal foreground of a cell and a specified color for an amount of time, and then repeats.
/// </summary>
[DataContract]
public class Blink : CellEffectBase
{
    private int _blinkCounter = 0;
    private bool _isOn;
    private System.TimeSpan _duration = System.TimeSpan.Zero;

    /// <summary>
    /// How long it takes to transition from blinking in and out.
    /// </summary>
    [DataMember]
    public System.TimeSpan BlinkSpeed { get; set; }

    /// <summary>
    /// When true, uses the current cells background color for fading instead of the value of <see cref="BlinkOutColor"/>.
    /// </summary>
    [DataMember]
    public bool UseCellBackgroundColor { get; set; }

    /// <summary>
    /// The color the foreground blinks to.
    /// </summary>
    [DataMember]
    public Color BlinkOutColor { get; set; }

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
    /// Creates a new instance of the blink effect.
    /// </summary>
    public Blink()
    {
        Duration = System.TimeSpan.MaxValue;
        BlinkCount = -1;
        BlinkSpeed = System.TimeSpan.FromSeconds(1);
        UseCellBackgroundColor = true;
        BlinkOutColor = Color.Transparent;
        _isOn = true;
        _blinkCounter = 0;
    }

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyphBase cell, ColoredGlyphBase originalState)
    {
        Color oldColor = cell.Foreground;

        if (!_isOn)
        {
            if (UseCellBackgroundColor)
                cell.Foreground = originalState.Background;
            else
                cell.Foreground = BlinkOutColor;
        }
        else
            cell.Foreground = originalState.Foreground;

        return cell.Foreground != oldColor;
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
    public override ICellEffect Clone() => new Blink()
    {
        BlinkOutColor = BlinkOutColor,
        BlinkSpeed = BlinkSpeed,
        _isOn = _isOn,
        UseCellBackgroundColor = UseCellBackgroundColor,
        BlinkCount = BlinkCount,

        IsFinished = IsFinished,
        StartDelay = StartDelay,
        CloneOnAdd = CloneOnAdd,
        RemoveOnFinished = RemoveOnFinished,
        RestoreCellOnRemoved = RestoreCellOnRemoved,
        RunEffectOnApply = RunEffectOnApply,
        _timeElapsed = _timeElapsed,
    };

    //public override bool Equals(ICellEffect effect)
    //{

    //    if (effect is Blink)
    //    {
    //        if (base.Equals(effect))
    //        {
    //            var effect2 = (Blink)effect;

    //            return BlinkOutColor == effect2.BlinkOutColor &&
    //                   BlinkSpeed == effect2.BlinkSpeed &&
    //                   UseCellBackgroundColor == effect2.UseCellBackgroundColor &&
    //                   StartDelay == effect2.StartDelay &&
    //                   BlinkCount == effect2.BlinkCount;
    //        }
    //    }

    //    return false;
    //}

    /// <inheritdoc />
    public override string ToString() => $"BLINK-{BlinkOutColor.PackedValue}-{BlinkSpeed}-{UseCellBackgroundColor}-{StartDelay}-{BlinkCount}";
}
