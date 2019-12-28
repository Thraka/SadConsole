using System;
using SadConsole.Input;
using SadConsole.UI.Themes;
using SadConsole.UI.Controls; 
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace SadConsoleEditor.Controls
{
    public class ColorPresenter : SadConsole.UI.Controls.ControlBase
    {
        public class ThemeType: ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is ColorPresenter presenter)) throw new Exception($"Theme can only be added to a {nameof(ColorPresenter)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.DefaultForeground = presenter._selectedColor;
                control.Surface.DefaultBackground = Color.Transparent;
                control.Surface.Clear();
                base.Attached(control);
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is ColorPresenter presenter)) return;

                if (!presenter.IsDirty) return;

                ColoredGlyph appearance;

                RefreshTheme(control.FindThemeColors(), control);

                if (Helpers.HasFlag((int)presenter.State, (int)ControlStates.Disabled))
                    appearance = Disabled;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(presenter.State, ControlStates.MouseRightButtonDown))
                //    appearance = MouseDown;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseOver))
                //    appearance = MouseOver;

                else if (Helpers.HasFlag((int)presenter.State, (int)ControlStates.Focused))
                    appearance = Focused;

                else
                    appearance = Normal;

                var middle = (presenter.Height != 1 ? presenter.Height / 2 : 0);

                // Redraw the control
                presenter.Surface.Fill(
                    appearance.Foreground,
                    appearance.Background,
                    appearance.Glyph, null);

                presenter.Surface.Print(0, 0, presenter._title);

                presenter.Surface.Print(presenter.Surface.BufferWidth - 3, 0, "   ", Color.Black, presenter._selectedColor);
                if (presenter._selectedGlyph != 0)
                {
                    presenter.Surface.SetGlyph(presenter.Surface.BufferWidth - 2, 0, presenter._selectedGlyph);
                    presenter.Surface.SetForeground(presenter.Surface.BufferWidth - 2, 0, presenter._selectedGlyphColor);
                }
                
                presenter.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ThemeType()
                {
                    Normal = Normal.Clone(),
                    Disabled = Disabled.Clone(),
                    MouseOver = MouseOver.Clone(),
                    MouseDown = MouseDown.Clone(),
                    Selected = Selected.Clone(),
                    Focused = Focused.Clone(),
                };
            }
        }


        public event EventHandler ColorChanged;
        public event EventHandler GlyphChanged;
        public event EventHandler RightClickedColor;

        private Color _selectedColor;
        private string _title;
        private Windows.ColorPickerPopup _popupColorPicker;
        private Windows.CharacterQuickSelectPopup _popupGlyphPicker;
        private Mirror _selectedGlyphMirror;
        private int _selectedGlyph;
        private Color _selectedGlyphColor;

        public string Title { get { return _title; } set { _title = value; IsDirty = true; } }
        

        public bool DisableColorPicker { get; set; }
        public bool EnableCharacterPicker { get; set; }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    ColorChanged?.Invoke(this, EventArgs.Empty);
                    IsDirty = true;
                }
            }
        }

        public int SelectedGlyph
        {
            get { return _selectedGlyph; }
            set
            {
                if (_selectedGlyph != value)
                {
                    _selectedGlyph = value;
                    GlyphChanged?.Invoke(this, EventArgs.Empty);
                    IsDirty = true;
                }
            }
        }

        public Mirror SelectedGlyphMirror
        {
            get => _selectedGlyphMirror;
            set
            {
                if (_selectedGlyphMirror != value)
                {
                    _selectedGlyphMirror = value;
                    GlyphChanged?.Invoke(this, EventArgs.Empty);
                    IsDirty = true;
                }
            }
        }

        public Color GlyphColor
        {
            get { return _selectedGlyphColor; }
            set
            {
                if (_selectedGlyphColor != value)
                {
                    _selectedGlyphColor = value;
                    IsDirty = true;
                }
            }
        }

        public ColorPresenter(string title, Color defaultColor, int width): base(width, 1)
        {
            _selectedColor = defaultColor;
            Theme = new ThemeType();
            _title = title;
            _popupColorPicker = new Windows.ColorPickerPopup();
            _popupColorPicker.Closed += (o, e) =>
                {
                    if (_popupColorPicker.DialogResult)
                        SelectedColor = _popupColorPicker.SelectedColor;
                };

            _popupGlyphPicker = new Windows.CharacterQuickSelectPopup(0);
            _popupGlyphPicker.Font = Config.Program.ScreenFont;
            _popupGlyphPicker.Closed += (o, e) => { SelectedGlyph = _popupGlyphPicker.SelectedCharacter; };
        }
        
        protected override void OnLeftMouseClicked(MouseScreenObjectState info)
        {
            if (!DisableColorPicker)
            {
                var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                if (location.X >= Width - 3)
                {
                    base.OnLeftMouseClicked(info);
                    _popupColorPicker.SelectedColor = _selectedColor;
                    _popupColorPicker.Show(true);
                }
            }
            else if (EnableCharacterPicker)
            {
                var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
                if (location.X >= Width - 3)
                {
                    base.OnLeftMouseClicked(info);
                    _popupGlyphPicker.SelectedCharacter = _selectedGlyph;
                    _popupGlyphPicker.MirrorEffect = _selectedGlyphMirror;
                    _popupGlyphPicker.Center();
                    _popupGlyphPicker.Show(true);
                }
            }
        }

        protected override void OnRightMouseClicked(MouseScreenObjectState info)
        {
            var location = this.TransformConsolePositionByControlPosition(info.CellPosition);
            if (location.X >= Width - 3)
            {
                base.OnRightMouseClicked(info);
                RightClickedColor?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
