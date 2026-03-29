using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using SadRogue.Primitives;
using UI = SadConsole.UI;

namespace Hexa.NET.ImGui;

public static partial class ImGuiSC
{
    /// <summary>
    /// Displays a confirmation popup dialog with a specified title and executes actions based on user response.
    /// </summary>
    /// <remarks>The popup presents 'Yes' and 'Cancel' options to the user. The corresponding action is
    /// invoked after the user makes a selection. The method should be called within the appropriate ImGui frame
    /// context.</remarks>
    /// <param name="id">A read-only span of bytes that uniquely identifies the popup instance. Used to manage the popup's open state.</param>
    /// <param name="text">A read-only span of bytes representing the text to display in the popup dialog.</param>
    /// <param name="onConfirmed">The action to execute if the user confirms the operation.</param>
    /// <param name="onCancel">The action to execute if the user cancels the operation. This parameter is optional.</param>
    /// <returns>true if the confirmation popup is currently open; otherwise, false.</returns>
    public static bool ConfirmPopup(ReadOnlySpan<byte> id, ReadOnlySpan<byte> text, Action? onConfirmed, Action? onCancel = null)
    {
        bool confirmed = false;

        if (ImGui.BeginPopup(id))
        {
            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out _);

            ImGui.Text(text);

            if (ImGui.Button("Cancel"))
                ImGui.CloseCurrentPopup();

            float pos = ImGui.CalcTextSize("Yes").X + framePadding.X / 2;
            ImGui.SameLine(ImGui.GetContentRegionAvail().X - pos);
            if (ImGui.Button("Yes"))
            {
                confirmed = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();

            if (!ImGui.IsPopupOpen(id))
            {
                if (confirmed)
                    onConfirmed?.Invoke();
                else
                    onCancel?.Invoke();

                return true;
            }
        }

        return false;
    }
}
