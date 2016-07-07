namespace SadConsole.Themes
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

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
        public CellAppearance FillStyle;

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
