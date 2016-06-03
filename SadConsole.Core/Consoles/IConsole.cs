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

        TextSurface Data { get; }

        Rectangle ViewArea { get; set; }

        Cursor VirtualCursor { get; }

        IConsoleList Parent { get; set; }

        bool UsePixelPositioning { get; set; }
    }
}
