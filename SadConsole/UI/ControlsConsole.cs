using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadConsole.UI.Themes;
using SadConsole.UI.Controls;
using Keyboard = SadConsole.Input.Keyboard;

namespace SadConsole.UI
{
    /// <summary>
    /// A basic console that can contain controls.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("Console (Controls)")]
    public class ControlsConsole : Console
    {
        /// <summary>
        /// The controls host holding all the controls.
        /// </summary>
        public ControlHost ControlHostComponent { get; }

        /// <summary>
        /// Creates a new console.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        public ControlsConsole(int width, int height) : this(width, height, width, height, null) { }

        /// <summary>
        /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
        /// </summary>
        /// <param name="width">The width in cells of the surface.</param>
        /// <param name="height">The height in cells of the surface.</param>
        /// <param name="initialCells">The initial cells to seed the surface.</param>
        public ControlsConsole(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells) { }

        /// <summary>
        /// Creates a new console with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The visible width of the console in cells.</param>
        /// <param name="height">The visible height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        public ControlsConsole(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

        /// <summary>
        /// Creates a new console using the specified surface's cells.
        /// </summary>
        /// <param name="surface">The surface.</param>
        public ControlsConsole(ICellSurface surface) : this(surface.View.Width, surface.View.Height, surface.BufferWidth, surface.BufferHeight, surface.Cells) { }

        /// <summary>
        /// Creates a console with the specified width and height, with <see cref="SadRogue.Primitives.Color.Transparent"/> for the background and <see cref="SadRogue.Primitives.Color.White"/> for the foreground.
        /// </summary>
        /// <param name="width">The width of the console in cells.</param>
        /// <param name="height">The height of the console in cells.</param>
        /// <param name="bufferWidth">The total width of the console in cells.</param>
        /// <param name="bufferHeight">The total height of the console in cells.</param>
        /// <param name="initialCells">The cells to seed the console with. If <see langword="null"/>, creates the cells for you.</param>
        public ControlsConsole(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
        {
            ControlHostComponent = new ControlHost();
            SadComponents.Add(ControlHostComponent);
        }
    }
}
