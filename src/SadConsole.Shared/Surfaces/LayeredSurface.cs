using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// Text surface with multiple layers.
    /// </summary>
    [DataContract]
    public class LayeredSurface: BasicSurface
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

            /// <summary>
            /// Converts the layer data into a <see cref="TextSurface"/>.
            /// </summary>
            /// <param name="width">Target width of the <see cref="TextSurface"/>.</param>
            /// <param name="height">Target height of the <see cref="TextSurface"/>.</param>
            /// <param name="font">Target font of the <see cref="TextSurface"/>.</param>
            /// <returns>A new <see cref="TextSurface"/> seeded by this layer data.</returns>
            public BasicSurface ToTextSurface(int width, int height, Font font)
            {
                return new BasicSurface(width, height, Cells, font, new Rectangle(0, 0, width, height));
            }
        }

        private bool initDone = false;

        /// <summary>
        /// Layers for the surface.
        /// </summary>
        [DataMember(Name = "Layers")]
        protected List<Layer> layers;

        /// <summary>
        /// Count of layers.
        /// </summary>
        public int LayerCount { get { return layers.Count; } }

        /// <summary>
        /// The current zero-based active layer index.
        /// </summary>
        [DataMember(Name = "ActiveLayer")]
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
        public LayeredSurface(int width, int height, int layers) : this(width, height, Global.FontDefault, layers) { }

        /// <summary>
        /// Creates a new layer text surface with the default <see cref="Font"/>.
        /// </summary>
        /// <param name="width">Width of the layers.</param>
        /// <param name="height">Height of the layers.</param>
        /// <param name="renderArea">Viewable part of the surface.</param>
        /// <param name="layers">The count of layers.</param>
        public LayeredSurface(int width, int height, Rectangle renderArea, int layers) : this(width, height, Global.FontDefault, renderArea, layers) { }


        /// <summary>
        /// Creates a new layer text surface with the specified font.
        /// </summary>
        /// <param name="width">Width of the layers.</param>
        /// <param name="height">Height of the layers.</param>
        /// <param name="layers">The count of layers.</param>
        /// <param name="font">The font.</param>
        public LayeredSurface(int width, int height, Font font, int layers): this(width, height, font, new Rectangle(0, 0, width, height), layers)
        {

        }

        /// <summary>
        /// Creates a new layer text surface with the specified font.
        /// </summary>
        /// <param name="width">Width of the layers.</param>
        /// <param name="height">Height of the layers.</param>
        /// <param name="layers">The count of layers.</param>
        /// <param name="renderArea">Viewable part of the surface.</param>
        /// <param name="font">The font.</param>
        public LayeredSurface(int width, int height, Font font, Rectangle renderArea, int layers) : base(width, height, font, renderArea)
        {
            initDone = true;

            this.layers = new List<Layer>(layers);

            for (int i = 0; i < layers; i++)
                this.layers.Add(new Layer());

            InitializeCells();
            ResetArea();
            SyncLayerIndex();

            SetActiveLayer(0);
        }

        /// <summary>
        /// Changes the active layer, which sets the current cell data for <see cref="ITextSurfaceRendered"/>.
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
        /// Changes the active layer, which sets the current cell data for <see cref="ITextSurfaceRendered"/>.
        /// </summary>
        /// <param name="layer">The layer to set active.</param>
        public void SetActiveLayer(Layer layer)
        {
            if (layers.Contains(layer))
                SetActiveLayer(layers.IndexOf(layer));
        }

        /// <summary>
        /// Sets up all the cells of a layer for the first time.
        /// </summary>
        protected override void InitializeCells()
        {
            if (!initDone)
                return;

            for (int i = 0; i < LayerCount; i++)
            {
                InitializeLayer(layers[i]);
            }
        }


        /// <summary>
        /// Initializes a layers cells.
        /// </summary>
        /// <param name="layer">The layer to setup.</param>
        protected void InitializeLayer(Layer layer)
        {
            layer.Cells = new Cell[width * height];

            for (int c = 0; c < layer.Cells.Length; c++)
            {
                layer.Cells[c] = new Cell();
                layer.Cells[c].Foreground = this.DefaultForeground;
                layer.Cells[c].Background = this.DefaultBackground;
            }

            layer.RenderCells = layer.Cells;
        }

        /// <summary>
        /// Resets all layer areas.
        /// </summary>
        protected override void ResetArea()
        {
            if (!initDone)
                return;

            RenderRects = new Rectangle[area.Width * area.Height];
            
            for (int l = 0; l < LayerCount; l++)
                ResetAreaLayer(layers[l]);

            int index = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    RenderRects[index] = Font.GetRenderRect(x, y);
                    index++;
                }
            }

            // TODO: Optimization by calculating AbsArea and seeing if it's diff from current, if so, don't create new RenderRects
            AbsoluteArea = new Rectangle(0, 0, area.Width * Font.Size.X, area.Height * Font.Size.Y);

            if (LastRenderResult.Bounds.Size != AbsoluteArea.Size)
            {
                LastRenderResult.Dispose();
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            }
        }

        /// <summary>
        /// Sets up the layers render cells.
        /// </summary>
        /// <param name="layer"></param>
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
        /// Creates and adds a new layer based on an existing surface.
        /// </summary>
        /// <param name="surface">The surface. Must be the same width/height of this </param>
        /// <returns></returns>
        public Layer Add(ISurface surface)
        {
            if (surface.Cells.Length != width * height)
                throw new Exception("The length of cells passed in must match the width * height of this surface");

            var layer = new Layer();

            layer.Cells = surface.Cells;
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
        /// Saves the <see cref="LayeredSurface"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        /// <param name="layerMetadataType">Type of <see cref="Layer.Metadata"/>.</param>
        public void Save(string file, Type layerMetadataType)
        {
            Serializer.Save((SerializedTypes.LayeredSurfaceSerialized)this, file, new Type[] { layerMetadataType });
        }

        /// <summary>
        /// Loads a <see cref="LayeredSurface"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="layerMetadataType">Type of <see cref="Layer.Metadata"/>.</param>
        /// <returns>The <see cref="LayeredSurface"/>The deserialized service.</returns>
        public static LayeredSurface Load(string file, Type layerMetadataType)
        {
            return Serializer.Load<SerializedTypes.LayeredSurfaceSerialized>(file, new Type[] { layerMetadataType });
        }

        /// <summary>
        /// Sets the <see cref="Layer.Index"/> of each layer to the index it appears in the <see cref="layers"/>.
        /// </summary>
        protected void SyncLayerIndex()
        {
            int i = 0;
            foreach (var layer in layers)
            {
                layer.Index = i;
                i++;
            }
        }

        Cell[] serializationCells;
        Cell[] serializationRenderCells;
        Rectangle[] serializationRenderRects;

        [OnSerializing]
        private void BeforeSerialized(StreamingContext context)
        {
            serializationCells = cells;
            serializationRenderCells = RenderCells;
            serializationRenderRects = RenderRects;
            cells = null;
            RenderCells = null;
            RenderRects = null;
        }

        [OnSerialized]
        private void AfterSerialized(StreamingContext context)
        {
            cells = serializationCells;
            RenderCells = serializationRenderCells;
            RenderRects = serializationRenderRects;
            serializationCells = null;
            serializationRenderCells = null;
            serializationRenderRects = null;
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            initDone = true;
            SyncLayerIndex();
            ResetArea();
            SetActiveLayer(ActiveLayerIndex);
        }
        #endregion
    }
}
