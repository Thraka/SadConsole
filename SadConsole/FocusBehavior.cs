namespace SadConsole
{
    /// <summary>
    /// How the console handles becoming focused and added to the <see cref="GameHost.FocusedScreenObjects"/> collection.
    /// </summary>
    public enum FocusBehavior
    {
        /// <summary>
        /// Becomes the only active input object when focused.
        /// </summary>
        Set,

        /// <summary>
        /// Pushes to the top of the stack when it becomes the active input object.
        /// </summary>
        Push,

        /// <summary>
        /// Don't use the global focus manager.
        /// </summary>
        None
    }
}
