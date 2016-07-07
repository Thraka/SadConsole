using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Ansi
{
    public class Document: IDisposable
    {
        private string _ansiData;
        private byte[] _ansiBytes;

        public byte[] AnsiBytes
        {
            get { return _ansiBytes; }
            set
            {
                _ansiBytes = value;
                //_ansiData = Encoding.UTF8.GetString(_ansiBytes);
                _ansiData = new string(_ansiBytes.Select(b => (char)b).ToArray());

                if (Stream != null)
                    Stream.Dispose();

                Stream = new MemoryStream(AnsiBytes);
            }
        }

        public string AnsiString { get { return _ansiData; } }

        public MemoryStream Stream { get; private set; }

        private Document() { }

        public Document(string file)
        {
            using (var stream = System.IO.File.OpenRead(file))
                using (var reader = new System.IO.BinaryReader(stream))
                    AnsiBytes = reader.ReadBytes((int)stream.Length);

            Stream = new MemoryStream(AnsiBytes);
        }

        public static Document FromAsciiString(string ansiContent)
        {
            Document doc = new Document();
            doc.AnsiBytes = Encoding.ASCII.GetBytes(ansiContent);
            doc.Stream = new MemoryStream(doc.AnsiBytes);
            return doc;
        }

        public static Document FromBytes(byte[] bytes)
        {
            Document doc = new Document();
            doc.AnsiBytes = bytes;
            doc.Stream = new MemoryStream(doc.AnsiBytes);
            return doc;
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
