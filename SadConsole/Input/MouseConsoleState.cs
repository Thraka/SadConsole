using SadRogue.Primitives;

namespace SadConsole.Input
{
    /// <summary>
    /// The state of the mouse.
    /// </summary>
    public class MouseScreenObjectState
    {
        /// <summary>
        /// The screen object used to create the mouse state.
        /// </summary>
        public readonly IScreenObjectSurface ScreenObject;

        /// <summary>
        /// The mouse data.
        /// </summary>
        public readonly Mouse Mouse;

        /// <summary>
        /// The cell the mouse is over, from <see cref="IScreenObject"/>.
        /// </summary>
        public readonly ColoredGlyph Cell;

        /// <summary>
        /// The position of the <see cref="Cell"/>.  
        /// </summary>
        public readonly Point CellPosition;

        /// <summary>
        /// The position of the mouse on the <see cref="IScreenObject"/>, based on the <see cref="WorldCellPosition"/> and the position of the <see cref="ScreenObject"/>.
        /// </summary>
        public readonly Point SurfaceCellPosition;

        /// <summary>
        /// A cell-based location of the mouse based on the screen, not the screen object.
        /// </summary>
        public readonly Point WorldCellPosition;

        /// <summary>
        /// The mouse position in pixels on the screen object.
        /// </summary>
        public readonly Point SurfacePixelPosition;

        /// <summary>
        /// Indicates that the mouse is within the bounds of <see cref="ScreenObject"/>.
        /// </summary>
        public readonly bool IsOnScreenObject;

        /// <summary>
        /// Calculates a new <see cref="MouseScreenObjectState"/> based on an <see cref="ScreenObject"/> and <see cref="Mouse"/> state.
        /// </summary>
        /// <param name="screenObject">The screen object to process with the mouse state.</param>
        /// <param name="mouseData">The current mouse state.</param>
        public MouseScreenObjectState(IScreenObjectSurface screenObject, Mouse mouseData)
        {
            bool isNegative = false;
            Mouse = mouseData.Clone();
            ScreenObject = screenObject;

            if (screenObject != null)
            {
                if (screenObject.UsePixelPositioning)
                {
                    WorldCellPosition = mouseData.ScreenPosition.PixelLocationToSurface(screenObject.FontSize);
                    SurfacePixelPosition = mouseData.ScreenPosition - screenObject.AbsolutePosition;

                    if (SurfacePixelPosition.X < 0 || SurfacePixelPosition.Y < 0)
                    {
                        isNegative = true;
                    }

                    SurfaceCellPosition = SurfacePixelPosition.PixelLocationToSurface(screenObject.FontSize);
                }
                else
                {
                    WorldCellPosition = mouseData.ScreenPosition.PixelLocationToSurface(screenObject.FontSize);
                    SurfacePixelPosition = mouseData.ScreenPosition - screenObject.AbsolutePosition;

                    if (SurfacePixelPosition.X < 0 || SurfacePixelPosition.Y < 0)
                    {
                        isNegative = true;
                    }

                    SurfaceCellPosition = SurfacePixelPosition.PixelLocationToSurface(screenObject.FontSize);
                }

                if (isNegative)
                {
                    IsOnScreenObject = false;
                    return;
                }

                if (screenObject.Surface.IsScrollable)
                {
                    Point tempCellPosition = SurfaceCellPosition + screenObject.Surface.ViewPosition;
                    IsOnScreenObject = screenObject.Surface.GetViewRectangle().Contains(tempCellPosition);

                    if (IsOnScreenObject)
                    {
                        CellPosition = tempCellPosition;
                        Cell = screenObject.Surface[CellPosition.X, CellPosition.Y];
                    }
                }
                else
                {
                    if (screenObject.Surface.IsValidCell(SurfaceCellPosition.X, SurfaceCellPosition.Y))
                    {
                        IsOnScreenObject = true;
                        CellPosition = SurfaceCellPosition;
                        Cell = ScreenObject.Surface[SurfaceCellPosition.X, SurfaceCellPosition.Y];
                    }
                }
            }
        }

        private MouseScreenObjectState(MouseScreenObjectState clonedCopy)
        {
            ScreenObject = clonedCopy.ScreenObject;
            Mouse = clonedCopy.Mouse.Clone();
            Cell = clonedCopy.Cell;
            SurfaceCellPosition = clonedCopy.SurfaceCellPosition;
            CellPosition = clonedCopy.CellPosition;
            WorldCellPosition = clonedCopy.WorldCellPosition;
            SurfacePixelPosition = clonedCopy.SurfacePixelPosition;
            IsOnScreenObject = clonedCopy.IsOnScreenObject;
        }

        /// <summary>
        /// Creates a copy.
        /// </summary>
        /// <returns>A copy of this class instance.</returns>
        public MouseScreenObjectState Clone() => new MouseScreenObjectState(this);
    }
}
