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
        protected bool _hideBorder;
        protected bool _initialized;
        [DataMember(Name="SelectedIndex")]
        protected int _selectedIndex;

        [DataMember(Name="Theme")]
        protected ListBoxTheme _theme;
        protected List<TItemContainer> _containers;
        protected object _selectedItem;
        protected TItemContainer _selectedItemContainer;
        [DataMember(Name = "Slider")]
        protected ScrollBar _slider;
        protected Point _sliderRenderLocation;
        [DataMember(Name = "ShowSlider")]
        protected bool _showSlider = false;
        [DataMember(Name = "Border")]
        protected Shapes.Box _border;
        protected bool _mouseIn = false;
        protected DateTime _leftMouseLastClick = DateTime.Now;

        [DataMember(Name = "ScrollBarOffset")]
        protected Point _scrollBarOffset = new Point(0, 0);
        [DataMember(Name = "ScrollBarSizeAdjust")]
        protected int _scrollBarSizeAdjust = 0;

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
        public bool HideBorder { get { return _hideBorder; } set { _hideBorder = value; ShowHideSlider(); IsDirty = true; } }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ListBoxTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Library.Default.ListBoxTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;

                if (_theme == null)
                    _slider.Theme = Library.Default.ListBoxTheme.ScrollBarTheme;
                else
                    _slider.Theme = _theme.ScrollBarTheme;
            }
        }

        [DataMember]
        public ObservableCollection<object> Items { get; private set; }

        public int SelectedIndex
        {
            get
            {
                int index = -1;
                if (_selectedItem != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (CompareByReference)
                        {
                            if (object.ReferenceEquals(Items[i], _selectedItem))
                            {
                                index = i;
                                break;
                            }
                        }
                        else
                        {
                            if (object.Equals(Items[i], _selectedItem))
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
            get { return _selectedItem; }
            set
            {
                // Check if we'll get the same container or not.
                var newContainer = GetContainer(value);
                if (newContainer != _selectedItemContainer)
                {
                    if (_selectedItemContainer != null)
                        _selectedItemContainer.IsSelected = false;

                    _selectedItem = value;
                    _selectedItemContainer = newContainer;
                    _selectedIndex = Items.IndexOf(_selectedItem);

                    if (_selectedItemContainer != null)
                        _selectedItemContainer.IsSelected = true;

                    OnSelectedItemChanged();
                }
            }
        }

        public Point ScrollBarOffset
        {
            get { return _scrollBarOffset; }
            set { _scrollBarOffset = value; SetupSlider();  }
        }

        public int ScrollBarSizeAdjust
        {
            get { return _scrollBarSizeAdjust; }
            set { _scrollBarSizeAdjust = value; SetupSlider(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the listbox control.
        /// </summary>
        public ListBox(int width, int height): base(width, height)
        {
            _initialized = true;
            _containers = new List<TItemContainer>();

            _border = Shapes.Box.GetDefaultBox();
            _border.Fill = true;

            SetupSlider();

            Items = new ObservableCollection<object>();

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            DetermineAppearance();
        }
        #endregion

        protected override void OnParentChanged()
        {
            _slider.Parent = this.Parent;
        }

        void _slider_ValueChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }

        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new SelectedItemEventArgs(_selectedItem));
        }

        protected virtual void OnItemAction()
        {
            if (SelectedItemExecuted != null)
                SelectedItemExecuted(this, new SelectedItemEventArgs(_selectedItem));
        }

        public override void DetermineAppearance()
        {
            
        }

        protected override void OnPositionChanged()
        {
            _slider.Position = new Point(this.Position.X + _sliderRenderLocation.X, this.Position.Y + _sliderRenderLocation.Y);
        }

        protected void SetupSlider()
        {
            if (_initialized)
            {
                //_slider.Width, height < 3 ? 3 : height - _scrollBarSizeAdjust
                _slider = ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, Height);
                _slider.ValueChanged += new EventHandler(_slider_ValueChanged);
                _slider.IsVisible = false;
                _slider.Theme = this.Theme.ScrollBarTheme;
                _sliderRenderLocation = new Point(Width - 1 + _scrollBarOffset.X, 0 + _scrollBarOffset.Y);
                _slider.Position = new Point(position.X + _sliderRenderLocation.X, position.Y + _sliderRenderLocation.Y);
                _border.Width = Width;
                _border.Height = Height;

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
                if (_containers.Count - 1 >= index)
                    return _containers[index];

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
                if (_containers.Count - 1 >= index)
                {
                    var oldContainer = _containers[index];
                    oldContainer.PropertyChanged -= ItemContainer_PropertyChanged;

                    container.PropertyChanged += ItemContainer_PropertyChanged;
                    container.Item = item;
                    if (!container.KeepOwnTheme)
                        container.Theme = Theme.Item;
                    container.IsDirty = true;

                    _containers[index] = container;

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

                    _containers.InsertRange(e.NewStartingIndex, tempContainers);
                }
                else if (e.NewItems.Count == 1)
                {
                    TItemContainer cont = new TItemContainer();
                    cont.PropertyChanged += ItemContainer_PropertyChanged;
                    cont.Item = e.NewItems[0];
                    if (!cont.KeepOwnTheme)
                        cont.Theme = Theme.Item;
                    cont.IsDirty = true;
                    
                    _containers.Insert(e.NewStartingIndex, cont);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                var item = _containers[e.OldStartingIndex];
                _containers.RemoveAt(e.OldStartingIndex);
                _containers.Insert(e.NewStartingIndex, item);
                item.IsDirty = true;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    TItemContainer cont = _containers[e.OldStartingIndex];
                    cont.PropertyChanged -= ItemContainer_PropertyChanged;
                    _containers.Remove(cont);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in _containers)
                {
                    item.PropertyChanged -= ItemContainer_PropertyChanged;
                }
                _slider.Value = 0;
                _containers.Clear();
            }

            if (SelectedItem != null && !Items.Contains(_selectedItem))
                SelectedItem = null;

            ShowHideSlider();

            this.IsDirty = true;
        }

        private void ShowHideSlider()
        {
            int heightOffset;
            if (HideBorder)
                heightOffset = 0;
            else
                heightOffset = 2;

            // process the slider
            int sliderItems = _containers.Count - (Height - heightOffset);

            if (sliderItems > 0)
            {
                _slider.Maximum = sliderItems;
                _showSlider = true;
            }
            else
            {
                _slider.Maximum = 0;
                _showSlider = false;
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
                int index = Items.IndexOf(_selectedItem);
                if (_selectedItem != null)
                {
                    if (index != 0)
                    {
                        SelectedItem = Items[index - 1];

                        if (index <= _slider.Value)
                            _slider.Value -= 1;

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

                        if (index + 1 >= _slider.Value + (textSurface.Height - 2))
                            _slider.Value += 1;

                    }
                }
                else if (Items.Count != 0)
                    SelectedItem = Items[0];
                return true;
            }
            else if (info.IsKeyReleased(Keys.Enter))
            {
                if (_selectedItem != null)
                    OnItemAction();

                return true;
            }

            return false;
        }

        protected override void OnMouseExit(Input.MouseConsoleState state)
        {
            base.OnMouseExit(state);

            _mouseIn = false;

            // Reset all containers
            foreach (var item in _containers)
                item.IsMouseOver = false;
        }

        protected override void OnMouseEnter(Input.MouseConsoleState state)
        {
            base.OnMouseEnter(state);

            _mouseIn = true;
        }

        protected override void OnMouseIn(Input.MouseConsoleState state)
        {
            base.OnMouseIn(state);

            // Reset all containers
            foreach (var item in _containers)
                item.IsMouseOver = false;

            int rowOffset = HideBorder ? 0 : 1;
            int rowOffsetReverse = HideBorder ? 1 : 0;
            int columnOffsetEnd = _showSlider || !HideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.textSurface.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.textSurface.Width - columnOffsetEnd)
            {
                if (_showSlider)
                {
                    _containers[mouseControlPosition.Y - rowOffset + _slider.Value].IsMouseOver = true;
                }
                else if (mouseControlPosition.Y <= _containers.Count - rowOffsetReverse)
                {
                    _containers[mouseControlPosition.Y - rowOffset].IsMouseOver = true;
                }
            }
        }

        protected override void OnLeftMouseClicked(Input.MouseConsoleState state)
        {
            base.OnLeftMouseClicked(state);

            DateTime click = DateTime.Now;
            bool doubleClicked = (click - _leftMouseLastClick).TotalSeconds <= 0.5;
            _leftMouseLastClick = click;

            int rowOffset = HideBorder ? 0 : 1;
            int rowOffsetReverse = HideBorder ? 1 : 0;
            int columnOffsetEnd = _showSlider || !HideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.textSurface.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.textSurface.Width - columnOffsetEnd)
            {
                object oldItem = _selectedItem;
                bool noItem = false;

                if (_showSlider)
                {
                    _selectedIndex = mouseControlPosition.Y - rowOffset + _slider.Value;
                    SelectedItem = Items[_selectedIndex];
                }
                else if (mouseControlPosition.Y <= _containers.Count - rowOffsetReverse)
                {
                    _selectedIndex = mouseControlPosition.Y - rowOffset;
                    SelectedItem = Items[_selectedIndex];
                }
                else
                    noItem = true;

                if (doubleClicked && oldItem == SelectedItem && !noItem)
                {
                    _leftMouseLastClick = DateTime.MinValue;
                    OnItemAction();
                }
            }
        }
        public override bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (isEnabled)
            {
                base.ProcessMouse(state);

                if (_mouseIn)
                {
                    var mouseControlPosition = TransformConsolePositionByControlPosition(state.CellPosition);

                    if (mouseControlPosition.X == this.textSurface.Width - 1 && _showSlider)
                    {
                        _slider.ProcessMouse(state);
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

                if (!HideBorder)
                {
                    endingRow = Height - 2;
                    startingRow = 1;
                    columnOffset = 1;
                    columnEnd = Width - 2;
                    _border.Foreground = this.Theme.Border.Foreground;
                    _border.BorderBackground = this.Theme.Border.Background;
                    _border.FillColor = this.Theme.Border.Background;
                    _border.Draw(this);
                }
                else
                {
                    endingRow = Height;
                    startingRow = 0;
                    columnOffset = 0;
                    columnEnd = Width;
                    this.Fill(this.Theme.Border.Foreground, this.Theme.Border.Background, 0, null);
                }

                int offset = _showSlider ? _slider.Value : 0;
                for (int i = 0; i < endingRow; i++)
                {
                    if (i + offset < _containers.Count)
                        _containers[i + offset].Draw(textSurface, new Rectangle(columnOffset, i + startingRow, columnEnd, 1));
                }

                if (_showSlider)
                {
                    _slider.Compose(true);
                    int y = _sliderRenderLocation.Y;

                    for (int ycell = 0; ycell < _slider.TextSurface.Height; ycell++)
                    {
                        this.SetGlyph(_sliderRenderLocation.X, y, _slider[0, ycell].Glyph);
                        this.SetCell(_sliderRenderLocation.X, y, _slider[0, ycell]);
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
            if (_selectedItem != null)
                _selectedIndex = Items.IndexOf(_selectedItem);
            else
                _selectedIndex = -1;
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            _initialized = true;
            _containers = new List<TItemContainer>();

            foreach (var item in Items)
            {
                TItemContainer cont = new TItemContainer();
                cont.PropertyChanged += ItemContainer_PropertyChanged;
                cont.Item = item;
                if (!cont.KeepOwnTheme)
                    cont.Theme = Theme.Item;
                cont.IsDirty = true;

                _containers.Add(cont);
            }

            _slider.ValueChanged += new EventHandler(_slider_ValueChanged);
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (_selectedIndex != -1)
                SelectedItem = Items[_selectedIndex];

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
