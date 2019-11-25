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
        private IScreenObjectSurface _lastMouseScreenObject;

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
            bool newLeftClicked = LeftButtonDown && !leftDown;
            bool newRightClicked = RightButtonDown && !rightDown;
            bool newMiddleClicked = MiddleButtonDown && !middleDown;

            if (!newLeftClicked)
            {
                LeftDoubleClicked = false;
            }

            if (!newRightClicked)
            {
                RightDoubleClicked = false;
            }

            if (!newMiddleClicked)
            {
                MiddleDoubleClicked = false;
            }

            if (LeftClicked && newLeftClicked && elapsedSeconds.TotalSeconds < 1000)
            {
                LeftDoubleClicked = true;
            }

            if (RightClicked && newRightClicked && elapsedSeconds.TotalSeconds < 1000)
            {
                RightDoubleClicked = true;
            }

            if (MiddleClicked && newMiddleClicked && elapsedSeconds.TotalSeconds < 1000)
            {
                MiddleDoubleClicked = true;
            }

            LeftClicked = newLeftClicked;
            RightClicked = newRightClicked;
            MiddleClicked = newMiddleClicked;
            _leftLastClickedTime = elapsedSeconds;
            _rightLastClickedTime = elapsedSeconds;
            LeftButtonDown = leftDown;
            RightButtonDown = rightDown;
            MiddleButtonDown = middleDown;
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
        }

        /// <summary>
        /// Builds information about the mouse state based on the <see cref="Global.FocusedScreenObjects"/> or <see cref="Global.CurrentScreen"/>. Should be called each frame.
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
            else if (Global.FocusedScreenObjects.ScreenObject != null && Global.FocusedScreenObjects.ScreenObject.IsExclusiveMouse)
            {
                var state = new MouseScreenObjectState(Global.FocusedScreenObjects.ScreenObject, this);

                // if the last screen object to have the mouse is not our global, signal
                if (_lastMouseScreenObject != null && _lastMouseScreenObject != Global.FocusedScreenObjects.ScreenObject)
                {
                    _lastMouseScreenObject.LostMouse(state);
                    _lastMouseScreenObject = null;
                }

                Global.FocusedScreenObjects.ScreenObject.ProcessMouse(state);

                _lastMouseScreenObject = Global.FocusedScreenObjects.ScreenObject;
            }

            // Scan through each "screen object" in the current screen, including children.
            else if (Global.Screen != null)
            {
                bool foundMouseTarget = false;

                // Build a list of all screen objects
                var screenObjects = new List<IScreenObjectSurface>();
                GetConsoles(Global.Screen, ref screenObjects);

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

        private void GetConsoles(IScreenObjectSurface screen, ref List<IScreenObjectSurface> list)
        {
            if (!screen.IsVisible)
            {
                return;
            }

            if (screen.UseMouse)
            {
                list.Add(screen);
            }

            foreach (IScreenObjectSurface child in screen.Children.OfType<IScreenObjectSurface>())
            {
                GetConsoles(child, ref list);
            }
        }

        /// <summary>
        /// Unlocks the last screen object the mouse was locked to. Allows another conosle to become locked to the mouse.
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
        public bool IsMouseOverScreenObjectSurface(IScreenObjectSurface screenObject) =>
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
