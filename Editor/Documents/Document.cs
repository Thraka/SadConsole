using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Serialization;
using SadConsole.Editor.Tools;
using SadConsole.Editor.Windows;
using SadConsole.Entities;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

// TODO:
// -------------------
// Move a lot of the common UI stuff like FontSize selection popups and windows
// into base class methods that render components and handle showing windows/popups
// when they're closed, call to new base methods like "FontChanged" so that the document
// can react as needed, like animations set it on the base animation object and surfaces
// set font on surface

[DataContract]
[JsonObject(memberSerialization: MemberSerialization.OptIn)]
public abstract partial class Document : ITitle, IHierarchicalItem<Document>
{
    protected string _uniqueIdentifier = GenerateCharacterId();
    protected RenderTarget2D? _displayTexture;

    [DataMember]
    public string Title { get; set; } = GenerateName("Document");

    public DocumentOptions Options = new();

    public ImTextureID VisualTextureId;
    public Vector2 VisualTextureSize;

    public LayeredScreenSurface VisualTool;
    public CellSurface VisualLayerToolUpper;
    public CellSurface VisualLayerToolMiddle;
    public CellSurface VisualLayerToolLower;

    public ImGuiList<ToolMode> ToolModes = new(ToolMode.DrawMode,
                                               ToolMode.EmptyMode,
                                               ToolMode.ObjectsMode,
                                               ToolMode.ZonesMode);

    [DataMember]
    public Dictionary<string, string> Metadata = new();

    [DataMember]
    public IScreenSurface EditingSurface;

    [DataMember]
    [JsonConverter(typeof(SerializedTypes.FontJsonConverter))]
    public IFont EditingSurfaceFont;

    [DataMember]
    public Point EditingSurfaceFontSize;

    [DataMember]
    public Point EditorFontSize;

    public bool IsDirty => EditingSurface.IsDirty || VisualTool.IsDirty;

    public ITool[] Tools = [new Info(), new Pencil(), new Recolor(), new Text(), new Line(), new LineDraw(), new Box(), new Circle(), new Fill(), new Selection(), new Operations()];


    protected ImGuiGuardedValue<int> _width;
    protected ImGuiGuardedValue<int> _height;

    public bool HasPalette = false;
    public EditorPalette Palette = new();

    protected FontSelectionWindow? FontSelectionWindow;

    /// <summary>
    /// Gets the icon to display for this document type in the hierarchy.
    /// Override in derived classes to provide a custom icon.
    /// </summary>
    public virtual string DocumentIcon => "\uf15b"; // Default: file icon

    protected Document()
    {
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        VisualTool = new LayeredScreenSurface(1, 1);
        VisualLayerToolLower = (CellSurface)VisualTool.Layers[0];
        VisualLayerToolMiddle = VisualTool.Layers.Create();
        VisualLayerToolUpper = VisualTool.Layers.Create();
        SyncToolModes();
    }

    [MemberNotNullWhen(true, nameof(FontSelectionWindow))]
    protected void FontSelectionWindow_Popup()
    {
        FontSelectionWindow = new FontSelectionWindow(EditingSurfaceFont, EditingSurfaceFontSize);
        FontSelectionWindow.IsOpen = true;
    }

    [MemberNotNullWhen(true, nameof(FontSelectionWindow))]
    protected bool FontSelectionWindow_BuildUI(ImGuiRenderer renderer)
    {
        if (FontSelectionWindow != null && FontSelectionWindow.IsOpen)
        {
            FontSelectionWindow.BuildUI(renderer);

            if (!FontSelectionWindow.IsOpen)
            {
                return FontSelectionWindow.DialogResult;
            }
        }

        return false;
    }

    protected void FontSelectionWindow_Reset() =>
        FontSelectionWindow = null;

    

    public virtual void OnSelected()
    {
        Core.State.Tools.Objects.Clear();

        // Sync the layers
        VisualTool.Font = EditingSurfaceFont;
        VisualTool.FontSize = EditingSurfaceFontSize;

        foreach (var tool in Tools)
            Core.State.Tools.Objects.Add(tool);

        Core.State.SyncEditorPalette();
    }

    public virtual void OnDeselected()
    {
        if (Core.State.Tools.IsItemSelected())
            Core.State.Tools.SelectedItem.OnDeselected(this);
        
        Core.State.Tools.Objects.Clear();
    }

    public virtual void Resync()
    {
        _width = new(EditingSurface.Surface.Width);
        _height = new(EditingSurface.Surface.Height);
    }

    public void SyncToolModes()
    {
        ToolMode.Modes previousMode = ToolModes.SelectedItem?.Mode ?? ToolMode.Modes.Draw;

        List<ToolMode> availableModes = [ToolMode.DrawMode, ToolMode.EmptyMode];

        if (Options.UseSimpleObjects)
            availableModes.Add(ToolMode.ObjectsMode);

        if (Options.UseZones)
            availableModes.Add(ToolMode.ZonesMode);

        ToolModes = new([.. availableModes]);

        // Restore previous mode if possible
        ToolModes.SelectedItemIndex = availableModes.FindIndex(mode => mode.Mode == previousMode);

        if (!ToolModes.IsItemSelected())
            ToolModes.SelectedItemIndex = 0;
    }

    public void ResetVisualLayers()
    {
        VisualLayerToolLower.DefaultForeground = Color.White;
        VisualLayerToolLower.DefaultBackground = Color.Transparent;
        VisualLayerToolLower.Clear();
        VisualLayerToolMiddle.DefaultForeground = Color.White;
        VisualLayerToolMiddle.DefaultBackground = Color.Transparent;
        VisualLayerToolMiddle.Clear();
        VisualLayerToolUpper.DefaultForeground = Color.White;
        VisualLayerToolUpper.DefaultBackground = Color.Transparent;
        VisualLayerToolUpper.Clear();
    }

    public virtual void SetSurfaceView(int x, int y, int width, int height)
    {
        EditingSurface.Surface.ViewPosition = new Point(x, y);
        EditingSurface.Surface.ViewWidth = width;
        EditingSurface.Surface.ViewHeight = height;
    }

    /// <summary>
    /// If <see cref="DocumentOptions.DrawSelf"/> is true, this method is called when rendering a document to the Document tab.
    /// </summary>
    /// <param name="renderer">The ImGuiRenderer instance used to draw ImGui controls. Cannot be null.</param>
    public virtual void ImGuiDrawSelf(ImGuiRenderer renderer) { }

    /// <summary>
    /// Performs custom ImGui rendering operations on the document tab after the main surface has been drawn.
    /// </summary>
    /// <remarks>Override this method to add additional ImGui UI elements or overlays that should appear on
    /// top of the main surface rendering. This method is called after the primary surface draw pass is
    /// complete.</remarks>
    /// <param name="renderer">The ImGuiRenderer instance used to issue ImGui draw commands.</param>
    public virtual void ImGuiDrawSurfaceTextureAfter(ImGuiRenderer renderer, Point hoveredCellPosition, bool isHovered, bool isActive) { }

    /// <summary>
    /// Performs custom ImGui rendering after all other editor components have been drawn.
    /// </summary>
    /// <remarks>Override this method to inject additional ImGui UI elements or overlays.</remarks>
    /// <param name="renderer">The ImGuiRenderer instance used to issue ImGui draw commands.</param>
    public virtual void ImGuiDrawAfterEverything(ImGuiRenderer renderer) { }

    /// <summary>
    /// Renders a main menu item after the File item. This is called during ImGui.BeginMainMenuBar.
    /// </summary>
    /// <remarks>Override this method to customize the appearance or behavior of the top bar in the ImGui
    /// interface.</remarks>
    /// <param name="renderer">The ImGuiRenderer instance used to draw the top bar components. Cannot be null.</param>
    public virtual void ImGuiDrawTopBar(ImGuiRenderer renderer) { }

    /// <summary>
    /// Draws custom ImGui controls in the tools panel using the specified renderer.
    /// </summary>
    /// <remarks>Override this method to add custom controls or UI elements to the tools panel. This method is
    /// typically called during the tools panel rendering phase.</remarks>
    /// <param name="renderer">The ImGuiRenderer instance used to render ImGui controls in the tools panel. Cannot be null.</param>
    public virtual void ImGuiDrawInToolsPanel(ImGuiRenderer renderer) { }

    /// <summary>
    /// Refreshes the display and tooling layers by re-rendering the editing surface and associated visual tools.
    /// </summary>
    /// <remarks>
    /// Call this method when changes to the editing surface or tooling require the display to be
    /// updated. This method manages texture resources and ensures that both the main surface and tooling layers are
    /// rendered with the latest state. Frequent calls may impact performance if large textures are involved.
    /// <see cref="GuiObjects.GuiFinalDrawDocument"/> automatically calls this method as part of its rendering process.
    /// </remarks>
    /// <param name="redrawSurface">Indicates whether the editing surface should be forced to refresh its renderer before rendering. Set to <see
    /// langword="true"/> to ensure the surface is fully redrawn.</param>
    /// <param name="redrawTooling">Indicates whether the tooling layer should be forced to refresh its renderer before rendering. Set to <see
    /// langword="true"/> to ensure the tooling visuals are fully redrawn.</param>
    public virtual void Redraw(bool redrawSurface, bool redrawTooling)
    {
        if (_displayTexture == null || EditingSurface.WidthPixels != _displayTexture.Width || EditingSurface.HeightPixels != _displayTexture.Height)
        {
            _displayTexture?.Dispose();
            _displayTexture = new RenderTarget2D(Host.Global.GraphicsDevice, EditingSurface.WidthPixels, EditingSurface.HeightPixels, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            if (VisualTextureId == IntPtr.Zero)
                VisualTextureId = ImGuiCore.Renderer.BindTexture(_displayTexture);
            else
                ImGuiCore.Renderer.ReplaceBoundTexture(VisualTextureId, _displayTexture);
        }

        VisualTextureSize = new Vector2(_displayTexture.Bounds.Width, _displayTexture.Bounds.Height);

        // Redraw the objects before projecting
        EditingSurface.Update(TimeSpan.Zero);
        EditingSurface.ForceRendererRefresh = redrawSurface;
        EditingSurface.Render(Game.Instance.UpdateFrameDelta);

        // Check for tooling layer changes
        if (VisualTool.Width != EditingSurface.Surface.ViewWidth || VisualTool.Height != EditingSurface.Surface.ViewHeight)
        {
            VisualTool.Resize(EditingSurface.Surface.ViewWidth, EditingSurface.Surface.ViewHeight, true);
        }

        VisualTool.Update(Game.Instance.UpdateFrameDelta);
        VisualTool.ForceRendererRefresh = redrawTooling;
        VisualTool.Render(Game.Instance.UpdateFrameDelta);

        // Refresh the texture
        Host.Global.GraphicsDevice.SetRenderTarget(_displayTexture);
        Host.Global.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
        Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

        // Compose each target
        Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)EditingSurface.Renderer!.Output).Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
        Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)VisualTool.Renderer!.Output).Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

        // End sprite batch
        Host.Global.SharedSpriteBatch.End();
        Host.Global.GraphicsDevice.SetRenderTarget(null);
    }

    public abstract void BuildUiDocumentSettings(ImGuiRenderer renderer);

    public abstract IEnumerable<IFileHandler> GetSaveHandlers();

    public bool TryLoadPalette(string file)
    {
        Palette = new EditorPalette();
        HasPalette = false;

        if (File.Exists(file))
        {
            Palette = EditorPalette.Load(file);
            HasPalette = true;
        }

        return HasPalette;
    }

    public void SavePalette(string file)
    {
        HasPalette = true;
        Palette.Save(file);
    }

    public static string GenerateName(string prefix) =>
        $"{prefix}|{GenerateCharacterId()}";

    protected static string GenerateCharacterId()
    {
        char[] characters = new char[6];
        foreach (int index in Enumerable.Range(1, 6))
        {
            characters[index - 1] = (char)Random.Shared.Next((int)'a', ((int)'z') + 1);
        }
        return new string(characters);
    }

    public override string ToString() =>
        Title;

    protected void ImGuiDrawMetadataSettings()
    {
        ImGui.SeparatorText("Metadata"u8);
        if (ImGui.Button("Edit"u8))
        {
            var window = new Windows.KeyValuePairEditor(Metadata);
            window.Closed += (windowObj, _) =>
            {
                if (((ImGuiWindowBase)windowObj).DialogResult)
                    Metadata = window.ToDictionary();
            };
            window.Open();
        }

        if (Metadata.Count > 0)
        {
            // Help wrap the table so that it can stretch and still show all rows plus the scroll bars
            if (ImGui.BeginChild("##doc_metadata_child", new Vector2(-1, ImGui.GetFrameHeight() * Metadata.Count + ImGui.GetStyle().FramePadding.Y * 2)))
            {
                if (ImGui.BeginTable("zone_settings_table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollX))
                {
                    ImGui.TableSetupColumn("Key"u8);
                    ImGui.TableSetupColumn("Value"u8);
                    ImGui.TableHeadersRow();

                    foreach (var kvp in Metadata)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(kvp.Key);
                        ImGui.TableNextColumn();
                        ImGui.Text(kvp.Value);
                    }

                    ImGui.EndTable();
                }
            }
            ImGui.EndChild();
        }
        else
            ImGui.Text("No metadata defined."u8);
    }

    #region IHierarchicalItem Implementation

    /// <summary>
    /// Gets or sets the parent document, or <see langword="null"/> if this is a root document.
    /// </summary>
    public Document? Parent { get; set; }

    /// <summary>
    /// Gets the child documents of this document.
    /// Override in derived classes that support children.
    /// </summary>
    public virtual IReadOnlyList<Document> Children => [];

    /// <summary>
    /// Gets a value indicating whether this document can contain children.
    /// Override in derived classes that support children.
    /// </summary>
    public virtual bool CanHaveChildren => false;

    /// <summary>
    /// Gets a value indicating whether children should be displayed in the hierarchy UI.
    /// Override in derived classes to control visibility.
    /// </summary>
    public virtual bool ShowChildrenInHierarchy => false;

    /// <summary>
    /// Gets or sets a value indicating whether this document's children are expanded in the UI.
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Gets the depth of this document in the hierarchy (0 for root documents).
    /// </summary>
    public int Depth => Parent == null ? 0 : Parent.Depth + 1;

    #endregion
}
