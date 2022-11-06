
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Framework.WpfInterop.Input
{
    //
    // Summary:
    //     Helper class that converts WPF mouse input to the XNA/MonoGame MonoGame.Framework.WpfInterop.Input.WpfMouse._mouseState.
    //     Required for any WPF hosted control.
    public class WpfMouseCustom
    {
        private readonly WpfGame _focusElement;

        private MouseState _mouseState;

        private bool _captureMouseWithin = true;

        //
        // Summary:
        //     Gets or sets the mouse capture behaviour. If true, the mouse will be captured
        //     within the control. This means that the control will still capture mouse events
        //     when the user drags the mouse outside the control. E.g. mouse down on game window,
        //     mouse drag to outside of the window, mouse release -> the game will still register
        //     the mouse release. The downside is that overlayed elements (textbox, etc.) will
        //     never be able to receive focus. If false, mouse events outside the game window
        //     are never registered. E.g. mouse down on game window, mouse drag to outside of
        //     the window, mouse release -> the game will still thing the mouse is pressed until
        //     the cursor enters the window again. The upside is that overlayed controls (e.g.
        //     textboxes) can receive focus and input. Defaults to true.
        public bool CaptureMouseWithin
        {
            get
            {
                return _captureMouseWithin;
            }
            set
            {
                if (!value && _focusElement.IsMouseCaptured)
                {
                    _focusElement.ReleaseMouseCapture();
                }

                _captureMouseWithin = value;
            }
        }

        //
        // Summary:
        //     Creates a new instance of the mouse helper.
        //
        // Parameters:
        //   focusElement:
        //     The element that will be used as the focus point. Provide your implementation
        //     of MonoGame.Framework.WpfInterop.WpfGame here.
        public WpfMouseCustom(WpfGame focusElement)
        {
            if (focusElement == null)
            {
                throw new ArgumentNullException("focusElement");
            }

            _focusElement = focusElement;
            _focusElement.MouseWheel += HandleMouse;
            _focusElement.MouseMove += HandleMouse;
            _focusElement.MouseEnter += HandleMouse;
            _focusElement.MouseLeave += HandleMouse;
            _focusElement.MouseLeftButtonDown += HandleMouse;
            _focusElement.MouseLeftButtonUp += HandleMouse;
            _focusElement.MouseRightButtonDown += HandleMouse;
            _focusElement.MouseRightButtonUp += HandleMouse;
        }

        public MouseState GetState()
        {
            return _mouseState;
        }

        private void HandleMouse(object sender, MouseEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            Point point = e.GetPosition(_focusElement);
            if (e.RoutedEvent == UIElement.MouseLeaveEvent)
            {
                _mouseState = new MouseState((int)point.X, (int)point.Y, _mouseState.ScrollWheelValue + ((e as MouseWheelEventArgs)?.Delta ?? 0), (ButtonState)e.LeftButton, (ButtonState)e.MiddleButton, (ButtonState)e.RightButton, (ButtonState)e.XButton1, (ButtonState)e.XButton2);

            }
            if (!CaptureMouseWithin)
            {
                point = new Point(Clamp(point.X, 0, _focusElement.ActualWidth), Clamp(point.Y, 0, _focusElement.ActualHeight));
            }

            if (_focusElement.IsMouseDirectlyOver && System.Windows.Input.Keyboard.FocusedElement != _focusElement && WindowHelper.IsControlOnActiveWindow(_focusElement))
            {
                if (_focusElement.FocusOnMouseOver)
                {
                    _focusElement.Focus();
                }
                else if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed || e.XButton1 == MouseButtonState.Pressed || e.XButton2 == MouseButtonState.Pressed)
                {
                    _focusElement.Focus();
                }
            }

            if ((!_focusElement.IsMouseDirectlyOver || _focusElement.IsMouseCaptured) && CaptureMouseWithin)
            {
                if (!_focusElement.IsMouseCaptured)
                {
                    return;
                }

                WpfGame focusElement = _focusElement;
                bool hit = false;
                VisualTreeHelper.HitTest(focusElement, (DependencyObject filterTarget) => HitTestFilterBehavior.Continue, delegate (HitTestResult target)
                {
                    if (target.VisualHit == _focusElement)
                    {
                        hit = true;
                    }

                    return HitTestResultBehavior.Continue;
                }, new PointHitTestParameters(point));
                if (!hit)
                {
                    _mouseState = new MouseState(_mouseState.X, _mouseState.Y, _mouseState.ScrollWheelValue, (ButtonState)e.LeftButton, (ButtonState)e.MiddleButton, (ButtonState)e.RightButton, (ButtonState)e.XButton1, (ButtonState)e.XButton2);
                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        _focusElement.ReleaseMouseCapture();
                    }

                    e.Handled = true;
                    return;
                }
            }

            if (CaptureMouseWithin)
            {
                if (!_focusElement.IsMouseCaptured)
                {
                    if (!WindowHelper.IsControlOnActiveWindow(_focusElement))
                    {
                        return;
                    }

                    _focusElement.CaptureMouse();
                }
            }
            else if (_focusElement.IsFocused && !WindowHelper.IsControlOnActiveWindow(_focusElement))
            {
                return;
            }

            e.Handled = true;
            MouseState mouseState = _mouseState;
            _mouseState = new MouseState((int)point.X, (int)point.Y, mouseState.ScrollWheelValue + ((e as MouseWheelEventArgs)?.Delta ?? 0), (ButtonState)e.LeftButton, (ButtonState)e.MiddleButton, (ButtonState)e.RightButton, (ButtonState)e.XButton1, (ButtonState)e.XButton2);
        }

        private static double Clamp(double v, int min, double max)
        {
            if (!(v < (double)min))
            {
                if (!(v > max))
                {
                    return v;
                }

                return max;
            }

            return min;
        }

        //
        // Summary:
        //     Sets the cursor to the specific coordinates within the attached game. This is
        //     required as the monogame Mouse.SetPosition function relies on the underlying
        //     Winforms implementation and will not work with WPF.
        //
        // Parameters:
        //   x:
        //
        //   y:
        public void SetCursor(int x, int y)
        {
            Point point = _focusElement.PointToScreen(new Point(x, y));
            SetCursorPos((int)point.X, (int)point.Y);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);
    }

    internal static class WindowHelper
    {
        //
        // Summary:
        //     Returns the window of the given control or null if unable to find a window. If
        //     null, the default implementation is used
        public static Func<IInputElement, Window> FindWindow;

        public static bool IsControlOnActiveWindow(IInputElement element)
        {
            Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault((Window x) => x.IsActive);
            return GetWindowFrom(element) == window;
        }

        private static Window GetWindowFrom(IInputElement focusElement)
        {
            Func<IInputElement, Window> findWindow = FindWindow;
            if (findWindow != null)
            {
                return findWindow(focusElement);
            }

            return Window.GetWindow((focusElement as FrameworkElement) ?? throw new NotSupportedException("Only FrameworkElement is currently supported."));
        }
    }
}
