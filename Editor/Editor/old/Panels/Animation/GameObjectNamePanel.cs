using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;
using SadConsole.Entities;

namespace SadConsoleEditor.Panels
{
    class EntityNamePanel: CustomPanel
    {
        private Button setName;
        private DrawingSurface nameTitle;
        private Entity entity;


        public EntityNamePanel()
        {
            Title = "Game Object";

            nameTitle = new DrawingSurface(Consoles.ToolPane.PanelWidth - 3, 2);

            setName = new Button(3);
            setName.ShowEnds = false;
            setName.Text = "Set";

            setName.Click += (s, e) =>
            {
                Windows.RenamePopup rename = new Windows.RenamePopup(entity.Name);
                rename.Closed += (s2, e2) => { if (rename.DialogResult) entity.Name = rename.NewName; PrintName(); };
                rename.Center();
                rename.Show(true);
            };

            Controls = new ControlBase[] { setName, nameTitle };
        }

        private void PrintName()
        {
            nameTitle.Clear();
            nameTitle.Print(0, 0, "Name", Settings.Green);
            nameTitle.Print(0, 1, entity.Name, Settings.Blue);
        }

        public override void Loaded()
        {
        }

        public override void ProcessMouse(MouseConsoleState info)
        {
        }

        public override int Redraw(ControlBase control)
        {
            if (control == setName)
            {
                control.Position = new Microsoft.Xna.Framework.Point(Consoles.ToolPane.PanelWidth - setName.Width - 1, control.Position.Y);
                return -1;
            }
            return 0;
        }

        public void SetEntity(Entity entity)
        {
            this.entity = entity;
            PrintName();
            setName.IsEnabled = entity != null;
        }
    }
}
