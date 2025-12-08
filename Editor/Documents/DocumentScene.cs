using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
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
/// Child documents are selected directly in the document list hierarchy.
/// </summary>
public partial class DocumentScene : Document, IDocumentSimpleObjects, IDocumentZones
{
    private readonly ScreenSurface _placeholderSurface;

    /// <summary>
    /// The list of scene child items with their metadata (position, label, etc.).
    /// </summary>
    public List<SceneChild> ChildSceneItems { get; } = [];

    /// <summary>
    /// Gets the icon for scene documents.
    /// </summary>
    public override string DocumentIcon => "\uf02d"; // object-group / scene icon

    #region IHierarchicalItem Overrides

    /// <summary>
    /// Gets the child documents of this scene.
    /// </summary>
    public override IReadOnlyList<Document> Children => ChildSceneItems.Select(c => c.Document).ToList();

    /// <summary>
    /// Gets a value indicating that scenes can contain children.
    /// </summary>
    public override bool CanHaveChildren => true;

    /// <summary>
    /// Gets a value indicating that scene children should be displayed in the hierarchy.
    /// </summary>
    public override bool ShowChildrenInHierarchy => true;

    /// <summary>
    /// Adds a child document to this scene with default metadata.
    /// </summary>
    /// <param name="document">The document to add as a child.</param>
    /// <returns>The created SceneChild wrapper.</returns>
    public SceneChild AddChildDocument(Document document)
    {
        var child = new SceneChild(document);
        document.Parent = this;
        ChildSceneItems.Add(child);
        return child;
    }

    /// <summary>
    /// Removes a child document from this scene.
    /// </summary>
    /// <param name="document">The document to remove.</param>
    /// <returns><see langword="true"/> if the document was found and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveChildDocument(Document document)
    {
        var child = ChildSceneItems.FirstOrDefault(c => c.Document == document);
        if (child != null)
        {
            document.Parent = null;
            ChildSceneItems.Remove(child);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the SceneChild metadata for a child document.
    /// </summary>
    /// <param name="document">The document to find metadata for.</param>
    /// <returns>The SceneChild if found; otherwise, <see langword="null"/>.</returns>
    public SceneChild? GetSceneChild(Document document) =>
        ChildSceneItems.FirstOrDefault(c => c.Document == document);

    #endregion

    /// <summary>
    /// Index of the selected child in the settings panel child properties list.
    /// </summary>
    public int SelectedChildIndex { get; set; } = -1;

    public ImGuiList<ZoneSimplified> Zones => new ImGuiList<ZoneSimplified>();

    public ImGuiList<SimpleObjectDefinition> SimpleObjects => new ImGuiList<SimpleObjectDefinition>();

    public DocumentScene()
    {
        // Create a minimal placeholder surface for scene overview
        _placeholderSurface = new ScreenSurface(1, 1);
        _placeholderSurface.Surface.DefaultBackground = Color.DarkSlateGray;
        _placeholderSurface.Surface.Clear();

        EditingSurface = _placeholderSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;

        // Scene draws itself (custom UI for overview)
        Options.DrawSelf = true;
        Options.UseToolsWindow = false;
        Options.DisableScrolling = true;
    }

    public override void OnSelected()
    {
        // Scene overview mode - no tools
        Core.State.Tools.Objects.Clear();
        Core.State.SyncEditorPalette();
    }

    public override void OnDeselected()
    {
        Core.State.Tools.Objects.Clear();
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        BuildSceneSettings(renderer);
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

        // Child documents info
        ImGui.SeparatorText("Child Documents"u8);

        int childCount = ChildSceneItems.Count;

        if (childCount == 0)
        {
            ImGuiSC.TextWrappedDisabled("No child documents. Use Document Options menu to import."u8);
        }
        else
        {
            ImGui.Text($"Contains {childCount} child document(s).");
            ImGui.TextDisabled("Select a child in the document list above to edit it."u8);

            ImGui.Separator();

            // Child list for metadata editing (position, label)
            ImGui.SeparatorText("Child Properties"u8);

            if (ImGui.BeginChild("child_props_list"u8, new Vector2(-1, ImGui.GetTextLineHeightWithSpacing() * Math.Min(5, childCount + 1)), ImGuiChildFlags.Borders))
            {
                for (int i = 0; i < childCount; i++)
                {
                    ImGui.PushID(i);

                    bool isSelected = SelectedChildIndex == i;
                    if (ImGui.Selectable(ChildSceneItems[i].Title, isSelected))
                        SelectedChildIndex = i;

                    ImGui.PopID();
                }
            }
            ImGui.EndChild();

            // Remove button
            ImGui.BeginDisabled(SelectedChildIndex < 0 || SelectedChildIndex >= childCount);
            if (ImGui.Button("Remove from Scene"u8))
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
                    var childToRemove = ChildSceneItems[SelectedChildIndex];
                    RemoveChildDocument(childToRemove.Document);
                    SelectedChildIndex = Math.Min(SelectedChildIndex, ChildSceneItems.Count - 1);
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            // Selected child properties
            if (SelectedChildIndex >= 0 && SelectedChildIndex < childCount)
            {
                ImGui.Separator();

                var child = ChildSceneItems[SelectedChildIndex];

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
        // Scene overview mode
        DrawSceneOverview(renderer);
    }

    private void DrawSceneOverview(ImGuiRenderer renderer)
    {
        ImGui.Text($"Scene: {Title}");
        ImGui.Separator();

        if (ChildSceneItems.Count == 0)
        {
            ImGuiSC.TextWrappedDisabled("This scene has no child documents."u8);
            ImGuiSC.TextWrappedDisabled("Use 'Document Options > Import document from list' to add documents."u8);
        }
        else
        {
            ImGui.Text($"Contains {ChildSceneItems.Count} child document(s).");

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
                        
                        // Add to scene using helper method that sets Parent
                        AddChildDocument(doc);
                        SelectedChildIndex = ChildSceneItems.Count - 1;

                        // If this was the selected document, clear selection
                        if (Core.State.SelectedDocument == doc)
                            Core.State.SelectedDocument = this;
                    }
                }
                ImGui.EndMenu();
            }
            ImGui.EndDisabled();

            // Transfer back to main list
            ImGui.BeginDisabled(ChildSceneItems.Count == 0);
            if (ImGui.BeginMenu("Transfer document to list"u8))
            {
                for (int i = 0; i < ChildSceneItems.Count; i++)
                {
                    if (ImGui.MenuItem(ChildSceneItems[i].Title))
                    {
                        var child = ChildSceneItems[i];
                        
                        // Remove from scene using helper method that clears Parent
                        RemoveChildDocument(child.Document);
                        
                        // Add to main list
                        Core.State.Documents.Objects.Add(child.Document);
                        
                        // Update selection
                        if (SelectedChildIndex >= ChildSceneItems.Count)
                            SelectedChildIndex = ChildSceneItems.Count - 1;
                    }
                }
                ImGui.EndMenu();
            }
            ImGui.EndDisabled();

            ImGui.EndMenu();
        }
    }

    public override void Redraw(bool redrawSurface, bool redrawTooling)
    {
        // Scene overview doesn't need complex rendering
        // Just ensure placeholder is valid
        _placeholderSurface.Render(Game.Instance.UpdateFrameDelta);
    }

    public override void SetSurfaceView(int x, int y, int width, int height)
    {
        // Scene overview doesn't have a scrollable view
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SceneDocument()];

    public override string ToString() =>
        Title;
}
