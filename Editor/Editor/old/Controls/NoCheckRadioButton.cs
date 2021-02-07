using SadConsoleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Controls
{
    class NoCheckRadioButton : SadConsole.Controls.RadioButton
    {

        public NoCheckRadioButton(int width, int height) : base(width, height) { }

        public override SadConsole.Themes.RadioButtonTheme Theme
        {
            get
            {
                if (_theme == null)
                    return Settings.NoCheckRadioButtonTheme;
                else
                    return base._theme;
            }
            set
            {
                _theme = value;
            }
        }

        public override void Compose()
        {

            if (this.IsDirty)
            {
                if (Width != 1)
                {
                    this.Fill(_currentAppearanceText.Foreground, _currentAppearanceText.Background, 0, null);
                    this.Print(0, 0, Text.Align(TextAlignment, this.Width));
                }


                this.IsDirty = false;
            }
        }
    }
}
