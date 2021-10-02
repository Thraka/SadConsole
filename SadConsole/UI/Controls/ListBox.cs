﻿using SadConsole.Input;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// A scrollable list control.
    /// </summary>
    [DataContract]
    public class ListBox : CompositeControl
    {
        /// <summary>
        /// The event args used when the selected item changes.
        /// </summary>
        public class SelectedItemEventArgs : EventArgs
        {
            /// <summary>
            /// The item selected.
            /// </summary>
            public readonly object Item;

            /// <summary>
            /// Creates a new instance of this type with the specified item.
            /// </summary>
            /// <param name="item">The selected item from the list.</param>
            public SelectedItemEventArgs(object item) =>
                Item = item;
        }

        [DataMember(Name = "SelectedIndex")]
        private int _selectedIndex = -1;
        private object _selectedItem;
        private DateTime _leftMouseLastClick = DateTime.Now;

        [DataMember]
        private Orientation _serializedScrollOrientation;

        [DataMember]
        private Point _serializedScrollPosition;

        [DataMember]
        private int _serializedScrollSizeValue;

        /// <summary>
        /// An event that triggers when the <see cref="SelectedItem"/> changes.
        /// </summary>
        public event EventHandler<SelectedItemEventArgs> SelectedItemChanged;

        /// <summary>
        /// An event that triggers when an item is double clicked or the Enter key is pressed while the listbox has focus.
        /// </summary>
        public event EventHandler<SelectedItemEventArgs> SelectedItemExecuted;

        /// <summary>
        /// The theme used by the listbox items.
        /// </summary>
        public ListBoxItemTheme ItemTheme { get; private set; }

        /// <summary>
        /// Internal use only; used in rendering.
        /// </summary>
        public bool IsScrollBarVisible
        {
            get => ScrollBar.IsVisible;
            set => ScrollBar.IsVisible = value;
        }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        [DataMember(Name = "ScrollBar")]
        public ScrollBar ScrollBar { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        public int RelativeIndexMouseOver { get; private set; }

        /// <summary>
        /// The total items visible in the listbox.
        /// </summary>
        public int VisibleItemsTotal { get; set; }

        /// <summary>
        /// The maximum amount of items that can be shown in the listbox.
        /// </summary>
        public int VisibleItemsMax { get; set; }

        /// <summary>
        /// When the <see cref="SelectedItem"/> changes, and this property is true, objects will be compared by reference. If false, they will be compared by value.
        /// </summary>
        [DataMember]
        public bool CompareByReference { get; set; }

        /// <summary>
        /// When set to <see langword="true"/>, the <see cref="SelectedItemExecuted"/> event will fire when an item is single-clicked instead of double-clicked.
        /// </summary>
        [DataMember]
        public bool SingleClickItemExecute { get; set; }

        /// <summary>
        /// The items in the listbox.
        /// </summary>
        [DataMember]
        public ObservableCollection<object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == -1)
                {
                    _selectedIndex = -1;
                    _selectedItem = null;
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
                else if (value < 0 || value >= Items.Count)
                    throw new IndexOutOfRangeException();
                else
                {
                    _selectedIndex = value;
                    _selectedItem = Items[value];
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null)
                {
                    _selectedIndex = -1;
                    _selectedItem = null;
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
                else
                {
                    // Find the item by index.
                    int index = -1;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (CompareByReference)
                        {
                            if (object.ReferenceEquals(Items[i], value))
                            {
                                index = i;
                                break;
                            }
                        }
                        else
                        {
                            if (object.Equals(Items[i], value))
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    if (index == -1)
                        throw new ArgumentOutOfRangeException("Item does not exist in collection.");

                    _selectedIndex = index;
                    _selectedItem = Items[index];
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the listbox control with the default theme for the items.
        /// </summary>
        /// <param name="width">The width of the listbox.</param>
        /// <param name="height">The height of the listbox.</param>
        public ListBox(int width, int height) : base(width, height)
        {
            Items = new ObservableCollection<object>();
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            ItemTheme = new ListBoxItemTheme();
        }

        /// <summary>
        /// Creates a new instance of the listbox control with the specified item theme.
        /// </summary>
        /// <param name="width">The width of the listbox.</param>
        /// <param name="height">The height of the listbox.</param>
        /// <param name="itemTheme">The theme to use with rendering the listbox items.</param>
        public ListBox(int width, int height, ListBoxItemTheme itemTheme) : this(width, height) => ItemTheme = itemTheme;

        private void _scrollbar_ValueChanged(object sender, EventArgs e) => IsDirty = true;

        /// <summary>
        /// Invokes the <see cref="SelectedItemChanged"/> event.
        /// </summary>
        protected virtual void OnSelectedItemChanged() => SelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(_selectedItem));

        /// <summary>
        /// Invokes the <see cref="SelectedItemExecuted"/> event.
        /// </summary>
        protected virtual void OnItemAction() => SelectedItemExecuted?.Invoke(this, new SelectedItemEventArgs(_selectedItem));

        /// <summary>
        /// Configures the associated <see cref="ScrollBar"/>.
        /// </summary>
        /// <param name="orientation">The orientation of the scrollbar.</param>
        /// <param name="sizeValue">The size of the scrollbar.</param>
        /// <param name="position">The position of the scrollbar.</param>
        public void SetupScrollBar(Orientation orientation, int sizeValue, Point position)
        {
            bool scrollBarExists = false;
            int value = 0;
            int max = 0;

            if (ScrollBar != null)
            {
                ScrollBar.ValueChanged -= _scrollbar_ValueChanged;
                value = ScrollBar.Value;
                max = ScrollBar.Maximum;
                RemoveControl(ScrollBar);
                scrollBarExists = true;
            }

            //_scrollBar.Width, height < 3 ? 3 : height - _scrollBarSizeAdjust
            ScrollBar = new ScrollBar(Orientation.Vertical, sizeValue);

            if (scrollBarExists)
            {
                ScrollBar.Maximum = max;
                ScrollBar.Value = value;
            }

            ScrollBar.ValueChanged += _scrollbar_ValueChanged;
            ScrollBar.Position = position;
            AddControl(ScrollBar);

            _serializedScrollSizeValue = sizeValue;
            _serializedScrollPosition = position;
            _serializedScrollOrientation = orientation;

            OnThemeChanged();
            DetermineState();
        }

        /// <summary>
        /// Scrolls the list to the item currently selected.
        /// </summary>
        public void ScrollToSelectedItem()
        {
            if (IsScrollBarVisible)
            {
                if (_selectedIndex < VisibleItemsMax)
                    ScrollBar.Value = 0;
                else if (SelectedIndex > Items.Count - VisibleItemsTotal)
                    ScrollBar.Value = ScrollBar.Maximum;
                else
                    ScrollBar.Value = _selectedIndex - VisibleItemsTotal;
            }
        }

        /// <summary>
        /// Sets the scrollbar's theme to the current theme's <see cref="ListBoxTheme.ScrollBarTheme"/>.
        /// </summary>
        protected override void OnThemeChanged()
        {
            if (ScrollBar == null) return;

            if (Theme is ListBoxTheme theme)
                ScrollBar.Theme = theme.ScrollBarTheme;
            else
                ScrollBar.Theme = null;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                ScrollBar.Value = 0;
            }

            if (SelectedItem != null && !Items.Contains(_selectedItem))
            {
                SelectedItem = null;
            }

            IsDirty = true;
        }

        /// <inheritdoc />
        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            //if (_hasFocus)
            if (info.IsKeyReleased(Keys.Up))
            {
                int index = Items.IndexOf(_selectedItem);
                if (_selectedItem != null)
                {
                    if (index != 0)
                    {
                        SelectedItem = Items[index - 1];

                        if (index <= ScrollBar.Value)
                        {
                            ScrollBar.Value -= 1;
                        }
                    }
                }
                return true;
            }
            else if (info.IsKeyReleased(Keys.Down))
            {
                int index = Items.IndexOf(_selectedItem);
                if (_selectedItem != null)
                {
                    if (index != Items.Count - 1)
                    {
                        SelectedItem = Items[index + 1];

                        if (index + 1 >= ScrollBar.Value + (Height - 2))
                        {
                            ScrollBar.Value += 1;
                        }
                    }
                }
                else if (Items.Count != 0)
                {
                    SelectedItem = Items[0];
                }

                return true;
            }
            else if (info.IsKeyReleased(Keys.Enter))
            {
                if (_selectedItem != null)
                {
                    OnItemAction();
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override void OnMouseIn(ControlMouseState state)
        {
            base.OnMouseIn(state);

            if (!(Theme is ListBoxTheme theme)) return;

            int rowOffset = ((ListBoxTheme)theme).DrawBorder ? 1 : 0;
            int rowOffsetReverse = ((ListBoxTheme)theme).DrawBorder ? 0 : 1;
            int columnOffsetEnd = IsScrollBarVisible || !((ListBoxTheme)theme).DrawBorder ? 1 : 0;

            Point mouseControlPosition = state.MousePosition;

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < Width - columnOffsetEnd)
            {
                if (IsScrollBarVisible)
                {
                    RelativeIndexMouseOver = mouseControlPosition.Y - rowOffset + ScrollBar.Value;
                }
                else if (mouseControlPosition.Y <= Items.Count - rowOffsetReverse)
                {
                    RelativeIndexMouseOver = mouseControlPosition.Y - rowOffset;
                }
                else
                {
                    RelativeIndexMouseOver = -1;
                }
            }
            else
            {
                RelativeIndexMouseOver = -1;
            }

            if (state.OriginalMouseState.Mouse.ScrollWheelValueChange != 0)
            {
                if (state.OriginalMouseState.Mouse.ScrollWheelValueChange < 0)
                    ScrollBar.Value -= 1;
                else
                    ScrollBar.Value += 1;
            }
        }

        /// <inheritdoc />
        protected override void OnLeftMouseClicked(ControlMouseState state)
        {
            base.OnLeftMouseClicked(state);

            if (!(Theme is ListBoxTheme theme)) return;

            DateTime click = DateTime.Now;
            bool doubleClicked = (click - _leftMouseLastClick).TotalSeconds <= 0.5;
            _leftMouseLastClick = click;

            int rowOffset = ((ListBoxTheme)theme).DrawBorder ? 1 : 0;
            int rowOffsetReverse = ((ListBoxTheme)theme).DrawBorder ? 0 : 1;
            int columnOffsetEnd = IsScrollBarVisible || !((ListBoxTheme)theme).DrawBorder ? 1 : 0;

            Point mouseControlPosition = state.MousePosition;

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < Width - columnOffsetEnd)
            {
                object oldItem = _selectedItem;
                bool noItem = false;

                if (IsScrollBarVisible)
                {
                    _selectedIndex = mouseControlPosition.Y - rowOffset + ScrollBar.Value;
                    SelectedItem = Items[_selectedIndex];
                }
                else if (mouseControlPosition.Y <= Items.Count - rowOffsetReverse)
                {
                    _selectedIndex = mouseControlPosition.Y - rowOffset;
                    SelectedItem = Items[_selectedIndex];
                }
                else
                {
                    noItem = true;
                }

                if (!noItem && (SingleClickItemExecute || (doubleClicked && oldItem == SelectedItem)))
                {
                    _leftMouseLastClick = DateTime.MinValue;
                    OnItemAction();
                }
            }
        }

        [OnSerializing]
        private void BeforeSerializing(StreamingContext context)
        {
            if (_selectedItem != null)
            {
                _selectedIndex = Items.IndexOf(_selectedItem);
            }
            else
            {
                _selectedIndex = -1;
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            SetupScrollBar(_serializedScrollOrientation, _serializedScrollSizeValue, _serializedScrollPosition);

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (_selectedIndex != -1)
            {
                SelectedItem = Items[_selectedIndex];
            }

            DetermineState();
            IsDirty = true;
        }


    }
}
