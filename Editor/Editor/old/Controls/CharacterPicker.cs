namespace SadConsoleEditor.Controls
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using System;
    using Console = SadConsole.Console;

    class CharacterPicker: SadConsole.Controls.ControlBase
    {
        private Color _charForeground;
        private Color _fill;
        private Color _selectedCharColor;
        Microsoft.Xna.Framework.Graphics.SpriteEffects _mirrorEffect;
        private SadConsole.Effects.EffectsManager effectsManager;

        private SadConsole.Controls.DrawingSurface _characterSurface;
        private SadConsole.Effects.Fade _selectedCharEffect;
        private int _selectedChar;

        public event EventHandler<SelectedCharacterEventArgs> SelectedCharacterChanged;
        public bool UseFullClick;

        public Microsoft.Xna.Framework.Graphics.SpriteEffects MirrorEffect
        {
            get { return _mirrorEffect; }
            set
            {
                _mirrorEffect = value;
                Compose();
            }
        }

        public int SelectedCharacter
        {
            get { return _selectedChar; }
            set
            {
                int old = _selectedChar;
                _selectedChar = value;

                var oldLocation = SadConsole.Surfaces.SadConsole.Surfaces.Basic.GetPointFromIndex(old, 16);
                var newLocation = SadConsole.Surfaces.SadConsole.Surfaces.Basic.GetPointFromIndex(value, 16);

                this.SetForeground(oldLocation.X, oldLocation.Y, _charForeground);
                this.SetForeground(newLocation.X, newLocation.Y, _selectedCharColor);

                effectsManager.SetEffect(this[oldLocation.X, oldLocation.Y], null);
                effectsManager.SetEffect(this[newLocation.X, newLocation.Y], _selectedCharEffect);
                
                if (SelectedCharacterChanged != null)
                    SelectedCharacterChanged(this, new SelectedCharacterEventArgs(old, value));

                Compose();
            }
        }

        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, Font characterFont)
            : this(foreground, fill, selectedCharacterColor)
        {
            _characterSurface.AlternateFont = characterFont;
        }
        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor):base(16, SadConsoleEditor.Settings.Config.ScreenFont.Rows)
        {
            effectsManager = new SadConsole.Effects.EffectsManager(textSurface);
            textSurface.DefaultForeground = _charForeground = foreground;
            textSurface.DefaultBackground = _fill = fill;
            Clear();
            

            this.UseMouse = true;

            _selectedCharColor = selectedCharacterColor;

            //_characterSurface = new SadConsole.Controls.DrawingSurface(16, 16);
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

        public override void Compose()
        {
            int i = 0;

            for (int y = 0; y < SadConsoleEditor.Settings.Config.ScreenFont.Rows; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    this.SetGlyph(x, y, i);
                    this.SetMirror(x, y, _mirrorEffect);
                    i++;
                }
            }

            OnComposed?.Invoke(this);
        }

        protected override void OnMouseIn(SadConsole.Input.MouseConsoleState info)
        {
            var mousePosition = TransformConsolePositionByControlPosition(info.CellPosition);

            if (new Rectangle(0, 0, 16, SadConsoleEditor.Settings.Config.ScreenFont.Rows).Contains(mousePosition) && info.Mouse.LeftButtonDown)
            {
                if (!UseFullClick)
                    SelectedCharacter = this[mousePosition.ToIndex(16)].Glyph;
            }

            base.OnMouseIn(info);
        }

        protected override void OnLeftMouseClicked(SadConsole.Input.MouseConsoleState info)
        {
            var mousePosition = TransformConsolePositionByControlPosition(info.CellPosition);

            if (new Rectangle(0, 0, 16, SadConsoleEditor.Settings.Config.ScreenFont.Rows).Contains(mousePosition))
            {
                SelectedCharacter = this[mousePosition.ToIndex(16)].Glyph;
            }
            
            base.OnLeftMouseClicked(info);
        }

        public override void DetermineAppearance()
        {
            
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
