using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.UI;

namespace SadConsole.Examples;

internal class CharacterSelectWindow : UI.Window
{
    public CharacterSelectWindow(int width, int height) : base(width, height)
    {
        Colors colors = Controls.GetThemeColors();

        UI.Controls.CharacterPicker picker = new(Color.Gray, colors.ControlHostBackground, colors.ControlForegroundSelected, (SadFont)Font, width - 2, height - 2);
        picker.Position = (1, 1);
        Controls.Add(picker);

        Center();
        CloseOnEscKey = true;
    }
}
