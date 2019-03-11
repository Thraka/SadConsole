using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GoRogue;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using GoRogue.MapViews;
using SadConsole;

namespace SadConsole.Maps
{
    /// <summary>
    /// Console representing a map.
    /// </summary>
    public class MapConsole: ScrollingConsole, ISettableMapView<Tile>, IEnumerable<Tile>
    {
        /// <summary>
        /// The game objects added to the map.
        /// </summary>
        public Components.EntityManager GameObjects { get; protected set; }

        /// <summary>
        /// The game object that will be controlled by the player.
        /// </summary>
        public GameObjects.GameObjectBase ControlledGameObject { get; set; }

        /// <summary>
        /// Fires when a map tile is replaced or gets an alert that its state has changed.
        /// </summary>
        public event EventHandler<TileChangedEventArgs> MapTileChanged;

        /// <summary>
        /// Instance of the FOV translator
        /// </summary>
        public TranslationFOV MapToFOV { get; }
        
        protected Tile[] Tiles;

        public List<Region> Regions;

        public new Tile this[int index]
        {
            get => Tiles[index];
            set => SetupTile(value, index.ToPoint(Width));
        }

        public new Tile this[int x, int y]
        {
            get => Tiles[Helpers.GetIndexFromPoint(x, y, Width)];
            set => SetupTile(value, new Point(x, y));
        }

        public Tile this[Coord position]
        {
            get => Tiles[position.ToIndex(Width)];
            set => SetupTile(value, position);
        }

        public Tile this[Point position]
        {
            get => Tiles[position.ToIndex(Width)];
            set => SetupTile(value, position);
        }

        /// <summary>
        /// Creates a map with the specified width and height. Uses a view port to display a subset of the map.
        /// </summary>
        /// <param name="width">The total width of the map.</param>
        /// <param name="height">The total height of the map.</param>
        /// <param name="defaultTileBlueprint">The tile blueprint used to fill the map.</param>
        public MapConsole(int width, int height, string defaultTileBlueprint = "wall"): base(width, height)
        {
            Width = width;
            Height = height;
            
            // Create our tiles for the map
            Tiles = new Tile[width * height];

            Regions = new List<Region>();

            // Be efficient by not using factory.Create each tile below. Instead, get the blueprint and use that to create each tile.
            var defaultTile = Tile.Factory.GetBlueprint(defaultTileBlueprint);

            // Fill the map with walls
            for (var i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = defaultTile.Create();
                Tiles[i].Position = new Point(i % Width, i / Width);
                Tiles[i].TileChanged += SimpleMap_TileChanged;
            }

            Cells = Tiles;
            OnCellsReset();

            // Instance of the FOV translator.
            MapToFOV = new TranslationFOV(this);

            GameObjects = new Components.EntityManager();
            Components.Add(GameObjects);
        }

        private void SetupTile(Tile tile, Point position)
        {
            // Dehook
            this[position].TileChanged -= SimpleMap_TileChanged;

            tile.Position = position;
            tile.TileChanged += SimpleMap_TileChanged;
            Tiles[position.ToIndex(Width)] = tile;
        }

        private void SimpleMap_TileChanged(object sender, EventArgs e) => MapTileChanged?.Invoke(this, new TileChangedEventArgs(this, (Tile)sender));
        
        /// <summary>
        /// Gets a tile from the map.
        /// </summary>
        /// <param name="position">The map position of the tile.</param>
        /// <returns>The tile if it exists, otherwise null.</returns>
        public Tile GetTile(Point position) => IsTileValid(position.X, position.Y) ? this[position] : null;

        /// <summary>
        /// Test if a tile blocks movement.
        /// </summary>
        /// <param name="x">Tile coordinate x.</param>
        /// <param name="y">Tile coordinate y.</param>
        /// <returns>True if the tile blocks movement.</returns>
        public bool IsTileWalkable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            return ! Helpers.HasFlag(Tiles[y * Width + x].Flags, (int)TileFlags.BlockMove);
        }

        /// <summary>
        /// Test if a tile blocks line of sight.
        /// </summary>
        /// <param name="x">Tile coordinate x.</param>
        /// <param name="y">Tile coordinate y.</param>
        /// <returns>True if the tile blocks light of sight.</returns>
        public bool IsTileOpaque(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return true;

            return Helpers.HasFlag(Tiles[y * Width + x].Flags, (int)TileFlags.BlockLOS);
        }

        /// <summary>
        /// Test if a tile coordinate is within the map bounds.
        /// </summary>
        /// <param name="x">Tile coordinate x.</param>
        /// <param name="y">Tile coordinate y.</param>
        /// <returns>True if the tile exists.</returns>
        public bool IsTileValid(int x, int y) => x >= 0 || y >= 0 || x < Width || y > Height;

        /// <summary>
        /// Returns a random floor tile.
        /// </summary>
        /// <returns>A tile that is not a wall.</returns>
        public Tile FindEmptyTile()
        {
            while (true)
            {
                var foundTile = this[this.RandomPosition(GoRogue.Random.SingletonRandom.DefaultRNG)];

                if (!Helpers.HasFlag(foundTile.Flags, (int)TileFlags.BlockMove))
                    return foundTile;
            }
        }
        
        /// <summary>
        /// Translates FOV values between GoRogue and our game tiles.
        /// </summary>
        public class TranslationFOV : GoRogue.MapViews.TranslationMap<Tile, bool>
        {
            public TranslationFOV(GoRogue.MapViews.IMapView<Tile> map) : base(map) { }

            protected override bool TranslateGet(Tile value)
            {
                // true = blocked
                return !Helpers.HasFlag(value.Flags, (int)TileFlags.BlockLOS);
            }
        }

        public new IEnumerator<Tile> GetEnumerator()
        {
            return new List<Tile>(Tiles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Event arguments for when a tile changed event fires.
        /// </summary>
        public class TileChangedEventArgs: EventArgs
        {
            /// <summary>
            /// The tile that changed.
            /// </summary>
            public readonly Tile Tile;

            /// <summary>
            /// The map that owns the tile.
            /// </summary>
            public readonly MapConsole Map;

            public TileChangedEventArgs(MapConsole map, Tile tile)
            {
                Map = map;
                Tile = tile;
            }
        }
    }
}
