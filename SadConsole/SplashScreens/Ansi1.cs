using System;
using System.IO;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.SplashScreens;

/// <summary>
/// A simple splashscreen that fades in a screen specifying "Powered by SadConsole".
/// </summary>
[System.Diagnostics.DebuggerDisplay("Splashscreen: Ansi Logo")]
public class Ansi1 : ScreenSurface
{
    private Instructions.InstructionSet _endAnimation;
    private bool _isEnding = false;
    private ScreenSurface _ansiChild;

    /// <summary>
    /// Creates the ansi splash screen using the specified font and size.
    /// </summary>
    /// <param name="font">The font to use.</param>
    /// <param name="fontSize">The font size to use.</param>
    public Ansi1(IFont font, IFont.Sizes fontSize) : base(GameHost.Instance.ScreenCellsX, GameHost.Instance.ScreenCellsY)
    {
        UsePixelPositioning = true;
        Surface.DefaultBackground = Color.Black;
        Surface.Fill();
        //Position = (Settings.Rendering.RenderWidth / 2 - AbsoluteArea.Width / 2, Settings.Rendering.RenderHeight / 2 - AbsoluteArea.Height / 2);
        //Surface.Print(0, 0, _title, Color.Black, Color.AnsiWhite);

        // Create surface
        _ansiChild = new ScreenSurface(80, 25);
        _ansiChild.Font = font;
        _ansiChild.FontSize = font.GetFontSize(fontSize);

        // Center surface on screen
        Point position = (0, 0);

        if (_ansiChild.WidthPixels > Settings.Rendering.RenderWidth)
            position = position.WithX(-((_ansiChild.WidthPixels - Settings.Rendering.RenderWidth) / 2));

        else if (_ansiChild.WidthPixels < Settings.Rendering.RenderWidth)
            position = position.WithX((Settings.Rendering.RenderWidth - _ansiChild.WidthPixels) / 2);


        if (_ansiChild.HeightPixels > Settings.Rendering.RenderHeight)
            position = position.WithY(-((_ansiChild.HeightPixels - Settings.Rendering.RenderHeight) / 2));

        else if (_ansiChild.HeightPixels < Settings.Rendering.RenderHeight)
            position = position.WithY((Settings.Rendering.RenderHeight - _ansiChild.HeightPixels) / 2);

        _ansiChild.UsePixelPositioning = true;
        _ansiChild.Position = position;
        _ansiChild.Surface.DefaultBackground = Color.Black;
        _ansiChild.Surface.Fill();
        Children.Add(_ansiChild);

        // Time out animations
        _endAnimation = new Instructions.InstructionSet() { RemoveOnFinished = true }
            .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(0), Settings.ClearColor.SetAlpha(255)), System.TimeSpan.FromSeconds(1)))
            .Wait(System.TimeSpan.FromMilliseconds(0.500))
            .Code((s, d) => { IsVisible = false; return true; });

        var endTimeout = new Instructions.InstructionSet() { RemoveOnFinished = true }
            .Wait(System.TimeSpan.FromSeconds(3))
            .Code((s, d) => { _isEnding = true; SadComponents.Add(_endAnimation); return true; });

        var startAnimation = new Instructions.InstructionSet { RemoveOnFinished = true }
            .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(255), Settings.ClearColor.SetAlpha(0)), System.TimeSpan.FromSeconds(1)))
            //.Wait(System.TimeSpan.FromMilliseconds(0.500));
            ;

        // Read the ansi file
        System.Reflection.Assembly assembly = typeof(ScreenSurface).Assembly;
        using Stream stream = assembly.GetManifestResourceStream("SadConsole.Resources.logo.ans")!;
        using BinaryReader sr = new BinaryReader(stream);

        SadConsole.Ansi.Document document = Ansi.Document.FromBytes(sr.ReadBytes((int)stream.Length));
        SadConsole.Ansi.AnsiWriter writer = new Ansi.AnsiWriter(document, _ansiChild.Surface);
        writer.Cursor.AutomaticallyShiftRowsUp = false;
        writer.ReadEntireDocument();

        // Settings for editing the ansi file
        var monitorRect = new Rectangle((34, 7), (45, 10));
        CellSurface monitorSurface = new CellSurface(monitorRect.Width, monitorRect.Height);
        ColoredGlyph tear = new ColoredGlyph();
        ColoredGlyph tearPrevious = new ColoredGlyph();
        Point tearPosition = (44, 9);
        Point textMadeWithPosition = (34, 19);
        Point textSadConsolePosition = (35, 21);
        Color fillColor = _ansiChild.Surface.GetForeground(0, 0);

        // Draw made with string
        var madeWithString = new ColoredString("made with");
        var fadeEffect = new Effects.Fade()
        {
            DestinationForeground = new Gradient(new[] { Color.Transparent, Color.White, Color.AnsiYellowBright, Color.Black },
                                                 new[] { 0.0f, 0.3f, 0.5f, 1.0f }),
            UseCellForeground = false,
            FadeForeground = true,
            FadeDuration = TimeSpan.FromSeconds(1),
            CloneOnAdd = true,
            RestoreCellOnRemoved = false,
            RemoveOnFinished = true,
        };
        madeWithString.SetEffect(fadeEffect);
        madeWithString.SetBackground(fillColor);
        madeWithString.SetForeground(fadeEffect.DestinationForeground.Stops[0].Color);
        var drawMadeWith = new Instructions.DrawString(madeWithString) { TotalTimeToPrint = TimeSpan.FromSeconds(0.4d) };
        drawMadeWith.Position = textMadeWithPosition;

        // Draw SadConsole string
        var sadConsoleString = new ColoredString("SadConsole");
        var fadeEffect2 = new Effects.Fade()
        {
            DestinationForeground = new Gradient(new[] { fillColor, fillColor, Color.White },
                                                 new[] { 0.0f, 0.5f, 1.0f }),
            UseCellForeground = false,
            FadeForeground = true,
            FadeDuration = TimeSpan.FromSeconds(0.8),
            CloneOnAdd = false,
            RestoreCellOnRemoved = false,
            RemoveOnFinished = true,
        };
        sadConsoleString.SetEffect(fadeEffect2);
        sadConsoleString.SetBackground(fillColor);
        sadConsoleString.SetForeground(fadeEffect2.DestinationForeground.Stops[0].Color);
        var drawSadConsole = new Instructions.DrawString(sadConsoleString) { TotalTimeToPrint = TimeSpan.FromSeconds(0.3d) };
        drawSadConsole.Position = textSadConsolePosition;

        // Tear drop animation
        var animatedTearSet = new Instructions.InstructionSet()
            .Code(() =>
            {
                tearPosition = (44, 9);

                _ansiChild.Surface[tearPosition].CopyAppearanceTo(tearPrevious);
                tear.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                _ansiChild.IsDirty = true;

            }).Wait(TimeSpan.FromSeconds(0.3))
            .Code(() =>
            {
                tearPrevious.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                tearPosition += (0, 1);
                _ansiChild.Surface[tearPosition].CopyAppearanceTo(tearPrevious);
                tear.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                _ansiChild.IsDirty = true;

            }).Wait(TimeSpan.FromSeconds(0.3))
            .Code(() =>
            {
                tearPrevious.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                tearPosition += (0, 1);
                _ansiChild.Surface[tearPosition].CopyAppearanceTo(tearPrevious);
                tear.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                _ansiChild.IsDirty = true;

            }).Wait(TimeSpan.FromSeconds(0.3))
            .Code(() =>
            {
                tearPrevious.CopyAppearanceTo(_ansiChild.Surface[tearPosition]);
                _ansiChild.IsDirty = true;
            }).Wait(TimeSpan.FromSeconds(0.3))
            ;
        animatedTearSet.RepeatCount = 2;

        // Main animation
        var set = new Instructions.InstructionSet()
            // Edit the ansi to get things ready for animation.
            .Code(() =>
            {
                // Copy and clear tear
                _ansiChild.Surface[tearPosition].CopyAppearanceTo(tear);
                _ansiChild.Surface.SetGlyph(tearPosition.X, tearPosition.Y, 0);

                // Block out the monitor
                _ansiChild.Surface.Copy(monitorRect, monitorSurface, 0, 0);
                _ansiChild.Surface.Fill(monitorRect, glyph: 0, background: Color.Black);

                // Recolor the power light
                _ansiChild.Surface.SetForeground(43, 13, Color.Gray);

                // Erase text
                _ansiChild.Surface.Print(textMadeWithPosition.X, textMadeWithPosition.Y, "         ", Color.Transparent);
                _ansiChild.Surface.Print(textSadConsolePosition.X, textSadConsolePosition.Y, "          ", Color.Transparent);
            })

            // Fade in the ansi
            .Instruct(startAnimation)

            // Blink light on power
            .Code(() =>
            {
                _ansiChild.Surface.SetEffect(43, 13, null);

                var effectSet = new Effects.EffectSet();

                effectSet.Add(
                    new Effects.Blink()
                    {
                        UseCellBackgroundColor = false,
                        BlinkCount = 3,
                        BlinkSpeed = TimeSpan.FromSeconds(0.2),
                        BlinkOutColor = Color.AnsiGreenBright,
                        RemoveOnFinished = true,
                        RestoreCellOnRemoved = false
                    });
                effectSet.Add(new Effects.Recolor() { Background = _ansiChild.Surface.GetBackground(43, 13), Foreground = Color.AnsiRed, RestoreCellOnRemoved = false, RemoveOnFinished = true });
                effectSet.RemoveOnFinished = true;
                effectSet.RestoreCellOnRemoved = false;

                _ansiChild.Surface.SetEffect(43, 13, effectSet);
            })
            .Wait(TimeSpan.FromSeconds(2))

            // Restore monitor
            .Code(() =>
            {
                monitorSurface.Copy(_ansiChild.Surface, monitorRect.X, monitorRect.Y);
            }).Wait(TimeSpan.FromSeconds(0.5))

            // Animate tear
            .InstructConcurrent(new Instructions.InstructionBase[]
            {
                animatedTearSet,
                new Instructions.InstructionSet()
                    .Wait(TimeSpan.FromSeconds(0.5))
                    .Instruct(drawMadeWith)
                    .Wait(TimeSpan.FromSeconds(0.8))
                    .Instruct(drawSadConsole)
            })

            // Wait and then end
            .Wait(TimeSpan.FromSeconds(1))
            .Instruct(_endAnimation)
            ;

        _ansiChild.SadComponents.Add(set);
    }


    /// <summary>
    /// Creates the ansi splash screen using the <see cref="GameHost.EmbeddedFont"/> and the specified font size.
    /// </summary>
    /// <param name="fontSize">The size of font to use.</param>
    public Ansi1(IFont.Sizes fontSize) : this(GameHost.Instance.EmbeddedFont, fontSize) { }

    /// <summary>
    /// Creates the ansi splash screen using the <see cref="GameHost.EmbeddedFont"/> and a font size of <see cref="IFont.Sizes.One"/>.
    /// </summary>
    public Ansi1() : this(GameHost.Instance.EmbeddedFont, IFont.Sizes.One) { }

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
            _ansiChild.SadComponents.Clear();
            _ansiChild.SadComponents.Add(_endAnimation);
        }

        return base.ProcessKeyboard(keyboard);
    }
}
