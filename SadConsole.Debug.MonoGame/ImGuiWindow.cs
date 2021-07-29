using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    public abstract class ImGuiObjectBase
    {
        public abstract void BuildUI(ImGuiRenderer renderer);
    }
}
