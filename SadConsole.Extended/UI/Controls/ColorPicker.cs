using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Displays a rectangle gradient area between <see cref="Color.Black"/> and a target color, and <see cref="Color.White"/> and a target color.
    /// </summary>
    public class ColorPicker : SadConsole.UI.Controls.ControlBase
    {
        private Color _selectedColor;
        private Color _masterColor;
        private Point _selectedColorPosition;

        /// <summary>
        /// Raised when <see cref="SelectedColor"/> changes value.
        /// </summary>
        public event EventHandler SelectedColorChanged;

        /// <summary>
        /// The color selected by the user. A shade of <see cref="MasterColor"/>.
        /// </summary>
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

        /// <summary>
        /// The main color being shown that all gradients are generated from.
        /// </summary>
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

        /// <summary>
        /// Creates a new panel with the specified width, height, and master color.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public ColorPicker(int width, int height, Color color): base(width, height)
        {
            CanFocus = false;

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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
