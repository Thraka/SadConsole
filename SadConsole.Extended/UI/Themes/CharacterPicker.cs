using System;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme used for the <see cref="Controls.CharacterPicker"/> control.
    /// </summary>
    public class CharacterPicker : ThemeBase
    {
        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            if (!(control is Controls.CharacterPicker)) throw new Exception($"Theme can only be added to a {nameof(CharacterPicker)}");

            control.Surface = new CellSurface(control.Width, control.Height);
            control.Surface.Clear();
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Controls.CharacterPicker picker)) return;

            if (!control.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);

            IFont font = control.AlternateFont ?? control.Parent.Host.ParentConsole.Font;

            // Sync font with control surface
            if (control.Surface.Width != font.Columns || control.Surface.Height != font.Rows)
            {
                control.Surface = new CellSurface(font.Columns, font.Rows);
                control.Surface.DefaultForeground = picker.GlyphForeground;
                control.Surface.DefaultBackground = picker.GlyphBackground;
                control.Surface.Fill();

                bool oldValue = control.EnableWidthHeightChange;
                control.EnableWidthHeightChange = true;
                control.Width = control.Surface.Width;
                control.Height = control.Surface.Height;
                control.EnableWidthHeightChange = oldValue;

                control.MouseArea = new Rectangle(0, 0, control.Width, control.Height);
            }

            if (picker.NewCharacterLocation != new Point(-1, -1))
                control.Surface.SetEffect(picker.OldCharacterLocation.X, picker.OldCharacterLocation.Y, null);

            control.Surface.Fill(picker.GlyphForeground, picker.GlyphBackground, 0, null);

            int i = 0;


            for (int y = 0; y < font.Rows; y++)
            {
                for (int x = 0; x < font.Columns; x++)
                {
                    control.Surface.SetGlyph(x, y, i);
                    control.Surface.SetMirror(x, y, picker.MirrorSetting);
                    i++;
                }
            }

            control.Surface.SetForeground(picker.NewCharacterLocation.X, picker.NewCharacterLocation.Y, picker.SelectedGlyphForeground);
            control.Surface.SetEffect(picker.NewCharacterLocation.X, picker.NewCharacterLocation.Y, picker.SelectedGlyphEffect);

            control.IsDirty = false;
        }

        /// <inheritdoc />
        public override ThemeBase Clone()
        {
            return new CharacterPicker()
            {
                ControlThemeState = ControlThemeState.Clone()
            };
        }
    }
}
