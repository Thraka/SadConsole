namespace SadConsole.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the theme of a control who has a selection state.
    /// </summary>
    [DataContract]
    public class ThemePartSelected : ThemePartClickable
    {
        /// <summary>
        /// The style of the selected part of the control.
        /// </summary>
        [DataMember]
        public Cell Selected;

        /// <summary>
        /// Clones this object.
        /// </summary>
        /// <returns>The cloned theme.</returns>
        public override object Clone()
        {
            var newItem = new ThemePartSelected();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.Disabled = this.Disabled.Clone();
            newItem.MouseClicking = this.MouseClicking.Clone();
            newItem.Selected = this.Selected.Clone();
            return newItem;
        }
    }
}
