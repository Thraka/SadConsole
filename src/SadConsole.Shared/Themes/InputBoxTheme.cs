using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;
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
        private int _oldCaretPosition;
        private ControlStates _oldState;
        private string _editingText;

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

        public override void Attached(InputBox control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(InputBox control, TimeSpan time)
        {
            if (control.Surface.Effects.Count != 0)
            {
                control.Surface.Update(time);
                control.IsDirty = true;
            }

            if (!control.IsDirty) return;

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

            if (control.IsFocused && !control.DisableKeyboard)
            {
                //TODO: Maybe just manage the cell effect myself? Do not use the ScratchSurface.SetEffect?

                if (!control.IsCaretVisible)
                {
                    _oldCaretPosition = control.CaretPosition;
                    _oldState = control.State;
                    _editingText = control.EditingText;
                    control.Surface.Print(0, 0, control.EditingText.Substring(control.LeftDrawOffset));
                    control.Surface.SetEffect(control.Surface[control.CaretPosition - control.LeftDrawOffset, 0], CaretEffect);
                    control.IsCaretVisible = true;
                }

                else if (_oldCaretPosition != control.CaretPosition || _oldState != control.State)
                {
                    control.Surface.Fill(appearance.Foreground, appearance.Background, 0, SpriteEffects.None);
                    control.Surface.Effects.Remove(CaretEffect);
                    control.Surface.Print(0, 0, control.EditingText.Substring(control.LeftDrawOffset));
                    control.Surface.SetEffect(control.Surface[control.CaretPosition - control.LeftDrawOffset, 0], CaretEffect);
                    _oldCaretPosition = control.CaretPosition;
                    _oldState = control.State;
                }
            }
            else
            {
                control.Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
                control.IsCaretVisible = false;
                control.Surface.Print(0, 0, control.Text.Align(control.TextAlignment, control.Width));
            }

            control.IsDirty = false;
        }
        public override object Clone()
        {
            return new InputBoxTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                CaretEffect = CaretEffect.Clone()
            };
        }
    }
}
