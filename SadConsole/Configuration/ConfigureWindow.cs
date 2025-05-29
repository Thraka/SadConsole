using System;
using SadRogue.Primitives;

namespace SadConsole.Configuration;

public static partial class Extensions
{
    /// <summary>
    /// Configures the game window.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="loader">The method to invoke when this is run.</param>
    /// <returns>The configuration builder.</returns>
    public static Builder ConfigureWindow(this Builder configBuilder, Action<ConfigureWindowConfig, BuilderBase, GameHost> loader)
    {
        ConfigureWindowConfig config = configBuilder.GetOrCreateConfig<ConfigureWindowConfig>();

        config.Loader = loader;

        return configBuilder;
    }

    /// <summary>
    /// Sets the initial screen size of the window, in cells.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="width">The cell count along the x-axis.</param>
    /// <param name="height">The cell count along the y-axis.</param>
    /// <returns>The configuration builder.</returns>
    [Obsolete($"Use {nameof(SetWindowSizeInCells)} instead.")]
    public static Builder SetScreenSize(this Builder configBuilder, int width, int height) =>
        SetWindowSizeInCells(configBuilder, width, height, false);

    /// <summary>
    /// Sets the desired resolution of the SadConsole window in cells using the default font.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="width">How wide the window is in font cells.</param>
    /// <param name="height">How tall the window is in font cells.</param>
    /// <param name="zoom">When true, creates a larger window to automatically zoom the contents.</param>
    /// <remarks>
    /// The <paramref name="width"/> and <paramref name="height"/> values are used to generate the game window based
    /// on how large the font is. For example, if the font is 10x8, and you request window size of 20x20, a window is
    /// created with a width of 10 * 20 and a height of 8 * 20 in pixels, which is 200x160.
    ///
    /// When <paramref name="zoom"/> is set to true, the window will try to get as big as possible while keeping
    /// the SadConsole output crisp.
    /// </remarks>
    public static Builder SetWindowSizeInCells(this Builder configBuilder, int width, int height, bool zoom = false)
    {
        configBuilder.ConfigureWindow((screenConfig, configBuilder, host) =>
        {
            screenConfig.SetWindowSizeInCells(width, height, zoom);
        });

        return configBuilder;
    }

    /// <summary>
    /// Sets the desired resolution of the SadConsole window in pixels. The output surface and initial cell counts are calculated based on
    /// the current font, and set to a value that allows the most cells to appear on screen.
    /// </summary>
    /// <param name="configBuilder">The builder object that composes the game startup.</param>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    public static Builder SetWindowSizeInPixels(this Builder configBuilder, int width, int height)
    {
        configBuilder.ConfigureWindow((screenConfig, configBuilder, host) =>
        {
            screenConfig.SetWindowSizeInPixels(width, height);
        });

        return configBuilder;
    }
}

/// <summary>
/// Holds the config state for the window.
/// </summary>
public sealed class ConfigureWindowConfig : IConfigurator
{
    /// <summary>
    /// The desired width of the game window in pixels.
    /// </summary>
    public int WindowWidthInPixels { get; set; }

    /// <summary>
    /// The desired height of the game window in pixels.
    /// </summary>
    public int WindowHeightInPixels { get; set; }

    /// <summary>
    /// The rendering width of the game.
    /// </summary>
    public int GameResolutionWidthInPixels { get; set; }

    /// <summary>
    /// The rendering height of the game.
    /// </summary>
    public int GameResolutionHeightInPixels { get; set; }

    /// <summary>
    /// The amount of cells that fit in the window along the x-axis.
    /// </summary>
    public int CellsX { get; set; }

    /// <summary>
    /// The amount of cells that fit in the window along the y-axis.
    /// </summary>
    public int CellsY { get; set; }

    /// <summary>
    /// When true, tells the game host to run in fullscreen mode.
    /// </summary>
    public bool Fullscreen { get; set; } = false;

    /// <summary>
    /// When true, fullscreen mode uses a borderless window.
    /// </summary>
    public bool BorderlessWindowedFullscreen { get; set; } = true;

    /// <summary>
    /// Sets the desired resolution of the SadConsole window in pixels. The output surface and initial cell counts are calculated based on
    /// the current font, and set to a value that allows the most cells to appear on screen.
    /// </summary>
    /// <param name="width">The width of the window.</param>
    /// <param name="height">The height of the window.</param>
    public void SetWindowSizeInPixels(int width, int height)
    {
        WindowWidthInPixels = width;
        WindowHeightInPixels = height;
        GameResolutionWidthInPixels = width;
        GameResolutionHeightInPixels = height;
        Point fontSize = _gameHost!.DefaultFont.GetFontSize(_gameHost.DefaultFontSize);
        CellsX = width / fontSize.X;
        CellsY = height / fontSize.Y;
    }

    /// <summary>
    /// Sets the desired resolution of the SadConsole window in cells using the default font.
    /// </summary>
    /// <param name="width">How wide the window is in font cells.</param>
    /// <param name="height">How tall the window is in font cells.</param>
    /// <param name="zoom">When true, creates a larger window to automatically zoom the contents.</param>
    /// <remarks>
    /// The <paramref name="width"/> and <paramref name="height"/> values are used to generate the game window based
    /// on how large the font is. For example, if the font is 10x8, and you request window size of 20x20, a window is
    /// created with a width of 10 * 20 and a height of 8 * 20 in pixels, which is 200x160.
    ///
    /// When <paramref name="zoom"/> is set to true, the window will try to get as big as possible while keeping
    /// the SadConsole output crisp.
    /// </remarks>
    public void SetWindowSizeInCells(int width, int height, bool zoom)
    {
        Point fontPixels = _gameHost!.DefaultFont.GetFontSize(_gameHost.DefaultFontSize);

        CellsX = width;
        CellsY = height;

        WindowWidthInPixels = width * fontPixels.X;
        WindowHeightInPixels = height * fontPixels.Y;
        GameResolutionWidthInPixels = WindowWidthInPixels;
        GameResolutionHeightInPixels = WindowHeightInPixels;

        if (zoom)
        {
            GetDeviceScreenSize(out int maxWidth, out int maxHeight);

            int multiplier = Math.Min(maxWidth / WindowWidthInPixels, maxHeight / WindowHeightInPixels);

            if (multiplier > 0)
            {
                WindowWidthInPixels = WindowWidthInPixels * multiplier;
                WindowHeightInPixels = WindowHeightInPixels * multiplier;
            }

        }
    }

    /// <summary>
    /// Gets the size of the device screen in pixels.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void GetDeviceScreenSize(out int width, out int height) =>
        _gameHost!.GetDeviceScreenSize(out width, out height);

    /// <summary>
    /// Validates that the window fits on the display.
    /// </summary>
    /// <returns>True when the <see cref="WindowWidthInPixels"/> and <see cref="WindowHeightInPixels"/> are
    /// less than or equal to the screen size; otherwise, false.</returns>
    public bool IsWindowSizeValid()
    {
        GetDeviceScreenSize(out int width, out int height);

        return WindowWidthInPixels < width && WindowHeightInPixels < height;
    }

    #region Configuration
    /// <summary>
    /// The method invoked by the <see cref="GameHost"/> as fonts are loaded.
    /// </summary>
    internal Action<ConfigureWindowConfig, BuilderBase, GameHost>? Loader { get; set; }

    private GameHost? _gameHost;

    void IConfigurator.Run(BuilderBase configBuilder, GameHost game)
    {
        _gameHost = game;
        Loader?.Invoke(this, configBuilder, game);
        _gameHost = null;
    }
    #endregion
}
