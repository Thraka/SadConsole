using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SadConsole.Console;
using SadConsole.Input;

namespace SadConsoleEditor.Consoles
{
	class QuickSelectPane : Console
	{
		private Color charForeground;
		private Color charBackground;
		private AsciiKey[] keys;
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

		public QuickSelectPane() : base(Settings.Config.WindowWidth - Settings.Config.ToolPaneWidth + 1, 3)
		{
            textSurface.Font = Settings.Config.ScreenFont;
            textSurface.DefaultBackground = Settings.Color_MenuBack;
            textSurface.DefaultForeground = Settings.Color_TitleText;

            currentCharSet = 0;
			
			keys = new AsciiKey[] { AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F1), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F2), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F3),
									AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F4), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F5), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F6),
									AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F7), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F8), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F9),
									AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F10), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F11), AsciiKey.Get(Microsoft.Xna.Framework.Input.Keys.F12) };

            if (System.IO.File.Exists("quickselect.json"))
            {
                CharacterSets = SadConsole.Serializer.Load<int[][]>("quickselect.json");
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
            Position = new Point(0, SadConsole.Global.WindowHeight - TextSurface.AbsoluteArea.Height);
        }

        public void Redraw()
		{
			Clear();

			int x = 2;
			int y = 1;

			for (int i = 1; i < 13; i++)
			{
				string text = "F" + i.ToString() + " ";
				Print(x, y, text);
                x += text.Length;
                SetGlyph(x, y, Characters[i - 1]);
                SetForeground(x, y, charForeground);
                SetBackground(x, y, charBackground);
				x += 2;
            }
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			bool shifted = info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);

			if (shifted && info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Down))
			{
				currentCharSet++;

				if (currentCharSet >= CharacterSets.Length)
					currentCharSet = 0;

				Characters = CharacterSets[currentCharSet];
				Redraw();
				return true;
			}
			else if (shifted && info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Up))
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
					if (key == keys[i])
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
