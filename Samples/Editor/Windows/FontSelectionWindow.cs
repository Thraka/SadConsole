using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;
using static System.Net.Mime.MediaTypeNames;

namespace SadConsole.Editor.Windows;


public class FontSelectionWindow: ImGuiWindowBase
{
    private static int _selectedSize = -1;
    private static string[] _sizes = ["x0.25", "x0.50", "x1", "x2", "x3", "x4"];

    public const string POPUP_ID = nameof(FontSelectionWindow);

    public IFont SelectedFont;
    public Point SelectedFontSize;

    public FontSelectionWindow(IFont currentFont, Point fontSize)
    {
        Title = "Select Font Size";
        SelectedFont = currentFont;
        Core.State.SadConsoleFonts.SelectedItem = currentFont;
        SelectedFontSize = fontSize;
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

                List<Point> _tempSizes = [SelectedFont.GetFontSize(IFont.Sizes.Quarter), SelectedFont.GetFontSize(IFont.Sizes.Half), SelectedFont.GetFontSize(IFont.Sizes.One), SelectedFont.GetFontSize(IFont.Sizes.Two), SelectedFont.GetFontSize(IFont.Sizes.Three), SelectedFont.GetFontSize(IFont.Sizes.Four)];
                _selectedSize = _tempSizes.IndexOf(SelectedFontSize);
                if (_selectedSize == -1)
                    _selectedSize = 2;

                ImGui.Text("Name: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.Name);
                ImGui.Text("Size: ");
                ImGui.SameLine();
                ImGui.Text(SelectedFont.GetFontSize(IFont.Sizes.One).ToString());

                ImGui.Separator();
                ImGui.Text("Choose a font size");
                ImGui.Combo("##fontsizes", ref _selectedSize, _sizes, _sizes.Length, _sizes.Length);
                ImGui.Separator();

                if (DrawButtons(out DialogResult, acceptButtonText: "Select"))
                {
                    SelectedFontSize = SelectedFont.GetFontSize((IFont.Sizes)_selectedSize);
                    Close();
                }

                ImGui.EndPopup();
            }
        }
    }
}
