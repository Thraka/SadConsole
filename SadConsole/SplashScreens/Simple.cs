using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.SplashScreens
{
    /// <summary>
    /// A simple splashscreen that fades in a screen specifying "Powered by SadConsole".
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Splashscreen: Simple")]
    public class Simple : ScreenSurface
    {
        private static string _title = " Powered by SadConsole ";
        private Instructions.InstructionSet _endAnimation;
        private bool _isEnding = false;

        /// <summary>
        /// A new instance of this class.
        /// </summary>
        public Simple() : base(_title.Length, 1)
        {
            UsePixelPositioning = true;
            Position = (Settings.Rendering.RenderWidth / 2 - AbsoluteArea.Width / 2, Settings.Rendering.RenderHeight / 2 - AbsoluteArea.Height / 2);
            Surface.Print(0, 0, _title, Color.Black, Color.AnsiWhite);

            _endAnimation = new Instructions.InstructionSet() { RemoveOnFinished = true }
                .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(0), Settings.ClearColor.SetAlpha(255)), System.TimeSpan.FromSeconds(1)))
                .Wait(System.TimeSpan.FromMilliseconds(0.500))
                .Code((s, d) => { IsVisible = false; return true; });

            var endTimeout = new Instructions.InstructionSet() { RemoveOnFinished = true }
                .Wait(System.TimeSpan.FromSeconds(3))
                .Code((s, d) => { _isEnding = true; SadComponents.Add(_endAnimation); return true; });

            var startAnimation = new Instructions.InstructionSet { RemoveOnFinished = true }
                .Instruct(new Instructions.FadeTextSurfaceTint(new Gradient(Settings.ClearColor.SetAlpha(255), Settings.ClearColor.SetAlpha(0)), System.TimeSpan.FromSeconds(1)))
                .Wait(System.TimeSpan.FromMilliseconds(0.500));

            SadComponents.Add(startAnimation);
            SadComponents.Add(endTimeout);
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
                SadComponents.Add(_endAnimation);
            }

            return base.ProcessKeyboard(keyboard);
        }
    }
}
