#nullable enable

using System;

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Configures which default font to use during game startup, as well as which other fonts to load for the game.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="fontLoader">A method that provides access to the <see cref="FontConfig"/> object that loads fonts.
    /// <returns>The configuration builder.</returns>
    public static Builder ConfigureFonts(this Builder configBuilder, Action<FontConfig, Game> fontLoader)
    {
        FontConfig config = configBuilder.GetOrCreateConfig<FontConfig>();

        config.FontLoader = fontLoader;

        return configBuilder;
    }

    /// <summary>
    /// Adds the embedded fonts to the font dictionary with the old incorrect name.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder FixOldFontName(this Builder configBuilder)
    {
         configBuilder.GetOrCreateConfig<OldFontNameConfig>();

        return configBuilder;
    }
}

internal class OldFontNameConfig : IConfigurator
{
    public void Run(Builder config, Game game)
    {
        game.Started += Game_Started;
    }

    private void Game_Started(object? sender, GameHost host)
    {
        host.Fonts["IBM_16x8"] = host.EmbeddedFont;
        host.Fonts["IBM_16x8_ext"] = host.EmbeddedFontExtended;

        host.Started -= Game_Started;
    }
}

public class FontConfig : IConfigurator
{
    internal Action<FontConfig, Game> FontLoader { get; set; }

    internal string[] CustomFonts = Array.Empty<string>();
    internal string? AlternativeDefaultFont;
    internal bool UseExtendedFont;

    /// <summary>
    /// Sets the default font to the SadConsole standard font, an IBM 8x16 font.
    /// </summary>
    public void UseBuiltinFont()
    {
        UseExtendedFont = false;
        AlternativeDefaultFont = null;
    }

    /// <summary>
    /// Sets the default font to the SadConsole extended font, an IBM 8x16 font with SadConsole specific characters past index 255.
    /// </summary>
    public void UseBuiltinFontExtended()
    {
        UseExtendedFont = true;
        AlternativeDefaultFont = null;
    }

    /// <summary>
    /// Sets the default font in SadConsole to the specified font file.
    /// </summary>
    /// <param name="fontFile"></param>
    public void UseCustomFont(string fontFile) =>
        AlternativeDefaultFont = fontFile;

    /// <summary>
    /// Loads the provided font files into SadConsole.
    /// </summary>
    /// <param name="fontFiles">An array of font files to load.</param>
    public void AddExtraFonts(params string[] fontFiles) =>
        CustomFonts = fontFiles;

    /// <summary>
    /// Invokes the <see cref="FontLoader"/> delegate.
    /// </summary>
    /// <param name="config">The builder running this configurator.</param>
    /// <param name="game">The game being created.</param>
    public void Run(Builder config, Game game)
    {
        FontLoader(this, game);
        Settings.UseDefaultExtendedFont = UseExtendedFont;
    }
}
