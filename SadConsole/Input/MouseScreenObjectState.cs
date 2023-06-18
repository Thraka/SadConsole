using System.Diagnostics.CodeAnalysis;
using SadRogue.Primitives;

namespace SadConsole.Input;

/// <summary>
/// The state of the mouse.
/// </summary>
public class MouseScreenObjectState
{
    /// <summary>
    /// The screen object used to create the mouse state.
    /// </summary>
    public IScreenObject? ScreenObject { get; private set; }

    /// <summary>
    /// The mouse data.
    /// </summary>
    public Mouse Mouse { get; private set; }

    /// <summary>
    /// The cell the mouse is over, from <see cref="IScreenObject"/>.
    /// </summary>
    public ColoredGlyph? Cell { get; set; }

    /// <summary>
    /// The position of the <see cref="Cell"/>.  
    /// </summary>
    public Point CellPosition { get; set; }

    /// <summary>
    /// The position of the mouse on the <see cref="IScreenObject"/>, based on the <see cref="WorldCellPosition"/> and the position of the <see cref="ScreenObject"/>.
    /// </summary>
    public Point SurfaceCellPosition { get; set; }

    /// <summary>
    /// A cell-based location of the mouse based on the screen, not the screen object.
    /// </summary>
    public Point WorldCellPosition { get; set; }

    /// <summary>
    /// The mouse position in pixels on the screen object.
    /// </summary>
    public Point SurfacePixelPosition { get; set; }

    /// <summary>
    /// Indicates that the mouse is within the bounds of <see cref="ScreenObject"/>.
    /// </summary>
    public bool IsOnScreenObject { get; set; }

    /// <summary>
    /// Calculates a new <see cref="MouseScreenObjectState"/> based on an <see cref="IScreenObject"/> and <see cref="SadConsole.Input.Mouse"/> state.
    /// </summary>
    /// <param name="screenObject">The screen object to process with the mouse state.</param>
    /// <param name="mouseData">The current mouse state.</param>
    public MouseScreenObjectState(IScreenObject? screenObject, Mouse mouseData) =>
        Refresh(screenObject, mouseData);

    /// <summary>
    /// Refreshes the mouse data in this object based on the parameters provided.
    /// </summary>
    /// <param name="screenObject">The screen object to process with the mouse state.</param>
    /// <param name="mouseData">The current mouse state.</param>
    [MemberNotNull("Mouse")]
    public void Refresh(IScreenObject? screenObject, Mouse mouseData)
    {
        bool isNegative = false;
        Mouse = mouseData;
        ScreenObject = screenObject;
        Cell = null;
        CellPosition = default;
        SurfaceCellPosition = default;
        WorldCellPosition = default;
        SurfacePixelPosition = default;
        IsOnScreenObject = false;

        if (screenObject != null && screenObject is IScreenSurface screenSurface)
        {
            if (screenSurface.UsePixelPositioning)
            {
                WorldCellPosition = mouseData.ScreenPosition.PixelLocationToSurface(screenSurface.FontSize);
                SurfacePixelPosition = mouseData.ScreenPosition - screenSurface.AbsolutePosition;

                if (SurfacePixelPosition.X < 0 || SurfacePixelPosition.Y < 0)
                {
                    isNegative = true;
                }

                SurfaceCellPosition = SurfacePixelPosition.PixelLocationToSurface(screenSurface.FontSize);
            }
            else
            {
                WorldCellPosition = mouseData.ScreenPosition.PixelLocationToSurface(screenSurface.FontSize);
                SurfacePixelPosition = mouseData.ScreenPosition - screenSurface.AbsolutePosition;

                if (SurfacePixelPosition.X < 0 || SurfacePixelPosition.Y < 0)
                {
                    isNegative = true;
                }

                SurfaceCellPosition = SurfacePixelPosition.PixelLocationToSurface(screenSurface.FontSize);
            }

            if (isNegative)
            {
                IsOnScreenObject = false;
                return;
            }

            if (screenSurface.Surface.IsScrollable)
            {
                Point tempCellPosition = SurfaceCellPosition + screenSurface.Surface.ViewPosition;
                IsOnScreenObject = screenSurface.Surface.View.Contains(tempCellPosition);

                if (IsOnScreenObject)
                {
                    CellPosition = tempCellPosition;
                    Cell = screenSurface.Surface[CellPosition.X, CellPosition.Y];
                }
            }
            else
            {
                if (screenSurface.Surface.IsValidCell(SurfaceCellPosition.X, SurfaceCellPosition.Y))
                {
                    IsOnScreenObject = true;
                    CellPosition = SurfaceCellPosition;
                    Cell = screenSurface.Surface[SurfaceCellPosition.X, SurfaceCellPosition.Y];
                }
            }
        }
    }

    private MouseScreenObjectState(MouseScreenObjectState cloneSource)
    {
        ScreenObject = cloneSource.ScreenObject;
        Mouse = cloneSource.Mouse.Clone();
        Cell = cloneSource.Cell;
        SurfaceCellPosition = cloneSource.SurfaceCellPosition;
        CellPosition = cloneSource.CellPosition;
        WorldCellPosition = cloneSource.WorldCellPosition;
        SurfacePixelPosition = cloneSource.SurfacePixelPosition;
        IsOnScreenObject = cloneSource.IsOnScreenObject;
    }

    /// <summary>
    /// Creates a copy.
    /// </summary>
    /// <returns>A copy of this class instance.</returns>
    public MouseScreenObjectState Clone() => new(this);
}
