using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System;

namespace SadConsoleEditor.Controls
{
    public class ColorPicker : SadConsole.UI.Controls.ControlBase
    {
        public class ThemeType : ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is ColorPicker)) throw new Exception($"Theme can only be added to a {nameof(ColorPicker)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.Clear();
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is ColorPicker picker)) return;

                if (!control.IsDirty) return;

                ColoredGlyph appearance;

                RefreshTheme(control.FindThemeColors(), control);

                if (Helpers.HasFlag((int)control.State, (int)ControlStates.Disabled))
                    appearance = ControlThemeState.Disabled;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(presenter.State, ControlStates.MouseRightButtonDown))
                //    appearance = MouseDown;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseOver))
                //    appearance = MouseOver;

                else if (Helpers.HasFlag((int)control.State, (int)ControlStates.Focused))
                    appearance = ControlThemeState.Focused;

                else
                    appearance = ControlThemeState.Normal;

                Color[] colors = Color.White.LerpSteps(Color.Black, control.Height);
                Color[] colorsEnd = picker._masterColor.LerpSteps(Color.Black, control.Height);

                for (int y = 0; y < control.Height; y++)
                {
                    control.Surface[0, y].Background = colors[y];
                    control.Surface[control.Width - 1, y].Background = colorsEnd[y];

                    control.Surface[0, y].Foreground = new Color(255 - colors[y].R, 255 - colors[y].G, 255 - colors[y].B);
                    control.Surface[control.Width - 1, y].Foreground = new Color(255 - colorsEnd[y].R, 255 - colorsEnd[y].G, 255 - colorsEnd[y].B);



                    Color[] rowColors = colors[y].LerpSteps(colorsEnd[y], control.Width);

                    for (int x = 1; x < control.Width - 1; x++)
                    {
                        control.Surface[x, y].Background = rowColors[x];
                        control.Surface[x, y].Foreground = new Color(255 - rowColors[x].R, 255 - rowColors[x].G, 255 - rowColors[x].B);
                    }
                }

                control.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ThemeType()
                {
                    ControlThemeState = ControlThemeState.Clone()
                };
            }
        }


        private Color _selectedColor;
        private Color _masterColor;
        private Point _selectedColorPosition;

        public event EventHandler SelectedColorChanged;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            private set
            {
                if (_selectedColor != value)
                {
                    ResetSelectedIndex(value);
                    _selectedColor = value;
                    SelectedColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Color SelectedColorSafe
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    SelectedColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Color MasterColor
        {
            get { return _masterColor; }
            set
            {
                if (_masterColor != value)
                {
                    _masterColor = value;
                    SelectedColor = value;
                    IsDirty = true;
                    //ResetSelectedColor();
                }
            }
        }


        public ColorPicker(int width, int height, Color color) : base(width, height)
        {
            //Theme = new ThemeType();
            //Theme.UpdateAndDraw(this, TimeSpan.Zero);
            CanFocus = false;
            //SelectedHue = hue;

            SelectedColor = color;
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
        }

        private void ResetSelectedIndex(Color color)
        {
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
            _selectedColorPosition = new Point(Width - 1, 0);
            Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;

            this.IsDirty = true;
        }

        protected override void OnMouseIn(ControlMouseState info)
        {
            base.OnMouseIn(info);

            if (Parent.Host.CapturedControl == null)
            {
                if (info.OriginalMouseState.Mouse.LeftButtonDown)
                {
                    var location = info.MousePosition;
                    Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                    _selectedColorPosition = location;
                    SelectedColorSafe = Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                    Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
                    IsDirty = true;

                    Parent.Host.CaptureControl(this);
                }
            }
        }

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
            if (Parent.Host.CapturedControl == this)
            {
                if (info.Mouse.LeftButtonDown == false)
                    Parent.Host.ReleaseControl();
                else
                {
                    var newState = new ControlMouseState(this, info);
                    var location = newState.MousePosition;

                    //if (info.ConsolePosition.X >= Position.X && info.ConsolePosition.X < Position.X + Width)
                    if (location.X >= -6 && location.X <= Width + 5 && location.Y > -4 && location.Y < Height + 3)
                    {
                        Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 0;
                        _selectedColorPosition = new Point(Math.Clamp(location.X, 0, Width - 1), Math.Clamp(location.Y, 0, Height - 1));
                        SelectedColorSafe = Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Background;
                        Surface[_selectedColorPosition.X, _selectedColorPosition.Y].Glyph = 4;
                    }

                    IsDirty = true;
                }
            }

            return base.ProcessMouse(info);
        }

    }
}
