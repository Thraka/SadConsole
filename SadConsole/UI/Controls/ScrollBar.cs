using System;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// Represents a scrollbar control.
/// </summary>
[DataContract]
public partial class ScrollBar : ControlBase
{
    private int _value = 0;
    private int _maxValue = 1;
    private int _arrowStep = 1;
    private int _mouseWheelStep = 1;
    private int[] _barValues;
    private int _gripGrabStart = 0;

    /// <summary>
    /// Raised when the <see cref="Value"/> property changes.
    /// </summary>
    public event EventHandler? ValueChanged;

    /// <summary>
    /// Indicates if the slider is horizontal or vertical.
    /// </summary>
    [DataMember]
    public Orientation Orientation { get; private set; }

    /// <summary>
    /// When true, indicates that the mouse is gripping the slider to scroll.
    /// </summary>
    public bool IsGripped { get; private set; }

    /// <summary>
    /// Gets or sets the maximum value for the scrollbar.
    /// </summary>
    [DataMember]
    public int MaximumValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;

            if (_maxValue <= 0)
                _maxValue = 1;

            CalculateGripSize();

            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the amount of values to add or subtract to the <see cref="Value"/> when the up or down arrows are used.
    /// </summary>
    [DataMember]
    public int ArrowStep
    {
        get => _arrowStep;
        set
        {
            _arrowStep = value;

            if (_arrowStep <= 0)
                _arrowStep = 1;

            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the amount of values to add or subtract to the <see cref="Value"/> when the up or down arrows are used.
    /// </summary>
    [DataMember]
    public int MouseWheelStep
    {
        get => _mouseWheelStep;
        set
        {
            _mouseWheelStep = value;

            if (_mouseWheelStep <= 0)
                _mouseWheelStep = 1;

            IsDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the value of the slider between the minimum and maximum values.
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
                SetGripToValue();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// When true, the arrow buttons simply move the grip instead of using the <see cref="ArrowStep"/> value to adjust the value.
    /// </summary>
    [DataMember]
    public bool ArrowsMoveGrip { get; set; }

    /// <summary>
    /// When true, the mouse wheelsimply moves the grip instead of using the <see cref="MouseWheelStep"/> value to adjust the value.
    /// </summary>
    [DataMember]
    public bool MouseWheelMovesGrip { get; set; }

    /// <summary>
    /// Creates a new Slider control
    /// </summary>
    /// <param name="orientation">Sets the control to either horizontal or vertical.</param>
    /// <param name="size">The height or width of the control, based on the <paramref name="orientation"/>, with a thickness of 1.</param>
    /// <exception cref="ArgumentOutOfRangeException">Size of the control must be 2 or more</exception>
    public ScrollBar(Orientation orientation, int size) : this(orientation,
                                                               orientation == Orientation.Horizontal ? size : 1, // width
                                                               orientation == Orientation.Vertical ? size : 1)   // height
    {

    }

    /// <summary>
    /// Creates a new Slider control with the specified width and height.
    /// </summary>
    /// <param name="orientation">Sets the control to either horizontal or vertical.</param>
    /// <param name="width">The width the control.</param>
    /// <param name="height">The height of the control.</param>
    /// <exception cref="ArgumentOutOfRangeException">With a horizontal Slider, width must be 2 or more. With a vertical Slider, height must be 2 or more.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ScrollBar(Orientation orientation, int width, int height) : base(width, height)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        if (orientation == Orientation.Horizontal && width < 2) throw new ArgumentOutOfRangeException(nameof(width), "With a horizontal Slider, width must be 2 or more.");
        if (orientation == Orientation.Vertical && height < 2) throw new ArgumentOutOfRangeException(nameof(height), "With a vertical Slider, height must be 2 or more.");

        _themeStyle = new ThemeStyle();

        Orientation = orientation;

        _maxValue = 1;
        CalculateGripSize();
        SetGripToValue();
    }

    /// <summary>
    /// Invokes the <see cref="ValueChanged"/> event.
    /// </summary>
    protected void OnValueChanged() =>
        ValueChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Sets the bar and grip sizes of the control.
    /// </summary>
    protected void CalculateGripSize()
    {
        Style.BarSize = Orientation == Orientation.Horizontal ? Width - 2 : Height - 2;
        Style.GripSize = Math.Max(Style.BarSize - MaximumValue, 1);

        if (Style.BarSize > 2)
        {
            _barValues = new int[Style.BarSize];
            for (int i = 0; i < Style.BarSize; i++)
                _barValues[i] = (int)Math.Round(((float)i / (Style.BarSize - 1)) * MaximumValue);
            _barValues[^1] = MaximumValue;
        }
        else if (Style.BarSize == 2)
        {
            Style.GripSize = 1;
        }
    }

    /// <summary>
    /// Moves the grip position based on the <see cref="Value"/> property.
    /// </summary>
    protected void SetGripToValue()
    {
        // Check for start vs end
        if (_value == 0)
            Style.GripStart = 1;
        else if (_value == _maxValue)
            Style.GripStart = Style.BarSize - Style.GripSize + 1;
        else
        {
            if (Style.BarSize == 2)
                Style.GripStart = (int)Math.Round((float)_value / MaximumValue) + 1;
            else if (Style.GripSize == 1)
                Style.GripStart = (int)Math.Round(((float)_value - 1) / (MaximumValue - 2) * (Style.BarSize - 3) + 2);
            else
                Style.GripStart = _value + 1;
        }
    }

    /// <summary>
    /// Resizes the control and recalculates the grip size.
    /// </summary>
    protected override void OnResized()
    {
        base.OnResized();

        CalculateGripSize();
    }

    /// <summary>
    /// Sets the value of the control without using the <see cref="Value"/> property. Optionally calls <see cref="SetGripToValue"/>.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="setGrip">Moves the grip when true.</param>
    protected void SetValue(int value, bool setGrip = true)
    {
        int oldValue = _value;
        _value = Math.Clamp(value, 0, MaximumValue);

        if (oldValue != _value)
        {
            if (setGrip && Style.BarSize > 1)
                SetGripToValue();

            OnValueChanged();
            IsDirty = true;
        }
    }

    /// <summary>
    /// Increases the value by the specified amount.
    /// </summary>
    /// <param name="value">The value to add to <see cref="Value"/>.</param>
    public void IncreaseValue(int value) =>
        SetValue(_value + value);

    /// <summary>
    /// Decreases the value by the specified amount.
    /// </summary>
    /// <param name="value">The value to subract from <see cref="Value"/>.</param>
    public void DecreaseValue(int value) =>
        SetValue(_value - value);

    /// <summary>
    /// Increases the grip position by one and sets the value.
    /// </summary>
    public void IncreaseGripByOne()
    {
        // Increase by grip, but clamp to max
        if (Style.BarSize - Style.GripSize == 1 && Style.GripStart < 2)
            SetValue(MaximumValue);

        // Increase by grip
        else if (Style.GripStart + Style.GripSize <= Style.BarSize)
        {
            Style.GripStart++;

            if (Style.GripSize == 1)
                SetValue(_barValues[Style.GripStart - 1], false);
            else
                SetValue(Style.GripStart - 1, false);
        }
    }

    /// <summary>
    /// Decreases the grip position by one and sets the value.
    /// </summary>
    public void DecreaseGripByOne()
    {
        // Decrease by grip, but clamp to start
        if (Style.BarSize - Style.GripSize == 1 && Style.GripStart > 1)
            SetValue(0);

        // Decrease by grip
        else if (Style.GripStart > 1)
        {
            Style.GripStart--;

            if (Style.GripSize == 1)
                SetValue(_barValues[Style.GripStart - 1], false);
            else
                SetValue(Style.GripStart - 1, false);
        }
    }

    /// <inheritdoc/>
    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (IsEnabled && Parent?.Host != null)
        {
            base.ProcessMouse(state);

            ControlMouseState controlMouseState = new(this, state);

            Style.IsMouseOverUpButton = false;
            Style.IsMouseOverDownButton = false;
            Style.IsMouseOverGripper = false;
            Style.IsMouseOverBar = false;

            // Dragging the gripper around
            if (IsGripped)
            {
                // Mouse released, don't hold on to control
                if (!state.Mouse.LeftButtonDown)
                {
                    Parent.Host.ReleaseControl();
                    IsGripped = false;
                    IsDirty = true;
                    MouseArea = new Rectangle(0, 0, Width, Height);
                    return controlMouseState.IsMouseOver;
                }
                else if (MouseArea.Contains(controlMouseState.MousePosition))
                {
                    int cell = (Orientation == Orientation.Horizontal ? controlMouseState.MousePosition.X : controlMouseState.MousePosition.Y) - _gripGrabStart;

                    // Cell is behind the bar, so move to min
                    if (cell < 1)
                    {
                        Style.GripStart = 1;
                        SetValue(0, false);
                    }

                    // Cell is after the bar, so move to max
                    else if (cell > Style.BarSize - Style.GripSize + 1)
                    {
                        Style.GripStart = Style.BarSize - Style.GripSize + 1;
                        SetValue(MaximumValue, false);
                    }

                    // Cell is in range of the bar
                    else
                    {
                        Style.GripStart = cell;

                        if (Style.BarSize == 2)
                        {
                            if (cell == 1)
                                SetValue(0);
                            else
                                SetValue(MaximumValue);
                        }
                        else if (Style.GripSize == 1)
                            SetValue(_barValues[cell - 1], false);
                        else
                            SetValue(Style.GripStart - 1, false);
                        IsDirty = true;
                    }
                }
            }

            else if (controlMouseState.IsMouseOver)
            {
                // Orientation independent cell position
                int cell = Orientation == Orientation.Horizontal ? controlMouseState.MousePosition.X : controlMouseState.MousePosition.Y;
                int size = Orientation == Orientation.Horizontal ? Width : Height;

                // Up button
                if (cell == 0)
                {
                    Style.IsMouseOverUpButton = true;

                    if (state.Mouse.LeftClicked)
                    {
                        // Just decrease by arrow stepping
                        if (!ArrowsMoveGrip || Style.BarSize < 2)
                            DecreaseValue(ArrowStep);

                        // Decrease the grip instead
                        else
                            DecreaseGripByOne();


                        IsDirty = true;

                        return true;
                    }
                }

                // Down button
                else if (cell == (Orientation == Orientation.Horizontal ? Width - 1 : Height - 1))
                {
                    Style.IsMouseOverDownButton = true;

                    if (state.Mouse.LeftClicked)
                    {
                        // Just increase by arrow stepping
                        if (!ArrowsMoveGrip || Style.BarSize < 2)
                            IncreaseValue(ArrowStep);

                        // Increase the grip instead
                        else
                            IncreaseGripByOne();

                        IsDirty = true;
                        return true;
                    }
                }

                // Gripper/Bar
                if (Style.BarSize > 1)
                {
                    // Scroll wheel
                    if (state.Mouse.ScrollWheelValueChange != 0)
                    {
                        if (state.Mouse.ScrollWheelValueChange < 0)
                        {
                            if (!MouseWheelMovesGrip || Style.BarSize < 2)
                                DecreaseValue(_mouseWheelStep);
                            else
                                DecreaseGripByOne();
                        }
                        else
                        {
                            if (!MouseWheelMovesGrip || Style.BarSize < 2)
                                IncreaseValue(_mouseWheelStep);
                            else
                                IncreaseGripByOne();
                        }

                        IsDirty = true;
                        return true;
                    }

                    if (cell >= Style.GripStart && cell <= Style.GripEnd)
                    {
                        Style.IsMouseOverGripper = true;

                        // Grab the gripper
                        if (state.Mouse.LeftButtonDown && !MouseState_EnteredWithButtonDown)
                        {
                            IsGripped = true;
                            Parent.Host.CaptureControl(this);
                            IsDirty = true;
                            _gripGrabStart = cell - Style.GripStart;

                            if (Orientation == Orientation.Vertical)
                                MouseArea = new Rectangle(-2, -1, Width + 4, Height + 2);
                            else
                                MouseArea = new Rectangle(-1, -2, Width + 2, Height + 4);

                            return true;
                        }
                    }

                    // Bar
                    else
                    {
                        Style.IsMouseOverBar = true;

                        if (state.Mouse.LeftClicked)
                        {
                            if (cell == 1)
                                SetValue(0);

                            else if (cell == size - 2)
                                SetValue(MaximumValue);

                            else if (cell < Style.GripStart || cell > Style.GripStart)
                            {
                                // Move the top of the grip to the spot clicked
                                if (Style.GripSize == 1)
                                {
                                    SetValue(_barValues[cell - 1], false);
                                    Style.GripStart = cell;
                                }
                                else
                                    SetValue(cell - 1);
                            }

                            return true;
                        }
                    }
                }
                else
                {
                    if (Style.BarSize > 1 && state.Mouse.LeftClicked)
                    {
                        if (cell == 1)
                            SetValue(0);
                        else
                            SetValue(MaximumValue);

                        return true;
                    }

                    // Small control with no bar, so enable scroll wheel
                    if (state.Mouse.ScrollWheelValueChange != 0)
                    {
                        if (state.Mouse.ScrollWheelValueChange < 0)
                            DecreaseValue(_mouseWheelStep);
                        else
                            IncreaseValue(_mouseWheelStep);

                        return true;
                    }
                }

                IsDirty = true;

                base.ProcessMouse(state);

                return true;
            }
        }

        return false;
    }
}
