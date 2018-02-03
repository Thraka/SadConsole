using System.Runtime.Serialization;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme of the button control
    /// </summary>
    [DataContract]
    public class ButtonTheme: ThemePartClickable
    {

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            var newItem = new ButtonTheme();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.MouseClicking = this.MouseClicking.Clone();
            newItem.Disabled = this.Disabled.Clone();
            return newItem;
        }
    }

}
