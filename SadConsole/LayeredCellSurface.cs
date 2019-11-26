using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial class LayeredScreenSurface : ScreenSurface
    {
        private List<ILayer> _layers;

        /// <summary>
        /// Gets the number of layers.
        /// </summary>
        public int LayerCount => _layers.Count;

        /// <summary>
        /// A read-only collection of layers.
        /// </summary>
        public IReadOnlyList<ILayer> Layers => _layers.AsReadOnly();

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="layers">The number of layers to add to this surface.</param>
        public LayeredScreenSurface(int width, int height, int layers) : this(width, height, width, height, layers)
        {
            
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the surface in cells.</param>
        /// <param name="height">The visible height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        /// <param name="layers">The number of layers to add to this surface.</param>
        public LayeredScreenSurface(int width, int height, int bufferWidth, int bufferHeight, int layers) : base(width, height, bufferWidth, bufferHeight, null)
        {
            if (layers < 1) throw new ArgumentOutOfRangeException(nameof(layers), "The layer count must be one or more.");
            _layers = new List<ILayer>(layers);

            // Create first layer from existing cells.
            _layers.Add(new Layer("root", bufferWidth, bufferHeight, Cells));

            for (int i = 1; i < layers; i++)
            {
                ColoredGlyph[] layerCells = ColoredGlyph.CreateArray(width * height);

                for (int c = 0; c < layerCells.Length; c++)
                    layerCells[c].Background = Color.Transparent;

                _layers.Add(new Layer($"layer{i}", bufferWidth, bufferHeight, layerCells));
            }
        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public LayeredScreenSurface(int width, int height, ColoredGlyph[][] initialCells) : this(width, height, width, height, initialCells)
        {

        }

        /// <summary>
        /// Creates a new surface with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the surface in cells.</param>
        /// <param name="height">The height of the surface in cells.</param>
        /// <param name="bufferWidth">The total width of the surface in cells.</param>
        /// <param name="bufferHeight">The total height of the surface in cells.</param>
        /// <param name="initialCells">The cells to seed the surface with. If <see langword="null"/>, creates the cell array for you.</param>
        public LayeredScreenSurface(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[][] initialCells): base(width, height, bufferWidth, bufferHeight, initialCells?[0])
        {
            if (initialCells == null) throw new ArgumentNullException(nameof(initialCells), $"You must pass an array of {nameof(ColoredGlyph)} arrays to represent the surface layers.");

            int layers = initialCells.GetUpperBound(0) + 1;
            _layers = new List<ILayer>(layers);

            // Create first layer from existing cells.
            _layers.Add(new Layer("root", 0, 0, Cells));

            for (int i = 1; i < layers; i++)
                _layers.Add(new Layer($"layer{i}", 0, 0, ColoredGlyph.CreateArray(width * height)));
        }
    }
}
