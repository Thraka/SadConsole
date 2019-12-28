using SadRogue.Primitives;
using SadConsole;
using SadConsole.Input;
using SadConsoleEditor.Panels;
using SadConsoleEditor.Tools;

namespace SadConsoleEditor.Tools
{
    class EmptyTool: ITool
    {
        private Console _console;
        private bool _firstSelect;
        private Rectangle _oldViewPort;
        private Color _emptyCellColor = Color.NavajoWhite;

        public const string ID = "EMPTY";
        public string Id
        {
            get { return ID; }
        }

        public string Title
        {
            get { return "Empty Cells"; }
        }
        public char Hotkey { get { return 'e'; } }


        public CustomPanel[] ControlPanels { get; private set; }

        //private EntityBrush _brush;
        public SadConsole.Entities.Entity Brush;

        public bool ShowGrid
        {
            get => _console.IsVisible;
            set
            {
                _console.IsVisible = value;
            }
        }

        public EmptyTool()
        {
            ControlPanels = System.Array.Empty<CustomPanel>();
        }

        public override string ToString() 
        {
            return Title;
        }

        public void OnSelected()
        {
            Brush = new SadConsole.Entities.Entity(1, 1, Config.Program.ScreenFont, Config.Program.ScreenFontSize);
            Brush.Animation.CurrentFrame[0].Glyph = 'X';
            Brush.IsVisible = false;
            MainConsole.Instance.Brush = Brush;

            _firstSelect = true;
        }
        
        public void OnDeselected()
        {
            if (MainConsole.Instance.ActiveEditor != null)
            {
                if (MainConsole.Instance.ActiveEditor.Surface.Children.Contains(_console))
                    MainConsole.Instance.ActiveEditor.Surface.Children.Remove(_console);
            }
        }

        public void RefreshTool()
        {
            if (MainConsole.Instance.ActiveEditor != null)
            {
                _oldViewPort = MainConsole.Instance.ActiveEditor.Surface.View;

                if (MainConsole.Instance.ActiveEditor.Surface.Children.Contains(_console))
                    MainConsole.Instance.ActiveEditor.Surface.Children.Remove(_console);

                _console = new Console(MainConsole.Instance.ActiveEditor.Surface.View.Width, MainConsole.Instance.ActiveEditor.Surface.View.Height);
                _console.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                _console.Clear();
                
                var clearCell = new ColoredGlyph(MainConsole.Instance.ActiveEditor.Surface.DefaultForeground, MainConsole.Instance.ActiveEditor.Surface.DefaultBackground, 0);

                for (int index = 0; index < _console.Cells.Length; index++)
                {
                    var renderCell = MainConsole.Instance.ActiveEditor.Surface.Cells[(Point.FromIndex(index, _console.BufferWidth) + MainConsole.Instance.ActiveEditor.Surface.ViewPosition).ToIndex(MainConsole.Instance.ActiveEditor.Surface.BufferWidth)];

                    if (renderCell.Foreground == clearCell.Foreground &&
                        renderCell.Background == clearCell.Background &&
                        renderCell.Glyph == clearCell.Glyph)

                        _console[index].Background = _emptyCellColor;
                }

                MainConsole.Instance.ActiveEditor.Surface.Children.Add(_console);
            }
        }

        public void Update()
        {
            if (_firstSelect)
            {
                RefreshTool();
                _firstSelect = false;
            }
            else if (_oldViewPort != MainConsole.Instance.ActiveEditor.Surface.View)
                RefreshTool();
        }

        public bool ProcessKeyboard(Keyboard info, Console surface)
        {
            //if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
            //{
            //    ShowGrid = !ShowGrid;
            //    return true;
            //}

            return false;
        }

        public void ProcessMouse(MouseScreenObjectState info, Console surface, bool isInBounds)
        {
            if (MainConsole.Instance.ActiveEditor == null)
                return;

            if (info.IsOnScreenObject)
            {                

                if (info.Mouse.LeftButtonDown)
                {
                    var position = new MouseScreenObjectState(MainConsole.Instance.ActiveEditor.Surface, info.Mouse).CellPosition;
                    MainConsole.Instance.ActiveEditor.Surface.Clear(position.X, position.Y);
                    var editPosition = position - MainConsole.Instance.ActiveEditor.Surface.ViewPosition;
                    _console.SetBackground(editPosition.X, editPosition.Y, _emptyCellColor);
                }

                if (info.Mouse.RightButtonDown)
                {
                    var cell = info.Cell;

                }
            }
        }
    }
}
