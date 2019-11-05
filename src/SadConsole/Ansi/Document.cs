using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SadConsole.Ansi
{
    /// <summary>
    /// Represents an ANSI.SYS formatted document.
    /// </summary>
    public class Document : IDisposable
    {
        private byte[] _ansiBytes;

        /// <summary>
        /// Gets or sets the ANSI.SYS bytes that make up the document.
        /// </summary>
        public byte[] AnsiBytes
        {
            get => _ansiBytes;
            set
            {
                _ansiBytes = value;
                //_ansiData = Encoding.ASCII.GetString(_ansiBytes);
                AnsiString = new string(_ansiBytes.Select(b => (char)b).ToArray());

                Stream?.Dispose();

                Stream = new MemoryStream(AnsiBytes);
            }
        }

        /// <summary>
        /// A string representing the <see cref="AnsiBytes"/>.
        /// </summary>
        public string AnsiString { get; private set; }

        /// <summary>
        /// A stream that points to the <see cref="AnsiBytes"/>.
        /// </summary>
        public MemoryStream Stream { get; private set; }

        private Document() { }

        /// <summary>
        /// Creates a new document from the provided file name.
        /// </summary>
        /// <param name="file">The file to load.</param>
        public Document(string file)
        {
            using (Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(file))
            using (var reader = new BinaryReader(stream))
            {
                AnsiBytes = reader.ReadBytes((int)stream.Length);
            }

            Stream = new MemoryStream(AnsiBytes);
        }

        /// <inheritdoc />
        ~Document() => ((IDisposable)this).Dispose();

        /// <summary>
        /// Creates a new document from an existing string representing ANSI.SYS codes and characters.
        /// </summary>
        /// <param name="ansiContent">The ANSI.SYS encoded string.</param>
        /// <returns>A new document.</returns>
        public static Document FromAsciiString(string ansiContent) => new Document { AnsiBytes = Encoding.ASCII.GetBytes(ansiContent) };

        /// <summary>
        /// Creates a new document from an existing byte array representing ANSI.SYS codes and characters.
        /// </summary>
        /// <param name="bytes">The ANSI.SYS encoded byte array.</param>
        /// <returns>A new document.</returns>
        public static Document FromBytes(byte[] bytes) => new Document { AnsiBytes = bytes };

        /// <summary>
        /// Disposes the <see cref="Stream"/>.
        /// </summary>
        void IDisposable.Dispose() => Stream?.Dispose();
    }
}
