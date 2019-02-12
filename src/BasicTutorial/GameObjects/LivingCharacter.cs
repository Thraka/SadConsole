using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Entities;
using SadConsole.Maps;
using SadConsole;
using SadConsole.Actions;

namespace BasicTutorial.GameObjects
{
    abstract class LivingCharacter : SadConsole.GameObjects.GameObjectBase
    {
        protected int baseHealthMax = 10;
        protected int baseAttack = 10;
        protected int baseDefense = 10;
        protected int baseVisibilityDistance = 10;
        protected int baseLightSourceDistance = 5;

        /// <summary>
        /// Current health of the character
        /// </summary>
        public int Health { get; protected set; }

        /// <summary>
        /// The health max of the character
        /// </summary>
        public int HealthMax { get { return baseLightSourceDistance + GetInventoryHealthMods(); } }

        /// <summary>
        /// The attack of the character
        /// </summary>
        public int Attack { get { return baseLightSourceDistance + GetInventoryAttackMods(); } }

        /// <summary>
        /// The defense of the character
        /// </summary>
        public int Defense { get { return baseLightSourceDistance + GetInventoryDefenseMods(); } }

        /// <summary>
        /// How far you can see 
        /// </summary>
        public int VisibilityDistance { get { return baseVisibilityDistance + GetInventoryVisibilityMods(); } }

        /// <summary>
        /// How far your light source goes
        /// </summary>
        public int LightSourceDistance { get { return baseLightSourceDistance + GetInventoryLightingMods(); } }


        /// <summary>
        /// The inventory of a character.
        /// </summary>
        public Items.Inventory Inventory = new Items.Inventory();

        /// <summary>
        /// What tiles the character can see.
        /// </summary>
        public List<Tile> VisibleTiles = new List<Tile>();
        public List<Tile> LightSourceTiles = new List<Tile>();

        protected List<Tile> newVisibleTiles = new List<Tile>();
        protected List<Tile> newLightSourceTiles = new List<Tile>();

        protected Region currentRegion;

        protected GoRogue.FOV FOVSight;
        protected GoRogue.FOV FOVLighted;

        protected LivingCharacter(MapConsole map, Color foreground, Color background, int glyph) : base(map, foreground, background, glyph)
        {
            FOVSight = new GoRogue.FOV(map.MapToFOV);
            FOVLighted = new GoRogue.FOV(map.MapToFOV);
        }
        
        public override void ProcessGameFrame()
        {
            RefreshVisibilityTiles();
        }
        
        public void RefreshVisibilityTiles()
        {

            // Check to see if have left a room
            if (currentRegion != null && !currentRegion.InnerPoints.Contains(Position))
            {
                // If player, handle room lighting
                if (this == map.ControlledGameObject)
                {
                    foreach (var point in currentRegion.InnerPoints)
                    {
                        map[point].UnsetFlag(TileFlags.Lighted, TileFlags.InLOS);
                    }
                    foreach (var point in currentRegion.OuterPoints)
                    {
                        map[point].UnsetFlag(TileFlags.Lighted, TileFlags.InLOS);
                    }

                    foreach (var tile in FOVSight.CurrentFOV)
                        map[tile].SetFlag(TileFlags.InLOS);

                    foreach (var tile in FOVLighted.CurrentFOV)
                        map[tile].SetFlag(TileFlags.Lighted);
                }

                // We're not in this region anymore
                currentRegion = null;
            }

            // Not in a region, so find one.
            if (currentRegion == null)
            {
                // See if we're in a different region
                foreach (var region in map.Regions)
                {
                    if (region.InnerPoints.Contains(Position))
                    {
                        currentRegion = region;
                        break;
                    }
                }
            }

            // TODO: This code was placed here, got working, but the region code and
            //       newly unused variables have not been scrubbed.

            // BUG: If I exit a region and stand on the doorway, the tiles in the room
            //      that should be visible are not.

            // Visibility
            FOVSight.Calculate(Position, VisibilityDistance);

            // If player, handle LOS flags for tiles.
            if (this == map.ControlledGameObject)
            {
                foreach (var tile in FOVSight.NewlyUnseen)
                    map[tile].UnsetFlag(TileFlags.InLOS);

                foreach (var tile in FOVSight.NewlySeen)
                    map[tile].SetFlag(TileFlags.InLOS);
            }

            // Lighting
            FOVLighted.Calculate(Position, LightSourceDistance);

            if (this == map.ControlledGameObject)
            {
                foreach (var tile in FOVLighted.NewlyUnseen)
                    map[tile].UnsetFlag(TileFlags.Lighted);

                foreach (var tile in FOVLighted.NewlySeen)
                    map[tile].SetFlag(TileFlags.Lighted, TileFlags.Seen);
            }


            // Check and see if we're in a region, ensure these tiles are always visible and lighted.
            if (currentRegion != null)
            {
                Tile tile;

                // Make sure these are lit
                foreach (var point in currentRegion.InnerPoints)
                {
                    tile = map[point];

                    // If player, handle room lighting
                    if (this == map.ControlledGameObject)
                        tile.SetFlag(TileFlags.Lighted, TileFlags.InLOS, TileFlags.Seen);

                    // Add tile to visible list, for calculating if the player can see.
                    VisibleTiles.Add(tile);
                }

                foreach (var point in currentRegion.OuterPoints)
                {
                    tile = map[point];

                    // If player, handle room lighting
                    if (this == map.ControlledGameObject)
                        tile.SetFlag(TileFlags.Lighted, TileFlags.InLOS, TileFlags.Seen);

                    // Add tile to visible list, for calculating if the player can see.
                    VisibleTiles.Add(tile);
                }
            }
        }

        protected void AddVisibleTile(int x, int y)
        {
            if (map.IsTileValid(x, y))
            {
                Tile tile = map[x, y];
                tile.SetFlag(TileFlags.InLOS);
                newVisibleTiles.Add(tile);
            }
        }

        protected void AddLightVisbilityTile(int x, int y)
        {
            if (map.IsTileValid(x, y))
            {
                Tile tile = map[x, y];
                tile.SetFlag(TileFlags.Lighted);
                newLightSourceTiles.Add(tile);
            }
        }
        
        protected int GetInventoryHealthMods()
        {
            int result = 0;

            foreach (var item in Inventory.GetEquippedItems())
                result += item.HealthModifier;

            return result;
        }

        protected int GetInventoryAttackMods()
        {
            int result = 0;

            foreach (var item in Inventory.GetEquippedItems())
                result += item.AttackModifier;

            return result;
        }

        protected int GetInventoryDefenseMods()
        {
            int result = 0;

            foreach (var item in Inventory.GetEquippedItems())
                result += item.DefenseModifier;

            return result;
        }

        protected int GetInventoryVisibilityMods()
        {
            int result = 0;

            foreach (var item in Inventory.GetEquippedItems())
                result += item.VisibilityModifier;

            return result;
        }

        protected int GetInventoryLightingMods()
        {
            int result = 0;

            foreach (var item in Inventory.GetEquippedItems())
                result += item.LightingModifier;

            return result;
        }
    }
}
