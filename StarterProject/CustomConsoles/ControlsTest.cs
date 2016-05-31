using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using SadConsole;
using System.Text;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class ControlsTest: ControlsConsole
    {
        public ControlsTest():base(80, 25)
        {
            IsVisible = false;
            Data.Print(1, 1, "CONTROL LIBRARY TEST");
            Data.Print(1, 2, "____________________", spriteEffect: Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically);
            
            var button1 = new SadConsole.Controls.Button(11, 1);
            button1.Text = "Click";
            button1.Position = new Microsoft.Xna.Framework.Point(1, 3);
            button1.ButtonClicked += (s, e) => Window.Message("Clicked!", "OK");
            Add(button1);

            var radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 1";
            radioButton.Position = new Microsoft.Xna.Framework.Point(1, 5);
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 2";
            radioButton.Position = new Microsoft.Xna.Framework.Point(1, 7);
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 2 Option 1";
            radioButton.Position = new Microsoft.Xna.Framework.Point(1, 9);
            radioButton.GroupName = "group2";
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 2 Option 2";
            radioButton.Position = new Microsoft.Xna.Framework.Point(1, 11);
            radioButton.GroupName = "group2";
            Add(radioButton);

            var checkbox = new SadConsole.Controls.CheckBox(13, 1);
            checkbox.Text = "Check box";
            checkbox.Position = new Microsoft.Xna.Framework.Point(1, 13);
            Add(checkbox);

            var listbox = new SadConsole.Controls.ListBox(20, 6);
            listbox.Position = new Microsoft.Xna.Framework.Point(25, 3);
            listbox.HideBorder = false;
            listbox.Items.Add("item 1");
            listbox.Items.Add("item 2");
            listbox.Items.Add("item 3");
            listbox.Items.Add("item 4");
            listbox.Items.Add("item 5");
            listbox.Items.Add("item 6");
            listbox.Items.Add("item 7");
            listbox.Items.Add("item 8");
            Add(listbox);

            var slider = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Horizontal, 20);
            slider.Position = new Microsoft.Xna.Framework.Point(25, 10);
            slider.Maximum = 18;
            Add(slider);

            slider = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, 8);
            slider.Position = new Microsoft.Xna.Framework.Point(47, 3);
            slider.Maximum = 6;
            Add(slider);

            var input = new SadConsole.Controls.InputBox(20);
            input.Position = new Microsoft.Xna.Framework.Point(25, 12);
            Add(input);

            FocusedControl = null;
            //DisableControlFocusing = true;

            _textSurface.Print(3, 23, "RED ".CreateColored(StarterProject.Theme.Red, null) +
                                      "BLUE ".CreateColored(StarterProject.Theme.Blue, null) +
                                      "GREEN ".CreateColored(StarterProject.Theme.Green, null) +
                                      "YELLOW ".CreateColored(StarterProject.Theme.Yellow, null) +
                                      "ORANGE ".CreateColored(StarterProject.Theme.Orange, null) + 
                                      "DRK GRAY ".CreateColored(StarterProject.Theme.GrayDark, null) +
                                      "PURPLE ".CreateColored(StarterProject.Theme.Purple, null) +
                                      "CYAN ".CreateColored(StarterProject.Theme.Cyan, null) +
                                      "DRK CYAN ".CreateColored(StarterProject.Theme.CyanDark, null) +
                                      "WHITE ".CreateColored(StarterProject.Theme.White, null) +
                                      "BLACK ".CreateColored(StarterProject.Theme.Black, null)
                                      );
        }

        public override bool ProcessMouse(MouseInfo info)
        {
            return base.ProcessMouse(info);
        }
    }
}
