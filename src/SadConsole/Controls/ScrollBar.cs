#if XNA
using Microsoft.Xna.Framework;
#endif

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

        protected int _topOrLeftCharacter;
        protected int _bottomOrRightCharacter;
        protected int _sliderCharacter;
        protected int _sliderBarCharacter;

        [DataMember(Name = "Minimum")]
        protected int _minValue = 0;
        protected int _maxValue = 1;
        protected int _value = 0;
        protected int _valueStep = 1;

        protected int[] _sliderPositionValues;

        public Orientation Orientation { get; private set; }

        public int SliderBarSize { get; private set; }

        public int CurrentSliderPosition { get; private set; }

        /// <summary>
        /// Gets or sets the value of the scrollbar between the minimum and maximum values.
        /// </summary>
        [DataMember]
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    if (_value < 0)
                    {
                        _value = 0;
                    }
                    else if (_value > _maxValue)
                    {
                        _value = _maxValue;
                    }

                    if (_value == 0)
                    {
                        CurrentSliderPosition = 0;
                    }
                    else if (_value == _maxValue)
                    {
                        CurrentSliderPosition = SliderBarSize - 1;
                    }
                    else
                    {
                        // Find which slot is < value where the slot after is > value
                        for (int i = 1; i < SliderBarSize - 1; i++)
                        {
                            if (_sliderPositionValues[i] == _value)
                            {
                                CurrentSliderPosition = i;
                                break;
                            }
                            if (_sliderPositionValues[i] > _value && _sliderPositionValues[i - 1] < _value && _sliderPositionValues[i] != -1)
                            {
                                CurrentSliderPosition = i;
                                break;
                            }
                        }
                    }

                    IsDirty = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for the scrollbar.
        /// </summary>
        public int Maximum
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                if (_maxValue <= 0)
                {
                    _maxValue = 1;
                }

                DetermineSliderPositions();
                IsDirty = true;
            }
        }

        public int Minimum => 0;

        /// <summary>
        /// Gets or sets the amount of values to add or substract to the <see cref="Value"/> when the up or down arrows are used.
        /// </summary>
        [DataMember]
        public int Step
        {
            get => _valueStep;
            set => _valueStep = value;
        }

        /// <summary>
        /// Creates a new ScrollBar control.
        /// </summary>
        /// <param name="orientation">Sets the control to either horizontal or vertical.</param>
        /// <param name="size">The height or width of the control.</param>
        /// <returns>The new control instance.</returns>
        public ScrollBar(Orientation orientation, int size) : base(orientation == Orientation.Horizontal ? size : 1,
                                                                  orientation == Orientation.Vertical ? size : 1)
        {
            if (size <= 2)
            {
                throw new Exception("The scroll bar must be 4 or more in size.");
            }

            _initialized = true;
            Orientation = orientation;

            _sliderCharacter = 219;

            if (orientation == Orientation.Horizontal)
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

            if (Width > Height)
            {
                SliderBarSize = Width - 2;
            }
            else
            {
                SliderBarSize = Height - 2;
            }

            _sliderPositionValues = new int[SliderBarSize];
            DetermineSliderPositions();
        }

        // Locking the mouse to this control is actually locking the parent console to the engine, and then
        // letting the controls console know that this control wants exclusive focus until mouse is unclicked.

        public override bool ProcessMouse(Input.MouseConsoleState state)
        {
            if (IsEnabled)
            {
                base.ProcessMouse(state);


                var mouseControlPosition = new Point(state.CellPosition.X - Position.X, state.CellPosition.Y - Position.Y);

                // This becomes the active mouse subject when the bar is being dragged.
                if (Parent.CapturedControl == null)
                {
                    if (state.CellPosition.X >= Position.X && state.CellPosition.X < Position.X + Width &&
                        state.CellPosition.Y >= Position.Y && state.CellPosition.Y < Position.Y + Height)
                    {
                        if (state.Mouse.ScrollWheelValueChange != 0)
                        {
                            Value += state.Mouse.ScrollWheelValueChange / 20;
                            return true;
                        }

                        if (state.Mouse.LeftClicked)
                        {
                            if (Orientation == Orientation.Horizontal)
                            {
                                if (mouseControlPosition.X == 0)
                                {
                                    Value -= Step;
                                }

                                if (mouseControlPosition.X == Width - 1)
                                {
                                    Value += Step;
                                }
                            }
                            else
                            {
                                if (mouseControlPosition.Y == 0)
                                {
                                    Value -= Step;
                                }

                                if (mouseControlPosition.Y == Height - 1)
                                {
                                    Value += Step;
                                }
                            }

                            Parent.FocusedControl = this;
                        }

                        // Need to set a flag signalling that we've locked in a drag.
                        // When the mouse button is let go, clear the flag.
                        if (state.Mouse.LeftButtonDown)
                        {
                            if (Orientation == Orientation.Horizontal)
                            {
                                if (mouseControlPosition.Y == 0)
                                {
                                    if (mouseControlPosition.X == CurrentSliderPosition + 1)
                                    {
                                        Parent.CaptureControl(this);
                                    }
                                }
                            }
                            else
                            {
                                if (mouseControlPosition.X == 0)
                                {
                                    if (mouseControlPosition.Y == CurrentSliderPosition + 1)
                                    {
                                        Parent.CaptureControl(this);
                                    }
                                }
                            }

                            Parent.FocusedControl = this;
                        }

                        return true;
                    }
                }
                else if (Parent.CapturedControl == this)
                {
                    if (!state.Mouse.LeftButtonDown)
                    {
                        Parent.ReleaseControl();
                        return false;
                    }

                    if (state.CellPosition.X >= Position.X - 2 && state.CellPosition.X < Position.X + Width + 2 &&
                        state.CellPosition.Y >= Position.Y - 3 && state.CellPosition.Y < Position.Y + Height + 3)
                    {

                        if (Orientation == Orientation.Horizontal)
                        {
                            //if (mouseControlPosition.Y == 0)
                            //{
                            //    if (mouseControlPosition.X == _currentSliderPosition + 1)
                            //        Value -= Step;
                            //}


                            if (mouseControlPosition.X >= 1 && mouseControlPosition.X <= SliderBarSize)
                            {

                                CurrentSliderPosition = mouseControlPosition.X - 1;

                                if (_sliderPositionValues[CurrentSliderPosition] != -1)
                                {
                                    _value = _sliderPositionValues[CurrentSliderPosition];
                                    if (ValueChanged != null)
                                    {
                                        ValueChanged.Invoke(this, EventArgs.Empty);
                                    }

                                    IsDirty = true;
                                }
                            }

                        }
                        else
                        {
                            if (mouseControlPosition.Y >= 1 && mouseControlPosition.Y <= SliderBarSize)
                            {

                                CurrentSliderPosition = mouseControlPosition.Y - 1;

                                if (_sliderPositionValues[CurrentSliderPosition] != -1)
                                {
                                    _value = _sliderPositionValues[CurrentSliderPosition];
                                    if (ValueChanged != null)
                                    {
                                        ValueChanged.Invoke(this, EventArgs.Empty);
                                    }

                                    IsDirty = true;
                                }
                            }
                        }

                        return true;
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
        public override bool ProcessKeyboard(Input.Keyboard state) => false;

        private void DetermineSliderPositions()
        {
            _sliderPositionValues[0] = 0;
            _sliderPositionValues[SliderBarSize - 1] = _maxValue;

            // Clear other spots
            for (int i = 1; i < SliderBarSize - 1; i++)
            {
                _sliderPositionValues[i] = -1;
            }

            int rest = SliderBarSize - 2;

            if (_maxValue == 1)
            {
                // Do nothing.
            }
            // Throw other item in middle
            else if (_maxValue == 2)
            {
                if (rest == 1)
                {
                    _sliderPositionValues[1] = _maxValue - 1;
                }
                else if (rest % 2 != 0)
                {
                    _sliderPositionValues[((rest - 1) / 2) + 1] = _maxValue - 1;
                }
                else
                {
                    _sliderPositionValues[rest / 2] = _maxValue - 1;
                }
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

                for (int i = 1; i < SliderBarSize - 1; i++)
                {
                    _sliderPositionValues[i] = (int)(i * itemValue);
                }
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext context)
        {
            _initialized = true;

            int temp = _value;
            _value = -22;
            Value = temp;

            IsDirty = true;
            DetermineState();
        }
    }
}
