using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// A theme for a Window object.
    /// </summary>
    [DataContract]
    public class Window : ControlsConsole
    {
        /// <summary>
        /// The style of the title.
        /// </summary>
        [DataMember]
        public ColoredGlyph TitleStyle;

        /// <summary>
        /// The Y coordinate of the title drawing area. This can be set to any value > 0 and &lt; the height.
        /// </summary>
        [DataMember]
        public int TitleAreaY;

        /// <summary>
        /// The X coordinate of the title drawing area. This is automatically set by the theme.
        /// </summary>
        [DataMember]
        public int TitleAreaX;

        /// <summary>
        /// The width of the title drawing area. This is automatically set by the theme.
        /// </summary>
        [DataMember]
        public int TitleAreaLength;

        /// <summary>
        /// The style of the border
        /// </summary>
        [DataMember]
        public ColoredGlyph BorderStyle;

        /// <summary>
        /// The line sytle for the border.
        /// </summary>
        [DataMember]
        public int[] BorderLineStyle;

        /// <summary>
        /// The color to tint the background when the window is shown as modal.
        /// </summary>
        [DataMember]
        public Color ModalTint;

        public Window()
        {
            ModalTint = Library.Default.Colors.ModalBackground;

            FillStyle = new ColoredGlyph(Library.Default.Colors.ControlHostFore, Library.Default.Colors.ControlHostBack);
            TitleStyle = new ColoredGlyph(Library.Default.Colors.TitleText, FillStyle.Background, FillStyle.Glyph);
            BorderStyle = new ColoredGlyph(Library.Default.Colors.Lines, FillStyle.Background, 0);
        }

        /// <summary>
        /// Returns a clone of this object. <see cref="BorderLineStyle"/> is referenced.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public new Window Clone() => new Window
        {
            TitleStyle = TitleStyle?.Clone(),
            BorderStyle = BorderStyle?.Clone(),
            FillStyle = FillStyle?.Clone(),
            TitleAreaY = TitleAreaY,
            TitleAreaX = TitleAreaX,
            TitleAreaLength = TitleAreaLength,
            ModalTint = ModalTint,
            BorderLineStyle = (int[])BorderLineStyle?.Clone()
        };

        /// <inheritdoc />
        public override void Draw(UI.ControlsConsole console)
        {
            var themeColors = console.FindThemeColors();

            FillStyle = new ColoredGlyph(themeColors.ControlHostFore, themeColors.ControlHostBack);
            TitleStyle = new ColoredGlyph(themeColors.TitleText, FillStyle.Background, FillStyle.Glyph);
            BorderStyle = new ColoredGlyph(themeColors.Lines, FillStyle.Background, 0);
            ModalTint = themeColors.ModalBackground;

            console.DefaultForeground = FillStyle.Foreground;
            console.DefaultBackground = FillStyle.Background;
            console.Fill(console.DefaultForeground, console.DefaultBackground, FillStyle.Glyph, null);

            if (!(console is UI.Window window))
            {
                return;
            }

            if (BorderLineStyle != null)
            {
                console.DrawBox(new Rectangle(0, 0, console.Width, console.Height), new ColoredGlyph(BorderStyle.Foreground,
                    BorderStyle.Background, 0), null, BorderLineStyle);
            }

            // Draw title
            string adjustedText = "";
            int adjustedWidth = console.Width - 2;
            TitleAreaLength = 0;
            TitleAreaX = 0;

            if (!string.IsNullOrEmpty(window.Title))
            {
                if (window.Title.Length > adjustedWidth)
                {
                    adjustedText = window.Title.Substring(0, window.Title.Length - (window.Title.Length - adjustedWidth));
                }
                else
                {
                    adjustedText = window.Title;
                }
            }

            if (!string.IsNullOrEmpty(adjustedText))
            {
                TitleAreaLength = adjustedText.Length;

                if (window.TitleAlignment == HorizontalAlignment.Left)
                {
                    TitleAreaX = 1;
                }
                else if (window.TitleAlignment == HorizontalAlignment.Center)
                {
                    TitleAreaX = ((adjustedWidth - adjustedText.Length) / 2) + 1;
                }
                else
                {
                    TitleAreaX = console.Width - 1 - adjustedText.Length;
                }

                console.Print(TitleAreaX, TitleAreaY, adjustedText, TitleStyle);
            }
        }

    }
}
