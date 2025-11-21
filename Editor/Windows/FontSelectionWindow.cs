using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;
using static System.Net.Mime.MediaTypeNames;

namespace SadConsole.Editor.Windows;


public class FontSelectionWindow: ImGuiWindowBase
{
    private ImGuiList<string> _fontSizes;

    private static string[] _sizes = ["x0.25", "x0.50", "x1", "x2", "x3", "x4"];

    public const string POPUP_ID = nameof(FontSelectionWindow);

    public IFont SelectedFont;
    public Point SelectedFontSize;
    private ImGuiList<FontSizeItem> _fontSizeItems = new();

    private class FontSizeItem: ITitle
    {
        public string Title { get; }
        public IFont.Sizes Size;

        public FontSizeItem(string name, IFont.Sizes size)
        {
            Title = name;
            Size = size;
        }
    }

    public FontSelectionWindow(IFont currentFont, Point fontSize)
    {
        Title = "Select Font Size";
        SelectedFont = currentFont;
        Core.State.SadConsoleFonts.SelectedItem = currentFont;
        SelectedFontSize = fontSize;

        _fontSizeItems = new(
            new FontSizeItem("x0.25", IFont.Sizes.Quarter),
            new FontSizeItem("x0.50", IFont.Sizes.Half),
            new FontSizeItem("x1", IFont.Sizes.One),
            new FontSizeItem("x2", IFont.Sizes.Two),
            new FontSizeItem("x3", IFont.Sizes.Three),
            new FontSizeItem("x4", IFont.Sizes.Four));

        _fontSizeItems.SelectedItem = _fontSizeItems.Objects.Where(item => fontSize == currentFont.GetFontSize(item.Size)).FirstOrDefault() ?? _fontSizeItems.Objects[2];
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            //ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Text("Fonts:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##fonttypes", ref Core.State.SadConsoleFonts.SelectedItemIndex, Core.State.SadConsoleFonts.Names, Core.State.SadConsoleFonts.Count, 5))
                    SelectedFont = Core.State.SadConsoleFonts.SelectedItem;

                ImGui.Separator();

                ImGui.Text("Name: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.Name);
                ImGui.Text("Size: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.GetFontSize(IFont.Sizes.One).ToString());

                ImGui.Separator();
                ImGui.Text("Choose a font size");
                ImGui.Combo("##fontsizes", ref _fontSizeItems.SelectedItemIndex, _fontSizeItems.Names, _fontSizeItems.Count, _fontSizeItems.Count);
                ImGui.Separator();

                if (DrawButtons(out DialogResult, acceptButtonText: "Select"))
                {
                    SelectedFontSize = SelectedFont.GetFontSize(_fontSizeItems.SelectedItem.Size);
                    Close();
                }

                ImGui.EndPopup();
            }
        }
    }
}
