using SadRogue.Primitives;
using System;
using SadConsole.Input;
using Console = SadConsole.Console;
using SadConsole;
using Keyboard = SadConsole.Input.Keyboard;

namespace SadConsoleEditor.Consoles
{
	public class QuickSelectPane : Console
	{
		private Color charForeground;
		private Color charBackground;
		private Keys[] keys;
		private int currentCharSet;

		public int[] Characters;

		public int[][] CharacterSets;

		public Color CharacterForeground
		{
			get { return charForeground; }
			set { charForeground = value; Redraw(); }
		}

		public Color CharacterBackground
		{
			get { return charBackground; }
			set { charBackground = value; Redraw(); }
		}

		internal QuickSelectPane() : base(Config.Program.WindowWidth - Config.Program.ToolPaneWidth + 1, 3)
		{
            Font = Config.Program.ScreenFont;
            FontSize = Config.Program.ScreenFontSize;
            DefaultBackground = SadConsole.UI.Themes.Library.Default.Colors.ControlHostBackground;
            DefaultForeground = SadConsole.UI.Themes.Library.Default.Colors.Title;

            currentCharSet = 0;
			
			keys = new Keys[] { Keys.F1, Keys.F2, Keys.F3,
								Keys.F4, Keys.F5, Keys.F6,
								Keys.F7, Keys.F8, Keys.F9,
								Keys.F10, Keys.F11, Keys.F12 };

            if (System.IO.File.Exists("quickselect.json"))
            {
                CharacterSets = SadConsole.Serializer.Load<int[][]>("quickselect.json", false);
                Characters = CharacterSets[0];
            }
            else
            {
                Characters = new int[] { (char)176, (char)177, (char)178, (char)219, (char)223, (char)220, (char)221, (char)222, (char)254, (char)250, (char)249, (char)0 };
                CharacterSets = new int[3][];
                CharacterSets[0] = Characters;
                CharacterSets[1] = new int[] { (char)218, (char)191, (char)192, (char)217, (char)196, (char)179, (char)195, (char)180, (char)193, (char)194, (char)197, (char)0 };
                CharacterSets[2] = new int[] { (char)201, (char)187, (char)200, (char)188, (char)205, (char)186, (char)204, (char)185, (char)202, (char)203, (char)206, (char)0 };
            }

			UseMouse = false;
			UseKeyboard = false;

            UsePixelPositioning = true;
            Position = new Point(0, SadConsole.GameHost.Instance.WindowSize.Y - AbsoluteArea.Height);
        }

        public void Redraw()
		{
			this.Clear();

			int x = 2;
			int y = 1;

			for (int i = 1; i < 13; i++)
			{
				string text = "F" + i.ToString() + " ";
				this.Print(x, y, text);
                x += text.Length;
                this.SetGlyph(x, y, Characters[i - 1]);
                this.SetForeground(x, y, charForeground);
                this.SetBackground(x, y, charBackground);
				x += 2;
            }
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			bool shifted = info.IsKeyDown(Keys.LeftShift) || info.IsKeyDown(Keys.RightShift);

			if (shifted && info.IsKeyReleased(Keys.Down))
			{
				currentCharSet++;

				if (currentCharSet >= CharacterSets.Length)
					currentCharSet = 0;

				Characters = CharacterSets[currentCharSet];
				Redraw();
				return true;
			}
			else if (shifted && info.IsKeyReleased(Keys.Up))
			{
				currentCharSet--;

				if (currentCharSet < 0)
					currentCharSet = CharacterSets.Length - 1;

				Characters = CharacterSets[currentCharSet];
				Redraw();
				return true;
			}

			foreach (var key in info.KeysPressed)
			{
				for (int i = 0; i < keys.Length; i++)
				{
					if (key.Key == keys[i])
					{
						Panels.CharacterPickPanel.SharedInstance.SettingCharacter = Characters[i];
                        return true;
					}
				}
			}

			return false;
		}

		public void CommonCharacterPickerPanel_ChangedHandler(object sender, EventArgs e)
		{
            CharacterForeground = ((Panels.CharacterPickPanel)sender).SettingForeground;
            CharacterBackground = ((Panels.CharacterPickPanel)sender).SettingBackground;
        }
	}
}
