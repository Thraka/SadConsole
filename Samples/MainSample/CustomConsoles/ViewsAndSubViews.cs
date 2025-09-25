using SadConsole;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    internal class ViewsAndSubViews : ScreenObject
    {
        private readonly ScreenSurface mainView;
        private readonly ScreenSurface subView;

        public ViewsAndSubViews()
        {
            mainView = new ScreenSurface(60, 23);
            subView = new ScreenSurface(mainView.Surface.GetSubSurface(new Rectangle(0, 0, 20, 23)));

            IsVisible = false;
            UseMouse = true;

            mainView.Surface.DrawLine(new Point(59, 0), new Point(59, 22), SadConsole.ICellSurface.ConnectedLineThin[(int)SadConsole.ICellSurface.ConnectedLineIndex.Left], Color.White);

            // Setup main view
            mainView.Position = new Point(0, 0);
            mainView.MouseMove += (s, e) => { if (e.Mouse.LeftButtonDown) { e.Cell.Background = Color.Blue; mainView.IsDirty = true; } };
            mainView.Surface.IsDirtyChanged += (s, e) => subView.IsDirty = true;

            // Setup sub view
            subView.Position = new Point(60, 0);
            //subView.SetViewFromSurface(new Rectangle(0, 0, 20, 23), mainView);
            subView.MouseMove += (s, e) => { if (e.Mouse.LeftButtonDown) { e.Cell.Background = Color.Red; subView.IsDirty = true; } };
            subView.Surface.IsDirtyChanged += (s, e) => mainView.IsDirty = true;

            // Ad the consoles to the list.
            Children.Add(mainView);
            Children.Add(subView);
        }
    }
}
