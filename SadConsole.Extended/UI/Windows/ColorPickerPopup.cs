﻿using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.UI.Windows
{
    /// <summary>
    /// A window that allows a user to select a color in various ways.
    /// </summary>
    public class ColorPickerPopup: SadConsole.UI.Window
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

        private NumberBox _redInput;
        private NumberBox _greenInput;
        private NumberBox _blueInput;
        private NumberBox _alphaInput;

        private OtherColorsPopup otherColorPopup;

        private ListBox _previousColors;
        private static List<Color> _previousColorList = new List<Color>();

        /// <summary>
        /// The color selected.
        /// </summary>
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    _barH.SelectedColor = _selectedColor;
                    _picker.MasterColor = _selectedColor;
                    _alphaInput.Text = _selectedColor.A.ToString();
                }
            }
        }

        /// <summary>
        /// An array of colors previously selected.
        /// </summary>
        public Color[] PreviousColors
        {
            get { return _previousColors.Items.Cast<Color>().ToArray(); }
        }

        /// <summary>
        /// Creates a new instance of the window.
        /// </summary>
        public ColorPickerPopup(): base(60, 35)
        {
            Border.CreateForWindow(this);
            Title = "Select color";
            CloseOnEscKey = true;
            CanDrag = false;

            UsePixelPositioning = true;
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

            _picker = new Controls.ColorPicker(Width - RideSideX - 1, Height - 11, Color.YellowGreen) { Position = new Point(1, 1) };
            _picker.SelectedColorChanged += _picker_SelectedColorChanged;
            Controls.Add(_picker);

            #region NumberBoxes
            _redInput = new NumberBox(5);
            _redInput.NumberMaximum = 255;
            _redInput.MaxLength = 3;
            _redInput.Position = new Point(Width - 7, Height - 14);
            _redInput.TextChanged += (sender, e) => { _barR.SelectedColor = new Color(int.Parse(_redInput.Text), 0, 0); };
            Controls.Add(_redInput);

            _greenInput = new NumberBox(5);
            _greenInput.NumberMaximum = 255;
            _greenInput.MaxLength = 3;
            _greenInput.Position = new Point(Width - 7, Height - 13);
            _greenInput.TextChanged += (sender, e) => { _barG.SelectedColor = new Color(0, int.Parse(_greenInput.Text), 0); };
            Controls.Add(_greenInput);

            _blueInput = new NumberBox(5);
            _blueInput.NumberMaximum = 255;
            _blueInput.MaxLength = 3;
            _blueInput.Position = new Point(Width - 7, Height - 12);
            _blueInput.TextChanged += (sender, e) => { _barB.SelectedColor = new Color(0, 0, int.Parse(_blueInput.Text)); };
            Controls.Add(_blueInput);

            _alphaInput = new NumberBox(5);
            _alphaInput.NumberMaximum = 255;
            _alphaInput.MaxLength = 3;
            _alphaInput.Position = new Point(Width - 7, Height - 11);
            _alphaInput.Text = "255";
            Controls.Add(_alphaInput);
            #endregion

            #region Bars
            _barH = new HueBar(Width - 2);

            _barH.Position = new Point(1, Height - 9);
            _barH.ColorChanged += _barH_ColorChanged;
            Controls.Add(_barH);

            _barR = new ColorBar(Width - 2);

            _barR.StartingColor = Color.Black;
            _barR.EndingColor = Color.Red;
            _barR.Position = new Point(1, Height - 7);
            _barR.ColorChanged += bar_ColorChanged;
            Controls.Add(_barR);

            _barG = new ColorBar(Width - 2);

            _barG.StartingColor = Color.Black;
            _barG.EndingColor = new Color(0, 255, 0);
            _barG.Position = new Point(1, Height - 5);
            _barG.ColorChanged += bar_ColorChanged;
            Controls.Add(_barG);

            _barB = new ColorBar(Width - 2);

            _barB.StartingColor = Color.Black;
            _barB.EndingColor = Color.Blue;
            _barB.Position = new Point(1, Height - 3);
            _barB.ColorChanged += bar_ColorChanged;
            Controls.Add(_barB);


            _selectedColor = _picker.SelectedColor;
            _barH.SelectedColor = _selectedColor;
            #endregion

            #region Buttons
            _okButton = new Button(RideSideX - 4);
            _okButton.Text = "OK";
            _okButton.Position = new Point(Width - RideSideX + 2, Height - 18);
            _okButton.Click += (sender, r) =>
            {
                this.DialogResult = true;
                _selectedColor = _selectedColor.SetAlpha(byte.Parse(_alphaInput.Text));
                AddPreviousColor(SelectedColor);
                Hide();
            };
            Controls.Add(_okButton);

            _cancelButton = new Button(RideSideX - 4);
            _cancelButton.Text = "Cancel";
            _cancelButton.Position = new Point(Width - RideSideX + 2, Height - 17);
            _cancelButton.Click += (sender, r) => { this.DialogResult = false; Hide(); };
            Controls.Add(_cancelButton);

            _otherColorsButton = new Button(RideSideX - 4);
            _otherColorsButton.Text = "Other Colors";
            _otherColorsButton.Position = new Point(Width - RideSideX + 2, Height - 20);
            _otherColorsButton.Click += (sender, e) => { otherColorPopup.Show(true); };
            Controls.Add(_otherColorsButton);
            #endregion

            _previousColors = new ListBox(RideSideX - 4, Height - 20 - 9 + 1, new ListBoxItemColorTheme());
            _previousColors.Position = new Point(Width - RideSideX + 2, 8);
            _previousColors.SelectedItemChanged += (sender, e) => { if (_previousColors.SelectedItem != null) SelectedColor = (Color)_previousColors.SelectedItem; };
            Controls.Add(_previousColors);

            var colors = Controls.GetThemeColors();

            this.Print(Width - RideSideX + 2, _redInput.Position.Y, "Red", colors.Red);
            this.Print(Width - RideSideX + 2, _greenInput.Position.Y, "Green", colors.Green);
            this.Print(Width - RideSideX + 2, _blueInput.Position.Y, "Blue", colors.Blue);
            this.Print(Width - RideSideX + 2, _alphaInput.Position.Y, "Alpha", colors.Gray);

            // Preview area
            this.Print(Width - RideSideX + 2, 1, "Selected Color");

            this.Fill(new Rectangle(Width - RideSideX + 2, 2, 14, 3), colors.ControlHostForeground, colors.ControlHostBackground, 219);

            // Current selected gradient colors
            this.Print(Width - RideSideX + 2, 5, SelectedColor.R.ToString().PadLeft(3, '0'), colors.Red);
            this.Print(Width - RideSideX + 2, 6, SelectedColor.G.ToString().PadLeft(3, '0'), colors.Green);
            this.Print(Width - RideSideX + 13, 5, SelectedColor.B.ToString().PadLeft(3, '0'), colors.Blue);

            // Previous Colors
            this.Print(Width - RideSideX + 2, _previousColors.Position.Y - 1, "Prior Colors");
        }

        void _picker_SelectedColorChanged(object sender, EventArgs e)
        {
            _selectedColor = _picker.SelectedColor;

            // Current selected gradient colors
            int lineX = Width - RideSideX;
            var colors = Controls.GetThemeColors();
            this.Print(lineX + 2, 5, SelectedColor.R.ToString().PadLeft(3, '0'), colors.Red);
            this.Print(lineX + 2, 6, SelectedColor.G.ToString().PadLeft(3, '0'), colors.Green);
            this.Print(lineX + 13, 5, SelectedColor.B.ToString().PadLeft(3, '0'), colors.Blue);

            this.Fill(new Rectangle(lineX + 2, 2, 14, 3), SelectedColor, colors.ControlHostBackground, 219);

            IsDirty = true;
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
            _picker.MasterColor = new Color(_barR.SelectedColor.R, _barG.SelectedColor.G, _barB.SelectedColor.B);

            _redInput.Text = _barR.SelectedColor.R.ToString();
            _greenInput.Text = _barG.SelectedColor.G.ToString();
            _blueInput.Text = _barB.SelectedColor.B.ToString();
        }

        /// <summary>
        /// Draws the border lines around the controls.
        /// </summary>
        protected override void DrawBorder()
        {
            base.DrawBorder();
            var colors = Controls.GetThemeColors();
            var fillStyle = new ColoredGlyph(colors.Lines, colors.ControlHostBackground);


            int lineY = Height - 10;
            int lineX = Width - RideSideX;


            // Bar above color bars
            this.DrawLine(new Point(1, lineY), new Point(lineX, lineY), BorderLineStyle[0], colors.Lines);
            //for (int x = 1; x < lineX; x++)
            //{
            //    this[x, lineY].Glyph = 196;
            //}
            //this[0, lineY].Glyph = 199;
            //this[lineX, lineY].Glyph = 217;

            // Long bar right of color picker
            this.DrawLine(new Point(lineX, 1), new Point(lineX, lineY), BorderLineStyle[0], colors.Lines);
            //for (int y = 1; y < lineY; y++)
            //{
            //    this[lineX, y].Glyph = 179;
            //}
            //this[lineX, lineY].Glyph = 217;
            //this[lineX, 0].Glyph = 209;


            // Bar above red input
            this.DrawLine(new Point(lineX + 1, lineY - 5), new Point(Width - 2, lineY - 5), BorderLineStyle[0], colors.Lines);
            //for (int x = lineX + 1; x < Width - 1; x++)
            //{
            //    this[x, lineY - 5].Glyph = 205;// 196;
            //}
            //this[lineX, lineY - 5].Glyph = 198;
            //this[Width - 1, lineY - 5].Glyph = 185;

            this.ConnectLines(BorderLineStyle);

        }

        /// <summary>
        /// Adds a color to the array of previous colors.
        /// </summary>
        /// <param name="color"></param>
        public void AddPreviousColor(Color color)
        {
            if (!_previousColors.Items.Contains(color))
            {
                if (!_previousColorList.Contains(color))
                    _previousColorList.Add(color);

                _previousColors.Items.Add(color);
            }
        }

        /// <inheritdoc/>
        public override void Show(bool modal)
        {
            if (IsVisible) return;

            _previousColors.Items.Clear();

            foreach (var item in _previousColorList)
                _previousColors.Items.Add(item);

            base.Show(modal);
        }
    }
}
