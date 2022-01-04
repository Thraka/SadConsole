using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.Model
{
    public class DocumentSurfaceSettings : IDocumentSettings
    {
        public int Width = 80;
        public int Height = 25;

        public Vector4 DefaultForeground = Color.White.ToVector4();
        public Vector4 DefaultBackground = Color.Black.ToVector4();

        public void BuildUINew(ImGuiRenderer renderer)
        {
            float paddingX = ImGui.GetStyle().WindowPadding.X;
            float windowWidth = ImGui.GetWindowWidth();

            ImGui.Text("Width: ");
            ImGui.SameLine(ImGui.CalcTextSize("Height: ").X + (ImGui.GetStyle().ItemSpacing.X * 2));
            ImGui.InputInt("##docwidth", ref Width);

            ImGui.Text("Height: ");
            ImGui.SameLine();
            ImGui.InputInt("##docheight", ref Height);
            
            ImGui.Text("Def. Foreground: ");
            ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
            ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.LayoutInfo.ColorEditBoxWidth = ImGui.GetItemRectSize().X;

            ImGui.Text("Def. Background: ");
            ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
            ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs);
        }

        public void BuildUIEdit(ImGuiRenderer renderer, bool readOnly)
        {
            float paddingX = ImGui.GetStyle().WindowPadding.X;
            float windowWidth = ImGui.GetWindowWidth();

            ImGui.Text("Width: ");
            ImGui.SameLine(windowWidth - paddingX - 100);
            ImGui.SetNextItemWidth(100);
            ImGui.Text(Width.ToString());

            ImGui.Text("Height: ");
            ImGui.SameLine(windowWidth - paddingX - 100);
            ImGui.SetNextItemWidth(100);
            ImGui.Text(Height.ToString());
        }
    }
}
