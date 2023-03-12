using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// Holds all of the <see cref="IRenderer"/> types.
    /// </summary>
    protected Dictionary<string, System.Type> _renderers = new Dictionary<string, Type>(5);

    /// <summary>
    /// Holds all of the <see cref="IRenderStep"/> types.
    /// </summary>
    protected Dictionary<string, System.Type> _rendererSteps = new Dictionary<string, Type>(5);

    /// <summary>
    /// The splashs screens to show on game startup.
    /// </summary>
    protected internal Queue<IScreenSurface> _splashScreens { get; set; } = new Queue<IScreenSurface>();

    /// <summary>
    /// Instance of the game host.
    /// </summary>
    public static GameHost Instance { get; protected set; } = null!;

    /// <summary>
    /// Temp variable to indicate that the fonts being loaded are the embedded fonts.
    /// </summary>
    protected static bool LoadingEmbeddedFont = false;

    /// <summary>
    /// Contains the path to a file being serialized or deserialized.
    /// </summary>
    protected internal static string SerializerPathHint { get; set; } = String.Empty;

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
    public Action? OnStart;

    /// <summary>
    /// A callback to run after the <see cref="Run"/> method is called;
    /// </summary>
    public Action? OnEnd;

    /// <summary>
    /// Draw calls registered for the next drawing frame.
    /// </summary>
    public Queue<DrawCalls.IDrawCall> DrawCalls { get; } = new Queue<DrawCalls.IDrawCall>();

    /// <summary>
    /// How many cells fit in the render area width used by SadConsole.
    /// </summary>
    public int ScreenCellsX { get; protected set; }

    /// <summary>
    /// How many cells fit in the render area width used by SadConsole.
    /// </summary>
    public int ScreenCellsY { get; protected set; }

    /// <summary>
    /// A frame number counter, incremented every game frame.
    /// </summary>
    public int FrameNumber { get; set; }

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
    /// <returns></returns>
    public abstract ITexture GetTexture(string resourcePath);

    /// <summary>
    /// Gets a texture from the implemented host.
    /// </summary>
    /// <param name="textureStream">A stream containing the texture.</param>
    /// <returns></returns>
    public abstract ITexture GetTexture(Stream textureStream);

    /// <summary>
    /// Creates and returns an <see cref="IRenderer"/> by name.
    /// </summary>
    /// <param name="name">The name of the renderer.</param>
    /// <returns>A new renderer.</returns>
    public virtual IRenderer GetRenderer(string name)
    {
        if (_renderers.TryGetValue(name, out Type? objType))
            return Activator.CreateInstance(objType) as IRenderer
                ?? throw new NullReferenceException($"Renderer was found registered, but the system was unable to create an instance of it as an {nameof(IRenderer)}.");

        return Activator.CreateInstance(_renderers["default"]) as IRenderer
            ?? throw new NullReferenceException("Unable to create the default renderer, it doesn't seem to be registered.");
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

            if (GameHost.Instance.Fonts.ContainsKey(masterFont.Name))
                GameHost.Instance.Fonts.Remove(masterFont.Name);

            GameHost.Instance.Fonts.Add(masterFont.Name, masterFont);

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
        System.Reflection.Assembly assembly = typeof(SadConsole.SadFont).Assembly;

        SadFont LoadFont(string fontName)
        {
            using Stream stream = assembly.GetManifestResourceStream(fontName)!;
            using StreamReader sr = new StreamReader(stream);

            SerializerPathHint = "";

            LoadingEmbeddedFont = true;
            var masterFont = (SadFont)Newtonsoft.Json.JsonConvert.DeserializeObject(
                sr.ReadToEnd(),
                typeof(SadFont),
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
                    Converters = null!
                })!;
            LoadingEmbeddedFont = false;

            Fonts.Add(masterFont.Name, masterFont);

            return masterFont;
        }

        EmbeddedFont = LoadFont("SadConsole.Resources.IBM.font");
        EmbeddedFontExtended = LoadFont("SadConsole.Resources.IBM_ext.font");

        // Configure default font
        if (string.IsNullOrEmpty(defaultFont))
            if (Settings.UseDefaultExtendedFont)
                DefaultFont = EmbeddedFontExtended;
            else
                DefaultFont = EmbeddedFont;
        else
            DefaultFont = this.LoadFont(defaultFont);
    }

    /// <summary>
    /// Uses reflection to examine the <see cref="Color"/> type and add any predefined colors into <see cref="ColorExtensions2.ColorMappings"/>.
    /// </summary>
    protected void LoadMappedColors()
    {
        if (Settings.AutomaticAddColorsToMappings)
        {
            //ColorExtensions.ColorMappings.Add
            var colorType = typeof(Color);
            foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) => t.FieldType.Name == colorType.Name))
                ColorExtensions2.ColorMappings.Add(item.Name.ToLower(), (Color)item.GetValue(null)!);
        }
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
                foreach (IFont font in GameHost.Instance.Fonts.Values)
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
    ~GameHost()
    {
        Dispose(false);
    }

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}
