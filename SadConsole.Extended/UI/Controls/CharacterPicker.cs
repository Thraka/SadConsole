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

            // Remove the surface control created by the baseclass
            RemoveControl(SurfaceControl);

            // Create our version of the control
            SurfaceControl = new DrawingAreaEffects(Width, Height);
            SurfaceControl.Surface.DefaultBackground = fill;
            SurfaceControl.Surface.DefaultForeground = foreground;
            AddControl(SurfaceControl);
            SetSurface(Surface);

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

        /// <inheritdoc/>
        protected override ICellSurface CreateControlSurface()
        {
            CellSurface surface = new CellSurface(Width, Height)
            {
                DefaultForeground = GlyphForeground,
                DefaultBackground = GlyphBackground
            };
            
            surface.Clear();
            return surface;
        }

        public override void UpdateAndRedraw(TimeSpan time)
        {
            if (!IsDirty) return;

            RefreshThemeStateColors(FindThemeColors());

            bool showWidthScroll = false;
            bool showHeightScroll = false;

            // Determine if any scroller is shown
            if (ScrollBarMode == SurfaceViewer.ScrollBarModes.Always)
            {
                showWidthScroll = true;
                showHeightScroll = true;
            }
            else if (ScrollBarMode == SurfaceViewer.ScrollBarModes.AsNeeded)
            {
                showWidthScroll = ChildSurface.Width > Width;
                showHeightScroll = ChildSurface.Height > Height;
            }

            // If only 1 scroller needs to be shown, but the opposite axis size == the available height,
            // it gets cut off so we need to show the other scroller.
            if (showWidthScroll && !showHeightScroll && ChildSurface.Height == Height)
                showHeightScroll = true;

            else if (showHeightScroll && !showWidthScroll && ChildSurface.Width == Width)
                showWidthScroll = true;

            // Show or hide the scrollers
            HorizontalScroller.IsVisible = showWidthScroll;
            VerticalScroller.IsVisible = showHeightScroll;

            // Based on which are shown, they may be different sizes
            // Resize and show them
            if (showWidthScroll && showHeightScroll)
            {
                // Account for the corner between them
                if (HorizontalScroller.Width != Width - 1)
                    HorizontalScroller.Resize(Width - 1, 1);

                if (VerticalScroller.Height != Height - 1)
                    VerticalScroller.Resize(1, Height - 1);

                Surface.ViewWidth = Width - 1;
                Surface.ViewHeight = Height - 1;
            }
            else if (showWidthScroll)
            {
                if (HorizontalScroller.Width != Width)
                    HorizontalScroller.Resize(Width, 1);

                Surface.ViewWidth = Width;
                Surface.ViewHeight = Height - 1;
            }
            else if (showHeightScroll)
            {
                if (VerticalScroller.Height != Height)
                    VerticalScroller.Resize(1, Height);

                Surface.ViewWidth = Width - 1;
                Surface.ViewHeight = Height;
            }

            // Ensure scroll bars positioned correctly
            VerticalScroller.Position = new Point(Width - 1, 0);
            HorizontalScroller.Position = new Point(0, Height - 1);
            HorizontalScroller.IsEnabled = false;
            VerticalScroller.IsEnabled = false;

            if (ChildSurface.ViewWidth != ChildSurface.Width)
            {
                HorizontalScroller.Maximum = ChildSurface.Width - ChildSurface.ViewWidth;
                HorizontalScroller.IsEnabled = true;
            }
            if (ChildSurface.ViewHeight != ChildSurface.Height)
            {
                VerticalScroller.Maximum = ChildSurface.Height - ChildSurface.ViewHeight;
                VerticalScroller.IsEnabled = true;
            }

            ChildSurface.IsDirty = false;
        }

        private class DrawingAreaEffects : DrawingArea
        {
            public DrawingAreaEffects(int width, int height) : base(width, height)
            {
            }

            public override void UpdateAndRedraw(TimeSpan time)
            {
                Surface.Effects.UpdateEffects(time);
            }
        }
    }
}
