using Microsoft.Xna.Framework;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    class BoxToolPanel : CustomPanel
    {
        private CheckBox fillBoxOption;
        private CheckBox useCharBorder;
        private Controls.ColorPresenter fillColor;
        private Controls.ColorPresenter lineForeColor;
        private Controls.ColorPresenter lineBackColor;
        private Controls.CharacterPicker characterPicker;

        public Color FillColor { get { return fillColor.SelectedColor; } }
        public Color LineForeColor { get { return lineForeColor.SelectedColor; } }
        public Color LineBackColor { get { return lineBackColor.SelectedColor; } }
        public bool UseFill { get { return fillBoxOption.IsSelected; } }
        public bool UseCharacterBorder { get { return useCharBorder.IsSelected; } }

        public int BorderCharacter { get { return characterPicker.SelectedCharacter; } }


        public BoxToolPanel()
        {
            Title = "Settings";

            fillBoxOption = new CheckBox(18, 1);
            fillBoxOption.Text = "Fill";

            useCharBorder = new CheckBox(18, 1);
            useCharBorder.Text = "Char. Border";
            useCharBorder.IsSelectedChanged += (s, o) => { characterPicker.IsVisible = useCharBorder.IsSelected; MainScreen.Instance.ToolsPane.RedrawPanels(); };

            lineForeColor = new Controls.ColorPresenter("Border Fore", Settings.Green, 18);
            lineForeColor.SelectedColor = Color.White;

            lineBackColor = new Controls.ColorPresenter("Border Back", Settings.Green, 18);
            lineBackColor.SelectedColor = Color.Black;

            fillColor = new Controls.ColorPresenter("Fill Color", Settings.Green, 18);
            fillColor.SelectedColor = Color.Black;

            characterPicker = new Controls.CharacterPicker(Settings.Red, Settings.Color_ControlBack, Settings.Green);
            characterPicker.IsVisible = false;

            Controls = new ControlBase[] { lineForeColor, lineBackColor, fillColor, fillBoxOption, useCharBorder, characterPicker };
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {
            
        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            if (control == characterPicker)
                characterPicker.Position = new Point(characterPicker.Position.X + 1, characterPicker.Position.Y);

            if (control == useCharBorder)
                return 1;

            if (control != fillColor)
                return 0;
            else
                return 1;
        }

        public override void Loaded()
        {
        }
    }
}
