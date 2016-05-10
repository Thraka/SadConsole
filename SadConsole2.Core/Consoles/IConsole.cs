namespace SadConsole.Consoles
{
    using Microsoft.Xna.Framework;
    using SadConsole.Input;

    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IInput, IDraw
    {
        Point Position { get; set; }

        TextSurface Data { get; set; }
        
        Console.Cursor VirtualCursor { get; set; }

        IConsoleList Parent { get; set; }

        bool UsePixelPositioning { get; set; }
    }
}
