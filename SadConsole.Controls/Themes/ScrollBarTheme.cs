namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The theme of the slider control.
    /// </summary>
    [DataContract]
    public class ScrollBarTheme
    {
        /// <summary>
        /// The theme part for the ends of the scroll bar.
        /// </summary>
        [DataMember]
        public ThemePartBase Ends;

        /// <summary>
        /// The theme part for the scroll bar bar where the slider is not located.
        /// </summary>
        [DataMember]
        public ThemePartBase Bar;

        /// <summary>
        /// The theme part for the scroll bar icon.
        /// </summary>
        [DataMember]
        public ThemePartBase Slider;

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            var newItem = new ScrollBarTheme();
            newItem.Ends = (ThemePartBase)this.Ends.Clone();
            newItem.Bar = (ThemePartBase)this.Bar.Clone();
            newItem.Slider = (ThemePartBase)this.Slider.Clone();
            return newItem;
        }
    }
}
