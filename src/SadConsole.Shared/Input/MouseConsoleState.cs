#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endif

using System;

namespace SadConsole.Input
{
    public class MouseConsoleState
    {
        /// <summary>
        /// The console used to create this object.
        /// </summary>
        public readonly Console Console;

        /// <summary>
        /// The mouse state.
        /// </summary>
        public readonly Mouse Mouse;

        /// <summary>
        /// The cell the mouse is over, from <see cref="Console"/>.
        /// </summary>
        public readonly Cell Cell;

        /// <summary>
        /// The position of the mouse on the <see cref="Console"/>, based on the <see cref="WorldPosition"/>.
        /// </summary>
        public readonly Point ConsolePosition;

        /// <summary>
        /// The position of the <see cref="Cell"/>.  
        /// </summary>
        public readonly Point CellPosition;

        /// <summary>
        /// A screen-based location based on the <see cref="ConsolePosition"/>.
        /// </summary>
        public readonly Point WorldPosition;

        /// <summary>
        /// The <see cref="ConsolePosition"/> translated to pixels.
        /// </summary>
        public readonly Point RelativePixelPosition;

        /// <summary>
        /// Indicates that the mouse is within the bounds of <see cref="Console"/>.
        /// </summary>
        public readonly bool IsOnConsole;

        /// <summary>
        /// Calculates a new <see cref="MouseConsoleState"/> based on an <see cref="Console"/> and <see cref="Mouse"/> state.
        /// </summary>
        /// <param name="screenObject">The console to process with the mouse state.</param>
        /// <param name="mouseData">The current mouse state.</param>
        public MouseConsoleState(Console screenObject, Mouse mouseData)
        {
            this.Mouse = mouseData.Clone();
            this.Console = screenObject;

            if (screenObject != null)
            {
                if (screenObject.UsePixelPositioning)
                {
                    RelativePixelPosition = mouseData.ScreenPosition - screenObject.CalculatedPosition;
                    WorldPosition = mouseData.ScreenPosition;
                    ConsolePosition = RelativePixelPosition.PixelLocationToConsole(screenObject.Font);
                }
                else
                {
                    RelativePixelPosition = mouseData.ScreenPosition - screenObject.CalculatedPosition.ConsoleLocationToPixel(screenObject.Font);
                    WorldPosition = mouseData.ScreenPosition.PixelLocationToConsole(screenObject.Font);
                    ConsolePosition = WorldPosition - screenObject.CalculatedPosition;
                }

                if (screenObject is IConsoleViewPort viewObject)
                {
                    var tempCellPosition = ConsolePosition + viewObject.ViewPort.Location;
                    IsOnConsole = viewObject.ViewPort.Contains(tempCellPosition);

                    if (IsOnConsole)
                    {
                        CellPosition = tempCellPosition;
                        Cell = screenObject[CellPosition.X, CellPosition.Y];
                    }
                }
                else
                {
                    if (screenObject.IsValidCell(ConsolePosition.X, ConsolePosition.Y))
                    {
                        IsOnConsole = true;
                        CellPosition = ConsolePosition;
                        Cell = Console[ConsolePosition.X, ConsolePosition.Y];
                    }
                }
            }
        }

        private MouseConsoleState(MouseConsoleState clonedCopy)
        {
            Console = clonedCopy.Console;
            Mouse = clonedCopy.Mouse.Clone();
            Cell = clonedCopy.Cell;
            ConsolePosition = clonedCopy.ConsolePosition;
            CellPosition = clonedCopy.CellPosition;
            WorldPosition = clonedCopy.WorldPosition;
            RelativePixelPosition = clonedCopy.RelativePixelPosition;
            IsOnConsole = clonedCopy.IsOnConsole;
        }

        /// <summary>
        /// Creates a copy.
        /// </summary>
        /// <returns>A copy of this class instance.</returns>
        public MouseConsoleState Clone()
        {
            return new MouseConsoleState(this);
        }
    }
}
