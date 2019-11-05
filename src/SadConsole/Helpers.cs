#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    using System.Runtime.CompilerServices;
    using SadConsole.Controls;

    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(ControlStates state, ControlStates flag) => (state & flag) == flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref ControlStates state, ControlStates flag) => state = state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag(ref ControlStates state, ControlStates flag) => state = state & ~flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(in int state, in int flag) => (state & flag) == flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetFlag(int state, int flag) => state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnsetFlag(int state, int flag) => state & ~flag;

        /// <summary>
        /// Gets the x,y of an index on the surface.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <param name="width">Width that includes the index.</param>
        /// <returns>The x,y as a <see cref="Point"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point GetPointFromIndex(int index, int width) => new Point(index % width, index / width);

        /// <summary>
        /// Gets the index of a location on the surface by coordinate.
        /// </summary>
        /// <param name="x">The x of the location.</param>
        /// <param name="y">The y of the location.</param>
        /// <param name="width">Width that includes the point.</param>
        /// <returns>The cell index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndexFromPoint(int x, int y, int width) => y * width + x;
    }
}
