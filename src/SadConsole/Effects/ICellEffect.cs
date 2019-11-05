namespace SadConsole.Effects
{
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
        bool DiscardCellState { get; set; }

        /// <summary>
        /// When true, indicates the effect this effect has on a cell should remain after this effect is cleared from the cell.
        /// </summary>
        bool Permanent { get; set; }

        /// <summary>
        /// Cell is first associated with the effect.
        /// </summary>
        /// <param name="cell">The surface cell using this effect.</param>
        void AddCell(Cell cell);

        /// <summary>
        /// Applies the state of the effect to a cell.
        /// </summary>
        /// <param name="cell">The surface cell using this effect.</param>
        bool UpdateCell(Cell cell);

        /// <summary>
        /// Updates the state of the effect.
        /// </summary>
        /// <param name="gameTimeSeconds">Time since the last call to this effect.</param>
        void Update(double gameTimeSeconds);

        /// <summary>
        /// Clears the effect from the cell.
        /// </summary>
        /// <param name="cell">The surface cell that will no longer use this effect.</param>
        void ClearCell(Cell cell);

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
}
