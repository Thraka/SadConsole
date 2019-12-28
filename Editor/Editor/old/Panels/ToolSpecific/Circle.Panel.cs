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
    class CircleToolPanel : CustomPanel
    {
        private DrawingSurface statusBox;
        private int circleWidth;
        private int circleHeight;

        public int CircleWidth { get {return circleWidth;} set{circleWidth = value; RedrawBox();} }
        public int CircleHeight { get { return circleHeight; } set { circleHeight = value; RedrawBox(); } }

        public CircleToolPanel()
        {
            Title = "Circle Status";

            statusBox = new DrawingSurface(SadConsoleEditor.Consoles.ToolPane.PanelWidthControls, 2);
            RedrawBox();
            Controls = new ControlBase[] { statusBox };
        }

        private void RedrawBox()
        {
            statusBox.Fill(Settings.Yellow, Color.Transparent, 0, null);
            
            var widthText = "Width: ".CreateColored(Settings.Yellow, Color.Transparent, null) + circleWidth.ToString().CreateColored(Settings.Blue, Color.Transparent, null);
            var heightText = "Height: ".CreateColored(Settings.Yellow, Color.Transparent, null) + circleHeight.ToString().CreateColored(Settings.Blue, Color.Transparent, null);

            statusBox.Print(0, 0, widthText);
            statusBox.Print(0, 1, heightText);
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
