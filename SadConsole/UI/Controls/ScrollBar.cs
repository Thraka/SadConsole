using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Represents a scrollbar control.
    /// </summary>
    [DataContract]
    public class ScrollBar : ControlBase
    {
        public event EventHandler ValueChanged;

        private bool _initialized;

        [DataMember(Name = "Minimum")]
        protected int _minValue = 0;
        protected int _maxValue = 1;
        protected int _value = 0;
        protected int _valueStep = 1;

        public Orientation Orientation { get; private set; }

        public int SliderBarSize { get; private set; }

        public int CurrentSliderPosition { get; private set; }

        /// <summary>
        /// When <see langword="true"/>, indicates this control is captured and the slider button is being used; otherwise, <see langword="false"/>.
        /// </summary>
        public bool IsSliding { get; set; }

        /// <summary>
        /// Gets or sets the value of the scrollbar between the minimum and maximum values.
        /// </summary>
        [DataMember]
        public int Value
        {
            get => _value;
            set
            {
                int oldValue = _value;
                _value = MathHelpers.Clamp(value, 0, _maxValue);

                if (oldValue != _value)
                {
                    SetSliderPositionFromValue();

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
                    _maxValue = 1;

                Value = Value;

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
            _initialized = true;
            Orientation = orientation;

            if (size < 2) throw new Exception("Slider bar size must be 2 or more");

            SliderBarSize = size - 2;

            SetSliderPositionFromValue();
        }

        // Locking the mouse to this control is actually locking the parent console to the engine, and then
        // letting the controls console know that this control wants exclusive focus until mouse is unclicked.

        public override bool ProcessMouse(Input.MouseScreenObjectState state)
        {
            if (IsEnabled)
            {
                base.ProcessMouse(state);

                var newState = new ControlMouseState(this, state);
                var mouseControlPosition = newState.MousePosition;

                // This becomes the active mouse subject when the bar is being dragged.
                if (Parent.Host.CapturedControl == null)
                {
                    if (newState.IsMouseOver)
                    {
                        if (state.Mouse.ScrollWheelValueChange != 0)
                        {
                            if (state.Mouse.ScrollWheelValueChange < 0)
                                Value -= 1;
                            else
                                Value += 1;

                            return true;
                        }

                        if (state.Mouse.LeftClicked)
                        {
                            if (Orientation == Orientation.Horizontal)
                            {
                                if (mouseControlPosition.X == 0)
                                    Value -= Step;

                                else if (mouseControlPosition.X == Width - 1)
                                    Value += Step;
                            }
                            else
                            {
                                if (mouseControlPosition.Y == 0)
                                    Value -= Step;

                                else if (mouseControlPosition.Y == Height - 1)
                                    Value += Step;
                            }

                            Parent.Host.FocusedControl = this;
                        }

                        // Need to set a flag signalling that we've locked in a drag.
                        // When the mouse button is let go, clear the flag.
                        if (state.Mouse.LeftButtonDown)
                        {
                            if (Orientation == Orientation.Horizontal)
                            {
                                if (mouseControlPosition.Y == 0)
                                {
                                    if (SliderBarSize != 0 && mouseControlPosition.X == CurrentSliderPosition + 1)
                                    {
                                        Parent.Host.CaptureControl(this);
                                        IsSliding = true;
                                        IsDirty = true;
                                    }
                                }
                            }
                            else
                            {
                                if (mouseControlPosition.X == 0)
                                {
                                    if (SliderBarSize != 0 && mouseControlPosition.Y == CurrentSliderPosition + 1)
                                    {
                                        Parent.Host.CaptureControl(this);
                                        IsSliding = true;
                                        IsDirty = true;
                                    }
                                }
                            }

                            Parent.Host.FocusedControl = this;
                        }

                        return true;
                    }
                }
                else if (Parent.Host.CapturedControl == this)
                {
                    if (!state.Mouse.LeftButtonDown)
                    {
                        Parent.Host.ReleaseControl();
                        IsSliding = false;
                        IsDirty = true;
                        return false;
                    }

                    if (newState.IsMouseOver)
                    {

                        if (Orientation == Orientation.Horizontal)
                        {
                            //if (mouseControlPosition.Y == 0)
                            //{
                            //    if (mouseControlPosition.X == _currentSliderPosition + 1)
                            //        Value -= Step;
                            //}


                            if (SliderBarSize != 0 && mouseControlPosition.X >= 1 && mouseControlPosition.X <= SliderBarSize)
                            {

                                CurrentSliderPosition = mouseControlPosition.X - 1;

                                SetValueFromSliderPosition();

                                IsDirty = true;
                            }

                        }
                        else
                        {
                            if (SliderBarSize != 0 && mouseControlPosition.Y >= 1 && mouseControlPosition.Y <= SliderBarSize)
                            {

                                CurrentSliderPosition = mouseControlPosition.Y - 1;

                                SetValueFromSliderPosition();

                                IsDirty = true;
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


        private void SetValueFromSliderPosition()
        {
            int oldValue = _value;

            if (CurrentSliderPosition == 0)
                _value = 0;
            else if (CurrentSliderPosition == SliderBarSize - 1)
                _value = _maxValue;
            else
                _value = (int)((float)Maximum / (float)SliderBarSize * (float)CurrentSliderPosition);

            if (oldValue != _value)
                ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetSliderPositionFromValue()
        {
            if (SliderBarSize == 0) return;

            // Check for start vs end
            if (_value == 0)
                CurrentSliderPosition = 0;
            else if (_value == _maxValue)
                CurrentSliderPosition = SliderBarSize - 1;
            else
                CurrentSliderPosition = (int)(((float)SliderBarSize / (float)Maximum) * (float)_value);

            IsDirty = true;
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
