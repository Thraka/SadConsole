using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SadConsole.Surfaces
{
    public partial class SurfaceEditor
    {
        /// <summary>
        /// Glyph indexes for a thin line.
        /// </summary>
        public static int[] LineStyleIndexesThin = { 218, 196, 191,
                                                     179, 197, 179,
                                                     192, 196, 217,

                                                          194,
                                                     195,      180,
                                                          193};

        /// <summary>
        /// Glyph indexes for a thick line.
        /// </summary>
        public static int[] LineStyleIndexesThick = { 201, 205, 187,
                                                      186, 206, 186,
                                                      200, 205, 188,
                                                       
                                                           203,
                                                      204,      185,
                                                           202};

        /// <summary>
        /// Array index enum for line glyphs.
        /// </summary>
        public enum LineRoadIndex : int
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            TopLeft, Top, TopRight,
            Left, Middle, Right,
            BottomLeft, Bottom, BottomRight,
            TopMiddleToDown,
            LeftMiddleToRight, RightMiddleToLeft,
            BottomMiddleToTop,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Connects all lines in a surface for both <see cref="LineStyleIndexesThin"/> and <see cref="LineStyleIndexesThick"/> styles.
        /// </summary>
        /// <param name="surface">The surface to process.</param>
        public static void ConnectLines(ISurface surface)
        {
            ConnectLines(surface, LineStyleIndexesThin);
            ConnectLines(surface, LineStyleIndexesThick);
        }

        /// <summary>
        /// Connects all lines in a surface based on the <paramref name="lineStyle"/> style provided.
        /// </summary>
        /// <param name="surface">The surface to process.</param>
        /// <param name="lineStyle">The array of line styles indexed by <see cref="LineRoadIndex"/>.</param>
        public static void ConnectLines(ISurface surface, int[] lineStyle)
        {
            Rectangle area = new Rectangle(0, 0, surface.Width, surface.Height);

            for (int x = 0; x < surface.Width; x++)
            {
                for (int y = 0; y < surface.Height; y++)
                {
                    Point pos = new Point(x, y);
                    int index = surface.GetIndexFromPoint(pos);

                    // Check if this pos is a road
                    if (!lineStyle.Contains(surface[index].Glyph))
                        continue;

                    // Get all valid positions and indexes around this point
                    var valids = GameHelpers.Directions.GetValidDirections(pos, area);
                    var posIndexes = GameHelpers.Directions.GetDirectionIndexes(pos, area);
                    var roads = new bool[] { false, false, false, false, false, false, false, false, false };

                    for (int i = 0; i < 8; i++)
                    {
                        if (valids[i])
                            if (lineStyle.Contains(surface[posIndexes[i]].Glyph))
                                roads[i] = true;
                    }

                    if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.Middle];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.TopMiddleToDown];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.BottomMiddleToTop];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.RightMiddleToLeft];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.LeftMiddleToRight];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    (roads[(int)GameHelpers.Directions.DirectionEnum.East] ||
                    roads[(int)GameHelpers.Directions.DirectionEnum.West]))
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.Top];
                        surface.IsDirty = true;
                    }
                    else if ((roads[(int)GameHelpers.Directions.DirectionEnum.North] ||
                    roads[(int)GameHelpers.Directions.DirectionEnum.South]) &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.Left];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.BottomRight];
                        surface.IsDirty = true;
                    }
                    else if (roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {

                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.BottomLeft];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {
                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.TopRight];
                        surface.IsDirty = true;
                    }
                    else if (!roads[(int)GameHelpers.Directions.DirectionEnum.North] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.South] &&
                    roads[(int)GameHelpers.Directions.DirectionEnum.East] &&
                    !roads[(int)GameHelpers.Directions.DirectionEnum.West])
                    {
                        surface[index].Glyph = lineStyle[(int)LineRoadIndex.TopLeft];
                        surface.IsDirty = true;
                    }
                }
            }

        }

    }
}
