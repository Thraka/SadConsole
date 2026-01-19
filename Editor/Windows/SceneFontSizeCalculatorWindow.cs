using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;
using static System.Net.Mime.MediaTypeNames;

namespace SadConsole.Editor.Windows;

public class SceneFontSizeCalculatorWindow : ImGuiWindowBase
{
    private ImGuiList<FontSizeItem> _fontSizeItems = new();

    private int _columns = 80;
    private int _rows = 25;
    public Point CalculatedPixelSize { get; private set; }

    public IFont SelectedFont;
    public Point SelectedFontSize;

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

    public SceneFontSizeCalculatorWindow()
    {
        Title = "Calculate Scene Size";
        SelectedFont = Core.State.SadConsoleFonts.SelectedItem;
        if (SelectedFont == null && Core.State.SadConsoleFonts.Count > 0)
            SelectedFont = Core.State.SadConsoleFonts.Objects[0];

        SelectedFontSize = SelectedFont.GetFontSize(IFont.Sizes.One);

        _fontSizeItems = new(
            new FontSizeItem("x0.25", IFont.Sizes.Quarter),
            new FontSizeItem("x0.50", IFont.Sizes.Half),
            new FontSizeItem("x1", IFont.Sizes.One),
            new FontSizeItem("x2", IFont.Sizes.Two),
            new FontSizeItem("x3", IFont.Sizes.Three),
            new FontSizeItem("x4", IFont.Sizes.Four));

        _fontSizeItems.SelectedItem = _fontSizeItems.Objects.FirstOrDefault(item => SelectedFontSize == SelectedFont.GetFontSize(item.Size)) ?? _fontSizeItems.Objects[2];
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Text("Fonts:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##fonttypes", ref Core.State.SadConsoleFonts.SelectedItemIndex, Core.State.SadConsoleFonts.Names, Core.State.SadConsoleFonts.Count, 5))
                {
                    SelectedFont = Core.State.SadConsoleFonts.SelectedItem;
                    SelectedFontSize = SelectedFont.GetFontSize(_fontSizeItems.SelectedItem.Size);
                }

                ImGui.Separator();

                ImGui.Text("Name: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.Name);
                ImGui.Text("Size: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.GetFontSize(IFont.Sizes.One).ToString());

                ImGui.Separator();
                ImGui.Text("Choose a font size");
                if (ImGui.Combo("##fontsizes", ref _fontSizeItems.SelectedItemIndex, _fontSizeItems.Names, _fontSizeItems.Count, _fontSizeItems.Count))
                    SelectedFontSize = SelectedFont.GetFontSize(_fontSizeItems.SelectedItem.Size);

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

                CalculatedPixelSize = new Point(_columns * SelectedFontSize.X, _rows * SelectedFontSize.Y);
                ImGui.Text($"Calculated Size: {CalculatedPixelSize.X} x {CalculatedPixelSize.Y}");

                ImGui.Separator();

                if (DrawButtons(out DialogResult, acceptButtonText: "Select"))
                {
                    Close();
                }

                ImGui.EndPopup();
            }
        }
    }
}
