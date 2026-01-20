using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;

public class GuiTopBar : ImGuiObjectBase
{
    public bool ShowDemoWindow;
    public bool ShowMetrics;
    private bool _debug;

    // GuideGrid temp values for input
    private int _guideGridCellsX = Core.State.GuideGrid.CellsX;
    private int _guideGridCellsY = Core.State.GuideGrid.CellsY;

    public List<(Vector4 Color, string Text)> StatusItems = new();

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_debug)
        {
            _debug = false;
            System.Diagnostics.Debugger.Break();
        }
        if (ShowMetrics)
            ImGui.ShowMetricsWindow();
        if (ShowDemoWindow)
            ImGui.ShowDemoWindow();

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"u8))
            {
                if (ImGui.MenuItem("\uea7f New"u8, "n"u8))
                    new Windows.NewDocument().Show();

                if (ImGui.MenuItem("\ue5fe Open"u8, "o"u8))
                {
                    Windows.OpenFile window = new();
                    window.Closed += (s, e) =>
                    {
                        if (window.DialogResult)
                        {
                            if (window.SelectedLoader.Load(window.SelectedFile.FullName) is Document document)
                            {
                                Core.State.Documents.Add(document);
                                //Core.State.Documents.SelectedItemIndex = Core.State.Documents.Count - 1;
                            }
                        }
                    };
                    window.Open();
                }

                if (ImGui.MenuItem("\U000f044e Import Image"u8, "i"u8))
                {
                    Windows.ImageToAsciiWindow imageToAscii = new(Game.Instance.DefaultFont, Game.Instance.DefaultFont.GetFontSize(Game.Instance.DefaultFontSize));
                    imageToAscii.Closed += ImportImage_Closed;
                    imageToAscii.Open();
                }

                ImGui.Separator();
                ImGui.BeginDisabled(!Core.State.HasSelectedDocument);
                if (ImGui.MenuItem("\ueb4b Save"u8, "s"u8))
                {
                    Windows.SaveFile window = new(Core.State.SelectedDocument!);
                    window.Open();
                }

                ImGui.BeginDisabled(Core.State.HasSelectedDocument && Core.State.SelectedDocument.Parent != null);
                if (ImGui.MenuItem("Close", "c"))
                {
                    PromptWindow window = new("Are you sure you want to close this document?", "Close", "Yes", "No");
                    window.Closed += (s, e) =>
                    {
                        if (((PromptWindow)s).DialogResult)
                        {
                            // Get the root document for closing
                            var docToClose = Core.State.SelectedDocument!.Parent != null 
                                ? HierarchyHelper.GetRoot(Core.State.SelectedDocument) 
                                : Core.State.SelectedDocument;
                            
                            Core.State.Documents.Remove(docToClose);
                            Core.State.SelectedDocument = null;
                            
                            // Select first available document if any
                            if (Core.State.Documents.Count > 0)
                            {
                                Core.State.SelectedDocument = Core.State.Documents[0];
                                Core.State.SelectedDocument.OnSelected();
                            }
                        }
                    };
                    window.Open();
                }
                ImGui.EndDisabled();

                ImGui.EndDisabled();

                ImGui.EndMenu();
            }

            // Draw the documents menu items
            if (Core.State.HasSelectedDocument)
                Core.State.SelectedDocument!.ImGuiDrawTopBar(renderer);

            // Draw the palette menu
            if (ImGui.BeginMenu("Palette"u8))
            {
                if (ImGui.MenuItem("Edit Editor Palette"u8))
                {
                    new Windows.PaletteEditorWindow(Core.State.Palette).Open();
                }
                
                if (Core.State.SelectedDocument is not null)
                {
                    if (Core.State.SelectedDocument.HasPalette)
                    {
                        if (ImGui.MenuItem("Edit Document Palette"u8))
                            new Windows.PaletteEditorWindow(Core.State.SelectedDocument.Palette!).Open();
                    }
                    else
                    {
                        ImGui.Separator();

                        if (ImGui.MenuItem("Add to Document"u8, "a"u8))
                        {
                            Core.State.SelectedDocument.Palette = new EditorPalette();
                            Core.State.SelectedDocument.HasPalette = true;
                        }
                    }
                    
                }
                ImGui.EndMenu();
            }

            // Draw the guide grid menu
            if (ImGui.BeginMenu("Grid Guide"u8))
            {
                bool enabled = Core.State.GuideGrid.Enabled;
                if (ImGui.MenuItem("Enabled"u8, "", ref enabled))
                    Core.State.GuideGrid.Enabled = enabled;

                ImGui.Separator();

                bool isLight = Core.State.GuideGrid.Mode == Core.State.GuideGrid.LineMode.Light;
                bool isDark = Core.State.GuideGrid.Mode == Core.State.GuideGrid.LineMode.Dark;

                if (ImGui.MenuItem("Light Lines"u8, ""u8, isLight))
                    Core.State.GuideGrid.Mode = Core.State.GuideGrid.LineMode.Light;

                if (ImGui.MenuItem("Dark Lines"u8, ""u8, isDark))
                    Core.State.GuideGrid.Mode = Core.State.GuideGrid.LineMode.Dark;

                ImGui.Separator();

                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt("Cells X"u8, ref _guideGridCellsX))
                {
                    if (_guideGridCellsX < 1) _guideGridCellsX = 1;
                    Core.State.GuideGrid.CellsX = _guideGridCellsX;
                }

                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt("Cells Y"u8, ref _guideGridCellsY))
                {
                    if (_guideGridCellsY < 1) _guideGridCellsY = 1;
                    Core.State.GuideGrid.CellsY = _guideGridCellsY;
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Debug"u8))
            {
                if (ImGui.MenuItem("Pause"u8, "p"u8))
                    _debug = true;

                ImGui.Separator();
                ImGui.MenuItem("Show Demo"u8, "s"u8, ref ShowDemoWindow);
                ImGui.MenuItem("Show Metrics"u8, "d"u8, ref ShowMetrics);

                ImGui.EndMenu();
            }

            // Write status items at the top
            foreach ((Vector4 Color, string Text) item in StatusItems)
                if (item.Color == Vector4.Zero)
                    ImGui.Text(item.Text);
                else
                    ImGui.TextColored(item.Color, item.Text);
            StatusItems.Clear();
            ImGui.EndMainMenuBar();
        }
    }

    private void ImportImage_Closed(object? sender, EventArgs e)
    {
        Windows.ImageToAsciiWindow window = (Windows.ImageToAsciiWindow)sender!;
        if (!window.DialogResult) return;

        DocumentSurface.Builder builder = new();
        builder.Width = window.ResultSurface!.Width;
        builder.Height = window.ResultSurface.Height;

        DocumentSurface document = (DocumentSurface)builder.CreateDocument();
        window.ResultSurface.Copy(document.EditingSurface.Surface);
        document.EditingSurface.IsDirty = true;

        Core.State.Documents.Add(document);
    }
}

