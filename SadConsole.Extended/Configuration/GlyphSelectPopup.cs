using SadConsole.Input;

namespace SadConsole.Configuration;


internal class GlyphSelectPopup : IConfigurator
{
    public Keys Key { get; set; }

    public void Run(BuilderBase config, GameHost game)
    {
        UI.Windows.GlyphSelectPopup.AddRootComponent(Key);

    }
}

/// <summary>
/// Extensions to enable the glyph picker via the startup config system.
/// </summary>
public static class BuilderExtensionsGlyphSelect
{
    /// <summary>
    /// Enables the glyph picker and uses the specified key to show the window.
    /// </summary>
    /// <param name="builder">The config builder.</param>
    /// <param name="key">The key.</param>
    /// <returns>The config builder.</returns>
    public static Builder PopupGlyphPicker(this Builder builder, Keys key)
    {
        GlyphSelectPopup popup = builder.GetOrCreateConfig<GlyphSelectPopup>();
        popup.Key = key;
        return builder;
    }
}
