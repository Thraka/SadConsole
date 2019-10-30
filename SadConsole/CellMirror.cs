namespace SadConsole
{
    /// <summary>
    /// The mirroring mode
    /// </summary>
    [System.Flags]
    public enum Mirror
    {
        /// <summary>
        /// No mirroring set.
        /// </summary>
        None,

        /// <summary>
        /// Mirror vertically.
        /// </summary>
        Vertical,

        /// <summary>
        /// Mirror horizontally.
        /// </summary>
        Horizontal
    }
}
