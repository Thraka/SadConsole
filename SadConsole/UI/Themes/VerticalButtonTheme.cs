using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
	/// <summary>
	/// The theme of the button control
	/// </summary>
	[DataContract]
	public class VerticalButtonTheme : ButtonTheme
	{
		// 220: ▄
		// 223: ▀
		public VerticalButtonTheme(int topEndGlyph = 30, int bottomEndGlyph = 31)
			: base(topEndGlyph, bottomEndGlyph) { }

		public override void UpdateAndDraw(ControlBase control, TimeSpan time)
		{
			if ((control is Button button) == false) return;
			if (button.IsDirty == false) return;

			RefreshTheme(control.FindThemeColors(), control);
			ColoredGlyph appearance = ControlThemeState.GetStateAppearance(control.State);
			ColoredGlyph endGlyphAppearance = EndsThemeState.GetStateAppearance(control.State);

			button.Surface.Fill(
				appearance.Foreground,
				appearance.Background,
				appearance.Glyph, null);

			int center = button.Width != 1 ? button.Width / 2 : 0;
			int y = 0;

			if (button.TextAlignment == HorizontalAlignment.Center)
			{
				y = (button.Height - button.Text.Length) / 2;
			}
			else if (button.TextAlignment == HorizontalAlignment.Right)
			{
				y = button.Height - button.Text.Length;
				if (ShowEnds) y--;
			}
			else if (ShowEnds && button.TextAlignment == HorizontalAlignment.Right)
			{
				y++;
			}

			if (ShowEnds && button.Height >= 3)
			{
				var lines = control.State == ControlStates.Disabled ? appearance.Foreground : _colorsLastUsed.Lines;

				button.Surface.SetCellAppearance(center, 0, endGlyphAppearance);
				button.Surface[center, 0].Glyph = LeftEndGlyph;

				button.Surface.SetCellAppearance(center, button.Height - 1, endGlyphAppearance);
				button.Surface[center, button.Height - 1].Glyph = RightEndGlyph;
			}

			foreach (char letter in button.Text)
			{
				button.Surface.SetGlyph(center, y, letter);
				y++;
			}

			button.IsDirty = false;
		}
	}
}
