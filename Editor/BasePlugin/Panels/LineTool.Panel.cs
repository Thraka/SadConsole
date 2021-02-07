using SadRogue.Primitives;
using SadConsole.UI.Controls;

namespace SadConsoleEditor.Panels
{
    class LineToolPanel : CustomPanel
    {
        private CheckBox _fillBoxOption;

        private Controls.ColorPresenter _lineCharPicker;
        private Controls.ColorPresenter _lineForeColor;
        private Controls.ColorPresenter _lineBackColor;

        public Color LineForeColor { get { return _lineForeColor.SelectedColor; } }
        public Color LineBackColor { get { return _lineBackColor.SelectedColor; } }

        public int LineGlyph { get { return _lineCharPicker.SelectedGlyph; } }


        public LineToolPanel()
        {
            Title = "Settings";

            _fillBoxOption = new CheckBox(18, 1);
            _fillBoxOption.Text = "Fill";

            _lineForeColor = new Controls.ColorPresenter("Foreground", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineForeColor.SelectedColor = Color.White;
            _lineForeColor.ColorChanged += (s, e) => _lineCharPicker.GlyphColor = _lineForeColor.SelectedColor;

            _lineBackColor = new Controls.ColorPresenter("Background", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineBackColor.SelectedColor = Color.Black;
            _lineBackColor.ColorChanged += (s, e) => _lineCharPicker.SelectedColor = _lineBackColor.SelectedColor;

            _lineCharPicker = new Controls.ColorPresenter("Glyph", SadConsole.UI.Themes.Library.Default.Colors.Green, 18);
            _lineCharPicker.EnableCharacterPicker = true;
            _lineCharPicker.DisableColorPicker = true;
            _lineCharPicker.SelectedColor = _lineBackColor.SelectedColor;
            _lineCharPicker.GlyphColor = _lineForeColor.SelectedColor;

            _lineForeColor.RightClickedColor += (s, e) => { var tempColor = _lineBackColor.SelectedColor; _lineBackColor.SelectedColor = _lineForeColor.SelectedColor; _lineForeColor.SelectedColor = tempColor; };
            _lineBackColor.RightClickedColor += (s, e) => { var tempColor = _lineForeColor.SelectedColor; _lineForeColor.SelectedColor = _lineBackColor.SelectedColor; _lineBackColor.SelectedColor = tempColor; };


            Controls = new ControlBase[] { _lineForeColor, _lineBackColor, _lineCharPicker };
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
