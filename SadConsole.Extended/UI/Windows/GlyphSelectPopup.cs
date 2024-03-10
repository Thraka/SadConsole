using System;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Windows;

/// <summary>
/// A popup window that displays the glyphs of a font.
/// </summary>
public class GlyphSelectPopup : Window
{
    /// <summary>
    /// Shows a modal version of this popup.
    /// </summary>
    /// <param name="font">The font to display.</param>
    /// <param name="fontSize">The size of the font.</param>
    public static void Show(IFont font, Point fontSize) =>
        new GlyphSelectPopup(19, 19, font, fontSize).Show(true);

    /// <summary>
    /// Adds a root component to SadConsole that displays the popup window when the specified key is pressed.
    /// </summary>
    /// <param name="key">The key to display the popup with.</param>
    public static void AddRootComponent(SadConsole.Input.Keys key) =>
        AddRootComponent(key, GameHost.Instance.DefaultFont, GameHost.Instance.DefaultFont.GetFontSize(GameHost.Instance.DefaultFontSize));

    /// <summary>
    /// Adds a root component to SadConsole that displays the popup window with a specific font and font size, when the specified key is pressed.
    /// </summary>
    /// <param name="key">The key to display the popup with.</param>
    /// <param name="font">The font to display.</param>
    /// <param name="fontSize">The size of the font.</param>
    public static void AddRootComponent(Input.Keys key, IFont font, Point fontSize)
    {
        GameHost.Instance.RootComponents.Add(new GlyphSelectRootComponent(key, font, fontSize));
    }

    /// <summary>
    /// Creates a new instance of this popup with the specified width, height, font, and font size.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    /// <param name="font">The font to use for the window and character picker.</param>
    /// <param name="fontSize">The font size of the window.</param>
    public GlyphSelectPopup(int width, int height, IFont font, Point fontSize) : base(width, height)
    {
        Font = font;
        FontSize = fontSize;

        Title = "Font Glyphs";

        Colors colors = Controls.GetThemeColors();

        CharacterPicker picker = new(Color.Gray, colors.ControlHostBackground, colors.ControlForegroundSelected, Font, width - 2, height - 4, 16)
        {
            Name = "picker",
            Position = (1, 1)
        };
        picker.SelectedCharacterChanged += (s, e) => DrawSelectedCharacter(((CharacterPicker)s!).SelectedCharacter);
        Controls.Add(picker);

        Center();
        CloseOnEscKey = true;

        DrawBorder();
    }

    /// <summary>
    /// Redraws the border, title, and lines.
    /// </summary>
    protected override void DrawBorder()
    {
        if (!Controls.HasNamedControl("picker")) return;

        base.DrawBorder();

        Colors themeColors = Controls.GetThemeColors();

        ColoredGlyph fillStyle = new(themeColors.ControlHostForeground, themeColors.ControlHostBackground);
        ColoredGlyph borderStyle = new(themeColors.Lines, fillStyle.Background, 0);

        Surface.DrawLine((1, Height - 3), (Width - 2, Height - 3), BorderLineStyle[0], borderStyle.Foreground, borderStyle.Background);
        Surface.ConnectLines(BorderLineStyle);
    }

    private void DrawSelectedCharacter(int glyph)
    {
        // Clear the text area
        Colors themeColors = Controls.GetThemeColors();
        ColoredGlyph fillStyle = new(themeColors.ControlHostForeground, themeColors.ControlHostBackground);
        Surface.DrawLine((1, Height - 2), (Width - 2, Height - 2), 0, fillStyle.Foreground, fillStyle.Background);

        // Print the text
        ColoredString text = ColoredString.Parser.Parse($"Glyph: [c:r f:AnsiRedBright]{glyph}[c:u] [c:r f:AnsiYellowBright]{(char)glyph}");
        text.IgnoreBackground = true;
        Surface.Print(2, Height - 2, text);
    }

    private class GlyphSelectRootComponent : Components.RootComponent
    {
        private GlyphSelectPopup _popup;
        private Input.Keys _key;

        public GlyphSelectRootComponent(Input.Keys key, IFont font, Point fontSize)
        {
            _key = key;
            _popup = new(19, 19, font, fontSize);
        }

        public override void Run(TimeSpan delta)
        {
            if (!_popup.IsVisible && GameHost.Instance.GetKeyboardState().IsKeyDown(_key))
                _popup.Show(true);
        }
    }
}
