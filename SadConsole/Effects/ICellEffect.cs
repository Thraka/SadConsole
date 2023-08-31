namespace SadConsole.Effects;

/// <summary>
/// The interface describing a cell effect
/// </summary>
public interface ICellEffect// : System.IEquatable<ICellEffect>
{
    /// <summary>
    /// True when the effect is finished.
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    /// Flags this effect to be cloned when assigned to a cell instead of reused.
    /// </summary>
    bool CloneOnAdd { get; set; }

    /// <summary>
    /// A delay applied to the effect only when it first runs or is restarted.
    /// </summary>
    System.TimeSpan StartDelay { get; set; }

    /// <summary>
    /// When true, the effect should be disassociated with cells when it has finished processing.
    /// </summary>
    bool RemoveOnFinished { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates the <see cref="EffectsManager"/> should restore the cell to its original state.
    /// </summary>
    bool RestoreCellOnRemoved { get; set; }

    /// <summary>
    /// When <see langword="true"/>, indicates that the <see cref="EffectsManager"/> should run one update frame on this effect when it's first added to the manager.
    /// </summary>
    bool RunEffectOnApply { get; set; }

    /// <summary>
    /// Applies the state of the effect to a cell.
    /// </summary>
    /// <param name="cell">The surface cell using this effect.</param>
    /// <param name="originalState">The state of the cell prior to the effect being applied.</param>
    /// <returns><see langword="true"/> when this method modified the <paramref name="cell"/>; otherwise <see langword="false"/>.</returns>
    bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState);

    /// <summary>
    /// Updates the state of the effect.
    /// </summary>
    /// <param name="delta">Time since the last call to this effect.</param>
    void Update(System.TimeSpan delta);

    /// <summary>
    /// Restarts the cell effect.
    /// </summary>
    void Restart();

    /// <summary>
    /// Returns a duplicate of this effect.
    /// </summary>
    /// <returns>A new copy of this effect.</returns>
    ICellEffect Clone();
}
