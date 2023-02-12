﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadConsole.Components;

/// <summary>
/// Manages a set of entities. Adds a render step and only renders the entities that are in the parent <see cref="IScreenSurface"/> visible area.
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("Entity host")]
public class LayeredSurface : Components.UpdateComponent, Components.IComponent
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
    /// The entities to process.
    /// </summary>
    [DataMember(Name = "Layers")] 
    protected List<ICellSurface> _layers = new(3);

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

    public IReadOnlyList<ICellSurface> Layers => _layers.AsReadOnly();

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
            layer.View = _screenCachedView;
        }
    }

    /// <summary>
    /// Adds a collection if layers to this component.
    /// </summary>
    /// <param name="layers">The layers to add.</param>
    public void AddRange(IEnumerable<ICellSurface> layers)
    {
        foreach (ICellSurface layer in _layers)
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
    public void Remove(ICellSurface layer) =>
        _layers.Remove(layer);

    /// <summary>
    /// Removes all layers from this component.
    /// </summary>
    public void RemoveAll()
    {
        while (_layers.Count != 0)
            Remove(_layers[_layers.Count - 1]);
    }

    /// <summary>
    /// Adds a new layer. The layer is based on the first layer's width and height.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CellSurface Create()
    {
        if (_layers.Count == 0) throw new Exception("Can't create a layer unless an existing layer has been added first. The size of an existing layer is used to generate a new layer.");

        CellSurface newSurface = new CellSurface(_layers[0].Width, _layers[0].Height, _layers[0].ViewWidth, _layers[0].ViewHeight);
        newSurface.DefaultBackground = Color.Transparent;
        newSurface.DefaultForeground = _layers[0].DefaultForeground;
        newSurface.DefaultGlyph = _layers[0].DefaultGlyph;
        newSurface.Clear();
        newSurface.View = _screenCachedView;

        _layers.Add(newSurface);

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

    /// <inheritdoc/>
    public override void OnAdded(IScreenObject host)
    {
        if (_screen != null) throw new Exception("Component has already been added to a host.");
        if (host is not IScreenSurface surface) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");
        if (host is not ISurfaceSettable) throw new ArgumentException($"Must add this component to a type that implements {nameof(ISurfaceSettable)}");

        if (RenderStep != null)
        {
            surface.RenderSteps.Remove(RenderStep);
            RenderStep.Dispose();
        }

        RenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.SurfaceLayered);
        RenderStep.SetData(this);
        surface.RenderSteps.Add(RenderStep);
        surface.RenderSteps.Sort(new Renderers.RenderStepComparer());
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
            ((IScreenSurface)host).RenderSteps.Remove(RenderStep);
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
