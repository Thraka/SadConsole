using System.Collections;
using System.Numerics;
using System.Runtime.Serialization;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Components;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Tools;
using SadConsole.Effects;
using SadConsole.ImGuiSystem;
using SadConsole.Renderers;

namespace SadConsole.Editor.Model;

public abstract class Document
{
    private static int _id;
    private string _uniqueIdentifier = GenerateCharacterId();

    //TODO: Kill this off and replace with CompositeRenderer
    // Do I need to remove IRenderer and just make a single
    // class used for this? Maybe just a method... I mean
    // this is really just composing VisualDoc + two Overlays...
    private BoundTexture2D? _surfaceDisplayTexture;
    private BoundTexture2D? _overlayDisplayTexture;

    public RenderTarget2D _displayTexture;

    public DocumentOptions Options = new DocumentOptions();

    public string UniqueIdentifier => _uniqueIdentifier;

    public string Name = GenerateName("Document");

    public DocumentTypes DocumentType;

    public ScreenObject VisualContainer;
    [DataMember]
    public IScreenSurface VisualDocument;
    public LayeredScreenSurface VisualToolContainer;
    public CellSurface VisualToolLayerLower;
    public CellSurface VisualToolLayerUpper;

    protected void ComposeVisual()
    {
        VisualToolContainer ??= new(VisualDocument.Surface.ViewWidth, VisualDocument.Surface.ViewHeight);

        VisualToolLayerLower = (CellSurface)VisualToolContainer.Layers.First();
        VisualToolLayerUpper ??= new(VisualDocument.Surface.ViewWidth, VisualDocument.Surface.ViewHeight);

        VisualToolContainer.Layers.Add(VisualToolLayerUpper);

        MatchVisualToolContainer();

        VisualContainer ??= new();
        VisualContainer.Children.Clear();
        VisualContainer.Children.Add(VisualDocument);
        VisualContainer.Children.Add(VisualToolContainer);
    }

    protected void MatchVisualToolContainer()
    {
        VisualToolLayerLower.Resize(VisualDocument.Surface.ViewWidth, VisualDocument.Surface.ViewHeight, false);
        VisualToolLayerUpper.Resize(VisualDocument.Surface.ViewWidth, VisualDocument.Surface.ViewHeight, false);
    }

    public virtual void OnShow(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = VisualContainer;
        SadConsole.Game.Instance.Screen.Update(TimeSpan.Zero);
        SadConsole.Game.Instance.Screen.Render(TimeSpan.Zero);
    }

    public virtual void OnHide(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = null;
    }

    public abstract void Create();

    public abstract void BuildUIEdit(ImGuiRenderer renderer, bool readOnly);

    public abstract void BuildUINew(ImGuiRenderer renderer);

    public abstract void BuildUIDocument(ImGuiRenderer renderer);

    public abstract IEnumerable<IFileHandler> GetLoadHandlers();

    public abstract IEnumerable<IFileHandler> GetSaveHandlers();

    public abstract bool HydrateFromFileHandler(IFileHandler handler, string file);

    public abstract object DehydrateToFileHandler(IFileHandler handler, string file);

    protected static string GenerateName(string prefix) =>
        $"{prefix}|{GenerateCharacterId()}";

    protected static string GenerateCharacterId()
    {
        char[] characters = new char[6];
        foreach (var index in Enumerable.Range(1, 6))
        {
            characters[index - 1] = (char)Random.Shared.Next((int)'a', ((int)'z') + 1);
        }
        return new string(characters);
    }

    protected void BuildUIDocumentStandard(ImGuiRenderer renderer)
    {
        MatchVisualToolContainer();

        // Figure out the ImGui size
        Vector2 topLeft = ImGui.GetCursorPos();
        Vector2 region = ImGui.GetContentRegionAvail();
        Vector2 imageSize = new(this.VisualDocument.Renderer!.Output.Width, this.VisualDocument.Renderer.Output.Height);
        int barSize = 15;
        Vector2 padding = ImGui.GetStyle().FramePadding;
        bool bigX = false;

        int newViewWidth = (int)(region.X - barSize - (padding.X * 2)) / this.VisualDocument.FontSize.X;
        int newViewHeight = (int)(region.Y - barSize - 2 - (padding.Y * 2)) / this.VisualDocument.FontSize.Y; // minus 2 is here because of button height

        newViewWidth = Math.Max(newViewWidth, 1);
        newViewHeight = Math.Max(newViewHeight, 1);

        if (this.VisualDocument.Surface.Width < newViewWidth && VisualDocument.Surface.Width != this.VisualDocument.Surface.ViewWidth)
            VisualDocument.Surface.ViewWidth = this.VisualDocument.Surface.Width;
        else if (this.VisualDocument.Surface.Width > newViewWidth)
            this.VisualDocument.Surface.ViewWidth = newViewWidth;

        if (VisualDocument.Surface.Height < newViewHeight && VisualDocument.Surface.Height != this.VisualDocument.Surface.ViewHeight)
            VisualDocument.Surface.ViewHeight = this.VisualDocument.Surface.Height;
        else if (this.VisualDocument.Surface.Height > newViewHeight)
            this.VisualDocument.Surface.ViewHeight = newViewHeight;

        // Print stats 
        ImGuiCore.GuiTopBar.StatusItems.Add((Vector4.Zero, "| ViewPort:"));
        ImGuiCore.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), this.VisualDocument.Surface.View.ToString()));

        // Force overlay to update and match surface
        //VisualToolLayerLower?.Update(VisualDocument, TimeSpan.Zero);
        //VisualToolLayerUpper?.Update(VisualDocument, TimeSpan.Zero);

        //Host.Global.GraphicsDevice.SetRenderTarget(((Renderers.ScreenSurfaceRenderer)VisualDocument.Renderer)._backingTexture);
        //Host.Global.SharedSpriteBatch.Begin();
        //Host.Global.SharedSpriteBatch.End();
        //Host.Global.GraphicsDevice.SetRenderTarget(null);


        ImGui.BeginChild("doc_surface");
        {
            //// Refresh the screen
            //VisualContainer.Update(TimeSpan.Zero);
            //VisualContainer.Render(TimeSpan.Zero);

            // (re)create texture if required
            if (_displayTexture == null || VisualDocument.WidthPixels != _displayTexture.Width || VisualDocument.HeightPixels != _displayTexture.Height)
            {
                _displayTexture?.Dispose();
                _displayTexture = new(Host.Global.GraphicsDevice, VisualDocument.WidthPixels, VisualDocument.HeightPixels, false, Host.Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

                // Capture the texture of the surface
                _surfaceDisplayTexture ??= new(renderer, _displayTexture);
                _surfaceDisplayTexture.Refresh(renderer, _displayTexture);
            }

            // Refresh the texture
            Host.Global.GraphicsDevice.SetRenderTarget(_displayTexture);
            Host.Global.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
            Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

            // Compose each target
            Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)VisualDocument.Renderer.Output).Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)VisualToolContainer.Renderer.Output).Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

            // End sprite batch
            Host.Global.SharedSpriteBatch.End();
            Host.Global.GraphicsDevice.SetRenderTarget(null);

            // Draw the surface
            ImGuiExt.DrawTextureChild("output_preview_surface1", true, ImGuiExt.ZoomNormal, _surfaceDisplayTexture.Texture, imageSize, renderer, out bool isActive, out bool isHovered);

            // Get the selected tool (if any)
            ITool? tool = ((IDocumentTools)this).State.SelectedTool as ITool;

            // Work with the selected tool
            if (tool is not null)
            {
                tool.DrawOverDocument(this, renderer);

                Vector2 mousePosition = ImGui.GetMousePos();
                Vector2 pos = mousePosition - ImGui.GetItemRectMin();
                if (this.VisualDocument.AbsoluteArea.WithPosition((0, 0)).Contains(new Point((int)pos.X, (int)pos.Y)))
                {
                    if (isHovered)
                    {
                        Point hoveredCellPosition = new Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(this.VisualDocument.FontSize) + VisualDocument.Surface.ViewPosition;
                        tool.MouseOver(this, hoveredCellPosition, isActive, renderer);
                        ImGuiCore.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Mouse:"));
                        ImGuiCore.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), hoveredCellPosition.ToString()));
                    }
                }
            }
            // Handle scrolling the surface if it's required
            Rectangle view = this.VisualDocument.Surface.View;

            if (view.Height != this.VisualDocument.Surface.Height)
            {
                ImGui.SameLine();

                int _sliderValueY = view.Position.Y;

                ImGui.BeginDisabled(Options.DisableScrolling);

                if (ImGuiExt.VSliderIntNudges("##height", new Vector2(barSize, imageSize.Y), ref _sliderValueY, VisualDocument.Surface.Height - view.Height, 0, ImGuiSliderFlags.AlwaysClamp))
                {
                    VisualDocument.Surface.ViewPosition = VisualDocument.Surface.ViewPosition.WithY(_sliderValueY);
                    tool?.DocumentViewChanged(this);
                }

                ImGui.EndDisabled();
            }

            if (view.Width != VisualDocument.Surface.Width)
            {
                int _sliderValueX = view.Position.X;

                ImGui.BeginDisabled(Options.DisableScrolling);

                if (ImGuiExt.SliderIntNudges("##width", (int)imageSize.X, ref _sliderValueX, 0, VisualDocument.Surface.Width - view.Width, bigX ? "BIG" : "%d", ImGuiSliderFlags.AlwaysClamp))
                {
                    VisualDocument.Surface.ViewPosition = VisualDocument.Surface.ViewPosition.WithX(_sliderValueX);
                    tool?.DocumentViewChanged(this);
                }

                ImGui.EndDisabled();
            }
        }
        ImGui.EndChild();
    }

}
