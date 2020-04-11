using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Effects;

namespace StarterProject.Windows
{
    internal class CharacterViewer : Window
    {

        #region Control Declares
        private readonly SadConsole.Controls.ScrollBar _charScrollBar;
        private readonly Button _closeButton;
        #endregion

        private Point? _lastMousePos = null;
        private readonly SadConsole.Effects.Recolor highlightedEffect;
        private readonly SadConsole.Effects.Fade unhighlightEffect;
        private int fontRowOffset = 0;
        private ColoredString selectedGlyphString;
        private int hoverGlyph;
        private bool isHover;
        private DrawingSurface _characterSurface;
        private bool hasDrawn = false;

        public event EventHandler ColorsChanged;
        public int SelectedCharacterIndex = 0;

        public Color Foreground = Color.White;
        public Color Background = Color.Black;

        #region Constructors
        public CharacterViewer()
            : base(27, 20)
        {
            //DefaultShowLocation = StartupLocation.CenterScreen;
            //Fill(Color.White, Color.Black, 0, null);
            Title = (char)198 + "Character" + (char)198;
            TitleAlignment = HorizontalAlignment.Left;
            //SetTitle(" Characters ", HorizontalAlignment.Center, Color.Blue, Color.LightGray);
            CloseOnEscKey = true;
            UsePixelPositioning = true;

            // CHARACTER SCROLL
            _charScrollBar = new ScrollBar(Orientation.Vertical, 16)
            {
                Position = new Point(17, 1),
                Name = "ScrollBar",
                Maximum = Font.Rows - 16,
                Value = 0
            };
            _charScrollBar.ValueChanged += new EventHandler(_charScrollBar_ValueChanged);
            _charScrollBar.IsEnabled = Font.Rows > 16;

            // Add all controls
            Add(_charScrollBar);

            _closeButton = new Button(6, 1) { Text = "Ok", Position = new Point(19, 1) }; Add(_closeButton); _closeButton.Click += (sender, e) => { DialogResult = true; Hide(); };

            // Effects
            highlightedEffect = new Recolor
            {
                Foreground = Color.Blue,
                Background = Color.DarkGray
            };

            unhighlightEffect = new SadConsole.Effects.Fade()
            {
                FadeBackground = true,
                FadeForeground = true,
                DestinationForeground = new ColorGradient(highlightedEffect.Foreground, Color.White),
                DestinationBackground = new ColorGradient(highlightedEffect.Background, Color.Black),
                FadeDuration = 0.3d,
                RemoveOnFinished = true,
                UseCellBackground = false,
                UseCellForeground = false,
                CloneOnApply = true,
                Permanent = true
            };

            Theme = SadConsole.Themes.Library.Default.WindowTheme.Clone();
            Theme.BorderLineStyle = CellSurface.ConnectedLineThick;

            // The frame will have been drawn by the base class, so redraw and our close button will be put on top of it
            //Invalidate();


            // Get the existing colors object
            var colors = SadConsole.Themes.Colors.CreateAnsi();

            // Use a common background color
            var backgroundColor = Color.Black;

            // Assign the background color to both the hosts (console) and controls
            colors.ControlHostBack = backgroundColor;
            colors.ControlBack = backgroundColor;

            // Build other colors used by themes based on the background color
            colors.ControlBackLight = (backgroundColor * 1.3f).FillAlpha();
            colors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();

            // When a control is selected, a color indicates it. this is either some other color or a lighter color
            // the colors object also defines a palette of colors, we'll use this.
            colors.ControlBackSelected = colors.GrayDark;

            // Build the colors into theme objects for control appearance states
            colors.RebuildAppearances();

            _characterSurface = new DrawingSurface(16, 16);
            _characterSurface.Position = new Point(1, 1);
            _characterSurface.OnDraw = (ds) =>
            {
                ds.Surface.Effects.UpdateEffects(Global.GameTimeElapsedUpdate);

                if (hasDrawn) return;

                ds.Surface.Fill(Foreground, Background, 0);

                int charIndex = fontRowOffset * 16;
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        ds.Surface.SetGlyph(x, y, charIndex);
                        charIndex++;
                    }
                }
                hasDrawn = true;
            };
            _characterSurface.MouseMove += _characterSurface_MouseMove;
            _characterSurface.MouseButtonClicked += _characterSurface_MouseButtonClicked;
            _characterSurface.MouseExit += _characterSurface_MouseExit;
            Add(_characterSurface);

            ThemeColors = colors;
            IsDirty = true;
        }

        private void _characterSurface_MouseExit(object sender, SadConsole.Input.MouseEventArgs e)
        {
            var ds = (DrawingSurface)sender;

            isHover = false;

            // Clear the special effect on the last known character
            if (_lastMousePos != null)
            {
                ds.Surface.SetEffect(_lastMousePos.Value.X, _lastMousePos.Value.Y, unhighlightEffect);
                _lastMousePos = null;
            }

            DrawSelectedItemString();
        }

        private void _characterSurface_MouseButtonClicked(object sender, SadConsole.Input.MouseEventArgs e)
        {
            var state = e.MouseState;

            if (!state.Mouse.LeftClicked) return;

            var pos = e.MouseState.CellPosition - _characterSurface.Position;

            if (state.Cell != null)// && trackedRegion.Contains(state.ConsoleCellPosition.X, state.ConsoleCellPosition.Y))
            {
                SelectedCharacterIndex = _characterSurface.Surface[pos.X, pos.Y].Glyph;
            }
        }

        private void _characterSurface_MouseMove(object sender, SadConsole.Input.MouseEventArgs e)
        {
            var state = e.MouseState;
            var ds = (DrawingSurface)sender;
            var mousePos = state.CellPosition - _characterSurface.Position;

            if (state.Cell != null)// && trackedRegion.Contains(state.ConsoleCellPosition.X, state.ConsoleCellPosition.Y))
            {
                isHover = true;
                hoverGlyph = ds.Surface[mousePos.X, mousePos.Y].Glyph;

                // Set the special effect on the current known character and clear it on the last known
                if (_lastMousePos == null)
                {
                }
                else if (_lastMousePos != mousePos)
                {
                    ds.Surface.SetEffect(_lastMousePos.Value.X, _lastMousePos.Value.Y, unhighlightEffect);
                }

                ds.Surface.SetEffect(mousePos.X, mousePos.Y, highlightedEffect);
                _lastMousePos = mousePos;

                DrawHoverItemString();
            }

            ds.Surface.IsDirty = true;
        }

        #endregion


        private void _charScrollBar_ValueChanged(object sender, EventArgs e)
        {
            fontRowOffset = ((SadConsole.Controls.ScrollBar)sender).Value;
            hasDrawn = false;
        }

        protected override void Invalidate()
        {
            base.Invalidate();

            // Draw the border between char sheet area and the text area
            SetGlyph(0, Height - 3, 204);
            SetGlyph(Width - 1, Height - 3, 185);
            for (int i = 1; i < Width - 1; i++)
            {
                SetGlyph(i, Height - 3, 205);
            }
            //SetCharacter(this.Width - 1, 0, 256);

            if (isHover)
                DrawHoverItemString();
            else
                DrawSelectedItemString();
        }

        private void DrawHoverItemString()
        {
            // Draw the character index and value in the status area
            string[] items = new string[] { "Index: ", hoverGlyph.ToString() + " ", ((char)hoverGlyph).ToString() };

            items[2] = items[2].PadRight(Width - 2 - (items[0].Length + items[1].Length));

            ColoredString text = items[0].CreateColored(Color.LightBlue, Theme.BorderStyle.Background, null) +
                       items[1].CreateColored(Color.LightCoral, Color.Black, null) +
                       items[2].CreateColored(Color.LightCyan, Color.Black, null);

            text.IgnoreBackground = true;
            text.IgnoreEffect = true;

            Print(1, Height - 2, text);
        }

        private void DrawSelectedItemString()
        {
            // Clear the information area and redraw
            Print(1, Height - 2, "".PadRight(Width - 2));

            //string[] items = new string[] { "Current Index:", SelectedCharacterIndex.ToString() + " ", ((char)SelectedCharacterIndex).ToString() };
            string[] items = new string[] { "Selected: ", ((char)SelectedCharacterIndex).ToString(), " (", SelectedCharacterIndex.ToString(), ")" };
            items[4] = items[4].PadRight(Width - 2 - (items[0].Length + items[1].Length + items[2].Length + items[3].Length));

            ColoredString text = items[0].CreateColored(Color.LightBlue, Theme.BorderStyle.Background, null) +
                       items[1].CreateColored(Color.LightCoral, Theme.BorderStyle.Background, null) +
                       items[2].CreateColored(Color.LightCyan, Theme.BorderStyle.Background, null) +
                       items[3].CreateColored(Color.LightCoral, Theme.BorderStyle.Background, null) +
                       items[4].CreateColored(Color.LightCyan, Theme.BorderStyle.Background, null);

            text.IgnoreBackground = true;
            text.IgnoreEffect = true;

            Print(1, Height - 2, text);
        }


        //public void UpdateCharSheetColors()
        //{
        //    for (int y = 1; y < 17; y++)
        //    {
        //        for (int x = 1; x < 17; x++)
        //        {
        //            SetForeground(x, y, Foreground);
        //            SetBackground(x, y, Background);
        //        }
        //    }

        //    ColorsChanged?.Invoke(this, EventArgs.Empty);
        //}

        public override void Show(bool modal)
        {
            if (IsVisible)
            {
                return;
            }

            Center();

            base.Show(modal);


            //UpdateCharSheetColors();

            string[] items = new string[] { "Current Index:", SelectedCharacterIndex.ToString() + " ", ((char)SelectedCharacterIndex).ToString() };

            items[2] = items[2].PadRight(Width - 2 - (items[0].Length + items[1].Length));

            ColoredString text = items[0].CreateColored(Color.LightBlue, Color.Black, null) +
                       items[1].CreateColored(Color.LightCoral, Color.Black, null) +
                       items[2].CreateColored(Color.LightCyan, Color.Black, null);

            text.IgnoreBackground = true;
            text.IgnoreEffect = true;

            Print(1, Height - 2, text);
        }

    }
}
