using SadConsole.Input;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SadConsole.UI.Controls
{
    [DataContract]
    public class ListBox : CompositeControl
    {
        public class SelectedItemEventArgs : EventArgs
        {
            public object Item;

            public SelectedItemEventArgs(object item) => Item = item;
        }

        protected bool initialized;

        [DataMember(Name = "SelectedIndex")]
        protected int selectedIndex;

        protected object selectedItem;
        //[DataMember(Name = "BorderLines")]
        //protected int[] borderLineStyle;
        protected DateTime leftMouseLastClick = DateTime.Now;

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

        public int VisibleItemsTotal { get; set; }
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

        [DataMember]
        public ObservableCollection<object> Items { get; private set; }

        public int SelectedIndex
        {
            get
            {
                int index = -1;
                if (selectedItem != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (CompareByReference)
                        {
                            if (object.ReferenceEquals(Items[i], selectedItem))
                            {
                                index = i;
                                break;
                            }
                        }
                        else
                        {
                            if (object.Equals(Items[i], selectedItem))
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }

                return index;
            }
        }

        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value == null)
                {
                    selectedIndex = -1;
                    selectedItem = null;
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
                else
                {
                    int index = Items.IndexOf(value);

                    if (index == -1)
                    {
                        throw new ArgumentOutOfRangeException("Item does not exist in collection.");
                    }

                    selectedIndex = index;
                    selectedItem = Items[index];
                    IsDirty = true;
                    OnSelectedItemChanged();
                }
            }
        }
        
        /// <summary>
        /// Creates a new instance of the listbox control.
        /// </summary>
        public ListBox(int width, int height) : base(width, height)
        {
            initialized = true;

            Items = new ObservableCollection<object>();
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            ItemTheme = new ListBoxItemTheme();
        }

        public ListBox(int width, int height, ListBoxItemTheme itemTheme) : this(width, height) => ItemTheme = itemTheme;

        private void _scrollbar_ValueChanged(object sender, EventArgs e) => IsDirty = true;

        protected virtual void OnSelectedItemChanged() => SelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected virtual void OnItemAction() => SelectedItemExecuted?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected override void CreateChildControls()
        {
            
        }

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

        public void ScrollToSelectedItem()
        {
            if (IsScrollBarVisible)
            {
                if (selectedIndex < VisibleItemsMax)
                    ScrollBar.Value = 0;
                else if (SelectedIndex > Items.Count - VisibleItemsTotal)
                    ScrollBar.Value = ScrollBar.Maximum;
                else
                    ScrollBar.Value = selectedIndex - VisibleItemsTotal;
            }
        }

        protected override void OnThemeChanged()
        {
            if (ScrollBar == null) return;

            if (ActiveTheme is ListBoxTheme theme)
            {
                ScrollBar.Theme = theme.ScrollBarTheme;
            }
            else
            {
                ScrollBar.Theme = null;
            }
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

            if (SelectedItem != null && !Items.Contains(selectedItem))
            {
                SelectedItem = null;
            }

            IsDirty = true;
        }

        public override bool ProcessKeyboard(Input.Keyboard info)
        {
            //if (_hasFocus)
            if (info.IsKeyReleased(Keys.Up))
            {
                int index = Items.IndexOf(selectedItem);
                if (selectedItem != null)
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
                int index = Items.IndexOf(selectedItem);
                if (selectedItem != null)
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
                if (selectedItem != null)
                {
                    OnItemAction();
                }

                return true;
            }

            return false;
        }

        protected override void OnMouseIn(ControlMouseState state)
        {
            base.OnMouseIn(state);

            int rowOffset = ((ListBoxTheme)ActiveTheme).DrawBorder ? 1 : 0;
            int rowOffsetReverse = ((ListBoxTheme)ActiveTheme).DrawBorder ? 0 : 1;
            int columnOffsetEnd = IsScrollBarVisible || !((ListBoxTheme)ActiveTheme).DrawBorder ? 1 : 0;

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

        protected override void OnLeftMouseClicked(ControlMouseState state)
        {
            base.OnLeftMouseClicked(state);

            DateTime click = DateTime.Now;
            bool doubleClicked = (click - leftMouseLastClick).TotalSeconds <= 0.5;
            leftMouseLastClick = click;

            int rowOffset = ((ListBoxTheme)ActiveTheme).DrawBorder ? 1 : 0;
            int rowOffsetReverse = ((ListBoxTheme)ActiveTheme).DrawBorder ? 0 : 1;
            int columnOffsetEnd = IsScrollBarVisible || !((ListBoxTheme)ActiveTheme).DrawBorder ? 1 : 0;

            Point mouseControlPosition = state.MousePosition;

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < Width - columnOffsetEnd)
            {
                object oldItem = selectedItem;
                bool noItem = false;

                if (IsScrollBarVisible)
                {
                    selectedIndex = mouseControlPosition.Y - rowOffset + ScrollBar.Value;
                    SelectedItem = Items[selectedIndex];
                }
                else if (mouseControlPosition.Y <= Items.Count - rowOffsetReverse)
                {
                    selectedIndex = mouseControlPosition.Y - rowOffset;
                    SelectedItem = Items[selectedIndex];
                }
                else
                {
                    noItem = true;
                }

                if (!noItem && (SingleClickItemExecute || (doubleClicked && oldItem == SelectedItem)))
                {
                    leftMouseLastClick = DateTime.MinValue;
                    OnItemAction();
                }
            }
        }

        [OnSerializing]
        private void BeforeSerializing(StreamingContext context)
        {
            if (selectedItem != null)
            {
                selectedIndex = Items.IndexOf(selectedItem);
            }
            else
            {
                selectedIndex = -1;
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            initialized = true;
            SetupScrollBar(_serializedScrollOrientation, _serializedScrollSizeValue, _serializedScrollPosition);

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (selectedIndex != -1)
            {
                SelectedItem = Items[selectedIndex];
            }

            DetermineState();
            IsDirty = true;
        }


    }
}
