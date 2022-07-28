using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SadConsole.Components;
using SadRogue.Primitives;

namespace SadConsole;

/// <summary>
/// A surface that has a cursor.
/// </summary>
[DataContract]
[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public partial class Console : ScreenSurface
{
    /// <summary>
    /// The virtual cursor reference.
    /// </summary>
    public Cursor Cursor { get; protected set; }

    /// <summary>
    /// Toggles the cursor as visible\hidden when the console if focused\unfocused.
    /// </summary>
    [DataMember]
    public bool AutoCursorOnFocus { get; set; }

    /// <summary>
    /// Creates a new console.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    public Console(int width, int height) : this(width, height, width, height, null) { }

    /// <summary>
    /// Creates a new screen object that can render a surface. Uses the specified cells to generate the surface.
    /// </summary>
    /// <param name="width">The width in cells of the surface.</param>
    /// <param name="height">The height in cells of the surface.</param>
    /// <param name="initialCells">The initial cells to seed the surface.</param>
    public Console(int width, int height, ColoredGlyph[] initialCells) : this(width, height, width, height, initialCells) { }

    /// <summary>
    /// Creates a new console with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="width">The visible width of the console in cells.</param>
    /// <param name="height">The visible height of the console in cells.</param>
    /// <param name="bufferWidth">The total width of the console in cells.</param>
    /// <param name="bufferHeight">The total height of the console in cells.</param>
    public Console(int width, int height, int bufferWidth, int bufferHeight) : this(width, height, bufferWidth, bufferHeight, null) { }

    /// <summary>
    /// Creates a console with the specified width and height, with <see cref="Color.Transparent"/> for the background and <see cref="Color.White"/> for the foreground.
    /// </summary>
    /// <param name="width">The width of the console in cells.</param>
    /// <param name="height">The height of the console in cells.</param>
    /// <param name="bufferWidth">The total width of the console in cells.</param>
    /// <param name="bufferHeight">The total height of the console in cells.</param>
    /// <param name="initialCells">The cells to seed the console with. If <see langword="null"/>, creates the cells for you.</param>
    public Console(int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells)
    {
        Cursor = new Cursor() { IsVisible = false, IsEnabled = false };
        SadComponents.Add(Cursor);
        UseKeyboard = Settings.DefaultConsoleUseKeyboard;
    }

    /// <summary>
    /// Creates a new console using the existing surface.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="font">The font to use with the surface.</param>
    /// <param name="fontSize">The font size.</param>
    public Console(ICellSurface surface, IFont font = null, Point? fontSize = null) : base(surface, font, fontSize)
    {
        Cursor = new Cursor() { IsVisible = false, IsEnabled = false };
        SadComponents.Add(Cursor);
        UseKeyboard = Settings.DefaultConsoleUseKeyboard;
    }

    private Console() : base(1, 1) { }

    /// <inheritdoc/>
    protected override void OnVisibleChanged()
    {
        if (!IsVisible && IsMouseOver)
            OnMouseExit(new Input.MouseScreenObjectState(this, SadConsole.GameHost.Instance.Mouse));

        base.OnVisibleChanged();
    }

    /// <summary>
    /// Called when this console's focus has been lost. Hides the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
    /// </summary>
    public override void OnFocusLost()
    {
        if (AutoCursorOnFocus)
            Cursor.IsVisible = false;
    }

    /// <summary>
    /// Called when this console is focused. Shows the <see cref="Cursor"/> if <see cref="AutoCursorOnFocus"/> is <see langword="true"/>.
    /// </summary>
    public override void OnFocused()
    {
        if (AutoCursorOnFocus)
            Cursor.IsVisible = true;
    }

    /// <summary>
    /// Returns the value "Console".
    /// </summary>
    /// <returns>The string "Console".</returns>
    public override string ToString() =>
        "Console";

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        Cursor = SadComponents.OfType<Cursor>().FirstOrDefault();

        if (Cursor == null)
            throw new System.Exception("Cursor not deserialized. Perhaps it was removed? A cursor should always be available on the console. Disable it instead of removing it.");
    }
}
