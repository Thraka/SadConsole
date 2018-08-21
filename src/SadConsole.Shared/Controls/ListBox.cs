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

        [DataMember(Name="Theme")]
        protected ListBoxTheme _theme;
        protected object selectedItem;
        [DataMember(Name = "ShowSlider")]
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
        public bool IsSliderVisible { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        [DataMember(Name = "Slider")]
        public ScrollBar Slider { get; private set; }

        /// <summary>
        /// Used in rendering.
        /// </summary>
        public Point SliderRenderLocation { get; private set; }

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
                ShowHideSlider();
                IsDirty = true;
            }
        }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ListBoxTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _theme.Attached(this);
                DetermineState();
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
            set { scrollBarOffset = value; SetupSlider();  }
        }

        public int ScrollBarSizeAdjust
        {
            get => scrollBarSizeAdjust;
            set { scrollBarSizeAdjust = value; SetupSlider(); }
        }

        #region Constructors
        /// <summary>
        /// Creates a new instance of the listbox control.
        /// </summary>
        public ListBox(int width, int height): base(width, height)
        {
            initialized = true;
            SliderRenderLocation = new Point(width - 1, 0);
            SetupSlider();

            Items = new ObservableCollection<object>();

            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
        }
        #endregion

        protected override void OnParentChanged() => Slider.Parent = this.Parent;

        private void _slider_ValueChanged(object sender, EventArgs e) => this.IsDirty = true;

        protected virtual void OnSelectedItemChanged() => SelectedItemChanged?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected virtual void OnItemAction() => SelectedItemExecuted?.Invoke(this, new SelectedItemEventArgs(selectedItem));

        protected override void OnPositionChanged() => Slider.Position = Position + new Point(Width - 1, 0);

        protected void SetupSlider()
        {
            if (!initialized) return;
            Theme = (ListBoxTheme)Library.Default.ListBoxTheme.Clone();
            //_slider.Width, height < 3 ? 3 : height - _scrollBarSizeAdjust
            Slider = ScrollBar.Create(Orientation.Vertical, Height);
            Slider.ValueChanged += new EventHandler(_slider_ValueChanged);
            Slider.IsVisible = false;
            Slider.Theme = this.Theme.ScrollBarTheme;
            Slider.Position = Position + new Point(Width - 1, 0);

            DetermineState();
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
                Slider.Value = 0;
            }

            if (SelectedItem != null && !Items.Contains(selectedItem))
                SelectedItem = null;

            ShowHideSlider();

            this.IsDirty = true;
        }

        private void ShowHideSlider()
        {
            var heightOffset = hideBorder ? 0 : 2;

            // process the slider
            var sliderItems = Items.Count - (Height - heightOffset);

            if (sliderItems > 0)
            {
                Slider.Maximum = sliderItems;
                IsSliderVisible = true;
            }
            else
            {
                Slider.Maximum = 0;
                IsSliderVisible = false;
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

                        if (index <= Slider.Value)
                            Slider.Value -= 1;

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

                        if (index + 1 >= Slider.Value + (Height - 2))
                            Slider.Value += 1;

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
            var columnOffsetEnd = IsSliderVisible || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.Width - columnOffsetEnd)
            {
                if (IsSliderVisible)
                {
                    RelativeIndexMouseOver = mouseControlPosition.Y - rowOffset + Slider.Value;
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
            int columnOffsetEnd = IsSliderVisible || !hideBorder ? 1 : 0;

            Point mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

            if (mouseControlPosition.Y >= rowOffset && mouseControlPosition.Y < this.Height - rowOffset &&
                mouseControlPosition.X >= rowOffset && mouseControlPosition.X < this.Width - columnOffsetEnd)
            {
                object oldItem = selectedItem;
                bool noItem = false;

                if (IsSliderVisible)
                {
                    selectedIndex = mouseControlPosition.Y - rowOffset + Slider.Value;
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

                    if (mouseControlPosition.X == SliderRenderLocation.X && IsSliderVisible)
                    {
                        Slider.ProcessMouse(state);
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

        public override void Update(TimeSpan time)
        {
            Theme.UpdateAndDraw(this, time);
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            initialized = true;

            Slider.ValueChanged += new EventHandler(_slider_ValueChanged);
            Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

            if (selectedIndex != -1)
                SelectedItem = Items[selectedIndex];

            SetupSlider();

            DetermineState();
            IsDirty = true;
        }


    }
}
