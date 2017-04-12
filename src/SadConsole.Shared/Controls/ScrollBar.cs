using Microsoft.Xna.Framework;

using SadConsole.Themes;
using System;
using System.Runtime.Serialization;

namespace SadConsole.Controls
{
    /// <summary>
    /// Represents a scrollbar control.
    /// </summary>
    [DataContract]
    public class ScrollBar : ControlBase
    {
        public event EventHandler ValueChanged;

        private bool _initialized;

        [DataMember(Name = "Theme")]
        protected ScrollBarTheme _theme;
        protected Cell _currentAppearanceEnds;
        protected Cell _currentAppearanceBar;
        protected Cell _currentAppearanceSlider;

        protected int _topOrLeftCharacter;
        protected int _bottomOrRightCharacter;
        protected int _sliderCharacter;
        protected int _sliderBarCharacter;

        [DataMember(Name="Orientation")]
        protected System.Windows.Controls.Orientation _barOrientation;
        [DataMember(Name = "Value")]
        protected int _value;
        [DataMember(Name = "Minimum")]
        protected int _minValue = 0;
        [DataMember(Name="Maximum")]
        protected int _maxValue = 1;
        protected int _sliderBarSize;
        protected int _currentSliderPosition;
        protected int _valueStep = 1;

        protected int[] _sliderPositionValues;

        /// <summary>
        /// Gets or sets the value of the scrollbar between the minimum and maximum values.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;

                    if (_value < 0)
                        _value = 0;
                    else if (_value > _maxValue)
                        _value = _maxValue;

                    if (_value == 0)
                        _currentSliderPosition = 0;
                    else if (_value == _maxValue)
                        _currentSliderPosition = _sliderBarSize - 1;
                    else
                    {
                        // Find which slot is < value where the slot after is > value
                        for (int i = 1; i < _sliderBarSize - 1; i++)
                        {
                            if (_sliderPositionValues[i] == _value)
                            {
                                _currentSliderPosition = i;
                                break;
                            }
                            if (_sliderPositionValues[i] > _value && _sliderPositionValues[i - 1] < _value && _sliderPositionValues[i] != -1)
                            {
                                _currentSliderPosition = i;
                                break;
                            }
                        }
                    }

                    this.IsDirty = true;

                    if (ValueChanged != null)
                        ValueChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for the scrollbar.
        /// </summary>
        public int Maximum
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                if (_maxValue <= 0)
                    _maxValue = 1;

                DetermineSliderPositions();
                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the amount of values to add or substract to the <see cref="Value"/> when the up or down arrows are used.
        /// </summary>
        [DataMember]
        public int Step
        {
            get { return _valueStep; }
            set { _valueStep = value; }
        }

        /// <summary>
        /// The theme of this control. If the theme is not explicitly set, the theme is taken from the library.
        /// </summary>
        public virtual ScrollBarTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Library.Default.ScrollBarTheme;
                else
                    return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        /// <summary>
        /// Gets or sets the character used on the top or left of the control depending on the orientation of the control when it was created.
        /// </summary>
        [DataMember]
        public int TopOrLeftCharacter
        {
            get { return _topOrLeftCharacter; }
            set { _topOrLeftCharacter = value; this.IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the character used on the bottom or right of the control depending on the orientation of the control when it was created.
        /// </summary>
        [DataMember]
        public int BottomOrRightCharacter
        {
            get { return _bottomOrRightCharacter; }
            set { _bottomOrRightCharacter = value; this.IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the character displayed for the slider.
        /// </summary>
        [DataMember]
        public int SliderCharacter
        {
            get { return _sliderCharacter; }
            set { _sliderCharacter = value; this.IsDirty = true; }
        }

        /// <summary>
        /// Gets or sets the character displayed for the slider bar.
        /// </summary>
        [DataMember]
        public int SliderBarCharacter
        {
            get { return _sliderBarCharacter; }
            set { _sliderBarCharacter = value; this.IsDirty = true; }
        }

        public static ScrollBar Create(System.Windows.Controls.Orientation orientation, int size)
        {
            if (size <= 2)
                throw new Exception("The scroll bar must be 4 or more in size.");

            if (orientation == System.Windows.Controls.Orientation.Vertical)
                return new ScrollBar(orientation, 1, size);
            else
                return new ScrollBar(orientation, size, 1);
        }


        private ScrollBar(System.Windows.Controls.Orientation orientation, int width, int height): base(width, height)
        {
            _initialized = true;
            _barOrientation = orientation;

            _sliderCharacter = 219;

            if (orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                _sliderBarCharacter = 176;
                _topOrLeftCharacter = 17;
                _bottomOrRightCharacter = 16;
            }
            else
            {
                _sliderBarCharacter = 176;
                _topOrLeftCharacter = 30;
                _bottomOrRightCharacter = 31;
            }

            if (width > height)
                _sliderBarSize = width - 2;
            else
                _sliderBarSize = height - 2;

            _sliderPositionValues = new int[_sliderBarSize];
            DetermineSliderPositions();
        }

        public override void DetermineAppearance()
        {
            Cell currentappearanceEnds = _currentAppearanceEnds;
            Cell currentappearanceBar = _currentAppearanceBar;
            Cell currentappearanceSlider = _currentAppearanceSlider;

            if (!isEnabled)
            {
                _currentAppearanceEnds = Theme.Ends.Disabled;
                _currentAppearanceBar = Theme.Bar.Disabled;
                _currentAppearanceSlider = Theme.Slider.Disabled;
            }
            else if (isMouseOver)
            {
                _currentAppearanceEnds = Theme.Ends.MouseOver;
                _currentAppearanceBar = Theme.Bar.MouseOver;
                _currentAppearanceSlider = Theme.Slider.MouseOver;
            }
            else if (!isMouseOver)
            {
                _currentAppearanceEnds = Theme.Ends.Normal;
                _currentAppearanceBar = Theme.Bar.Normal;
                _currentAppearanceSlider = Theme.Slider.Normal;
            }

            if (currentappearanceEnds != _currentAppearanceEnds ||
                currentappearanceBar != _currentAppearanceBar ||
                currentappearanceSlider != _currentAppearanceSlider)

                this.IsDirty = true;
        }
        

        public override void Compose()
        {
            if (IsDirty)
            {
                if (_barOrientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    this.SetCell(0, 0, Theme.Ends.Normal);
                    this.SetGlyph(0, 0, _topOrLeftCharacter);

                    this.SetCell(textSurface.Width - 1, 0, Theme.Ends.Normal);
                    this.SetGlyph(textSurface.Width - 1, 0, _bottomOrRightCharacter);

                    for (int i = 1; i <= _sliderBarSize; i++)
                    {
                        this.SetCell(i, 0, Theme.Bar.Normal);
                        this.SetGlyph(i, 0, _sliderBarCharacter);
                    }

                    if (_value >= _minValue && _value <= _maxValue && _minValue != _maxValue)
                    {
                        if (IsEnabled)
                        {
                            this.SetCell(1 + _currentSliderPosition, 0, Theme.Slider.Normal);
                            this.SetGlyph(1 + _currentSliderPosition, 0, _sliderCharacter);
                        }
                    }
                }
                else
                {
                    this.SetCell(0, 0, Theme.Ends.Normal);
                    this.SetGlyph(0, 0, _topOrLeftCharacter);

                    this.SetCell(0, textSurface.Height - 1, Theme.Ends.Normal);
                    this.SetGlyph(0, textSurface.Height - 1, _bottomOrRightCharacter);

                    for (int i = 0; i < _sliderBarSize; i++)
                    {
                        this.SetCell(0, i + 1, Theme.Bar.Normal);
                        this.SetGlyph(0, i + 1, _sliderBarCharacter);
                    }

                    if (_value >= _minValue && _value <= _maxValue && _minValue != _maxValue)
                    {
                        if (IsEnabled)
                        {
                            this.SetCell(0, 1 + _currentSliderPosition, Theme.Slider.Normal);
                            this.SetGlyph(0, 1 + _currentSliderPosition, _sliderCharacter);
                        }
                    }

                }
                OnComposed?.Invoke(this);

                this.IsDirty = false;
            }
        }

        // Locking the mouse to this control is actually locking the parent console to the engine, and then
        // letting the controls console know that this control wants exclusive focus until mouse is unclicked.

        public override bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (IsEnabled)
            {
                base.ProcessMouse(state);


                var mouseControlPosition = new Point(state.CellPosition.X - this.Position.X, state.CellPosition.Y - this.Position.Y);

                // This becomes the active mouse subject when the bar is being dragged.
                if (Parent.CapturedControl == null)
                {
                    if (state.CellPosition.X >= this.Position.X && state.CellPosition.X < this.Position.X + textSurface.Width &&
                        state.CellPosition.Y >= this.Position.Y && state.CellPosition.Y < this.Position.Y + textSurface.Height)
                    {

                        if (state.Mouse.LeftClicked)
                        {
                            if (_barOrientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                if (mouseControlPosition.X == 0)
                                    Value -= Step;
                                if (mouseControlPosition.X == textSurface.Width - 1)
                                    Value += Step;
                            }
                            else
                            {
                                if (mouseControlPosition.Y == 0)
                                    Value -= Step;
                                if (mouseControlPosition.Y == textSurface.Height - 1)
                                    Value += Step;
                            }

                            Parent.FocusedControl = this;
                        }

                        // Need to set a flag signalling that we've locked in a drag.
                        // When the mouse button is let go, clear the flag.
                        if (state.Mouse.LeftButtonDown)
                        {
                            if (_barOrientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                if (mouseControlPosition.Y == 0)
                                    if (mouseControlPosition.X == _currentSliderPosition + 1)
                                    {
                                        Parent.CaptureControl(this);
                                    }
                            }
                            else
                            {
                                if (mouseControlPosition.X == 0)
                                    if (mouseControlPosition.Y == _currentSliderPosition + 1)
                                    {
                                        Parent.CaptureControl(this);
                                    }
                            }

                            Parent.FocusedControl = this;
                        }

                        return true;
                    }
                }
                else if (Parent.CapturedControl == this)
                {
                    if (state.CellPosition.X >= this.Position.X - 2 && state.CellPosition.X < this.Position.X + textSurface.Width + 2 &&
                        state.CellPosition.Y >= this.Position.Y - 3 && state.CellPosition.Y < this.Position.Y + textSurface.Height + 3)
                    {
                        if (state.Mouse.LeftButtonDown)
                        {
                            if (_barOrientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                //if (mouseControlPosition.Y == 0)
                                //{
                                //    if (mouseControlPosition.X == _currentSliderPosition + 1)
                                //        Value -= Step;
                                //}


                                if (mouseControlPosition.X >= 1 && mouseControlPosition.X <= _sliderBarSize)
                                {

                                    _currentSliderPosition = mouseControlPosition.X - 1;

                                    if (_sliderPositionValues[_currentSliderPosition] != -1)
                                    {
                                        _value = _sliderPositionValues[_currentSliderPosition];
                                        if (ValueChanged != null)
                                            ValueChanged.Invoke(this, EventArgs.Empty);

                                        this.IsDirty = true;
                                    }
                                }

                            }
                            else
                            {
                                if (mouseControlPosition.Y >= 1 && mouseControlPosition.Y <= _sliderBarSize)
                                {

                                    _currentSliderPosition = mouseControlPosition.Y - 1;

                                    if (_sliderPositionValues[_currentSliderPosition] != -1)
                                    {
                                        _value = _sliderPositionValues[_currentSliderPosition];
                                        if (ValueChanged != null)
                                            ValueChanged.Invoke(this, EventArgs.Empty);

                                        this.IsDirty = true;
                                    }
                                }
                            }

                            return true;
                        }
                        else
                            Parent.ReleaseControl();

                        return false;

                    }
                }
                
                //else if(Parent.CapturedControl == this && !info.LeftButtonDown)
                //{
                //    Parent.ReleaseControl();
                //}


            }

            return false;
        }

        /// <summary>
        /// Not Used.
        /// </summary>
        /// <param name="state"></param>
        public override bool ProcessKeyboard(Input.Keyboard state)
        {
            return false;
        }

        private void DetermineSliderPositions()
        {
            _sliderPositionValues[0] = 0;
            _sliderPositionValues[_sliderBarSize - 1] = _maxValue;

            // Clear other spots
            for (int i = 1; i < _sliderBarSize - 1; i++)
            {
                _sliderPositionValues[i] = -1;
            }

            int rest = _sliderBarSize - 2;

            if (_maxValue == 1)
            {
                // Do nothing.
            }
            // Throw other item in middle
            else if (_maxValue == 2)
            {
                if (rest == 1)
                    _sliderPositionValues[1] = _maxValue - 1;
                else if (rest % 2 != 0)
                    _sliderPositionValues[((rest - 1) / 2) + 1] = _maxValue - 1;
                else
                    _sliderPositionValues[rest / 2] = _maxValue - 1;
            }

            else if (rest >= _maxValue - 1)
            {
                for (int i = 1; i < _maxValue; i++)
                {
                    _sliderPositionValues[i] = i;
                }
            }
            else
            {
                float itemValue = (float)(_maxValue - 1) / rest;

                for (int i = 1; i < _sliderBarSize - 1; i++)
                {
                    _sliderPositionValues[i] = (int)((float)i * itemValue);
                }
            }
        }

        [OnDeserializedAttribute]
        private void AfterDeserialized(StreamingContext context)
        {
            _initialized = true;

            var temp = _value;
            _value = -22;
            Value = temp;

            Compose(true);
        }
    }

}
