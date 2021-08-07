using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    public abstract class ImGuiWindow: ImGuiObjectBase
    {
        public string Title { get; set; }

        public bool IsOpen;
    }
}
