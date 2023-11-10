using System;
using SadConsole.Input;

namespace SadConsole.Components
{
    /// <summary>
    /// Moves the view of an <see cref="IScreenSurface"/> with a set of specified keyboard keys.
    /// </summary>
    public class MoveViewPortKeyboardHandler : KeyboardConsoleComponent
    {
        private int _originalWidth;
        private int _originalHeight;

        /// <summary>
        /// The key to move left.
        /// </summary>
        public Keys Left { get; set; }

        /// <summary>
        /// The key to move right.
        /// </summary>
        public Keys Right { get; set; }

        /// <summary>
        /// The key to move up.
        /// </summary>
        public Keys Up { get; set; }

        /// <summary>
        /// The key to move down.
        /// </summary>
        public Keys Down { get; set; }

        /// <summary>
        /// Creates this handler with the arrow keys.
        /// </summary>
        public MoveViewPortKeyboardHandler() : this(Keys.Left, Keys.Right, Keys.Up, Keys.Down) { }

        /// <summary>
        /// Creates this handler with the specified keys.
        /// </summary>
        /// <param name="left">The key to move left.</param>
        /// <param name="right">The key to move right.</param>
        /// <param name="up">The key to move up.</param>
        /// <param name="down">The key to move down.</param>
        public MoveViewPortKeyboardHandler(Keys left, Keys right, Keys up, Keys down) =>
            (Left, Right, Up, Down) = (left, right, up, down);

        /// <inheritdoc/>
        public override void OnAdded(IScreenObject console)
        {
            if (console is IScreenSurface con)
            {
                _originalWidth = con.Surface.Width;
                _originalHeight = con.Surface.Height;
            }
            else
            {
                throw new Exception($"{nameof(MoveViewPortKeyboardHandler)} can only be used on {nameof(IScreenSurface)}");
            }
        }

        /// <inheritdoc/>
        public override void ProcessKeyboard(IScreenObject consoleObject, SadConsole.Input.Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            var console = (IScreenSurface)consoleObject;

            if (info.IsKeyDown(Left))
                console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((-1, 0));

            if (info.IsKeyDown(Right))
                console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((1, 0));

            if (info.IsKeyDown(Up))
                console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((0, -1));

            if (info.IsKeyDown(Down))
                console.Surface.ViewPosition = console.Surface.ViewPosition.Translate((0, 1));

            handled = true;
        }
    }
}
