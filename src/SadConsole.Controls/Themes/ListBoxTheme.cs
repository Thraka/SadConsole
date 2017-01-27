using Microsoft.Xna.Framework;

using System.Runtime.Serialization;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme for a ListBox control.
    /// </summary>
    [DataContract]
    public class ListBoxTheme : ThemePartBase
    {
        /// <summary>
        /// The appearance of the border.
        /// </summary>
        [DataMember]
        public Cell Border;

        /// <summary>
        /// The appearance of an item.
        /// </summary>
        [DataMember]
        public ThemePartSelected Item;

        /// <summary>
        /// The appearance of the scrollbar used by the listbox control.
        /// </summary>
        [DataMember]
        public ScrollBarTheme ScrollBarTheme;

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            var newItem = new ListBoxTheme();
            newItem.Border = this.Border.Clone();
            newItem.ScrollBarTheme = (ScrollBarTheme)this.ScrollBarTheme.Clone();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.Disabled = this.Disabled.Clone();
            newItem.Item = (ThemePartSelected)this.Item.Clone();
            return newItem;
        }
    }
}
