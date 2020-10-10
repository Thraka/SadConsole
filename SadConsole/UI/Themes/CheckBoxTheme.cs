using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme of a checkbox control.
    /// </summary>
    [DataContract]
    public class CheckBoxTheme : ThemeBase
    {
        private ThemeStates _checkedIcon;
        private ThemeStates _uncheckedIcon;
        private ThemeStates _leftBracket;
        private ThemeStates _rightBracket;

        protected ThemeStates _checkedIconDefault;
        protected ThemeStates _uncheckedIconDefault;
        protected ThemeStates _leftBracketDefault;
        protected ThemeStates _rightBracketDefault;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates CheckedIcon
        {
            get => _checkedIcon ?? _checkedIconDefault;
            set => _checkedIcon = value;
        }

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates UncheckedIcon
        {
            get => _uncheckedIcon ?? _uncheckedIconDefault;
            set => _uncheckedIcon = value;
        }

        /// <summary>
        /// The icon displayed for the brack left of the check icon.
        /// </summary>
        [DataMember] public ThemeStates LeftBracket
        {
            get => _leftBracket ?? _leftBracketDefault;
            set => _leftBracket = value;
        }

        /// <summary>
        /// The icon displayed for the brack right of the check icon.
        /// </summary>
        [DataMember] public ThemeStates RightBracket
        {
            get => _rightBracket ?? _rightBracketDefault;
            set => _rightBracket = value;
        }

        /// <summary>
        /// Creates a new theme used by the <see cref="CheckBox"/>.
        /// </summary>
        public CheckBoxTheme()
        {
            _checkedIconDefault = new ThemeStates();
            _uncheckedIconDefault = new ThemeStates();
            _leftBracketDefault = new ThemeStates();
            _rightBracketDefault = new ThemeStates();
        }

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            _checkedIconDefault.RefreshTheme(themeColors, control); _checkedIconDefault.SetForeground(themeColors.Lines);
            _uncheckedIconDefault.RefreshTheme(themeColors, control); _uncheckedIconDefault.SetForeground(themeColors.Lines);
            _leftBracketDefault.RefreshTheme(themeColors, control); _leftBracketDefault.SetForeground(themeColors.Lines);
            _rightBracketDefault.RefreshTheme(themeColors, control); _rightBracketDefault.SetForeground(themeColors.Lines);

            _checkedIconDefault.SetGlyph(251);
            _uncheckedIconDefault.SetGlyph(0);
            _leftBracketDefault.SetGlyph('[');
            _rightBracketDefault.SetGlyph(']');
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is CheckBox checkbox)) return;
            if (!control.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance, iconAppearance, leftBracketAppearance, rightBracketAppearance;

            if (Helpers.HasFlag((int)checkbox.State, (int)ControlStates.Disabled))
            {
                appearance = Disabled;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.Disabled : UncheckedIcon.Disabled;
                leftBracketAppearance = LeftBracket.Disabled;
                rightBracketAppearance = RightBracket.Disabled;
            }

            else if (Helpers.HasFlag((int)checkbox.State, (int)ControlStates.MouseLeftButtonDown) ||
                     Helpers.HasFlag((int)checkbox.State, (int)ControlStates.MouseRightButtonDown))
            {
                appearance = MouseDown;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.MouseDown : UncheckedIcon.MouseDown;
                leftBracketAppearance = LeftBracket.MouseDown;
                rightBracketAppearance = RightBracket.MouseDown;
            }

            else if (Helpers.HasFlag((int)checkbox.State, (int)ControlStates.MouseOver))
            {
                appearance = MouseOver;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.MouseOver : UncheckedIcon.MouseOver;
                leftBracketAppearance = LeftBracket.MouseOver;
                rightBracketAppearance = RightBracket.MouseOver;
            }

            else if (Helpers.HasFlag((int)checkbox.State, (int)ControlStates.Focused))
            {
                appearance = Focused;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.Focused : UncheckedIcon.Focused;
                leftBracketAppearance = LeftBracket.Focused;
                rightBracketAppearance = RightBracket.Focused;
            }

            else
            {
                appearance = Normal;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.Normal : UncheckedIcon.Normal;
                leftBracketAppearance = LeftBracket.Normal;
                rightBracketAppearance = RightBracket.Normal;
            }

            checkbox.Surface.Fill(appearance.Foreground, appearance.Background, null);

            // If we are doing text, then print it otherwise we're just displaying the button part
            if (checkbox.Width >= 5)
            {
                leftBracketAppearance.CopyAppearanceTo(checkbox.Surface[0, 0]);
                iconAppearance.CopyAppearanceTo(checkbox.Surface[1, 0]);
                rightBracketAppearance.CopyAppearanceTo(checkbox.Surface[2, 0]);

                checkbox.Surface.Print(4, 0, checkbox.Text.Align(checkbox.TextAlignment, checkbox.Width - 4));
            }

            checkbox.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new CheckBoxTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            _checkedIcon = _checkedIcon?.Clone(),
            _uncheckedIcon = _uncheckedIcon?.Clone(),
            _leftBracket = _leftBracket?.Clone(),
            _rightBracket = _rightBracket?.Clone(),
        };
    }
}
