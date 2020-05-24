using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System;
using System.Reflection;

namespace SadConsoleEditor.Windows
{
    class OtherColorsPopup : SadConsole.UI.Window
    {
        private bool _isCreated;
        private RadioButton _ansiSelectButton;
        private RadioButton _knownSelectButton;

        private Button _okButton;
        private Button _cancelButton;

        private Button[] _ansiButtons = new Button[16];

        private ListBox _namedColorsList;

        private Color _selectedAnsiColor;
        private Point _selectedAnsiColorIcon;
        private Point _selectedAnsiColorIconPrevious;

        public Color SelectedColor { get; private set; }

        public OtherColorsPopup()
            : base(40, 20)
        {
            Center();

            _ansiSelectButton = new RadioButton(Width - 4, 1);
            _ansiSelectButton.Position = new Point(2, 2);
            _ansiSelectButton.Text = "Ansi Colors";
            _ansiSelectButton.CanFocus = false;
            _ansiSelectButton.IsSelectedChanged += _ansiSelectButton_IsSelectedChanged;
            Add(_ansiSelectButton);

            _knownSelectButton = new RadioButton(Width - 4, 1);
            _knownSelectButton.Position = new Point(2, 3);
            _knownSelectButton.Text = "List of Known Colors";
            _knownSelectButton.CanFocus = false;
            _knownSelectButton.IsSelectedChanged += _ansiSelectButton_IsSelectedChanged;
            Add(_knownSelectButton);

            #region Ansi Buttons
            int ansiButtonStartY = 5;
            int ansiButtonStartX = 3;
            int ansiButtonStartBrightX = 20;

            _ansiButtons[0] = new Button(15, 1);
            _ansiButtons[0].Position = new Point(ansiButtonStartX, ansiButtonStartY);
            _ansiButtons[0].Text = "Red Dark";
            _ansiButtons[0].Theme = new AnsiButtonTheme(ColorAnsi.Red, ColorAnsi.RedBright);
            _ansiButtons[0].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[0]);

            _ansiButtons[1] = new Button(15, 1);
            _ansiButtons[1].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY);
            _ansiButtons[1].Text = "Red Bright";
            _ansiButtons[1].Theme = new AnsiButtonTheme(ColorAnsi.RedBright, ColorAnsi.Red);
            _ansiButtons[1].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[1]);

            _ansiButtons[2] = new Button(15, 1);
            _ansiButtons[2].Position = new Point(ansiButtonStartX, ansiButtonStartY + 1);
            _ansiButtons[2].Text = "Yellow Dark";
            _ansiButtons[2].Theme = new AnsiButtonTheme(ColorAnsi.Yellow, ColorAnsi.YellowBright);
            _ansiButtons[2].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[2]);

            _ansiButtons[3] = new Button(15, 1);
            _ansiButtons[3].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 1);
            _ansiButtons[3].Text = "Yellow Bright";
            _ansiButtons[3].Theme = new AnsiButtonTheme(ColorAnsi.YellowBright, ColorAnsi.Yellow);
            _ansiButtons[3].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[3]);

            _ansiButtons[4] = new Button(15, 1);
            _ansiButtons[4].Position = new Point(ansiButtonStartX, ansiButtonStartY + 2);
            _ansiButtons[4].Text = "Green Dark";
            _ansiButtons[4].Theme = new AnsiButtonTheme(ColorAnsi.Green, ColorAnsi.GreenBright);
            _ansiButtons[4].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[4]);

            _ansiButtons[5] = new Button(15, 1);
            _ansiButtons[5].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 2);
            _ansiButtons[5].Text = "Green Bright";
            _ansiButtons[5].Theme = new AnsiButtonTheme(ColorAnsi.GreenBright, ColorAnsi.Green);
            _ansiButtons[5].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[5]);

            _ansiButtons[6] = new Button(15, 1);
            _ansiButtons[6].Position = new Point(ansiButtonStartX, ansiButtonStartY + 3);
            _ansiButtons[6].Text = "Cyan Dark";
            _ansiButtons[6].Theme = new AnsiButtonTheme(ColorAnsi.Cyan, ColorAnsi.CyanBright);
            _ansiButtons[6].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[6]);

            _ansiButtons[7] = new Button(15, 1);
            _ansiButtons[7].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 3);
            _ansiButtons[7].Text = "Cyan Bright";
            _ansiButtons[7].Theme = new AnsiButtonTheme(ColorAnsi.CyanBright, ColorAnsi.Cyan);
            _ansiButtons[7].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[7]);

            _ansiButtons[8] = new Button(15, 1);
            _ansiButtons[8].Position = new Point(ansiButtonStartX, ansiButtonStartY + 4);
            _ansiButtons[8].Text = "Blue Dark";
            _ansiButtons[8].Theme = new AnsiButtonTheme(ColorAnsi.Blue, ColorAnsi.BlueBright);
            _ansiButtons[8].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[8]);

            _ansiButtons[9] = new Button(15, 1);
            _ansiButtons[9].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 4);
            _ansiButtons[9].Text = "Blue Bright";
            _ansiButtons[9].Theme = new AnsiButtonTheme(ColorAnsi.BlueBright, ColorAnsi.Blue);
            _ansiButtons[9].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[9]);

            _ansiButtons[10] = new Button(15, 1);
            _ansiButtons[10].Position = new Point(ansiButtonStartX, ansiButtonStartY + 5);
            _ansiButtons[10].Text = "Magenta Dark";
            _ansiButtons[10].Theme = new AnsiButtonTheme(ColorAnsi.Magenta, ColorAnsi.MagentaBright);
            _ansiButtons[10].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[10]);

            _ansiButtons[11] = new Button(15, 1);
            _ansiButtons[11].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 5);
            _ansiButtons[11].Text = "Magenta Bright";
            _ansiButtons[11].Theme = new AnsiButtonTheme(ColorAnsi.MagentaBright, ColorAnsi.Magenta);
            _ansiButtons[11].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[11]);

            _ansiButtons[12] = new Button(15, 1);
            _ansiButtons[12].Position = new Point(ansiButtonStartX, ansiButtonStartY + 6);
            _ansiButtons[12].Text = "Black Dark";
            _ansiButtons[12].Theme = new AnsiButtonTheme(ColorAnsi.Black, ColorAnsi.BlackBright);
            _ansiButtons[12].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[12]);

            _ansiButtons[13] = new Button(15, 1);
            _ansiButtons[13].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 6);
            _ansiButtons[13].Text = "Black Bright";
            _ansiButtons[13].Theme = new AnsiButtonTheme(ColorAnsi.BlackBright, ColorAnsi.Black);
            _ansiButtons[13].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[13]);

            _ansiButtons[14] = new Button(15, 1);
            _ansiButtons[14].Position = new Point(ansiButtonStartX, ansiButtonStartY + 7);
            _ansiButtons[14].Text = "White Dark";
            _ansiButtons[14].Theme = new AnsiButtonTheme(ColorAnsi.White, ColorAnsi.WhiteBright);
            _ansiButtons[14].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[14]);

            _ansiButtons[15] = new Button(15, 1);
            _ansiButtons[15].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 7);
            _ansiButtons[15].Text = "White Bright";
            _ansiButtons[15].Theme = new AnsiButtonTheme(ColorAnsi.WhiteBright, ColorAnsi.White);
            _ansiButtons[15].Click += _ansiGreenBright_Click;
            Add(_ansiButtons[15]);
            #endregion

            #region Named Color Control
            _namedColorsList = new ListBox(Width - 4, Height - 3 - ansiButtonStartY, new SadConsole.UI.Themes.ListBoxItemColorTheme());
            _namedColorsList.Position = new Point(ansiButtonStartX - 1, ansiButtonStartY);
            Add(_namedColorsList);
            
            // Fill out the named colors
            Color testColor = Color.AliceBlue;
            Type colorType = testColor.GetType();
            if (null != colorType)
            {
                PropertyInfo[] propInfoList =
                 colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly
                    | BindingFlags.Public);
                int nNumProps = propInfoList.Length;
                for (int i = 0; i < nNumProps; i++)
                {
                    PropertyInfo propInfo = (PropertyInfo)propInfoList[i];
                    Color color = (Color)propInfo.GetValue(null, null);
                    _namedColorsList.Items.Add(new Tuple<Color, Color, string>(color, new Color(255 - color.R, 255 - color.G, 255 - color.B), propInfo.Name));
                }
            }
            _namedColorsList.SelectedItem = _namedColorsList.Items[0];
            _namedColorsList.SelectedItemChanged += (s, e) => { var a = _namedColorsList.Theme; };
            #endregion

            _cancelButton = new Button(12, 1);
            _cancelButton.Position = new Point(2, Height - 2);
            _cancelButton.Text = "Cancel";
            _cancelButton.Click += (sender, e) => { DialogResult = false; Hide(); };
            Add(_cancelButton);

            _okButton = new Button(12, 1);
            _okButton.Position = new Point(Width - 2 - _okButton.Width, Height - 2);
            _okButton.Text = "OK";
            _okButton.Click += (sender, e) => { SelectedColor = _ansiSelectButton.IsSelected ? _selectedAnsiColor : (Color)((Tuple<Color, Color, string>)_namedColorsList.SelectedItem).Item1; DialogResult = true; Hide(); };
            Add(_okButton);

            _ansiSelectButton.IsSelected = true;
            this.CloseOnEscKey = true;
            this.Title = "Pick a known color";

            _isCreated = true;
            IsDirty = true;

            _ansiButtons[0].DoClick();
        }

        void _ansiSelectButton_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_knownSelectButton.IsSelected)
            {
                for (int i = 0; i < 16; i++)
                    _ansiButtons[i].IsVisible = false;

                _namedColorsList.IsVisible = true;
            }
            else
            {
                for (int i = 0; i < 16; i++)
                    _ansiButtons[i].IsVisible = true;

                _namedColorsList.IsVisible = false;
            }

            IsDirty = true;
        }

        void _ansiGreenBright_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                _selectedAnsiColor = ((Button)sender).Theme.Normal.Background;
                _selectedAnsiColorIconPrevious = _selectedAnsiColorIcon;
                _selectedAnsiColorIcon = new Point(((Button)sender).Position.X - 1, ((Button)sender).Position.Y);

                IsDirty = true;
            }
        }
        
        protected override void OnInvalidated()
        {
            if (!_isCreated) return;

            base.OnInvalidated();
            var colors = GetThemeColors();

            if (!_namedColorsList.IsVisible)
            {
                if (_selectedAnsiColorIconPrevious != Point.None)
                    this[_selectedAnsiColorIconPrevious.X, _selectedAnsiColorIconPrevious.Y].Glyph = 0;

                if (_selectedAnsiColorIcon != Point.None)
                {
                    this[_selectedAnsiColorIcon.X, _selectedAnsiColorIcon.Y].Glyph = 16;
                    this[_selectedAnsiColorIcon.X, _selectedAnsiColorIcon.Y].Foreground = colors.TitleText;
                }
            }

            // Bar above buttons
            int lineY = Height - 3;
            for (int x = 1; x < Width - 1; x++)
            {
                this[x, lineY].Glyph = 196;
            }
            this[0, lineY].Glyph = 199;
            this[Width - 1, lineY].Glyph = 182;
        }

        private class AnsiButtonTheme: SadConsole.UI.Themes.ButtonTheme
        {
            public AnsiButtonTheme(Color color, Color textColor)
            {
                Normal =
                    MouseOver =
                    MouseDown =
                    Focused =
                    Disabled =
                    new SadConsole.ColoredGlyph(textColor, color);

                ShowEnds = false;
            }
        }
    }
}
