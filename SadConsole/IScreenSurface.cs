using System;
using SadConsole.Input;
using SadConsole.Renderers;

namespace SadConsole
{
    /// <summary>
    /// A screen object that has mouse input, surface, and render information.
    /// </summary>
    public interface IScreenSurface: IScreenObject, ISurfaceRenderData
    {
        /// <summary>
        /// Raised when the a mouse button is clicked on this console.
        /// </summary>
        event EventHandler<MouseScreenObjectState> MouseButtonClicked;

        /// <summary>
        /// Raised when the mouse enters this console.
        /// </summary>
        event EventHandler<MouseScreenObjectState> MouseEnter;

        /// <summary>
        /// Raised when the mouse exits this console.
        /// </summary>
        event EventHandler<MouseScreenObjectState> MouseExit;

        /// <summary>
        /// Raised when the mouse moves around the this console.
        /// </summary>
        event EventHandler<MouseScreenObjectState> MouseMove;

        /// <summary>
        /// How the object should handle becoming active.
        /// </summary>
        FocusBehavior FocusedMode { get; set; }

        /// <summary>
        /// When true, this console will set <see cref="IsFocused"/> to true when the mouse is clicked.
        /// </summary>
        bool FocusOnMouseClick { get; set; }

        /// <summary>
        /// The height of the surface in pixels.
        /// </summary>
        int HeightPixels { get; }

        /// <summary>
        /// Gets or sets whether or not this console has exclusive access to the mouse events.
        /// </summary>
        bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Gets or sets this console as the focused object for input.
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// When true, this console will move to the front of its parent console when the mouse is clicked.
        /// </summary>
        bool MoveToFrontOnMouseClick { get; set; }

        /// <summary>
        /// The renderer used to draw this surface.
        /// </summary>
        IRenderer Renderer { get; set; }

        /// <summary>
        /// Treats the <see cref="IScreenObject.Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        bool UsePixelPositioning { get; set; }

        /// <summary>
        /// The width of the surface in pixels.
        /// </summary>
        int WidthPixels { get; }

        /// <summary>
        /// Called when the mouse is no longer over the object.
        /// </summary>
        /// <param name="state">The current state of the mouse based on this object.</param>
        void LostMouse(MouseScreenObjectState state);

        /// <summary>
        /// Called when this object is focused.
        /// </summary>
        void OnFocused();

        /// <summary>
        /// Called when this object's focus has been lost.
        /// </summary>
        void OnFocusLost();

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state">The mouse state related to this console.</param>
        /// <returns>True when the mouse is over this console and processing should stop.</returns>
        bool ProcessMouse(MouseScreenObjectState state);
    }
}
