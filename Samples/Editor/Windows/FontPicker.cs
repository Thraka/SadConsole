using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class FontPicker : ImGuiWindow
{
    private int _fontSelectedIndex = -1;
    private string _fontName = "";
    private string[] _fontNames;
    private IFont[] _fonts;
    private IFont? _listboxFont;

    private int _selectedSize = -1;
    private string[] _sizes;

    public IFont Font { get; set; }
    public Point FontSize { get; set; }

    public FontPicker(IFont font, Point fontSize)
    {
        Title = "Font picker";
        Font = font;
        FontSize = fontSize;

        List<IFont> fonts = [Game.Instance.EmbeddedFont, Game.Instance.EmbeddedFontExtended];

        foreach (IFont fontItem in Game.Instance.Fonts.Values)
            if (!fonts.Contains(fontItem))
                fonts.Add(fontItem);

        _fontNames = Game.Instance.Fonts.Select(f => f.Key).ToArray();
        _fontNames[0] = "SadConsole Built in IBM 8x16";
        _fontNames[1] = "SadConsole Built in IBM 8x16 extended";

        _fontSelectedIndex = fonts.IndexOf(font);
        _listboxFont = font;

        _sizes = ["x0.25", "x0.50", "x1", "x2", "x3", "x4"];
        List<Point> _tempSizes = [Font.GetFontSize(IFont.Sizes.Quarter), Font.GetFontSize(IFont.Sizes.Half), Font.GetFontSize(IFont.Sizes.One), Font.GetFontSize(IFont.Sizes.Two), Font.GetFontSize(IFont.Sizes.Three), Font.GetFontSize(IFont.Sizes.Four)];
        _selectedSize = _tempSizes.IndexOf(fontSize);
        if (_selectedSize == -1)
            _selectedSize = 2;
    }

    public void Show()
    {
        IsOpen = true;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiExt.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Text("Fonts");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##Document Type", ref _fontSelectedIndex, _fontNames, _fontNames.Length, 4))
                    _listboxFont = Game.Instance.Fonts.ToArray()[_fontSelectedIndex].Value;

                ImGui.Text("Name: ");
                ImGui.SameLine();
                ImGui.Text(_listboxFont!.Name);
                ImGui.Text("Size: ");
                ImGui.SameLine();
                ImGui.Text(_listboxFont.GetFontSize(IFont.Sizes.One).ToString());

                ImGui.Separator();
                ImGui.Text("Choose a font size");
                if (ImGui.ListBox("##fontsizes", ref _selectedSize, _sizes, _sizes.Length, _sizes.Length))
                {

                }
                ImGui.SameLine();
                ImGui.BeginGroup();
                ImGui.Text("Size:");
                ImGui.SameLine();
                ImGui.Text(_listboxFont.GetFontSize((IFont.Sizes)_selectedSize).ToString());
                ImGui.EndGroup();
                //
                // Bottom buttons
                //
                ImGui.Separator();

                if (ImGui.Button("Cancel")) { DialogResult = false; IsOpen = false; }

                // Right-align button
                float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
                ImGui.SameLine(ImGui.GetWindowWidth() - pos);

                if (ImGui.Button("Accept"))
                {
                    Font = _listboxFont;
                    FontSize = _listboxFont.GetFontSize((IFont.Sizes)_selectedSize);
                    DialogResult = true;
                    IsOpen = false;
                }

                ImGui.EndPopup();
            }
        }
        else
        {
            OnClosed();
            ImGuiCore.GuiComponents.Remove(this);
        }
    }
}
