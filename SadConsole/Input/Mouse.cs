using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives;

namespace SadConsole.Input
{
    /// <summary>
    /// The state of the mouse.
    /// </summary>
    public class Mouse
    {
        private TimeSpan _leftLastClickedTime;
        private TimeSpan _rightLastClickedTime;
        private TimeSpan _middleLastClickedTime;

        private bool _leftPressedLastFrame;
        private bool _rightPressedLastFrame;
        private bool _middlePressedLastFrame;

        private IScreenObject _lastMouseScreenObject;

        /// <summary>
        /// The pixel position of the mouse on the screen.
        /// </summary>
        public Point ScreenPosition { get; set; }

        /// <summary>
        /// Indicates the middle mouse button is currently being pressed.
        /// </summary>
        public bool MiddleButtonDown { get; set; }

        /// <summary>
        /// Indicates the middle mouse button was clicked. (Held and then released)
        /// </summary>
        public bool MiddleClicked { get; set; }

        /// <summary>
        /// Inidcates the middle mouse button was double-clicked within one second.
        /// </summary>
        public bool MiddleDoubleClicked { get; set; }

        /// <summary>
        /// Indicates the left mouse button is currently being pressed.
        /// </summary>
        public bool LeftButtonDown { get; set; }

        /// <summary>
        /// Indicates the left mouse button was clicked. (Held and then released)
        /// </summary>
        public bool LeftClicked { get; set; }

        /// <summary>
        /// Inidcates the left mouse button was double-clicked within one second.
        /// </summary>
        public bool LeftDoubleClicked { get; set; }

        /// <summary>
        /// Indicates the right mouse button is currently being pressed.
        /// </summary>
        public bool RightButtonDown { get; set; }

        /// <summary>
        /// Indicates the right mouse button was clicked. (Held and then released)
        /// </summary>
        public bool RightClicked { get; set; }

        /// <summary>
        /// Indicates the right mouse buttion was double-clicked within one second.
        /// </summary>
        public bool RightDoubleClicked { get; set; }

        /// <summary>
        /// The cumulative value of the scroll wheel. 
        /// </summary>
        public int ScrollWheelValue { get; set; }

        /// <summary>
        /// The scroll wheel value change between frames.
        /// </summary>
        public int ScrollWheelValueChange { get; set; }

        /// <summary>
        /// Indicates that the mouse is currently within the bounds of the rendering area.
        /// </summary>
        public bool IsOnScreen => Settings.Rendering.RenderRect.Contains(ScreenPosition + Settings.Rendering.RenderRect.Position);

        /// <summary>
        /// Reads the mouse state from <see cref="GameHost.GetMouseState"/>.
        /// </summary>
        /// <param name="elapsedSeconds">Fractional seconds passed since Update was called.</param>
        public void Update(TimeSpan elapsedSeconds)
        {
            IMouseState currentState = GameHost.Instance.GetMouseState();

            // Update local state
            bool leftDown = currentState.IsLeftButtonDown;
            bool rightDown = currentState.IsRightButtonDown;
            bool middleDown = currentState.IsMiddleButtonDown;

            ScrollWheelValueChange = ScrollWheelValue - currentState.MouseWheel;
            ScrollWheelValue = currentState.MouseWheel;

            ScreenPosition = new Point((int)(currentState.ScreenPosition.X * Settings.Rendering.RenderScale.X), (int)(currentState.ScreenPosition.Y * Settings.Rendering.RenderScale.Y)) - new Point((int)(Settings.Rendering.RenderRect.X * Settings.Rendering.RenderScale.X), (int)(Settings.Rendering.RenderRect.Y * Settings.Rendering.RenderScale.Y));

            // Count time for full clicks
            if (!_leftPressedLastFrame && leftDown)
                _leftPressedLastFrame = true;
            else if (leftDown)
                _leftLastClickedTime += elapsedSeconds;

            if (!_rightPressedLastFrame && leftDown)
                _rightPressedLastFrame = true;
            else if (rightDown)
                _rightLastClickedTime += elapsedSeconds;

            if (!_middlePressedLastFrame && leftDown)
                _middlePressedLastFrame = true;
            else if (middleDown)
                _middleLastClickedTime += elapsedSeconds;

            // Get the mouse button state change
            bool newLeftClicked = LeftButtonDown && !leftDown;
            bool newRightClicked = RightButtonDown && !rightDown;
            bool newMiddleClicked = MiddleButtonDown && !middleDown;

            // Set state of double clicks
            if (!newLeftClicked)
                LeftDoubleClicked = false;

            if (!newRightClicked)
                RightDoubleClicked = false;

            if (!newMiddleClicked)
                MiddleDoubleClicked = false;

            if (LeftClicked && newLeftClicked && _leftLastClickedTime < Settings.Input.MouseDoubleClickTime)
                LeftDoubleClicked = true;

            if (RightClicked && newRightClicked && _rightLastClickedTime < Settings.Input.MouseDoubleClickTime)
                RightDoubleClicked = true;

            if (MiddleClicked && newMiddleClicked && _middleLastClickedTime < Settings.Input.MouseDoubleClickTime)
                MiddleDoubleClicked = true;

            // Set state of click and mouse down
            LeftClicked = newLeftClicked && _leftLastClickedTime < Settings.Input.MouseClickTime;
            RightClicked = newRightClicked && _rightLastClickedTime < Settings.Input.MouseClickTime;
            MiddleClicked = newMiddleClicked && _middleLastClickedTime < Settings.Input.MouseClickTime;
            LeftButtonDown = leftDown;
            RightButtonDown = rightDown;
            MiddleButtonDown = middleDown;

            // Clear any click frame tracking
            if (_leftPressedLastFrame && !leftDown)
            {
                _leftPressedLastFrame = false;
                _leftLastClickedTime = TimeSpan.Zero;
            }

            if (_rightPressedLastFrame && !rightDown)
            {
                _rightPressedLastFrame = false;
                _rightLastClickedTime = TimeSpan.Zero;
            }

            if (_middlePressedLastFrame && !middleDown)
            {
                _middlePressedLastFrame = false;
                _middleLastClickedTime = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Clears the buttons, position, wheel information.
        /// </summary>
        public void Clear()
        {
            RightDoubleClicked = false;
            RightClicked = false;
            RightButtonDown = false;
            LeftDoubleClicked = false;
            LeftClicked = false;
            LeftButtonDown = false;
            MiddleDoubleClicked = false;
            MiddleClicked = false;
            MiddleButtonDown = false;
            ScrollWheelValue = 0;
            ScrollWheelValueChange = 0;
            ScreenPosition = new Point(0, 0);

            _leftLastClickedTime = TimeSpan.Zero;
            _leftPressedLastFrame = false;
        }

        /// <summary>
        /// Builds information about the mouse state based on the <see cref="GameHost.FocusedScreenObjects"/> or <see cref="GameHost.Screen"/>. Should be called each frame.
        /// </summary>
        public virtual void Process()
        {
            // Check if last mouse was marked exclusive
            if (_lastMouseScreenObject != null && _lastMouseScreenObject.IsExclusiveMouse)
            {
                var state = new MouseScreenObjectState(_lastMouseScreenObject, this);

                _lastMouseScreenObject.ProcessMouse(state);
            }

            // Check if the focused input screen object wants exclusive mouse
            else if (GameHost.Instance.FocusedScreenObjects.ScreenObject != null && GameHost.Instance.FocusedScreenObjects.ScreenObject.IsExclusiveMouse)
            {
                var state = new MouseScreenObjectState(GameHost.Instance.FocusedScreenObjects.ScreenObject, this);

                // if the last screen object to have the mouse is not our global, signal
                if (_lastMouseScreenObject != null && _lastMouseScreenObject != GameHost.Instance.FocusedScreenObjects.ScreenObject)
                {
                    _lastMouseScreenObject.LostMouse(state);
                    _lastMouseScreenObject = null;
                }

                GameHost.Instance.FocusedScreenObjects.ScreenObject.ProcessMouse(state);

                _lastMouseScreenObject = GameHost.Instance.FocusedScreenObjects.ScreenObject;
            }

            // Scan through each "screen object" in the current screen, including children.
            else if (GameHost.Instance.Screen != null)
            {
                bool foundMouseTarget = false;

                // Build a list of all screen objects
                var screenObjects = new List<IScreenObject>();
                GetConsoles(GameHost.Instance.Screen, ref screenObjects);

                // Process top-most screen objects first.
                screenObjects.Reverse();

                for (int i = 0; i < screenObjects.Count; i++)
                {
                    var state = new MouseScreenObjectState(screenObjects[i], this);

                    if (screenObjects[i].ProcessMouse(state))
                    {
                        if (_lastMouseScreenObject != null && _lastMouseScreenObject != screenObjects[i])
                        {
                            _lastMouseScreenObject.LostMouse(state);
                        }

                        foundMouseTarget = true;
                        _lastMouseScreenObject = screenObjects[i];
                        break;
                    }
                }

                if (!foundMouseTarget)
                {
                    _lastMouseScreenObject?.LostMouse(new MouseScreenObjectState(null, this));
                }
            }

        }

        private void GetConsoles(IScreenObject screen, ref List<IScreenObject> list)
        {
            if (!screen.IsVisible) return;

            if (screen.UseMouse)
                list.Add(screen);

            foreach (IScreenObject child in screen.Children)
            {
                GetConsoles(child, ref list);
            }
        }

        /// <summary>
        /// Unlocks the last screen object the mouse was locked to. Allows another console to become locked to the mouse.
        /// </summary>
        public void ClearLastMouseScreenObject()
        {
            _lastMouseScreenObject?.LostMouse(new MouseScreenObjectState(null, this));
            _lastMouseScreenObject = null;
        }

        /// <summary>
        /// Returns true when the mouse is currently over the provided screen object.
        /// </summary>
        /// <param name="screenObject">The screen object to check.</param>
        /// <returns>True or false indicating if the mouse is over the screen object.</returns>
        public bool IsMouseOverScreenObjectSurface(IScreenSurface screenObject) =>
            new MouseScreenObjectState(screenObject, this).IsOnScreenObject;

        /// <summary>
        /// Clones this mouse into a new object.
        /// </summary>
        /// <returns>A clone.</returns>
        public Mouse Clone() => new Mouse()
        {
            ScreenPosition = ScreenPosition,
            LeftButtonDown = LeftButtonDown,
            LeftClicked = LeftClicked,
            LeftDoubleClicked = LeftDoubleClicked,
            RightButtonDown = RightButtonDown,
            RightClicked = RightClicked,
            RightDoubleClicked = RightDoubleClicked,
            MiddleButtonDown = MiddleButtonDown,
            MiddleClicked = MiddleClicked,
            MiddleDoubleClicked = MiddleDoubleClicked,
            ScrollWheelValue = ScrollWheelValue,
            ScrollWheelValueChange = ScrollWheelValueChange
        };
    }
}
