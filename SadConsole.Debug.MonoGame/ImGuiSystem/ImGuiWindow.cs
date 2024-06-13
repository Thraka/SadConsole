using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace SadConsole.ImGuiSystem
{
    public abstract class ImGuiWindow : ImGuiObjectBase
    {
        public string Title { get; set; } = "";

        public bool IsOpen;

        public event EventHandler Closed;

        public bool DialogResult;

        protected void OnClosed() =>
            Closed?.Invoke(this, EventArgs.Empty);


        public static bool DrawButtons(out bool result, bool acceptDisabled = false)
        {
            bool buttonClicked = false;
            result = false;

            ImGui.Separator();

            if (ImGui.Button("Cancel")) { buttonClicked = true; }

            // Right-align button
            float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
            ImGui.SameLine(ImGui.GetWindowWidth() - pos);

            ImGui.BeginDisabled(acceptDisabled);
            if (ImGui.Button("Accept"))
            {
                buttonClicked = true;
                result = true;
            }
            ImGui.EndDisabled();

            return buttonClicked;
        }
    }
}
