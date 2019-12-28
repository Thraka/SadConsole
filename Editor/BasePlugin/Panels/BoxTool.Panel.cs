using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsoleEditor.Panels
{
    class BoxToolPanel : CustomPanel
    {
        private CheckBox _fillBoxOption;

        private Controls.ColorPresenter _lineCharPicker;
        private Controls.ColorPresenter _lineForeColor;
        private Controls.ColorPresenter _lineBackColor;

        private Controls.ColorPresenter _fillCharPicker;
        private Controls.ColorPresenter _fillForeColor;
        private Controls.ColorPresenter _fillBackColor;

        public Color FillBackColor { get { return _fillBackColor.SelectedColor; } }
        public Color FillForeColor { get { return _fillForeColor.SelectedColor; } }
        public Color LineForeColor { get { return _lineForeColor.SelectedColor; } }
        public Color LineBackColor { get { return _lineBackColor.SelectedColor; } }

        public bool UseFill { get { return _fillBoxOption.IsSelected; } }

        public int LineGlyph { get { return _lineCharPicker.SelectedGlyph; } }
        public int FillGlyph { get { return _fillCharPicker.SelectedGlyph; } }


        public BoxToolPanel()
        {
            Title = "Settings";

            _fillBoxOption = new CheckBox(18, 1);
            _fillBoxOption.Text = "Fill";

            _lineForeColor = new Controls.ColorPresenter("Border Fore", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineForeColor.SelectedColor = Color.White;
            _lineForeColor.ColorChanged += (s, e) => _lineCharPicker.GlyphColor = _lineForeColor.SelectedColor;

            _lineBackColor = new Controls.ColorPresenter("Border Back", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineBackColor.SelectedColor = Color.Black;
            _lineBackColor.ColorChanged += (s, e) => _lineCharPicker.SelectedColor = _lineBackColor.SelectedColor;

            _lineCharPicker = new Controls.ColorPresenter("Border Glyph", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineCharPicker.EnableCharacterPicker = true;
            _lineCharPicker.DisableColorPicker = true;
            _lineCharPicker.SelectedColor = _lineBackColor.SelectedColor;
            _lineCharPicker.GlyphColor = _lineForeColor.SelectedColor;

            _lineForeColor.RightClickedColor += (s, e) => { var tempColor = _lineBackColor.SelectedColor; _lineBackColor.SelectedColor = _lineForeColor.SelectedColor; _lineForeColor.SelectedColor = tempColor; };
            _lineBackColor.RightClickedColor += (s, e) => { var tempColor = _lineForeColor.SelectedColor; _lineForeColor.SelectedColor = _lineBackColor.SelectedColor; _lineBackColor.SelectedColor = tempColor; };

            _fillForeColor = new Controls.ColorPresenter("Fill Fore", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _fillForeColor.SelectedColor = Color.White;
            _fillForeColor.ColorChanged += (s, e) => _fillCharPicker.GlyphColor = _fillForeColor.SelectedColor;

            _fillBackColor = new Controls.ColorPresenter("Fill Back", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _fillBackColor.SelectedColor = Color.Black;
            _fillBackColor.ColorChanged += (s, e) => _fillCharPicker.SelectedColor = _fillBackColor.SelectedColor;

            _fillCharPicker = new Controls.ColorPresenter("Fill Glyph", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _fillCharPicker.EnableCharacterPicker = true;
            _fillCharPicker.DisableColorPicker = true;
            _fillCharPicker.SelectedColor = _fillBackColor.SelectedColor;
            _fillCharPicker.GlyphColor = _fillForeColor.SelectedColor;

            _fillForeColor.RightClickedColor += (s, e) => { var tempColor = _fillBackColor.SelectedColor; _fillBackColor.SelectedColor = _fillForeColor.SelectedColor; _fillForeColor.SelectedColor = tempColor; };
            _fillBackColor.RightClickedColor += (s, e) => { var tempColor = _fillForeColor.SelectedColor; _fillForeColor.SelectedColor = _fillBackColor.SelectedColor; _fillBackColor.SelectedColor = tempColor; };


            Controls = new ControlBase[] { _lineForeColor, _lineBackColor, _lineCharPicker, null, _fillForeColor, _fillBackColor, _fillCharPicker, null, _fillBoxOption };
        }

        public override void ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
            
        }

        public override int Redraw(SadConsole.UI.Controls.ControlBase control)
        {
            return 0;
        }

        public override void Loaded()
        {
        }
    }
}
