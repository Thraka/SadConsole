// Copyright (c) 2015 Andrew Varnerin
// Taken from https://github.com/BaconSoap/RexReader
// Namespace changed to match SadConsole

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SadConsole.Readers.REXPaint {
    /// <summary>
    /// Reads a compressed .xp stream and provides methods to read the data.
    /// </summary>
    public class RexReader: IDisposable {
        private Stream Deflated { get; set; }
        private BinaryReader Reader { get; set; }
        private int? _layers;
        private int?[,] _layerSizeCache;
        private bool _disposed = false;
        private int _layerCountOffset;

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("RexReader");
            }
        }

        /// <summary>
        /// Construct a RexReader from a compressed stream (of the .xp format)
        /// </summary>
        /// <param name="inputStream">The compressed stream of the .xp file</param>
        public RexReader(Stream inputStream) {
            SetupFromStream(inputStream);
        }

        /// <summary>
        /// Construct a RexReader from a compressed stream (of the .xp format)
        /// </summary>
        /// <param name="inputStream"></param>
        private void SetupFromStream(Stream inputStream) {
            Deflated = new MemoryStream();

            //These are all over the place. Every method in this file should guarantee that the stream
            // is reset, so that the abstraction doesn't leak.
            inputStream.Position = 0;
            using (var deflate = new GZipStream(inputStream, CompressionMode.Decompress)) {
                CopyStream(deflate, Deflated);
            }

            Reader = new BinaryReader(Deflated);
            Deflated.Position = 0;
            var firstVal = Reader.ReadInt32();
            if (firstVal < 0)
            {
                _layerCountOffset = 4;
            }
            Deflated.Position = _layerCountOffset;
        }
        
        /// <summary>
        /// Construct a RexReader from an .xp file. Throws standard errors if the file doesn't exist.
        /// </summary>
        /// <param name="filePath">Path to the .xp file</param>
        public RexReader(string filePath) {
            using (var memoryStream = new MemoryStream()) {
                using (var filestream = new FileStream(filePath, FileMode.Open)) {
                    CopyStream(filestream, memoryStream);
                }
                SetupFromStream(memoryStream);
            }
        }

        /// <summary>
        /// Retrieve the number of layers in the source file
        /// </summary>
        /// <returns>Number of layers in image</returns>
        public int GetLayerCount() {
            CheckDisposed();
            if (_layers.HasValue)
            {
                return _layers.Value;
            }
            Deflated.Position = _layerCountOffset;
            int layerCount = Reader.ReadInt32();
            Deflated.Position = _layerCountOffset;
            _layers = layerCount;
            _layerSizeCache = new int?[layerCount, 2];
            return layerCount;
        }

        /// <summary>
        /// Gets the width of the layer specified
        /// </summary>
        /// <param name="layer">The 0-based layer number</param>
        /// <returns>The width in cells of the specified layer</returns>
        public int GetLayerWidth(int layer) {
            CheckDisposed();
            if (layer < 0 || layer >= GetLayerCount())
                throw new ArgumentOutOfRangeException("layer");

            if (_layerSizeCache[layer, 0].HasValue)
            {
                return _layerSizeCache[layer, 0].Value;
            }

            var offset = (_layerCountOffset*8 + 32 + layer * 64) / 8;

            Deflated.Seek(offset, SeekOrigin.Begin);
            var width = Reader.ReadInt32();
            _layerSizeCache[layer, 0] = width;
            Deflated.Seek(0, SeekOrigin.Begin);
            return width;
        }

        /// <summary>
        /// Gets the height of the layer specified. Throws 
        /// </summary>
        /// <param name="layer">The 0-based layer number</param>
        /// <returns>The height in cells of the specified layer</returns>
        public int GetLayerHeight(int layer) {
            CheckDisposed();
            if (layer < 0 || layer >= GetLayerCount())
                throw new ArgumentOutOfRangeException("layer");

            if (_layerSizeCache[layer, 1].HasValue)
            {
                return _layerSizeCache[layer, 1].Value;
            }

            var offset = (_layerCountOffset*8 + 32 + 32 + layer * 64) / 8;

            Deflated.Seek(offset, SeekOrigin.Begin);
            var height = Reader.ReadInt32();
            _layerSizeCache[layer, 1] = height;
            Deflated.Seek(0, SeekOrigin.Begin);
            return height;
        }

        /// <summary>
        /// Reads the characters of the specified layer into a string and returns it. No newlines/colors are included.
        /// Returned string goes down, then right (down each column and wraps up to the top on a new column) ie column-major.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public string ReadLayerAsString(int layer) {
            CheckDisposed();
            if (layer < 0 || layer >= GetLayerCount())
                throw new ArgumentOutOfRangeException("layer");

            var count = GetLayerWidth(layer) * GetLayerHeight(layer);
            var builder = new StringBuilder(count);

            //Find the starting offset of the layer
            var offset = GetFirstTileOffset();
            Deflated.Seek(offset, SeekOrigin.Begin);

            //The number of bytes to skip after each character
            const int charColorSize = 6;
            for (var i = 0; i < count; i++) {
                builder.Append((char)Reader.ReadInt32());
                Deflated.Seek(charColorSize, SeekOrigin.Current);
            }
            Deflated.Seek(0, SeekOrigin.Begin);
            return builder.ToString();
        }

        /// <summary>
        /// Get the offset of the first tile in the stream.
        /// </summary>
        /// <returns>The 0-based byte position</returns>
        private int GetFirstTileOffset() {
            return (_layerCountOffset * 8 + 32 + GetLayerCount() * 64) / 8;
        }

        /// <summary>
        /// Retrieves the entire map, including all its layers. Row-major order for tiles (y, then x).
        /// </summary>
        /// <returns>The corresponding TileMap that is contained in the .xp file</returns>
        /// <remarks>
        /// Note that (255,0,255) looks to be the 'transparent' code. But only for backgrounds?
        /// If you see magenta where you thought was black, that's why.
        /// </remarks>
        public TileMap GetMap() {
            CheckDisposed();
            int layers;
            int width;
            int height;

            //Get out early if the data is corrupt/malformed
            try {
                layers = GetLayerCount();
                width = GetLayerWidth(0);
                height = GetLayerHeight(0);
            } catch (Exception e) {
                ((IDisposable)Reader).Dispose();
                throw new InvalidDataException("Bad .xp data", e);
            }
            var map = new TileMap(width, height, layers);

            //Move to the first tile
            Deflated.Seek(GetFirstTileOffset(), SeekOrigin.Begin);

            //Read every tile in column-major order, and place it in the map in row-major order
            for (var layer = 0; layer < layers; layer++) {
                for (var x = 0; x < width; x++) {
                    for (var y = 0; y < height; y++) {
                        map.Layers[layer].Tiles[y, x] = new Tile {
                            CharacterCode = (byte)Reader.ReadInt32(),
                            ForegroundRed = Reader.ReadByte(),
                            ForegroundGreen = Reader.ReadByte(),
                            ForegroundBlue = Reader.ReadByte(),
                            BackgroundRed = Reader.ReadByte(),
                            BackgroundGreen = Reader.ReadByte(),
                            BackgroundBlue = Reader.ReadByte()
                        };
                    }
                }
            }
            Deflated.Seek(0, SeekOrigin.Begin);
            return map;
        }

        /// <summary>
        /// Replicates Stream.CopyTo so I can bump down the .NET version
        /// </summary>
        /// <param name="source">The stream to read from</param>
        /// <param name="destination">The stream to writed to</param>
        private void CopyStream(Stream source, Stream destination) {
            var buffer = new byte[4096];
            int byt;
            while ((byt = source.Read(buffer, 0, buffer.Length)) != 0) {
                destination.Write(buffer, 0, byt);
            }
        }

        public Consoles.LayeredConsole ToLayeredConsole()
        {
            CheckDisposed();

            Consoles.LayeredConsole console = null;
            Reader.BaseStream.Position = 0;
            var version = Reader.ReadInt32();
            var layerCount = Reader.ReadInt32();
            Microsoft.Xna.Framework.Color transparentColor = new Microsoft.Xna.Framework.Color(255, 0, 255);

            for (int layer = 0; layer < layerCount; layer++)
            {
                int width = Reader.ReadInt32();
                int height = Reader.ReadInt32();

                if (layer == 0)
                {
                    console = new Consoles.LayeredConsole(layerCount, width, height);
                }

                var currentLayer = console[layer].CellData;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var cell = currentLayer[y, x];

                        int character = Reader.ReadInt32();
                        Microsoft.Xna.Framework.Color foreground = new Microsoft.Xna.Framework.Color(Reader.ReadByte(), Reader.ReadByte(), Reader.ReadByte());
                        Microsoft.Xna.Framework.Color background = new Microsoft.Xna.Framework.Color(Reader.ReadByte(), Reader.ReadByte(), Reader.ReadByte());

                        if (background != transparentColor)
                        {
                            cell.CharacterIndex = character;
                            cell.Foreground = foreground;
                            cell.Background = background;
                        }
                    }
                }
            }

            return console;
        }

        public void Dispose()
        {
            _disposed = true;
            Deflated.Dispose();
            Reader.Close();
        }
    }
}
