using System.Runtime.Serialization;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
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

        public ControlsConsoleTheme(Colors themeColors)
        {
            Refresh(themeColors);
        }

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

        public virtual void Draw(ControlsConsole console, CellSurface hostSurface)
        {
            hostSurface.DefaultForeground = FillStyle.Foreground;
            hostSurface.DefaultBackground = FillStyle.Background;
            hostSurface.Fill(hostSurface.DefaultForeground, hostSurface.DefaultBackground, FillStyle.Glyph, null);
        }

        public virtual void Refresh(Colors themeColors)
        {
            FillStyle = new Cell(themeColors.ControlHostFore, themeColors.ControlHostBack);
        }
    }
}
