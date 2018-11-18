using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SadConsole.Maps
{
    public partial class Tile: IFactoryObject
    {
        /// <summary>Represents a floor tile type.</summary>
        internal const int TileTypeFloor = 0;

        /// <summary>Represents a wall tile type.</summary>
        internal const int TileTypeWall = 1;

        /// <summary>
        /// The factory instance to generate tiles from.
        /// </summary>
        public static TileFactory Factory;

        static Tile()
        {
            Factory = new TileFactory
            {
                new TileBlueprint("wall")
                {
                    Appearance = new Cell(Color.White, Color.Gray, 176),
                    Flags = (int) (TileFlags.BlockLOS | TileFlags.BlockMove),
                    Type = TileTypeWall,
                    Title = "Wall",
                    Description = "Crumbling stone wall."
                },

                new TileBlueprint("floor")
                {
                    Appearance = new Cell(new Color(120, 120, 120), Color.Black, 46),
                    Type = TileTypeFloor,
                    Title = "Floor",
                    Description = "Ancient rock and dirt."
                }
            };

        }

        /// <summary>
        ///  Simple implementation of the Factory class that deals with tiles.
        /// </summary>
        public class TileFactory: Factory<TileBlueprint, Tile> { }

        /// <summary>
        /// A simple blueprint representing a tile.
        /// </summary>
        public class TileBlueprint: IFactoryBlueprint<Tile>
        {
            public string Id { get; }

            public string Title { get; set; }
            public string Description { get; set; }
            public int Flags { get; set; }
            public int Type { get; set; }
            public int State { get; set; }
            public Cell Appearance { get; set; }
            public Action<Tile, int> OnTileStateChanged { get; set; }
            public Action<Tile, int> OnTileFlagsChanged { get; set; }
            public Action<Tile, SadConsole.Actions.ActionBase> OnProcessAction { get; set; }

            public TileBlueprint(string id) => Id = id;

            public virtual Tile Create()
            {
                var tile = new Tile(Appearance.Foreground, Appearance.Background, Appearance.Glyph)
                {
                    tileType = Type,
                    Title = Title,
                    Description = Description,
                    Flags = Flags,
                    TileState = State,
                    DefinitionId = Id,
                    OnTileStateChanged = OnTileStateChanged,
                    OnTileFlagsChanged = OnTileFlagsChanged,
                    OnProcessAction = OnProcessAction
                };

                return tile;
            }
        }
    }
}
