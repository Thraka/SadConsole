using Microsoft.Xna.Framework;
using SadConsole.Input;
using ScrollingConsole = SadConsole.ScrollingConsole;

namespace FeatureDemo.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    internal class ViewsAndSubViews : ScrollingConsole
    {
        private readonly ScrollingConsole mainView;
        private readonly ScrollingConsole subView;

        public ViewsAndSubViews() : base(1, 1)
        {
            mainView = new ScrollingConsole(60, 23);
            subView = ScrollingConsole.FromSurface(mainView.GetSubSurface(new Rectangle(0, 0, 20, 23)), Font);

            IsVisible = false;
            UseMouse = true;

            mainView.DrawLine(new Point(59, 0), new Point(59, 22), Color.White, glyph: ConnectedLineThin[(int)ConnectedLineIndex.Left]);

            // Setup main view
            mainView.Position = new Point(0, 0);
            mainView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) { e.MouseState.Cell.Background = Color.Blue; mainView.IsDirty = true; } };
            mainView.DirtyChanged += (s, e) => subView.IsDirty = true;

            // Setup sub view
            subView.Position = new Point(60, 0);
            //subView.SetViewFromSurface(new Rectangle(0, 0, 20, 23), mainView);
            subView.MouseMove += (s, e) => { if (e.MouseState.Mouse.LeftButtonDown) { e.MouseState.Cell.Background = Color.Red; subView.IsDirty = true; } };
            subView.DirtyChanged += (s, e) => mainView.IsDirty = true;

            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Process mouse for each console
            var childState = new MouseConsoleState(mainView, state.Mouse);

            if (childState.IsOnConsole)
            {
                return mainView.ProcessMouse(childState);
            }

            childState = new MouseConsoleState(subView, state.Mouse);

            if (childState.IsOnConsole)
            {
                return subView.ProcessMouse(childState);
            }

            return false;
        }
    }
}
