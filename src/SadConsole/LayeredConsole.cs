#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SadConsole.SerializedTypes;

    public class CellSurfaceLayer : CellSurface
    {
        public CellSurfaceLayer(int width, int height) : base(width, height) { }

        public CellSurfaceLayer(int width, int height, Cell[] initialCells) : base(width, height, initialCells) { }

        public bool IsVisible { get; set; } = true;

        public string Name { get; set; }

        protected override void OnCellsReset() => throw new Exception("This surface cannot be resized");
    }

    /// <summary>
    /// Represents mutliple surfaces grouped together and rendered at the same time.
    /// </summary>
    [JsonConverter(typeof(LayeredJsonConverter))]
    [System.Diagnostics.DebuggerDisplay("Layered Surface")]
    public class LayeredConsole : ScrollingConsole
    {
        internal List<CellSurfaceLayer> _layers;

        public LayeredConsole(int width, int height, int layers) : this(width, height, layers, SadConsole.Global.FontDefault, new Rectangle(0, 0, width, height)) { }

        public LayeredConsole(int width, int height, int layers, Font font) : this(width, height, layers, font, new Rectangle(0, 0, width, height)) { }

        public LayeredConsole(int width, int height, int layers, Font font, Rectangle viewPort) : base(width, height, font, viewPort)
        {
            IsCursorDisabled = true;

            if (layers <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(layers), "Layer count must be 1 or more.");
            }

            _layers = new List<CellSurfaceLayer>(layers);

            for (int i = 0; i < layers; i++)
            {
                AddLayer(new CellSurfaceLayer(width, height));
            }

            Renderer = new Renderers.LayeredConsole() { Layers = _layers };

            Cells = _layers[0].Cells;
        }

        public LayeredConsole(int width, int height, IEnumerable<CellSurfaceLayer> layers) : base(width, height, SadConsole.Global.FontDefault, new Rectangle(0, 0, width, height)) { }

        public LayeredConsole(int width, int height, IEnumerable<CellSurfaceLayer> layers, Font font) : base(width, height, font, new Rectangle(0, 0, width, height)) { }

        public LayeredConsole(int width, int height, IEnumerable<CellSurfaceLayer> layers, Font font, Rectangle viewPort) : base(width, height, font, viewPort)
        {
            IsCursorDisabled = true;

            if (layers == null)
            {
                _layers = new List<CellSurfaceLayer>();
            }
            else
            {
                _layers = new List<CellSurfaceLayer>(layers);
            }

            foreach (CellSurfaceLayer layer in layers)
            {
                if (layer.Width != width || layer.Height != height)
                {
                    throw new ArgumentException(nameof(layers), "One of the layers in the array does not match the size of the layered console.");
                }
            }

            Renderer = new Renderers.LayeredConsole() { Layers = _layers };
        }

        public override void Update(TimeSpan timeElapsed)
        {
            if (IsPaused)
            {
                return;
            }

            foreach (CellSurfaceLayer layer in _layers)
            {
                layer.Effects.UpdateEffects(timeElapsed.TotalSeconds);
            }

            var copyList = new List<Console>(Children);

            foreach (Console child in copyList)
            {
                child.Update(timeElapsed);
            }
        }

        public CellSurfaceLayer GetLayer(int index) => _layers[index];

        public void AddLayer(CellSurfaceLayer item)
        {
            _layers.Add(item);
            Cells = _layers[0].Cells;
            IsDirty = true;
        }

        public void ClearLayers()
        {
            _layers.Clear();

            Cells = new Cell[Width * Height];

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
            }

            OnCellsReset();

            IsDirty = true;
        }

        public bool ContainsLayer(CellSurfaceLayer item) => _layers.Contains(item);

        public bool RemoveLayer(CellSurfaceLayer item)
        {
            bool result = _layers.Remove(item);

            if (_layers.Count == 0)
            {
                Cells = new Cell[Width * Height];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
                }

                OnCellsReset();
            }
            else
            {
                Cells = _layers[0].Cells;
            }

            IsDirty = true;

            return result;
        }

        public int IndexOf(CellSurfaceLayer item) => _layers.IndexOf(item);

        public void InsertLayer(int index, CellSurfaceLayer item)
        {
            _layers.Insert(index, item);
            Cells = _layers[0].Cells;
            IsDirty = true;
        }

        public void RemoveLayerAt(int index)
        {
            _layers.RemoveAt(index);

            if (_layers.Count == 0)
            {
                Cells = new Cell[Width * Height];

                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell(DefaultForeground, DefaultBackground, 0);
                }

                OnCellsReset();
            }
            else
            {
                Cells = _layers[0].Cells;
            }

            IsDirty = true;
        }

        public int LayerCount => _layers.Count;

        /// <summary>
        /// Saves the <see cref="SurfaceBase"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="SurfaceBase"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static LayeredConsole Load(string file) => Serializer.Load<LayeredConsole>(file, Settings.SerializationIsCompressed);
    }
}
