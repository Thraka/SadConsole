using SadRogue.Primitives;
using System;

namespace SadConsole.UI.Controls
{
    /// <summary>
    /// Displays the glyphs associated with a font and allows the user to select one.
    /// </summary>
    public class CharacterPicker : SurfaceViewer
    {
        Mirror _mirrorSetting;

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
                if (value < 0 || value >= SurfaceControl.Surface.Count) return;

                var old = _selectedChar;
                _selectedChar = value;

                OldCharacterLocation = Point.FromIndex(old, SurfaceControl.Surface.Width);
                NewCharacterLocation = Point.FromIndex(value, SurfaceControl.Surface.Width);

                SelectedCharacterChanged?.Invoke(this, new ValueChangedEventArgs<int>(old, value));

                SurfaceControl.Surface.SetEffect(OldCharacterLocation.X, OldCharacterLocation.Y, null);
                _selectedCharEffect.Restart();
                SurfaceControl.Surface.SetEffect(NewCharacterLocation.X, NewCharacterLocation.Y, _selectedCharEffect);
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
        /// <param name="visibleColumns">The number of columns to show. The control will be this wide plus 1.</param>
        /// <param name="visibleRows">The number of rows to show. The control will be this high plus 1.</param>
        public CharacterPicker(Color foreground, Color fill, Color selectedCharacterColor, SadFont characterFont, int visibleColumns, int visibleRows)
            : base(visibleColumns, visibleRows, new CellSurface(characterFont.Columns, characterFont.Rows) { })
        {
            //_characterSurface.AlternateFont = characterFont;
            AlternateFont = characterFont;
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
                FadeForeground = true,
                UseCellBackground = false,
                UseCellForeground = false,
                DestinationForeground = new Gradient(SelectedGlyphForeground, GlyphForeground),
                FadeDuration = System.TimeSpan.FromSeconds(0.5d),
                CloneOnAdd = false,
                AutoReverse = true,
                Repeat = true,
                RestoreCellOnRemoved = true
            };

            ScrollBarMode = ScrollBarModes.AsNeeded;
            SurfaceControl.Surface.DefaultBackground = fill;
            SurfaceControl.Surface.DefaultForeground = foreground;
            SurfaceControl.Theme = new EffectDrawingTheme();

            for (int i = 0; i < SurfaceControl.Surface.Count; i++)
                SurfaceControl.Surface[i].Glyph = i;
        }

        /// <inheritdoc/>
        protected override void OnMouseIn(ControlMouseState info)
        {
            // If the mouse is in the area of the child surface content, check for click
            if (!MouseState_EnteredWithButtonDown && info.IsMouseOver && info.OriginalMouseState.Mouse.LeftButtonDown)
            {
                if (SurfaceControl.MouseArea.WithPosition(SurfaceControl.Position).Contains(info.MousePosition) && !UseFullClick)
                    SelectedCharacter = SurfaceControl.Surface[info.MousePosition.Add(SurfaceControl.Surface.ViewPosition).ToIndex(SurfaceControl.Surface.Width)].Glyph;
            }

            base.OnMouseIn(info);
        }

        /// <inheritdoc/>
        protected override void OnLeftMouseClicked(ControlMouseState info)
        {
            if (info.IsMouseOver)
            {
                SelectedCharacter = SurfaceControl.Surface[info.MousePosition.Add(SurfaceControl.Surface.ViewPosition).ToIndex(SurfaceControl.Surface.Width)].Glyph;
            }

            base.OnLeftMouseClicked(info);
        }

        private class EffectDrawingTheme : Themes.DrawingAreaTheme
        {
            public override void Attached(ControlBase control)
            {
                if (control is DrawingArea drawingControl)
                    drawingControl.Surface.Clear();

                else
                    throw new InvalidCastException();
            }

            public override void UpdateAndDraw(ControlBase control, TimeSpan time)
            {
                control.Surface.Effects.UpdateEffects(time);
            }
        }
    }
}
