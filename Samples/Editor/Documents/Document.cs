using System.Numerics;
using System.Runtime.Serialization;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

[DataContract]
[JsonObject(memberSerialization: MemberSerialization.OptIn)]
public abstract partial class Document : ITitle
{
    protected string _uniqueIdentifier = GenerateCharacterId();
    protected RenderTarget2D? _displayTexture;

    [DataMember]
    public string Title { get; set; } = GenerateName("Document");

    public ImTextureID VisualTextureId;
    public Vector2 VisualTextureSize;

    public LayeredScreenSurface VisualTool;
    public CellSurface VisualLayerToolUpper;
    public CellSurface VisualLayerToolLower;

    [DataMember]
    public ScreenSurface EditingSurface;

    [DataMember]
    [JsonConverter(typeof(SerializedTypes.FontJsonConverter))]
    public IFont EditingSurfaceFont;

    [DataMember]
    public Point EditingSurfaceFontSize;

    [DataMember]
    public Point EditorFontSize;

    public bool IsDirty => EditingSurface.IsDirty || VisualTool.IsDirty;

    public ITool[] Tools = [new Info(), new Pencil(), new Empty(), new Recolor(), new Text(), new Line(), new Box(), new Circle(), new Fill(), new Selection(), new Operations()];


    protected ImGuiGuardedValue<int> _width;
    protected ImGuiGuardedValue<int> _height;

    public bool HasPalette = false;
    public EditorPalette Palette = new();

    protected Document()
    {
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        VisualTool = new LayeredScreenSurface(1, 1);
        VisualLayerToolLower = (CellSurface)VisualTool.Layers[0];
        VisualLayerToolUpper = VisualTool.Layers.Create();
    }

    public virtual void OnSelected()
    {
        Core.State.Tools.Objects.Clear();
        
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
        _width = new(EditingSurface.Width);
        _height = new(EditingSurface.Height);
    }

    public virtual void ImGuiDraw(ImGuiRenderer renderer) { }

    public void Redraw(bool redrawSurface, bool redrawTooling)
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
        EditingSurface.ForceRendererRefresh = redrawSurface;
        EditingSurface.Render(Game.Instance.UpdateFrameDelta);

        // Check for tooling layer changes
        if (VisualTool.Width != EditingSurface.ViewWidth || VisualTool.Height != EditingSurface.ViewHeight)
        {
            VisualTool.Resize(EditingSurface.ViewWidth, EditingSurface.ViewHeight, true);
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
}
