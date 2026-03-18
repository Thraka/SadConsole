using System.Diagnostics.CodeAnalysis;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class SceneFontSizeCalculatorPopup
{
    private static ImGuiList<FontSizeItem> _fontSizeItems = new();

    private static int _columns = 80;
    private static int _rows = 25;

    private static IFont _tempFont;
    private static Point _tempFontSize;

    private class FontSizeItem : ITitle
    {
        public string Title { get; }
        public IFont.Sizes Size;

        public FontSizeItem(string name, IFont.Sizes size)
        {
            Title = name;
            Size = size;
        }
    }

    public static void Open()
    {
        _tempFont = Core.State.SadConsoleFonts.SelectedItem;
        if (_tempFont == null && Core.State.SadConsoleFonts.Count > 0)
            _tempFont = Core.State.SadConsoleFonts.Objects[0];

        _tempFontSize = _tempFont.GetFontSize(IFont.Sizes.One);

        _fontSizeItems = new(
            new FontSizeItem("x0.25", IFont.Sizes.Quarter),
            new FontSizeItem("x0.50", IFont.Sizes.Half),
            new FontSizeItem("x1", IFont.Sizes.One),
            new FontSizeItem("x2", IFont.Sizes.Two),
            new FontSizeItem("x3", IFont.Sizes.Three),
            new FontSizeItem("x4", IFont.Sizes.Four));

        _fontSizeItems.SelectedItem = _fontSizeItems.Objects.FirstOrDefault(item => _tempFontSize == _tempFont.GetFontSize(item.Size)) ?? _fontSizeItems.Objects[2];

        ImGui.OpenPopup("Calculate Scene Size"u8);
    }

    public static bool Draw(ImGuiRenderer renderer, out Point pixelSize)
    {
        pixelSize = Point.Zero;

        ImGuiSC.CenterNextWindow();
        if (ImGui.BeginPopupModal("Calculate Scene Size"u8, ImGuiWindowFlags.NoResize))
        {
            ImGui.Text("Fonts:");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.ListBox("##fonttypes", ref Core.State.SadConsoleFonts.SelectedItemIndex, Core.State.SadConsoleFonts.Names, Core.State.SadConsoleFonts.Count, 5))
            {
                _tempFont = Core.State.SadConsoleFonts.SelectedItem;
                _tempFontSize = _tempFont.GetFontSize(_fontSizeItems.SelectedItem.Size);
            }

            ImGui.Separator();

            ImGui.Text("Name: ");
            ImGui.SameLine();
            ImGui.Text(_tempFont.Name);
            ImGui.Text("Size: ");
            ImGui.SameLine();
            ImGui.Text(_tempFont.GetFontSize(IFont.Sizes.One).ToString());

            ImGui.Separator();
            ImGui.Text("Choose a font size");
            if (ImGui.Combo("##fontsizes", ref _fontSizeItems.SelectedItemIndex, _fontSizeItems.Names, _fontSizeItems.Count, _fontSizeItems.Count))
                pixelSize = _tempFont.GetFontSize(_fontSizeItems.SelectedItem.Size);

            ImGui.Separator();

            ImGui.AlignTextToFramePadding();
            ImGui.Text("Columns:"u8);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(-1);
            ImGui.InputInt("##columns", ref _columns);

            ImGui.AlignTextToFramePadding();
            ImGui.Text("Rows:"u8);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(-1);
            ImGui.InputInt("##rows", ref _rows);

            ImGui.Separator();

            pixelSize = new Point(_columns * _tempFontSize.X, _rows * _tempFontSize.Y);
            ImGui.Text($"Calculated Size: {pixelSize.X} x {pixelSize.Y}");

            ImGui.Separator();

            if (ImGuiSC.WindowDrawButtons(out bool dialogResult, acceptButtonText: "Select"))
            {
                ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
                return dialogResult;
            }

            ImGui.EndPopup();
        }

        return false;
    }
}
