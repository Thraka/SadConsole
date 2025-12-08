using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;


public static class FontSizePopup
{
    private static int _selectedSize = -1;
    private static string[] _sizes = ["x0.25", "x0.50", "x1", "x2", "x3", "x4"];

    public static bool Show(string popupId, IFont font, ref Point fontSize)
    {
        bool returnValue = false;

        if (ImGui.BeginPopup(popupId))
        {
            List<Point> _tempSizes = [font.GetFontSize(IFont.Sizes.Quarter), font.GetFontSize(IFont.Sizes.Half), font.GetFontSize(IFont.Sizes.One), font.GetFontSize(IFont.Sizes.Two), font.GetFontSize(IFont.Sizes.Three), font.GetFontSize(IFont.Sizes.Four)];
            _selectedSize = _tempSizes.IndexOf(fontSize);
            if (_selectedSize == -1)
                _selectedSize = 2;

            ImGui.Text("Name: ");
            ImGui.SameLine();
            ImGui.Text(font.Name);
            ImGui.Text("Size: ");
            ImGui.SameLine();
            ImGui.Text(font.GetFontSize(IFont.Sizes.One).ToString());

            ImGui.Separator();
            ImGui.Text("Choose a font size");
            if (ImGui.ListBox("##fontsizes", ref _selectedSize, _sizes, _sizes.Length, _sizes.Length))
            {
                fontSize = font.GetFontSize((IFont.Sizes)_selectedSize);
                ImGui.CloseCurrentPopup();
                returnValue = true;
            }

            ImGui.EndPopup();
        }

        return returnValue;
    }
}
