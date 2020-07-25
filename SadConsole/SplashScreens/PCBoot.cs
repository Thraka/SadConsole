using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.SplashScreens
{
    public class PCBoot : ScreenSurface
    {
        private static string _title = " Powered by SadConsole ";
        private Instructions.InstructionSet _endAnimation;
        private Instructions.InstructionSet _startAnimation;
        private bool _isEnding = false;
        private Instructions.AnimatedValue memoryCounter;
        private Point memoryCursorPosition = (0, 0);
        private Components.Cursor cursor;

        public PCBoot() : base(new CellSurface((Settings.Rendering.RenderWidth / GameHost.Instance.EmbeddedFont.GlyphWidth) + GameHost.Instance.EmbeddedFont.GlyphWidth,
                                               (Settings.Rendering.RenderHeight / GameHost.Instance.EmbeddedFont.GlyphHeight) + GameHost.Instance.EmbeddedFont.GlyphHeight),
                                               GameHost.Instance.EmbeddedFont, GameHost.Instance.EmbeddedFont.GetFontSize(Font.Sizes.One))
        {

            cursor = new Components.Cursor(Surface) { DisableWordBreak = true };

            _endAnimation = new Instructions.InstructionSet() { RemoveOnFinished = true }
                .Instruct(new Instructions.FadeTextSurfaceTint(new ColorGradient(Settings.ClearColor.SetAlpha(0), Settings.ClearColor.SetAlpha(255)), System.TimeSpan.FromSeconds(1)))
                .Wait(System.TimeSpan.FromMilliseconds(0.500))
                .Code((s, d) => { IsVisible = false; return true; });

            _startAnimation = new Instructions.InstructionSet { RemoveOnFinished = true }
                .Instruct(new Instructions.FadeTextSurfaceTint(new ColorGradient(Settings.ClearColor.SetAlpha(255), Settings.ClearColor.SetAlpha(0)), System.TimeSpan.FromSeconds(1)))
                .Wait(System.TimeSpan.FromMilliseconds(0.500))
                .Code(() =>
                {
                    cursor
                        .SetPrintAppearance(new ColoredGlyph(ColorAnsi.BlueBright))
                        .Move(0, 0)
                        .Print($"{(char)222}{(char)219}")
                        .NewLine()
                        .Print($"{(char)219}{(char)221}")
                        .Move(3, 0)
                        .SetPrintAppearance(new ColoredGlyph(ColorAnsi.White))
                        .Print("Aw0rd Modular BIoS v.4.50G, An En3rgy Star Ally")
                        .Move(3, 1)
                        .Print("Copyright (C) 1984-85, Award Softcare, Inc.")
                        .NewLine()
                        .NewLine()
                        .Print("7/20/2020")
                        .NewLine()
                        .NewLine()
                        ;
                    memoryCursorPosition = cursor.Position;
                })
                .Wait(System.TimeSpan.FromSeconds(1))
                .Code(() =>
                {
                    cursor
                        .Print("80486SX CPU at 33Mhz")
                        .NewLine()
                        ;
                    memoryCursorPosition = cursor.Position;
                })
                .Wait(System.TimeSpan.FromSeconds(1))
                .Code(() =>
                {
                    cursor
                        .NewLine()
                        .Print("Memory Test : ")
                        ;
                    memoryCursorPosition = cursor.Position;
                    cursor.Print("0k");
                })
                .Code(InstructMemoryCounterReset)
                .Code(InstructMemoryCounter)
                .Wait(System.TimeSpan.FromSeconds(0.5))
                .Code(InstructMemoryCounterReset)
                .Code(InstructMemoryCounter)
                .Wait(System.TimeSpan.FromSeconds(1))
                .Code(() => cursor.NewLine().NewLine().Print("Checks: [c:r f:AnsiGreenBright]PASSED[c:u]"))
                .Wait(System.TimeSpan.FromSeconds(1))
                .Code(() => cursor.NewLine().NewLine().Print("Powered by SadConsole"))
                .Wait(System.TimeSpan.FromSeconds(0.4))
                .Code(() => cursor.NewLine().Print("https://www.sadconsole.com"))
                .Wait(System.TimeSpan.FromSeconds(2.0))
                .Instruct(_endAnimation)
                ;

            SadComponents.Add(_startAnimation);
        }

        private bool InstructMemoryCounterReset(IScreenObject host, System.TimeSpan delta)
        {
            memoryCounter = new Instructions.AnimatedValue(System.TimeSpan.FromSeconds(2), 0, 8192);
            return true;
        }

        private bool InstructMemoryCounter(IScreenObject host, System.TimeSpan delta)
        {
            memoryCounter.Update(host, delta);

            cursor
                .Move(memoryCursorPosition)
                .Print($"{(int)memoryCounter.Value}K OK".PadRight(10))
                ;

            return memoryCounter.IsFinished;
        }

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
}
