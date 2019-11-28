
using SadConsole.Input;
using ScrollingConsole = SadConsole.ScrollingConsole;


namespace FeatureDemo.CustomConsoles
{
    internal class TextCursorConsole : ScrollingConsole
    {
        private readonly SadConsole.Console mouseCursor;

        public TextCursorConsole()
            : base(80, 23)
        {
            mouseCursor = new SadConsole.Console(1, 1);
            mouseCursor.SetGlyph(0, 0, 178);
            mouseCursor.UseMouse = false;

            Children.Add(mouseCursor);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            mouseCursor.IsVisible = state.IsOnConsole;
            mouseCursor.Position = state.ConsoleCellPosition;

            return base.ProcessMouse(state);
        }
    }
}
