#if XNA
using Microsoft.Xna.Framework;
#endif


namespace SadConsole.Input
{
    public class MouseConsoleState
    {
        /// <summary>
        /// The console used to create the mouse state.
        /// </summary>
        public readonly Console Console;

        /// <summary>
        /// The mouse data.
        /// </summary>
        public readonly Mouse Mouse;

        /// <summary>
        /// The cell the mouse is over, from <see cref="Console"/>.
        /// </summary>
        public readonly Cell Cell;

        /// <summary>
        /// The position of the <see cref="Cell"/>.  
        /// </summary>
        public readonly Point CellPosition;

        /// <summary>
        /// The position of the mouse on the <see cref="Console"/>, based on the <see cref="WorldCellPosition"/> and the position of the <see cref="Console"/>.
        /// </summary>
        public readonly Point ConsoleCellPosition;

        /// <summary>
        /// A cell-based location of the mouse based on the screen, not the console.
        /// </summary>
        public readonly Point WorldCellPosition;

        /// <summary>
        /// The mouse position in pixels on the console.
        /// </summary>
        public readonly Point ConsolePixelPosition;

        /// <summary>
        /// Indicates that the mouse is within the bounds of <see cref="Console"/>.
        /// </summary>
        public readonly bool IsOnConsole;

        /// <summary>
        /// Calculates a new <see cref="MouseConsoleState"/> based on an <see cref="Console"/> and <see cref="Mouse"/> state.
        /// </summary>
        /// <param name="console">The console to process with the mouse state.</param>
        /// <param name="mouseData">The current mouse state.</param>
        public MouseConsoleState(Console console, Mouse mouseData)
        {
            bool isNegative = false;
            Mouse = mouseData.Clone();
            Console = console;

            if (console != null)
            {
                if (console.UsePixelPositioning)
                {
                    WorldCellPosition = mouseData.ScreenPosition.PixelLocationToConsole(console.Font);
                    ConsolePixelPosition = mouseData.ScreenPosition - console.CalculatedPosition;

                    if (ConsolePixelPosition.X < 0 || ConsolePixelPosition.Y < 0)
                    {
                        isNegative = true;
                    }

                    ConsoleCellPosition = ConsolePixelPosition.PixelLocationToConsole(console.Font);
                }
                else
                {
                    WorldCellPosition = mouseData.ScreenPosition.PixelLocationToConsole(console.Font);
                    ConsolePixelPosition = mouseData.ScreenPosition - console.CalculatedPosition;

                    if (ConsolePixelPosition.X < 0 || ConsolePixelPosition.Y < 0)
                    {
                        isNegative = true;
                    }

                    ConsoleCellPosition = ConsolePixelPosition.PixelLocationToConsole(console.Font);
                }

                if (isNegative)
                {
                    IsOnConsole = false;
                    return;
                }

                if (console is IConsoleViewPort viewObject)
                {
                    Point tempCellPosition = ConsoleCellPosition + viewObject.ViewPort.Location;
                    IsOnConsole = viewObject.ViewPort.Contains(tempCellPosition);

                    if (IsOnConsole)
                    {
                        CellPosition = tempCellPosition;
                        Cell = console[CellPosition.X, CellPosition.Y];
                    }
                }
                else
                {
                    if (console.IsValidCell(ConsoleCellPosition.X, ConsoleCellPosition.Y))
                    {
                        IsOnConsole = true;
                        CellPosition = ConsoleCellPosition;
                        Cell = Console[ConsoleCellPosition.X, ConsoleCellPosition.Y];
                    }
                }
            }
        }

        private MouseConsoleState(MouseConsoleState clonedCopy)
        {
            Console = clonedCopy.Console;
            Mouse = clonedCopy.Mouse.Clone();
            Cell = clonedCopy.Cell;
            ConsoleCellPosition = clonedCopy.ConsoleCellPosition;
            CellPosition = clonedCopy.CellPosition;
            WorldCellPosition = clonedCopy.WorldCellPosition;
            ConsolePixelPosition = clonedCopy.ConsolePixelPosition;
            IsOnConsole = clonedCopy.IsOnConsole;
        }

        /// <summary>
        /// Creates a copy.
        /// </summary>
        /// <returns>A copy of this class instance.</returns>
        public MouseConsoleState Clone() => new MouseConsoleState(this);
    }
}
