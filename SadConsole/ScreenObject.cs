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
    //[Newtonsoft.Json.JsonConverter(typeof(SerializedTypes.ScreenObjectJsonConverter))]
    [DataContract]
    public class ScreenObject : IScreenObject
    {
        [DataMember(Name = "Children")]
        private IScreenObject[] _childrenSerialized;

        [DataMember(Name = "Components")]
        private IComponent[] _componentsSerialized;

        private IScreenObject _parentObject;
        private Point _position;
        private bool _isVisible = true;
        private bool _isEnabled = true;

        /// <inheritdoc/>
        public event EventHandler<NewOldValueEventArgs<IScreenObject>> ParentChanged;

        /// <inheritdoc/>
        public event EventHandler<NewOldValueEventArgs<Point>> PositionChanged;

        /// <inheritdoc/>
        public event EventHandler VisibleChanged;

        /// <inheritdoc/>
        public event EventHandler EnabledChanged;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IComponent.IsUpdate"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsUpdate;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IComponent.IsDraw"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsDraw;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IComponent.IsMouse"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsMouse;

        /// <summary>
        /// A filtered list from <see cref="Components"/> where <see cref="IComponent.IsKeyboard"/> is <see langword="true"/>.
        /// </summary>
        protected List<IComponent> ComponentsKeyboard;

        /// <summary>
        /// A filtered list from <see cref="Components"/> that is not set for update, draw, mouse, or keyboard.
        /// </summary>
        protected List<IComponent> ComponentsEmpty;

        /// <inheritdoc/>
        public ObservableCollection<IComponent> Components { get; private set; }

        /// <inheritdoc/>
        public ScreenObjectCollection Children { get; }

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
        [DataMember]
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
            Components = new ObservableCollection<IComponent>();
            ComponentsUpdate = new List<IComponent>();
            ComponentsDraw = new List<IComponent>();
            ComponentsKeyboard = new List<IComponent>();
            ComponentsMouse = new List<IComponent>();
            Components.CollectionChanged += Components_CollectionChanged;
            Children = new ScreenObjectCollection(this);
        }

        /// <inheritdoc/>
        public virtual void Draw()
        {
            if (!IsVisible) return;

            foreach (IComponent component in ComponentsDraw.ToArray())
                component.Draw(this);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Draw();
        }

        /// <inheritdoc/>
        public virtual void Update()
        {
            if (!IsEnabled) return;

            foreach (IComponent component in ComponentsUpdate.ToArray())
                component.Update(this);

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Update();
        }

        /// <inheritdoc/>
        public virtual bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;

            foreach (Components.IComponent component in ComponentsKeyboard.ToArray())
            {
                component.ProcessKeyboard(this, keyboard, out bool isHandled);

                if (isHandled)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public IEnumerable<IComponent> GetComponents<TComponent>()
            where TComponent : IComponent
        {
            foreach (IComponent component in Components)
            {
                if (component is TComponent)
                    yield return component;
            }
        }

        /// <inheritdoc/>
        public IComponent GetComponent<TComponent>()
            where TComponent : IComponent
        {
            foreach (IComponent component in Components)
            {
                if (component is TComponent)
                    return component;
            }

            return null;
        }


        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        FilterAddItem((IComponent)item);
                        ((IComponent)item).OnAdded(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        FilterRemoveItem((IComponent)item);
                        ((IComponent)item).OnRemoved(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.NewItems)
                    {
                        FilterAddItem((IComponent)item);
                        ((IComponent)item).OnAdded(this);
                    }
                    foreach (object item in e.OldItems)
                    {
                        FilterRemoveItem((IComponent)item);
                        ((IComponent)item).OnRemoved(this);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Calling Clear in this object is not supported. Use the RemoveAll extension method.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            void FilterAddItem(IComponent component)
            {
                if (component.IsDraw)
                {
                    if (!ComponentsDraw.Contains(component))
                        ComponentsDraw.Add(component);
                }

                if (component.IsUpdate)
                {
                    if (!ComponentsUpdate.Contains(component))
                        ComponentsUpdate.Add(component);
                }

                if (component.IsKeyboard)
                {
                    if (!ComponentsKeyboard.Contains(component))
                        ComponentsKeyboard.Add(component);
                }

                if (component.IsMouse)
                {
                    if (!ComponentsMouse.Contains(component))
                        ComponentsMouse.Add(component);
                }

                if (!component.IsDraw && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                {
                    if (!ComponentsEmpty.Contains(component))
                        ComponentsEmpty.Add(component);
                }

                ComponentsDraw.Sort(CompareComponent);
                ComponentsUpdate.Sort(CompareComponent);
                ComponentsKeyboard.Sort(CompareComponent);
                ComponentsMouse.Sort(CompareComponent);
            }

            void FilterRemoveItem(IComponent component)
            {
                if (component.IsDraw)
                {
                    if (ComponentsDraw.Contains(component))
                        ComponentsDraw.Remove(component);
                }

                if (component.IsUpdate)
                {
                    if (ComponentsUpdate.Contains(component))
                        ComponentsUpdate.Remove(component);
                }

                if (component.IsKeyboard)
                {
                    if (ComponentsKeyboard.Contains(component))
                        ComponentsKeyboard.Remove(component);
                }

                if (component.IsMouse)
                {
                    if (ComponentsMouse.Contains(component))
                        ComponentsMouse.Remove(component);
                }

                if (!component.IsDraw && !component.IsUpdate && !component.IsKeyboard && !component.IsMouse)
                {
                    if (!ComponentsEmpty.Contains(component))
                        ComponentsEmpty.Remove(component);
                }

                ComponentsDraw.Sort(CompareComponent);
                ComponentsUpdate.Sort(CompareComponent);
                ComponentsKeyboard.Sort(CompareComponent);
                ComponentsMouse.Sort(CompareComponent);
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

        /// <summary>
        /// Raises the <see cref="ParentChanged"/> event.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
        {
            UpdateAbsolutePosition();
            ParentChanged?.Invoke(this, new NewOldValueEventArgs<IScreenObject>(oldParent, newParent));
        }

        /// <summary>
        /// Raises the <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="oldPosition">The previous position.</param>
        /// <param name="newPosition">The new position.</param>
        protected virtual void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            UpdateAbsolutePosition();
            PositionChanged?.Invoke(this, new NewOldValueEventArgs<Point>(oldPosition, newPosition));
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

        [OnSerializing]
        protected void OnSerializingMethod(StreamingContext context)
        {
            _childrenSerialized = Children.ToArray();
            _componentsSerialized = Components.ToArray();
        }

        [OnSerialized]
        protected void OnSerializedMethod(StreamingContext context)
        {
            _childrenSerialized = null;
            _componentsSerialized = null;
        }

        [OnDeserialized]
        protected void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var item in _childrenSerialized)
                Children.Add(item);

            foreach (var item in _componentsSerialized)
                Components.Add(item);

            _componentsSerialized = null;
            _childrenSerialized = null;

            UpdateAbsolutePosition();
        }
    }
}
