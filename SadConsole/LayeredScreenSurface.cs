using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// Represents a layered surface.
    /// </summary>
    public partial class LayeredScreenSurface : ScreenSurface
    {
        /// <summary>
        /// When <see cref="RenderClipped"/> is <see langword="true"/>, the renderer will create a texture of this width to draw all layers.
        /// </summary>
        public int RenderClippedWidth { get; }

        /// <summary>
        /// When <see cref="RenderClipped"/> is <see langword="true"/>, the renderer will create a texture of this height to draw all layers.
        /// </summary>
        public int RenderClippedHeight { get; }

        /// <summary>
        /// When <see langword="true"/>, each layer is rendered within the limits of first layer. When <see langword="false"/>, each layer is rendered individually to its own bounds.
        /// </summary>
        public bool RenderClipped { get; }

        /// <summary>
        /// The layers of this surface.
        /// </summary>
        public ScreenObjectCollection<Layer> Layers { get; }

        /// <summary>
        /// Creates a new layered screen surface with an initial layer.
        /// </summary>
        /// <param name="layer">The initial layer.</param>
        /// <param name="renderClipped">When <see langword="true"/>, each layer is rendered within the limits of first layer. When <see langword="false"/>, each layer is rendered individually to its own bounds.</param>
        /// <param name="font">The font to use with the surface.</param>
        /// <param name="fontSize">The font size.</param>
        public LayeredScreenSurface(Layer layer, bool renderClipped) : base(new CellSurface(1, 1))
        {
            RenderClipped = renderClipped;
            RenderClippedWidth = layer.AbsoluteArea.Width;
            RenderClippedHeight = layer.AbsoluteArea.Height;
            Layers = new ScreenObjectCollection<Layer>(this);
            Layers.Add(layer);
            Renderer = GameHost.Instance.GetRenderer("layered");
            SetActiveLayer(0);
        }

        /// <summary>
        /// Creates a new layered screen surface with the specified layers.
        /// </summary>
        /// <param name="layers">A collection of layers.</param>
        /// <param name="renderClipped">When <see langword="true"/>, each layer is rendered within the limits of first layer. When <see langword="false"/>, each layer is rendered individually to its own bounds.</param>
        /// <param name="font">The font to use with the surface.</param>
        /// <param name="fontSize">The font size.</param>
        public LayeredScreenSurface(IEnumerable<Layer> layers, bool renderClipped) : base(new CellSurface(1, 1))
        {
            RenderClipped = renderClipped;
            
            Layers = new ScreenObjectCollection<Layer>(this);
            foreach (var item in layers)
                Layers.Add(item);
            RenderClippedWidth = Layers[0].AbsoluteArea.Width;
            RenderClippedHeight = Layers[0].AbsoluteArea.Height;
            Renderer = GameHost.Instance.GetRenderer("layered");
            SetActiveLayer(0);
        }

        [JsonConstructor]
        private LayeredScreenSurface(IEnumerable<Layer> layers, bool renderClipped, int renderClippedWidth, int renderClippedHeight) : base(new CellSurface(1, 1))
        {
            Layers = new ScreenObjectCollection<Layer>(this);
            RenderClipped = renderClipped;
            RenderClippedWidth = renderClippedWidth;
            RenderClippedHeight = renderClippedHeight;

            foreach (var item in layers)
                Layers.Add(item);

            SetActiveLayer(0);
        }

        /// <summary>
        /// Sets a layer as the <see cref="ScreenSurface.Surface"/> used by this object.
        /// </summary>
        /// <param name="index">The index of the layer in the <see cref="Layers"/> collection.</param>
        /// <remarks>This object always points to a specific layer through the <see cref="ScreenSurface.Surface"/> property. This object doesn't have it's own surface.</remarks>
        public void SetActiveLayer(int index)
        {
            if (index < 0 || index > Layers.Count - 1) throw new ArgumentOutOfRangeException(nameof(index));
            
            Surface = Layers[index].Surface;
            Font = Layers[index].Font;
            FontSize = Layers[index].FontSize;
        }

        /// <summary>
        /// Sets a layer as the <see cref="ScreenSurface.Surface"/> used by this object.
        /// </summary>
        /// <param name="layer">The layer in the <see cref="Layers"/> collection.</param>
        /// <remarks>This object always points to a specific layer through the <see cref="ScreenSurface.Surface"/> property. This object doesn't have it's own surface.</remarks>
        public void SetActiveLayer(Layer layer)
        {
            if (!Layers.Contains(layer)) throw new Exception("The specified layer doesn't exist in this surface.");

            Surface = layer.Surface;
            Font = layer.Font;
            FontSize = layer.FontSize;
        }

        /// <summary>
        /// Draws the <see cref="Layers"/>.
        /// </summary>
        /// <param name="delta"></param>
        public override void Draw(TimeSpan delta)
        {
            if (!IsVisible) return;

            foreach (var item in Layers)
                item.Draw(delta);

            if (Renderer != null)
            {
                Renderer.Refresh(this, ForceRendererRefresh);
                Renderer.Render(this);
                ForceRendererRefresh = false;
            }

            foreach (Components.IComponent component in ComponentsDraw.ToArray())
                component.Draw(this, delta);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Draw(delta);
        }
    }
}
