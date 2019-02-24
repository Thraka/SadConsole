namespace SadConsole.Themes
{
    using System.Runtime.Serialization;
    
    /// <summary>
    /// A theme for a Window object.
    /// </summary>
    [DataContract]
    public class ControlsConsoleTheme
    {
        /// <summary>
        /// The style of of the console surface.
        /// </summary>
        [DataMember]
        public Cell FillStyle;

        /// <summary>
        /// Creates a new controls console theme with the specified colors.
        /// </summary>
        /// <param name="themeColors">The colors used with this theme.</param>
        public ControlsConsoleTheme(Colors themeColors)
        {
            RefreshTheme(themeColors);
        }

        /// <summary>
        /// Creates a new theme without specifying the colors.
        /// </summary>
        protected ControlsConsoleTheme() { }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public ControlsConsoleTheme Clone()
        {
            var newItem = new ControlsConsoleTheme();
            newItem.FillStyle = this.FillStyle.Clone();
            return newItem;
        }

        /// <summary>
        /// Draws the theme to the console.
        /// </summary>
        /// <param name="console">Console associated with the theme.</param>
        /// <param name="hostSurface">Surface used for drawing.</param>
        public virtual void Draw(ControlsConsole console, CellSurface hostSurface)
        {
            hostSurface.DefaultForeground = FillStyle.Foreground;
            hostSurface.DefaultBackground = FillStyle.Background;
            hostSurface.Fill(hostSurface.DefaultForeground, hostSurface.DefaultBackground, FillStyle.Glyph, null);
        }

        /// <summary>
        /// Updates the theme with a color scheme.
        /// </summary>
        /// <param name="themeColors">The colors to update with.</param>
        public virtual void RefreshTheme(Colors themeColors)
        {
            FillStyle = new Cell(themeColors.ControlHostFore, themeColors.ControlHostBack);
        }
    }
}
