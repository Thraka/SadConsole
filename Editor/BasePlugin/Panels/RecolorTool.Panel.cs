using SadConsole.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    public class RecolorToolPanel: CustomPanel
    {
        private CheckBox ignoreForeCheck;
        private CheckBox ignoreBackCheck;

        public bool IgnoreForeground => ignoreForeCheck.IsSelected;
        public bool IgnoreBackground => ignoreBackCheck.IsSelected;


        public RecolorToolPanel()
        {
            ignoreBackCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            ignoreBackCheck.Text = "Ignore Back";

            ignoreForeCheck = new CheckBox(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 1);
            ignoreForeCheck.Text = "Ignore Fore";

            Title = "Recolor Options";

            Controls = new ControlBase[] { ignoreForeCheck, ignoreBackCheck };
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
