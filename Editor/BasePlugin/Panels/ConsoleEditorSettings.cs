using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    public class ConsoleEditorSettings : CustomPanel
    {
        public event EventHandler ForegroundChanged
        {
            add { _foreColor.ColorChanged += value; }
            remove { _foreColor.ColorChanged -= value; }
        }

        public event EventHandler BackgroundChanged
        {
            add { _backColor.ColorChanged += value; }
            remove { _backColor.ColorChanged -= value; }
        }

        private Controls.ColorPresenter _foreColor;
        private Controls.ColorPresenter _backColor;

        public Color Foreground { get => _foreColor.SelectedColor; set => _foreColor.SelectedColor = value; }

        public Color Background { get => _backColor.SelectedColor; set => _backColor.SelectedColor = value; }


        public ConsoleEditorSettings(Color foreground, Color background)
        {
            _foreColor = new Controls.ColorPresenter("Foreground", foreground, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            _backColor = new Controls.ColorPresenter("Background", background, SadConsoleEditor.Consoles.ToolPane.PanelWidthControls);
            
            Title = "Console Defaults";

            Controls = new ControlBase[] { _foreColor, _backColor };
        }

        public override void ProcessMouse(SadConsole.Input.MouseScreenObjectState info)
        {
        }

        public override int Redraw(SadConsole.UI.Controls.ControlBase control)
        {
            return 0;
        }

        public override void Loaded()
        {
        }
    }
}
