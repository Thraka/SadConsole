using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme of a radio button control.
    /// </summary>
    [DataContract]
    public class ProgressBarTheme : ThemeBase
    {
        /// <summary>
        /// The theme of the unprogressed part of the bar.
        /// </summary>
        public ThemeStates Background { get; protected set; }

        /// <summary>
        /// The theme of the progressed part of the bar.
        /// </summary>
        public ThemeStates Foreground { get; protected set; }

        /// <summary>
        /// The theme of the text displayed on the bar.
        /// </summary>
        public ThemeStates DisplayText { get; protected set; }

        /// <summary>
        /// When <see langword="true"/>, prints the <see cref="Label.DisplayText"/> on the control in decorators instead of replacing the portation of the bar that overlaps the text.
        /// </summary>
        public bool PrintDisplayAsDecorator { get; set; }

        /// <summary>
        /// Creates a new theme used by the <see cref="ProgressBar"/>.
        /// </summary>
        public ProgressBarTheme()
        {
            PrintDisplayAsDecorator = true;

            Background = new ThemeStates();
            Foreground = new ThemeStates();
            DisplayText = new ThemeStates();

            Background.SetGlyph(176);
            Foreground.SetGlyph(219);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!control.IsDirty) return;
            if (!(control is ProgressBar progressbar)) return;

            RefreshTheme(control.FindThemeColors(), control);

            ColoredGlyph foregroundAppearance = Foreground.GetStateAppearance(control.State);
            ColoredGlyph backgroundAppearance = Background.GetStateAppearance(control.State);
            ColoredGlyph displayTextAppearance = DisplayText.GetStateAppearance(control.State);

            progressbar.Surface.Fill(backgroundAppearance.Foreground, backgroundAppearance.Background, backgroundAppearance.Glyph);

            if (progressbar.IsHorizontal)
            {
                Rectangle fillRect;

                if (progressbar.HorizontalAlignment == HorizontalAlignment.Left)
                    fillRect = new Rectangle(0, 0, progressbar.fillSize, progressbar.Height);
                else
                    fillRect = new Rectangle(progressbar.Width - progressbar.fillSize, 0, progressbar.fillSize, progressbar.Height);

                progressbar.Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);

                if (progressbar.DisplayTextColor.A != 0 && !string.IsNullOrEmpty(progressbar.DisplayText))
                {
                    string alignedString;
                    if (progressbar.DisplayText == "%")
                        alignedString = $"{(int)(progressbar.Progress * 100)}%".Align(progressbar.DisplayTextAlignment, progressbar.Width);
                    else
                        alignedString= progressbar.DisplayText.Align(progressbar.DisplayTextAlignment, progressbar.Width);

                    int centerRow = progressbar.Surface.Height / 2;

                    for (int i = 0; i < alignedString.Length; i++)
                    {
                        if (alignedString[i] != ' ')
                        {
                            if (PrintDisplayAsDecorator)
                                progressbar.Surface.AddDecorator(i, centerRow, 1, new CellDecorator(displayTextAppearance.Foreground, alignedString[i], Mirror.None));
                            else
                                progressbar.Surface.SetGlyph(i, centerRow, alignedString[i], displayTextAppearance.Foreground);
                        }
                    }
                }
            }

            else
            {
                Rectangle fillRect;

                if (progressbar.VerticalAlignment == VerticalAlignment.Top)
                    fillRect = new Rectangle(0, 0, progressbar.Width, progressbar.fillSize);
                else
                    fillRect = new Rectangle(0, progressbar.Height - progressbar.fillSize, progressbar.Width, progressbar.fillSize);

                progressbar.Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);
            }

            

            progressbar.IsDirty = false;
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            var bar = (ProgressBar)control;

            Background.RefreshTheme(themeColors);
            Background.SetForeground(Background.Normal.Foreground);
            Background.SetBackground(Background.Normal.Background);
            Background.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 176);
            Foreground.RefreshTheme(themeColors);
            Foreground.SetForeground(Foreground.Normal.Foreground);
            Foreground.SetBackground(Foreground.Normal.Background);
            Foreground.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 219);
            DisplayText.RefreshTheme(themeColors);
            DisplayText.SetForeground(bar.DisplayTextColor);
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new ProgressBarTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
            Foreground = Foreground?.Clone(),
            Background = Background?.Clone()
        };
    }
}
