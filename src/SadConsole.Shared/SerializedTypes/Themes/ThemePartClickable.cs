namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the theme of a control that can be clicked.
    /// </summary>
    [DataContract]
    public class ThemePartClickable : ThemePartBase
    {
        /// <summary>
        /// The appearance of the control when the mouse button is held down over it.
        /// </summary>
        [DataMember]
        public Cell MouseClicking;

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            var newItem = new ThemePartClickable();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.Disabled = this.Disabled.Clone();
            newItem.MouseClicking = this.MouseClicking.Clone();
            return newItem;
        }
    }
}
