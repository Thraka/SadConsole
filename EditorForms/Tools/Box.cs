using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    internal class Box : ITool
    {
        public string Name => "Box";
        public ToolBrush Brush { get; private set; } = new ToolBrush(1, 1);

        public System.Windows.Forms.Control GetUI()
        {
            return new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Box", Name = "ToolPanel" };
        }

        public void OnUpdate(MouseConsoleState mouse)
        {

        }
    }
}
