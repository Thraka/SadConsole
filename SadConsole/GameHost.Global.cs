using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;
using SadConsole.Renderers;
using System.IO;

namespace SadConsole
{
    /// <summary>
    /// Represents the SadConsole game engine.
    /// </summary>
    public abstract partial class GameHost : IDisposable
    {
        private GlobalState _state;
        protected DateTime _gameStartedAt = DateTime.Now;

        /// <summary>
        /// Collection of fonts. Used mainly by the deserialization system.
        /// </summary>
        public Dictionary<string, IFont> Fonts { get; } = new Dictionary<string, IFont>();

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font.
        /// </summary>
        public SadFont EmbeddedFont { get; internal set; }

        /// <summary>
        /// The font automatically loaded by SadConsole. Standard IBM style font. Extended with extra SadConsole characters.
        /// </summary>
        public SadFont EmbeddedFontExtended { get; internal set; }

        /// <summary>
        /// The default font for any type that does not provide a font.
        /// </summary>
        public IFont DefaultFont { get; set; }

        /// <summary>
        /// The default font to use with <see cref="DefaultFont"/>.
        /// </summary>
        public IFont.Sizes DefaultFontSize { get; set; } = IFont.Sizes.One;

        /// <summary>
        /// Global keyboard object used by SadConsole during the update frame.
        /// </summary>
        public Input.Keyboard Keyboard { get; } = new Input.Keyboard();

        /// <summary>
        /// Global mouse object used by SadConsole during the update frame.
        /// </summary>
        public Input.Mouse Mouse { get; } = new Input.Mouse();

        /// <summary>
        /// The elapsed time between now and the last update call.
        /// </summary>
        public TimeSpan UpdateFrameDelta { get; set; }

        /// <summary>
        /// The elapsed time between now and the last draw call.
        /// </summary>
        public TimeSpan DrawFrameDelta { get; set; }

        /// <summary>
        /// The total time the game has been running.
        /// </summary>
        public TimeSpan GameRunningTotalTime => DateTime.Now - _gameStartedAt;

        /// <summary>
        /// The console created by the game and automatically assigned to <see cref="Screen"/>.
        /// </summary>
        public Console StartingConsole { get; protected set; }

        /// <summary>
        /// The active screen processed by the game.
        /// </summary>
        public IScreenObject Screen { get; set; }

        /// <summary>
        /// The stack of focused consoles used by the mouse and keyboard.
        /// </summary>
        public FocusedScreenObjectStack FocusedScreenObjects { get; set; } = new FocusedScreenObjectStack();

        /// <summary>
        /// A global random number generator.
        /// </summary>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// Resizes the window to the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the window in pixels.</param>
        /// <param name="height">The height of the window in pixels.</param>
        public abstract void ResizeWindow(int width, int height);

        /// <summary>
        /// Resizes the window to the specified cell count along the X-axis and Y-axis.
        /// </summary>
        /// <param name="cellsX">The number of cells to fit horizontally.</param>
        /// <param name="cellsY">The number of cells to fit vertically.</param>
        /// <param name="cellSize">The size of the cells in pixels.</param>
        public void ResizeWindow(int cellsX, int cellsY, Point cellSize) =>
            ResizeWindow(cellsX * cellSize.X, cellsY * cellSize.Y);

        /// <summary>
        /// Saves the global state, mainly the <see cref="FocusedScreenObjects"/> and <see cref="Screen"/> objects.
        /// </summary>
        public void SaveGlobalState()
        {
            _state = new GlobalState()
            {
                FocusedScreenObjects = FocusedScreenObjects,
                Screen = Screen,
                DefaultFont = DefaultFont,
                DefaultFontSize = DefaultFontSize
            };
        }

        /// <summary>
        /// Restores the global state that was saved with <see cref="SaveGlobalState"/>.
        /// </summary>
        public void RestoreGlobalState()
        {
            if (_state == null) return;

            FocusedScreenObjects = _state.FocusedScreenObjects;
            Screen = _state.Screen;
            DefaultFont = _state.DefaultFont;
            DefaultFontSize = _state.DefaultFontSize;
        }

        private class GlobalState
        {
            public FocusedScreenObjectStack FocusedScreenObjects;
            public IScreenObject Screen;
            public IFont DefaultFont;
            public IFont.Sizes DefaultFontSize;
        }
    }
}
