namespace SadConsoleEditor.Controls
{
    using SadRogue.Primitives;
    using SadConsole;
    using SadConsole.UI.Controls;
    using SadConsole.UI.Themes;
    using System;
    using Console = SadConsole.Console;

    public class CharacterPicker: SadConsole.UI.Controls.ControlBase
    {
        public class ThemeType : ThemeBase
        {
            /// <inheritdoc />
            public override void Attached(ControlBase control)
            {
                if (!(control is CharacterPicker)) throw new Exception($"Theme can only be added to a {nameof(CharacterPicker)}");

                control.Surface = new CellSurface(control.Width, control.Height);
                control.Surface.Clear();
            }

            /// <inheritdoc />
            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                if (!(control is CharacterPicker picker)) return;

                if (!control.IsDirty) return;

                RefreshTheme(control.FindThemeColors(), control);

                ColoredGlyph appearance;

                if (Helpers.HasFlag((int)control.State, (int)ControlStates.Disabled))
                    appearance = ControlThemeState.Disabled;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(presenter.State, ControlStates.MouseRightButtonDown))
                //    appearance = MouseDown;

                //else if (Helpers.HasFlag(presenter.State, ControlStates.MouseOver))
                //    appearance = MouseOver;

                else if (Helpers.HasFlag((int)control.State, (int)ControlStates.Focused))
                    appearance = ControlThemeState.Focused;

                else
                    appearance = ControlThemeState.Normal;

                if (picker._newCharacterLocation != new Point(-1, -1))
                {
                    control.Surface.SetEffect(picker._oldCharacterLocation.X, picker._oldCharacterLocation.Y, null);
                }
                
                control.Surface.Fill(picker._charForeground, picker._fill, 0, null);

                int i = 0;

                for (int y = 0; y < Config.Program.ScreenFont.Rows; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        control.Surface.SetGlyph(x, y, i);
                        control.Surface.SetMirror(x, y, picker._mirrorEffect);
                        i++;
                    }
                }

                control.Surface.SetForeground(picker._newCharacterLocation.X, picker._newCharacterLocation.Y, picker._selectedCharColor);
                control.Surface.SetEffect(picker._newCharacterLocation.X, picker._newCharacterLocation.Y, picker._selectedCharEffect);

                control.IsDirty = false;
            }

            /// <inheritdoc />
            public override ThemeBase Clone()
            {
                return new ThemeType()
                {
                    ControlThemeState = ControlThemeState.Clone()
                };
            }
        }


        private Color _charForeground;
        private Color _fill;
        private Color _selectedCharColor;
        Mirror _mirrorEffect;

        private SadConsole.UI.Controls.DrawingSurface _characterSurface;
        private SadConsole.Effects.Fade _selectedCharEffect;
        private int _selectedChar;

        private Point _oldCharacterLocation = new Point(-1, -1);
        private Point _newCharacterLocation = new Point(-1, -1);

        public event EventHandler<SelectedCharacterEventArgs> SelectedCharacterChanged;
        public bool UseFullClick;
        
        public Mirror MirrorEffect
        {
            get { return _mirrorEffect; }
            set
            {
                _mirrorEffect = value;
                IsDirty = true;
            }
        }

        public int SelectedCharacter
        {
            get { return _selectedChar; }
            set
            {
                if (_selectedChar == value) return;

                var old = _selectedChar;
                _selectedChar = value;

                _oldCharacterLocation = Point.FromIndex(old, 16);
                _newCharacterLocation = Point.FromIndex(value, 16);

                SelectedCharacterChanged?.Invoke(this, new SelectedCharacterEventArgs(old, value));

                IsDirty = true;
            }
        }

        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, Font characterFont)
            : this(foreground, fill, selectedCharacterColor)
        {
            _characterSurface.AlternateFont = characterFont;
            Theme = new ThemeType();
        }
        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor):base(16, Config.Program.ScreenFont.Rows)
        {
            this.UseMouse = true;
            Theme = new ThemeType();

            _selectedCharColor = selectedCharacterColor;
            _charForeground = foreground;
            _fill = fill;

            //_characterSurface = new SadConsole.UI.Controls.DrawingSurface(16, 16);
            //_characterSurface.DefaultBackground = fill;
            //_characterSurface.DefaultForeground = foreground;
            //_characterSurface.Clear();

            _selectedCharEffect = new SadConsole.Effects.Fade()
            {
                FadeBackground = true,
                UseCellBackground = false,
                DestinationBackground = new ColorGradient(_fill, _selectedCharColor * 0.8f),
                FadeDuration = 2d,
                CloneOnApply = false,
                AutoReverse = true,
                Repeat = true,
            };

            SelectedCharacter = 1;
        }
        protected override void OnMouseIn(ControlMouseState info)
        {
            if (new Rectangle(0, 0, 16, Config.Program.ScreenFont.Rows).Contains(info.MousePosition) && info.OriginalMouseState.Mouse.LeftButtonDown)
            {
                if (!UseFullClick)
                    SelectedCharacter = Surface[info.MousePosition.ToIndex(16)].Glyph;
            }

            base.OnMouseIn(info);
        }

        protected override void OnLeftMouseClicked(ControlMouseState info)
        {
            if (new Rectangle(0, 0, 16, Config.Program.ScreenFont.Rows).Contains(info.MousePosition))
            {
                SelectedCharacter = Surface[info.MousePosition.ToIndex(16)].Glyph;
            }
            
            base.OnLeftMouseClicked(info);
        }

        public class SelectedCharacterEventArgs: EventArgs
        {
            public int NewCharacter;
            public int OldCharacter;

            public SelectedCharacterEventArgs(int oldCharacter, int newCharacter)
            {
                NewCharacter = newCharacter;
                OldCharacter = oldCharacter;
            }
        }
    }
}
