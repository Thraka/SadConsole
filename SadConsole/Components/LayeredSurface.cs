using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// Manages a set of surface layers.
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("Layered surface")]
public class LayeredSurface : Components.UpdateComponent, Components.IComponent, IList<ICellSurface>, ICellSurfaceResize
{
    /// <summary>
    /// Indicates that the entity renderer has been added to a parent object.
    /// </summary>
    [MemberNotNullWhen(true, "_screen")]
    protected bool IsAttached { get; set; }


    /// <summary>
    /// The parent screen hosting this component.
    /// </summary>
    protected IScreenSurface? _screen;

    /// <summary>
    /// A cached copy of the <see cref="ICellSurface.View"/> of the hosting screen surface.
    /// </summary>
    protected Rectangle _screenCachedView;

    /// <summary>
    /// The layers.
    /// </summary>
    [DataMember(Name = "Layers")]
    protected List<ICellSurface> _layers = new(3);

    /// <summary>
    /// Visibility state for each layer.
    /// </summary>
    [DataMember(Name = "LayerVisibility")]
    protected List<bool> _layerVisibility = new(3);

    /// <summary>
    /// Internal use only
    /// </summary>
    public Renderers.IRenderStep? RenderStep;

    /// <summary>
    /// The visible portion of the surface layers.
    /// </summary>
    public Rectangle View
    {
        get
        {
            if (_layers.Count == 0)
                throw new Exception($"At least one layer must be added to this component to get the {nameof(View)}");

            return _layers[0].View;
        }
        set
        {
            if (_layers.Count == 0)
                throw new Exception($"At least one layer must be added to this component to set the {nameof(View)}");

            foreach (ICellSurface layer in _layers)
                layer.View = value;

            if (IsAttached)
                _screen.IsDirty = true;
        }
    }

    /// <summary>
    /// The default color to clear the rendering surface. This is used instead of the individual layer's default background.
    /// </summary>
    public Color DefaultBackground { get; set; } = Color.Transparent;

    /// <summary>
    /// The numbers of layers.
    /// </summary>
    public int Count => _layers.Count;

    /// <summary>
    /// Always returns <see langword="false"/>.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the visibility of a layer at the specified index.
    /// </summary>
    /// <param name="index">The index of the layer.</param>
    /// <returns><see langword="true"/> if the layer is visible; otherwise, <see langword="false"/>.</returns>
    public bool GetLayerVisibility(int index) => _layerVisibility[index];

    /// <summary>
    /// Sets the visibility of a layer at the specified index.
    /// </summary>
    /// <param name="index">The index of the layer.</param>
    /// <param name="isVisible"><see langword="true"/> to make the layer visible; otherwise, <see langword="false"/>.</param>
    public void SetLayerVisibility(int index, bool isVisible)
    {
        _layerVisibility[index] = isVisible;

        if (IsAttached)
            _screen.IsDirty = true;
    }

    /// <summary>
    /// Gets or sets a layer by index.
    /// </summary>
    /// <param name="index">The index of the layer to get or set.</param>
    /// <returns>The layer.</returns>
    public ICellSurface this[int index]
    {
        get => _layers[index];
        set
        {
            RemoveAt(index);
            Insert(index, value);
        }
    }

    /// <summary>
    /// Adds a layer to this component.
    /// </summary>
    /// <param name="layer">The layer to add.</param>
    public void Add(ICellSurface layer)
    {
        if (!CheckLayerValidity(layer)) throw new ArgumentException("Layer is invalid, it must be the same size as the other layers.");

        if (!_layers.Contains(layer))
        {
            if (_layers.Count == 0 && _screen != null)
            {
                ((ISurfaceSettable)_screen).Surface = layer;
                _screenCachedView = layer.View;
            }
            _layers.Add(layer);
            _layerVisibility.Add(true);
            layer.View = _screenCachedView;
        }
    }

    /// <summary>
    /// Adds a collection if layers to this component.
    /// </summary>
    /// <param name="layers">The layers to add.</param>
    public void AddRange(IEnumerable<ICellSurface> layers)
    {
        foreach (ICellSurface layer in layers)
            Add(layer);
    }

    /// <summary>
    /// Replaces a layer with a new instance.
    /// </summary>
    /// <param name="oldLayer">The layer to remove.</param>
    /// <param name="newLayer">The layer to add.</param>
    /// <exception cref="ArgumentException">
    /// <para>Thrown when the old layer doesn't exist in this collection.</para>
    /// <para>- or -</para>
    /// <para>Thrown when the new layer already exists in this collection.</para>
    /// <para>- or -</para>
    /// <para>Thrown when the new layer isn't the same size as the other layers.</para>
    /// </exception>
    public void Replace(ICellSurface oldLayer, ICellSurface newLayer)
    {
        if (_layers.Contains(newLayer)) throw new ArgumentException("The new layer is already in this collection.", nameof(newLayer));
        if (!CheckLayerValidity(newLayer)) throw new ArgumentException("Layer is invalid, it must be the same size as the other layers.");

        int oldIndex = _layers.IndexOf(oldLayer);

        if (oldIndex == -1)
            throw new ArgumentException("The layer doesn't exist in this collection.", nameof(oldLayer));

        _layers.Remove(oldLayer);
        _layers.Insert(oldIndex, newLayer);
        newLayer.View = _screenCachedView;
    }

    /// <summary>
    /// Removes a layer from this component.
    /// </summary>
    /// <param name="layer">The layer to remove.</param>
    public bool Remove(ICellSurface layer)
    {
        int index = _layers.IndexOf(layer);
        if (index != -1)
        {
            _layers.RemoveAt(index);
            _layerVisibility.RemoveAt(index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the index of the specified layer.
    /// </summary>
    /// <param name="layer">The layer to search for.</param>
    /// <returns>The index of the layer.</returns>
    public int IndexOf(ICellSurface layer) =>
        _layers.IndexOf(layer);

    /// <summary>
    /// Inserts the layer at the specified index.
    /// </summary>
    /// <param name="index">The index to insert at.</param>
    /// <param name="layer">The layer to insert.</param>
    public void Insert(int index, ICellSurface layer)
    {
        if (!CheckLayerValidity(layer)) throw new ArgumentException("Layer is invalid, it must be the same size as the other layers.");

        if (!_layers.Contains(layer))
        {
            if (_layers.Count == 0 && _screen != null)
            {
                ((ISurfaceSettable)_screen).Surface = layer;
                _screenCachedView = layer.View;
            }
            _layers.Insert(index, layer);
            _layerVisibility.Insert(index, true);
            layer.View = _screenCachedView;
        }
    }

    /// <summary>
    /// Removes a layer at the specified index.
    /// </summary>
    /// <param name="index">The index of the layer to remove.</param>
    public void RemoveAt(int index)
    {
        _layers.RemoveAt(index);
        _layerVisibility.RemoveAt(index);
    }

    /// <summary>
    /// Removes all layers.
    /// </summary>
    public void Clear()
    {
        _layers.Clear();
        _layerVisibility.Clear();
    }

    /// <summary>
    /// Removes all layers and adds the <paramref name="initialLayer"/> parameter as the first layer.
    /// </summary>
    /// <param name="initialLayer">The new first layer.</param>
    public void Clear(ICellSurface initialLayer)
    {
        _layers.Clear();
        _layerVisibility.Clear();
        Add(initialLayer);
    }

    /// <inheritdoc/>
    public bool Contains(ICellSurface layer) =>
        _layers.Contains(layer);

    /// <inheritdoc/>
    public void CopyTo(ICellSurface[] array, int arrayIndex) =>
        _layers.CopyTo(array, arrayIndex);


    /// <inheritdoc/>
    public IEnumerator<ICellSurface> GetEnumerator() =>
        _layers.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() =>
        _layers.GetEnumerator();

    /// <summary>
    /// Adds a new layer. The layer is based on the first layer's width and height.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CellSurface Create()
    {
        if (_layers.Count == 0) throw new Exception("Can't create a layer unless an existing layer has been added first. The size of an existing layer is used to generate a new layer.");

        CellSurface newSurface = new(_layers[0].ViewWidth, _layers[0].ViewHeight, _layers[0].Width, _layers[0].Height);
        newSurface.DefaultBackground = Color.Transparent;
        newSurface.DefaultForeground = _layers[0].DefaultForeground;
        newSurface.DefaultGlyph = _layers[0].DefaultGlyph;
        newSurface.Clear();
        newSurface.View = _screenCachedView;

        _layers.Add(newSurface);
        _layerVisibility.Add(true);

        return newSurface;
    }

    private bool CheckLayerValidity(ICellSurface layer)
    {
        // No layers exist, so nothing to compare against
        if (_layers.Count == 0) return true;

        // Check first layer, if different, we can't add this one.
        if (layer.Width != _layers[0].Width || layer.Height != _layers[0].Height)
            return false;

        return true;
    }


    /// <summary>
    /// Resizes each layer to the specified width and height.
    /// </summary>
    /// <param name="viewWidth">The viewable width of the surface.</param>
    /// <param name="viewHeight">The viewable height of the surface.</param>
    /// <param name="totalWidth">The maximum width of the surface.</param>
    /// <param name="totalHeight">The maximum height of the surface.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public void Resize(int viewWidth, int viewHeight, int totalWidth, int totalHeight, bool clear)
    {
        foreach (ICellSurface layer in _layers)
        {
            if (layer is not ICellSurfaceResize surface)
                throw new Exception("Surface doesn't support resize.");

            surface.Resize(viewWidth, viewHeight, totalWidth, totalHeight, clear);
        }

        _screenCachedView = _layers[0].View;
    }

    /// <summary>
    /// Resizes the surface and view to the specified width and height.
    /// </summary>
    /// <param name="width">The width of the surface and view.</param>
    /// <param name="height">The height of the surface and view.</param>
    /// <param name="clear">When <see langword="true"/>, resets every cell to the <see cref="ICellSurface.DefaultForeground"/>, <see cref="ICellSurface.DefaultBackground"/> and glyph 0.</param>
    public void Resize(int width, int height, bool clear)
    {
        foreach (ICellSurface layer in _layers)
        {
            if (layer is not ICellSurfaceResize surface)
                throw new Exception("Surface doesn't support resize.");

            surface.Resize(width, height, clear);
        }

        _screenCachedView = _layers[0].View;
    }

    /// <inheritdoc/>
    public override void OnAdded(IScreenObject host)
    {
        if (_screen != null) throw new Exception("Component has already been added to a host.");
        if (host is not IScreenSurface surface) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");
        if (host is not ISurfaceSettable) throw new ArgumentException($"Must add this component to a type that implements {nameof(ISurfaceSettable)}");

        if (RenderStep != null)
        {
            surface.Renderer?.Steps.Remove(RenderStep);
            RenderStep.Dispose();
        }

        // If the renderer is a layered screen surface, skip adding our own
        if (surface.Renderer != null && surface.Renderer.Name != Renderers.Constants.RendererNames.LayeredScreenSurface)
        {
            RenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.SurfaceLayered);
            RenderStep.SetData(this);
            surface.Renderer?.Steps.Add(RenderStep);
            surface.Renderer?.Steps.Sort(Renderers.RenderStepComparer.Instance);
        }
        _screen = surface;

        Add(surface.Surface);

        _screenCachedView = surface.Surface.View;
        IsAttached = true;
    }

    /// <inheritdoc/>
    public override void OnRemoved(IScreenObject host)
    {
        if (RenderStep != null)
        {
            ((IScreenSurface)host).Renderer?.Steps.Remove(RenderStep);
            RenderStep.Dispose();
            RenderStep = null;
        }

        _screen = null;
        _screenCachedView = Rectangle.Empty;

        IsAttached = false;
    }

    /// <inheritdoc/>
    public override void Update(IScreenObject host, TimeSpan delta)
    {
        // View or font changed on parent surface, re-evaluate everything
        if (IsAttached)
        {
            if (_screenCachedView != _screen.Surface.View)
            {
                _screenCachedView = _screen.Surface.View;

                foreach (ICellSurface layer in _layers)
                    layer.View = _screenCachedView;

                _screen.IsDirty = true;
            }

            // If the current screen isn't dirty, check if any layer is.
            if (!_screen.IsDirty)
            {
                foreach (ICellSurface layer in _layers)
                {
                    if (layer.IsDirty)
                    {
                        _screen.IsDirty = true;
                        break;
                    }
                }
            }
        }
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        //List<Entity> entitiesBackup = _layers;
        //_layers = new List<Entity>(entitiesBackup.Count);

        //AddRange(entitiesBackup);

        //IsDirty = true;
    }
}
