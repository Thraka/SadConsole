using System;
using System.Runtime.Serialization;
using SadConsole.Controls;
using SadConsole.Surfaces;


namespace SadConsole.Themes
{
    /// <summary>
    /// The theme of a checkbox control.
    /// </summary>
    [DataContract]
    public class CheckBoxTheme : ThemeBase<CheckBox>
    {
        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates CheckedIcon;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates UncheckedIcon;

        [DataMember] public ThemeStates LeftBracket;

        [DataMember] public ThemeStates RightBracket;

        public CheckBoxTheme()
        {
            CheckedIcon = new ThemeStates();
            UncheckedIcon = new ThemeStates();
            LeftBracket = new ThemeStates();
            RightBracket = new ThemeStates();

            CheckedIcon.SetGlyph(251);
            UncheckedIcon.SetGlyph(0);
            LeftBracket.SetGlyph('[');
            RightBracket.SetGlyph(']');
        }

        public override void Attached(CheckBox control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(CheckBox control, TimeSpan time)
        {
            if (control.IsDirty)
            {
                Cell appearance, iconAppearance, leftBracketAppearance, rightBracketAppearance;

                if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                {
                    appearance = Disabled;
                    iconAppearance = control.IsSelected ? CheckedIcon.Disabled : UncheckedIcon.Disabled;
                    leftBracketAppearance = LeftBracket.Disabled;
                    rightBracketAppearance = RightBracket.Disabled;
                }

                else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) ||
                         Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                {
                    appearance = MouseDown;
                    iconAppearance = control.IsSelected ? CheckedIcon.MouseDown : UncheckedIcon.MouseDown;
                    leftBracketAppearance = LeftBracket.MouseDown;
                    rightBracketAppearance = RightBracket.MouseDown;
                }

                else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                {
                    appearance = MouseOver;
                    iconAppearance = control.IsSelected ? CheckedIcon.MouseOver : UncheckedIcon.MouseOver;
                    leftBracketAppearance = LeftBracket.MouseOver;
                    rightBracketAppearance = RightBracket.MouseOver;
                }

                else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                {
                    appearance = Focused;
                    iconAppearance = control.IsSelected ? CheckedIcon.Focused : UncheckedIcon.Focused;
                    leftBracketAppearance = LeftBracket.Focused;
                    rightBracketAppearance = RightBracket.Focused;
                }

                else
                {
                    appearance = Normal;
                    iconAppearance = control.IsSelected ? CheckedIcon.Normal : UncheckedIcon.Normal;
                    leftBracketAppearance = LeftBracket.Normal;
                    rightBracketAppearance = RightBracket.Normal;
                }

                control.Surface.Fill(appearance.Foreground, appearance.Background, null);

                // If we are doing text, then print it otherwise we're just displaying the button part
                if (control.Width >= 5)
                {
                    leftBracketAppearance.CopyAppearanceTo(control.Surface[0, 0]);
                    iconAppearance.CopyAppearanceTo(control.Surface[1, 0]);
                    rightBracketAppearance.CopyAppearanceTo(control.Surface[2, 0]);

                    control.Surface.Print(4, 0, control.Text.Align(control.TextAlignment, control.Width - 4));
                }
                else
                {
                }

                control.IsDirty = false;
            }
        }

        public override object Clone()
        {
            return new CheckBoxTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                CheckedIcon = CheckedIcon.Clone(),
                UncheckedIcon = UncheckedIcon.Clone(),
                LeftBracket = LeftBracket.Clone(),
                RightBracket = RightBracket.Clone(),
            };
        }
    }

}
