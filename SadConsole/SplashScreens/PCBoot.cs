using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.SplashScreens;

/// <summary>
/// A spashscreen that simulates an old computer boot up process.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Splashscreen: PC Boot")]
public class PCBoot : ScreenSurface
{
    private Instructions.InstructionSet _endAnimation;
    private Instructions.InstructionSet _startAnimation;
    private bool _isEnding = false;
    private Instructions.AnimatedValue? _memoryCounter;
    private Point _memoryCursorPosition = (0, 0);
    private Components.Cursor _cursor;

    /// <summary>
    /// Creates a new instance of this spashscreen.
    /// </summary>
    public PCBoot() : base(new CellSurface((Settings.Rendering.RenderWidth / GameHost.Instance.EmbeddedFont.GlyphWidth) + GameHost.Instance.EmbeddedFont.GlyphWidth,
                                           (Settings.Rendering.RenderHeight / GameHost.Instance.EmbeddedFont.GlyphHeight) + GameHost.Instance.EmbeddedFont.GlyphHeight),
                                           GameHost.Instance.EmbeddedFont, ((IFont)GameHost.Instance.EmbeddedFont).GetFontSize(IFont.Sizes.One))
    {

        _cursor = new Components.Cursor(Surface) { DisableWordBreak = true, PrintAppearanceMatchesHost = false };

        _endAnimation = new Instructions.InstructionSet() { RemoveOnFinished = true }
            .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(0), Settings.ClearColor.SetAlpha(255)), System.TimeSpan.FromSeconds(1)))
            .Wait(System.TimeSpan.FromMilliseconds(0.500))
            .Code((s, d) => { IsVisible = false; return true; });

        _startAnimation = new Instructions.InstructionSet { RemoveOnFinished = true }
            .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(255), Settings.ClearColor.SetAlpha(0)), System.TimeSpan.FromSeconds(1)))
            .Wait(System.TimeSpan.FromMilliseconds(0.500))
            .Code(() =>
            {
                _cursor
                    .SetPrintAppearance(new ColoredGlyph(Color.AnsiBlueBright))
                    .Move(0, 0)
                    .Print($"{(char)222}{(char)219}")
                    .NewLine()
                    .Print($"{(char)219}{(char)221}")
                    .Move(3, 0)
                    .SetPrintAppearance(new ColoredGlyph(Color.AnsiWhite))
                    .Print("Aw0rd Modular BIoS v.4.50G, An En3rgy Star Ally")
                    .Move(3, 1)
                    .Print("Copyright (C) 1984-85, Award Softcare, Inc.")
                    .NewLine()
                    .NewLine()
                    .Print("7/20/2020")
                    .NewLine()
                    .NewLine()
                    ;
                _memoryCursorPosition = _cursor.Position;
            })
            .Wait(System.TimeSpan.FromSeconds(1))
            .Code(() =>
            {
                _cursor
                    .Print("80486SX CPU at 33Mhz")
                    .NewLine()
                    ;
                _memoryCursorPosition = _cursor.Position;
            })
            .Wait(System.TimeSpan.FromSeconds(1))
            .Code(() =>
            {
                _cursor
                    .NewLine()
                    .Print("Memory Test : ")
                    ;
                _memoryCursorPosition = _cursor.Position;
                _cursor.Print("0k");
            })
            .Code(InstructMemoryCounterReset)
            .Code(InstructMemoryCounter)
            .Wait(System.TimeSpan.FromSeconds(0.5))
            .Code(InstructMemoryCounterReset)
            .Code(InstructMemoryCounter)
            .Wait(System.TimeSpan.FromSeconds(1))
            .Code(() => _cursor.NewLine().NewLine().Print("Checks: ").SetPrintAppearance(new ColoredGlyph(Color.AnsiGreenBright)).Print("PASSED"))
            .Wait(System.TimeSpan.FromSeconds(1))
            .Code(() => _cursor.SetPrintAppearance(new ColoredGlyph(Color.AnsiWhite)).NewLine().NewLine().Print("Powered by SadConsole"))
            .Wait(System.TimeSpan.FromSeconds(0.4))
            .Code(() => _cursor.NewLine().Print("https://www.sadconsole.com"))
            .Wait(System.TimeSpan.FromSeconds(2.0))
            .Instruct(_endAnimation)
            ;

        SadComponents.Add(_startAnimation);
    }

    private bool InstructMemoryCounterReset(IScreenObject host, System.TimeSpan delta)
    {
        _memoryCounter = new Instructions.AnimatedValue(System.TimeSpan.FromSeconds(2), 0, 8192);
        return true;
    }

    private bool InstructMemoryCounter(IScreenObject host, System.TimeSpan delta)
    {
        _memoryCounter!.Update(host, delta);

        _cursor
            .Move(_memoryCursorPosition)
            .Print($"{(int)_memoryCounter.Value}K OK".PadRight(10))
            ;

        return _memoryCounter.IsFinished;
    }

    /// <summary>
    /// Ends the animation when a key is pressed.
    /// </summary>
    /// <param name="keyboard">The keyboard state.</param>
    /// <returns>The base implementation of the keyboard.</returns>
    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (!_isEnding && keyboard.KeysReleased.Count != 0)
        {
            _isEnding = true;
            SadComponents.Remove(_startAnimation);
            SadComponents.Add(_endAnimation);
        }

        return base.ProcessKeyboard(keyboard);
    }
}
