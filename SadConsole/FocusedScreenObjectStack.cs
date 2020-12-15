using System.Collections.Generic;
using System.Linq;

namespace SadConsole
{
    /// <summary>
    /// A stack of screen objects. The top-most of the stack is considered active and represented by the <see cref="IScreenObject"/> property.
    /// </summary>
    public class FocusedScreenObjectStack
    {
        private IScreenObject _activeScreenObject;

        /// <summary>
        /// Gets the current active screen object.
        /// </summary>
        public IScreenObject ScreenObject => _activeScreenObject;

        /// <summary>
        /// The stack of screen objects for input processing.
        /// </summary>
        private readonly List<IScreenObject> _screenObjects;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public FocusedScreenObjectStack()
        {
            _screenObjects = new List<IScreenObject>();
            _activeScreenObject = null;
        }

        /// <summary>
        /// Clears all screen objects from the active stack along with the current active screen object.
        /// </summary>
        public void Clear()
        {
            if (_activeScreenObject == null) return;

            _screenObjects.Clear();

            IScreenObject temp = _activeScreenObject;
            _activeScreenObject = null;

            if (temp != null)
                temp.IsFocused = false;

        }

        /// <summary>
        /// Adds another screen object to active stack, setting it as the active (top most in the stack) screen object.
        /// </summary>
        /// <param name="screenObject"></param>
        public void Push(IScreenObject screenObject)
        {
            if (screenObject != _activeScreenObject && screenObject != null)
            {
                if (_screenObjects.Contains(screenObject))
                    _screenObjects.Remove(screenObject);

                if (_activeScreenObject != null)
                {
                    IScreenObject temp = _activeScreenObject;
                    _activeScreenObject = null;
                    temp.IsFocused = false;
                }

                _screenObjects.Add(screenObject);
                _activeScreenObject = screenObject;
                _activeScreenObject.IsFocused = true;
            }
        }

        /// <summary>
        /// Replaces the top screen object (active screen object) with the provided instance. Sets <see cref="IScreenObject"/> to this instance.
        /// </summary>
        /// <param name="screenObject">The screen object to make active.</param>
        public void Set(IScreenObject screenObject)
        {
            if (_activeScreenObject == screenObject) return;

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
        public void Pop(IScreenObject screenObject)
        {
            if (screenObject == _activeScreenObject)
            {
                _activeScreenObject = null;
                _screenObjects.Remove(screenObject);
                screenObject.IsFocused = false;

                if (_screenObjects.Count != 0)
                {
                    _activeScreenObject = _screenObjects.Last();
                    _activeScreenObject.IsFocused = true;
                }
            }
            else
                _screenObjects.Remove(screenObject);
        }

        /// <summary>
        /// Removes the top screen object from the stack.
        /// </summary>
        public void Pop()
        {
            if (_screenObjects.Count != 0)
                Pop(_screenObjects.Last());
        }
    }
}
