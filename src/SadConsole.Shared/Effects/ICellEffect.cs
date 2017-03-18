namespace SadConsole.Effects
{
    /// <summary>
    /// The interface describing a cell effect
    /// </summary>
    public interface ICellEffect// : System.IEquatable<ICellEffect>
    {
        /// <summary>
        /// True when the effect is finished but attached to the cell.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Flags this effect to be cloned when applied to a cell instead of reused.
        /// </summary>
        bool CloneOnApply { get; set; }

        /// <summary>
        /// A delay applied to the effect only when it first runs.
        /// </summary>
        double StartDelay { get; set; }

        /// <summary>
        /// When true, the effect should be disassociated with cells when it has finished processing.
        /// </summary>
        bool RemoveOnFinished { get; set; }

        /// <summary>
        /// When true, the effect should not call <see cref="Cell.RestoreState"/> when it has finished processing.
        /// </summary>
        bool KeepStateOnFinished { get; set; }

        /// <summary>
        /// When true, indicates the effect this effect has on a cell should remain after this effect is cleared from the cell.
        /// </summary>
        bool Permanent { get; set; }

        /// <summary>
        /// Applies the state of the effect to a cell.
        /// </summary>
        /// <param name="callingCell">The console cell using this effect.</param>
        bool Apply(Cell cell);

        /// <summary>
        /// Updates the state of the effect.
        /// </summary>
        /// <param name="gameTimeSeconds">Time since the last call to this effect.</param>
        void Update(double timeElapsed);

        /// <summary>
        /// Clears the effect from the cell.
        /// </summary>
        /// <param name="cell">The console cell that will no longer use this effect.</param>
        void Clear(Cell cell);

        /// <summary>
        /// Restarts the cell effect but does not reset it.
        /// </summary>
        void Restart();

        /// <summary>
        /// Returns a duplicate of this effect.
        /// </summary>
        /// <returns>A new copy of this effect.</returns>
        ICellEffect Clone();
    }
}
