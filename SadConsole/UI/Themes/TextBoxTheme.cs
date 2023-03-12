using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// A theme for the input box control.
/// </summary>
[DataContract]
public class TextBoxTheme : ThemeBase
{
    private int _oldCaretPosition;
    private ControlStates _oldState;
    private string _editingText = string.Empty;

    /// <summary>
    /// The style to use for the carrot.
    /// </summary>
    [DataMember]
    public Effects.ICellEffect CaretEffect;

    /// <summary>
    /// The color to use with a <see cref="NumberBox"/> control when <see cref="NumberBox.IsEditingNumberInvalid"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public Color? NumberBoxInvalidNumberForeground { get; set; }

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

        bool isFocusedSameAsBack = ControlThemeState.Focused.Background == _colorsLastUsed.ControlHostBackground;

        ControlThemeState.Normal.Background = GetOffColor(ControlThemeState.Normal.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseOver.Background = GetOffColor(ControlThemeState.MouseOver.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.MouseDown.Background = GetOffColor(ControlThemeState.MouseDown.Background, _colorsLastUsed.ControlHostBackground);
        ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, _colorsLastUsed.ControlHostBackground);

        // Further alter the color to indicate focus
        if (isFocusedSameAsBack)
            ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, ControlThemeState.Focused.Background);

        // If the focused background color is the same as the non-focused, alter it so it stands out
        ControlThemeState.Focused.Background = GetOffColor(ControlThemeState.Focused.Background, ControlThemeState.Normal.Background);
    }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not TextBox textbox) return;

        if (textbox.Surface.Effects.Count != 0)
        {
            textbox.Surface.Effects.UpdateEffects(time);
            textbox.IsDirty = true;
        }

        if (!textbox.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);
        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(textbox.State);

        if (textbox is NumberBox numberBox && (numberBox.Text.Length != 0 || (numberBox.Text.Length == 1 && numberBox.Text[0] != '-')))
        {
            if (numberBox.IsEditingNumberInvalid)
                appearance.Foreground = NumberBoxInvalidNumberForeground ?? _colorsLastUsed.Red;
        }

        if (textbox.IsFocused && !textbox.DisableKeyboard)
        {
            // TextBox was just focused
            if (textbox.State.HasFlag(ControlStates.Focused) && !_oldState.HasFlag(ControlStates.Focused))
            {
                _oldCaretPosition = textbox.CaretPosition;
                _oldState = textbox.State;
                _editingText = textbox.Text;
                textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (textbox.Mask == null)
                    textbox.Surface.Print(0, 0, textbox.Text.Substring(textbox.LeftDrawOffset));
                else
                    textbox.Surface.Print(0, 0, textbox.Text.Substring(textbox.LeftDrawOffset).Masked(textbox.Mask.Value));

                textbox.Surface.SetEffect(textbox.CaretPosition - textbox.LeftDrawOffset, 0, CaretEffect);
            }

            else if (_oldCaretPosition != textbox.CaretPosition || _oldState != textbox.State || _editingText != textbox.Text)
            {
                textbox.Surface.Effects.RemoveAll();
                textbox.Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (textbox.Mask == null)
                    textbox.Surface.Print(0, 0, textbox.Text.Substring(textbox.LeftDrawOffset));
                else
                    textbox.Surface.Print(0, 0, textbox.Text.Substring(textbox.LeftDrawOffset).Masked(textbox.Mask.Value));

                // TODO: If the keyboard repeat is down and the text goes off the end of the textbox and we're hitting the left arrow then sometimes control.LeftDrawOffset can exceed control.CaretPosition
                // This causes an Out of Bounds error here.  I don't think it's new - I think it's been in for a long time so I'm gonna check in and come back to this.
                // It might be that we just need to take Max(0, "bad value") below but I think it should be checked into to really understand the situation.
                textbox.Surface.SetEffect(textbox.CaretPosition - textbox.LeftDrawOffset, 0, CaretEffect);
                _oldCaretPosition = textbox.CaretPosition;
                _oldState = control.State;
                _editingText = textbox.Text;
            }

            textbox.IsDirty = true;
        }
        else
        {
            textbox.Surface.Effects.RemoveAll();
            textbox.Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
            _oldState = control.State;

            if (textbox.Mask == null)
                textbox.Surface.Print(0, 0, textbox.Text.Align(HorizontalAlignment.Left, textbox.Width));
            else
                textbox.Surface.Print(0, 0, textbox.Text.Masked(textbox.Mask.Value).Align(HorizontalAlignment.Left, textbox.Width));

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
