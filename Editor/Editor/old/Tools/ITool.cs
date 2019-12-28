using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsoleEditor.Panels;
using SadConsole.Surfaces;

namespace SadConsoleEditor.Tools
{
    interface ITool
    {
        string Id { get; }

        string Title { get; }

        char Hotkey { get; }

        void OnSelected();

        void OnDeselected();

        bool ProcessKeyboard(Keyboard info, SurfaceBase surface);

        void ProcessMouse(MouseConsoleState info, SurfaceBase surface, bool isInBounds);

        void RefreshTool();

        void Update();

        CustomPanel[] ControlPanels { get; }
    }
}
