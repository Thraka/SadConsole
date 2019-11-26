using System.Collections.Generic;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// A stack of screen objects. The top-most of the stack is considered active and represented by the <see cref="IScreenObject"/> property.
    /// </summary>
    public class FocusedScreenObjectStack
    {
        private IScreenSurface _activeScreenObject;

        /// <summary>
        /// Gets the current active screen object.
        /// </summary>
        public IScreenSurface ScreenObject => _activeScreenObject;

        /// <summary>
        /// The stack of screen objects for input processing.
        /// </summary>
        private readonly List<IScreenSurface> _screenObjects;

        public FocusedScreenObjectStack()
        {
            _screenObjects = new List<IScreenSurface>();
            _activeScreenObject = null;
        }

        /// <summary>
        /// Clears all screen objects from the active stack along with the current active screen object.
        /// </summary>
        public void Clear()
        {
            _screenObjects.Clear();

            if (_activeScreenObject != null)
            {
                _activeScreenObject.OnFocusLost();
            }

            _activeScreenObject = null;
        }

        /// <summary>
        /// Adds another screen object to active stack, setting it as the active (top most in the stack) screen object.
        /// </summary>
        /// <param name="screenObject"></param>
        public void Push(IScreenSurface screenObject)
        {
            if (screenObject != _activeScreenObject && screenObject != null)
            {
                if (_screenObjects.Contains(screenObject))
                {
                    _screenObjects.Remove(screenObject);
                }

                if (_activeScreenObject != null)
                {
                    _activeScreenObject.OnFocusLost();
                }

                _screenObjects.Add(screenObject);
                _activeScreenObject = screenObject;
                _activeScreenObject.OnFocused();
            }
        }

        /// <summary>
        /// Replaces the top screen object (active screen object) with the provided instance. Sets <see cref="IScreenSurface"/> to this instance.
        /// </summary>
        /// <param name="screenObject">The screen object to make active.</param>
        public void Set(IScreenSurface screenObject)
        {
            if (_activeScreenObject == screenObject)
            {
                return;
            }

            if (_screenObjects.Count != 0)
            {
                _screenObjects.Remove(_screenObjects.Last());
            }

            Push(screenObject);
        }

        /// <summary>
        /// Removes the screen object from the active stack. If the instance is the current active screen object, the active screen object is set to the last screen object in the previous screen object.
        /// </summary>
        /// <param name="screenObject">The screen object to remove.</param>
        public void Pop(IScreenSurface screenObject)
        {
            if (screenObject == _activeScreenObject)
            {
                _activeScreenObject.OnFocusLost();
                _screenObjects.Remove(screenObject);

                if (_screenObjects.Count != 0)
                {
                    _activeScreenObject = _screenObjects.Last();
                    _activeScreenObject.OnFocused();
                }
                else
                {
                    _activeScreenObject = null;
                }
            }
            else
            {
                _screenObjects.Remove(screenObject);
            }
        }

        /// <summary>
        /// Removes the top screen object from the stack.
        /// </summary>
        public void Pop()
        {
            if (_screenObjects.Count != 0)
            {
                Pop(_screenObjects.Last());
            }
        }

        public static bool operator !=(FocusedScreenObjectStack left, IScreenSurface right) => left._activeScreenObject != right;

        public static bool operator ==(FocusedScreenObjectStack left, IScreenSurface right) => left._activeScreenObject == right;
    }
}
