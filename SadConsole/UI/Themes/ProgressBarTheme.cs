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
        /// Creates a new theme used by the <see cref="ProgressBar"/>.
        /// </summary>
        public ProgressBarTheme()
        {
            Background = new ThemeStates();
            Foreground = new ThemeStates();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!control.IsDirty) return;
            if (!(control is ProgressBar progressbar)) return;

            RefreshTheme(control.FindThemeColors(), control);

            ColoredGlyph foregroundAppearance = Foreground.GetStateAppearance(control.State);
            ColoredGlyph backgroundAppearance = Background.GetStateAppearance(control.State);

            progressbar.Surface.Fill(backgroundAppearance.Foreground, backgroundAppearance.Background, backgroundAppearance.Glyph);

            if (progressbar.IsHorizontal)
            {
                Rectangle fillRect;

                if (progressbar.HorizontalAlignment == HorizontalAlignment.Left)
                    fillRect = new Rectangle(0, 0, progressbar.fillSize, progressbar.Height);
                else
                    fillRect = new Rectangle(progressbar.Width - progressbar.fillSize, 0, progressbar.fillSize, progressbar.Height);

                progressbar.Surface.Fill(fillRect, foregroundAppearance.Foreground, foregroundAppearance.Background, foregroundAppearance.Glyph);
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

        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            Background.RefreshTheme(themeColors);
            Background.SetForeground(Background.Normal.Foreground);
            Background.SetBackground(Background.Normal.Background);
            Background.SetGlyph(176);
            Background.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 176);
            Foreground.RefreshTheme(themeColors);
            Foreground.SetForeground(Background.Normal.Foreground);
            Foreground.SetBackground(Background.Normal.Background);
            Foreground.SetGlyph(219);
            Foreground.Disabled = new ColoredGlyph(Color.Gray, Color.Black, 219);
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
