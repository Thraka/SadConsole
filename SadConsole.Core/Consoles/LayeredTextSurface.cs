using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Text surface with multiple layers.
    /// </summary>
    public class LayeredTextSurface: TextSurface
    {
        /// <summary>
        /// A layer.
        /// </summary>
        public class Layer
        {
            /// <summary>
            /// All cells of the layer.
            /// </summary>
            public Cell[] Cells;

            /// <summary>
            /// The cells that will be rendered.
            /// </summary>
            public Cell[] RenderCells;

            /// <summary>
            /// When true, the layer will be drawn.
            /// </summary>
            public bool IsVisible = true;
        }

        /// <summary>
        /// Layers for the surface.
        /// </summary>
        protected Layer[] Layers;

        /// <summary>
        /// Count of layers.
        /// </summary>
        public int LayerCount { get; private set; }

        /// <summary>
        /// The current zero-based active layer index.
        /// </summary>
        public int ActiveLayerIndex { get; private set; }

        /// <summary>
        /// Gets the active layer.
        /// </summary>
        public Layer ActiveLayer { get { return Layers[ActiveLayerIndex]; } }

        /// <summary>
        /// Creates a new layer text surface with the default <see cref="Font"/>.
        /// </summary>
        /// <param name="width">Width of the layers.</param>
        /// <param name="height">Height of the layers.</param>
        /// <param name="layers">The count of layers.</param>
        public LayeredTextSurface(int width, int height, int layers) : this(width, height, layers, Engine.DefaultFont) { }

        /// <summary>
        /// Creates a new layer text surface with the specified font.
        /// </summary>
        /// <param name="width">Width of the layers.</param>
        /// <param name="height">Height of the layers.</param>
        /// <param name="layers">The count of layers.</param>
        /// <param name="font">The font.</param>
        public LayeredTextSurface(int width, int height, int layers, Font font)
        {
            Layers = new Layer[layers];
            LayerCount = layers;

            for (int i = 0; i < layers; i++)
                Layers[i] = new Layer();

            this.width = width;
            this.height = height;
            InitializeCells();
            Font = font;
            SetActiveLayer(0);
            RenderArea = new Rectangle(0, 0, width, height);
        }

        /// <summary>
        /// Changes the active layer, which sets the current cell data for <see cref="ITextSurface"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the layer.</param>
        public void SetActiveLayer(int index)
        {
            if (index < 0 || index >= LayerCount)
                throw new ArgumentOutOfRangeException("index");

            ActiveLayerIndex = index;
            base.cells = Layers[index].Cells;
            base.RenderCells = Layers[index].RenderCells;
        }

        protected override void InitializeCells()
        {
            for (int i = 0; i < LayerCount; i++)
            {
                Layers[i].Cells = new Cell[width * height];

                for (int c = 0; c < Layers[i].Cells.Length; c++)
                {
                    Layers[i].Cells[c] = new Cell();
                    Layers[i].Cells[c].Foreground = this.DefaultForeground;
                    Layers[i].Cells[c].Background = this.DefaultBackground;
                    Layers[i].Cells[c].OnCreated();
                }

                Layers[i].RenderCells = Layers[i].Cells;
            }
        }

        protected override void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];

            int index = 0;

            for (int l = 0; l < LayerCount; l++)
                Layers[l].RenderCells = new Cell[area.Width * area.Height];

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    for (int i = 0; i < LayerCount; i++)
                    {
                        Layers[i].RenderCells[index] = Layers[i].Cells[(y + area.Top) * width + (x + area.Left)];
                    }

                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }

        /// <summary>
        /// Gets all of the layers.
        /// </summary>
        /// <returns>The layers.</returns>
        public Layer[] GetLayers()
        {
            return Layers;
        }

        /// <summary>
        /// Gets a specific layer.
        /// </summary>
        /// <param name="index">The zero-based layer to get.</param>
        /// <returns></returns>
        public Layer GetLayer(int index)
        {
            return Layers[index];
        }
    }
}
