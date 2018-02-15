using System.Runtime.Serialization;

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

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            var newItem = new ControlsConsoleTheme();
            newItem.FillStyle = this.FillStyle.Clone();
            return newItem;
        }
    }
}
