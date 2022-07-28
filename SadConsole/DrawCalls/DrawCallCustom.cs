using System;

namespace SadConsole.DrawCalls;

/// <summary>
/// A draw call that invokes an <see cref="Action"/> delegate.
/// </summary>
public class DrawCallCustom : IDrawCall
{
    /// <summary>
    /// The delegate to call.
    /// </summary>
    public Action DrawCallback { get; set; }

    /// <summary>
    /// Creates a new instance of this object.
    /// </summary>
    /// <param name="draw">The delegate to call when the draw call is drawn.</param>
    public DrawCallCustom(Action draw) =>
        DrawCallback = draw;

    /// <summary>
    /// Invokes <see cref="DrawCallback"/>.
    /// </summary>
    public void Draw() =>
        DrawCallback();
}
