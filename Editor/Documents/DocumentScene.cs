using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

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
    /// The child currently being dragged, or null if no drag is in progress.
    /// </summary>
    private SceneChild? _draggingChild;

    /// <summary>
    /// The offset from the mouse position to the child's position when drag started.
    /// </summary>
    private Vector2 _dragOffset;

    /// <summary>
    /// Tracks if the mouse was down last frame for click detection.
    /// </summary>
    private bool _wasDragging;

    /// <summary>
    /// The child currently being resized, or null if no resize is in progress.
    /// </summary>
    private SceneChild? _resizingChild;

    /// <summary>
    /// The original viewport size when resize started, used to calculate delta.
    /// </summary>
    private Vector2 _resizeStartSize;

    /// <summary>
    /// The mouse position when resize started.
    /// </summary>
    private Vector2 _resizeStartMousePos;

    /// <summary>
    /// The child currently having its viewport panned, or null if no pan is in progress.
    /// </summary>
    private SceneChild? _panningChild;

    /// <summary>
    /// The original viewport position when pan started.
    /// </summary>
    private Point _panStartViewportPos;

    /// <summary>
    /// The mouse position when pan started.
    /// </summary>
    private Vector2 _panStartMousePos;

    /// <summary>
    /// Tracks if right mouse was down last frame for pan click detection.
    /// </summary>
    private bool _wasPanning;

    /// <summary>
    /// Sets the non-selected docs to be semi transparent in the scene view.
    /// </summary>
    private bool _otherDocsTranparent = false;

    private uint _docsTransparent = Color.White.SetAlpha(150).PackedValue;
    private uint _docsNormal = Color.White.PackedValue;

    /// <summary>
    /// The size of the resize grip handle in pixels.
    /// </summary>
    private const float ResizeGripSize = 12f;

    private Windows.SceneFontSizeCalculatorWindow? _sceneSizeCalculatorWindow;

    /// <summary>
    /// Gets the icon for scene documents.
    /// </summary>
    public override string DocumentIcon => ""; // nf-fa-object_group

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
            child.Dispose(); // Dispose texture resources
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

    /// <summary>
    /// Checks if this scene is a descendant of another scene (directly or indirectly).
    /// Used to prevent circular references when importing scenes.
    /// </summary>
    /// <param name="potentialAncestor">The scene to check as a potential ancestor.</param>
    /// <returns><see langword="true"/> if this scene is a descendant of the specified scene; otherwise, <see langword="false"/>.</returns>
    public bool IsDescendantOf(DocumentScene potentialAncestor)
    {
        // Check if this scene exists anywhere in the potentialAncestor's child hierarchy
        return ContainsDocumentRecursive(potentialAncestor, this);
    }

    /// <summary>
    /// Recursively checks if a scene contains a specific document.
    /// </summary>
    private static bool ContainsDocumentRecursive(DocumentScene scene, Document documentToFind)
    {
        foreach (var child in scene.ChildSceneItems)
        {
            if (child.Document == documentToFind)
                return true;

            if (child.Document is DocumentScene childScene)
            {
                if (ContainsDocumentRecursive(childScene, documentToFind))
                    return true;
            }
        }
        return false;
    }

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
        Options.UseToolsWindow = true;
        Options.ToolsWindowShowToolsList = false;
        Options.DisableScrolling = false;

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

    public override void ImGuiDrawInToolsPanel(ImGuiRenderer renderer)
    {
        BuildChildDocumentsSettings(renderer);
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

        if (ImGui.Button("Set By Font"))
        {
            _sceneSizeCalculatorWindow = new Windows.SceneFontSizeCalculatorWindow();
            _sceneSizeCalculatorWindow.AddOnOpen = false;
            _sceneSizeCalculatorWindow.Open();
        }

        if (_sceneSizeCalculatorWindow != null)
        {
            _sceneSizeCalculatorWindow.BuildUI(renderer);

            if (!_sceneSizeCalculatorWindow.IsOpen)
            {
                if (_sceneSizeCalculatorWindow.DialogResult)
                    ScenePixelSize = _sceneSizeCalculatorWindow.CalculatedPixelSize;

                _sceneSizeCalculatorWindow = null;
            }
        }

        if (Options.SupportsMetadata)
            base.ImGuiDrawMetadataSettings();
    }

    private void BuildChildDocumentsSettings(ImGuiRenderer renderer)
    {
        // Child documents info
        ImGui.SeparatorText("Child Documents"u8);

        int childCount = ChildSceneItems.Count;

        if (childCount == 0)
        {
            ImGuiSC.TextWrappedDisabled("No child documents. Use Document Options menu to import."u8);
        }
        else
        {
            float buttonSize = ImGui.GetFrameHeight();
            Vector2 squareButtonSize = new Vector2(buttonSize, buttonSize);
            ImGui.GetStyle().GetSpacing(out Vector2 framePadding, out Vector2 itemSpacing);

            if (ImGui.BeginChild("child_props_list"u8, new Vector2(-1, ImGui.GetTextLineHeightWithSpacing() * Math.Min(5, childCount + 1)), ImGuiChildFlags.Borders))
            {
                Vector2 iconSize = ImGui.CalcTextSize("\uf06e\uf06e"u8);

                for (int i = 0; i < childCount; i++)
                {
                    ImGui.PushID(i);
                    var child = ChildSceneItems[i];

                    char icon = child.IsVisible ? '\uf06e' : '\uf070';
                    bool isSelected = SelectedChildIndex == i;
                    Vector2 pos = ImGui.GetCursorScreenPos();
                    if (ImGui.Selectable($"{icon}  {child.Document.DocumentIcon} {child.Title}", isSelected, ImGuiSelectableFlags.AllowOverlap))
                        SelectedChildIndex = i;

                    ImGui.SetCursorScreenPos(pos);
                    if (ImGui.InvisibleButton("##vis", iconSize))
                        child.IsVisible = !child.IsVisible;

                    ImGui.PopID();
                }
            }
            ImGui.EndChild();

            // Reorder and Remove buttons
            // Arrow up
            ImGui.BeginDisabled(SelectedChildIndex <= 0);
            if (ImGui.Button("\uf062"u8, squareButtonSize))
            {
                var item = ChildSceneItems[SelectedChildIndex];
                ChildSceneItems.RemoveAt(SelectedChildIndex);
                ChildSceneItems.Insert(SelectedChildIndex - 1, item);
                SelectedChildIndex--;
            }
            ImGui.EndDisabled();

            ImGui.SameLine();

            // Arrow down
            ImGui.BeginDisabled(SelectedChildIndex < 0 || SelectedChildIndex >= childCount - 1);
            if (ImGui.Button("\uf063"u8, squareButtonSize)) // Arrow down
            {
                var item = ChildSceneItems[SelectedChildIndex];
                ChildSceneItems.RemoveAt(SelectedChildIndex);
                ChildSceneItems.Insert(SelectedChildIndex + 1, item);
                SelectedChildIndex++;
            }
            ImGui.EndDisabled();

            ImGui.SameLine();

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

            // Option to make semi transparent non-selected docs
            ImGui.Checkbox("Show Non-Selected Transparent"u8, ref _otherDocsTranparent);

            // Selected child properties
            if (SelectedChildIndex >= 0 && SelectedChildIndex < childCount)
            {
                // Child list for metadata editing (position, label)
                ImGui.SeparatorText("Child Properties"u8);

                if (ImGui.BeginChild("child_props_container"u8))
                {

                    var child = ChildSceneItems[SelectedChildIndex];

                    // Document type info
                    ImGui.TextDisabled($"Type: {child.Document.GetType().Name}");
                    ImGui.Separator();


                    // Label
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Label:"u8);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(-1);
                    string label = child.Label;
                    if (ImGui.InputText("##childlabel"u8, ref label, 50))
                        child.Label = label;

                    // Position
                    ImGui.BeginGroup();
                    float inputWidth = ImGui.CalcTextSize("00000000").X;
                    int posX = child.Position.X;
                    int posY = child.Position.Y;

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Position X:"u8);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(squareButtonSize.X * 2 + framePadding.X * 2 + inputWidth);
                    if (ImGui.InputInt("##posx"u8, ref posX))
                    {
                        child.Position = new Point(posX, child.Position.Y);
                    }

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Position Y:"u8);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(squareButtonSize.X * 2 + framePadding.X * 2 + inputWidth);
                    if (ImGui.InputInt("##posy"u8, ref posY))
                    {
                        child.Position = new Point(child.Position.X, posY);
                    }

                    // Pixel positioning
                    bool usePixel = child.UsePixelPositioning;
                    if (ImGui.Checkbox("Use Pixel Positioning"u8, ref usePixel))
                    {
                        child.UsePixelPositioning = usePixel;
                        child.Document.EditingSurface.UsePixelPositioning = usePixel;
                    }
                    ImGui.EndGroup();

                    ImGui.SameLine();
                    ImGuiP.SeparatorEx(ImGuiSeparatorFlags.Vertical, 1);
                    ImGui.SameLine();
                    ImGui.BeginGroup();
                    // Alignment buttons
                    

                    static void SetChildPosition(SceneChild child, Vector2 pixelPos)
                    {
                        if (child.UsePixelPositioning)
                        {
                            child.Position = new Point((int)pixelPos.X, (int)pixelPos.Y);
                        }
                        else
                        {
                            Point fontSize = child.GetEffectiveFontSize();
                            child.Position = new Point((int)(pixelPos.X / fontSize.X), (int)(pixelPos.Y / fontSize.Y));
                        }
                        child.Document.EditingSurface.Position = child.Position;
                    }

                    Vector2 childSize = child.SceneTextureSize;
                    Vector2 sceneSize = new Vector2(ScenePixelSize.X, ScenePixelSize.Y);

                    // Top row
                    if (ImGui.Button("\ue0bc"u8, squareButtonSize)) SetChildPosition(child, new Vector2(0, 0)); // Top-left
                    ImGui.SameLine();
                    if (ImGui.Button("\U000f11c7"u8, squareButtonSize)) SetChildPosition(child, new Vector2((sceneSize.X - childSize.X) / 2, 0)); // Top-center
                    ImGui.SameLine();
                    if (ImGui.Button("\ue0be"u8, squareButtonSize)) SetChildPosition(child, new Vector2(sceneSize.X - childSize.X, 0)); // Top-right

                    // Middle row
                    if (ImGui.Button("\U000f11c2"u8, squareButtonSize)) SetChildPosition(child, new Vector2(0, (sceneSize.Y - childSize.Y) / 2)); // Middle-left
                    ImGui.SameLine();
                    if (ImGui.Button("\U000f11c3"u8, squareButtonSize)) SetChildPosition(child, new Vector2((sceneSize.X - childSize.X) / 2, (sceneSize.Y - childSize.Y) / 2)); // Middle-center
                    ImGui.SameLine();
                    if (ImGui.Button("\U000f11c4"u8, squareButtonSize)) SetChildPosition(child, new Vector2(sceneSize.X - childSize.X, (sceneSize.Y - childSize.Y) / 2)); // Middle-right

                    // Bottom row
                    if (ImGui.Button("\ue0b8"u8, squareButtonSize)) SetChildPosition(child, new Vector2(0, sceneSize.Y - childSize.Y)); // Bottom-left
                    ImGui.SameLine();
                    if (ImGui.Button("\U000f11c5"u8, squareButtonSize)) SetChildPosition(child, new Vector2((sceneSize.X - childSize.X) / 2, sceneSize.Y - childSize.Y)); // Bottom-center
                    ImGui.SameLine();
                    if (ImGui.Button("\ue0ba"u8, squareButtonSize)) SetChildPosition(child, new Vector2(sceneSize.X - childSize.X, sceneSize.Y - childSize.Y)); // Bottom-right
                    ImGui.EndGroup();
                    ImGui.Separator();

                    // For DocumentScene children, show size info but no viewport controls
                    // For other documents, show viewport controls
                    if (child.Document is DocumentScene childSceneDoc)
                    {
                        // Show scene size info (read-only)
                        ImGui.TextDisabled($"Scene Size: {childSceneDoc.ScenePixelSize.X}x{childSceneDoc.ScenePixelSize.Y} px");

                        // Show font selector for cell-based positioning when not using pixel positioning
                        if (!child.UsePixelPositioning)
                        {
                            ImGui.SeparatorText("Cell Size (for positioning)"u8);

                            ImGui.AlignTextToFramePadding();
                            ImGui.Text("Cell Width:"u8);
                            ImGui.SameLine();
                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                            int cellWidth = child.SceneFontSize.X;
                            if (ImGui.InputInt("##cellwidth"u8, ref cellWidth))
                            {
                                cellWidth = Math.Max(1, cellWidth);
                                child.SceneFontSize = new Point(cellWidth, child.SceneFontSize.Y);
                            }

                            ImGui.AlignTextToFramePadding();
                            ImGui.Text("Cell Height:"u8);
                            ImGui.SameLine();
                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                            int cellHeight = child.SceneFontSize.Y;
                            if (ImGui.InputInt("##cellheight"u8, ref cellHeight))
                            {
                                cellHeight = Math.Max(1, cellHeight);
                                child.SceneFontSize = new Point(child.SceneFontSize.X, cellHeight);
                            }

                            // Quick font size selection from available fonts
                            if (ImGui.BeginCombo("##fontpreset"u8, "Set from font..."u8))
                            {
                                foreach (var font in Core.State.SadConsoleFonts.Objects)
                                {
                                    Point fontSize = font.GetFontSize(IFont.Sizes.One);
                                    if (ImGui.Selectable($"{font.Name} ({fontSize.X}x{fontSize.Y})"))
                                    {
                                        child.SceneFontSize = fontSize;
                                    }
                                }
                                ImGui.EndCombo();
                            }
                        }
                    }
                    else
                    {
                        // Viewport input for non-scene documents
                        int viewWidth = child.Viewport?.Width ?? child.Document.EditingSurface.Surface.View.Width;
                        int viewHeight = child.Viewport?.Height ?? child.Document.EditingSurface.Surface.View.Height;
                        int viewPositionX = child.Viewport?.X ?? 0;
                        int viewPositionY = child.Viewport?.Y ?? 0;

                        ImGui.AlignTextToFramePadding();
                        ImGui.Text("View Pos X:"u8);
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - squareButtonSize.X - itemSpacing.X - 1);
                        if (ImGui.InputInt("##viewposx"u8, ref viewPositionX))
                            viewPositionX = Math.Max(viewPositionX, 0);
                        ImGui.SameLine();
                        if (ImGui.Button("\uf0e2##viewposx"u8, squareButtonSize))
                            viewPositionX = 0;

                        ImGui.AlignTextToFramePadding();
                        ImGui.Text("View Pos Y:"u8);
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - squareButtonSize.X - itemSpacing.X - 1);
                        if (ImGui.InputInt("##viewposy"u8, ref viewPositionY))
                            viewPositionY = Math.Max(viewPositionY, 0);
                        ImGui.SameLine();
                        if (ImGui.Button("\uf0e2##viewposy"u8, squareButtonSize))
                            viewPositionY = 0;

                        ImGui.AlignTextToFramePadding();
                        ImGui.Text("View Width:"u8);
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - squareButtonSize.X - itemSpacing.X - 1);
                        if (ImGui.InputInt("##viewwidth"u8, ref viewWidth))
                            viewWidth = Math.Clamp(viewWidth, 1, child.Document.EditingSurface.Surface.Width);
                        ImGui.SameLine();
                        if (ImGui.Button("\uf0e2##viewwidth"u8, squareButtonSize))
                            viewWidth = child.Document.EditingSurface.Surface.Width;

                        ImGui.AlignTextToFramePadding();
                        ImGui.Text("View Height:"u8);
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - squareButtonSize.X - itemSpacing.X - 1);
                        if (ImGui.InputInt("##viewheight"u8, ref viewHeight))
                            viewHeight = Math.Clamp(viewHeight, 1, child.Document.EditingSurface.Surface.Height);
                        ImGui.SameLine();
                        if (ImGui.Button("\uf0e2##viewheight"u8, squareButtonSize))
                            viewHeight = child.Document.EditingSurface.Surface.Height;

                        child.Viewport = new Rectangle(
                            viewPositionX,
                            viewPositionY,
                            viewWidth,
                            viewHeight
                        );
                    }

                    // Animation controls for DocumentAnimated children
                    if (child.Document is DocumentAnimated animDoc)
                    {
                        ImGui.SeparatorText("Animation Controls"u8);

                        // Reset button
                        if (ImGui.Button("\uf0e2##animreset"u8, squareButtonSize))
                        {
                            animDoc._baseAnimation.Stop();
                            animDoc._baseAnimation.CurrentFrameIndex = 0;
                            animDoc.SetFrameIndex(0);
                            child.RefreshTexture();
                        }

                        ImGui.SameLine();

                        int currentFrameIndex = animDoc._baseAnimation.CurrentFrameIndex;

                        // Play/Stop button
                        if (animDoc._baseAnimation.IsPlaying)
                        {
                            if (ImGui.Button("\uf04d Stop"u8))
                                animDoc._baseAnimation.Stop();

                            animDoc._baseAnimation.Update(Game.Instance.UpdateFrameDelta);

                            if (animDoc.EditingSurface.Surface != animDoc._baseAnimation.CurrentFrame)
                            {
                                ((ScreenSurface)animDoc.EditingSurface).Surface = animDoc._baseAnimation.CurrentFrame;
                                animDoc.EditingSurface.IsDirty = true;
                                child.RefreshTexture();
                            }
                        }
                        else
                        {
                            if (ImGui.Button("\uf04b Play"u8))
                                animDoc._baseAnimation.Restart();
                        }

                        ImGui.SameLine();

                        // Repeat checkbox
                        bool repeat = animDoc._baseAnimation.Repeat;
                        if (ImGui.Checkbox("Repeat"u8, ref repeat))
                            animDoc._baseAnimation.Repeat = repeat;


                        // Frame navigation (disabled while playing)
                        ImGui.BeginDisabled(animDoc._baseAnimation.IsPlaying);

                        float frameButtonArea = ImGui.GetStyle().ItemSpacing.X * 2 + ImGui.GetStyle().FramePadding.X * 4 + ImGui.CalcTextSize("\uf049 "u8).X + ImGui.CalcTextSize("\uf04a "u8).X;

                        ImGui.BeginDisabled(animDoc._baseAnimation.CurrentFrameIndex == 0);
                        if (ImGui.Button("\uf049 "u8)) animDoc._baseAnimation.MoveStart(); ImGui.EndDisabled(); ImGui.SameLine();
                        ImGui.BeginDisabled(animDoc._baseAnimation.CurrentFrameIndex == 0);
                        if (ImGui.Button("\uf04a "u8)) animDoc._baseAnimation.MovePrevious(); ImGui.EndDisabled(); ImGui.SameLine();

                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - frameButtonArea);
                        int frameIndex = animDoc._baseAnimation.CurrentFrameIndex + 1;
                        if (ImGui.SliderInt("##frameslider"u8, ref frameIndex, 1, animDoc._baseAnimation.Frames.Count))
                            animDoc._baseAnimation.CurrentFrameIndex = frameIndex - 1;

                        ImGui.SameLine();

                        ImGui.BeginDisabled(animDoc._baseAnimation.CurrentFrameIndex == animDoc._baseAnimation.Frames.Count - 1);
                        if (ImGui.Button("\uf04e "u8)) animDoc._baseAnimation.MoveNext(); ImGui.EndDisabled(); ImGui.SameLine();
                        ImGui.BeginDisabled(animDoc._baseAnimation.CurrentFrameIndex == animDoc._baseAnimation.Frames.Count - 1);
                        if (ImGui.Button("\uf050 "u8)) animDoc._baseAnimation.MoveEnd(); ImGui.EndDisabled();

                        // If animation frame changed, sync surface
                        if (currentFrameIndex != animDoc._baseAnimation.CurrentFrameIndex)
                        {
                            animDoc.SetFrameIndex(animDoc._baseAnimation.CurrentFrameIndex);
                            child.RefreshTexture();
                        }

                        ImGui.EndDisabled();

                        ImGui.Separator();
                    }

                }

                ImGui.EndChild();
            }
        }
    }

    public override void ImGuiDrawSelf(ImGuiRenderer renderer)
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

            // Get mouse position relative to scene
            Vector2 mousePosition = ImGui.GetMousePos();
            Vector2 relativeMousePos = mousePosition - startPos;
            Vector2 sceneMousePos = relativeMousePos + new Vector2(ViewPosition.X, ViewPosition.Y);

            // Handle mouse drag for child positioning
            HandleChildDragging(startPos, pixelArea, sceneMousePos, isHovered, isActive);

            // Draw checkerboard background for the scene area
            ImGuiP.RenderColorRectWithAlphaCheckerboard(drawList, startPos, startPos + pixelArea, 
                ImGui.GetColorU32(new Vector4(0.2f, 0.2f, 0.2f, 1f)), 16f, Vector2.Zero);

            // Draw border around scene area
            drawList.AddRect(startPos, startPos + pixelArea, ImGuiSC.Color_White);

            // Draw each child document at its position (offset by view position)
            foreach (var child in ChildSceneItems)
            {
                // Skip if not visible
                if (!child.IsVisible)
                    continue;

                // Ensure child has a valid scene texture
                if (!child.HasValidTexture)
                    continue;

                // Calculate child position in pixels relative to the scene
                Vector2 childPixelPos = GetChildPixelPosition(child);

                // Offset by current view position
                Vector2 screenPos = startPos + childPixelPos - new Vector2(ViewPosition.X, ViewPosition.Y);

                // Get child's scene texture size (full document, not viewport-clipped)
                Vector2 childSize = child.SceneTextureSize;

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

                    // Draw the child's scene texture (full document)
                    uint color =
                                 SelectedChildIndex != -1
                                    ? child == ChildSceneItems[SelectedChildIndex]
                                        ? _docsNormal
                                        : _otherDocsTranparent
                                            ? _docsTransparent
                                            : _docsNormal
                                    : _docsNormal;

                    drawList.AddImage(child.SceneTextureId, clampedMin, clampedMax, uvMin, uvMax, color);

                    // Highlight if being dragged or selected
                    bool isBeingDragged = _draggingChild == child;
                    bool isBeingResized = _resizingChild == child;
                    bool isSelected = SelectedChildIndex >= 0 && SelectedChildIndex < ChildSceneItems.Count && 
                                     ChildSceneItems[SelectedChildIndex] == child;

                    uint borderColor;
                    if (isBeingDragged || isBeingResized)
                        borderColor = ImGui.GetColorU32(new Vector4(1f, 1f, 0f, 1f)); // Yellow when dragging/resizing
                    else if (isSelected)
                        borderColor = ImGui.GetColorU32(new Vector4(0f, 1f, 1f, 1f)); // Cyan when selected
                    else
                        borderColor = ImGui.GetColorU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)); // Gray otherwise

                    drawList.AddRect(clampedMin, clampedMax, borderColor);

                    // Draw resize grip at bottom-right corner (only if selected or hovered, and not a scene document)
                    bool isSceneChild = child.Document is DocumentScene;
                    if ((isSelected || isBeingResized) && !isSceneChild)
                    {
                        Vector2 gripMin = new Vector2(maxPos.X - ResizeGripSize, maxPos.Y - ResizeGripSize);
                        Vector2 gripMax = maxPos;

                        // Clamp grip to visible area
                        gripMin = Vector2.Max(gripMin, startPos);
                        gripMax = Vector2.Min(gripMax, startPos + pixelArea);

                        if (gripMin.X < gripMax.X && gripMin.Y < gripMax.Y)
                        {
                            // Draw grip handle (filled triangle or rectangle)
                            uint gripColor = isBeingResized 
                                ? ImGui.GetColorU32(new Vector4(1f, 1f, 0f, 1f))  // Yellow when resizing
                                : ImGui.GetColorU32(new Vector4(0f, 1f, 1f, 0.8f)); // Cyan otherwise
                            

                            // Draw a filled triangle in the corner
                            drawList.AddTriangleFilled(
                                new Vector2(gripMax.X, gripMin.Y),
                                gripMax,
                                new Vector2(gripMin.X, gripMax.Y),
                                gripColor);
                        }
                    }
                }
            }

            // Handle mouse position for status bar
            if (isHovered)
            {
                Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Scene Pos:"));
                Core.State.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), $"({(int)sceneMousePos.X}, {(int)sceneMousePos.Y})"));

                if (_draggingChild != null)
                {
                    Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Dragging:"));
                    Core.State.GuiTopBar.StatusItems.Add((Color.Cyan.ToVector4(), _draggingChild.Title));
                }
                else if (_resizingChild != null)
                {
                    Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Resizing:"));
                    Core.State.GuiTopBar.StatusItems.Add((Color.Cyan.ToVector4(), _resizingChild.Title));
                    if (_resizingChild.Viewport.HasValue)
                    {
                        Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, $"({_resizingChild.Viewport.Value.Width}x{_resizingChild.Viewport.Value.Height})"));
                    }
                }
                else if (_panningChild != null)
                {
                    Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Panning:"));
                    Core.State.GuiTopBar.StatusItems.Add((Color.Cyan.ToVector4(), _panningChild.Title));
                    if (_panningChild.Viewport.HasValue)
                    {
                        var vp = _panningChild.Viewport.Value;
                        int fullW = _panningChild.Document.EditingSurface.Surface.Width;
                        int fullH = _panningChild.Document.EditingSurface.Surface.Height;
                        Core.State.GuiTopBar.StatusItems.Add((Vector4.Zero, $"at ({vp.X},{vp.Y}) size ({vp.Width}x{vp.Height}) max ({fullW},{fullH})"));
                    }
                }
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

    /// <summary>
    /// Gets the pixel position of a child in scene coordinates.
    /// </summary>
    private Vector2 GetChildPixelPosition(SceneChild child)
    {
        if (child.UsePixelPositioning)
        {
            return new Vector2(child.Position.X, child.Position.Y);
        }
        else
        {
            // Cell positioning - convert to pixels using the effective font size
            Point fontSize = child.GetEffectiveFontSize();
            return new Vector2(
                child.Position.X * fontSize.X,
                child.Position.Y * fontSize.Y
            );
        }
    }

    /// <summary>
    /// Handles mouse dragging of child objects for repositioning, resizing, and viewport panning.
    /// </summary>
    private void HandleChildDragging(Vector2 startPos, Vector2 pixelArea, Vector2 sceneMousePos, bool isHovered, bool isActive)
    {
        bool leftMouseDown = ImGuiP.IsMouseDown(ImGuiMouseButton.Left);
        bool leftMouseClicked = isHovered && leftMouseDown && !_wasDragging;
        bool rightMouseDown = ImGuiP.IsMouseDown(ImGuiMouseButton.Right);
        bool rightMouseClicked = isHovered && rightMouseDown && !_wasPanning;

        // Handle ongoing viewport pan (right-click drag)
        if (_panningChild != null)
        {
            if (rightMouseDown)
            {
                // Calculate mouse delta from start position
                Vector2 mouseDelta = sceneMousePos - _panStartMousePos;
                Point fontSizePoint = _panningChild.Document.EditingSurfaceFontSize;

                // Convert pixel delta to cell delta (inverted for natural panning feel)
                int cellDeltaX = (int)(mouseDelta.X / fontSizePoint.X);
                int cellDeltaY = (int)(mouseDelta.Y / fontSizePoint.Y);

                // Calculate new viewport position based on start position plus delta
                int newX = _panStartViewportPos.X - cellDeltaX;
                int newY = _panStartViewportPos.Y - cellDeltaY;

                // Get the full surface dimensions and current viewport size
                int fullWidth = _panningChild.Document.EditingSurface.Surface.Width;
                int fullHeight = _panningChild.Document.EditingSurface.Surface.Height;
                int viewportWidth = _panningChild.Viewport?.Width ?? fullWidth;
                int viewportHeight = _panningChild.Viewport?.Height ?? fullHeight;

                // Clamp to valid range (viewport can't go past surface bounds)
                newX = Math.Clamp(newX, 0, Math.Max(0, fullWidth - viewportWidth));
                newY = Math.Clamp(newY, 0, Math.Max(0, fullHeight - viewportHeight));

                // Update viewport position
                _panningChild.Viewport = new Rectangle(newX, newY, viewportWidth, viewportHeight);
            }
            else
            {
                // Mouse released, stop panning
                _panningChild = null;
            }
        }
        // Handle ongoing resize
        else if (_resizingChild != null)
        {
            if (leftMouseDown)
            {
                // Calculate size delta from start position
                Vector2 mouseDelta = sceneMousePos - _resizeStartMousePos;
                Point fontSizePoint = _resizingChild.Document.EditingSurfaceFontSize;

                // Convert pixel delta to cell delta
                float cellDeltaX = mouseDelta.X / fontSizePoint.X;
                float cellDeltaY = mouseDelta.Y / fontSizePoint.Y;

                // Calculate new size based on start size plus delta
                int newCellWidth = Math.Max(1, (int)(_resizeStartSize.X + cellDeltaX));
                int newCellHeight = Math.Max(1, (int)(_resizeStartSize.Y + cellDeltaY));

                // Get the full surface dimensions and clamp
                int fullWidth = _resizingChild.Document.EditingSurface.Surface.Width;
                int fullHeight = _resizingChild.Document.EditingSurface.Surface.Height;
                newCellWidth = Math.Min(newCellWidth, fullWidth);
                newCellHeight = Math.Min(newCellHeight, fullHeight);

                // Update viewport (preserve position if set)
                int posX = _resizingChild.Viewport?.Position.X ?? 0;
                int posY = _resizingChild.Viewport?.Position.Y ?? 0;
                
                // Ensure position is still valid with new size
                posX = Math.Min(posX, Math.Max(0, fullWidth - newCellWidth));
                posY = Math.Min(posY, Math.Max(0, fullHeight - newCellHeight));
                
                _resizingChild.Viewport = new Rectangle(posX, posY, newCellWidth, newCellHeight);
            }
            else
            {
                // Mouse released, stop resizing
                _resizingChild = null;
            }
        }
        // Handle ongoing drag
        else if (_draggingChild != null)
        {
            if (leftMouseDown)
            {
                // Update position while dragging
                Vector2 newPos = sceneMousePos - _dragOffset;

                if (_draggingChild.UsePixelPositioning)
                {
                    _draggingChild.Position = new Point((int)newPos.X, (int)newPos.Y);
                }
                else
                {
                    // Convert back to cell coordinates using effective font size
                    Point fontSize = _draggingChild.GetEffectiveFontSize();
                    int cellX = (int)(newPos.X / fontSize.X);
                    int cellY = (int)(newPos.Y / fontSize.Y);
                    _draggingChild.Position = new Point(cellX, cellY);
                }

                // Sync with document's surface position
                _draggingChild.Document.EditingSurface.Position = _draggingChild.Position;
            }
            else
            {
                // Mouse released, stop dragging
                _draggingChild = null;
            }
        }
        // Check for new interactions
        else if (leftMouseClicked || rightMouseClicked)
        {
            // Check if we clicked on a child's resize grip or body (iterate in reverse for top-most first)
            for (int i = ChildSceneItems.Count - 1; i >= 0; i--)
            {
                var child = ChildSceneItems[i];
                if (!child.HasValidTexture || !child.IsVisible)
                    continue;

                Vector2 childPixelPos = GetChildPixelPosition(child);
                Vector2 childSize = child.SceneTextureSize;
                Vector2 childMax = childPixelPos + childSize;

                // Check if mouse is on the resize grip (bottom-right corner)
                Vector2 gripMin = new Vector2(childMax.X - ResizeGripSize, childMax.Y - ResizeGripSize);
                bool isOnGrip = sceneMousePos.X >= gripMin.X && sceneMousePos.X < childMax.X &&
                               sceneMousePos.Y >= gripMin.Y && sceneMousePos.Y < childMax.Y;

                // Check if mouse is within child bounds
                bool isOnChild = sceneMousePos.X >= childPixelPos.X && sceneMousePos.X < childMax.X &&
                                sceneMousePos.Y >= childPixelPos.Y && sceneMousePos.Y < childMax.Y;

                if (isOnChild)
                {
                    bool isSceneChild = child.Document is DocumentScene;

                    // Right-click on child: start panning viewport (not for scene documents)
                    if (rightMouseClicked && !isSceneChild)
                    {
                        _panningChild = child;
                        _panStartMousePos = sceneMousePos;
                        SelectedChildIndex = i;

                        // Initialize viewport if not set
                        if (!child.Viewport.HasValue)
                        {
                            child.Viewport = new Rectangle(0, 0,
                                child.Document.EditingSurface.Surface.Width,
                                child.Document.EditingSurface.Surface.Height);
                        }
                        _panStartViewportPos = child.Viewport.Value.Position;
                        break;
                    }
                    // Left-click on resize grip: start resizing (not for scene documents)
                    else if (leftMouseClicked && isOnGrip && !isSceneChild)
                    {
                        _resizingChild = child;
                        _resizeStartMousePos = sceneMousePos;
                        SelectedChildIndex = i;

                        // Initialize viewport if not set, and store starting size
                        if (!child.Viewport.HasValue)
                        {
                            child.Viewport = new Rectangle(0, 0,
                                child.Document.EditingSurface.Surface.Width,
                                child.Document.EditingSurface.Surface.Height);
                        }
                        _resizeStartSize = new Vector2(child.Viewport.Value.Width, child.Viewport.Value.Height);
                        break;
                    }
                    // Left-click on child body: start dragging
                    else if (leftMouseClicked)
                    {
                        _draggingChild = child;
                        _dragOffset = sceneMousePos - childPixelPos;

                        // Also select this child in the properties list
                        SelectedChildIndex = i;
                        break;
                    }
                    // Right-click on scene child: just select it
                    else if (rightMouseClicked && isSceneChild)
                    {
                        SelectedChildIndex = i;
                        break;
                    }
                }
            }
        }

        // Track if mouse was down this frame for next frame's click detection
        _wasDragging = leftMouseDown;
        _wasPanning = rightMouseDown;
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
                    var doc = Core.State.Documents[i];
                    
                    // Can't import self
                    if (doc == this)
                        continue;

                    // Can't import a scene that contains this scene (would create circular reference)
                    if (doc is DocumentScene sceneDoc && IsDescendantOf(sceneDoc))
                        continue;

                    if (ImGui.MenuItem(doc.Title))
                    {
                        // Remove from main list
                        Core.State.Documents.Remove(doc);
                        
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
                        
                        // Remove from scene using helper method that clears Parent and disposes texture
                        RemoveChildDocument(child.Document);
                        
                        // Add to main list
                        Core.State.Documents.Add(child.Document);
                        
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
        // Ensure placeholder is valid
        _placeholderSurface.Render(Game.Instance.UpdateFrameDelta);

        // Refresh all child document textures for scene rendering
        foreach (var child in ChildSceneItems)
        {
            child.RefreshTexture();
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
