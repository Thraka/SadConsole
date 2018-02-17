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

        System.Windows.Forms.Control GetUI();

        void OnUpdate(MouseConsoleState mouse);
    }
}
