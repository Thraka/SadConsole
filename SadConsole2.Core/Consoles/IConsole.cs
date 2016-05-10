namespace SadConsole.Consoles
{
    using SadConsole.Input;

    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IInput, IUpdate
    {
        TextSurface Data { get; set; }

        ITextSurfaceView DataView { get; set; }

        Consoles.Console.Cursor VirtualCursor { get; set; }

        Consoles.IParentConsole Parent { get; set; }
    }
}
