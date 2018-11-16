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
    public class SimpleMap: ScreenObject, ISettableMapView<Tile>, IEnumerable<Tile>
    {
        public Surfaces.Basic Surface { get; set; }
        
        public GameObjects.GameObjectManager GameObjects { get; protected set; }

        public GameObjects.GameObjectBase ControlledGameObject { get; set; }

        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Instance of the FOV translator
        /// </summary>
        public TranslationFOV MapToFOV { get; }
        
        protected Tile[] Tiles;

        public List<Region> Regions;

        public Tile this[int index]
        {
            get => Tiles[index];
            set => SetupTile(value, new Point(index % Width, index / Width));
        }

        public Tile this[int x, int y]
        {
            get => Tiles[Surface.GetIndexFromPoint(x, y)];
            set => SetupTile(value, new Point(x, y));
        }

        public Tile this[Coord position]
        {
            get => Tiles[position.ToIndex(Width)];
            set => SetupTile(value, position.ToPoint());
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
        /// <param name="viewPort">A view of the map that is displayed on the screen.</param>
        /// <param name="defaultTileBlueprint">The tile blueprint used to fill the map.</param>
        /// <param name="font">The font used with the map. Defaults to <see cref="SadConsole.Global.FontDefault"/>.</param>
        public SimpleMap(int width, int height, Rectangle viewPort, string defaultTileBlueprint = "wall", Font font = null)
        {
            Width = width;
            Height = height;

            if (font == null)
                font = SadConsole.Global.FontDefault;

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

            // Instance of the FOV translator.
            MapToFOV = new TranslationFOV(this);
            
            Surface = new SadConsole.Surfaces.Basic(width, height, font, viewPort, Tiles);
            GameObjects = new GameObjects.GameObjectManager(this);
            
            // Parent the surface to the map.
            Children.Add(Surface);
        }

        private void SetupTile(Tile tile, Point position)
        {
            // Dehook
            this[position].TileChanged -= SimpleMap_TileChanged;

            tile.Position = position;
            tile.TileChanged += SimpleMap_TileChanged;
            Tiles[position.ToIndex(Width)] = tile;
            Surface.SetRenderCells();
        }

        private void SimpleMap_TileChanged(object sender, EventArgs e) => Surface.IsDirty = true;

        /// <summary>
        /// Returns <see cref="SadConsole.GameObjects.GameObjectBase"/> by position.
        /// </summary>
        /// <param name="position">The position of the <see cref="SadConsole.GameObjects.GameObjectBase"/>.</param>
        /// <returns>The <see cref="SadConsole.GameObjects.GameObjectBase"/> at the position, otherwise null.</returns>
        public GameObjects.GameObjectBase GetGameObject(Point position) => GameObjects.GetItem(position.ToCoord());

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

        public override void Draw(TimeSpan timeElapsed)
        {
            base.Draw(timeElapsed);

            foreach (var gameObject in GameObjects)
                gameObject.Item.Draw(timeElapsed);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            foreach (var gameObject in GameObjects)
                gameObject.Item.Update(timeElapsed);

            GameObjects.SyncView();
        }

        /// <summary>
        /// Translates FOV values between GoRogue and our game tiles.
        /// </summary>
        public class TranslationFOV : GoRogue.MapViews.TranslationMap<Tile, double>
        {
            public TranslationFOV(GoRogue.MapViews.IMapView<Tile> map) : base(map) { }

            protected override double TranslateGet(Tile value)
            {
                // 1 = blocked; 0 = see thru
                return Helpers.HasFlag(value.Flags, (int)TileFlags.BlockLOS)  ? 1.0 : 0.0;
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return new List<Tile>(Tiles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
