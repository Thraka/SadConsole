using System;
using System.Runtime.Serialization;
using System.Text;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class NumberBox
{
    private int _oldCaretPosition;
    private ControlStates _oldState;
    private string _editingText = string.Empty;

    /// <summary>
    /// Used by the mouse logic.
    /// </summary>
    public bool State_IsMouseOverUpButton { get; set; } = false;

    /// <summary>
    /// Used by the mouse logic.
    /// </summary>
    public bool State_IsMouseOverDownButton { get; set; } = false;

    /// <summary>
    /// The color to use with a <see cref="NumberBox"/> control when <see cref="NumberBox.IsEditingNumberInvalid"/> is <see langword="true"/>.
    /// </summary>
    [DataMember]
    public Color? NumberBoxInvalidNumberForeground { get; set; }

    /// <summary>
    /// The glyph for the up button.
    /// </summary>
    [DataMember]
    public int UpButtonGlyph { get; set; } = 30;

    /// <summary>
    /// The glyph for the down button.
    /// </summary>
    [DataMember]
    public int DownButtonGlyph { get; set; } = 31;

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        // IMPORTANT:
        // Code fixed here should go into the TextBox control

        if (Surface.Effects.Count != 0)
        {
            Surface.Effects.UpdateEffects(time);
            IsDirty = true;
        }

        if (!IsDirty) return;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        bool isFocusedSameAsBack = ThemeState.Focused.Background == colors.ControlHostBackground;

        ThemeState.Normal.Background = colors.GetOffColor(ThemeState.Normal.Background, colors.ControlHostBackground);
        ThemeState.MouseOver.Background = colors.GetOffColor(ThemeState.MouseOver.Background, colors.ControlHostBackground);
        ThemeState.MouseDown.Background = colors.GetOffColor(ThemeState.MouseDown.Background, colors.ControlHostBackground);
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, colors.ControlHostBackground);

        // Further alter the color to indicate focus
        if (isFocusedSameAsBack)
            ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Focused.Background);

        // If the focused background color is the same as the non-focused, alter it so it stands out
        ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Normal.Background);

        ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);

        // TODO: Fix this hack...
        if (Text.Length != 0 || (Text.Length == 1 && Text[0] != '-'))
        {
            if (IsEditingNumberInvalid)
                appearance.Foreground = NumberBoxInvalidNumberForeground ?? colors.Red;
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

            if (ShowUpDownButtons)
            {
                ColoredGlyphBase normal = ThemeState.GetStateAppearance(State);

                Surface[^1].Glyph = UpButtonGlyph;
                Surface[^2].Glyph = DownButtonGlyph;
                Surface[^1].Foreground = normal.Foreground;
                Surface[^1].Background = normal.Background;
                Surface[^2].Foreground = normal.Foreground;
                Surface[^2].Background = normal.Background;
            }

            IsDirty = true;
        }
        else
        {
            if (ShowUpDownButtons && !Helpers.HasFlag((int)State, (int)ControlStates.Disabled))
            {
                ColoredGlyphBase textAreaAppearance = appearance;

                if (State_IsMouseOverUpButton || State_IsMouseOverDownButton)
                    textAreaAppearance = ThemeState.GetStateAppearance(ControlStates.Normal);

                Surface.Effects.RemoveAll();
                Surface.Fill(textAreaAppearance.Foreground, textAreaAppearance.Background, textAreaAppearance.Glyph, textAreaAppearance.Mirror);

                if (Mask == null)
                    Surface.Print(0, 0, Text.Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));
                else
                    Surface.Print(0, 0, Text.Masked(Mask.Value).Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));

                Surface[^1].Glyph = UpButtonGlyph;
                Surface[^2].Glyph = DownButtonGlyph;

                if (State_IsMouseOverUpButton)
                {
                    Surface[^1].Foreground = appearance.Foreground;
                    Surface[^1].Background = appearance.Background;
                }
                else if (State_IsMouseOverDownButton)
                {
                    Surface[^2].Foreground = appearance.Foreground;
                    Surface[^2].Background = appearance.Background;
                }
            }
            else
            {
                appearance = ThemeState.GetStateAppearance(ControlStates.Normal);
                Surface.Effects.RemoveAll();
                Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);

                if (Mask == null)
                    Surface.Print(0, 0, Text.Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));
                else
                    Surface.Print(0, 0, Text.Masked(Mask.Value).Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));
            }

            IsDirty = false;

            _oldState = State;
        }
    }
}
