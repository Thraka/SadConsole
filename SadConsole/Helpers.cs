using System.Runtime.CompilerServices;
using SadRogue.Primitives;

namespace SadConsole
{
    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(in int state, in int flag) => (state & flag) == flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetFlag(int state, int flag) => state | flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnsetFlag(int state, int flag) => state & ~flag;
    }
}
