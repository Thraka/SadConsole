namespace SadConsoleEditor.Controls
{
    using Microsoft.Xna.Framework;
    using System;
    using SadConsole.Input;

    class ColorPresenter : SadConsole.Controls.ControlBase
    {
        public event EventHandler ColorChanged;
        public event EventHandler RightClickedColor;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        public string Title { get { return _title; } set { _title = value; Compose(); } }

        public Color CharacterColor { get { return _characterColor; } set { _characterColor = value; Compose(); } }
        public int Character { get { return _character; } set { _character = value; Compose(); } }

        public bool DisableColorPicker { get; set; }
        public bool EnableCharacterPicker { get; set; }

        private Color _selectedColor;
        private string _title;
        private Windows.ColorPickerPopup _popup;
        private int _character;
        private Color _characterColor;

        
        public ColorPresenter(string title, Color foreground, int width): base(width, 1)
        {
            textSurface.DefaultForeground = foreground;
            Clear();
            _title = title;
            _popup = new Windows.ColorPickerPopup();
            _popup.Closed += (o, e) =>
                {
                    if (_popup.DialogResult)
                        SelectedColor = _popup.SelectedColor;
                };
        }

        public override void Compose()
        {
            Clear();

            Print(0, 0, _title);

            Print(Width - 3, 0, "   ", Color.Black, _selectedColor);
            if (_character != 0)
            {
                SetGlyph(Width - 2, 0, _character);
                SetForeground(Width - 2, 0, _characterColor);
            }

            OnComposed?.Invoke(this);
        }

        public override void DetermineAppearance()
        {

        }

        protected override void OnLeftMouseClicked(MouseConsoleState info)
        {
            if (!DisableColorPicker)
            {
                var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                if (location.X >= Width - 3)
                {
                    _popup.SelectedColor = _selectedColor;
                    _popup.Show(true);
                    base.OnLeftMouseClicked(info);
                }
            }
            else if (EnableCharacterPicker)
            {
                var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                if (location.X >= Width - 3)
                {
                    base.OnLeftMouseClicked(info);
                }
            }
        }

        protected override void OnRightMouseClicked(MouseConsoleState info)
        {
            var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
            if (location.X >= Width - 3)
            {
                RightClickedColor?.Invoke(this, EventArgs.Empty);
                base.OnRightMouseClicked(info);
            }
        }
    }
}
