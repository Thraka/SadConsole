namespace SadConsole.Controls
{
    using System.Runtime.Serialization;

#if XNA
    using Microsoft.Xna.Framework;
#endif

    /// <summary>
    /// A simple surface for drawing text that can be moved and sized like a control.
    /// </summary>
    [DataContract]
    public class Label : ControlBase
    {
        private string _text;

        /// <summary>
        /// When <see langword="true"/>, shows an underline on the text.
        /// </summary>
        public bool ShowUnderline { get; set; }

        /// <summary>
        /// When <see langword="true"/>, shows a strikethrough on the text.
        /// </summary>
        public bool ShowStrikethrough { get; set; }

        /// <summary>
        /// Optional text color for the label. Otherwise the theme controls the color.
        /// </summary>
        public Color? TextColor { get; set; }

        /// <summary>
        /// Sets the horizontal alignment of the label. Defaults to <see cref="HorizontalAlignment.Left"/>.
        /// </summary>
        public HorizontalAlignment Alignment { get; set; }

        /// <summary>
        /// The text to display on the label. The label size is set in the constructor and cannot be changed.
        /// </summary>
        public string DisplayText
        {
            get => _text;
            set
            {
                if (value == null)
                {
                    _text = "";
                }
                else if (value.Length > Width)
                {
                    _text = value.Substring(0, Width);
                }
                else
                {
                    _text = value;
                }
            }
        }

        /// <summary>
        /// A control to display simple one-line text.
        /// </summary>
        /// <param name="displayText">The text to display. Sets the width based on the length.</param>
        public Label(string displayText) : base(displayText.Length, 1)
        {
            _text = displayText;
            TabStop = false;
        }

        /// <summary>
        /// A control to display simple one-line text.
        /// </summary>
        /// <param name="length">The initial length of the label without any text.</param>
        public Label(int length) : base(length, 1) => TabStop = false;

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            TabStop = false;
            DetermineState();
            IsDirty = true;
        }
    }
}
