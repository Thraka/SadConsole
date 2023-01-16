using System.Runtime.Serialization;

namespace SadConsole.Effects;

/// <summary>
/// An effect that doesn't do anything except run the <see cref="CellEffectBase.StartDelay"/> timer. Usually used with the <see cref="EffectSet"/> effect.
/// </summary>
[DataContract]
public class Delay : CellEffectBase
{
    /// <inheritdoc />
    public override void Update(System.TimeSpan delta)
    {
        base.Update(delta);

        if (_delayFinished)
            IsFinished = true;
    }

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState state) => false;

    /// <inheritdoc />
    public override ICellEffect Clone() => new Delay()
    {
        IsFinished = IsFinished,
        StartDelay = StartDelay,
        CloneOnAdd = CloneOnAdd,
        RemoveOnFinished = RemoveOnFinished,
        RestoreCellOnRemoved = RestoreCellOnRemoved,
        RunEffectOnApply = RunEffectOnApply,
        _timeElapsed = _timeElapsed,
    };

    public override string ToString() =>
        string.Format("DELAY-{0}", StartDelay);
}
