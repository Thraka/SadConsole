using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using SadConsole.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using SadConsole.Input;

namespace SadConsole.Controls
{
    [DataContract]
    public class ListBox : ControlBase
    {
        public class SelectedItemEventArgs : EventArgs
        {
            public object Item;

            public SelectedItemEventArgs(object item)
            {
                Item = item;
            }
        }

        protected bool hideBorder;
        protected bool initialized;
        [DataMember(Name="SelectedIndex")]
        protected int selectedIndex;

        protected object selectedItem;
        //[DataMember(Name = "BorderLines")]
        //protected int[] borderLineStyle;
        protected DateTime leftMouseLastClick = DateTime.Now;

        [DataMember(Name = "ScrollBarOffset")]
        protected Point scrollBarOffset = new Point(0, 0);
        [DataMember(Name = "ScrollBarSizeAdjust")]
        protected int scrollBarSizeAdjust = 0;

        public event EventHandler<SelectedItemEventArgs> SelectedItemChanged;
        public event EventHandler<SelectedItemEventArgs> SelectedItemExecuted;

        /// <summary>
        /// Used in rendering.
        /// </summary>
        public bool IsScrollBarVisible { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        [DataMember(Name = "ScrollBar")]
        public ScrollBar ScrollBar { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        public Point ScrollBarRenderLocation { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        public int RelativeIndexMouseOver { get; private set; }

        /// <summary>
        /// When the <see cref="SelectedItem"/> changes, and this property is true, objects will be compared by reference. If false, they will be compared by value.
        /// </summary>
        [DataMember]
        public bool CompareByReference { get; set; }

        /// <summary>
        /// When set to true, does not render the border.
        /// </summary>
        [DataMember]
        public bool HideBorder
        {
            get => hideBorder;
            set
            {
                hideBorder = value;
                ShowHideScrollBar();
                IsDirty = true;
            }
        }
        

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
                var index = Items.IndexOf(value);

                if (index == -1)
                    throw new ArgumentOutOfRangeException("Item does not exist in collection.");

                selectedIndex = index;
                selectedItem = Items[index];
                IsDirty = true;
                OnSelectedItemChanged();
            }
        }

        public Point ScrollBarOffset
        {
            get => scrollBarOffset;
            set { scrollBarOffset = value; SetupScrollBar();  }
        }

        public int ScrollBarSizeAdjust
        {
            get => scrollBarSizeAdjust;
            set { scrollBarSizeAdjust = value; SetupScrollBar(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the listbox control.
        /// </summary>
        public ListBox(int width, int height): base(width, height)
        {
            initialized = true;
            ScrollBarRenderLocation = new Point(width - 1, 0);
            SetupScrollBar();

            Items = new ObservableCollection<object>();

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
        }
        #endregion

        protected override void OnParentChanged() => ScrollBar.Parent = this.Parent;

        private void _scrollbar_ValueChanged(object sender, EventArgs e) => this.IsDirty = true;

        protected virtual void OnSelectedItemChanged() => SelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected virtual void OnItemAction() => SelectedItemExecuted?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected override void OnPositionChanged() => ScrollBar.Position = Position + new Point(Width - 1, 0);

        protected void SetupScrollBar()
        {
            if (!initialized) return;
            //_scrollBar.Width, height < 3 ? 3 : height - _scrollBarSizeAdjust
            ScrollBar = new ScrollBar(Orientation.Vertical, Height);
            ScrollBar.ValueChanged += new EventHandler(_scrollbar_ValueChanged);
            ScrollBar.IsVisible = false;
            ScrollBar.Position = Position + new Point(Width - 1, 0);
            
            DetermineState();
        }

        protected override void OnThemeChanged()
        {
            if (ActiveTheme is ListBoxTheme theme)
                ScrollBar.Theme = theme.ScrollBarTheme;
            else
                ScrollBar.Theme = null;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                SelectedItem = null;

            ShowHideScrollBar();

            this.IsDirty = true;
        }

        private void ShowHideScrollBar()
        {
            var heightOffset = hideBorder ? 0 : 2;

            // process the scroll bar
            var scrollbarItems = Items.Count - (Height - heightOffset);

            if (scrollbarItems > 0)
            {
                ScrollBar.Maximum = scrollbarItems;
                IsScrollBarVisible = true;
            }
            else
            {
                ScrollBar.Maximum = 0;
                IsScrollBarVisible = false;
            }
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
                            ScrollBar.Value -= 1;

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
                            ScrollBar.Value += 1;

                    }
                }
                else if (Items.Count != 0)
                    SelectedItem = Items[0];
                return true;
            }
            else if (info.IsKeyReleased(Keys.Enter))
            {
                if (selectedItem != null)
                    OnItemAction();

                return true;
            }

            return false;
        }

        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            base.OnMouseIn(state);

            var rowOffset = hideBorder ? 0 : 1;
            var rowOffsetReverse = hideBorder ? 1 : 0;
            var columnOffsetEnd = IsScrollBarVisible || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.Width - columnOffsetEnd)
            {
                if (IsScrollBarVisible)
                {
                    RelativeIndexMouseOver = mouseControlPosition.Y - rowOffset + ScrollBar.Value;
                }
                else if (mouseControlPosition.Y <= Items.Count - rowOffsetReverse)
                {
                    RelativeIndexMouseOver = mouseControlPosition.Y - rowOffset;
                }
            }
            else
            {
                RelativeIndexMouseOver = -1;
            }
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            DateTime click = DateTime.Now;
            bool doubleClicked = (click - leftMouseLastClick).TotalSeconds <= 0.5;
            leftMouseLastClick = click;

            int rowOffset = hideBorder ? 0 : 1;
            int rowOffsetReverse = hideBorder ? 1 : 0;
            int columnOffsetEnd = IsScrollBarVisible || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.Width - columnOffsetEnd)
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
                    noItem = true;

                if (doubleClicked && oldItem == SelectedItem && !noItem)
                {
                    leftMouseLastClick = DateTime.MinValue;
                    OnItemAction();
                }
            }
        }
        public override bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (isEnabled)
            {
                base.ProcessMouse(state);

                if (isMouseOver)
                {
                    var mouseControlPosition = TransformConsolePositionByControlPosition(state.CellPosition);

                    if (mouseControlPosition.X == ScrollBarRenderLocation.X && IsScrollBarVisible)
                    {
                        ScrollBar.ProcessMouse(state);
                    }
                }
            }

            return false;
        }

        [OnSerializing]
        private void BeforeSerializing(StreamingContext context)
        {
            if (selectedItem != null)
                selectedIndex = Items.IndexOf(selectedItem);
            else
                selectedIndex = -1;
        }
        
        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            initialized = true;

            ScrollBar.ValueChanged += new EventHandler(_scrollbar_ValueChanged);
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (selectedIndex != -1)
                SelectedItem = Items[selectedIndex];

            SetupScrollBar();

            DetermineState();
            IsDirty = true;
        }


    }
}
