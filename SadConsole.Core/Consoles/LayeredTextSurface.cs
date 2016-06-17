using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Text surface with multiple layers.
    /// </summary>
    [DataContract]
    public class LayeredTextSurface: TextSurface
    {
        /// <summary>
        /// A layer.
        /// </summary>
        [DataContract]
        public class Layer
        {
            /// <summary>
            /// All cells of the layer.
            /// </summary>
            [DataMember]
            public Cell[] Cells;

            /// <summary>
            /// The cells that will be rendered.
            /// </summary>
            [DataMember]
            public Cell[] RenderCells;

            /// <summary>
            /// When true, the layer will be drawn.
            /// </summary>
            [DataMember]
            public bool IsVisible = true;

            /// <summary>
            /// Custom object.
            /// </summary>
            [DataMember]
            public object Metadata;

            /// <summary>
            /// The index of the layer.
            /// </summary>
            public int Index;
        }

        /// <summary>
        /// Layers for the surface.
        /// </summary>
        protected List<Layer> layers;

        /// <summary>
        /// Count of layers.
        /// </summary>
        public int LayerCount { get { return layers.Count; } }

        /// <summary>
        /// The current zero-based active layer index.
        /// </summary>
        public int ActiveLayerIndex { get; private set; }

        /// <summary>
        /// Gets the active layer.
        /// </summary>
        public Layer ActiveLayer { get { return layers[ActiveLayerIndex]; } }

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
            this.layers = new List<Layer>(layers);

            for (int i = 0; i < layers; i++)
                this.layers.Add(new Layer());

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
            base.cells = layers[index].Cells;
            base.RenderCells = layers[index].RenderCells;
        }

        /// <summary>
        /// Changes the active layer, which sets the current cell data for <see cref="ITextSurface"/>.
        /// </summary>
        /// <param name="layer">The layer to set active.</param>
        public void SetActiveLayer(Layer layer)
        {
            if (layers.Contains(layer))
                SetActiveLayer(layers.IndexOf(layer));
        }

        protected override void InitializeCells()
        {
            for (int i = 0; i < LayerCount; i++)
            {
                InitializeLayer(layers[i]);
            }
        }

        protected void InitializeLayer(Layer layer)
        {
            layer.Cells = new Cell[width * height];

            for (int c = 0; c < layer.Cells.Length; c++)
            {
                layer.Cells[c] = new Cell();
                layer.Cells[c].Foreground = this.DefaultForeground;
                layer.Cells[c].Background = this.DefaultBackground;
                layer.Cells[c].OnCreated();
            }

            layer.RenderCells = layer.Cells;
        }

        protected override void ResetArea()
        {
            RenderRects = new Rectangle[area.Width * area.Height];
            
            for (int l = 0; l < LayerCount; l++)
                ResetAreaLayer(layers[l]);

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = new Rectangle(x * Font.Size.X, y * Font.Size.Y, Font.Size.X, Font.Size.Y);
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);
        }

        protected void ResetAreaLayer(Layer layer)
        {
            layer.RenderCells = new Cell[area.Width * area.Height];

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    layer.RenderCells[index] = layer.Cells[(y + area.Top) * width + (x + area.Left)];
                    index++;
                }
            }
        }

        /// <summary>
        /// Gets all of the layers.
        /// </summary>
        /// <returns>The layers.</returns>
        public Layer[] GetLayers()
        {
            return layers.ToArray();
        }

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <returns>The created layer.</returns>
        public Layer Add()
        {
            var layer = new Layer();
            InitializeLayer(layer);
            ResetAreaLayer(layer);
            layers.Add(layer);
            SyncLayerIndex();
            return layer;
        }

        /// <summary>
        /// Removes a layer.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void Remove(Layer layer)
        {
            if (layers.Contains(layer))
            {
                int index = layers.IndexOf(layer);

                layers.Remove(layer);

                if (ActiveLayerIndex == index || ActiveLayerIndex >= layers.Count)
                    SetActiveLayer(0);
                else
                    SetActiveLayer(ActiveLayerIndex);

                SyncLayerIndex();
            }
        }

        /// <summary>
        /// Removes a layer.
        /// </summary>
        /// <param name="index">The layer index to remove.</param>
        public void Remove(int index)
        {
            if (index < 0 || index >= layers.Count)
                throw new IndexOutOfRangeException();

            Remove(layers[index]);
            SyncLayerIndex();
        }

        /// <summary>
        /// Moves a layer to the specified index.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <param name="index">The new index of the layer.</param>
        public void Move(Layer layer, int index)
        {
            if (layers.Contains(layer))
            {
                layers.Remove(layer);
                layers.Insert(index, layer);
                SyncLayerIndex();
            }
        }

        /// <summary>
        /// Moves a layer to the top.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        public void MoveToTop(Layer layer)
        {
            if (layers.Contains(layer))
            {
                layers.Remove(layer);
                layers.Add(layer);
                SyncLayerIndex();
            }
        }

        /// <summary>
        /// Moves a layer to the bottom.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        public void MoveToBottom(Layer layer)
        {
            if (layers.Contains(layer))
            {
                layers.Remove(layer);
                layers.Insert(0, layer);
                SyncLayerIndex();
            }
        }

        /// <summary>
        /// Gets the index of a layer.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <returns>The index of the layer.</returns>
        public int IndexOf(Layer layer)
        {
            if (layers.Contains(layer))
                return layers.IndexOf(layer);
            else
                return -1;
        }

        /// <summary>
        /// Gets a specific layer.
        /// </summary>
        /// <param name="index">The zero-based layer to get.</param>
        /// <returns></returns>
        public Layer GetLayer(int index)
        {
            return layers[index];
        }

        #region Serialization
        /// <summary>
        /// Saves the <see cref="LayeredTextSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        /// <param name="knownTypes">Types to provide if the <see cref="SurfaceEditor.TextSurface"/> and <see cref="Renderer" /> types are custom and unknown to the serializer.</param>
        public void Save(string file, params Type[] knownTypes)
        {
            Serializer.Save(this, file, knownTypes.Union(Serializer.ConsoleTypes).ToArray());
        }

        /// <summary>
        /// Loads a <see cref="LayeredTextSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="knownTypes">Types to provide if the <see cref="SurfaceEditor.TextSurface"/> and <see cref="Renderer" /> types are custom and unknown to the serializer.</param>
        /// <returns>The <see cref="LayeredTextSurface"/>.</returns>
        public static LayeredTextSurface Load(string file, params Type[] knownTypes)
        {
            return Serializer.Load<LayeredTextSurface>(file, knownTypes.Union(Serializer.ConsoleTypes).ToArray());
        }

        /// <summary>
        /// Saves the <see cref="LayeredTextSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public new void Save(string file)
        {
            Serializer.Save(this, file, Serializer.ConsoleTypes);
        }

        /// <summary>
        /// Loads a <see cref="LayeredTextSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public new static LayeredTextSurface Load(string file)
        {
            return Serializer.Load<LayeredTextSurface>(file, Serializer.ConsoleTypes);
        }

        protected void SyncLayerIndex()
        {
            int i = 0;
            foreach (var layer in layers)
            {
                layer.Index = i;
                i++;
            }
        }


        [OnSerializing]
        private void BeforeSerializing(StreamingContext context)
        {
            fontName = Font.Name;
            fontSize = Font.SizeMultiple;
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            Font font;

            // Try to find font
            if (Engine.Fonts.ContainsKey(fontName))
                font = Engine.Fonts[fontName].GetFont(fontSize);
            else
                font = Engine.DefaultFont;

            Font = font;
            SyncLayerIndex();
            SetActiveLayer(ActiveLayerIndex);
        }
        #endregion
    }
}
