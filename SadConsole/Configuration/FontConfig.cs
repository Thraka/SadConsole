#nullable enable

using System;

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Configures which default font to use during game startup, as well as which other fonts to load for the game.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="fontLoader">A method that provides access to the <see cref="FontConfig"/> object that loads fonts.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder ConfigureFonts(this Builder configBuilder, Action<FontConfig, GameHost> fontLoader)
    {
        FontConfig config = configBuilder.GetOrCreateConfig<FontConfig>();

        config.FontLoader = fontLoader;

        return configBuilder;
    }

    /// <summary>
    /// Configures SadConsole to use the specified font file as the default font.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="customDefaultFont">Creates the font config for SadConsole using this font file as the default.</param>
    /// <param name="extraFonts">Extra fonts to load into SadConsole.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder ConfigureFonts(this Builder configBuilder, string customDefaultFont, string[]? extraFonts = null)
    {
        FontConfig config = configBuilder.GetOrCreateConfig<FontConfig>();

        if (extraFonts != null)
            config.AddExtraFonts(extraFonts);
        else
            config.CustomFonts = Array.Empty<string>();

        config.UseCustomFont(customDefaultFont);

        return configBuilder;
    }

    /// <summary>
    /// Configures SadConsole to use the built in default fonts.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="useExtendedDefault">When <see langword="true"/>, SadConsole sets the default font to <see cref="GameHost.EmbeddedFontExtended"/>; otherwise <see cref="GameHost.EmbeddedFont"/> is used.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder ConfigureFonts(this Builder configBuilder, bool useExtendedDefault = false)
    {
        FontConfig config = configBuilder.GetOrCreateConfig<FontConfig>();

        if (useExtendedDefault)
            config.UseBuiltinFontExtended();
        else
            config.UseBuiltinFont();

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

    /// <summary>
    /// Sets the default font size for the game. The default is <see cref="IFont.Sizes.One"/>.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="size">The size of the default font.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder SetDefaultFontSize(this Builder configBuilder, IFont.Sizes size)
    {
        FontConfig config = configBuilder.GetOrCreateConfig<FontConfig>();
        config.SetDefaultFontSize(size);
        return configBuilder;
    }
}

internal class OldFontNameConfig : IConfigurator
{
    public void Run(Builder config, GameHost game)
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

/// <summary>
/// The config settings for loading the default fonts when the game starts.
/// </summary>
public class FontConfig : IConfigurator
{
    /// <summary>
    /// The method invoked by the <see cref="GameHost"/> as fonts are loaded.
    /// </summary>
    public Action<FontConfig, GameHost>? FontLoader { get; set; }

    /// <summary>
    /// An array of custom font files to load into SadConsole. The default is an empty array.
    /// </summary>
    public string[] CustomFonts = Array.Empty<string>();

    /// <summary>
    /// The default font to use when the game starts. A <see langword="null"/> value indicates that the default font is the built-in font.
    /// </summary>
    public string? AlternativeDefaultFont = null;

    /// <summary>
    /// When <see langword="true"/>, the default font is the SadConsole extended font. The default is <see langword="false"/>.
    /// </summary>
    public bool UseExtendedFont = false;

    /// <summary>
    /// The size of font to use with the default font. The default is <see cref="IFont.Sizes.One"/>.
    /// </summary>
    public IFont.Sizes DefaultFontSize = IFont.Sizes.One;

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
        CustomFonts = (string[])fontFiles.Clone();

    /// <summary>
    /// Sets the default font size for the game. The default is <see cref="IFont.Sizes.One"/>.
    /// </summary>
    /// <param name="size">The default font size.</param>
    public void SetDefaultFontSize(IFont.Sizes size) =>
        DefaultFontSize = size;

    /// <summary>
    /// Invokes the <see cref="FontLoader"/> delegate.
    /// </summary>
    /// <param name="config">The builder running this configurator.</param>
    /// <param name="game">The game being created.</param>
    public void Run(Builder config, GameHost game)
    {
        FontLoader?.Invoke(this, game);
        Settings.UseDefaultExtendedFont = UseExtendedFont;
    }
}
