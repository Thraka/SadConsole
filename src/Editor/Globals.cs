using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    static class Globals
    {
        private static bool blockSadConsoleInput = false;

        public static bool BlockSadConsoleInput
        {
            get => blockSadConsoleInput;
            set
            {
                blockSadConsoleInput = value;
                if (value)
                {
                    // Input has been blocked, make sure no "console update" gets continual input.
                    SadConsole.Global.KeyboardState.Clear();
                    SadConsole.Global.MouseState.Clear();
                }
            }
        }

        public static Noesis.Grid RootFrameworkElement => (Noesis.Grid)SadConsole.EditorGameComponent.noesisGUIWrapper.ControlTreeRoot;

    }
}
