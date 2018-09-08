using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SadConsole.Controls;

namespace SadConsole
{
    public static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(ControlStates state, ControlStates flag)
        {
            return (state & flag) == flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref ControlStates state, ControlStates flag)
        {
            state = state | flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag(ref ControlStates state, ControlStates flag)
        {
            state = state & ~flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag(in int state, int flag)
        {
            return (state & flag) == flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref int state, int flag)
        {
            state = state | flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag(ref int state, int flag)
        {
            state = state & ~flag;
        }
    }
}
