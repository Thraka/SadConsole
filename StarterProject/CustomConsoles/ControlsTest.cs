using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class ControlsTest: ControlsConsole
    {
        public ControlsTest():base(80, 25)
        {
            Data.Print(1, 1, "CONTROL LIBRARY TEST");

            var button1 = new SadConsole.Controls.Button(10, 1);
            button1.Text = "Click";
            button1.Position = new Microsoft.Xna.Framework.Point(1, 3);
            Add(button1);
        }

        public override bool ProcessMouse(MouseInfo info)
        {
            return base.ProcessMouse(info);
        }
    }
}
