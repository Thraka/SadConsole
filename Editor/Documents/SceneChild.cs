using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

/// <summary>
/// Represents a child document within a scene, including its position and metadata.
/// </summary>
public class SceneChild : ITitle, IDisposable
{
    private RenderTarget2D? _sceneTexture;
    private bool _disposed;

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

    /// <summary>
    /// The viewport rectangle defining which portion of the document to render.
    /// If null, the entire document surface is rendered.
    /// </summary>
    public Rectangle? Viewport { get; set; }

    /// <summary>
    /// The ImGui texture ID for this child's scene rendering.
    /// </summary>
    public ImTextureID SceneTextureId { get; private set; }

    /// <summary>
    /// The size of the scene texture in pixels.
    /// </summary>
    public Vector2 SceneTextureSize { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this child has a valid scene texture.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_sceneTexture))]
    public bool HasValidTexture => _sceneTexture != null && !_sceneTexture.IsDisposed && SceneTextureId != IntPtr.Zero;

    public SceneChild(Document document)
    {
        Document = document;
        Position = Point.Zero;
        UsePixelPositioning = false;
        Viewport = null; // Default to full surface
    }

    /// <summary>
    /// Refreshes the scene texture by rendering the document's surface.
    /// Uses the Viewport if set, otherwise renders the full surface.
    /// </summary>
    public void RefreshTexture()
    {
        var surface = Document.EditingSurface;
        var fontSizePoint = Document.EditingSurfaceFontSize;

        // Determine the area to render
        int renderWidth, renderHeight;
        Point viewPosition;

        if (Viewport.HasValue)
        {
            // Use specified viewport
            renderWidth = Viewport.Value.Width;
            renderHeight = Viewport.Value.Height;
            viewPosition = Viewport.Value.Position;
        }
        else
        {
            // Render full surface
            renderWidth = surface.Surface.Width;
            renderHeight = surface.Surface.Height;
            viewPosition = Point.Zero;
        }

        // Calculate pixel dimensions
        int pixelWidth = renderWidth * fontSizePoint.X;
        int pixelHeight = renderHeight * fontSizePoint.Y;

        if (pixelWidth <= 0 || pixelHeight <= 0)
            return;

        // Store original view settings
        Point originalViewPosition = surface.Surface.ViewPosition;
        int originalViewWidth = surface.Surface.ViewWidth;
        int originalViewHeight = surface.Surface.ViewHeight;

        // Set view to render the desired area
        surface.Surface.ViewWidth = renderWidth;
        surface.Surface.ViewHeight = renderHeight;
        surface.Surface.ViewPosition = viewPosition;

        // Force render the surface
        surface.ForceRendererRefresh = true;
        surface.Render(Game.Instance.UpdateFrameDelta);

        // Create or resize the render target if needed
        if (_sceneTexture == null || _sceneTexture.IsDisposed || 
            _sceneTexture.Width != pixelWidth || _sceneTexture.Height != pixelHeight)
        {
            _sceneTexture?.Dispose();
            _sceneTexture = new RenderTarget2D(
                Host.Global.GraphicsDevice, 
                pixelWidth, 
                pixelHeight, 
                false, 
                Host.Global.GraphicsDevice.DisplayMode.Format, 
                DepthFormat.Depth24, 
                0, 
                RenderTargetUsage.DiscardContents);

            // Bind or rebind to ImGui
            if (SceneTextureId == IntPtr.Zero)
                SceneTextureId = ImGuiCore.Renderer.BindTexture(_sceneTexture);
            else
                ImGuiCore.Renderer.ReplaceBoundTexture(SceneTextureId, _sceneTexture);
        }

        SceneTextureSize = new Vector2(pixelWidth, pixelHeight);

        // Render to our texture
        Host.Global.GraphicsDevice.SetRenderTarget(_sceneTexture);
        Host.Global.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
        Host.Global.SharedSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

        // Draw the surface renderer output
        if (surface.Renderer?.Output != null)
        {
            Host.Global.SharedSpriteBatch.Draw(
                ((Host.GameTexture)surface.Renderer.Output).Texture, 
                Microsoft.Xna.Framework.Vector2.Zero, 
                Microsoft.Xna.Framework.Color.White);
        }

        Host.Global.SharedSpriteBatch.End();
        Host.Global.GraphicsDevice.SetRenderTarget(null);

        // Restore original view settings
        surface.Surface.ViewWidth = originalViewWidth;
        surface.Surface.ViewHeight = originalViewHeight;
        surface.Surface.ViewPosition = originalViewPosition;

        // Re-render with original settings so the document's normal texture is correct
        surface.ForceRendererRefresh = true;
        surface.Render(Game.Instance.UpdateFrameDelta);
    }

    public override bool Equals(object obj)
    {
        if (obj is SceneChild other)
        {
            return Document.Equals(other.Document) &&
                   Position.Equals(other.Position) &&
                   UsePixelPositioning == other.UsePixelPositioning &&
                   Label == other.Label &&
                   EqualityComparer<Rectangle?>.Default.Equals(Viewport, other.Viewport);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Document, Position, UsePixelPositioning, Label, Viewport);
    }

    public override string ToString() => Title;

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unbind from ImGui if bound
                if (SceneTextureId != IntPtr.Zero)
                {
                    ImGuiCore.Renderer.UnbindTexture(SceneTextureId);
                    SceneTextureId = default;
                }

                // Dispose the render target
                _sceneTexture?.Dispose();
                _sceneTexture = null;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
