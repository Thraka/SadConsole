using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;


public static class FontSelectionWindow
{
    public static void Show(ImGuiRenderer renderer, IFont currentFont, Point fontSize, Action<IFont, Point> onFontSelected, Action? onCancelled = null)
    {
        Instance instance = new(currentFont, fontSize, onFontSelected, onCancelled);
        renderer.UIObjects.Add(instance);
    }

    protected class Instance : ImGuiObjectBase
    {
        private IFont _selectedFont;
        private ImGuiList<FontSizeItem> _fontSizeItems = new();

        private Action<IFont, Point> _onFontSelected;
        private Action? _onCancelled;
        private bool _firstShow = true;

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

        public Instance(IFont currentFont, Point fontSize, Action<IFont, Point> onFontSelected, Action? onCancelled)
        {
            _selectedFont = currentFont;
            Core.State.SadConsoleFonts.SelectedItem = currentFont;
            _onFontSelected = onFontSelected;
            _onCancelled = onCancelled;

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
            if (_firstShow)
            {
                ImGui.OpenPopup("Select Font Size"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindowOnAppearing(new System.Numerics.Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));

            if (ImGui.BeginPopupModal("Select Font Size"u8))
            {
                ImGui.Text("Fonts:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##fonttypes", ref Core.State.SadConsoleFonts.SelectedItemIndex, Core.State.SadConsoleFonts.Names, Core.State.SadConsoleFonts.Count, 5))
                    _selectedFont = Core.State.SadConsoleFonts.SelectedItem;

                ImGui.Separator();

                ImGui.Text("Name: ");
                ImGui.SameLine();
                ImGui.Text(_selectedFont.Name);
                ImGui.Text("Size: ");
                ImGui.SameLine();
                ImGui.Text(_selectedFont.GetFontSize(IFont.Sizes.One).ToString());

                ImGui.Separator();
                ImGui.Text("Choose a font size");
                ImGui.Combo("##fontsizes", ref _fontSizeItems.SelectedItemIndex, _fontSizeItems.Names, _fontSizeItems.Count, _fontSizeItems.Count);
                ImGui.Separator();

                bool dialogResult;
                if (ImGuiSC.WindowDrawButtons(out dialogResult, cancelButtonText: "Cancel", acceptButtonText: "Select"))
                {
                    if (dialogResult)
                        _onFontSelected(_selectedFont, _selectedFont.GetFontSize(_fontSizeItems.SelectedItem.Size));
                    else
                        _onCancelled?.Invoke();

                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                }

                ImGui.EndPopup();
            }
        }
    }
}
