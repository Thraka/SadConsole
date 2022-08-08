using System;

namespace SadConsole.Effects;

// Not sure if I should make this serailizable...
/// <summary>
/// Effect that runs code for the apply and update actions of an effect.
/// </summary>
public class CodeEffect : CellEffectBase
{
    /// <summary>
    /// A user defined identifier of the effect.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// An object associated with this effect.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// When <see langword="true" />, uses the <see cref="Duration"/> timer to stop this effect; otherwise <see langword="false" />.
    /// </summary>
    public bool UseDuration { get; set; }

    /// <summary>
    /// The amount of time this effect runs for in seconds.
    /// </summary>
    public System.TimeSpan Duration { get; set; }

    private readonly Func<CodeEffect, ColoredGlyph, ColoredGlyphState, bool> _applyAction;
    private readonly Action<CodeEffect, System.TimeSpan> _updateAction;
    private readonly Action<CodeEffect> _restartAction;

    /// <summary>
    /// Creates a cell effect that runs custom code instead of hardcoded effect actions.
    /// </summary>
    /// <param name="id">A user-definable identifier.</param>
    /// <param name="apply">The code to run for <see cref="ICellEffect.ApplyToCell(ColoredGlyph, ColoredGlyphState)"/>.</param>
    /// <param name="update">The code to run for <see cref="ICellEffect.Update(TimeSpan)"/>.</param>
    /// <param name="restart">The code to run for <see cref="ICellEffect.Restart"/>.</param>
    public CodeEffect(string id, Func<CodeEffect, ColoredGlyph, ColoredGlyphState, bool> apply, Action<CodeEffect, System.TimeSpan> update, Action<CodeEffect> restart) =>
        (Id, _applyAction, _updateAction, _restartAction) = (id, apply, update, restart);

    /// <inheritdoc />
    public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState) =>
        _applyAction(this, cell, originalState);

    /// <summary>
    /// Not supported.
    /// </summary>
    public override ICellEffect Clone() => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Update(System.TimeSpan delta)
    {
        if (UseDuration)
        {
            _timeElapsed += delta;

            if (_timeElapsed >= Duration)
            {
                IsFinished = true;
            }
        }

        _updateAction(this, delta);
    }

    /// <inheritdoc />
    public override void Restart()
    {
        base.Restart();
        _restartAction(this);
    }

    /// <inheritdoc />
    public override string ToString() =>
        string.Format("CODE-{0}", Id);
}
