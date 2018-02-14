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
    public class ListBox : ListBox<ListBoxItem>
    {
        public ListBox(int width, int height) : base(width, height) { }
    }

    [DataContract]
    public class ListBox<TItemContainer> : ControlBase
        where TItemContainer : ListBoxItem, new()
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

        [DataMember(Name="Theme")]
        protected ListBoxTheme theme;
        protected List<TItemContainer> containers;
        protected object selectedItem;
        protected TItemContainer selectedItemContainer;
        [DataMember(Name = "Slider")]
        protected ScrollBar slider;
        protected Point sliderRenderLocation;
        [DataMember(Name = "ShowSlider")]
        protected bool showSlider = false;
        [DataMember(Name = "Border")]
        protected Shapes.Box border;
        protected bool mouseIn = false;
        protected DateTime leftMouseLastClick = DateTime.Now;

        [DataMember(Name = "ScrollBarOffset")]
        protected Point scrollBarOffset = new Point(0, 0);
        [DataMember(Name = "ScrollBarSizeAdjust")]
        protected int scrollBarSizeAdjust = 0;

        public event EventHandler<SelectedItemEventArgs> SelectedItemChanged;
        public event EventHandler<SelectedItemEventArgs> SelectedItemExecuted;

        /// <summary>
        /// When the <see cref="SelectedItem"/> changes, and this property is true, objects will be compared by reference. If false, they will be compared by value.
        /// </summary>
        [DataMember]
        public bool CompareByReference { get; set; }

        /// <summary>
        /// When set to true, does not render the border.
        /// </summary>
        [DataMember]
        public bool HideBorder { get { return hideBorder; } set { hideBorder = value; ShowHideSlider(); Compose(true); } }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ListBoxTheme Theme
        {
            get
            {
                if (theme == null)
                    return Library.Default.ListBoxTheme;
                else
                    return theme;
            }
            set
            {
                theme = value;

                if (theme == null)
                    slider.Theme = Library.Default.ListBoxTheme.ScrollBarTheme;
                else
                    slider.Theme = theme.ScrollBarTheme;

                DetermineAppearance();
                Compose();
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
            get { return selectedItem; }
            set
            {
                // Check if we'll get the same container or not.
                var newContainer = GetContainer(value);
                if (newContainer != selectedItemContainer)
                {
                    if (selectedItemContainer != null)
                        selectedItemContainer.IsSelected = false;

                    selectedItem = value;
                    selectedItemContainer = newContainer;
                    selectedIndex = Items.IndexOf(selectedItem);

                    if (selectedItemContainer != null)
                        selectedItemContainer.IsSelected = true;

                    OnSelectedItemChanged();
                }
            }
        }

        public Point ScrollBarOffset
        {
            get { return scrollBarOffset; }
            set { scrollBarOffset = value; SetupSlider();  }
        }

        public int ScrollBarSizeAdjust
        {
            get { return scrollBarSizeAdjust; }
            set { scrollBarSizeAdjust = value; SetupSlider(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the listbox control.
        /// </summary>
        public ListBox(int width, int height): base(width, height)
        {
            initialized = true;
            containers = new List<TItemContainer>();

            border = Shapes.Box.GetDefaultBox();
            border.Fill = true;

            SetupSlider();

            Items = new ObservableCollection<object>();

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            DetermineAppearance();
        }
        #endregion

        protected override void OnParentChanged()
        {
            slider.Parent = this.Parent;
        }

        void _slider_ValueChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }

        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new SelectedItemEventArgs(selectedItem));
        }

        protected virtual void OnItemAction()
        {
            if (SelectedItemExecuted != null)
                SelectedItemExecuted(this, new SelectedItemEventArgs(selectedItem));
        }

        public override void DetermineAppearance()
        {
            
        }

        protected override void OnPositionChanged()
        {
            slider.Position = new Point(this.Position.X + sliderRenderLocation.X, this.Position.Y + sliderRenderLocation.Y);
        }

        protected void SetupSlider()
        {
            if (initialized)
            {
                //_slider.Width, height < 3 ? 3 : height - _scrollBarSizeAdjust
                slider = ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, Height);
                slider.ValueChanged += new EventHandler(_slider_ValueChanged);
                slider.IsVisible = false;
                slider.Theme = this.Theme.ScrollBarTheme;
                sliderRenderLocation = new Point(Width - 1 + scrollBarOffset.X, 0 + scrollBarOffset.Y);
                slider.Position = new Point(position.X + sliderRenderLocation.X, position.Y + sliderRenderLocation.Y);
                border.Width = Width;
                border.Height = Height;

                Compose();
            }
        }
        
        public TItemContainer GetContainer(object item)
        {
            int index = -1;

            for (int i = 0; i < Items.Count; i++)
            {
                if (CompareByReference)
                {
                    if (object.ReferenceEquals(Items[i], item))
                    {
                        index = i;
                        break;
                    }
                }
                else
                {
                    if (object.Equals(Items[i], item))
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
                if (containers.Count - 1 >= index)
                    return containers[index];

            return null;
        }

        public void SetContainer(object item, TItemContainer container)
        {
            int index = -1;

            for (int i = 0; i < Items.Count; i++)
            {
                if (object.ReferenceEquals(Items[i], item))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
                if (containers.Count - 1 >= index)
                {
                    var oldContainer = containers[index];
                    oldContainer.PropertyChanged -= ItemContainer_PropertyChanged;

                    container.PropertyChanged += ItemContainer_PropertyChanged;
                    container.Item = item;
                    if (!container.KeepOwnTheme)
                        container.Theme = Theme.Item;
                    container.IsDirty = true;

                    containers[index] = container;

                }
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 1)
                {
                    List<TItemContainer> tempContainers = new List<TItemContainer>(e.NewItems.Count);
                    foreach (var item in e.NewItems)
                    {
                        TItemContainer cont = new TItemContainer();
                        cont.PropertyChanged += ItemContainer_PropertyChanged;
                        cont.Item = item;
                        if (!cont.KeepOwnTheme)
                            cont.Theme = Theme.Item;
                        cont.IsDirty = true;

                        tempContainers.Add(cont);
                    }

                    containers.InsertRange(e.NewStartingIndex, tempContainers);
                }
                else if (e.NewItems.Count == 1)
                {
                    TItemContainer cont = new TItemContainer();
                    cont.PropertyChanged += ItemContainer_PropertyChanged;
                    cont.Item = e.NewItems[0];
                    if (!cont.KeepOwnTheme)
                        cont.Theme = Theme.Item;
                    cont.IsDirty = true;
                    
                    containers.Insert(e.NewStartingIndex, cont);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                var item = containers[e.OldStartingIndex];
                containers.RemoveAt(e.OldStartingIndex);
                containers.Insert(e.NewStartingIndex, item);
                item.IsDirty = true;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    TItemContainer cont = containers[e.OldStartingIndex];
                    cont.PropertyChanged -= ItemContainer_PropertyChanged;
                    containers.Remove(cont);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in containers)
                {
                    item.PropertyChanged -= ItemContainer_PropertyChanged;
                }
                slider.Value = 0;
                containers.Clear();
            }

            if (SelectedItem != null && !Items.Contains(selectedItem))
                SelectedItem = null;

            ShowHideSlider();

            this.IsDirty = true;
        }

        private void ShowHideSlider()
        {
            int heightOffset;
            if (hideBorder)
                heightOffset = 0;
            else
                heightOffset = 2;

            // process the slider
            int sliderItems = containers.Count - (Height - heightOffset);

            if (sliderItems > 0)
            {
                slider.Maximum = sliderItems;
                showSlider = true;
            }
            else
            {
                slider.Maximum = 0;
                showSlider = false;
            }
        }

        void ItemContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
                if (((ListBoxItem)sender).IsDirty)
                    this.IsDirty = true;
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

                        if (index <= slider.Value)
                            slider.Value -= 1;

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

                        if (index + 1 >= slider.Value + (textSurface.Height - 2))
                            slider.Value += 1;

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

        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            base.OnMouseExit(state);

            mouseIn = false;

            // Reset all containers
            foreach (var item in containers)
                item.IsMouseOver = false;
        }

        protected override void OnMouseEnter(Input.MouseConsoleState state)
        {
            base.OnMouseEnter(state);

            mouseIn = true;
        }

        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            base.OnMouseIn(state);

            // Reset all containers
            foreach (var item in containers)
                item.IsMouseOver = false;

            int rowOffset = hideBorder ? 0 : 1;
            int rowOffsetReverse = hideBorder ? 1 : 0;
            int columnOffsetEnd = showSlider || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.textSurface.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.textSurface.Width - columnOffsetEnd)
            {
                if (showSlider)
                {
                    containers[mouseControlPosition.Y - rowOffset + slider.Value].IsMouseOver = true;
                }
                else if (mouseControlPosition.Y <= containers.Count - rowOffsetReverse)
                {
                    containers[mouseControlPosition.Y - rowOffset].IsMouseOver = true;
                }
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
            int columnOffsetEnd = showSlider || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.textSurface.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.textSurface.Width - columnOffsetEnd)
            {
                object oldItem = selectedItem;
                bool noItem = false;

                if (showSlider)
                {
                    selectedIndex = mouseControlPosition.Y - rowOffset + slider.Value;
                    SelectedItem = Items[selectedIndex];
                }
                else if (mouseControlPosition.Y <= containers.Count - rowOffsetReverse)
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

                if (mouseIn)
                {
                    var mouseControlPosition = TransformConsolePositionByControlPosition(state.CellPosition);

                    if (mouseControlPosition.X == this.textSurface.Width - 1 && showSlider)
                    {
                        slider.ProcessMouse(state);
                    }
                }
            }

            return false;
        }

        public override void Compose()
        {
            if (IsDirty)
            {
                int columnOffset;
                int columnEnd;
                int startingRow;
                int endingRow;


                Clear();

                if (!hideBorder)
                {
                    endingRow = Height - 2;
                    startingRow = 1;
                    columnOffset = 1;
                    columnEnd = Width - 2;
                    border.Foreground = this.Theme.Border.Foreground;
                    border.BorderBackground = this.Theme.Border.Background;
                    border.FillColor = this.Theme.Border.Background;
                    border.Draw(this);
                }
                else
                {
                    endingRow = Height;
                    startingRow = 0;
                    columnOffset = 0;
                    columnEnd = Width;
                    this.Fill(this.Theme.Border.Foreground, this.Theme.Border.Background, 0, null);
                }

                int offset = showSlider ? slider.Value : 0;
                for (int i = 0; i < endingRow; i++)
                {
                    if (i + offset < containers.Count)
                        containers[i + offset].Draw(textSurface, new Rectangle(columnOffset, i + startingRow, columnEnd, 1));
                }

                if (showSlider)
                {
                    slider.Compose(true);
                    int y = sliderRenderLocation.Y;

                    for (int ycell = 0; ycell < slider.TextSurface.Height; ycell++)
                    {
                        this.SetGlyph(sliderRenderLocation.X, y, slider[0, ycell].Glyph);
                        this.SetCell(sliderRenderLocation.X, y, slider[0, ycell]);
                        y++;
                    }
                }

                OnComposed?.Invoke(this);

                IsDirty = false;
            }
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
            containers = new List<TItemContainer>();

            foreach (var item in Items)
            {
                TItemContainer cont = new TItemContainer();
                cont.PropertyChanged += ItemContainer_PropertyChanged;
                cont.Item = item;
                if (!cont.KeepOwnTheme)
                    cont.Theme = Theme.Item;
                cont.IsDirty = true;

                containers.Add(cont);
            }

            slider.ValueChanged += new EventHandler(_slider_ValueChanged);
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (selectedIndex != -1)
                SelectedItem = Items[selectedIndex];

            SetupSlider();

            DetermineAppearance();
            Compose(true);
        }


    }

    [DataContract]
    public class ListBoxItem : INotifyPropertyChanged
    {
        protected ThemePartSelected _theme;

        protected bool _isDirty = false;
        protected bool _isSelected = false;
        protected bool _isMouseOver = false;
        [DataMember(Name="Content")]
        protected object _item;

        /// <summary>
        /// The theme of this control.
        /// </summary>
        public ThemePartSelected Theme
        {
            get { return _theme; }
            set { _theme = value; DetermineAppearance(); }
        }

        public ListBoxItem()
        {
            Theme = (ThemePartSelected)Library.Default.ListBoxTheme.Item.Clone();
        }

        protected Cell _currentAppearance = new Cell(Color.White, Color.Black, 0);

        /// <summary>
        /// When set to true, a listbox will not override the theme with its own.
        /// </summary>
        public bool KeepOwnTheme { get; set; }

        public object Item
        {
            get { return _item; }
            set
            {
                object oldItem = _item;
                _item = value;
                OnItemChanged(oldItem, _item);
                OnPropertyChanged("Item");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; DetermineAppearance(); }
        }

        protected virtual void DetermineAppearance()
        {
            Cell currentappearance = _currentAppearance;

            if (_isSelected)
                _currentAppearance = Theme.Selected;

            else if (_isMouseOver)
                _currentAppearance = Theme.MouseOver;

            else
                _currentAppearance = Theme.Normal;

            if (currentappearance != _currentAppearance)
                IsDirty = true;
        }

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set { _isMouseOver = value; DetermineAppearance(); }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; OnPropertyChanged("IsDirty"); }
        }

        public virtual void Draw(ISurface surface, Rectangle area)
        {
            string value = Item.ToString();
            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);
            var editor = new SurfaceEditor(surface);
            editor.Print(area.Left, area.Top, value, _currentAppearance);
            _isDirty = false;
        }

        protected virtual void OnItemChanged(object oldItem, object newItem) { }

#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
#endregion
    }

    [DataContract]
    public class ListBoxItemColor : ListBoxItem
    {
        public override void Draw(ISurface surface, Rectangle area)
        {
            if (Item is Color || Item is Tuple<Color, Color, string>)
            {
                var editor = new SurfaceEditor(surface);
                string value = new string(' ', area.Width - 2);

                Cell cellLook = new Cell();
                _currentAppearance.CopyAppearanceTo(cellLook);

                if (Item is Color)
                {
                    cellLook.Background = (Color)Item;
                    editor.Print(area.Left + 1, area.Top, value, cellLook);
                }
                else
                {
                    cellLook.Foreground = ((Tuple<Color, Color, string>)Item).Item2;
                    cellLook.Background = ((Tuple<Color, Color, string>)Item).Item1;
                    value = ((Tuple<Color, Color, string>)Item).Item3.Align(HorizontalAlignment.Left, area.Width - 2);
                    editor.Print(area.Left + 1, area.Top, value, cellLook);
                }

                editor.Print(area.Left, area.Top, " ", _currentAppearance);
                editor.Print(area.Left + area.Width - 1, area.Top, " ", _currentAppearance);

                if (IsSelected)
                {
                    editor.SetGlyph(area.Left, area.Top, 16);
                    editor.SetGlyph(area.Left + area.Width - 1, area.Top, 17);
                }

                IsDirty = false;
            }
            else
                base.Draw(surface, area);
        }
    }
}
