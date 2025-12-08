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
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("\uea7f New", "n"))
                    new Windows.NewDocument().Show();

                if (ImGui.MenuItem("\ue5fe Open", "o"))
                {
                    Windows.OpenFile window = new();
                    window.Closed += (s, e) =>
                    {
                        if (window.DialogResult)
                        {
                            if (window.SelectedLoader.Load(window.SelectedFile.FullName) is Document document)
                            {
                                Core.State.Documents.Objects.Add(document);
                                //Core.State.Documents.SelectedItemIndex = Core.State.Documents.Objects.Count - 1;
                            }
                        }
                    };
                    window.Open();
                }

                ImGui.Separator();
                ImGui.BeginDisabled(!Core.State.HasSelectedDocument);
                if (ImGui.MenuItem("\ueb4b Save", "s"))
                {
                    // Get the root document for saving (if child is selected, save the parent scene)
                    var docToSave = Core.State.SelectedDocument!.Parent != null 
                        ? HierarchyHelper.GetRoot(Core.State.SelectedDocument) 
                        : Core.State.SelectedDocument;
                    Windows.SaveFile window = new(docToSave);
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
                            
                            Core.State.Documents.Objects.Remove(docToClose);
                            Core.State.SelectedDocument = null;
                            
                            // Select first available document if any
                            if (Core.State.Documents.Count > 0)
                            {
                                Core.State.SelectedDocument = Core.State.Documents.Objects[0];
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
            if (ImGui.BeginMenu("Palette"))
            {
                if (ImGui.MenuItem("Edit Editor Palette"))
                {
                    new Windows.PaletteEditorWindow(Core.State.Palette).Open();
                }
                
                if (Core.State.SelectedDocument is not null)
                {
                    if (Core.State.SelectedDocument.HasPalette)
                    {
                        if (ImGui.MenuItem("Edit Document Palette"))
                            new Windows.PaletteEditorWindow(Core.State.SelectedDocument.Palette!).Open();
                    }
                    else
                    {
                        ImGui.Separator();

                        if (ImGui.MenuItem("Add to Document", "a"))
                        {
                            Core.State.SelectedDocument.Palette = new EditorPalette();
                            Core.State.SelectedDocument.HasPalette = true;
                        }
                    }
                    
                }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Debug"))
            {
                if (ImGui.MenuItem("Pause", "p"))
                    _debug = true;

                ImGui.Separator();
                ImGui.MenuItem("Show Demo", "s", ref ShowDemoWindow);
                ImGui.MenuItem("Show Metrics", "d", ref ShowMetrics);

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
}

