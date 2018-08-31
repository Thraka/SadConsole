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
    public class TextBoxTheme: ThemeBase<TextBox>
    {
        private int _oldCaretPosition;
        private ControlStates _oldState;
        private string _editingText;
        private Cell _oldAppearance;

        /// <summary>
        /// The style to use for the carrot.
        /// </summary>
        [DataMember]
        public SadConsole.Effects.ICellEffect CaretEffect;

        public TextBoxTheme()
        {
            CaretEffect = new Effects.BlinkGlyph()
            {
                GlyphIndex = 95,
                BlinkSpeed = 0.4f
            };

            Normal = new SadConsole.Cell(Colors.Text, Colors.GrayDark);
        }

        public override void Attached(TextBox control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(TextBox control, TimeSpan time)
        {
            if (control.Surface.Effects.Count != 0)
            {
                control.Surface.Update(time);
                control.IsDirty = true;
            }

            if (!control.IsDirty) return;

            Cell appearance = GetStateAppearance(control.State);

            if (control.IsFocused && !control.DisableKeyboard)
            {
                if (!control.IsCaretVisible)
                {
                    _oldCaretPosition = control.CaretPosition;
                    _oldState = control.State;
                    _editingText = control.EditingText;
                    control.Surface.Fill(appearance.Foreground, appearance.Background, 0, SpriteEffects.None);
                    control.Surface.Print(0, 0, control.EditingText.Substring(control.LeftDrawOffset));
                    control.Surface.SetEffect(control.Surface[control.CaretPosition - control.LeftDrawOffset, 0], CaretEffect);
                    control.IsCaretVisible = true;
                }

                else if (_oldCaretPosition != control.CaretPosition || _oldState != control.State)
                {
                    control.Surface.Effects.Remove(CaretEffect);
                    control.Surface.Fill(appearance.Foreground, appearance.Background, 0, SpriteEffects.None);
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
            return new TextBoxTheme()
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
