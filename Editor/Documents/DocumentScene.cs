using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

/// <summary>
/// Represents a child document within a scene, including its position and metadata.
/// </summary>
public class SceneChild : ITitle
{
    /// <summary>
    /// The child document.
    /// </summary>
    public Document Document { get; set; }

    /// <summary>
    /// The position of the child document within the scene.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Whether the child uses pixel positioning instead of cell positioning.
    /// </summary>
    public bool UsePixelPositioning { get; set; }

    /// <summary>
    /// Optional display label for the child in the scene editor.
    /// If empty, the document's Title is used.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets the display title for this child (Label if set, otherwise Document.Title).
    /// </summary>
    public string Title => string.IsNullOrWhiteSpace(Label) ? Document.Title : Label;

    public SceneChild(Document document)
    {
        Document = document;
        Position = Point.Zero;
        UsePixelPositioning = false;
    }

    public override string ToString() => Title;
}

/// <summary>
/// A document that represents a scene containing multiple child documents positioned in space.
/// The scene itself is not directly editable - you select child documents to edit them.
/// </summary>
public partial class DocumentScene : Document, IDocumentSimpleObjects, IDocumentZones
{
    private readonly ScreenSurface _placeholderSurface;
    private SceneChild? _activeChild;

    /// <summary>
    /// The list of child documents in this scene.
    /// </summary>
    public List<SceneChild> Children { get; } = [];

    /// <summary>
    /// The currently selected child for editing. Null when in scene overview mode.
    /// </summary>
    public SceneChild? ActiveChild
    {
        get => _activeChild;
        private set => _activeChild = value;
    }

    /// <summary>
    /// Returns true when a child document is currently being edited.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ActiveChild))]
    public bool IsEditingChild => ActiveChild != null;

    /// <summary>
    /// Index of the selected child in the children list UI.
    /// </summary>
    public int SelectedChildIndex { get; set; } = -1;

    public ImGuiList<ZoneSimplified> Zones => (ActiveChild?.Document as IDocumentZones)?.Zones ?? new ImGuiList<ZoneSimplified>();

    public ImGuiList<SimpleObjectDefinition> SimpleObjects => (ActiveChild?.Document as IDocumentSimpleObjects)?.SimpleObjects ?? new ImGuiList<SimpleObjectDefinition>();

    public DocumentScene()
    {
        // Create a minimal placeholder surface for when no child is being edited
        _placeholderSurface = new ScreenSurface(1, 1);
        _placeholderSurface.Surface.DefaultBackground = Color.DarkSlateGray;
        _placeholderSurface.Surface.Clear();

        EditingSurface = _placeholderSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;

        // Scene draws itself (custom UI)
        Options.DrawSelf = true;
        Options.UseToolsWindow = false;
        Options.DisableScrolling = true;

        // No tool modes for scene itself
        //ToolModes = new([ToolMode.EmptyMode]);
    }

    /// <summary>
    /// Begins editing a child document.
    /// </summary>
    public void BeginEditChild(SceneChild child)
    {
        if (!Children.Contains(child))
            return;

        // Deselect current tool if any
        if (Core.State.Tools.IsItemSelected())
            Core.State.Tools.SelectedItem.OnDeselected(this);

        ActiveChild = child;

        // Delegate to child's surface
        EditingSurface = child.Document.EditingSurface;
        EditingSurfaceFont = child.Document.EditingSurfaceFont;
        EditingSurfaceFontSize = child.Document.EditingSurfaceFontSize;
        EditorFontSize = child.Document.EditorFontSize;
        Resync();

        // Sync visual tool layers
        VisualTool = child.Document.VisualTool;
        VisualLayerToolLower = child.Document.VisualLayerToolLower;
        VisualLayerToolMiddle = child.Document.VisualLayerToolMiddle;
        VisualLayerToolUpper = child.Document.VisualLayerToolUpper;

        // Load child's tools
        Core.State.Tools.Objects.Clear();
        foreach (var tool in child.Document.Tools)
            Core.State.Tools.Objects.Add(tool);

        // Copy child's tool modes
        Options = child.Document.Options;
        ToolModes = child.Document.ToolModes;

        // Trigger redraw
        child.Document.Redraw(true, true);
    }

    /// <summary>
    /// Ends editing the current child and returns to scene overview.
    /// </summary>
    public void EndEditChild()
    {
        if (!IsEditingChild)
            return;

        // Deselect current tool
        if (Core.State.Tools.IsItemSelected())
            Core.State.Tools.SelectedItem.OnDeselected(ActiveChild!.Document);

        ActiveChild = null;

        // Return to placeholder surface
        EditingSurface = _placeholderSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;

        // Clear tools - scene has no editing tools
        Core.State.Tools.Objects.Clear();
        ToolModes = new([ToolMode.EmptyMode]);

        Options = new();
        Options.DrawSelf = true;
        Options.UseToolsWindow = false;
        Options.DisableScrolling = true;
    }

    public override void OnSelected()
    {
        if (IsEditingChild)
        {
            // Delegate to child
            ActiveChild!.Document.OnSelected();
        }
        else
        {
            // Scene overview mode - no tools
            Core.State.Tools.Objects.Clear();
            Core.State.SyncEditorPalette();
        }
    }

    public override void OnDeselected()
    {
        if (IsEditingChild)
        {
            ActiveChild!.Document.OnDeselected();
        }
        else
        {
            Core.State.Tools.Objects.Clear();
        }
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        if (IsEditingChild)
        {
            // Show breadcrumb to return to scene
            if (ImGui.Button($"\uf060 Back to {Title}"))
            {
                EndEditChild();
                return;
            }

            ImGui.Separator();

            // Show child's settings
            ActiveChild!.Document.BuildUiDocumentSettings(renderer);
        }
        else
        {
            BuildSceneSettings(renderer);
        }
    }

    private void BuildSceneSettings(ImGuiRenderer renderer)
    {
        ImGui.SeparatorText("Scene Settings"u8);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Name:"u8);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);

        string editTitle = Title;
        if (ImGui.InputText("##name"u8, ref editTitle, 50))
            Title = editTitle;

        ImGui.Separator();

        // Child documents list
        ImGui.SeparatorText("Child Documents"u8);

        int childCount = Children.Count;

        if (childCount == 0)
        {
            ImGui.TextDisabled("No child documents. Use Document Options menu to import."u8);
        }
        else
        {
            // Child list
            if (ImGui.BeginChild("child_list"u8, new Vector2(-1, ImGui.GetTextLineHeightWithSpacing() * Math.Min(8, childCount + 1)), ImGuiChildFlags.Borders))
            {
                for (int i = 0; i < childCount; i++)
                {
                    ImGui.PushID(i);

                    bool isSelected = SelectedChildIndex == i;
                    if (ImGui.Selectable(Children[i].Title, isSelected))
                        SelectedChildIndex = i;

                    // Double-click to edit
                    if (ImGui.IsItemHovered() && ImGuiP.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        BeginEditChild(Children[i]);
                    }

                    ImGui.PopID();
                }
            }
            ImGui.EndChild();

            // Edit button
            ImGui.BeginDisabled(SelectedChildIndex < 0 || SelectedChildIndex >= childCount);
            if (ImGui.Button("Edit Selected"u8))
            {
                BeginEditChild(Children[SelectedChildIndex]);
            }
            ImGui.EndDisabled();

            ImGui.SameLine();

            // Remove button
            ImGui.BeginDisabled(SelectedChildIndex < 0 || SelectedChildIndex >= childCount);
            if (ImGui.Button("Remove"u8))
            {
                ImGui.OpenPopup("remove_child_popup");
            }
            ImGui.EndDisabled();

            // Remove confirmation popup
            if (ImGui.BeginPopup("remove_child_popup"))
            {
                ImGui.Text("Remove from scene? (Document will be lost)"u8);
                if (ImGui.Button("Cancel"u8))
                    ImGui.CloseCurrentPopup();
                ImGui.SameLine();
                if (ImGui.Button("Remove"u8))
                {
                    Children.RemoveAt(SelectedChildIndex);
                    SelectedChildIndex = Math.Min(SelectedChildIndex, Children.Count - 1);
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            // Selected child properties
            if (SelectedChildIndex >= 0 && SelectedChildIndex < childCount)
            {
                ImGui.Separator();
                ImGui.SeparatorText("Selected Child Properties"u8);

                var child = Children[SelectedChildIndex];

                // Label
                ImGui.AlignTextToFramePadding();
                ImGui.Text("Label:"u8);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1);
                string label = child.Label;
                if (ImGui.InputText("##childlabel"u8, ref label, 50))
                    child.Label = label;

                // Position
                int posX = child.Position.X;
                int posY = child.Position.Y;

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Position X:"u8);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1);
                if (ImGui.InputInt("##posx"u8, ref posX))
                {
                    child.Position = new Point(posX, child.Position.Y);
                    child.Document.EditingSurface.Position = child.Position;
                }

                ImGui.AlignTextToFramePadding();
                ImGui.Text("Position Y:"u8);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(-1);
                if (ImGui.InputInt("##posy"u8, ref posY))
                {
                    child.Position = new Point(child.Position.X, posY);
                    child.Document.EditingSurface.Position = child.Position;
                }

                // Pixel positioning
                bool usePixel = child.UsePixelPositioning;
                if (ImGui.Checkbox("Use Pixel Positioning"u8, ref usePixel))
                {
                    child.UsePixelPositioning = usePixel;
                    child.Document.EditingSurface.UsePixelPositioning = usePixel;
                }

                // Document type info
                ImGui.TextDisabled($"Type: {child.Document.GetType().Name}");
            }
        }
    }

    public override void ImGuiDraw(ImGuiRenderer renderer)
    {
        if (IsEditingChild)
        {
            // Let the normal document host handle rendering via the delegated EditingSurface
            // This is handled by GuiDocumentsHost since Options.DrawSelf delegates here,
            // but we're in editing mode so we need to NOT draw self
        }
        else
        {
            // Scene overview mode
            DrawSceneOverview(renderer);
        }
    }

    private void DrawSceneOverview(ImGuiRenderer renderer)
    {
        ImGui.Text($"Scene: {Title}");
        ImGui.Separator();

        if (Children.Count == 0)
        {
            ImGui.TextDisabled("This scene has no child documents."u8);
            ImGui.TextDisabled("Use 'Document Options > Import document from list' to add documents."u8);
        }
        else
        {
            ImGui.Text($"Contains {Children.Count} child document(s).");
            ImGui.TextDisabled("Select a child in the settings panel and click 'Edit Selected' to edit it."u8);
            ImGui.TextDisabled("Or double-click a child in the list."u8);

            // TODO: Future iteration - render composite preview of all children
        }
    }

    public override void ImGuiDrawTopBar(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMenu("Document Options"u8))
        {
            // Import from main document list
            ImGui.BeginDisabled(Core.State.Documents.Count <= 1); // Need at least one other document besides this scene
            if (ImGui.BeginMenu("Import document from list"u8))
            {
                for (int i = 0; i < Core.State.Documents.Count; i++)
                {
                    var doc = Core.State.Documents.Objects[i];
                    
                    // Can't import self or other scenes
                    if (doc == this || doc is DocumentScene)
                        continue;

                    if (ImGui.MenuItem(doc.Title))
                    {
                        // Remove from main list
                        Core.State.Documents.Objects.Remove(doc);
                        
                        // Add to scene
                        var child = new SceneChild(doc);
                        Children.Add(child);
                        SelectedChildIndex = Children.Count - 1;

                        // If this was the selected document, update selection
                        if (Core.State.Documents.SelectedItemIndex >= Core.State.Documents.Count)
                            Core.State.Documents.SelectedItemIndex = Core.State.Documents.Count - 1;
                    }
                }
                ImGui.EndMenu();
            }
            ImGui.EndDisabled();

            // Transfer back to main list
            ImGui.BeginDisabled(Children.Count == 0);
            if (ImGui.BeginMenu("Transfer document to list"u8))
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (ImGui.MenuItem(Children[i].Title))
                    {
                        var child = Children[i];
                        
                        // Remove from scene
                        Children.RemoveAt(i);
                        
                        // Add to main list
                        Core.State.Documents.Objects.Add(child.Document);
                        
                        // Update selection
                        if (SelectedChildIndex >= Children.Count)
                            SelectedChildIndex = Children.Count - 1;
                    }
                }
                ImGui.EndMenu();
            }
            ImGui.EndDisabled();

            ImGui.EndMenu();
        }

        // If editing a child, also show child's menu items
        if (IsEditingChild)
        {
            bool oldZones = ActiveChild.Document.Options.UseZones;
            bool oldObjects = ActiveChild.Document.Options.UseSimpleObjects;

            ActiveChild.Document.ImGuiDrawTopBar(renderer);

            if (oldZones != ActiveChild.Document.Options.UseZones || oldObjects != ActiveChild.Document.Options.UseSimpleObjects)
            {
                ToolModes = ActiveChild.Document.ToolModes;
            }
        }
    }

    public override void Redraw(bool redrawSurface, bool redrawTooling)
    {
        if (IsEditingChild)
        {
            // Delegate to child
            ActiveChild!.Document.Redraw(redrawSurface, redrawTooling);
            
            // Copy texture info from child
            VisualTextureId = ActiveChild.Document.VisualTextureId;
            VisualTextureSize = ActiveChild.Document.VisualTextureSize;
        }
        else
        {
            // Scene overview doesn't need complex rendering
            // Just ensure placeholder is valid
            _placeholderSurface.Render(Game.Instance.UpdateFrameDelta);
        }
    }

    public override void SetSurfaceView(int x, int y, int width, int height)
    {
        if (IsEditingChild)
        {
            ActiveChild!.Document.SetSurfaceView(x, y, width, height);
        }
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SceneDocument()];

    public override string ToString() =>
        Title;
}
