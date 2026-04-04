using Coroutine;
using SadConsole.Components;

namespace SadConsole.Examples;

internal class DemoCoroutines : IDemo
{
    public string Title => "Coroutines";

    public string Description => "Demonstrates the [c:r f:yellow]Coroutine[c:u] library integrated with SadConsole via [c:r f:yellow]CoroutineHandlerComponent[c:u].\r\n\r\nShows time-based waits, concurrent coroutines, event signaling, and cancellation.\r\n\r\nPress [c:r f:Red:5]Space to restart the demo.";

    public string CodeFile => "DemoCoroutine.cs";

    public IScreenSurface CreateDemoScreen() =>
        new CoroutineSurface();

    public override string ToString() =>
        Title;
}

// ──────────────────────────────────────────────────────────────────────────────
// HOW COROUTINES WORK IN SADCONSOLE
// ──────────────────────────────────────────────────────────────────────────────
//
// SadConsole integrates the "Coroutine" NuGet package (by Ellpeck) through the
// CoroutineHandlerComponent class. A coroutine lets you write sequential logic
// that spans many frames without blocking the game loop.
//
// SETUP (3 steps)
// ────────────────
//  1. Add "using Coroutine;" to access Wait, Event, and ActiveCoroutine.
//  2. Create a CoroutineHandlerComponent and add it to your surface:
//
//         var handler = new CoroutineHandlerComponent();
//         SadComponents.Add(handler);
//
//     The component calls handler.Tick(delta) every Update, which advances
//     all active coroutines by the elapsed time.
//
//  3. Write a method that returns IEnumerable<Wait> and use "yield return"
//     to pause execution:
//
//         IEnumerable<Wait> MyCoroutine()
//         {
//             Surface.Print(0, 0, "Hello");
//             yield return new Wait(1.0);        // pause 1 second
//             Surface.Print(0, 1, "World");
//         }
//
//     Start it with:  handler.Start(MyCoroutine(), "OptionalName");
//
// TYPES OF WAITS
// ──────────────
//  • new Wait(double seconds)   – resumes after the given wall-clock time.
//  • new Wait(TimeSpan time)    – same, but accepts a TimeSpan.
//  • new Wait(Event evt)        – suspends until handler.RaiseEvent(evt) is
//                                  called from any other code or coroutine.
//
// RUNNING MULTIPLE COROUTINES
// ───────────────────────────
//  Call handler.Start() for each coroutine. They all run concurrently; each
//  one is independently advanced every tick. This demo starts 8+ coroutines
//  at once (border drawing, text typing, spinner, color wave, etc.).
//
// EVENTS (cross-coroutine signaling)
// ──────────────────────────────────
//  Create an Event instance:       var evt = new Event();
//  One coroutine waits on it:      yield return new Wait(evt);
//  Another coroutine (or code)
//  signals it when ready:          handler.RaiseEvent(evt);
//
//  See CountdownCoroutine (raises) and WaitForEventCoroutine (listens) below.
//
// CANCELLATION
// ────────────
//  handler.Start() returns an ActiveCoroutine reference. Call .Cancel() on it
//  to stop the coroutine immediately; all remaining yields and code are skipped.
//  You can also check .IsFinished or subscribe to .OnFinished.
//
//  See BlinkingTextCoroutine (infinite loop) and CancelAfterDelayCoroutine
//  (cancels it after 3 seconds) below.
//
// WHAT THIS DEMO SHOWS
// ────────────────────
//  1. DrawBorderCoroutine      – Animates a box border cell-by-cell (time waits)
//  2. TypeTextCoroutine        – Types characters one at a time (reusable helper)
//  3. CountdownCoroutine       – 5-second countdown, then raises an Event
//  4. WaitForEventCoroutine    – Suspended on an Event, reacts when signaled
//  5. SpinnerCoroutine         – Rapid-update animation running concurrently
//  6. ColorWaveCoroutine       – Per-frame HSL color cycling across a row
//  7. BlinkingTextCoroutine    – Infinite loop, cancelled externally
//  8. CancelAfterDelayCoroutine– Cancels another coroutine after a delay
//  9. SequentialCoroutine      – Waits, then sweep-reveals a message
//
// Press Space to cancel everything and restart the demo.
// ──────────────────────────────────────────────────────────────────────────────

internal class CoroutineSurface : ScreenSurface
{
    private readonly CoroutineHandlerComponent _coroutineHandler;
    private readonly Event _textFinishedEvent = new();
    private readonly List<ActiveCoroutine> _trackedCoroutines = [];

    // Status panel dimensions and position (bottom-right corner)
    private const int PanelWidth = 28;
    private const int PanelHeight = 14;
    private int PanelX => Surface.Width - PanelWidth - 1;
    private int PanelY => Surface.Height - PanelHeight - 1;

    // Glyphs used for the spinner animation
    private static readonly int[] SpinnerFrames = ['|', '/', '-', '\\'];

    public CoroutineSurface() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        IsFocused = true;
        UseKeyboard = true;

        _coroutineHandler = new CoroutineHandlerComponent();
        SadComponents.Add(_coroutineHandler);

        StartAllCoroutines();
    }

    public override bool ProcessKeyboard(SadConsole.Input.Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(SadConsole.Input.Keys.Space))
        {
            // Cancel all running coroutines and restart
            foreach (ActiveCoroutine c in _coroutineHandler.GetActiveCoroutines())
                c.Cancel();

            Surface.Clear();
            StartAllCoroutines();
            return true;
        }

        return base.ProcessKeyboard(keyboard);
    }

    private void StartAllCoroutines()
    {
        _trackedCoroutines.Clear();

        // 1) Animated border drawn over time
        _trackedCoroutines.Add(_coroutineHandler.Start(DrawBorderCoroutine(), "DrawBorder"));

        // 2) Type out a welcome message character by character
        _trackedCoroutines.Add(_coroutineHandler.Start(TypeTextCoroutine(2, 2,
            "Hello from a coroutine! Each character",
            Color.White, 0.04), "TypeLine1"));

        _trackedCoroutines.Add(_coroutineHandler.Start(TypeTextCoroutine(2, 3,
            "is printed with a timed yield/wait.",
            Color.LightGray, 0.04), "TypeLine2"));

        // 3) A second coroutine waits for the text event, then reacts
        _trackedCoroutines.Add(_coroutineHandler.Start(WaitForEventCoroutine(), "WaitForEvent"));

        // 4) Spinner that runs concurrently with everything else
        _trackedCoroutines.Add(_coroutineHandler.Start(SpinnerCoroutine(Surface.Width - 4, 1), "Spinner"));

        // 5) Countdown coroutine that fires the event when done
        _trackedCoroutines.Add(_coroutineHandler.Start(CountdownCoroutine(2, 5, 5), "Countdown"));

        // 6) Color wave effect across a row
        _trackedCoroutines.Add(_coroutineHandler.Start(ColorWaveCoroutine(8), "ColorWave"));

        // 7) A coroutine that starts, then gets cancelled by another coroutine
        ActiveCoroutine cancelTarget = _coroutineHandler.Start(BlinkingTextCoroutine(2, 14, "I will be cancelled..."), "BlinkTarget");
        _trackedCoroutines.Add(cancelTarget);
        _trackedCoroutines.Add(_coroutineHandler.Start(CancelAfterDelayCoroutine(cancelTarget, 3.0, 2, 15), "CancelDemo"));

        // 8) Sequential coroutine: waits for border to finish, then draws a message
        _trackedCoroutines.Add(_coroutineHandler.Start(SequentialCoroutine(), "Sequential"));

        // Status panel coroutine (not tracked — it draws the panel itself)
        _coroutineHandler.Start(StatusPanelCoroutine(), "StatusPanel");
    }

    /// <summary>
    /// Draws an animated border around the surface, one cell at a time.
    /// </summary>
    private IEnumerable<Wait> DrawBorderCoroutine()
    {
        int w = Surface.Width;
        int h = Surface.Height;
        double delay = 0.005;

        // Top edge
        for (int x = 0; x < w; x++)
        {
            Surface.SetGlyph(x, 0, 196, Color.Cyan); // ─
            yield return new Wait(delay);
        }
        // Right edge
        Surface.SetGlyph(w - 1, 0, 191, Color.Cyan); // ┐
        for (int y = 1; y < h; y++)
        {
            Surface.SetGlyph(w - 1, y, 179, Color.Cyan); // │
            yield return new Wait(delay);
        }
        // Bottom edge
        Surface.SetGlyph(w - 1, h - 1, 217, Color.Cyan); // ┘
        for (int x = w - 2; x >= 0; x--)
        {
            Surface.SetGlyph(x, h - 1, 196, Color.Cyan); // ─
            yield return new Wait(delay);
        }
        // Left edge
        Surface.SetGlyph(0, h - 1, 192, Color.Cyan); // └
        for (int y = h - 2; y >= 1; y--)
        {
            Surface.SetGlyph(0, y, 179, Color.Cyan); // │
            yield return new Wait(delay);
        }
        Surface.SetGlyph(0, 0, 218, Color.Cyan); // ┌

        IsDirty = true;
    }

    /// <summary>
    /// Types text one character at a time with a timed delay.
    /// </summary>
    private IEnumerable<Wait> TypeTextCoroutine(int x, int y, string text, Color color, double charDelay)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Surface.SetGlyph(x + i, y, text[i], color);
            IsDirty = true;
            yield return new Wait(charDelay);
        }
    }

    /// <summary>
    /// Counts down from a value, then raises the text-finished event.
    /// </summary>
    private IEnumerable<Wait> CountdownCoroutine(int x, int y, int seconds)
    {
        Surface.Print(x, y, "Countdown: ", Color.Yellow);

        for (int i = seconds; i > 0; i--)
        {
            Surface.Print(x + 11, y, $"{i} ", Color.White);
            IsDirty = true;
            yield return new Wait(1.0);
        }

        Surface.Print(x + 11, y, "GO!", Color.Green);
        IsDirty = true;

        // Signal the event so the waiting coroutine can continue
        _coroutineHandler.RaiseEvent(_textFinishedEvent);
    }

    /// <summary>
    /// Waits for the text-finished event, then prints a response.
    /// </summary>
    private IEnumerable<Wait> WaitForEventCoroutine()
    {
        Surface.Print(2, 7, "Waiting for countdown event...", Color.DarkGray);
        IsDirty = true;

        // This coroutine suspends until the event is raised
        yield return new Wait(_textFinishedEvent);

        Surface.Print(2, 7, "Event received! Continuing.   ", Color.LightGreen);
        IsDirty = true;

        // Flash the message a few times
        for (int i = 0; i < 6; i++)
        {
            Color c = i % 2 == 0 ? Color.Green : Color.LightGreen;
            Surface.Print(2, 7, "Event received! Continuing.   ", c);
            IsDirty = true;
            yield return new Wait(0.3);
        }
    }

    /// <summary>
    /// Shows a spinning character that updates rapidly.
    /// </summary>
    private IEnumerable<Wait> SpinnerCoroutine(int x, int y)
    {
        Surface.Print(x - 2, y, "[ ]", Color.Yellow);
        int frame = 0;

        // Runs 200 frames then stops
        for (int i = 0; i < 200; i++)
        {
            Surface.SetGlyph(x - 1, y, SpinnerFrames[frame], Color.White);
            IsDirty = true;
            frame = (frame + 1) % SpinnerFrames.Length;
            yield return new Wait(0.05);
        }

        Surface.SetGlyph(x - 1, y, '*', Color.Green);
        IsDirty = true;
    }

    /// <summary>
    /// Draws a color wave: fills a row with block characters that cycle hue over time.
    /// </summary>
    private IEnumerable<Wait> ColorWaveCoroutine(int y)
    {
        int startX = 2;
        int length = Surface.Width - 4;
        Surface.Print(startX, y, "Color wave:", Color.White);
        y++;

        // Initial draw
        for (int x = 0; x < length; x++)
            Surface.SetGlyph(startX + x, y, 219, Color.Black); // █

        // Animate 300 frames
        for (int frame = 0; frame < 300; frame++)
        {
            for (int x = 0; x < length; x++)
            {
                float hue = ((x + frame) * 5) % 360;
                Color c = Color.FromHSL(hue, 1f, 0.5f);
                Surface.SetGlyph(startX + x, y, 219, c); // █
            }
            IsDirty = true;
            yield return new Wait(0.03);
        }
    }

    /// <summary>
    /// Blinks text on and off until cancelled.
    /// </summary>
    private IEnumerable<Wait> BlinkingTextCoroutine(int x, int y, string text)
    {
        bool visible = true;
        while (true) // Runs forever until cancelled externally
        {
            Color color = visible ? Color.Red : Color.Transparent;
            Surface.Print(x, y, text, color);
            IsDirty = true;
            visible = !visible;
            yield return new Wait(0.4);
        }
    }

    /// <summary>
    /// Cancels another coroutine after a delay, demonstrating cancellation.
    /// </summary>
    private IEnumerable<Wait> CancelAfterDelayCoroutine(ActiveCoroutine target, double delay, int msgX, int msgY)
    {
        Surface.Print(msgX, msgY, $"(will cancel in {delay:0}s)", Color.DarkGray);
        IsDirty = true;

        yield return new Wait(delay);

        target.Cancel();

        // Clear the blinking text line and show a message
        Surface.Print(msgX, msgY - 1, new string(' ', 30));
        Surface.Print(msgX, msgY, "Coroutine cancelled!          ", Color.Orange);
        IsDirty = true;
    }

    /// <summary>
    /// Demonstrates sequential execution: waits for the border coroutine to
    /// likely finish (timed), then prints a final message at the bottom.
    /// </summary>
    private IEnumerable<Wait> SequentialCoroutine()
    {
        // Wait enough time for the border to complete
        yield return new Wait(2.0);
        string message = ">> All coroutines running concurrently! <<";
        int x = 4;
        int y = Surface.Height - 3;

        // Reveal message with a sweep effect
        for (int i = 0; i < message.Length; i++)
        {
            Surface.SetGlyph(x + i, y, message[i], Color.White, Color.DarkBlue);
            IsDirty = true;
            yield return new Wait(0.02);
        }
    }

    /// <summary>
    /// Continuously redraws a status panel in the bottom-right corner showing
    /// each tracked coroutine's name and current state (Running/Done/Cancelled).
    /// </summary>
    private IEnumerable<Wait> StatusPanelCoroutine()
    {
        int px = PanelX;
        int py = PanelY;
        int innerW = PanelWidth - 2;

        while (true)
        {
            // Draw panel border
            Surface.SetGlyph(px, py, 218, Color.DarkCyan);                    // ┌
            Surface.SetGlyph(px + PanelWidth - 1, py, 191, Color.DarkCyan);   // ┐
            Surface.SetGlyph(px, py + PanelHeight - 1, 192, Color.DarkCyan);  // └
            Surface.SetGlyph(px + PanelWidth - 1, py + PanelHeight - 1, 217, Color.DarkCyan); // ┘

            for (int i = 1; i < PanelWidth - 1; i++)
            {
                Surface.SetGlyph(px + i, py, 196, Color.DarkCyan);                    // ─ top
                Surface.SetGlyph(px + i, py + PanelHeight - 1, 196, Color.DarkCyan);   // ─ bottom
            }
            for (int i = 1; i < PanelHeight - 1; i++)
            {
                Surface.SetGlyph(px, py + i, 179, Color.DarkCyan);                    // │ left
                Surface.SetGlyph(px + PanelWidth - 1, py + i, 179, Color.DarkCyan);   // │ right
            }

            // Header row
            string header = " COROUTINE STATUS ";
            Surface.Print(px + (PanelWidth - header.Length) / 2, py, header, Color.White, Color.DarkCyan);

            // Separator line under header
            for (int i = 1; i < PanelWidth - 1; i++)
                Surface.SetGlyph(px + i, py + 1, 196, Color.DarkCyan); // ─
            Surface.SetGlyph(px, py + 1, 195, Color.DarkCyan);                    // ├
            Surface.SetGlyph(px + PanelWidth - 1, py + 1, 180, Color.DarkCyan);   // ┤

            // Draw each tracked coroutine's status
            for (int i = 0; i < _trackedCoroutines.Count; i++)
            {
                ActiveCoroutine co = _trackedCoroutines[i];
                int row = py + 2 + i;

                // Clear the inner row
                Surface.Print(px + 1, row, new string(' ', innerW));

                // Determine status text and color
                string status;
                Color statusColor;

                if (co.WasCanceled)
                {
                    status = "CANCEL";
                    statusColor = Color.Red;
                }
                else if (co.IsFinished)
                {
                    status = "DONE";
                    statusColor = Color.Green;
                }
                else
                {
                    status = "RUN";
                    statusColor = Color.Yellow;
                }

                // Print name (left-aligned) and status (right-aligned)
                string name = co.Name.Length > innerW - 8
                    ? co.Name[..(innerW - 8)]
                    : co.Name;

                Surface.Print(px + 2, row, name, Color.LightGray);
                Surface.Print(px + PanelWidth - 2 - status.Length, row, status, statusColor);
            }

            IsDirty = true;
            yield return new Wait(0.2);
        }
    }
}
