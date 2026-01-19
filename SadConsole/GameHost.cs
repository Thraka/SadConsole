using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// Represents the SadConsole game engine.
/// </summary>
/// <remarks>
/// When a new host is created, the host should do the following:
///
/// - Run `LoadDefaultFonts`.
/// - Run `SetRenderer` for `window`, `controls`, `layered`, and `default` renderers.
/// - Run `LoadMappedColors`.
/// - Configure the `Screen` to a new console with `ScreenCellsX` and `ScreenCellsY`.
/// - Prior to running the game, run `SplashScreens.SplashScreenManager.CheckRun()`.
/// </remarks>
public abstract partial class GameHost : IDisposable
{
    private GlobalState? _state;
    private IFont _defaultFont;
    private IFont.Sizes _defaultFontSize = IFont.Sizes.One;

    /// <summary>
    /// Holds all of the <see cref="IRenderer"/> types.
    /// </summary>
    protected Dictionary<string, System.Type> _renderers = new(5);

    /// <summary>
    /// Holds all of the <see cref="IRenderStep"/> types.
    /// </summary>
    protected Dictionary<string, System.Type> _rendererSteps = new(5);

    /// <summary>
    /// The splash screens to show on game startup.
    /// </summary>
    protected internal Queue<IScreenSurface>? _splashScreens { get; set; } = new Queue<IScreenSurface>();

    /// <summary>
    /// Instance of the game host.
    /// </summary>
    public static GameHost Instance { get; protected set; } = null!;

    /// <summary>
    /// Contains the path to a file being serialized or deserialized.
    /// </summary>
    public static string SerializerPathHint { get; internal set; } = string.Empty;

    /// <summary>
    /// Raised when the game draws a frame to the screen.
    /// </summary>
    public event EventHandler<GameHost>? FrameRender;

    /// <summary>
    /// Raised when the game updates prior to drawing a frame.
    /// </summary>
    public event EventHandler<GameHost>? FrameUpdate;

    /// <summary>
    /// A callback to run before the <see cref="Run"/> method is called;
    /// </summary>
    public event EventHandler<GameHost>? Started;

    /// <summary>
    /// A callback to run after the <see cref="Run"/> method is called;
    /// </summary>
    public event EventHandler<GameHost>? Ending;

    /// <summary>
    /// Raised when the <see cref="DefaultFont"/> property changes.
    /// </summary>
    public event EventHandler<FontChangedEventArgs>? DefaultFontChanged;

    /// <summary>
    /// Raised when the <see cref="DefaultFontSize"/> property changes.
    /// </summary>
    public event EventHandler<FontSizeChangedEventArgs>? DefaultFontSizeChanged;

    /// <summary>
    /// Draw calls registered for the next drawing frame.
    /// </summary>
    public Queue<DrawCalls.IDrawCall> DrawCalls { get; } = new Queue<DrawCalls.IDrawCall>();

    /// <summary>
    /// How many cells fit in the render area width used by SadConsole.
    /// </summary>
    public int ScreenCellsX => Settings.Rendering.RenderWidth / DefaultFont.GetFontSize(DefaultFontSize).X;

    /// <summary>
    /// How many cells fit in the render area width used by SadConsole.
    /// </summary>
    public int ScreenCellsY => Settings.Rendering.RenderHeight / DefaultFont.GetFontSize(DefaultFontSize).Y;

    /// <summary>
    /// A frame number counter, incremented every game frame.
    /// </summary>
    public int FrameNumber { get; set; }

    /// <summary>
    /// The date and time the game was started.
    /// </summary>
    protected DateTime _gameStartedAt = DateTime.Now;

    /// <summary>
    /// Collection of fonts. Used mainly by the deserialization system.
    /// </summary>
    public Dictionary<string, IFont> Fonts { get; } = new Dictionary<string, IFont>();

    /// <summary>
    /// The font automatically loaded by SadConsole. Standard IBM style font.
    /// </summary>
    public SadFont EmbeddedFont { get; internal set; } = null!;

    /// <summary>
    /// The font automatically loaded by SadConsole. Standard IBM style font. Extended with extra SadConsole characters.
    /// </summary>
    public SadFont EmbeddedFontExtended { get; internal set; } = null!;

    /// <summary>
    /// The default font for any type that does not provide a font.
    /// </summary>
    public IFont DefaultFont
    {
        get => _defaultFont;
        set
        {
            if (_defaultFont == value) return;

            IFont oldFont = _defaultFont;
            _defaultFont = value ?? throw new NullReferenceException("The default font can't be set to a null value.");

            OnDefaultFontChanged(new FontChangedEventArgs(oldFont, _defaultFont));
        }
    }

    /// <summary>
    /// The default font size to use with the <see cref="DefaultFont"/>.
    /// </summary>
    public IFont.Sizes DefaultFontSize
    {
        get => _defaultFontSize;
        set
        {
            if (_defaultFontSize == value) return;

            IFont.Sizes oldSize = _defaultFontSize;
            _defaultFontSize = value;

            OnDefaultFontSizeChanged(new FontSizeChangedEventArgs(oldSize, _defaultFontSize));
        }
    }

    /// <summary>
    /// Global keyboard object used by SadConsole during the update frame.
    /// </summary>
    public Input.Keyboard Keyboard { get; } = new Input.Keyboard();

    /// <summary>
    /// Global mouse object used by SadConsole during the update frame.
    /// </summary>
    public Input.Mouse Mouse { get; } = new Input.Mouse();

    /// <summary>
    /// The elapsed time between now and the last update call.
    /// </summary>
    public TimeSpan UpdateFrameDelta { get; set; }

    /// <summary>
    /// The elapsed time between now and the last draw call.
    /// </summary>
    public TimeSpan DrawFrameDelta { get; set; }

    /// <summary>
    /// The total time the game has been running.
    /// </summary>
    public TimeSpan GameRunningTotalTime => DateTime.Now - _gameStartedAt;

    /// <summary>
    /// The console created by the game and automatically assigned to <see cref="Screen"/>.
    /// </summary>
    public Console? StartingConsole { get; protected internal set; }

    /// <summary>
    /// The active screen processed by the game.
    /// </summary>
    public IScreenObject? Screen { get; set; }

    /// <summary>
    /// Update components that run before the <see cref="Screen"/> is processed.
    /// </summary>
    public List<Components.RootComponent> RootComponents { get; set; } = new();

    /// <summary>
    /// The stack of focused consoles used by the mouse and keyboard.
    /// </summary>
    public FocusedScreenObjectStack FocusedScreenObjects { get; set; } = new FocusedScreenObjectStack();

    /// <summary>
    /// A global random number generator.
    /// </summary>
    public Random Random { get; set; } = new Random();

    /// <summary>
    /// Raises the <see cref="Started"/> event.
    /// </summary>
    protected virtual void OnGameStarted() =>
        Started?.Invoke(this, this);

    /// <summary>
    /// Raises the <see cref="Ending"/> event.
    /// </summary>
    protected virtual void OnGameEnding() =>
        Ending?.Invoke(this, this);

    /// <summary>
    /// Raises the <see cref="FrameRender"/> event.
    /// </summary>
    protected virtual void OnFrameRender() =>
        FrameRender?.Invoke(this, this);

    /// <summary>
    /// Raises the <see cref="FrameUpdate"/> event.
    /// </summary>
    protected virtual void OnFrameUpdate()
    {
        FrameNumber += 1;
        FrameUpdate?.Invoke(this, this);
    }

    /// <summary>
    /// Runs the game.
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Gets a texture from the implemented host.
    /// </summary>
    /// <param name="resourcePath">The path to the texture to load.</param>
    /// <returns>The texture from the game host.</returns>
    public abstract ITexture GetTexture(string resourcePath);

    /// <summary>
    /// Gets a texture from the implemented host.
    /// </summary>
    /// <param name="textureStream">A stream containing the texture.</param>
    /// <returns>The texture from the game host.</returns>
    public abstract ITexture GetTexture(Stream textureStream);

    /// <summary>
    /// Creates a texture.
    /// </summary>
    /// <param name="width">The width of the texture in pixels.</param>
    /// <param name="height">The height of the texture in pixels.</param>
    /// <returns>The texture from the game host.</returns>
    public abstract ITexture CreateTexture(int width, int height);

    /// <summary>
    /// Creates and returns an <see cref="IRenderer"/> by name.
    /// </summary>
    /// <param name="name">The name of the renderer.</param>
    /// <returns>A new renderer.</returns>
    public virtual IRenderer? GetRenderer(string name)
    {
        if (name.Equals(Renderers.Constants.RendererNames.None, StringComparison.OrdinalIgnoreCase)) return null;

        IRenderer? result = null;

        if (_renderers.TryGetValue(name, out Type? objType))
            result = Activator.CreateInstance(objType) as IRenderer
                ?? throw new NullReferenceException($"Renderer was found registered, but the system was unable to create an instance of it as an {nameof(IRenderer)}.");

        if (result is null) throw new KeyNotFoundException($"Renderer '{name}' isn't registered with the host.");

        result.Name = name;

        return result;
    }

    /// <summary>
    /// Sets the default <see cref="IRenderer"/> for a type.
    /// </summary>
    /// <param name="name">The name to register the renderer as.</param>
    /// <param name="rendererType">The renderer type.</param>
    public void SetRenderer(string name, System.Type rendererType) =>
        _renderers[name] = rendererType;

    /// <summary>
    /// Sets the default <see cref="IRenderStep"/> for a type.
    /// </summary>
    /// <param name="name">The name to register the render step as.</param>
    /// <param name="rendererStepType">The render step type.</param>
    public void SetRendererStep(string name, System.Type rendererStepType) =>
        _rendererSteps[name] = rendererStepType;

    /// <summary>
    /// Creates and returns a <see cref="IRenderStep"/> by name.
    /// </summary>
    /// <param name="name">The name of the renderer.</param>
    /// <returns>A new renderer.</returns>
    public virtual IRenderStep GetRendererStep(string name)
    {
        if (_rendererSteps.TryGetValue(name, out Type? objType))
            return Activator.CreateInstance(objType) as IRenderStep
                ?? throw new NullReferenceException($"Render step was found registered, but the system was unable to create an instance of it as an {nameof(IRenderStep)}.");

        throw new KeyNotFoundException("RenderStep not found.");
    }

    /// <summary>
    /// Gets the state of the keyboard from the implemented host.
    /// </summary>
    /// <returns>The state of the keyboard.</returns>
    public abstract IKeyboardState GetKeyboardState();

    /// <summary>
    /// Gets the state of the mouse from the implemented host.
    /// </summary>
    /// <returns>The state of the mouse.</returns>
    public abstract IMouseState GetMouseState();

    /// <summary>
    /// Gets the size of the current device's screen in pixels.
    /// </summary>
    /// <param name="width">The width of the screen.</param>
    /// <param name="height">The height of the screen.</param>
    public abstract void GetDeviceScreenSize(out int width, out int height);

    /// <summary>
    /// The splash screens the game should sequentially show on startup.
    /// </summary>
    /// <param name="surfaces">The splash screens to show.</param>
    public void SetSplashScreens(params IScreenSurface[] surfaces) =>
        _splashScreens = new Queue<IScreenSurface>(surfaces);

    /// <summary>
    /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
    /// </summary>
    /// <param name="font">The font file to load.</param>
    /// <returns>A master font that you can generate a usable font from.</returns>
    public IFont LoadFont(string font)
    {
        try
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
            };

            IFont masterFont;

            try
            {
                masterFont = SadConsole.Serializer.Load<IFont>(font, false, settings);
            }
            catch (JsonSerializationException j)
            {
                throw new JsonSerializationException("Unable to load font. You either have a malformed json file, or you're missing the $type declaration as the first entry of the json content:\n \"$type\": \"SadConsole.SadFont, SadConsole\",", j);
            }

            if (Fonts.TryGetValue(masterFont.Name, out IFont? value))
                return value;

            Fonts.Add(masterFont.Name, masterFont);

            return masterFont;
        }
        catch (System.Runtime.Serialization.SerializationException)
        {

            throw;
        }
    }

    /// <summary>
    /// Destroys the <see cref="StartingConsole"/> instance.
    /// </summary>
    /// <remarks>
    /// Prior to calling this method, you must set <see cref="Screen"/> to an object other than <see cref="StartingConsole"/>.
    /// </remarks>
    public void DestroyDefaultStartingConsole()
    {
        if (StartingConsole == null) return;

        if (Screen == StartingConsole)
            throw new Exception($"{nameof(Screen)} must be reassigned to a new instance before this method can be called.");

        StartingConsole.IsFocused = false;
        StartingConsole.Dispose();
        StartingConsole = null;
    }

    /// <summary>
    /// Opens a file stream with the specified mode and access.
    /// </summary>
    /// <param name="file">The file to open.</param>
    /// <param name="mode">The mode for opening. Defaults to <see cref="FileMode.Open"/>.</param>
    /// <param name="access">The type of access for the stream. Defaults to <see cref="FileAccess.Read"/>.</param>
    /// <returns>The stream object.</returns>
    public virtual Stream OpenStream(string file, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) =>
        File.Open(file, mode, access);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns><see langword="true"/> if the file exists; otherwise <see langword="false"/>.</returns>
    public virtual bool FileExists(string file) =>
        File.Exists(file);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="file">The file to delete.</param>
    /// <returns><see langword="true"/> if the file was deleted; otherwise <see langword="false"/>.</returns>
    public virtual bool FileDelete(string file)
    {
        try
        {
            File.Delete(file);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Loads the embedded <c>IBM.font</c> files. Sets the <see cref="DefaultFont"/> property.
    /// </summary>
    /// <param name="defaultFont">An optional font to load and set as the default.</param>
    /// <remarks>
    /// If <paramref name="defaultFont"/> is <see langword="null"/>, the <see cref="EmbeddedFont"/> or <see cref="EmbeddedFontExtended"/> font is set based on the value of <see cref="Settings.UseDefaultExtendedFont"/>.
    /// </remarks>
    protected void LoadDefaultFonts(string? defaultFont)
    {
        // Load the embedded fonts.
        EmbeddedFont = LoadResourceFont("SadConsole.Resources.IBM.font");
        EmbeddedFontExtended = LoadResourceFont("SadConsole.Resources.IBM_ext.font");

        // Configure default font
        if (string.IsNullOrEmpty(defaultFont))
            if (Settings.UseDefaultExtendedFont)
                DefaultFont = EmbeddedFontExtended;
            else
                DefaultFont = EmbeddedFont;
        else
            DefaultFont = LoadFont(defaultFont);

        // Local method to load a font from the built in assembly resource
        SadFont LoadResourceFont(string fontName)
        {
            System.Reflection.Assembly assembly = typeof(SadConsole.SadFont).Assembly;

            using Stream stream = assembly.GetManifestResourceStream(fontName)!;
            using StreamReader sr = new(stream);

            SerializerPathHint = "";

            var masterFont = (SadFont)Newtonsoft.Json.JsonConvert.DeserializeObject(
                sr.ReadToEnd(),
                typeof(SadFont),
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
                    Converters = null!
                })!;

            Fonts.Add(masterFont.Name, masterFont);

            return masterFont;
        }
    }

    /// <summary>
    /// Uses reflection to examine the <see cref="Color"/> type and add any predefined colors into <see cref="ColorExtensions2.ColorMappings"/>.
    /// </summary>
    protected static void LoadMappedColors()
    {
        //ColorExtensions.ColorMappings.Add
        Type colorType = typeof(Color);
        foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) => t.FieldType.Name == colorType.Name))
            ColorExtensions2.ColorMappings.Add(item.Name.ToLower(), (Color)item.GetValue(null)!);
    }

    /// <summary>
    /// Resizes the window to the specified dimensions.
    /// </summary>
    /// <param name="width">The width of the window in pixels.</param>
    /// <param name="height">The height of the window in pixels.</param>
    /// <param name="resizeOutputSurface">When <see langword="true"/> resizes the screen output surface along with the window. Defaults to <see langword="false"/></param>
    public abstract void ResizeWindow(int width, int height, bool resizeOutputSurface = false);

    /// <summary>
    /// Resizes the window to the specified cell count along the X-axis and Y-axis.
    /// </summary>
    /// <param name="cellsX">The number of cells to fit horizontally.</param>
    /// <param name="cellsY">The number of cells to fit vertically.</param>
    /// <param name="cellSize">The size of the cells in pixels.</param>
    /// <param name="resizeOutputSurface">When <see langword="true"/> resizes the screen output surface along with the window. Defaults to <see langword="false"/></param>
    public void ResizeWindow(int cellsX, int cellsY, Point cellSize, bool resizeOutputSurface = false) =>
        ResizeWindow(cellsX * cellSize.X, cellsY * cellSize.Y, resizeOutputSurface);

    /// <summary>
    /// Saves the global state, mainly the <see cref="FocusedScreenObjects"/> and <see cref="Screen"/> objects.
    /// </summary>
    public void SaveGlobalState()
    {
        _state = new GlobalState(FocusedScreenObjects, Screen, DefaultFont, DefaultFontSize);
    }

    /// <summary>
    /// Restores the global state that was saved with <see cref="SaveGlobalState"/>.
    /// </summary>
    public void RestoreGlobalState()
    {
        if (_state == null) return;

        FocusedScreenObjects = _state.FocusedScreenObjects;
        Screen = _state.Screen;
        DefaultFont = _state.DefaultFont;
        DefaultFontSize = _state.DefaultFontSize;

        _state = null;
    }

    #region IDisposable Support
    private bool _disposedValue = false; // To detect redundant calls

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (IFont font in Fonts.Values)
                    font.Image.Dispose();

                EmbeddedFont.Image.Dispose();
                EmbeddedFontExtended.Image.Dispose();
                DefaultFont.Image.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes this object.
    /// </summary>
    ~GameHost() =>
        Dispose(false);

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    private class GlobalState
    {
        public FocusedScreenObjectStack FocusedScreenObjects;
        public IScreenObject? Screen;
        public IFont DefaultFont;
        public IFont.Sizes DefaultFontSize;

        public GlobalState(FocusedScreenObjectStack focusedScreenObjects, IScreenObject? screen, IFont defaultFont, IFont.Sizes defaultFontSize)
        {
            FocusedScreenObjects = focusedScreenObjects;
            Screen = screen;
            DefaultFont = defaultFont;
            DefaultFontSize = defaultFontSize;
        }
    }

    /// <summary>
    /// Raises the <see cref="DefaultFontChanged"/> event and updates existing screen objects that are using the old default font.
    /// </summary>
    /// <param name="e">Event args containing the old and new font references.</param>
    protected virtual void OnDefaultFontChanged(FontChangedEventArgs e)
    {
        DefaultFontChanged?.Invoke(this, e);

        // Update all existing screen objects that were using the old default font
        UpdateScreenObjectFonts(Screen, e.OldFont, e.NewFont);

        // re-layout all of the children
        Screen?.UpdateAbsolutePosition();
    }

    /// <summary>
    /// Raises the <see cref="DefaultFontSizeChanged"/> event and updates existing screen objects that are using the old default font size.
    /// </summary>
    /// <param name="e">Event args containing the old and new font size.</param>
    protected virtual void OnDefaultFontSizeChanged(FontSizeChangedEventArgs e)
    {
        DefaultFontSizeChanged?.Invoke(this, e);

        // Update all existing screen objects that were using the old default font size
        UpdateScreenObjectFontSizes(Screen, e.OldSize, e.NewSize);

        // re-layout all of the children
        Screen?.UpdateAbsolutePosition();
    }

    /// <summary>
    /// Recursively updates the font of screen objects that are using the old default font.
    /// </summary>
    /// <param name="obj">The screen object to check and update.</param>
    /// <param name="oldFont">The old default font to check for.</param>
    /// <param name="newFont">The new default font to apply.</param>
    private void UpdateScreenObjectFonts(IScreenObject? obj, IFont oldFont, IFont newFont)
    {
        if (obj == null) return;

        // If this is a screen surface and it's using the old default font, update it
        if (obj is IScreenSurface surface && ReferenceEquals(surface.Font, oldFont))
        {
            Point currentPixelSize = surface.FontSize;
            
            // Try to determine which IFont.Sizes enum was used with the old font
            IFont.Sizes? detectedSize = null;
            foreach (IFont.Sizes size in Enum.GetValues<IFont.Sizes>())
            {
                if (oldFont.GetFontSize(size) == currentPixelSize)
                {
                    detectedSize = size;
                    break;
                }
            }
            
            // Update the font
            surface.Font = newFont;
            
            // If we detected the size enum, reapply it with the new font
            // Otherwise, keep the current pixel size (it may be custom)
            if (detectedSize.HasValue)
            {
                surface.FontSize = newFont.GetFontSize(detectedSize.Value);
            }
            else
            {
                // Keep the existing pixel size (custom size)
                surface.FontSize = currentPixelSize;
            }
        }

        // Recursively update all children
        for (int i = 0; i < obj.Children.Count; i++)
        {
            UpdateScreenObjectFonts(obj.Children[i], oldFont, newFont);
        }
    }

    /// <summary>
    /// Recursively updates the font size of screen objects that are using the old default font size.
    /// </summary>
    /// <param name="obj">The screen object to check and update.</param>
    /// <param name="oldSize">The old default font size to check for.</param>
    /// <param name="newSize">The new default font size to apply.</param>
    private void UpdateScreenObjectFontSizes(IScreenObject? obj, IFont.Sizes oldSize, IFont.Sizes newSize)
    {
        if (obj == null) return;

        // If this is a screen surface, check if it's using the default font and old font size
        if (obj is IScreenSurface surface && ReferenceEquals(surface.Font, _defaultFont))
        {
            Point currentPixelSize = surface.FontSize;
            Point oldDefaultPixelSize = surface.Font.GetFontSize(oldSize);
            
            // If the surface is using the old default font size, update it to the new default
            if (currentPixelSize == oldDefaultPixelSize)
            {
                surface.FontSize = surface.Font.GetFontSize(newSize);
            }
        }

        // Recursively update all children
        for (int i = 0; i < obj.Children.Count; i++)
        {
            UpdateScreenObjectFontSizes(obj.Children[i], oldSize, newSize);
        }
    }
}
