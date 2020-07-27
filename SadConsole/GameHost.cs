using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole.Renderers;
using System.IO;
using SadConsole.Input;
using System.Reflection;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// Represents the SadConsole game engine.
    /// </summary>
    public abstract partial class GameHost : IDisposable
    {
        private DateTime _gameStartedAt = DateTime.Now;

        /// <summary>
        /// Holds all of the renderer types.
        /// </summary>
        protected Dictionary<string, System.Type> _renderers = new Dictionary<string, Type>(5);

        /// <summary>
        /// The splashs screens to show on game startup.
        /// </summary>
        protected internal Queue<ScreenSurface> _splashScreens { get; set; } = new Queue<ScreenSurface>();

        /// <summary>
        /// Instance of the game host.
        /// </summary>
        public static GameHost Instance { get; protected set; }

        /// <summary>
        /// Temp variable to indicate that the fonts being loaded are the embedded fonts.
        /// </summary>
        protected static bool LoadingEmbeddedFont = false;

        /// <summary>
        /// Contains the path to a file being serialized or deserialized.
        /// </summary>
        protected internal static string SerializerPathHint { get; set; }

        /// <summary>
        /// Raised when the game draws a frame to the screen.
        /// </summary>
        public event EventHandler<GameHost> FrameRender;

        /// <summary>
        /// Raised when the game updates prior to drawing a frame.
        /// </summary>
        public event EventHandler<GameHost> FrameUpdate;

        /// <summary>
        /// A callback to run before the <see cref="Run"/> method is called;
        /// </summary>
        public Action OnStart;

        /// <summary>
        /// A callback to run after the <see cref="Run"/> method is called;
        /// </summary>
        public Action OnEnd;

        /// <summary>
        /// Draw calls registered for the next drawing frame.
        /// </summary>
        public Queue<DrawCalls.IDrawCall> DrawCalls { get; } = new Queue<DrawCalls.IDrawCall>();

        /// <summary>
        /// The size of the game window.
        /// </summary>
        public Point WindowSize { get; protected set; }

        /// <summary>
        /// How many cells fit in the render area width used by SadConsole.
        /// </summary>
        public int ScreenCellsX { get; protected set; }

        /// <summary>
        /// How many cells fit in the render area width used by SadConsole.
        /// </summary>
        public int ScreenCellsY { get; protected set; }

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
            GameHost.Instance.GameRunningTotalTime = DateTime.Now - _gameStartedAt;
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
        /// Creates and returns a renderer by name.
        /// </summary>
        /// <param name="name">The name of the renderer.</param>
        /// <returns>A new renderer.</returns>
        public virtual IRenderer GetRenderer(string name)
        {
            if (_renderers.TryGetValue(name, out Type objType))
                return (IRenderer)Activator.CreateInstance(objType);

            return (IRenderer)Activator.CreateInstance(_renderers["default"]);
        }

        /// <summary>
        /// Sets the default <see cref="IRenderer"/> for a type.
        /// </summary>
        /// <param name="name">The name to register the renderer as.</param>
        /// <param name="rendererType">The renderer type.</param>
        public void SetRenderer(string name, System.Type rendererType) =>
            _renderers[name] = rendererType;

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
        public void SetSplashScreens(params ScreenSurface[] surfaces) =>
            _splashScreens = new Queue<ScreenSurface>(surfaces);

        /// <summary>
        /// Loads a font from a file and adds it to the <see cref="Fonts"/> collection.
        /// </summary>
        /// <param name="font">The font file to load.</param>
        /// <returns>A master font that you can generate a usable font from.</returns>
        public Font LoadFont(string font)
        {
            //if (!File.Exists(font))
            //{
            //    font = Path.Combine(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(font)), "fonts"), Path.GetFileName(font));
            //    if (!File.Exists(font))
            //        throw new Exception($"Font does not exist: {font}");
            //}                    

            //FontPathHint = Path.GetDirectoryName(Path.GetFullPath(font));
            try
            {
                var oldSettings = SadConsole.Serializer.Settings;
                SadConsole.Serializer.Settings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                };

                Font masterFont = SadConsole.Serializer.Load<Font>(font, false);

                if (GameHost.Instance.Fonts.ContainsKey(masterFont.Name))
                    GameHost.Instance.Fonts.Remove(masterFont.Name);

                GameHost.Instance.Fonts.Add(masterFont.Name, masterFont);

                SadConsole.Serializer.Settings = oldSettings;

                return masterFont;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {

                throw;
            }

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
        /// Loads the <c>IBM.font</c> built into the binary.
        /// </summary>
        protected void LoadEmbeddedFont()
        {
            System.Reflection.Assembly assembly = typeof(SadConsole.Font).Assembly;

            Font LoadFont(string fontName)
            {
                using Stream stream = assembly.GetManifestResourceStream(fontName);
                using StreamReader sr = new StreamReader(stream);

                SerializerPathHint = "";

                LoadingEmbeddedFont = true;
                var masterFont = (Font)Newtonsoft.Json.JsonConvert.DeserializeObject(
                    sr.ReadToEnd(),
                    typeof(Font),
                    new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
                        Converters = null
                    });
                LoadingEmbeddedFont = false;

                GameHost.Instance.Fonts.Add(masterFont.Name, masterFont);

                return masterFont;
            }

            EmbeddedFont = LoadFont("SadConsole.Resources.IBM.font");
            EmbeddedFontExtended = LoadFont("SadConsole.Resources.IBM_ext.font");
        }

        /// <summary>
        /// Uses reflection to examine the <see cref="Color"/> type and add any predefined colors into <see cref="ColorExtensions.ColorMappings"/>.
        /// </summary>
        protected void LoadMappedColors()
        {
            if (Settings.AutomaticAddColorsToMappings)
            {
                //ColorExtensions.ColorMappings.Add
                var colorType = typeof(Color);
                foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) => t.FieldType.Name == colorType.Name))
                    ColorExtensions.ColorMappings.Add(item.Name.ToLower(), (Color)item.GetValue(null));
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
                    foreach (Font font in GameHost.Instance.Fonts.Values)
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
}
