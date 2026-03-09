using System;
using SadConsole.Renderers;
using SadConsole.Terminal;

namespace SadConsole;

/// <summary>
/// A surface that wires together a <see cref="Terminal.Writer"/> and <see cref="Terminal.TerminalCursor"/> for ANSI/VT terminal emulation.
/// Inherits from <see cref="ScreenSurface"/> directly — does not use <see cref="Components.Cursor"/> or <see cref="Console"/>.
/// </summary>
public class TerminalConsole : ScreenSurface
{
    private IRenderStep? _cursorRenderStep;

    /// <summary>
    /// The terminal writer that renders parsed ANSI/VT sequences onto this surface.
    /// </summary>
    public Writer Writer { get; }

    /// <summary>
    /// The terminal cursor used for visual display of the cursor position.
    /// </summary>
    public TerminalCursor TerminalCursor { get; }

    /// <summary>
    /// The keyboard encoder that converts SadConsole key events into ANSI escape sequences.
    /// </summary>
    public KeyboardEncoder KeyboardEncoder { get; }

    /// <summary>
    /// Gets or sets the terminal output channel for interactive mode.
    /// When set, terminal query responses (DA, DSR) are sent through this channel,
    /// and it is also wired to <see cref="Writer"/>.<see cref="Writer.Output"/>.
    /// When <see langword="null"/>, the terminal operates in silent data-stream mode.
    /// </summary>
    public ITerminalOutput? Output
    {
        get => Writer.Output;
        set => Writer.Output = value;
    }

    /// <summary>
    /// Creates a new terminal console with the specified dimensions, using the default font.
    /// </summary>
    /// <param name="width">The width of the surface in cells.</param>
    /// <param name="height">The height of the surface in cells.</param>
    public TerminalConsole(int width, int height) : this(width, height, null) { }

    /// <summary>
    /// Creates a new terminal console with the specified dimensions and font.
    /// </summary>
    /// <param name="width">The width of the surface in cells.</param>
    /// <param name="height">The height of the surface in cells.</param>
    /// <param name="font">The font to use. If <see langword="null"/>, uses the default font.</param>
    public TerminalConsole(int width, int height, IFont? font) : base(width, height)
    {
        if (font != null)
            Font = font;

        TerminalCursor = new TerminalCursor();
        KeyboardEncoder = new KeyboardEncoder();

        Writer = new Writer(Surface, Font) { Cursor = TerminalCursor };

        UseKeyboard = Settings.DefaultConsoleUseKeyboard;

        AddCursorRenderStep();
    }

    /// <summary>
    /// Feeds a string through the terminal parser and renders it onto the surface.
    /// </summary>
    /// <param name="text">The text or ANSI escape sequences to process.</param>
    public void Feed(string text) => Writer.Feed(text);

    /// <summary>
    /// Feeds raw bytes through the terminal parser and renders them onto the surface.
    /// </summary>
    /// <param name="data">The raw byte data to process.</param>
    public void Feed(ReadOnlySpan<byte> data) => Writer.Feed(data);

    /// <inheritdoc/>
    public override bool ProcessKeyboard(Input.Keyboard keyboard)
    {
        // Let components handle first
        if (base.ProcessKeyboard(keyboard))
            return true;

        // Sync DECCKM state from Writer → KeyboardEncoder so arrow key mode stays consistent
        KeyboardEncoder.ApplicationCursorKeys = Writer.State.CursorKeyMode;

        // Encode pressed keys into ANSI sequences
        byte[] encoded = KeyboardEncoder.Encode(keyboard);

        if (encoded.Length > 0)
        {
            // Feed the encoded sequences back through Writer so the terminal processes its own input
            Writer.Feed(encoded.AsSpan());
        }

        return true;
    }

    /// <inheritdoc/>
    public override void OnFocusLost()
    {
        TerminalCursor.IsVisible = false;
        IsDirty = true;
    }

    /// <inheritdoc/>
    public override void OnFocused()
    {
        TerminalCursor.IsVisible = true;
        IsDirty = true;
    }

    /// <inheritdoc/>
    protected override void OnRendererChanged()
    {
        base.OnRendererChanged();

        // Re-add the cursor render step to the new renderer
        AddCursorRenderStep();
    }

    /// <summary>
    /// Returns the value "TerminalConsole".
    /// </summary>
    /// <returns>The string "TerminalConsole".</returns>
    public override string ToString() => "TerminalConsole";

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cursorRenderStep?.Dispose();
            _cursorRenderStep = null;
        }

        base.Dispose(disposing);
    }

    private void AddCursorRenderStep()
    {
        _cursorRenderStep?.Dispose();
        _cursorRenderStep = GameHost.Instance.GetRendererStep(Renderers.Constants.RenderStepNames.TerminalCursor);
        _cursorRenderStep.SetData(TerminalCursor);
        Renderer?.Steps.Add(_cursorRenderStep);
        Renderer?.Steps.Sort(RenderStepComparer.Instance);
    }
}
