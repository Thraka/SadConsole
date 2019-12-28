using Microsoft.Xna.Framework;
using SadConsole.Controls;
using SadConsoleEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Windows
{
    class ColorPickerPopup: SadConsole.Window
    {
        private const int BarHeightStart = 21;
        private const int RideSideX = 18;

        private ColorBar _barR;
        private ColorBar _barG;
        private ColorBar _barB;
        private HueBar _barH;
        private ColorPicker _picker;
        private Color _selectedColor;

        private Button _okButton;
        private Button _otherColorsButton;
        private Button _cancelButton;

        private InputBox _redInput;
        private InputBox _greenInput;
        private InputBox _blueInput;
        private InputBox _alphaInput;

        private OtherColorsPopup otherColorPopup;

        private ListBox<ListBoxItemColor> _previousColors;
        private static List<Color> _previousColorList = new List<Color>();

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    _barH.SelectedColor = _selectedColor;
                    _picker.SelectedColor = _selectedColor;
                    _alphaInput.Text = _selectedColor.A.ToString();
                }
            }
        }

        public Color[] PreviousColors
        {
            get { return _previousColors.Items.Cast<Color>().ToArray(); }
        }
        
        public ColorPickerPopup(): base(Settings.Config.ColorPickerSettings.WindowWidth, Settings.Config.ColorPickerSettings.WindowHeight)
        {
            Center();

            otherColorPopup = new OtherColorsPopup();
            otherColorPopup.Closed += (sender2, e2) =>
            {
                if (otherColorPopup.DialogResult)
                {
                    _barR.SelectedColor = otherColorPopup.SelectedColor.RedOnly();
                    _barG.SelectedColor = otherColorPopup.SelectedColor.GreenOnly();
                    _barB.SelectedColor = otherColorPopup.SelectedColor.BlueOnly();
                    _alphaInput.Text = otherColorPopup.SelectedColor.A.ToString();
                }
            };

            _picker = new Controls.ColorPicker(TextSurface.Width - RideSideX - 1, TextSurface.Height - 11, Color.YellowGreen) { Position = new Point(1, 1) };
            _picker.SelectedColorChanged += _picker_SelectedColorChanged;
            Add(_picker);

            #region TextBoxes
            _redInput = new InputBox(5);
            _redInput.IsNumeric = true;
            _redInput.MaxLength = 3;
            _redInput.Position = new Point(TextSurface.Width - 7, TextSurface.Height - 14);
            _redInput.TextChanged += (sender, e) => { _barR.SelectedColor = new Color(int.Parse(_redInput.Text), 0, 0); };
            Add(_redInput);

            _greenInput = new InputBox(5);
            _greenInput.IsNumeric = true;
            _greenInput.MaxLength = 3;
            _greenInput.Position = new Point(TextSurface.Width - 7, TextSurface.Height - 13);
            _greenInput.TextChanged += (sender, e) => { _barG.SelectedColor = new Color(0, int.Parse(_greenInput.Text), 0); };
            Add(_greenInput);

            _blueInput = new InputBox(5);
            _blueInput.IsNumeric = true;
            _blueInput.MaxLength = 3;
            _blueInput.Position = new Point(TextSurface.Width - 7, TextSurface.Height - 12);
            _blueInput.TextChanged += (sender, e) => { _barB.SelectedColor = new Color(0, 0, int.Parse(_blueInput.Text)); };
            Add(_blueInput);

            _alphaInput = new InputBox(5);
            _alphaInput.IsNumeric = true;
            _alphaInput.MaxLength = 3;
            _alphaInput.Position = new Point(TextSurface.Width - 7, TextSurface.Height - 11);
            _alphaInput.Text = "255";
            Add(_alphaInput);
            #endregion

            #region Bars
            _barH = new HueBar(base.TextSurface.RenderArea.Width - 2);

            _barH.Position = new Point(1, TextSurface.Height - 9);
            _barH.ColorChanged += _barH_ColorChanged;
            Add(_barH);

            _barR = new ColorBar(base.TextSurface.RenderArea.Width - 2);

            _barR.StartingColor = Color.Black;
            _barR.EndingColor = Color.Red;
            _barR.Position = new Point(1, TextSurface.Height - 7);
            _barR.ColorChanged += bar_ColorChanged;
            Add(_barR);

            _barG = new ColorBar(base.TextSurface.RenderArea.Width - 2);

            _barG.StartingColor = Color.Black;
            _barG.EndingColor = new Color(0, 255, 0);
            _barG.Position = new Point(1, TextSurface.Height - 5);
            _barG.ColorChanged += bar_ColorChanged;
            Add(_barG);

            _barB = new ColorBar(base.TextSurface.RenderArea.Width - 2);

            _barB.StartingColor = Color.Black;
            _barB.EndingColor = Color.Blue;
            _barB.Position = new Point(1, TextSurface.Height - 3);
            _barB.ColorChanged += bar_ColorChanged;
            Add(_barB);


            _selectedColor = _picker.SelectedColor;
            _barH.SelectedColor = _selectedColor;
            #endregion

            #region Buttons
            _okButton = new Button(RideSideX - 4);
            _okButton.Text = "OK";
            _okButton.Position = new Point(TextSurface.Width - RideSideX + 2, TextSurface.Height - 18);
            _okButton.Click += (sender, r) =>
            {
                this.DialogResult = true;
                _selectedColor.A = byte.Parse(_alphaInput.Text);
                AddPreviousColor(SelectedColor);
                Hide();
            };
            Add(_okButton);

            _cancelButton = new Button(RideSideX - 4);
            _cancelButton.Text = "Cancel";
            _cancelButton.Position = new Point(TextSurface.Width - RideSideX + 2, TextSurface.Height - 16);
            _cancelButton.Click += (sender, r) => { this.DialogResult = false; Hide(); };
            Add(_cancelButton);

            _otherColorsButton = new Button(RideSideX - 4);
            _otherColorsButton.Text = "Other Colors";
            _otherColorsButton.Position = new Point(TextSurface.Width - RideSideX + 2, TextSurface.Height - 20);
            _otherColorsButton.Click += (sender, e) => { otherColorPopup.Show(true); };
            Add(_otherColorsButton);
            #endregion

            _previousColors = new ListBox<ListBoxItemColor>(RideSideX - 4, TextSurface.Height - 20 - 9 + 1);
            _previousColors.Position = new Point(TextSurface.Width - RideSideX + 2, 8);
            _previousColors.SelectedItemChanged += (sender, e) => { if (_previousColors.SelectedItem != null) SelectedColor = (Color)_previousColors.SelectedItem; };
            Add(_previousColors);

            this.CloseOnESC = true;
            this.Title = "Select Color";
        }

        void _picker_SelectedColorChanged(object sender, EventArgs e)
        {
            _selectedColor = _picker.SelectedColor;
            Redraw();
        }

        void _barH_ColorChanged(object sender, EventArgs e)
        {
            _barR.SelectedColor = new Color(_barH.SelectedColor.R, 0, 0);
            _barG.SelectedColor = new Color(0, _barH.SelectedColor.G, 0);
            _barB.SelectedColor = new Color(0, 0, _barH.SelectedColor.B);

            _redInput.Text = _barH.SelectedColor.R.ToString();
            _greenInput.Text = _barH.SelectedColor.G.ToString();
            _blueInput.Text = _barH.SelectedColor.B.ToString();

            _alphaInput.Text = "255";
        }

        void bar_ColorChanged(object sender, EventArgs e)
        {
            _picker.SelectedHue = new Color(_barR.SelectedColor.R, _barG.SelectedColor.G, _barB.SelectedColor.B);

            _redInput.Text = _barR.SelectedColor.R.ToString();
            _greenInput.Text = _barG.SelectedColor.G.ToString();
            _blueInput.Text = _barB.SelectedColor.B.ToString();
        }

        public override void Redraw()
        {
            base.Redraw();
            int lineY = TextSurface.Height - 10;
            int lineX = TextSurface.Width - RideSideX;

            // Bar above color bars
            for (int x = 1; x < lineX; x++)
            {
                textSurface[x, lineY].Glyph = 196;
            }
            textSurface[0, lineY].Glyph = 199;
            textSurface[lineX, lineY].Glyph = 217;

            // Long bar right of color picker
            for (int y = 1; y < lineY; y++)
            {
                textSurface[lineX, y].Glyph = 179;
            }
            textSurface[lineX, lineY].Glyph = 217;
            textSurface[lineX, 0].Glyph = 209;


            // Bar above red input
            for (int x = lineX + 1; x < TextSurface.Width - 1; x++)
            {
                textSurface[x, lineY - 5].Glyph = 205;// 196;
            }
            //SadConsole.Surfaces.Basic[lineX, lineY - 5].Glyph = 195;
            //SadConsole.Surfaces.Basic[TextSurface.Width - 1, lineY - 5].Glyph = 182;
            textSurface[lineX, lineY - 5].Glyph = 198;
            textSurface[TextSurface.Width - 1, lineY - 5].Glyph = 185;

            Print(lineX + 2, lineY - 4, "Red", ColorAnsi.Red);
            Print(lineX + 2, lineY - 3, "Green", ColorAnsi.Green);
            Print(lineX + 2, lineY - 2, "Blue", ColorAnsi.BlueBright);
            Print(lineX + 2, lineY - 1, "Alpha", Color.Gray);

            // Preview area
            Print(lineX + 2, 1, "Selected Color");

            SadConsole.Shapes.Box box = new SadConsole.Shapes.Box();
            box.Fill = true;
            box.FillColor = SelectedColor;
            box.Width = 14;
            box.Height = 3;
            box.Position = new Point(lineX + 2, 2);
            box.TopLeftCharacter = box.TopRightCharacter = box.TopSideCharacter = 223;
            box.BottomLeftCharacter = box.BottomRightCharacter = box.BottomSideCharacter = 220;
            box.Foreground = Theme.FillStyle.Background;

            box.Draw(this);

            // Current selected gradient colors
            Print(lineX + 2, 5, SelectedColor.R.ToString().PadLeft(3, '0'), ColorAnsi.Red);
            Print(lineX + 2, 6, SelectedColor.G.ToString().PadLeft(3, '0'), ColorAnsi.Green);
            Print(lineX + 13, 5, SelectedColor.B.ToString().PadLeft(3, '0'), ColorAnsi.BlueBright);

            // Previous Colors
            Print(lineX + 2, 8, "Prior Colors");

        }
        
        public void AddPreviousColor(Color color)
        {
            if (!_previousColors.Items.Contains(color))
            {
                if (!_previousColorList.Contains(color))
                    _previousColorList.Add(color);

                _previousColors.Items.Add(color);
            }
        }

        public override void Show(bool modal)
        {
            _previousColors.Items.Clear();

            foreach (var item in _previousColorList)
                _previousColors.Items.Add(item);

            base.Show(modal);
        }
    }
}
