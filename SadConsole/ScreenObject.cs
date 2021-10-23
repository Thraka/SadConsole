using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// A generic object processed by SadConsole. Provides parent/child, components, and position.
    /// </summary>
    [DataContract]
    public class ScreenObject : IScreenObject
    {
        [DataMember(Name = "Children")]
        private IScreenObject[] _childrenSerialized;

        [DataMember(Name = "ChildrenLocked")]
        private bool _isChildrenLocked;

        [DataMember(Name = "Components")]
        private IComponent[] _componentsSerialized;

        [DataMember(Name = "Position")]
        private Point _position;

        private IScreenObject _parentObject;
        private bool _isVisible = true;
        private bool _isEnabled = true;
        private bool _isfocused;


        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<IScreenObject>> ParentChanged;

        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<Point>> PositionChanged;

        /// <inheritdoc/>
        public event EventHandler VisibleChanged;

        /// <inheritdoc/>
        public event EventHandler EnabledChanged;

        /// <inheritdoc/>
        public event EventHandler FocusLost;

        /// <inheritdoc/>
        public event EventHandler Focused;

        /// <summary>
        /// A filtered list from <see cref="SadComponents"/> where <see cref="IComponent.IsUpdate"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsUpdate;

        /// <summary>
        /// A filtered list from <see cref="SadComponents"/> where <see cref="IComponent.IsRender"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsRender;

        /// <summary>
        /// A filtered list from <see cref="SadComponents"/> where <see cref="IComponent.IsMouse"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsMouse;

        /// <summary>
        /// A filtered list from <see cref="SadComponents"/> where <see cref="IComponent.IsKeyboard"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsKeyboard;

        /// <summary>
        /// A filtered list from <see cref="SadComponents"/> that is not set for update, render, mouse, or keyboard.
        /// </summary>
        protected List<IComponent> ComponentsEmpty;

        /// <inheritdoc/>
        public ObservableCollection<IComponent> SadComponents { get; protected set; }

        /// <inheritdoc/>
        public ScreenObjectCollection Children { get; protected set; }

        /// <inheritdoc/>
        public IScreenObject Parent
        {
            get => _parentObject;
            set
            {
                if (value == this) throw new Exception("Cannot set parent to itself.");
                if (_parentObject == value) return;

                if (_parentObject == null)
                {
                    _parentObject = value;
                    _parentObject.Children.Add(this);
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

        /// <inheritdoc/>
        public Point Position
        {
            get => _position;
            set
            {
                if (_position == value) return;

                Point oldPoint = _position;
                _position = value;
                OnPositionChanged(oldPoint, _position);
            }
        }

        /// <inheritdoc/>
        public Point AbsolutePosition { get; protected set; }

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
            get => _isfocused;
            set
            {
                if ((_isfocused && value) || (!_isfocused && !value)) return;

                _isfocused = value;

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
            SadComponents = new ObservableCollection<IComponent>();
            ComponentsUpdate = new List<IComponent>();
            ComponentsRender = new List<IComponent>();
            ComponentsKeyboard = new List<IComponent>();
            ComponentsMouse = new List<IComponent>();
            ComponentsEmpty = new List<IComponent>();
            SadComponents.CollectionChanged += Components_CollectionChanged;
            Children = new ScreenObjectCollection(this);
        }

        /// <inheritdoc/>
        public virtual void Render(TimeSpan delta)
        {
            if (!IsVisible) return;

            var components = ComponentsRender.ToArray();
            for (int i = 0; i < components.Length; i++)
                components[i].Render(this, delta);

            var children = new List<IScreenObject>(Children);
            for (int i = 0; i < children.Count; i++)
                children[i].Render(delta);
        }

        /// <inheritdoc/>
        public virtual void Update(TimeSpan delta)
        {
            if (!IsEnabled) return;

            var components = ComponentsUpdate.ToArray();
            for (int i = 0; i < components.Length; i++)
                components[i].Update(this, delta);

            var children = new List<IScreenObject>(Children);
            for (int i = 0; i < children.Count; i++)
                children[i].Update(delta);
        }

        /// <inheritdoc/>
        public virtual bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;

            var components = ComponentsKeyboard.ToArray();
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

            if (!UseMouse)
                return false;

            var components = ComponentsMouse.ToArray();
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

        /// <inheritdoc/>
        public IEnumerable<TComponent> GetSadComponents<TComponent>()
            where TComponent : class, IComponent
        {
            foreach (IComponent component in SadComponents)
            {
                if (component is TComponent)
                    yield return (TComponent)component;
            }
        }

        /// <inheritdoc/>
        public TComponent GetSadComponent<TComponent>()
            where TComponent : class, IComponent
        {
            foreach (IComponent component in SadComponents)
            {
                if (component is TComponent)
                    return (TComponent)component;
            }

            return null;
        }

        /// <summary>
        /// Called when a component is added to the <see cref="SadComponents"/> collection.
        /// </summary>
        /// <param name="component">The component added.</param>
        protected virtual void OnSadComponentAdded(IComponent component) { }

        /// <summary>
        /// Called when a component is removed from the <see cref="SadComponents"/> collection.
        /// </summary>
        /// <param name="component">The component removed.</param>
        protected virtual void OnSadComponentRemoved(IComponent component) { }

        /// <inheritdoc/>
        public bool HasSadComponent<TComponent>(out TComponent component)
            where TComponent: class, IComponent
        {
            foreach (IComponent comp in SadComponents)
            {
                if (comp is TComponent)
                {
                    component = (TComponent)comp;
                    return true;
                }
            }

            component = null;
            return false;
        }

        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        Components_FilterAddItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnAdded(this);
                        OnSadComponentAdded((IComponent)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        Components_FilterRemoveItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnRemoved(this);
                        OnSadComponentRemoved((IComponent)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.NewItems)
                    {
                        Components_FilterAddItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnAdded(this);
                        OnSadComponentAdded((IComponent)item);
                    }
                    foreach (object item in e.OldItems)
                    {
                        Components_FilterRemoveItem((IComponent)item, ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                        ((IComponent)item).OnRemoved(this);
                        OnSadComponentRemoved((IComponent)item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    SortComponents();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    List<IComponent> items = new List<IComponent>(ComponentsRender.Count + ComponentsUpdate.Count + ComponentsKeyboard.Count + ComponentsMouse.Count);

                    while (ComponentsRender.Count != 0)
                    {
                        ComponentsRender[0].OnRemoved(this);

                        if (!items.Contains(ComponentsRender[0]))
                            items.Add(ComponentsRender[0]);

                        Components_FilterRemoveItem(ComponentsRender[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                    }
                    while (ComponentsUpdate.Count != 0)
                    {
                        ComponentsUpdate[0].OnRemoved(this);

                        if (!items.Contains(ComponentsUpdate[0]))
                            items.Add(ComponentsUpdate[0]);

                        Components_FilterRemoveItem(ComponentsUpdate[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                    }
                    while (ComponentsKeyboard.Count != 0)
                    {
                        ComponentsKeyboard[0].OnRemoved(this);

                        if (!items.Contains(ComponentsKeyboard[0]))
                            items.Add(ComponentsKeyboard[0]);

                        Components_FilterRemoveItem(ComponentsKeyboard[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                    }
                    while (ComponentsMouse.Count != 0)
                    {
                        ComponentsMouse[0].OnRemoved(this);

                        if (!items.Contains(ComponentsMouse[0]))
                            items.Add(ComponentsMouse[0]);

                        Components_FilterRemoveItem(ComponentsMouse[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                    }
                    while (ComponentsEmpty.Count != 0)
                    {
                        ComponentsEmpty[0].OnRemoved(this);

                        if (!items.Contains(ComponentsEmpty[0]))
                            items.Add(ComponentsEmpty[0]);

                        Components_FilterRemoveItem(ComponentsEmpty[0], ComponentsRender, ComponentsUpdate, ComponentsKeyboard, ComponentsMouse, ComponentsEmpty);
                    }

                    foreach (IComponent item in items)
                        OnSadComponentRemoved(item);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Raises the <see cref="ParentChanged"/> event.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
        {
            UpdateAbsolutePosition();
            ParentChanged?.Invoke(this, new ValueChangedEventArgs<IScreenObject>(oldParent, newParent));
        }

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
            VisibleChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the paused status of the object changes.
        /// </summary>
        protected virtual void OnEnabledChanged() =>
            EnabledChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc/>
        public virtual void UpdateAbsolutePosition()
        {
            AbsolutePosition = Position + (Parent?.AbsolutePosition ?? new Point(0, 0));

            foreach (IScreenObject child in Children)
                child.UpdateAbsolutePosition();
        }

        /// <summary>
        /// Sorts the components based on the <see cref="IComponent.SortOrder"/> value.
        /// </summary>
        public void SortComponents()
        {
            ComponentsRender.Sort(CompareComponent);
            ComponentsUpdate.Sort(CompareComponent);
            ComponentsKeyboard.Sort(CompareComponent);
            ComponentsMouse.Sort(CompareComponent);
            ComponentsEmpty.Sort(CompareComponent);
        }

        /// <summary>
        /// Returns the value "ScreenObject".
        /// </summary>
        /// <returns>The string "ScreenObject".</returns>
        public override string ToString() =>
            "ScreenObject";

        /// <summary>
        /// Nothing.
        /// </summary>
        /// <param name="context">Nothing.</param>
        [OnSerializing]
        protected void OnSerializingMethod(StreamingContext context)
        {
            _childrenSerialized = Children.ToArray();
            _componentsSerialized = SadComponents.ToArray();
            _isChildrenLocked = Children.IsLocked;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            _childrenSerialized = null;
            _componentsSerialized = null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (IScreenObject item in _childrenSerialized)
                Children.Add(item);

            foreach (IComponent item in _componentsSerialized)
                SadComponents.Add(item);

            Children.IsLocked = _isChildrenLocked;

            _componentsSerialized = null;
            _childrenSerialized = null;

            UpdateAbsolutePosition();
        }


        /// <summary>
        /// Adds a component to the provided collections, based on its configuration.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="componentsRender">The render collection.</param>
        /// <param name="componentsUpdate">The update collection.</param>
        /// <param name="componentsKeyboard">The keyboard collection.</param>
        /// <param name="componentsMouse">The mouse collection.</param>
        /// <param name="componentsEmpty">The empty collection.</param>
        public static void Components_FilterAddItem(IComponent component,
                                                    List<IComponent> componentsRender,
                                                    List<IComponent> componentsUpdate,
                                                    List<IComponent> componentsKeyboard,
                                                    List<IComponent> componentsMouse,
                                                    List<IComponent> componentsEmpty)
        {
            if (component.IsRender)
                if (!componentsRender.Contains(component))
                    componentsRender.Add(component);

            if (component.IsUpdate)
                if (!componentsUpdate.Contains(component))
                    componentsUpdate.Add(component);

            if (component.IsKeyboard)
                if (!componentsKeyboard.Contains(component))
                    componentsKeyboard.Add(component);

            if (component.IsMouse)
                if (!componentsMouse.Contains(component))
                    componentsMouse.Add(component);

            if (!component.IsRender && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                if (!componentsEmpty.Contains(component))
                    componentsEmpty.Add(component);

            componentsRender.Sort(CompareComponent);
            componentsUpdate.Sort(CompareComponent);
            componentsKeyboard.Sort(CompareComponent);
            componentsMouse.Sort(CompareComponent);
            componentsEmpty.Sort(CompareComponent);
        }

        /// <summary>
        /// Removes a component to the provided collections, based on its configuration.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="componentsRender">The render collection.</param>
        /// <param name="componentsUpdate">The update collection.</param>
        /// <param name="componentsKeyboard">The keyboard collection.</param>
        /// <param name="componentsMouse">The mouse collection.</param>
        /// <param name="componentsEmpty">The empty collection.</param>
        public static void Components_FilterRemoveItem(IComponent component,
                                                        List<IComponent> componentsRender,
                                                        List<IComponent> componentsUpdate,
                                                        List<IComponent> componentsKeyboard,
                                                        List<IComponent> componentsMouse,
                                                        List<IComponent> componentsEmpty)
        {
            if (component.IsRender)
                if (componentsRender.Contains(component))
                    componentsRender.Remove(component);

            if (component.IsUpdate)
                if (componentsUpdate.Contains(component))
                    componentsUpdate.Remove(component);

            if (component.IsKeyboard)
                if (componentsKeyboard.Contains(component))
                    componentsKeyboard.Remove(component);

            if (component.IsMouse)
                if (componentsMouse.Contains(component))
                    componentsMouse.Remove(component);

            if (!component.IsRender && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                if (!componentsEmpty.Contains(component))
                    componentsEmpty.Remove(component);

            componentsRender.Sort(CompareComponent);
            componentsUpdate.Sort(CompareComponent);
            componentsKeyboard.Sort(CompareComponent);
            componentsMouse.Sort(CompareComponent);
            componentsEmpty.Sort(CompareComponent);
        }

        /// <summary>
        /// Helper to sort the components in the split collections.
        /// </summary>
        /// <param name="componentsRender">The render collection.</param>
        /// <param name="componentsUpdate">The update collection.</param>
        /// <param name="componentsKeyboard">The keyboard collection.</param>
        /// <param name="componentsMouse">The mouse collection.</param>
        /// <param name="componentsEmpty">The empty collection.</param>
        public static void Components_Sort(List<IComponent> componentsRender,
                                            List<IComponent> componentsUpdate,
                                            List<IComponent> componentsKeyboard,
                                            List<IComponent> componentsMouse,
                                            List<IComponent> componentsEmpty)
        {
            componentsRender.Sort(CompareComponent);
            componentsUpdate.Sort(CompareComponent);
            componentsKeyboard.Sort(CompareComponent);
            componentsMouse.Sort(CompareComponent);
            componentsEmpty.Sort(CompareComponent);
        }

        static int CompareComponent(IComponent left, IComponent right)
        {
            if (left.SortOrder > right.SortOrder)
                return 1;

            if (left.SortOrder < right.SortOrder)
                return -1;

            return 0;
        }
    }
}
