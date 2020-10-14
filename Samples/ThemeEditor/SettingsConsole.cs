using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.Input;

namespace ThemeEditor
{
    class SettingsConsole: ControlsConsole
    {
        private Point _shadesTextStart;
        private Rectangle _themePartsArea;
        private int _themePartSelectedIndex;
        private int _themePartSelectedAreaSize = 4;
        private int _themePartSelectedIndexOld = -1;

        private AdjustableColor[] _themeParts;

        private RadioButton _themePartControlCustomRadio;
        private RadioButton _themePartControlColorRadio;
        
        private TextField _colorShadeFieldNormal;
        private TextField _colorShadeFieldBrightest;
        private TextField _colorShadeFieldBrighter;
        private TextField _colorShadeFieldDarker;
        private TextField _colorShadeFieldDarkest;
        private ColoredString _colorShadeText;
        private TextField _themePartsShadeBoxes;

        private RadioButton _themePartSettingIsCustomColor;
        private RadioButton _themePartSettingIsPredefinedColor;
        private Button _themePartSettingColorSet;
        private Point _themePartSettingShadeBrightestPosition;
        private Point _themePartSettingShadeBrighterPosition;
        private Point _themePartSettingShadeNormalPosition;
        private Point _themePartSettingShadeDarkerPosition;
        private Point _themePartSettingShadeDarkestPosition;


        public SettingsConsole(int width, int height) : base(width, height)
        {
            var colors = Controls.GetThemeColors();

            Container.PrintHeader(Surface, colors, (2, 2), width - 4, "Colors");

            // =================================
            // List box for color definitions
            ListBox lst = new ListBox(17, 8, new SadConsole.UI.Themes.ListBoxItemColorTheme());
            lst.Position = (2, 3);
            Controls.Add(lst);
            ((SadConsole.UI.Themes.ListBoxTheme)lst.Theme).DrawBorder = true;
            FillListBoxColors(lst);
            lst.SelectedItemChanged += (s, e) =>
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
            };

            // =================================
            // Shades region
            Surface.Print(lst.Bounds.MaxExtentX + 2, lst.Position.Y, "Shades", colors.Title);
            _shadesTextStart = (lst.Bounds.MaxExtentX + 2, lst.Position.Y + 1);

            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y, "150% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 1, "125% ");
            //Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 2, "100% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 3, " 75% ");
            Surface.Print(_shadesTextStart.X, _shadesTextStart.Y + 4, " 50% ");

            _colorShadeText = new ColoredString(new string((char)303, 8));

            _colorShadeFieldBrightest = new TextField(_shadesTextStart + (5, 0), 3, this.Surface);
            _colorShadeFieldBrighter = new TextField(_shadesTextStart + (5, 1), 3, this.Surface);
            _colorShadeFieldNormal = new TextField(_shadesTextStart + (0, 2), _colorShadeText.Length, this.Surface);
            _colorShadeFieldDarker = new TextField(_shadesTextStart + (5, 3), 3, this.Surface);
            _colorShadeFieldDarkest = new TextField(_shadesTextStart + (5, 4), 3, this.Surface);

            lst.SelectedItem = lst.Items[0];

            // =================================
            // Button to edit colors
            Button btn = new Button(8, 1)
            {
                Position = _shadesTextStart + (0, 6),
                Text = "Edit"
            };
            btn.Click += (s, e) =>
            {
                ColorPickerPopup window = new ColorPickerPopup();
                window.Center();
                window.SelectedColor = ((ValueTuple<Color, string>)lst.SelectedItem).Item1;
                window.Closed += (s, e) =>
                {
                    if (window.DialogResult)
                    {
                        var selectedColor = window.SelectedColor;
                        var selectedItem = (ValueTuple<Color, string>)lst.SelectedItem;
                        var newItem = (selectedColor, selectedItem.Item2);

                        if (!Container.EditingColors.TryToColorName(selectedItem.Item1, out var colorEnumValue))
                            throw new Exception("How did this happen? Color not in editing colors collection");

                        lst.Items.Insert(lst.SelectedIndex, newItem);
                        lst.Items.Remove(selectedItem);
                        lst.SelectedItem = newItem;

                        Container.EditingColors.SetColorByName(colorEnumValue, selectedColor);

                        DrawThemeParts();
                    }
                };
                window.Show(true);
            };
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
            _themePartSettingColorSet = new Button(11, 1)
            {
                Text = "Set Color",
                Theme = new SadConsole.UI.Themes.ButtonLinesTheme(),
                IsVisible = false
            };
            _themePartSettingColorSet.Click += (s, e) =>
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
                    SelectPaletteColor window = new SelectPaletteColor(setting.UIColor);
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
            };

            Controls.Add(_themePartSettingIsCustomColor);
            Controls.Add(_themePartSettingIsPredefinedColor);
            Controls.Add(_themePartSettingColorSet);

            _themeParts = new AdjustableColor[16];
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

            _themePartsArea = new Rectangle(2, lst.Bounds.MaxExtentY + 3, width - 4, _themeParts.Length + 5);

            Container.PrintHeader(Surface, colors, (lst.Position.X, _themePartsArea.Y - 1), width - 4, "Theme Parts");
            DrawThemeParts();
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

        string GetThemePartString(Color color, string part) =>
            $"[c:r f:{color.ToParser()}:2][c:sg 301]m[c:sg 302]m[c:r f:{DefaultForeground.ToParser()}] {part}";

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
                    Surface.Print(3, row, ColoredString.Parse(GetThemePartString(selectedSetting.ComputedColor, selectedSetting.Name.Replace("Control ", ""))));
                    Surface.SetGlyph(2, row, ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(2, row + 1), (_themePartsArea.MaxExtentX, row + 1), ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(_themePartsArea.MaxExtentX, row + 1), new Point(_themePartsArea.MaxExtentX, row + _themePartSelectedAreaSize), ICellSurface.ConnectedLineThin[0], colors.Lines);
                    Surface.DrawLine(new Point(_themePartsArea.MaxExtentX, row + _themePartSelectedAreaSize + 1), new Point(2, row + _themePartSelectedAreaSize + 1), ICellSurface.ConnectedLineThin[0], colors.Lines);

                    y += _themePartSelectedAreaSize + 1;
                }
                else
                {
                    Surface.Print(3, row, ColoredString.Parse(GetThemePartString(_themeParts[i].ComputedColor, _themeParts[i].Name.Replace("Control ", ""))));
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

        private void FillListBoxColors(ListBox lst)
        {
            lst.Items.Add((Container.EditingColors.White, "White"));
            lst.Items.Add((Container.EditingColors.Black, "Black"));
            lst.Items.Add((Container.EditingColors.Gray, "Gray"));
            lst.Items.Add((Container.EditingColors.GrayDark, "GrayDark"));
            lst.Items.Add((Container.EditingColors.Red, "Red"));
            lst.Items.Add((Container.EditingColors.RedDark, "RedDark"));
            lst.Items.Add((Container.EditingColors.Green, "Green"));
            lst.Items.Add((Container.EditingColors.GreenDark, "GreenDark"));
            lst.Items.Add((Container.EditingColors.Cyan, "Cyan"));
            lst.Items.Add((Container.EditingColors.CyanDark, "CyanDark"));
            lst.Items.Add((Container.EditingColors.Blue, "Blue"));
            lst.Items.Add((Container.EditingColors.BlueDark, "BlueDark"));
            lst.Items.Add((Container.EditingColors.Purple, "Purple"));
            lst.Items.Add((Container.EditingColors.PurpleDark, "PurpleDark"));
            lst.Items.Add((Container.EditingColors.Yellow, "Yellow"));
            lst.Items.Add((Container.EditingColors.YellowDark, "YellowDark"));
            lst.Items.Add((Container.EditingColors.Orange, "Orange"));
            lst.Items.Add((Container.EditingColors.OrangeDark, "OrangeDark"));
            lst.Items.Add((Container.EditingColors.Brown, "Brown"));
            lst.Items.Add((Container.EditingColors.BrownDark, "BrownDark"));
            lst.Items.Add((Container.EditingColors.Gold, "Gold"));
            lst.Items.Add((Container.EditingColors.GoldDark, "GoldDark"));
            lst.Items.Add((Container.EditingColors.Silver, "Silver"));
            lst.Items.Add((Container.EditingColors.SilverDark, "SilverDark"));
            lst.Items.Add((Container.EditingColors.Bronze, "Bronze"));
            lst.Items.Add((Container.EditingColors.BronzeDark, "BronzeDark"));
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
                int start = Position.ToIndex(_surface.BufferWidth);
                int index = position.ToIndex(_surface.BufferWidth);
                return index >= start && index < start + Width;
            }
        }

    }
}
