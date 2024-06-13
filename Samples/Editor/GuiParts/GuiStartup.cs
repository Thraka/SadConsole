using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts
{
    public class GuiStartup : ImGuiObjectBase
    {
        public override void BuildUI(ImGuiRenderer renderer)
        {
            ImGuiCore.State.IsPopupOpen = false;
        }
    }
}
