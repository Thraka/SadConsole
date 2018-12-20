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
    public class TextBoxTheme: ThemeBase
    {
        private int _oldCaretPosition;
        private ControlStates _oldState;
        private string _editingText;

        /// <summary>
        /// The style to use for the carrot.
        /// </summary>
        [DataMember]
        public SadConsole.Effects.ICellEffect CaretEffect;

        /// <summary>
        /// Creates a new theme used by the <see cref="TextBox"/>.
        /// </summary>
        public TextBoxTheme()
        {
            CaretEffect = new Effects.BlinkGlyph()
            {
                GlyphIndex = 95,
                BlinkSpeed = 0.4f
            };
        }

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            control.Surface = new CellSurface(control.Width, control.Height);

            base.Attached(control);
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors)
        {
            base.RefreshTheme(themeColors);

            Normal = new SadConsole.Cell(themeColors.Text, themeColors.GrayDark);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is TextBox textbox)) return;

            if (textbox.Surface.Effects.Count != 0)
            {
                textbox.Surface.Effects.UpdateEffects(time.TotalSeconds);
                textbox.IsDirty = true;
            }

            if (!textbox.IsDirty) return;

            Cell appearance = GetStateAppearance(textbox.State);

            if (textbox.IsFocused && !textbox.DisableKeyboard)
            {
                if (!textbox.IsCaretVisible)
                {
                    _oldCaretPosition = textbox.CaretPosition;
                    _oldState = textbox.State;
                    _editingText = textbox.EditingText;
                    textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, SpriteEffects.None);
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset));
                    textbox.Surface.SetEffect(textbox.Surface[textbox.CaretPosition - textbox.LeftDrawOffset, 0], CaretEffect);
                    textbox.IsCaretVisible = true;
                }

                else if (_oldCaretPosition != textbox.CaretPosition || _oldState != textbox.State)
                {
                    textbox.Surface.Effects.RemoveAll();
                    textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, SpriteEffects.None);
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset));
                    textbox.Surface.SetEffect(textbox.Surface[textbox.CaretPosition - textbox.LeftDrawOffset, 0], CaretEffect);
                    _oldCaretPosition = textbox.CaretPosition;
                    _oldState = textbox.State;
                }
            }
            else
            {
                textbox.Surface.Effects.RemoveAll();
                textbox.Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
                textbox.IsCaretVisible = false;
                textbox.Surface.Print(0, 0, textbox.Text.Align(textbox.TextAlignment, textbox.Width));
            }

            textbox.IsDirty = false;
        }


        /// <inheritdoc />
        public override ThemeBase Clone()
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
