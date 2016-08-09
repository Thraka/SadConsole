namespace SadConsole
{
    /// <summary>
    /// An object that draws to the screen and receives update ticks.
    /// </summary>
    public interface IDraw
    {
        /// <summary>
        /// Chooses to skip rendering or not.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Chooses to do updates or not.
        /// </summary>
        bool DoUpdate { get; set; }

        /// <summary>
        /// Updates the object.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws the object to the screen.
        /// </summary>
        void Render();
    }
}
