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
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public ControlsConsoleTheme Clone()
        {
            var newItem = new ControlsConsoleTheme
            {
                FillStyle = FillStyle.Clone()
            };
            return newItem;
        }

        public ControlsConsoleTheme()
        {
            FillStyle = Library.Default.Colors.Appearance_ControlNormal;
        }

        /// <summary>
        /// Draws the theme to the console.
        /// </summary>
        /// <param name="console">Console associated with the theme.</param>
        /// <param name="hostSurface">Surface used for drawing.</param>
        public virtual void Draw(ControlsConsole console, CellSurface hostSurface)
        {
            Colors colors = console.ThemeColors ?? Library.Default.Colors;

            FillStyle = colors.Appearance_ControlNormal;

            hostSurface.DefaultForeground = FillStyle.Foreground;
            hostSurface.DefaultBackground = FillStyle.Background;
            hostSurface.Fill(hostSurface.DefaultForeground, hostSurface.DefaultBackground, FillStyle.Glyph, null);
        }

    }
}
