using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    internal interface ITool
    {
        string Name { get; }

        ToolBrush Brush { get; }

        System.Windows.Forms.Control UI { get; }

        void Refresh();

        void OnUpdate(MouseConsoleState mouse);
    }
}
