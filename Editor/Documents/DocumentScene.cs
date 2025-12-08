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
    /// The total size of the scene in pixels. This defines the virtual space bounds.
    /// </summary>
    public Point ScenePixelSize { get; set; } = new Point(640, 480);

    /// <summary>
    /// The current view position (top-left corner) within the scene in pixels.
    /// </summary>
    public Point ViewPosition { get; set; } = Point.Zero;

    /// <summary>
    /// The size of the visible area in the editor in pixels. This is calculated based on the available ImGui region.
    /// </summary>
    public Point ViewPixelSize { get; private set; } = Point.Zero;

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

        // Scene draws itself with custom rendering
        Options.DrawSelf = true;
        Options.UseToolsWindow = false;
        Options.DisableScrolling = false; // Enable scrolling for scene view

        // Initialize scene bounds (default 640x480 virtual space)
        ScenePixelSize = new Point(640, 480);
        ViewPosition = Point.Zero;
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

        // Scene size configuration
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Width (px):"u8);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        int sceneWidth = ScenePixelSize.X;
        if (ImGui.InputInt("##scenewidth"u8, ref sceneWidth))
        {
            sceneWidth = Math.Max(1, sceneWidth);
            ScenePixelSize = new Point(sceneWidth, ScenePixelSize.Y);
        }

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Height (px):"u8);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        int sceneHeight = ScenePixelSize.Y;
        if (ImGui.InputInt("##sceneheight"u8, ref sceneHeight))
        {
            sceneHeight = Math.Max(1, sceneHeight);
            ScenePixelSize = new Point(ScenePixelSize.X, sceneHeight);
        }

        ImGui.TextDisabled($"View: {ViewPixelSize.X}x{ViewPixelSize.Y} at ({ViewPosition.X}, {ViewPosition.Y})");

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
        if (ImGui.BeginChild("scene_viewport"u8))
        {
            int fontSize = (int)ImGui.GetFontSize();
            Vector2 padding = ImGui.GetStyle().FramePadding;
            Vector2 barSize = new Vector2(fontSize + padding.X * 2, fontSize + padding.Y * 2);

            // Calculate available region for the scene view (minus scrollbars)
            Vector2 availRegion = ImGui.GetContentRegionAvail() - new Vector2(barSize.X, barSize.Y);

            // Update view pixel size based on available region
            int viewWidth = Math.Max(1, (int)availRegion.X);
            int viewHeight = Math.Max(1, (int)availRegion.Y);
            ViewPixelSize = new Point(viewWidth, viewHeight);

            // Determine the actual drawable area (smaller of scene size or available region)
            Vector2 pixelArea = new Vector2(
                Math.Min(ScenePixelSize.X, viewWidth),
                Math.Min(ScenePixelSize.Y, viewHeight)
            );

            // Get draw list for custom rendering
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            Vector2 startPos = ImGui.GetCursorScreenPos();

            // Create invisible button for interaction area
            ImGui.InvisibleButton("##scene_area"u8, pixelArea, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight);
            bool isHovered = ImGui.IsItemHovered();
            bool isActive = ImGui.IsItemActive();

            // Draw checkerboard background for the scene area
            ImGuiP.RenderColorRectWithAlphaCheckerboard(drawList, startPos, startPos + pixelArea, 
                ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1f)), 16f, Vector2.Zero);

            // Draw border around scene area
            drawList.AddRect(startPos, startPos + pixelArea, ImGuiSC.Color_White);

            // Draw each child document at its position (offset by view position)
            foreach (var child in ChildSceneItems)
            {
                // Ensure child document has a valid texture
                if (child.Document.VisualTextureId == IntPtr.Zero)
                    continue;

                // Calculate child position in pixels relative to the scene
                Vector2 childPixelPos;
                if (child.UsePixelPositioning)
                {
                    childPixelPos = new Vector2(child.Position.X, child.Position.Y);
                }
                else
                {
                    // Cell positioning - convert to pixels using the child's font size
                    childPixelPos = new Vector2(
                        child.Position.X * child.Document.EditorFontSize.X,
                        child.Position.Y * child.Document.EditorFontSize.Y
                    );
                }

                // Offset by current view position
                Vector2 screenPos = startPos + childPixelPos - new Vector2(ViewPosition.X, ViewPosition.Y);

                // Get child texture size
                Vector2 childSize = child.Document.VisualTextureSize;

                // Calculate visible portion of the child (clipping)
                Vector2 minPos = screenPos;
                Vector2 maxPos = screenPos + childSize;

                // Clamp to visible area
                Vector2 clampedMin = Vector2.Max(minPos, startPos);
                Vector2 clampedMax = Vector2.Min(maxPos, startPos + pixelArea);

                // Only draw if there's something visible
                if (clampedMin.X < clampedMax.X && clampedMin.Y < clampedMax.Y)
                {
                    // Calculate UV coordinates for clipping
                    Vector2 uvMin = (clampedMin - minPos) / childSize;
                    Vector2 uvMax = Vector2.One - (maxPos - clampedMax) / childSize;

                    // Draw the child document texture
                    drawList.AddImage(child.Document.VisualTextureId, clampedMin, clampedMax, uvMin, uvMax);

                    // Draw a subtle border around the child
                    drawList.AddRect(clampedMin, clampedMax, ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
                }
            }

            // Handle mouse position for status bar
            if (isHovered)
            {
                Vector2 mousePosition = ImGui.GetMousePos();
                Vector2 relativeMousePos = mousePosition - startPos;
                Point sceneMousePos = new Point(
                    (int)relativeMousePos.X + ViewPosition.X,
                    (int)relativeMousePos.Y + ViewPosition.Y
                );

                Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Scene Pos:"));
                Core.State.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), sceneMousePos.ToString()));
            }

            // Print viewport stats
            Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| View:"));
            Core.State.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), $"{ViewPixelSize.X}x{ViewPixelSize.Y} at ({ViewPosition.X},{ViewPosition.Y})"));

            // Draw vertical scrollbar
            bool enableScrollY = ScenePixelSize.Y > ViewPixelSize.Y;
            int sliderValueY = ViewPosition.Y;

            ImGui.SameLine();
            ImGui.BeginDisabled(!enableScrollY);
            if (ImGuiSC.VSliderIntNudges("##scene_scrolly", new Vector2(barSize.X, pixelArea.Y), ref sliderValueY,
                                        Math.Max(0, ScenePixelSize.Y - ViewPixelSize.Y), 0, ImGuiSliderFlags.AlwaysClamp))
            {
                SetSurfaceView(ViewPosition.X, sliderValueY, 0, 0);
            }
            ImGui.EndDisabled();

            // Draw horizontal scrollbar
            bool enableScrollX = ScenePixelSize.X > ViewPixelSize.X;
            int sliderValueX = ViewPosition.X;

            ImGui.BeginDisabled(!enableScrollX);
            if (ImGuiSC.SliderIntNudges("##scene_scrollx", (int)pixelArea.X, ref sliderValueX, 0,
                                       Math.Max(0, ScenePixelSize.X - ViewPixelSize.X), "%d", ImGuiSliderFlags.AlwaysClamp))
            {
                SetSurfaceView(sliderValueX, ViewPosition.Y, 0, 0);
            }
            ImGui.EndDisabled();
        }
        ImGui.EndChild();
    }

    public override void Redraw(bool redrawSurface, bool redrawTooling)
    {
        // Ensure placeholder is valid
        _placeholderSurface.Render(Game.Instance.UpdateFrameDelta);

        // Redraw all child documents to ensure their textures are up to date
        foreach (var child in ChildSceneItems)
        {
            child.Document.Redraw(redrawSurface, redrawTooling);
        }
    }

    public override void SetSurfaceView(int x, int y, int width, int height)
    {
        // Clamp view position within scene bounds
        int maxX = Math.Max(0, ScenePixelSize.X - ViewPixelSize.X);
        int maxY = Math.Max(0, ScenePixelSize.Y - ViewPixelSize.Y);
        
        ViewPosition = new Point(
            Math.Clamp(x, 0, maxX),
            Math.Clamp(y, 0, maxY)
        );
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SceneDocument()];

    public override string ToString() =>
        Title;
}
