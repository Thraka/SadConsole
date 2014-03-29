namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The theme of a radio button control.
    /// </summary>
    [DataContract]
    public class RadioButtonTheme : ThemePartSelected
    {
        /// <summary>
        /// The theme part for the button icon.
        /// </summary>
        [DataMember]
        public ThemePartSelected Button;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember]
        public int CheckedIcon;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember]
        public int UncheckedIcon;

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            var newItem = new RadioButtonTheme();
            newItem.Button = (ThemePartSelected)this.Button.Clone();
            newItem.CheckedIcon = this.CheckedIcon;
            newItem.UncheckedIcon = this.UncheckedIcon;
            newItem.Normal = base.Normal.Clone();
            newItem.Focused = base.Focused.Clone();
            newItem.MouseOver = base.MouseOver.Clone();
            newItem.MouseClicking = base.MouseClicking.Clone();
            newItem.Disabled = base.Disabled.Clone();
            newItem.Selected = base.Selected.Clone();
            return newItem;
        }
    }
}
