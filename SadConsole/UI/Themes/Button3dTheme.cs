using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// A 3D shadow theme of the button control
    /// </summary>
    [DataContract]
    public class Button3dTheme : ButtonTheme
    {
        protected ColoredGlyph _shade = new ColoredGlyph();

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width + 2, control.Height + 1)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance = ControlThemeState.GetStateAppearance(control.State);

            int middle = button.Height != 1 ? button.Height / 2 : 0;

            var shadowBounds = new Rectangle(0, 0, button.Width, button.Height).WithPosition((2, 1));

            button.Surface.Clear();

            if (appearance.Matches(ControlThemeState.MouseDown))
            {
                middle += 1;

                // Redraw the control
                button.Surface.Fill(shadowBounds,
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                button.Surface.Print(shadowBounds.X, middle, button.Text.Align(button.TextAlignment, button.Width));
                button.MouseArea = new Rectangle(0, 0, button.Width + 2, button.Height + 1);
            }
            else
            {
                // Redraw the control
                button.Surface.Fill(new Rectangle(0, 0, button.Width, button.Height),
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width));

                // Bottom line
                button.Surface.DrawLine(new Point(shadowBounds.X, shadowBounds.MaxExtentY),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), _shade.Glyph, _shade.Foreground, _shade.Background);

                // Side line 1
                button.Surface.DrawLine(new Point(shadowBounds.MaxExtentX - 1, shadowBounds.Y),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), _shade.Glyph, _shade.Foreground, _shade.Background);

                // Side line 2
                button.Surface.DrawLine(new Point(shadowBounds.MaxExtentX, shadowBounds.Y),
                    new Point(shadowBounds.MaxExtentX, shadowBounds.MaxExtentY), _shade.Glyph, _shade.Foreground, _shade.Background);

                button.MouseArea = new Rectangle(0, 0, button.Width, button.Height);
            }

            button.IsDirty = false;
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors colors, ControlBase control)
        {
            base.RefreshTheme(colors, control);

            _shade.Foreground = _colorsLastUsed.ControlForegroundNormal;
            _shade.Background = _colorsLastUsed.ControlBackgroundNormal;
            _shade.Glyph = 176;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new Button3dTheme()
        {
            _shade = _shade.Clone(),
            ControlThemeState = ControlThemeState.Clone(),
            ShowEnds = ShowEnds,
            EndsThemeState = EndsThemeState.Clone()
        };
    }
}
