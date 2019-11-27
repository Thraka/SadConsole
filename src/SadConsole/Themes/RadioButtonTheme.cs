#if XNA
using Microsoft.Xna.Framework;
#endif

using SadConsole.Controls;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme of a radio button control.
    /// </summary>
    [DataContract]
    public class RadioButtonTheme : ThemeBase
    {
        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates CheckedIcon { get; protected set; }

        /// <summary>
        /// The icon displayed when the radio button is checked.
        /// </summary>
        [DataMember] public ThemeStates UncheckedIcon { get; protected set; }

        /// <summary>
        /// The icon displayed for the brack left of the check icon.
        /// </summary>
        [DataMember] public ThemeStates LeftBracket { get; protected set; }

        /// <summary>
        /// The icon displayed for the brack right of the check icon.
        /// </summary>
        [DataMember] public ThemeStates RightBracket { get; protected set; }

        /// <summary>
        /// Creates a new theme used by the <see cref="RadioButton"/>.
        /// </summary>
        public RadioButtonTheme()
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

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is RadioButton radiobutton))
            {
                return;
            }

            if (!radiobutton.IsDirty)
            {
                return;
            }

            RefreshTheme(control.ThemeColors, control);

            //TODO Hardcoded radio button glyphs
            //CheckedIcon.SetGlyph(251);
            CheckedIcon.SetGlyph(15);
            UncheckedIcon.SetGlyph(0);
            LeftBracket.SetGlyph('(');
            RightBracket.SetGlyph(')');

            Cell appearance, iconAppearance, leftBracketAppearance, rightBracketAppearance;

            if (Helpers.HasFlag(radiobutton.State, ControlStates.Disabled))
            {
                appearance = Disabled;
                iconAppearance = radiobutton.IsSelected ? CheckedIcon.Disabled : UncheckedIcon.Disabled;
                leftBracketAppearance = LeftBracket.Disabled;
                rightBracketAppearance = RightBracket.Disabled;
            }

            else if (Helpers.HasFlag(radiobutton.State, ControlStates.MouseLeftButtonDown) ||
                     Helpers.HasFlag(radiobutton.State, ControlStates.MouseRightButtonDown))
            {
                appearance = MouseDown;
                iconAppearance = radiobutton.IsSelected ? CheckedIcon.MouseDown : UncheckedIcon.MouseDown;
                leftBracketAppearance = LeftBracket.MouseDown;
                rightBracketAppearance = RightBracket.MouseDown;
            }

            else if (Helpers.HasFlag(radiobutton.State, ControlStates.MouseOver))
            {
                appearance = MouseOver;
                iconAppearance = radiobutton.IsSelected ? CheckedIcon.MouseOver : UncheckedIcon.MouseOver;
                leftBracketAppearance = LeftBracket.MouseOver;
                rightBracketAppearance = RightBracket.MouseOver;
            }

            else if (Helpers.HasFlag(radiobutton.State, ControlStates.Focused))
            {
                appearance = Focused;
                iconAppearance = radiobutton.IsSelected ? CheckedIcon.Focused : UncheckedIcon.Focused;
                leftBracketAppearance = LeftBracket.Focused;
                rightBracketAppearance = RightBracket.Focused;
            }

            else
            {
                appearance = Normal;
                iconAppearance = radiobutton.IsSelected ? CheckedIcon.Normal : UncheckedIcon.Normal;
                leftBracketAppearance = LeftBracket.Normal;
                rightBracketAppearance = RightBracket.Normal;
            }

            radiobutton.Surface.Clear();


            radiobutton.Surface.Fill(appearance.Foreground, appearance.Background, null);

            // If we are doing text, then print it otherwise we're just displaying the button part
            if (radiobutton.Width >= 5)
            {
                leftBracketAppearance.CopyAppearanceTo(radiobutton.Surface[0, 0]);
                iconAppearance.CopyAppearanceTo(radiobutton.Surface[1, 0]);
                rightBracketAppearance.CopyAppearanceTo(radiobutton.Surface[2, 0]);

                radiobutton.Surface.Print(4, 0, radiobutton.Text.Align(radiobutton.TextAlignment, radiobutton.Width - 4));
            }

            radiobutton.IsDirty = false;
        }

        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            CheckedIcon.RefreshTheme(themeColors, control);
            UncheckedIcon.RefreshTheme(themeColors, control);
            LeftBracket.RefreshTheme(themeColors, control);
            RightBracket.RefreshTheme(themeColors, control);
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new RadioButtonTheme()
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
