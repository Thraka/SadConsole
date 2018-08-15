using System;
using System.Runtime.Serialization;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    /// <summary>
    /// A theme for the input box control.
    /// </summary>
    [DataContract]
    public class InputBoxTheme: ThemeBase<InputBox>
    {
        /// <summary>
        /// The style to use for the carrot.
        /// </summary>
        [DataMember]
        public SadConsole.Effects.ICellEffect CaretEffect;

        public InputBoxTheme()
        {
            CaretEffect = new Effects.BlinkGlyph()
            {
                GlyphIndex = 95,
                BlinkSpeed = 0.4f
            };
        }

        public override void Draw(InputBox control, SurfaceBase hostSurface)
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

                // Clear the existing area of the control

                Cell[] controlCells = hostSurface.GetCells(control.Bounds);

                if (controlCells.Length == 0) return;

                hostSurface.SetEffect(hostSurface.Fill(control.Bounds, appearance.Foreground, appearance.Background, 0), null);

                if (control.IsFocused && !control.DisableKeyboard)
                {
                    hostSurface.Print(control.Bounds.Left, control.Bounds.Top, control.EditingText.Substring(control.LeftDrawOffset));
                    hostSurface.SetEffect(hostSurface[control.Bounds.Left + (control.CaretPosition - control.LeftDrawOffset), control.Bounds.Top], CaretEffect);
                    control.IsCaretVisible = true;
                }
                else
                {
                    control.IsCaretVisible = false;
                    hostSurface.Print(control.Bounds.Left, control.Bounds.Top, control.Text.Align(control.TextAlignment, control.Width));
                }
                
                control.IsDirty = false;
            }
        }
    }
}
