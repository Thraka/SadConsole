using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.Input;
using SadConsole.UI.Windows;
using ThemeEditor.Windows;

namespace ThemeEditor
{
    class SettingsConsole: ControlsConsole
    {
        private Point _shadesTextStart;
        private Rectangle _themePartsArea;
        private int _themePartSelectedIndex;
        private int _themePartSelectedAreaSize = 4;

        private AdjustableColor[] _themeParts;

        private ListBox _colorsListBox;

        private TextField _colorShadeFieldNormal;
        private TextField _colorShadeFieldBrightest;
        private TextField _colorShadeFieldBrighter;
        private TextField _colorShadeFieldDarker;
        private TextField _colorShadeFieldDarkest;
        private ColoredString _colorShadeText;
        private TextField _themePartsShadeBoxes;

        private RadioButton _themePartSettingIsCustomColor;
        private RadioButton _themePartSettingIsPredefinedColor;
        private ButtonBox _themePartSettingColorSet;

        private CheckBox _isThemeLight;

        public SettingsConsole(int width, int height) : base(width, height)
        {
            var colors = Controls.GetThemeColors();

            Container.PrintHeader(Surface, colors, (2, 2), width - 4, "Colors");

            // =================================
            // List box for color definitions
            _colorsListBox = new ListBox(17, 8);
            _colorsListBox.Position = (2, 3);
            Controls.Add(_colorsListBox);
            _colorsListBox.DrawBorder = true;
            _colorsListBox.ItemTheme = new ListBoxItemColorTheme();
            _colorsListBox.SelectedItemChanged += colorsListBox_SelectedItemChanged;

            // =================================
            // Shades region
            Surface.Print(_colorsListBox.Bounds.MaxExtentX + 2, _colorsListBox.Position.Y, "Shades", colors.Title);
            _shadesTextStart = (_colorsListBox.Bounds.MaxExtentX + 2, _colorsListBox.Position.Y + 1);

            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y, "150% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 1, "125% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 3, " 75% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 4, " 50% ");

            _colorShadeText = new ColoredString(new string((char)303, 8));

            _colorShadeFieldBrightest = new TextField(_shadesTextStart + (5, 0), 3, this.Surface);
            _colorShadeFieldBrighter = new TextField(_shadesTextStart + (5, 1), 3, this.Surface);
            _colorShadeFieldNormal = new TextField(_shadesTextStart + (0, 2), _colorShadeText.Length, this.Surface);
            _colorShadeFieldDarker = new TextField(_shadesTextStart + (5, 3), 3, this.Surface);
            _colorShadeFieldDarkest = new TextField(_shadesTextStart + (5, 4), 3, this.Surface);


            // =================================
            // Button to edit colors
            Button btn = new Button(8, 1)
            {
                Position = _shadesTextStart + (0, 6),
                Text = "Edit"
            };
            btn.Click += editColorsButton_Click;
            Controls.Add(btn);

            // =================================
            // Theme settings
            _themePartSettingIsCustomColor = new RadioButton(11, 1)
            {
                GroupName = "theme_color",
                Text = "Custom",
                IsVisible = false
            };
            _themePartSettingIsCustomColor.IsSelectedChanged += ThemePartSettingColorRadio_IsSelectedChanged;
            _themePartSettingIsPredefinedColor = new RadioButton(11, 1)
            {
                GroupName = "theme_color",
                Text = "Library",
                IsVisible = false
            };
            _themePartSettingIsPredefinedColor.IsSelectedChanged += ThemePartSettingColorRadio_IsSelectedChanged;
            _themePartSettingColorSet = new ButtonBox(11, 1)
            {
                Text = "Set Color",
                UseExtended = true,
                IsVisible = false
            };
            _themePartSettingColorSet.Click += themePartSettingColorSet_Click;
            
            Controls.Add(_themePartSettingIsCustomColor);
            Controls.Add(_themePartSettingIsPredefinedColor);
            Controls.Add(_themePartSettingColorSet);

            _themeParts = new AdjustableColor[16];
            _themePartsArea = new Rectangle(2, _colorsListBox.Bounds.MaxExtentY + 3, width - 4, _themeParts.Length + 5);


            Container.PrintHeader(Surface, colors, (_colorsListBox.Position.X, _themePartsArea.Y - 1), width - 4, "Theme Parts");

            // =================================
            // Light theme checkbox
            _isThemeLight = new CheckBox(_themePartsArea.MaxExtentX, 1)
            {
                Text = "Light color scheme",
                Position = (_themePartsArea.Position.X, _themePartsArea.MaxExtentY + 2)
            };
            _isThemeLight.IsSelectedChanged += ThemeLightColor_IsSelectedChanged;
            Controls.Add(_isThemeLight);

            // Redraw everything
            RefreshColors();
        }

        private void ThemeLightColor_IsSelectedChanged(object sender, EventArgs e)
        {
            if (Container.EditingColors.IsLightTheme != _isThemeLight.IsSelected)
            {
                Container.EditingColors.IsLightTheme = _isThemeLight.IsSelected;
                RefreshColors();
            }
        }

        private void themePartSettingColorSet_Click(object sender, EventArgs e)
        {
            AdjustableColor setting = _themeParts[_themePartSelectedIndex];

            if (setting.IsCustomColor)
            {
                ColorPickerPopup window = new ColorPickerPopup();
                window.Center();
                window.SelectedColor = _themeParts[_themePartSelectedIndex].BaseColor;
                window.Closed += (s, e) =>
                {
                    if (window.DialogResult)
                    {
                        _themeParts[_themePartSelectedIndex].SetColor(window.SelectedColor, Container.EditingColors, Colors.Brightness.Normal);
                        DrawThemeParts();
                    }
                };
                window.Show(true);
            }
            else
            {
                SelectPaletteColorPopup window = new SelectPaletteColorPopup(setting.UIColor);
                window.Center();
                window.Closed += (s, e) =>
                {
                    if (window.DialogResult)
                    {
                        _themeParts[_themePartSelectedIndex].SetUIColor(window.SelectedColor, Container.EditingColors, Colors.Brightness.Normal);
                        DrawThemeParts();
                    }
                };
                window.Show(true);
            }
        }

        private void editColorsButton_Click(object sender, EventArgs e)
        {
            ColorPickerPopup window = new ColorPickerPopup();
            window.Center();
            window.SelectedColor = ((ValueTuple<Color, string>)_colorsListBox.SelectedItem).Item1;
            window.Closed += (s, e) =>
            {
                if (window.DialogResult)
                {
                    var selectedColor = window.SelectedColor;
                    var selectedItem = (ValueTuple<Color, string>)_colorsListBox.SelectedItem;
                    var newItem = (selectedColor, selectedItem.Item2);

                    if (!Container.EditingColors.TryToColorName(selectedItem.Item1, out var colorEnumValue))
                        throw new Exception("How did this happen? Color not in editing colors collection");

                    _colorsListBox.Items.Insert(_colorsListBox.SelectedIndex, newItem);
                    _colorsListBox.Items.Remove(selectedItem);
                    _colorsListBox.SelectedItem = newItem;

                    Container.EditingColors.SetColorByName(colorEnumValue, selectedColor);

                    DrawThemeParts();
                }
            };
            window.Show(true);
        }

        private void colorsListBox_SelectedItemChanged(object? sender, ListBox.SelectedItemEventArgs e)
        {
            if (e.Item != null)
            {
                _colorShadeText = new ColoredString(new string((char)303, 8));
                Color color = ((ValueTuple<Color, string>)e.Item).Item1;
                _colorShadeText.SetForeground(color.GetBrightest()); _colorShadeFieldBrightest.Print(_colorShadeText);
                _colorShadeText.SetForeground(color.GetBright()); _colorShadeFieldBrighter.Print(_colorShadeText);
                _colorShadeText.SetForeground(color); _colorShadeFieldNormal.Print(_colorShadeText);
                _colorShadeText.SetForeground(color.GetDark()); _colorShadeFieldDarker.Print(_colorShadeText);
                _colorShadeText.SetForeground(color.GetDarkest()); _colorShadeFieldDarkest.Print(_colorShadeText);
            }
        }

        private void ThemePartSettingColorRadio_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_themePartSettingIsCustomColor.IsSelected && !_themeParts[_themePartSelectedIndex].IsCustomColor)
            {
                _themeParts[_themePartSelectedIndex].SetColor(Container.EditingColors.FromColorName(_themeParts[_themePartSelectedIndex].UIColor));
            }
            else if (!_themePartSettingIsCustomColor.IsSelected && _themeParts[_themePartSelectedIndex].IsCustomColor)
            {
                if (Container.EditingColors.TryToColorName(_themeParts[_themePartSelectedIndex].BaseColor, out var colorName))
                    _themeParts[_themePartSelectedIndex].SetUIColor(colorName, Container.EditingColors, _themeParts[_themePartSelectedIndex].Brightness);
                else
                    _themeParts[_themePartSelectedIndex].SetUIColor(Colors.ColorNames.White, Container.EditingColors, Colors.Brightness.Normal);
            }
            DrawThemeParts();
        }

        public void RefreshColors()
        {
            _colorsListBox.Items.Clear();

            _colorsListBox.Items.Add((Container.EditingColors.White, "White"));
            _colorsListBox.Items.Add((Container.EditingColors.Black, "Black"));
            _colorsListBox.Items.Add((Container.EditingColors.Gray, "Gray"));
            _colorsListBox.Items.Add((Container.EditingColors.GrayDark, "GrayDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Red, "Red"));
            _colorsListBox.Items.Add((Container.EditingColors.RedDark, "RedDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Green, "Green"));
            _colorsListBox.Items.Add((Container.EditingColors.GreenDark, "GreenDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Cyan, "Cyan"));
            _colorsListBox.Items.Add((Container.EditingColors.CyanDark, "CyanDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Blue, "Blue"));
            _colorsListBox.Items.Add((Container.EditingColors.BlueDark, "BlueDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Purple, "Purple"));
            _colorsListBox.Items.Add((Container.EditingColors.PurpleDark, "PurpleDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Yellow, "Yellow"));
            _colorsListBox.Items.Add((Container.EditingColors.YellowDark, "YellowDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Orange, "Orange"));
            _colorsListBox.Items.Add((Container.EditingColors.OrangeDark, "OrangeDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Brown, "Brown"));
            _colorsListBox.Items.Add((Container.EditingColors.BrownDark, "BrownDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Gold, "Gold"));
            _colorsListBox.Items.Add((Container.EditingColors.GoldDark, "GoldDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Silver, "Silver"));
            _colorsListBox.Items.Add((Container.EditingColors.SilverDark, "SilverDark"));
            _colorsListBox.Items.Add((Container.EditingColors.Bronze, "Bronze"));
            _colorsListBox.Items.Add((Container.EditingColors.BronzeDark, "BronzeDark"));

            _colorsListBox.SelectedItem = _colorsListBox.Items[0];

            _themeParts[0] = Container.EditingColors.Title;
            _themeParts[1] = Container.EditingColors.Lines;
            _themeParts[2] = Container.EditingColors.ControlForegroundNormal;
            _themeParts[3] = Container.EditingColors.ControlForegroundDisabled;
            _themeParts[4] = Container.EditingColors.ControlForegroundMouseOver;
            _themeParts[5] = Container.EditingColors.ControlForegroundMouseDown;
            _themeParts[6] = Container.EditingColors.ControlForegroundSelected;
            _themeParts[7] = Container.EditingColors.ControlForegroundFocused;
            _themeParts[8] = Container.EditingColors.ControlBackgroundNormal;
            _themeParts[9] = Container.EditingColors.ControlBackgroundDisabled;
            _themeParts[10] = Container.EditingColors.ControlBackgroundMouseOver;
            _themeParts[11] = Container.EditingColors.ControlBackgroundMouseDown;
            _themeParts[12] = Container.EditingColors.ControlBackgroundSelected;
            _themeParts[13] = Container.EditingColors.ControlBackgroundFocused;
            _themeParts[14] = Container.EditingColors.ControlHostForeground;
            _themeParts[15] = Container.EditingColors.ControlHostBackground;

            DrawThemeParts();

            _isThemeLight.IsSelected = Container.EditingColors.IsLightTheme;
        }

        string GetThemePartString(Color color, string part) =>
            $"[c:r f:{color.ToParser()}:2][c:sg 301]m[c:sg 302]m[c:r f:{Surface.DefaultForeground.ToParser()}] {part}";

        private void UpdateColors()
        {
            foreach (var item in _themeParts)
            {
                if (!item.IsCustomColor)
                    item.SetUIColor(item.UIColor, Container.EditingColors, item.Brightness);
            }

            Container.EditingColors.RebuildAppearances();
            ((Container)Parent)?.RefreshTestingPanel();
        }

        private void DrawThemeParts()
        {
            SadConsole.UI.Colors colors = Controls.GetThemeColors();
            Surface.Clear(_themePartsArea);
            int y = _themePartsArea.Y;
            int themeY = 0;

            UpdateColors();

            AdjustableColor selectedSetting = _themeParts[_themePartSelectedIndex];

            for (int i = 0; i < _themeParts.Length; i++)
            {
                int row = y + i;
                if (i == _themePartSelectedIndex)
                {
                    themeY = row;
                    Surface.Print(3, row, ColoredString.Parser.Parse(GetThemePartString(selectedSetting.ComputedColor, selectedSetting.Name.Replace("Control ", ""))));
                    Surface.SetGlyph(2, row, ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(2, row + 1), (_themePartsArea.MaxExtentX, row + 1), ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(_themePartsArea.MaxExtentX, row + 1), new Point(_themePartsArea.MaxExtentX, row + _themePartSelectedAreaSize), ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(_themePartsArea.MaxExtentX, row + _themePartSelectedAreaSize + 1), new Point(2, row + _themePartSelectedAreaSize + 1), ICellSurface.ConnectedLineThin[0], colors.Lines);

                    y += _themePartSelectedAreaSize + 1;
                }
                else
                {
                    Surface.Print(3, row, ColoredString.Parser.Parse(GetThemePartString(_themeParts[i].ComputedColor, _themeParts[i].Name.Replace("Control ", ""))));
                    Surface.SetGlyph(2, row, ICellSurface.ConnectedLineThin[0], colors.Lines);
                }
            }

            // Connect all the lines for the selected theme area
            Surface.ConnectLines(ICellSurface.ConnectedLineThin, _themePartsArea);

            // Draw the shade selection area
            _themePartsShadeBoxes = new TextField(new Point(_themePartsArea.MaxExtentX - 10, themeY + 3), 10, Surface);
            var colorPreviewText = new TextField(_themePartsShadeBoxes.Position.WithY(themeY + 2), 5, Surface);
            var colorPreviewBarsText = new TextField(new Point(colorPreviewText.Position.X + colorPreviewText.Width, themeY + 2), 5, Surface);
            var shadePreviewBarsText = new TextField(_themePartsShadeBoxes.Position.WithY(themeY + 4), 5, Surface);
            var shadePreviewText = new TextField(new Point(shadePreviewBarsText.Position.X + shadePreviewBarsText.Width, themeY + 4), 5, Surface);
            var intoColorPreviewText = new TextField(new Point(colorPreviewText.Position.X - 3, colorPreviewText.Position.Y), 3, Surface);

            var intoColorPreviewString = new ColoredString(3);
            intoColorPreviewString.SetForeground(colors.Lines);
            intoColorPreviewString.SetGlyph(ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.Top]);

            var colorPreviewString = new ColoredString(5);
            colorPreviewString.SetForeground(selectedSetting.BaseColor);
            colorPreviewString[0].Glyph = 301;
            colorPreviewString[1].Glyph = 303;
            colorPreviewString[2].Glyph = 303;
            colorPreviewString[3].Glyph = 303;
            colorPreviewString[4].Glyph = 302;

            var colorPreviewBarsString = new ColoredString(5);
            colorPreviewBarsString.SetForeground(colors.Lines);
            colorPreviewBarsString.SetGlyph(ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.Top]);
            colorPreviewBarsString[4].Glyph = ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.TopRight];

            var shadeBarsString = new ColoredString(10);
            shadeBarsString[0].Glyph = 299; shadeBarsString[0].Foreground = selectedSetting.BaseColor.GetBrightest();
            shadeBarsString[1].Glyph = 300; shadeBarsString[1].Foreground = selectedSetting.BaseColor.GetBrightest();
            shadeBarsString[2].Glyph = 299; shadeBarsString[2].Foreground = selectedSetting.BaseColor.GetBright();
            shadeBarsString[3].Glyph = 300; shadeBarsString[3].Foreground = selectedSetting.BaseColor.GetBright();
            shadeBarsString[4].Glyph = 299; shadeBarsString[4].Foreground = selectedSetting.BaseColor;
            shadeBarsString[5].Glyph = 300; shadeBarsString[5].Foreground = selectedSetting.BaseColor;
            shadeBarsString[6].Glyph = 299; shadeBarsString[6].Foreground = selectedSetting.BaseColor.GetDark();
            shadeBarsString[7].Glyph = 300; shadeBarsString[7].Foreground = selectedSetting.BaseColor.GetDark();
            shadeBarsString[8].Glyph = 299; shadeBarsString[8].Foreground = selectedSetting.BaseColor.GetDarkest();
            shadeBarsString[9].Glyph = 300; shadeBarsString[9].Foreground = selectedSetting.BaseColor.GetDarkest();

            var shadePreviewBarsString = new ColoredString(5);
            shadePreviewBarsString.SetForeground(colors.Lines);
            shadePreviewBarsString.SetGlyph(ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.Top]);
            shadePreviewBarsString[0].Glyph = ICellSurface.ConnectedLineThin[(int)ICellSurface.ConnectedLineIndex.BottomLeft];

            var shadePreviewString = new ColoredString(5);
            shadePreviewString.SetForeground(selectedSetting.ComputedColor);
            shadePreviewString[0].Glyph = 301;
            shadePreviewString[1].Glyph = 303;
            shadePreviewString[2].Glyph = 303;
            shadePreviewString[3].Glyph = 303;
            shadePreviewString[4].Glyph = 302;

            intoColorPreviewText.Print(intoColorPreviewString);
            colorPreviewText.Print(colorPreviewString);
            colorPreviewBarsText.Print(colorPreviewBarsString);
            shadePreviewBarsText.Print(shadePreviewBarsString);
            shadePreviewText.Print(shadePreviewString);

            // Position the controls
            _themePartSettingColorSet.Position = (3, themeY + 2);
            _themePartSettingIsPredefinedColor.Position = (3, themeY + 3);
            _themePartSettingIsCustomColor.Position = (3, themeY + 4);
            _themePartSettingColorSet.IsVisible = true;
            _themePartSettingIsCustomColor.IsVisible = true;
            _themePartSettingIsPredefinedColor.IsVisible = true;

            // Configure controls/shade
            _themePartSettingIsCustomColor.IsSelected = selectedSetting.IsCustomColor;
            _themePartSettingIsPredefinedColor.IsSelected = !selectedSetting.IsCustomColor;

            switch (selectedSetting.Brightness)
            {
                case Colors.Brightness.Brightest:
                    shadeBarsString[0].Glyph = 301;
                    shadeBarsString[1].Glyph = 302;
                    break;
                case Colors.Brightness.Bright:
                    shadeBarsString[2].Glyph = 301;
                    shadeBarsString[3].Glyph = 302;
                    break;
                case Colors.Brightness.Normal:
                    shadeBarsString[4].Glyph = 301;
                    shadeBarsString[5].Glyph = 302;
                    break;
                case Colors.Brightness.Dark:
                    shadeBarsString[6].Glyph = 301;
                    shadeBarsString[7].Glyph = 302;
                    break;
                case Colors.Brightness.Darkest:
                    shadeBarsString[8].Glyph = 301;
                    shadeBarsString[9].Glyph = 302;
                    break;
                default:
                    break;
            }
            _themePartsShadeBoxes.Print(shadeBarsString);


            Controls.IsDirty = true;
        }

        protected override void OnMouseMove(MouseScreenObjectState state)
        {
            if (_themePartsArea.Contains(state.CellPosition) && state.Mouse.LeftClicked)
            {
                var index = state.CellPosition.Y - _themePartsArea.Y;

                if (index == _themePartSelectedIndex) return;
                
                if (index < _themePartSelectedIndex)
                {
                    _themePartSelectedIndex = index;
                    DrawThemeParts();
                    return;
                }

                if (index > _themePartSelectedIndex + _themePartSelectedAreaSize + 1)
                {
                    _themePartSelectedIndex = index - _themePartSelectedAreaSize - 1;

                    if (_themePartSelectedIndex < _themeParts.Length)
                        DrawThemeParts();
                    return;
                }

                if (_themePartsShadeBoxes.ContainsPoint(state.CellPosition))
                {
                    var boxPosition = (state.CellPosition - _themePartsShadeBoxes.Position).ToIndex(_themePartsShadeBoxes.Width) / 2;

                    _themeParts[_themePartSelectedIndex].Brightness = boxPosition switch
                    {
                        0 => Colors.Brightness.Brightest,
                        1 => Colors.Brightness.Bright,
                        2 => Colors.Brightness.Normal,
                        3 => Colors.Brightness.Dark,
                        4 => Colors.Brightness.Darkest,
                        _ => throw new IndexOutOfRangeException()
                    };

                    DrawThemeParts();
                    return;
                }

                //if (index > _themePartSelectedIndex && index < _themePartSelectedIndex + _themePartSelectedAreaSize + 1) return;

                //index -= _themePartSelectedAreaSize + 1;

                //if (index >= _themeParts.Length + _themePartSelectedAreaSize + 1)
                //{
                //    _themePartSelectedIndex = index;
                //    DrawThemeParts();
                //    return;
                //}

                //if (index >= _themePartSelectedIndex + _themePartSelectedAreaSize + 1)
                //    _themePartSelectedIndex = index + 2;
                //else
                //    _themePartSelectedIndex = index;

                //DrawThemeParts();
            }
        }

        class TextField
        {
            public readonly Point Position;
            public readonly int Width;

            private ICellSurface _surface;

            public TextField(Point position, int width, ICellSurface surface) =>
                (Position, Width, _surface) = (position, width, surface);

            public void Print(ColoredString value)
            {
                if (value == null || value.Length == 0)
                {
                    Clear();
                    return;
                }

                if (value.Length > Width)
                    value = value.SubString(0, Width);

                _surface.Clear(Position.X, Position.Y, Width);
                _surface.Print(Position.X, Position.Y, value);
            }

            public void Clear() =>
                _surface.Clear(Position.X, Position.Y, Width);

            public bool ContainsPoint(Point position)
            {
                int start = Position.ToIndex(_surface.Width);
                int index = position.ToIndex(_surface.Width);
                return index >= start && index < start + Width;
            }
        }

    }
}
