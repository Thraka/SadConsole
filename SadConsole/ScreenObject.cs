using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A generic object processed by SadConsole. Provides parent/child, components, and position.
/// </summary>
[DataContract]
[JsonObject(memberSerialization: MemberSerialization.OptIn)]
public partial class ScreenObject : IScreenObject
{
    [DataMember(Name = "Position")]
    private Point _position;

    private IScreenObject? _parentObject;
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private bool _isFocused;

    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<IScreenObject?>>? ParentChanged;

    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanged;

    /// <inheritdoc/>
    public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanging;

    /// <inheritdoc/>
    public event EventHandler? IsVisibleChanged;

    /// <inheritdoc/>
    public event EventHandler? IsEnabledChanged;

    /// <inheritdoc/>
    public event EventHandler? FocusLost;

    /// <inheritdoc/>
    public event EventHandler? Focused;

    /// <summary>
    /// Indicates the sorting order this object should use when parented. Sorting is a manual operation on the <see cref="Children"/> collection.
    /// </summary>
    [DataMember]
    public uint SortOrder { get; set; }

    /// <inheritdoc/>
    public ScreenObjectCollection Children { get; protected set; }

    /// <inheritdoc/>
    public IScreenObject? Parent
    {
        get => _parentObject;
        set
        {
            if (value == this) throw new Exception("Cannot set parent to itself.");
            if (_parentObject == value) return;

            if (_parentObject == null)
            {
                _parentObject = value;
                _parentObject?.Children.Add(this);
                OnParentChanged(null, _parentObject);
            }
            else
            {
                IScreenObject oldParent = _parentObject;
                _parentObject = null;
                oldParent.Children.Remove(this);
                _parentObject = value;

                _parentObject?.Children.Add(this);
                OnParentChanged(oldParent, _parentObject);
            }
        }
    }

    /// <summary>
    /// A position that is based on the current <see cref="Position"/> and <see cref="Parent"/> position, in pixels.
    /// </summary>
    public Point Position
    {
        get => _position;
        set
        {
            if (_position == value) return;

            Point oldPosition = _position;

            try
            {
                OnPositionChanging(_position, value);
                _position = value;
                OnPositionChanged(oldPosition, _position);
            }
            catch (Exception)
            {
                _position = oldPosition;
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public Point AbsolutePosition { get; protected set; }

    /// <inheritdoc/>
    [DataMember]
    public bool IgnoreParentPosition { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value) return;

            _isVisible = value;
            OnVisibleChanged();
        }
    }

    /// <inheritdoc/>
    [DataMember]
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value) return;

            _isEnabled = value;
            OnEnabledChanged();
        }
    }

    /// <inheritdoc/>
    public bool IsFocused
    {
        get => _isFocused;
        set
        {
            if ((_isFocused && value) || (!_isFocused && !value)) return;

            _isFocused = value;

            if (value)
            {
                switch (FocusedMode)
                {
                    case FocusBehavior.Set:
                        GameHost.Instance.FocusedScreenObjects.Set(this);
                        break;
                    case FocusBehavior.Push:
                        GameHost.Instance.FocusedScreenObjects.Push(this);
                        break;
                    default:
                        break;
                }

                Focused?.Invoke(this, EventArgs.Empty);
                OnFocused();
            }
            else
            {
                if (GameHost.Instance.FocusedScreenObjects.ScreenObject == this && FocusedMode != FocusBehavior.None)
                    GameHost.Instance.FocusedScreenObjects.Pop(this);

                FocusLost?.Invoke(this, EventArgs.Empty);
                OnFocusLost();
            }
        }
    }

    /// <inheritdoc/>
    [DataMember]
    public FocusBehavior FocusedMode { get; set; } = FocusBehavior.Set;

    /// <inheritdoc/>
    [DataMember]
    public bool IsExclusiveMouse { get; set; }
    /// <inheritdoc/>
    [DataMember]
    public bool UseKeyboard { get; set; }

    /// <inheritdoc/>
    [DataMember]
    public bool UseMouse { get; set; }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    public ScreenObject()
    {
        UseMouse = Settings.DefaultScreenObjectUseMouse;
        UseKeyboard = Settings.DefaultScreenObjectUseKeyboard;
        Children = new ScreenObjectCollection(this);
        SadComponents = new ObservableCollection<IComponent>();
        ComponentsUpdate = new List<IComponent>();
        ComponentsRender = new List<IComponent>();
        ComponentsKeyboard = new List<IComponent>();
        ComponentsMouse = new List<IComponent>();
        ComponentsEmpty = new List<IComponent>();
        SadComponents.CollectionChanged += Components_CollectionChanged;
    }

    /// <inheritdoc/>
    public virtual void Render(TimeSpan delta)
    {
        IComponent[] components = ComponentsRender.ToArray();
        int count = components.Length;
        for (int i = 0; i < count; i++)
            components[i].Render(this, delta);

        IScreenObject[] children = Children.ToArray();
        count = children.Length;
        for (int i = 0; i < count; i++)
            if (children[i].IsVisible)
                children[i].Render(delta);
    }

    /// <inheritdoc/>
    public virtual void Update(TimeSpan delta)
    {
        if (ComponentsUpdate.Count > 0)
        {
            IComponent[] components = ComponentsUpdate.ToArray();
            int count = components.Length;
            for (int i = 0; i < count; i++)
                components[i].Update(this, delta);
        }

        if (Children.Count > 0)
        {
            IScreenObject[] children = Children.ToArray();
            int count = children.Length;
            for (int i = 0; i < count; i++)
                if (children[i].IsEnabled)
                    children[i].Update(delta);
        }
    }

    /// <inheritdoc/>
    public virtual bool ProcessKeyboard(Keyboard keyboard)
    {
        IComponent[] components = ComponentsKeyboard.ToArray();
        for (int i = 0; i < components.Length; i++)
        {
            components[i].ProcessKeyboard(this, keyboard, out bool isHandled);

            if (isHandled)
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public virtual bool ProcessMouse(MouseScreenObjectState state)
    {
        if (!IsVisible)
            return false;

        IComponent[] components = ComponentsMouse.ToArray();
        for (int i = 0; i < components.Length; i++)
        {
            components[i].ProcessMouse(this, state, out bool isHandled);

            if (isHandled)
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public virtual void LostMouse(MouseScreenObjectState state) { }

    /// <inheritdoc/>
    public virtual void OnFocusLost() { }

    /// <inheritdoc/>
    public virtual void OnFocused() { }

    /// <summary>
    /// Raises the <see cref="ParentChanged"/> event.
    /// </summary>
    /// <param name="oldParent">The previous parent.</param>
    /// <param name="newParent">The new parent.</param>
    protected virtual void OnParentChanged(IScreenObject? oldParent, IScreenObject? newParent)
    {
        UpdateAbsolutePosition();
        ParentChanged?.Invoke(this, new ValueChangedEventArgs<IScreenObject?>(oldParent, newParent));
    }

    /// <summary>
    /// Raises the <see cref="PositionChanging"/> event.
    /// </summary>
    /// <param name="oldPosition">The previous position.</param>
    /// <param name="newPosition">The new position.</param>
    protected virtual void OnPositionChanging(Point oldPosition, Point newPosition) =>
        PositionChanging?.Invoke(this, new ValueChangedEventArgs<Point>(oldPosition, newPosition));

    /// <summary>
    /// Raises the <see cref="PositionChanged"/> event.
    /// </summary>
    /// <param name="oldPosition">The previous position.</param>
    /// <param name="newPosition">The new position.</param>
    protected virtual void OnPositionChanged(Point oldPosition, Point newPosition)
    {
        UpdateAbsolutePosition();

        PositionChanged?.Invoke(this, new ValueChangedEventArgs<Point>(oldPosition, newPosition));
    }

    /// <summary>
    /// Called when the visibility of the object changes.
    /// </summary>
    protected virtual void OnVisibleChanged() =>
        IsVisibleChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Called when the paused status of the object changes.
    /// </summary>
    protected virtual void OnEnabledChanged() =>
        IsEnabledChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc/>
    public virtual void UpdateAbsolutePosition()
    {
        AbsolutePosition = !IgnoreParentPosition ? Position + (Parent?.AbsolutePosition ?? Point.Zero) : Position;

        int count = Children.Count;
        for (int i = 0; i < count; i++)
            Children[i].UpdateAbsolutePosition();
    }

    /// <summary>
    /// Returns the value "ScreenObject".
    /// </summary>
    /// <returns>The string "ScreenObject".</returns>
    public override string ToString() =>
        "ScreenObject";
}
