namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The theme of a radio button control.
    /// </summary>
    [DataContract]
    public class ProgressBarTheme
    {
        /// <summary>
        /// The theme of the unprogressed part of the bar.
        /// </summary>
        [DataMember]
        public ThemePartBase Background;

        /// <summary>
        /// The theme of the progressed part of the bar.
        /// </summary>
        [DataMember]
        public ThemePartBase Foreground;

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public ProgressBarTheme Clone()
        {
            var newItem = new ProgressBarTheme();
            newItem.Background = (ThemePartBase)Background.Clone();
            newItem.Foreground = (ThemePartBase)Foreground.Clone();
            return newItem;
        }
    }
}
