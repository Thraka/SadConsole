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
        [DataMember] public int CheckedIcon = 251;

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public int UncheckedIcon = 0;

        [DataMember] public int LeftBracket = '[';

        [DataMember] public int RightBracket = ']';

        public CheckBoxTheme()
        {
            Normal = Library.Default.Appearance_ControlNormal;
            Disabled = Library.Default.Appearance_ControlDisabled;
            MouseOver = Library.Default.Appearance_ControlOver;
            MouseDown = Library.Default.Appearance_ControlMouseDown;
            Selected = Library.Default.Appearance_ControlSelected;
            Focused = Library.Default.Appearance_ControlFocused;
        }

        public override void Draw(CheckBox control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
                Cell appearance;

                if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                    appearance = Disabled;

                else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                    appearance = MouseDown;

                else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                    appearance = MouseOver;

                else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                    appearance = Focused;

                else
                    appearance = Normal;

                hostSurface.Fill(control.Bounds, appearance.Foreground, appearance.Background, null);

                // If we are doing text, then print it otherwise we're just displaying the button part
                if (control.Width >= 3)
                {
                    hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top, LeftBracket);
                    hostSurface.SetGlyph(control.Bounds.Left + 2, control.Bounds.Top, RightBracket);

                    hostSurface.Print(control.Bounds.Left + 4, control.Bounds.Top, control.Text.Align(control.TextAlignment, control.Width - 4));

                    hostSurface.SetGlyph(control.Bounds.Left + 1, control.Bounds.Top, control.IsSelected ? CheckedIcon : UncheckedIcon);
                }
                else
                {
                }

                control.IsDirty = false;
            }
        }
    }

}
