using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;

namespace SadConsole.UI.Themes;

/// <summary>
/// A theme for the input box control.
/// </summary>
[DataContract]
public class TextBoxTheme : ThemeBase
{
    private int _oldCaretPosition;
    private ControlStates _oldState;
    private string _editingText;

    /// <summary>
    /// The style to use for the carrot.
    /// </summary>
    [DataMember]
    public Effects.ICellEffect CaretEffect;

    /// <summary>
    /// Creates a new theme used by the <see cref="TextBox"/>.
    /// </summary>
    public TextBoxTheme() =>
        CaretEffect = new Effects.BlinkGlyph()
        {
            GlyphIndex = 95,
            BlinkSpeed = System.TimeSpan.FromSeconds(0.4d)
        };

    /// <inheritdoc />
    public override void RefreshTheme(Colors themeColors, ControlBase control)
    {
        base.RefreshTheme(themeColors, control);

        bool isFocsuedSameAsBack = ControlThemeState.Focused.Background == _colorsLastUsed.ControlHostBackground;

        ControlThemeState.Normal.Background = GetOffColor(ControlThemeState.Normal.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseOver.Background = GetOffColor(ControlThemeState.MouseOver.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseDown.Background = GetOffColor(ControlThemeState.MouseDown.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, _colorsLastUsed.ControlHostBackground);

        // Further alter the color to indicate focus
        if (isFocsuedSameAsBack)
            ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, ControlThemeState.Focused.Background);
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (!(control is TextBox textbox)) return;

        if (textbox.Surface.Effects.Count != 0)
        {
            textbox.Surface.Effects.UpdateEffects(time);
            textbox.IsDirty = true;
        }

        if (!textbox.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);
        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(textbox.State);

        if (textbox.IsFocused && !textbox.DisableKeyboard)
        {
            if (!textbox.IsCaretVisible)
            {
                _oldCaretPosition = textbox.CaretPosition;
                _oldState = textbox.State;
                _editingText = textbox.EditingText;
                textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (textbox.Mask == null)
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset));
                else
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset).Masked(textbox.Mask.Value));

                textbox.Surface.SetEffect(textbox.CaretPosition - textbox.LeftDrawOffset, 0, CaretEffect);
                textbox.IsCaretVisible = true;
            }

            else if (_oldCaretPosition != textbox.CaretPosition || _oldState != textbox.State || _editingText != textbox.EditingText)
            {
                textbox.Surface.Effects.RemoveAll();
                textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (textbox.Mask == null)
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset));
                else
                    textbox.Surface.Print(0, 0, textbox.EditingText.Substring(textbox.LeftDrawOffset).Masked(textbox.Mask.Value));

                // TODO: If the keyboard repeat is down and the text goes off the end of the textbox and we're hitting the left arrow then sometimes control.LeftDrawOffset can exceed control.CaretPosition
                // This causes an Out of Bounds error here.  I don't think it's new - I think it's been in for a long time so I'm gonna check in and come back to this.
                // It might be that we just need to take Max(0, "bad value") below but I think it should be checked into to really understand the situation.
                textbox.Surface.SetEffect(textbox.CaretPosition - textbox.LeftDrawOffset, 0, CaretEffect);
                _oldCaretPosition = textbox.CaretPosition;
                _oldState = control.State;
                _editingText = textbox.EditingText;

            }

            textbox.IsDirty = true;
        }
        else
        {
            textbox.Surface.Effects.RemoveAll();
            textbox.Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
            textbox.IsCaretVisible = false;

            if (textbox.Mask == null)
                textbox.Surface.Print(0, 0, textbox.Text.Align(textbox.TextAlignment, textbox.Width));
            else
                textbox.Surface.Print(0, 0, textbox.Text.Masked(textbox.Mask.Value).Align(textbox.TextAlignment, textbox.Width));

            textbox.IsDirty = false;
        }
    }


    /// <inheritdoc />
    public override ThemeBase Clone() => new TextBoxTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        CaretEffect = CaretEffect.Clone()
    };
}
