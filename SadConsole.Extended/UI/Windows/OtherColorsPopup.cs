using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System;
using System.Reflection;
using System.Linq;
using SadConsole;

namespace SadConsole.UI.Windows
{
    /// <summary>
    /// A window that displays all known colors.
    /// </summary>
    public class OtherColorsPopup : SadConsole.UI.Window
    {
        private RadioButton _ansiSelectButton;
        private RadioButton _knownSelectButton;

        private Button _okButton;
        private Button _cancelButton;

        private Button[] _ansiButtons = new Button[16];

        private ListBox _namedColorsList;

        private Color _selectedAnsiColor;
        private Point _selectedAnsiColorIcon = Point.None;
        private Point _selectedAnsiColorIconPrevious = Point.None;

        /// <summary>
        /// The selected color.
        /// </summary>
        public Color SelectedColor { get; private set; }

        /// <summary>
        /// Creates a new instance of this window with size 40, 20.
        /// </summary>
        public OtherColorsPopup()
            : base(40, 20)
        {
            Border.CreateForWindow(this);
            Title = "Pick known color";
            Center();
            CloseOnEscKey = true;
            CanDrag = false;

            _ansiSelectButton = new RadioButton(Width - 4, 1);
            _ansiSelectButton.Position = new Point(2, 2);
            _ansiSelectButton.Text = "Ansi Colors";
            _ansiSelectButton.CanFocus = false;
            _ansiSelectButton.IsSelectedChanged += _ansiSelectButton_IsSelectedChanged;
            Controls.Add(_ansiSelectButton);

            _knownSelectButton = new RadioButton(Width - 4, 1);
            _knownSelectButton.Position = new Point(2, 3);
            _knownSelectButton.Text = "List of Known Colors";
            _knownSelectButton.CanFocus = false;
            _knownSelectButton.IsSelectedChanged += _ansiSelectButton_IsSelectedChanged;
            Controls.Add(_knownSelectButton);

            #region Ansi Buttons
            int ansiButtonStartY = 5;
            int ansiButtonStartX = 3;
            int ansiButtonStartBrightX = 20;

            _ansiButtons[0] = new AnsiButton(Color.AnsiRed, Color.AnsiRedBright, 15, 1);
            _ansiButtons[0].Position = new Point(ansiButtonStartX, ansiButtonStartY);
            _ansiButtons[0].Text = "Red Dark";
            _ansiButtons[0].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[0]);

            _ansiButtons[1] = new AnsiButton(Color.AnsiRedBright, Color.AnsiRed, 15, 1);
            _ansiButtons[1].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY);
            _ansiButtons[1].Text = "Red Bright";
            _ansiButtons[1].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[1]);

            _ansiButtons[2] = new AnsiButton(Color.AnsiYellow, Color.AnsiYellowBright, 15, 1);
            _ansiButtons[2].Position = new Point(ansiButtonStartX, ansiButtonStartY + 1);
            _ansiButtons[2].Text = "Yellow Dark";
            _ansiButtons[2].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[2]);

            _ansiButtons[3] = new AnsiButton(Color.AnsiYellowBright, Color.AnsiYellow, 15, 1);
            _ansiButtons[3].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 1);
            _ansiButtons[3].Text = "Yellow Bright";
            _ansiButtons[3].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[3]);

            _ansiButtons[4] = new AnsiButton(Color.AnsiGreen, Color.AnsiGreenBright, 15, 1);
            _ansiButtons[4].Position = new Point(ansiButtonStartX, ansiButtonStartY + 2);
            _ansiButtons[4].Text = "Green Dark";
            _ansiButtons[4].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[4]);

            _ansiButtons[5] = new AnsiButton(Color.AnsiGreenBright, Color.AnsiGreen, 15, 1);
            _ansiButtons[5].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 2);
            _ansiButtons[5].Text = "Green Bright";
            _ansiButtons[5].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[5]);

            _ansiButtons[6] = new AnsiButton(Color.AnsiCyan, Color.AnsiCyanBright, 15, 1);
            _ansiButtons[6].Position = new Point(ansiButtonStartX, ansiButtonStartY + 3);
            _ansiButtons[6].Text = "Cyan Dark";
            _ansiButtons[6].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[6]);

            _ansiButtons[7] = new AnsiButton(Color.AnsiCyanBright, Color.AnsiCyan, 15, 1);
            _ansiButtons[7].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 3);
            _ansiButtons[7].Text = "Cyan Bright";
            _ansiButtons[7].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[7]);

            _ansiButtons[8] = new AnsiButton(Color.AnsiBlue, Color.AnsiBlueBright, 15, 1);
            _ansiButtons[8].Position = new Point(ansiButtonStartX, ansiButtonStartY + 4);
            _ansiButtons[8].Text = "Blue Dark";
            _ansiButtons[8].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[8]);

            _ansiButtons[9] = new AnsiButton(Color.AnsiBlueBright, Color.AnsiBlue, 15, 1);
            _ansiButtons[9].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 4);
            _ansiButtons[9].Text = "Blue Bright";
            _ansiButtons[9].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[9]);

            _ansiButtons[10] = new AnsiButton(Color.AnsiMagenta, Color.AnsiMagentaBright, 15, 1);
            _ansiButtons[10].Position = new Point(ansiButtonStartX, ansiButtonStartY + 5);
            _ansiButtons[10].Text = "Magenta Dark";
            _ansiButtons[10].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[10]);

            _ansiButtons[11] = new AnsiButton(Color.AnsiMagentaBright, Color.AnsiMagenta, 15, 1);
            _ansiButtons[11].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 5);
            _ansiButtons[11].Text = "Magenta Bright";
            _ansiButtons[11].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[11]);

            _ansiButtons[12] = new AnsiButton(Color.AnsiBlack, Color.AnsiBlackBright, 15, 1);
            _ansiButtons[12].Position = new Point(ansiButtonStartX, ansiButtonStartY + 6);
            _ansiButtons[12].Text = "Black Dark";
            _ansiButtons[12].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[12]);

            _ansiButtons[13] = new AnsiButton(Color.AnsiBlackBright, Color.AnsiBlack, 15, 1);
            _ansiButtons[13].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 6);
            _ansiButtons[13].Text = "Black Bright";
            _ansiButtons[13].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[13]);

            _ansiButtons[14] = new AnsiButton(Color.AnsiWhite, Color.AnsiWhiteBright, 15, 1);
            _ansiButtons[14].Position = new Point(ansiButtonStartX, ansiButtonStartY + 7);
            _ansiButtons[14].Text = "White Dark";
            _ansiButtons[14].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[14]);

            _ansiButtons[15] = new AnsiButton(Color.AnsiWhiteBright, Color.AnsiWhite, 15, 1);
            _ansiButtons[15].Position = new Point(ansiButtonStartBrightX, ansiButtonStartY + 7);
            _ansiButtons[15].Text = "White Bright";
            _ansiButtons[15].Click += AnsiColorButton_Click;
            Controls.Add(_ansiButtons[15]);
            #endregion

            #region Named Color Control
            _namedColorsList = new ListBox(Width - 4, Height - 3 - ansiButtonStartY, new ListBoxItemColorTheme());
            _namedColorsList.Position = new Point(ansiButtonStartX - 1, ansiButtonStartY);
            Controls.Add(_namedColorsList);

            // Fill out the named colors

            var colorType = typeof(Color);
            foreach (FieldInfo item in colorType.GetFields(BindingFlags.Public | BindingFlags.Static).Where((t) => t.FieldType.Name == colorType.Name))
            {
                var color = (Color)item.GetValue(null);
                _namedColorsList.Items.Add((color, item.Name));
            }

            _namedColorsList.SelectedItem = _namedColorsList.Items[0];
            _namedColorsList.SelectedItemChanged += (s, e) => { var a = _namedColorsList.ThemeState; };
            #endregion

            _cancelButton = new Button(12, 1);
            _cancelButton.Position = new Point(2, Height - 2);
            _cancelButton.Text = "Cancel";
            _cancelButton.Click += (sender, e) => { DialogResult = false; Hide(); };
            Controls.Add(_cancelButton);

            _okButton = new Button(12, 1);
            _okButton.Position = new Point(Width - 2 - _okButton.Width, Height - 2);
            _okButton.Text = "OK";
            _okButton.Click += (sender, e) => { SelectedColor = _ansiSelectButton.IsSelected ? _selectedAnsiColor : (Color)(((Color, string))_namedColorsList.SelectedItem).Item1; DialogResult = true; Hide(); };
            Controls.Add(_okButton);

            _ansiSelectButton.IsSelected = true;
            this.CloseOnEscKey = true;
            
            _ansiButtons[0].InvokeClick();
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

        void AnsiColorButton_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                _selectedAnsiColor = ((Button)sender).ThemeState.Normal.Background;
                _selectedAnsiColorIconPrevious = _selectedAnsiColorIcon;
                _selectedAnsiColorIcon = new Point(((Button)sender).Position.X - 1, ((Button)sender).Position.Y);

                if (!_namedColorsList.IsVisible)
                {
                    var colors = Controls.GetThemeColors();

                    if (_selectedAnsiColorIconPrevious != Point.None)
                        Surface[_selectedAnsiColorIconPrevious.X, _selectedAnsiColorIconPrevious.Y].Glyph = 0;

                    if (_selectedAnsiColorIcon != Point.None)
                    {
                        Surface[_selectedAnsiColorIcon.X, _selectedAnsiColorIcon.Y].Glyph = 16;
                        Surface[_selectedAnsiColorIcon.X, _selectedAnsiColorIcon.Y].Foreground = colors.Title;
                    }
                }

                IsDirty = true;
            }
        }

        /// <summary>
        /// Draws the border around the controls.
        /// </summary>
        protected override void DrawBorder()
        {
            base.DrawBorder();
            var colors = Controls.GetThemeColors();
            this.DrawLine(new Point(1, Height - 3), new Point(Width - 1, Height - 3), BorderLineStyle[0], colors.Lines);
            this.ConnectLines(BorderLineStyle);
        }

        private class AnsiButton : SadConsole.UI.Controls.Button
        {
            private Color _colorValue;
            private Color _textColor;

            public AnsiButton(Color assignedColor, Color textColor, int width, int height) : base(width, height)
            {
                _textColor = textColor;
                _colorValue = assignedColor;

                ShowEnds = false;
            }

            protected override void RefreshThemeStateColors(Colors colors)
            {
                ThemeState.Normal =
                    ThemeState.MouseOver =
                    ThemeState.MouseDown =
                    ThemeState.Focused =
                    ThemeState.Disabled =
                    new SadConsole.ColoredGlyph(_textColor, _colorValue);
            }
        }
    }
}
