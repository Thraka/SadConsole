using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Controls;

namespace SadConsole
{
    public static class Helpers
    {
        public static bool HasFlag(ControlStates state, ControlStates flag)
        {
            return (state & flag) == flag;
        }

        public static void SetFlag(ref ControlStates state, ControlStates flag)
        {
            state = state | flag;
        }

        public static void UnsetFlag(ref ControlStates state, ControlStates flag)
        {
            state = state & ~flag;
        }
    }
}
