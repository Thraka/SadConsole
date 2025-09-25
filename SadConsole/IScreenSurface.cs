using System;
using SadConsole.Input;
using SadConsole.Renderers;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A screen object that has mouse input, surface, and render information.
/// </summary>
public interface IScreenSurface : IScreenObject
{
    /// <summary>
    /// Raised when a mouse button is clicked on this object.
    /// </summary>
    event EventHandler<MouseScreenObjectState> MouseButtonClicked;

    /// <summary>
    /// Raised when the mouse enters this object.
    /// </summary>
    event EventHandler<MouseScreenObjectState> MouseEnter;

    /// <summary>
    /// Raised when the mouse exits this object.
    /// </summary>
    event EventHandler<MouseScreenObjectState> MouseExit;

    /// <summary>
    /// Raised when the mouse moves around the this object.
    /// </summary>
    event EventHandler<MouseScreenObjectState> MouseMove;

    /// <summary>
    /// The name of the default renderer for this object.
    /// </summary>
    public string DefaultRendererName { get; }

    /// <summary>
    /// When true, this object will set <see cref="IScreenObject.IsFocused"/> to true when the mouse is clicked.
    /// </summary>
    bool FocusOnMouseClick { get; set; }

    /// <summary>
    /// The height of the surface in pixels.
    /// </summary>
    int HeightPixels { get; }

    /// <summary>
    /// When true, this object will move to the front of its parent object when the mouse is clicked.
    /// </summary>
    bool MoveToFrontOnMouseClick { get; set; }

    /// <summary>
    /// The renderer used to draw this surface.
    /// </summary>
    IRenderer? Renderer { get; }

    /// <summary>
    /// The surface.
    /// </summary>
    ICellSurface Surface { get; }

    /// <summary>
    /// Treats the <see cref="IPositionable.Position"/> of the object as if it is pixels and not cells.
    /// </summary>
    bool UsePixelPositioning { get; set; }

    /// <summary>
    /// The width of the surface in pixels.
    /// </summary>
    int WidthPixels { get; }

    /// <summary>
    /// The pixel area on the screen this surface occupies.
    /// </summary>
    Rectangle AbsoluteArea { get; }

    /// <summary>
    /// Font used with rendering.
    /// </summary>
    IFont Font { get; set; }

    /// <summary>
    /// The size of the <see cref="Font"/> cells applied to the object when rendering.
    /// </summary>
    Point FontSize { get; set; }

    /// <summary>
    /// When <see langword="true"/>, forces the <see cref="Renderer"/> to refresh the backing texture with the latest state of the object.
    /// </summary>
    bool ForceRendererRefresh { get; set; }

    /// <summary>
    /// Indicates the surface has changed and needs to be rendered.
    /// </summary>
    bool IsDirty { get; set; }

    /// <summary>
    /// A tint used in rendering.
    /// </summary>
    Color Tint { get; set; }
}
