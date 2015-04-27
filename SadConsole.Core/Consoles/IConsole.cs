namespace SadConsole.Consoles
{
    using SadConsole.Input;

    /// <summary>
    /// Supports basic console management and input functionallity.
    /// </summary>
    public interface IConsole : IInput, IRender, IUpdate
    {
        Console.Cursor VirtualCursor { get; set; }
        IParentConsole Parent { get; set; }
    }
}
