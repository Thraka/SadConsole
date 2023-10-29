using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Effects;

/// <summary>
/// Blinks the foreground and background colors of a cell with the specified colors.
/// </summary>
[DataContract]
public class Blinker : CellEffectBase
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
    /// The color the foreground blinks to.
    /// </summary>
    [DataMember]
    public Color BlinkOutForegroundColor { get; set; }

    /// <summary>
    /// The color the background blinks to.
    /// </summary>
    [DataMember]
    public Color BlinkOutBackgroundColor { get; set; }

    /// <summary>
    /// When <see langword="true"/>, ignores the <see cref="BlinkOutBackgroundColor"/> and <see cref="BlinkOutForegroundColor"/> colors and instead swaps the glyph's foreground and background colors.
    /// </summary>
    [DataMember]
    public bool SwapColorsFromCell { get; set; }

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
    public Blinker()
    {
        Duration = System.TimeSpan.MaxValue;
        BlinkCount = -1;
        BlinkSpeed = System.TimeSpan.FromSeconds(1);
        BlinkOutBackgroundColor = Color.Transparent;
        BlinkOutForegroundColor = Color.Transparent;
        _isOn = true;
        _blinkCounter = 0;
    }

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyphBase cell, ColoredGlyphBase originalState)
    {
        Color oldColor = cell.Foreground;

        if (!_isOn)
        {
            if (SwapColorsFromCell)
            {
                cell.Foreground = originalState.Background;
                cell.Background = originalState.Foreground;
            }
            else
            {
                cell.Foreground = BlinkOutForegroundColor;
                cell.Background = BlinkOutBackgroundColor;
            }
        }
        else
        {
            cell.Foreground = originalState.Foreground;
            cell.Background = originalState.Background;
        }

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
    public override ICellEffect Clone() => new Blinker()
    {
        BlinkOutBackgroundColor = BlinkOutBackgroundColor,
        BlinkOutForegroundColor = BlinkOutForegroundColor,
        BlinkSpeed = BlinkSpeed,
        _isOn = _isOn,
        SwapColorsFromCell = SwapColorsFromCell,
        BlinkCount = BlinkCount,

        IsFinished = IsFinished,
        StartDelay = StartDelay,
        CloneOnAdd = CloneOnAdd,
        RemoveOnFinished = RemoveOnFinished,
        RestoreCellOnRemoved = RestoreCellOnRemoved,
        RunEffectOnApply = RunEffectOnApply,
        _timeElapsed = _timeElapsed,
    };

    /// <inheritdoc />
    public override string ToString() => $"BLINKER-{BlinkOutBackgroundColor.PackedValue}-{BlinkOutForegroundColor.PackedValue}-{BlinkSpeed}-{SwapColorsFromCell}-{StartDelay}";
}
