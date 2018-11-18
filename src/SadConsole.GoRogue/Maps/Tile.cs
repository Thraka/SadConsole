using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole.Actions;

namespace SadConsole.Maps
{
    /// <summary>
    /// A map tile.
    /// </summary>
    public partial class Tile : Cell
    {
        protected int tileState;
        protected int tileType;
        protected int flags;

        protected Cell AppearanceNormal;
        protected Cell AppearanceDim;

        public static Cell AppearanceNeverSeen = new Cell(Color.Black, Color.Black, '.');
        public static float DimAmount = 0.4f;

        public event EventHandler TileChanged;

        public string Title { get; set; }
        public string Description { get; set; }

        public Action<Tile, int> OnTileStateChanged { get; set; }
        public Action<Tile, int> OnTileFlagsChanged { get; set; }
        public Action<Tile, ActionBase> OnProcessAction { get; set; }


        public string DefinitionId { get; protected set; }

        /// <summary>
        /// The type of tile represented.
        /// </summary>
        public int Type => tileType;

        /// <summary>
        /// Flags for the tile such as blocks LOS.
        /// </summary>
        public int Flags
        {
            get => flags;
            set
            {
                if (flags == value) return;

                var oldFlags = flags;
                flags = value;
                OnTileFlagsChanged?.Invoke(this, oldFlags);
                UpdateAppearance();
                TileChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The state of the tile.
        /// </summary>
        public int TileState
        {
            get => tileState;
            set
            {
                if (tileState == value) return;

                var oldState = tileState;
                tileState = value;
                OnTileStateChanged?.Invoke(this, oldState);
                UpdateAppearance();
                TileChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// Where this tile is located on the map.
        /// </summary>
        public Point Position { get; set; }
        
        public Tile(Color foreground, Color background, int glyph) : base(foreground, background, glyph)
        {

            Color dimFore = foreground * DimAmount;
            Color dimBack = background * DimAmount;
            dimFore.A = 255;
            dimBack.A = 255;

            AppearanceDim = new Cell(dimFore, dimBack, glyph);
            AppearanceNormal = new Cell(foreground, background, glyph);

            AppearanceNeverSeen.CopyAppearanceTo(this);
        }

        public Tile() : base(Color.Transparent, Color.Transparent, 0)
        {
        }
        
        public void ChangeAppearance(Cell normal)
        {
            Color dimFore = normal.Foreground * DimAmount;
            Color dimBack = normal.Background * DimAmount;
            dimFore.A = 255;
            dimBack.A = 255;

            ChangeAppearance(normal, new Cell(dimFore, dimBack, normal.Glyph));
        }

        public void ChangeAppearance(Cell normal, Cell dim)
        {
            AppearanceNormal = normal;
            AppearanceDim = dim;

            UpdateAppearance();
        }

        public void ChangeGlyph(int glyph)
        {
            AppearanceNormal.Glyph = glyph;
            AppearanceDim.Glyph = glyph;

            UpdateAppearance();
        }

        /// <summary>
        /// Adds the specified flags to the <see cref="flags"/> property.
        /// </summary>
        /// <param name="flags">The flags to set.</param>
        public void SetFlag(params TileFlags[] flags)
        {
            var total = 0;

            foreach (var flag in flags)
                total = total | (int)flag;

            Flags = Helpers.SetFlag(this.flags, total);
        }
        /// <summary>
        /// Removes the specified flags to the <see cref="flags"/> property.
        /// </summary>
        /// <param name="flags">The flags to remove.</param>
        public void UnsetFlag(params TileFlags[] flags)
        {
            var total = 0;

            foreach (var flag in flags)
                total = total | (int)flag;

            Flags = Helpers.UnsetFlag(this.flags, total);
        }

        public void ProcessAction(Actions.ActionBase action) => OnProcessAction?.Invoke(this, action);

        protected virtual void UpdateAppearance()
        {
            if (!Helpers.HasFlag(in flags, (int)TileFlags.Seen))
            {
                AppearanceNeverSeen.CopyAppearanceTo(this);
            }
            else if ((Helpers.HasFlag(in flags, (int)TileFlags.InLOS) || Helpers.HasFlag(in flags, (int)TileFlags.PermaInLOS))
                     && Helpers.HasFlag(in flags, (int)TileFlags.Lighted) || Helpers.HasFlag(in flags, (int)TileFlags.PermaLight) || Helpers.HasFlag(in flags, (int)TileFlags.RegionLighted))
            {
                AppearanceNormal.CopyAppearanceTo(this);
            }
            else // Seen but not lighted/los
            {
                AppearanceDim.CopyAppearanceTo(this);
            }

            TileChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
