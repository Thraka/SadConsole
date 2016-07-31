using System.Runtime.Serialization;

namespace SadConsole.Themes
{
    /// <summary>
    /// A theme for the input box control.
    /// </summary>
    [DataContract]
    public class InputBoxTheme: ThemePartBase
    {
        /// <summary>
        /// The style to use for the carrot.
        /// </summary>
        [DataMember]
        public SadConsole.Effects.ICellEffect CarrotEffect;

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            var newItem = new InputBoxTheme();
            newItem.Normal = this.Normal.Clone();
            newItem.Focused = this.Focused.Clone();
            newItem.MouseOver = this.MouseOver.Clone();
            newItem.Disabled = this.Disabled.Clone();
            newItem.CarrotEffect = this.CarrotEffect.Clone();
            return newItem;
        }
    }
}
