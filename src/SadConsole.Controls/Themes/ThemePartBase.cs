namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for a theme part or a simple theme.
    /// </summary>
    [DataContract]
    public class ThemePartBase
    {
        /// <summary>
        /// The normal appearance of the control.
        /// </summary>
        [DataMember]
        public CellAppearance Normal;

        /// <summary>
        /// The appearance of the control when it is focused.
        /// </summary>
        [DataMember]
        public CellAppearance Focused;

        /// <summary>
        /// The appearance of the control when the mouse is over it.
        /// </summary>
        [DataMember]
        public CellAppearance MouseOver;

        /// <summary>
        /// The appearance of the control when it is disabled.
        /// </summary>
        [DataMember]
        public CellAppearance Disabled;

        /// <summary>
        /// Clones this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public virtual object Clone()
        {
            var newItem = new ThemePartBase();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.Disabled = this.Disabled.Clone();
            return newItem;
        }
    }
}
