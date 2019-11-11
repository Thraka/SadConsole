using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole.Renderers;
using System.IO;
using SadConsole.Input;

namespace SadConsole
{
    /// <summary>
    /// Represents 
    /// </summary>
    public abstract class GameHost : IDisposable
    {
        /// <summary>
        /// Instance of the game host.
        /// </summary>
        public static GameHost Instance { get; protected set; }

        protected static bool LoadingEmbeddedFont = false;

        protected internal static string SerializerPathHint { get; set; }

        /// <summary>
        /// Raised when the game draws a frame.
        /// </summary>
        public event EventHandler<GameHost> FrameDraw;

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

        public Point WindowSize { get; protected set; }

        public int ScreenCellsX { get; protected set; }

        public int ScreenCellsY { get; protected set; }

        /// <summary>
        /// Raises the <see cref="FrameDraw"/> event.
        /// </summary>
        protected virtual void OnFrameDraw() =>
            FrameDraw?.Invoke(this, this);

        /// <summary>
        /// Raises the <see cref="FrameUpdate"/> event.
        /// </summary>
        protected virtual void OnFrameUpdate() =>
            FrameUpdate?.Invoke(this, this);

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
        /// Gets the default <see cref="IRenderer"/> implemented by the host.
        /// </summary>
        /// <param name="screenObject">The object to get a renderer for.</param>
        /// <returns>A renderer.</returns>
        public abstract IRenderer GetDefaultRenderer(ScreenObjectSurface screenObject);

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
                Font masterFont = SadConsole.Serializer.Load<Font>(font, false);

                if (Global.Fonts.ContainsKey(masterFont.Name))
                {
                    Global.Fonts.Remove(masterFont.Name);
                }

                Global.Fonts.Add(masterFont.Name, masterFont);
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
        /// Loads the <c>IBM.font</c> built into the binary.
        /// </summary>
        protected void LoadEmbeddedFont()
        {
            System.Reflection.Assembly assembly = typeof(SadConsole.Font).Assembly;
            string resourceNameFont;

            if (Settings.UseDefaultExtendedFont)
                resourceNameFont = "SadConsole.Resources.IBM_ext.font";
            else
                resourceNameFont = "SadConsole.Resources.IBM.font";


            using (Stream stream = assembly.GetManifestResourceStream(resourceNameFont))
            using (StreamReader sr = new StreamReader(stream))
            {
                LoadingEmbeddedFont = true;
                SerializerPathHint = "";
                var masterFont = (Font)Newtonsoft.Json.JsonConvert.DeserializeObject(
                    sr.ReadToEnd(),
                    typeof(Font),
                    new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
                    });

                Global.Fonts.Add(masterFont.Name, masterFont);
                Global.DefaultFont = masterFont;

                LoadingEmbeddedFont = false;
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var font in Global.Fonts.Values)
                    {
                        font.Image.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Game()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
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
