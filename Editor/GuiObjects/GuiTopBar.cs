using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Addins;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.GuiObjects;

public class GuiTopBar : ImGuiObjectBase
{
    public bool ShowDemoWindow;
    public bool ShowMetrics;
    private bool _debug;
    private bool _closeDocumentConfirm;
    private bool _exitConfirm;

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
                    Windows.NewDocumentWindow.Show(renderer);

                if (ImGui.MenuItem("\ue5fe Open"u8, "o"u8))
                {
                    Windows.OpenFileWindow.Show(renderer, (loader, file) =>
                    {
                        if (loader.Load(file.FullName) is Document document)
                        {
                            Core.State.Documents.Add(document);
                        }
                    }, null);
                }

                ImGui.Separator();
                ImGui.BeginDisabled(!Core.State.HasSelectedDocument);
                if (ImGui.MenuItem("\ueb4b Save"u8, "s"u8))
                    Windows.SaveFileWindow.Show(renderer, Core.State.SelectedDocument!);

                ImGui.BeginDisabled(Core.State.HasSelectedDocument && Core.State.SelectedDocument.Parent != null);
                if (ImGui.MenuItem("Close", "c"))
                    _closeDocumentConfirm = true;

                ImGui.EndDisabled();

                ImGui.EndDisabled();

                if (ImGui.MenuItem("\U000f044e Import Image"u8, "i"u8))
                {
                    Windows.ImageToAsciiWindow.Show(renderer,
                        Game.Instance.DefaultFont,
                        Game.Instance.DefaultFont.GetFontSize(Game.Instance.DefaultFontSize),
                        (resultSurface) =>
                        {
                            DocumentSurface.Builder builder = new();
                            builder.Width = resultSurface.Width;
                            builder.Height = resultSurface.Height;

                            DocumentSurface document = (DocumentSurface)builder.CreateDocument();
                            resultSurface.Copy(document.EditingSurface.Surface);
                            document.EditingSurface.IsDirty = true;

                            Core.State.Documents.Add(document);
                        }, null);
                }

                ImGui.Separator();

                if (ImGui.Checkbox("Fullscreen"u8, ref Core.Settings.UseFullscreen))
                {
                    if (Core.Settings.UseFullscreen)
                        Game.Instance.ToggleFullScreen();
                    else
                        Game.Instance.ToggleFullScreen();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Exit", "x"))
                    _exitConfirm = true;

                ImGui.EndMenu();
            }

            if (_closeDocumentConfirm)
            {
                _closeDocumentConfirm = false;
                ImGui.OpenPopup("ConfirmCloseDocument"u8);
            }

            if (_exitConfirm)
            {
                _exitConfirm = false;
                ImGui.OpenPopup("ExitApp"u8);
            }

            ImGuiSC.CenterNextWindow();
            ImGuiSC.ConfirmPopup("ConfirmCloseDocument"u8, "Are you sure you want to close this document?"u8, () =>
            {
                var docToClose = Core.State.SelectedDocument!.Parent != null
                    ? HierarchyHelper.GetRoot(Core.State.SelectedDocument)
                    : Core.State.SelectedDocument;

                Core.State.Documents.Remove(docToClose);
                Core.State.SelectedDocument = null;

                if (Core.State.Documents.Count > 0)
                {
                    Core.State.SelectedDocument = Core.State.Documents[0];
                    Core.State.SelectedDocument.OnSelected();
                }
            });

            ImGuiSC.CenterNextWindow();
            ImGuiSC.ConfirmPopup("ExitApp"u8, "Are you sure you want to quit?\n\n(make sure you saved your documents!)"u8, () =>
            {
                Game.Instance.MonoGameInstance.Exit();
            });

            // Draw the documents menu items
            if (Core.State.HasSelectedDocument)
                Core.State.SelectedDocument!.ImGuiDrawTopBar(renderer);

            // Draw the palette menu
            if (ImGui.BeginMenu("Palette"u8))
            {
                if (ImGui.MenuItem("Edit Editor Palette"u8))
                {
                    Windows.PaletteEditorWindow.Show(renderer, Core.State.Palette);
                }

                if (Core.State.SelectedDocument is not null)
                {
                    if (Core.State.SelectedDocument.HasPalette)
                    {
                        if (ImGui.MenuItem("Edit Document Palette"u8))
                            Windows.PaletteEditorWindow.Show(renderer, Core.State.SelectedDocument.Palette!);
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

            // Render addin menu contributions
            var addinMenuGroups = Core.State.AddinMenuItems.GroupBy(m => m.Menu);
            foreach (var group in addinMenuGroups)
            {
                if (ImGui.BeginMenu(group.Key))
                {
                    foreach (var item in group)
                    {
                        if (ImGui.MenuItem(item.Label))
                            item.OnClick();
                    }
                    ImGui.EndMenu();
                }
            }

            //if (ImGui.BeginMenu("Debug"u8))
            //{
            //    if (ImGui.MenuItem("Pause"u8, "p"u8))
            //        _debug = true;

            //    ImGui.Separator();
            //    ImGui.MenuItem("Show Demo"u8, "s"u8, ref ShowDemoWindow);
            //    ImGui.MenuItem("Show Metrics"u8, "d"u8, ref ShowMetrics);

            //    ImGui.EndMenu();
            //}

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
}

