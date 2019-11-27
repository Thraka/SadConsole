#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using System.Runtime.Serialization;
using SadConsole.Controls;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme of a checkbox control.
    /// </summary>
    [DataContract]
    public class CheckBoxTheme : ThemeBase
    {
        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates CheckedIcon;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates UncheckedIcon;

        /// <summary>
        /// The icon displayed for the brack left of the check icon.
        /// </summary>
        [DataMember] public ThemeStates LeftBracket;

        /// <summary>
        /// The icon displayed for the brack right of the check icon.
        /// </summary>
        [DataMember] public ThemeStates RightBracket;

        /// <summary>
        /// Creates a new theme used by the <see cref="CheckBox"/>.
        /// </summary>
        public CheckBoxTheme()
        {
            CheckedIcon = new ThemeStates();
            UncheckedIcon = new ThemeStates();
            LeftBracket = new ThemeStates();
            RightBracket = new ThemeStates();
        }

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();

            base.Attached(control);
        }

        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            CheckedIcon.RefreshTheme(themeColors, control);
            UncheckedIcon.RefreshTheme(themeColors, control);
            LeftBracket.RefreshTheme(themeColors, control);
            RightBracket.RefreshTheme(themeColors, control);

            CheckedIcon.SetGlyph(251);
            UncheckedIcon.SetGlyph(0);
            LeftBracket.SetGlyph('[');
            RightBracket.SetGlyph(']');
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is CheckBox checkbox))
            {
                return;
            }

            if (!control.IsDirty)
            {
                return;
            }

            RefreshTheme(control.ThemeColors, control);
            Cell appearance, iconAppearance, leftBracketAppearance, rightBracketAppearance;

            if (Helpers.HasFlag(checkbox.State, ControlStates.Disabled))
            {
                appearance = Disabled;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.Disabled : UncheckedIcon.Disabled;
                leftBracketAppearance = LeftBracket.Disabled;
                rightBracketAppearance = RightBracket.Disabled;
            }

            else if (Helpers.HasFlag(checkbox.State, ControlStates.MouseLeftButtonDown) ||
                     Helpers.HasFlag(checkbox.State, ControlStates.MouseRightButtonDown))
            {
                appearance = MouseDown;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.MouseDown : UncheckedIcon.MouseDown;
                leftBracketAppearance = LeftBracket.MouseDown;
                rightBracketAppearance = RightBracket.MouseDown;
            }

            else if (Helpers.HasFlag(checkbox.State, ControlStates.MouseOver))
            {
                appearance = MouseOver;
                iconAppearance = checkbox.IsSelected ? CheckedIcon.MouseOver : UncheckedIcon.MouseOver;
                leftBracketAppearance = LeftBracket.MouseOver;
                rightBracketAppearance = RightBracket.MouseOver;
            }

            else if (Helpers.HasFlag(checkbox.State, ControlStates.Focused))
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
            CheckedIcon = CheckedIcon?.Clone(),
            UncheckedIcon = UncheckedIcon?.Clone(),
            LeftBracket = LeftBracket?.Clone(),
            RightBracket = RightBracket?.Clone(),
        };
    }
}
