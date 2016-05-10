//// Copyright (c) 2015 Andrew Varnerin
//// Taken from https://github.com/BaconSoap/RexReader
//// Namespace changed to match SadConsole

//namespace SadConsole.Readers.REXPaint
//{
//    /// <summary>
//    /// A two-dimensional array tiles in single or multiple layers
//    /// </summary>
//    public class TileMap {
//        /// <summary>
//        /// Individual layers, ordered by their order in the source file.
//        /// </summary>
//        public TileLayer[] Layers { get; private set; }

//        /// <summary>
//        /// The width of the TileMap. Every layer has the same width.
//        /// </summary>
//        public int Width { get; private set; }

//        /// <summary>
//        /// The height of the TileMap. Every layer has the same height.
//        /// </summary>
//        public int Height { get; private set; }

//        /// <summary>
//        /// The number of layers in the TileMap
//        /// </summary>
//        public int LayerCount { get { return Layers.Length; } }

//        /// <summary>
//        /// Creates a new TileMap with specified dimensions
//        /// </summary>
//        /// <param name="width">The width of the TileMap, to be passed to each layer</param>
//        /// <param name="height">The height of the TileMap, to be passed to each layer</param>
//        /// <param name="layers">The number of layers to construct on initialization</param>
//        public TileMap(int width, int height, int layers) {
//            Layers = new TileLayer[layers];
//            Width = width;
//            Height = height;
//            for (var i = 0; i < layers; i++) {
//                Layers[i] = new TileLayer(width, height);
//            }
//        }

//        /// <summary>
//        /// Converts a REXPaint .xp file to a <see cref="Consoles.LayeredConsole">LayeredConsole</see>
//        /// </summary>
//        /// <returns>The converted console.</returns>
//        public Consoles.LayeredConsole ToLayeredConsole()
//        {
//            var console = new Consoles.LayeredConsole(Layers.Length, Width, Height);

//            for (int i = 0; i < Layers.Length; i++)
//                console[i].CellData = Layers[i].ToTextSurface();

//            return console;
//        }

//    }
//}
