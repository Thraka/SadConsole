using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    internal class Recolor : ITool
    {
        public string Name => "Recolor";

        public System.Windows.Forms.Control GetUI()
        {
            return new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Recolor", Name = "ToolPanel" };
        }

        public void OnUpdate(MouseConsoleState mouse)
        {

        }
    }
}
