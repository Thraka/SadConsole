using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.ImGuiSystem
{
    public abstract class ImGuiObjectBase
    {
        public abstract void BuildUI(ImGuiRenderer renderer);
    }
}
