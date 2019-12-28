using Console = SadConsole.Console;
using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsoleEditor.Panels;

namespace SadConsoleEditor.Tools
{
    public interface ITool
    {
        string Id { get; }

        string Title { get; }

        char Hotkey { get; }

        void OnSelected();

        void OnDeselected();

        bool ProcessKeyboard(Keyboard info, Console surface);

        void ProcessMouse(MouseScreenObjectState info, Console surface, bool isInBounds);

        void RefreshTool();

        void Update();

        CustomPanel[] ControlPanels { get; }
    }
}
