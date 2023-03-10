using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class ShapesConsole : ScreenSurface
    {
        private readonly ScreenSurface _mouseCursor;

        public ShapesConsole()
            : base(80, 23)
        {
            UseKeyboard = false;

            Surface.DrawLine(new Point(2, 2), new Point(Surface.ViewWidth - 4, 2), (int)'=', Color.Yellow);
            Surface.DrawBox(new Rectangle(2, 4, 6, 6), ShapeParameters.CreateBorder(new ColoredGlyph(Color.Yellow, Color.Black, '=')));
            Surface.DrawBox(new Rectangle(9, 4, 6, 6), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Yellow, Color.Black, '=')));
            Surface.DrawBox(new Rectangle(16, 4, 6, 6), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, new ColoredGlyph(Color.Yellow, Color.Black, '=')));
            Surface.DrawBox(new Rectangle(23, 4, 6, 6), ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick, new ColoredGlyph(Color.Black, Color.Yellow, '='), new ColoredGlyph(Color.Black, Color.Yellow, 0)));

            Surface.DrawCircle(new Rectangle(2, 12, 16, 10), ShapeParameters.CreateBorder(new ColoredGlyph(Color.White, Color.Black, 176)));
            Surface.DrawCircle(new Rectangle(19, 12, 16, 10), ShapeParameters.CreateFilled(new ColoredGlyph(Color.White, Color.Black, 176), new ColoredGlyph(Color.Green, Color.Black, 178)));

            IsDirty = true;
            IsVisible = false;

            _mouseCursor = new SadConsole.ScreenSurface(1, 1);
            _mouseCursor.Surface.SetGlyph(0, 0, 178);
            _mouseCursor.UseMouse = false;

            UseMouse = true;

            Children.Add(_mouseCursor);
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            _mouseCursor.IsVisible = state.IsOnScreenObject;
            _mouseCursor.Position = state.CellPosition;

            return base.ProcessMouse(state);
        }
    }
}
