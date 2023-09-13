using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole;

public sealed partial class Game
{
    /// <summary>
    /// An object used during the creation of the game to configure SadConsole and MonoGame.
    /// </summary>
    public class Configuration
    {
        internal ConfigurationFontLoader FontLoaderData;

        internal Action<ConfigurationFontLoader> FontLoader { get; set; }
        internal Func<Game, IScreenObject> GenerateStartingObject { get; set; }
        internal Func<Game, IScreenSurface[]> GenerateSplashScreen { get; set; }

        internal bool FocusStartingObject { get; set; }

        internal int ScreenSizeWidth { get; set; }
        internal int ScreenSizeHeight { get; set; }

        internal Action? OnStartCallback { get; set; }
        internal Action? OnEndCallback { get; set; }

        internal EventHandler<GameHost> event_FrameUpdate { get; private set; }
        internal EventHandler<GameHost> event_FrameRender { get; private set; }

        // MonoGame host-specific settings
        internal bool UseUnlimitedFPSVisual { get; set; }
        internal Action<Host.Game> MonoGameCtorCallback { get; set; }
        internal Action<Host.Game> MonoGameInitCallback { get; set; }

        internal bool CreateStartingConsole { get; set; }

        /// <summary>
        /// Creates a new configuration object with a screen size of 80x25 and the default SadConsole font.
        /// </summary>
        public Configuration()
        {
            FontLoaderData = new ConfigurationFontLoader();
            CreateStartingConsole = true;
            FocusStartingObject = true;
            SetScreenSize(80, 25);
            ConfigureFonts(loader =>
            {
                loader.UseBuiltinFont();
            });
        }

        internal void RunFontConfig() =>
            FontLoader(FontLoaderData);


        /// <summary>
        /// Configures which default font to use during game startup, as well as which other fonts to load for the game.
        /// </summary>
        /// <param name="fontLoader">A method that provides the <see cref="ConfigurationFontLoader"/> object to load fonts.</param>
        /// <returns>The configuration object.</returns>
        public Configuration ConfigureFonts(Action<ConfigurationFontLoader> fontLoader)
        {
            FontLoader = fontLoader;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="SadConsole.GameHost.Screen"/> property to the specified type.
        /// </summary>
        /// <typeparam name="TRootObject">A parameterless <see cref="IScreenObject"/> object.</typeparam>
        /// <returns>The configuration object.</returns>
        public Configuration SetStartingScreen<TRootObject>() where TRootObject : IScreenObject, new()
        {
            CreateStartingConsole = false;
            GenerateStartingObject = _ => new TRootObject();
            return this;
        }

        /// <summary>
        /// Sets the <see cref="SadConsole.GameHost.Screen"/> property to the return value of the <paramref name="creator"/> parameter.
        /// </summary>
        /// <param name="creator">A method that returns an object as the starting screen.</param>
        public Configuration SetStartingScreen(Func<Game, IScreenObject> creator)
        {
            CreateStartingConsole = false;
            GenerateStartingObject = creator;
            return this;
        }

        /// <summary>
        /// Sets the starting screen's <see cref="IScreenObject.IsFocused"/> property to the provided value, when it's created.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The configuration object.</returns>
        public Configuration IsStartingScreenFocused(bool value)
        {
            FocusStartingObject = value;
            return this;
        }

        /// <summary>
        /// Sets the initial screen size of the window, in cells.
        /// </summary>
        /// <param name="width">The cell count along the x-axis.</param>
        /// <param name="height">The cell count along the y-axis.</param>
        /// <returns>The configuration object.</returns>
        public Configuration SetScreenSize(int width, int height)
        {
            (ScreenSizeWidth, ScreenSizeHeight) = (width, height);
            return this;
        }

        /// <summary>
        /// Sets a method to run after SadConsole is initialized but before the game loop has started.
        /// </summary>
        /// <param name="onStartCallback">A method.</param>
        /// <returns>The configuration object.</returns>
        public Configuration OnStart(Action onStartCallback)
        {
            OnStartCallback = onStartCallback;
            return this;
        }

        /// <summary>
        /// Sets a method to run after SadConsole the game loop has ended.
        /// </summary>
        /// <param name="onEndCallback">A method.</param>
        /// <returns>The configuration object.</returns>
        public Configuration OnEnd(Action onEndCallback)
        {
            OnEndCallback = onEndCallback;
            return this;
        }

        /// <summary>
        /// Sets an event handler for the <see cref="GameHost.FrameUpdate"/> event.
        /// </summary>
        /// <param name="instance_FrameUpdate">The event handler</param>
        /// <returns>The configuration object.</returns>
        public Configuration UseFrameUpdateEvent(EventHandler<GameHost> instance_FrameUpdate)
        {
            this.event_FrameUpdate = instance_FrameUpdate;
            return this;
        }

        /// <summary>
        /// Sets an event handler for the <see cref="GameHost.FrameRender"/> event.
        /// </summary>
        /// <param name="instance_FrameRender">The event handler</param>
        /// <returns>The configuration object.</returns>
        public Configuration UseFrameRenderEvent(EventHandler<GameHost> instance_FrameRender)
        {
            this.event_FrameRender = instance_FrameRender;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Settings.UnlimitedFPS"/> setting to <see langword="true"/>.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public Configuration UseUnlimitedFPS()
        {
            Settings.UnlimitedFPS = true;
            return this;
        }

        /// <summary>
        /// Adds a MonoGame game component that renders the frames per-second of the game.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public Configuration ShowMonoGameFPS()
        {
            UseUnlimitedFPSVisual = true;
            return this;
        }

        /// <summary>
        /// The <paramref name="monogameCtorCallback"/> method is called by the MonoGame constructor. Some MonoGame specific settings may only be settable via the constructor.
        /// </summary>
        /// <param name="monogameCtorCallback">A method.</param>
        /// <returns>The configuration object.</returns>
        public Configuration WithMonoGameCtor(Action<Host.Game> monogameCtorCallback)
        {
            MonoGameCtorCallback = monogameCtorCallback;
            return this;
        }

        /// <summary>
        /// Internal only. Called by the MonoGame game to finish configuring SadConsole.
        /// </summary>
        /// <param name="monogameInitCallback">A method.</param>
        /// <returns>The configuration object.</returns>
        internal Configuration WithMonoGameInit(Action<Host.Game> monogameInitCallback)
        {
            MonoGameInitCallback = monogameInitCallback;
            return this;
        }


        /// <summary>
        /// Sets the startup splash screen to the specified object.
        /// </summary>
        /// <typeparam name="TSplashScreen">A parameterless <see cref="IScreenSurface"/> object.</typeparam>
        /// <returns>The configuration object.</returns>
        public Configuration SetSplashScreen<TSplashScreen>() where TSplashScreen : IScreenSurface, new()
        {
            CreateStartingConsole = false;
            GenerateSplashScreen = _ => new IScreenSurface[] { new TSplashScreen() };
            return this;
        }

        /// <summary>
        /// A method callback to generate splash screen objects to the return value of the <paramref name="creator"/> parameter.
        /// </summary>
        /// <param name="creator">A method that returns an array of screens to be used as the splash screens.</param>
        public Configuration SetSplashScreen(Func<Game, ScreenSurface[]> creator)
        {
            CreateStartingConsole = false;
            GenerateSplashScreen = creator;
            return this;
        }
    }

    /// <summary>
    /// Settings for loading fonts at the start of a game.
    /// </summary>
    public class ConfigurationFontLoader
    {
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
    }
}
