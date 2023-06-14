using System;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class TextBox
{
    private int _oldCaretPosition;
    private ControlStates _oldState;
    private string _editingText = string.Empty;

    /// <summary>
    /// The style to use for the carrot.
    /// </summary>
    [DataMember]
    public Effects.ICellEffect CaretEffect { get; set; }

    /// <summary>
    /// The color to use with a <see cref="NumberBox"/> control when <see cref="NumberBox.IsEditingNumberInvalid"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public Color? NumberBoxInvalidNumberForeground { get; set; }


    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (Surface.Effects.Count != 0)
        {
            Surface.Effects.UpdateEffects(time);
            IsDirty = true;
        }

        if (!IsDirty) return;

        Colors _colorsLastUsed = FindThemeColors();

        ThemeState.RefreshTheme(_colorsLastUsed);

        bool isFocusedSameAsBack = ThemeState.Focused.Background == _colorsLastUsed.ControlHostBackground;

        ThemeState.Normal.Background = _colorsLastUsed.GetOffColor(ThemeState.Normal.Background, _colorsLastUsed.ControlHostBackground);
        ThemeState.MouseOver.Background = _colorsLastUsed.GetOffColor(ThemeState.MouseOver.Background, _colorsLastUsed.ControlHostBackground);
        ThemeState.MouseDown.Background = _colorsLastUsed.GetOffColor(ThemeState.MouseDown.Background, _colorsLastUsed.ControlHostBackground);
        ThemeState.Focused.Background = _colorsLastUsed.GetOffColor(ThemeState.Focused.Background, _colorsLastUsed.ControlHostBackground);

        // Further alter the color to indicate focus
        if (isFocusedSameAsBack)
            ThemeState.Focused.Background = _colorsLastUsed.GetOffColor(ThemeState.Focused.Background, ThemeState.Focused.Background);

        // If the focused background color is the same as the non-focused, alter it so it stands out
        ThemeState.Focused.Background = _colorsLastUsed.GetOffColor(ThemeState.Focused.Background, ThemeState.Normal.Background);

        ColoredGlyph appearance = ThemeState.GetStateAppearance(State);

        // TODO: Fix this hack...
        if (this is NumberBox numberBox && (numberBox.Text.Length != 0 || (numberBox.Text.Length == 1 && numberBox.Text[0] != '-')))
        {
            if (numberBox.IsEditingNumberInvalid)
                appearance.Foreground = NumberBoxInvalidNumberForeground ?? _colorsLastUsed.Red;
        }

        if (IsFocused && (Parent?.Host?.ParentConsole?.IsFocused).GetValueOrDefault(false) && !DisableKeyboard)
        {
            // TextBox was just focused
            if (State.HasFlag(ControlStates.Focused) && !_oldState.HasFlag(ControlStates.Focused))
            {
                _oldCaretPosition = CaretPosition;
                _oldState = State;
                _editingText = Text;
                Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (Mask == null)
                    Surface.Print(0, 0, Text.Substring(LeftDrawOffset));
                else
                    Surface.Print(0, 0, Text.Substring(LeftDrawOffset).Masked(Mask.Value));

                Surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);
            }

            else if (_oldCaretPosition != CaretPosition || _oldState != State || _editingText != Text)
            {
                Surface.Effects.RemoveAll();
                Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                if (Mask == null)
                    Surface.Print(0, 0, Text.Substring(LeftDrawOffset));
                else
                    Surface.Print(0, 0, Text.Substring(LeftDrawOffset).Masked(Mask.Value));

                // TODO: If the keyboard repeat is down and the text goes off the end of the textbox and we're hitting the left arrow then sometimes control.LeftDrawOffset can exceed control.CaretPosition
                // This causes an Out of Bounds error here.  I don't think it's new - I think it's been in for a long time so I'm gonna check in and come back to this.
                // It might be that we just need to take Max(0, "bad value") below but I think it should be checked into to really understand the situation.
                Surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);
                _oldCaretPosition = CaretPosition;
                _oldState = State;
                _editingText = Text;
            }

            IsDirty = true;
        }
        else
        {
            Surface.Effects.RemoveAll();
            Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
            _oldState = State;

            if (Mask == null)
                Surface.Print(0, 0, Text.Align(HorizontalAlignment.Left, Width));
            else
                Surface.Print(0, 0, Text.Masked(Mask.Value).Align(HorizontalAlignment.Left, Width));

            IsDirty = false;
        }
    }
}
