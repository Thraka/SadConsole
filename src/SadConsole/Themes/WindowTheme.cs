#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole.Themes
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A theme for a Window object.
    /// </summary>
    [DataContract]
    public class WindowTheme : ControlsConsoleTheme
    {
        /// <summary>
        /// The style of the title.
        /// </summary>
        [DataMember]
        public Cell TitleStyle;

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
        public Cell BorderStyle;

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

        public WindowTheme()
        {
            ModalTint = Library.Default.Colors.ModalBackground;

            FillStyle = new Cell(Library.Default.Colors.ControlHostFore, Library.Default.Colors.ControlHostBack);
            TitleStyle = new Cell(Library.Default.Colors.TitleText, FillStyle.Background, FillStyle.Glyph);
            BorderStyle = new Cell(Library.Default.Colors.Lines, FillStyle.Background, 0);
        }

        /// <summary>
        /// Returns a clone of this object. <see cref="BorderLineStyle"/> is referenced.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public new WindowTheme Clone() => new WindowTheme
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
        public override void Draw(ControlsConsole console, CellSurface hostSurface)
        {
            var themeColors = console.ThemeColors ?? Library.Default.Colors;

            FillStyle = new Cell(themeColors.ControlHostFore, themeColors.ControlHostBack);
            TitleStyle = new Cell(themeColors.TitleText, FillStyle.Background, FillStyle.Glyph);
            BorderStyle = new Cell(themeColors.Lines, FillStyle.Background, 0);
            ModalTint = themeColors.ModalBackground;

            hostSurface.DefaultForeground = FillStyle.Foreground;
            hostSurface.DefaultBackground = FillStyle.Background;
            hostSurface.Fill(hostSurface.DefaultForeground, hostSurface.DefaultBackground, FillStyle.Glyph, null);

            if (!(console is Window window))
            {
                return;
            }

            if (BorderLineStyle != null)
            {
                hostSurface.DrawBox(new Rectangle(0, 0, hostSurface.Width, hostSurface.Height), new Cell(BorderStyle.Foreground,
                    BorderStyle.Background, 0), null, BorderLineStyle);
            }

            // Draw title
            string adjustedText = "";
            int adjustedWidth = hostSurface.Width - 2;
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
                    TitleAreaX = hostSurface.Width - 1 - adjustedText.Length;
                }

                hostSurface.Print(TitleAreaX, TitleAreaY, adjustedText, TitleStyle);
            }
        }
    }
}
