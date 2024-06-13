using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public static class RenameWindow
{
    static string _tempString = string.Empty;

    public static bool Show(string popupIdAndTitle, string message, ref string output, out bool accepted, Func<string, bool> validateText)
    {
        bool returnValue = false;

        ImGuiExt.CenterNextWindow();
        accepted = false;
        if (ImGui.BeginPopupModal(popupIdAndTitle))
        {
            ImGui.TextWrapped(message);
            
            ImGui.InputText("##input", ref _tempString, 512);

            if (ImGuiWindow.DrawButtons(out bool result, validateText(_tempString)))
            {
                output = _tempString;
                _tempString = string.Empty;

                if (result)
                {
                    returnValue = true;
                    accepted = true;
                }

                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGuiCore.State.CheckSetPopupOpen(popupIdAndTitle);

        return returnValue;
    }
}
