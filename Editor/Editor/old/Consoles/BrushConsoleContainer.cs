using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;
using SadConsole.Entities;

namespace SadConsoleEditor.Consoles
{
    //TODO: Simplify, kill this and just use a local consolecontainer in mainscreen.
    class BrushConsoleContainer: SadConsole.ConsoleContainer
    {
        private Entity brush;
        public Entity Brush
        {
            get { return brush; }
            set { brush = value; Children.Clear(); if (brush != null) Children.Add(brush); }
        }

        public BrushConsoleContainer()
        {
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            //return base.ProcessMouse(state);
            return false;
        }
    }
}
