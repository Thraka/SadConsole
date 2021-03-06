using SadRogue.Primitives;
using System;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Displays the glyphs associated with a font and allows the user to select one.
    /// </summary>
    public class CharacterPicker : ControlBase
    {
        Mirror _mirrorSetting;

        private DrawingArea _characterSurface;
        private Effects.Fade _selectedCharEffect;
        private int _selectedChar;

        /// <summary>
        /// Raised when the <see cref="SelectedCharacter"/> property changes.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<int>> SelectedCharacterChanged;

        /// <summary>
        /// When <see langword="true"/>, indicates that the control should use a mouse click to select a new character; otherwise <see langword="false"/> to indicate that just having the mouse down will select a new character.
        /// </summary>
        public bool UseFullClick { get; set; }

        /// <summary>
        /// Gets the position of the previously selected character. Used by the theme.
        /// </summary>
        public Point OldCharacterLocation { get; private set; } = new Point(-1, -1);

        /// <summary>
        /// Gets the position of the newly selected character. Used by the theme.
        /// </summary>
        public Point NewCharacterLocation { get; private set; } = new Point(-1, -1);

        /// <summary>
        /// Gets or sets the mirror effect to apply in the theme.
        /// </summary>
        public Mirror MirrorSetting
        {
            get { return _mirrorSetting; }
            set
            {
                _mirrorSetting = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Gets the foreground color used when drawing the glyphs.
        /// </summary>
        public Color GlyphForeground { get; private set; }

        /// <summary>
        /// Gets the background color used when drawing the glyphs.
        /// </summary>
        public Color GlyphBackground { get; private set; }

        /// <summary>
        /// Gets the foreground color used when drawing the selected glyph.
        /// </summary>
        public Color SelectedGlyphForeground { get; private set; }

        /// <summary>
        /// Gets the effect to apply when drawing the selected glyph.
        /// </summary>
        public Effects.ICellEffect SelectedGlyphEffect => _selectedCharEffect;

        /// <summary>
        /// Gets or sets the selected glyph character.
        /// </summary>
        public int SelectedCharacter
        {
            get { return _selectedChar; }
            set
            {
                if (_selectedChar == value) return;

                var old = _selectedChar;
                _selectedChar = value;

                OldCharacterLocation = Point.FromIndex(old, 16);
                NewCharacterLocation = Point.FromIndex(value, 16);

                SelectedCharacterChanged?.Invoke(this, new ValueChangedEventArgs<int>(old, value));

                IsDirty = true;
            }
        }

        /// <summary>
        /// Creates a new picker control with the specified font.
        /// </summary>
        /// <param name="foreground">The default foreground for glyphs.</param>
        /// <param name="fill">The default backround for glyphs.</param>
        /// <param name="selectedCharacterColor">The foreground for the selected glyph.</param>
        /// <param name="characterFont">The font to use with the control.</param>
        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, IFont characterFont)
            : this(foreground, fill, selectedCharacterColor)
        {
            //_characterSurface.AlternateFont = characterFont;
            AlternateFont = characterFont;
        }

        /// <summary>
        /// Creates a new picker control that uses the parent's font.
        /// </summary>
        /// <param name="foreground">The default foreground for glyphs.</param>
        /// <param name="fill">The default backround for glyphs.</param>
        /// <param name="selectedCharacterColor">The foreground for the selected glyph.</param>
        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor) : base(16, 16)
        {
            UseMouse = true;

            SelectedGlyphForeground = selectedCharacterColor;
            GlyphForeground = foreground;
            GlyphBackground = fill;

            //_characterSurface = new SadConsole.UI.Controls.DrawingSurface(16, 16);
            //_characterSurface.DefaultBackground = fill;
            //_characterSurface.DefaultForeground = foreground;
            //_characterSurface.Clear();

            _selectedCharEffect = new SadConsole.Effects.Fade()
            {
                FadeBackground = true,
                UseCellBackground = false,
                DestinationBackground = new ColorGradient(GlyphBackground, SelectedGlyphForeground * 0.8f),
                FadeDuration = 2d,
                CloneOnAdd = false,
                AutoReverse = true,
                Repeat = true,
            };

            SelectedCharacter = 1;
        }

        /// <inheritdoc/>
        protected override void OnMouseIn(ControlMouseState info)
        {
            if (!MouseState_EnteredWithButtonDown && MouseArea.Contains(info.MousePosition) && info.OriginalMouseState.Mouse.LeftButtonDown)
            {
                if (!UseFullClick)
                    SelectedCharacter = Surface[info.MousePosition.ToIndex(Width)].Glyph;
            }

            base.OnMouseIn(info);
        }

        /// <inheritdoc/>
        protected override void OnLeftMouseClicked(ControlMouseState info)
        {
            if (MouseArea.Contains(info.MousePosition))
            {
                SelectedCharacter = Surface[info.MousePosition.ToIndex(Width)].Glyph;
            }

            base.OnLeftMouseClicked(info);
        }
    }
}
