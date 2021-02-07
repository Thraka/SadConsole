using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    public abstract class ImGuiObjectBase
    {
        internal protected abstract void BuildUI(ImGuiRenderer renderer);
    }
}
