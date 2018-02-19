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
        public ToolBrush Brush { get; private set; } = new ToolBrush(1, 1);

        public System.Windows.Forms.Control UI => new System.Windows.Forms.Button() { Width = 100, Height = 25, Text = "Refresh", Name = "ToolPanel" };

        public void Refresh()
        {

        }

        public void OnUpdate(MouseConsoleState mouse)
        {

        }
    }
}
