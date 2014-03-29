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
    public class WindowTheme
    {
        /// <summary>
        /// The style of the title.
        /// </summary>
        [DataMember]
        public CellAppearance TitleStyle;

        /// <summary>
        /// The style of the border
        /// </summary>
        [DataMember]
        public CellAppearance BorderStyle;

        /// <summary>
        /// The style of window space inside the border.
        /// </summary>
        [DataMember]
        public CellAppearance FillStyle;

        /// <summary>
        /// The color to tint the background when the window is shown as modal.
        /// </summary>
        [DataMember]
        public Color ModalTint = new Color(20, 20, 20, 200);

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            var newItem = new WindowTheme();
            newItem.TitleStyle = this.TitleStyle.Clone();
            newItem.BorderStyle = this.BorderStyle.Clone();
            newItem.FillStyle = this.FillStyle.Clone();
            return newItem;
        }
    }
}
