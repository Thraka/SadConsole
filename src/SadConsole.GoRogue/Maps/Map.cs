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
    public class GameObjectManager: SpatialMap<GameObjects.GameObjectBase>
    {
        WeakReference<SimpleMap> mapReference;
        Rectangle cachedViewPort;

        public GameObjectManager(SimpleMap map)
        {
            this.mapReference = new WeakReference<SimpleMap>(map);
            this.cachedViewPort = map.Surface.ViewPort;
            this.ItemAdded += GameObjectManager_ItemAdded;
            this.ItemRemoved += GameObjectManager_ItemRemoved;
            this.ItemMoved += GameObjectManager_ItemMoved;
        }

        private void GameObjectManager_ItemMoved(object sender, ItemMovedEventArgs<GameObjects.GameObjectBase> e)
        {
            mapReference.TryGetTarget(out SimpleMap map);
            if (map == null) return;

            e.Item.MoveTo(e.NewPosition.ToPoint(), map);
            e.Item.IsVisible = map.Surface.ViewPort.Contains(e.Item.Position);
        }

        private void GameObjectManager_ItemRemoved(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
        }

        private void GameObjectManager_ItemAdded(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
            mapReference.TryGetTarget(out SimpleMap map);
            if (map == null) return;

            e.Item.MoveTo(e.Position.ToPoint(), map);
            e.Item.IsVisible = map.Surface.ViewPort.Contains(e.Item.Position);
        }

        public void SyncView()
        {
            mapReference.TryGetTarget(out SimpleMap map);
            if (map == null) return;
            if (cachedViewPort == map.Surface.ViewPort) return;

            cachedViewPort = map.Surface.ViewPort;

            foreach (var item in Items)
            {
                item.PositionOffset = map.Surface.ViewPort.Location;
                item.IsVisible = map.Surface.ViewPort.Contains(item.Position);
            }
        }
    }

    public class SimpleMap: ScreenObject, ISettableMapView<Tile>, IEnumerable<Tile>
    {
        public Surfaces.Basic Surface;
        //public Entities.EntityManager EntityManager;
        protected GoRogue.SpatialMap<GameObjects.GameObjectBase> EntityManager;

        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Instance of the FOV translator
        /// </summary>
        public TranslationFOV MapToFOV { get; private set; }
        
        private Tile[] Tiles;

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
        public SimpleMap(int width, int height, Rectangle viewPort)
        {
            Width = width;
            Height = height;
            
            // Create our tiles for the map
            Tiles = new Tile[width * height];
            Regions = new List<Region>();

            // Be efficient by not using factory.Create each tile below. Instead, get the blueprint and use that to create each tile.
            var defaultTile = Tile.Factory.GetBlueprint("wall");

            // Fill the map with walls.
            for (var i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = defaultTile.Create();
                Tiles[i].Position = new Point(i % Width, i / Width);
                Tiles[i].TileChanged += SimpleMap_TileChanged;
            }

            // Instance of the FOV translator.
            MapToFOV = new TranslationFOV(this);

            Surface = new SadConsole.Surfaces.Basic(width, height, SadConsole.Global.FontDefault, viewPort, Tiles);
            //EntityManager = new Entities.EntityManager();
            
            // Parent the entity manager to the view surface of the map.
            //Surface.Children.Add(EntityManager);

            EntityManager = new GameObjectManager(this);

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
        /// Returns an entity by position.
        /// </summary>
        /// <param name="position">The position of the entity.</param>
        /// <returns>The entity at the position, otherwise null.</returns>
        public Entities.Entity GetEntity(Point position)
        {
            return EntityManager.GetItem(position.ToCoord());

            //foreach (var ent in EntityManager.Entities)
            //{
            //    if (ent.Position == position)
            //        return ent;
            //}

            //return null;
        }

        /// <summary>
        /// Gets a tile from the map.
        /// </summary>
        /// <param name="position">The map position of the tile.</param>
        /// <returns>The tile if it exists, otherwise null.</returns>
        public Tile GetTile(Point position)
        {
            if (IsTileValid(position.X, position.Y))
                return this[position];
            else
                return null;
        }

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
