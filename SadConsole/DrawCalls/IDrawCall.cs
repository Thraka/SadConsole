namespace SadConsole.DrawCalls;

/// <summary>
/// A draw call used by final rendering.
/// </summary>
public interface IDrawCall
{
    /// <summary>
    /// Draws an object.
    /// </summary>
    void Draw();
}
