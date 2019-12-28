using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Panels
{
    class LineToolPanel : CustomPanel
    {
        private DrawingSurface statusBox;
        private int lineLength;

        public int LineLength { get { return lineLength; } set { lineLength = value; RedrawBox(); } }
        

        public LineToolPanel()
        {
            Title = "Line Status";

            statusBox = new DrawingSurface(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 2);
            RedrawBox();
            Controls = new ControlBase[] { statusBox };
        }

        private void RedrawBox()
        {
            statusBox.Fill(Settings.Yellow, Color.Transparent, 0, null);

            var widthText = "Length: ".CreateColored(Settings.Yellow, Color.Transparent, null) + lineLength.ToString().CreateColored(Settings.Blue, Color.Transparent, null);

            statusBox.Print(0, 0, widthText);
        }

        public override void ProcessMouse(SadConsole.Input.MouseConsoleState info)
        {

        }

        public override int Redraw(SadConsole.Controls.ControlBase control)
        {
            return 0;
        }

        public override void Loaded()
        {
        }


    }
}
