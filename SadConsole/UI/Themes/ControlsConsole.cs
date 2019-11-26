using System.Runtime.Serialization;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// A theme for a Window object.
    /// </summary>
    [DataContract]
    public class ControlsConsole
    {
        /// <summary>
        /// The style of of the console surface.
        /// </summary>
        [DataMember]
        public ColoredGlyph FillStyle { get; set; } = null;

        /// <summary>
        /// Creates a new theme without specifying the colors.
        /// </summary>
        public ControlsConsole() { }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual ControlsConsole Clone()
        {
            return new ControlsConsole
            {
                FillStyle = FillStyle?.Clone()
            };
        }

        /// <summary>
        /// Draws the theme to the console.
        /// </summary>
        /// <param name="console">Console associated with the theme.</param>
        public virtual void Draw(UI.ControlsConsole console)
        {
            ColoredGlyph fillStyle = FillStyle ?? console.ThemeColors.Appearance_ControlNormal;

            console.DefaultForeground = fillStyle.Foreground;
            console.DefaultBackground = fillStyle.Background;
            console.Fill(console.DefaultForeground, console.DefaultBackground, fillStyle.Glyph, null);
        }
    }
}
