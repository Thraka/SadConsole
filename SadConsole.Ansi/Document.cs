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
                _ansiData = new string(_ansiBytes.Select(b => (char)b).ToArray());

                if (Stream != null)
                    Stream.Dispose();

                Stream = new MemoryStream(AnsiBytes);
            }
        }

        public string AnsiString { get { return _ansiData; } }

        public MemoryStream Stream { get; private set; }

        public Document(string file)
        {
            using (var stream = System.IO.File.OpenRead(file))
                using (var reader = new System.IO.BinaryReader(stream))
                    AnsiBytes = reader.ReadBytes((int)stream.Length);

            Stream = new MemoryStream(AnsiBytes);
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
