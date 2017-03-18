using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Input;
using System.Linq;

namespace StarterProject.CustomConsoles
{
    class ControlsTest: ControlsConsole, IConsoleMetadata
    {
        Color[] backgroundcycle;
        int backIndex = 0;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "Controls Test", Summary = "Interact with SadConsole controls" };
            }
        }

        public ControlsTest():base(80, 23)
        {
            var button1 = new SadConsole.Controls.Button(11);
            button1.Text = "Click";
            button1.Position = new Point(1, 1);
            button1.Click += (s, e) => Window.Message("Clicked!", "OK");
            Add(button1);

            var radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 1";
            radioButton.Position = new Point(1, 3);
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 2";
            radioButton.Position = new Point(1, 4);
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 2 Option 1";
            radioButton.Position = new Point(1, 6);
            radioButton.GroupName = "group2";
            Add(radioButton);

            radioButton = new SadConsole.Controls.RadioButton(20, 1);
            radioButton.Text = "Group 2 Option 2";
            radioButton.Position = new Point(1, 7);
            radioButton.GroupName = "group2";
            Add(radioButton);

            var checkbox = new SadConsole.Controls.CheckBox(13, 1);
            checkbox.Text = "Check box";
            checkbox.Position = new Point(1, 9);
            Add(checkbox);

            var listbox = new SadConsole.Controls.ListBox(20, 6);
            listbox.Position = new Point(25, 1);
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
            slider.Position = new Point(25, 7);
            slider.Maximum = 18;
            Add(slider);

            slider = SadConsole.Controls.ScrollBar.Create(System.Windows.Controls.Orientation.Vertical, 8);
            slider.Position = new Point(47, 1);
            slider.Maximum = 6;
            Add(slider);

            var input = new SadConsole.Controls.InputBox(20);
            input.Position = new Point(25, 9);
            Add(input);

            var selButton = new SadConsole.Controls.SelectionButton(20);
            selButton.Text = "Selection Button 1";
            selButton.Position = new Point(55, 3);
            Add(selButton);

            var selButton1 = new SadConsole.Controls.SelectionButton(20);
            selButton1.Text = "Selection Button 2";
            selButton1.Position = new Point(55, 4);
            Add(selButton1);

            var selButton2 = new SadConsole.Controls.SelectionButton(20);
            selButton2.Text = "Selection Button 3";
            selButton2.Position = new Point(55, 5);
            Add(selButton2);

            var selButton3 = new SadConsole.Controls.SelectionButton(20);
            selButton3.Text = "Selection Button 4";
            selButton3.Position = new Point(55, 6);
            Add(selButton3);

            var selButton4 = new SadConsole.Controls.SelectionButton(20);
            selButton4.Text = "Selection Button 5";
            selButton4.Position = new Point(55, 7);
            Add(selButton4);

            selButton.PreviousSelection = selButton4;
            selButton.NextSelection = selButton1;
            selButton1.PreviousSelection = selButton;
            selButton1.NextSelection = selButton2;
            selButton2.PreviousSelection = selButton1;
            selButton2.NextSelection = selButton3;
            selButton3.PreviousSelection = selButton2;
            selButton3.NextSelection = selButton4;
            selButton4.PreviousSelection = selButton3;
            selButton4.NextSelection = selButton;

            FocusedControl = null;
            //DisableControlFocusing = true;
            
            List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>();
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Red, "Red"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.RedDark, "DRed"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Purple, "Prp"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.PurpleDark, "DPrp"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Blue, "Blu"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.BlueDark, "DBlu"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Cyan, "Cya"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.CyanDark, "DCya"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Green, "Gre"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.GreenDark, "DGre"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Yellow, "Yel"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.YellowDark, "DYel"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Orange, "Ora"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.OrangeDark, "DOra"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Brown, "Bro"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.BrownDark, "DBrow"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Gray, "Gray"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.GrayDark, "DGray"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.White, "White"));
            colors.Add(new Tuple<Color, string>(StarterProject.Theme.Black, "Black"));

            backgroundcycle = colors.Select(i => i.Item1).ToArray();
            backIndex = 5;


            //int y = 25 - 20;
            //int x = 0;
            //int colorLength = 4;
            //foreach (var color1 in colors)
            //{
            //    foreach (var color2 in colors)
            //    {
            //        _Print(x, y, new ColoredString(color2.Item2.PadRight(colorLength).Substring(0, colorLength), color2.Item1, color1.Item1, null));
            //        y++;
            //    }

            //    y = 25 -20;
            //    x += colorLength;
            //}


        }

        

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if (info.IsKeyReleased(Keys.C))
            {
                backIndex++;

                if (backIndex == backgroundcycle.Length)
                    backIndex = 0;

                var theme = Theme;
                theme.FillStyle.Background = backgroundcycle[backIndex];
                Theme = theme;
                
            }


            return base.ProcessKeyboard(info);
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState state)
        {
            return base.ProcessMouse(state);
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Print(50, 2, "CLICK BTN - USE UP/DOWN KEYS", StarterProject.Theme.YellowDark);

            Print(2, 15, "RED ".CreateColored(StarterProject.Theme.Red, null) +
                                      "PURPLE ".CreateColored(StarterProject.Theme.Purple, null) +
                                      "BLUE ".CreateColored(StarterProject.Theme.Blue, null) +
                                      "CYAN ".CreateColored(StarterProject.Theme.Cyan, null) +
                                      "GREEN ".CreateColored(StarterProject.Theme.Green, null) +
                                      "YELLOW ".CreateColored(StarterProject.Theme.Yellow, null) +
                                      "ORANGE ".CreateColored(StarterProject.Theme.Orange, null) +
                                      "BROWN ".CreateColored(StarterProject.Theme.Brown, null) +
                                      "GRAY ".CreateColored(StarterProject.Theme.Gray, null) +
                                      "WHITE ".CreateColored(StarterProject.Theme.White, null)
                                      );

            Print(2, 16, "RED ".CreateColored(StarterProject.Theme.RedDark, null) +
                                      "PURPLE ".CreateColored(StarterProject.Theme.PurpleDark, null) +
                                      "BLUE ".CreateColored(StarterProject.Theme.BlueDark, null) +
                                      "CYAN ".CreateColored(StarterProject.Theme.CyanDark, null) +
                                      "GREEN ".CreateColored(StarterProject.Theme.GreenDark, null) +
                                      "YELLOW ".CreateColored(StarterProject.Theme.YellowDark, null) +
                                      "ORANGE ".CreateColored(StarterProject.Theme.OrangeDark, null) +
                                      "BROWN ".CreateColored(StarterProject.Theme.BrownDark, null) +
                                      "GRAY ".CreateColored(StarterProject.Theme.GrayDark, null) +
                                      "BLACK ".CreateColored(StarterProject.Theme.Black, null)
                                      );
            Print(2, 18, CreateGradientExample("RED", StarterProject.Theme.Red, StarterProject.Theme.RedDark));
            Print(2, 19, CreateGradientExample("PURPLE", StarterProject.Theme.Purple, StarterProject.Theme.PurpleDark));
            Print(2, 20, CreateGradientExample("BLUE", StarterProject.Theme.Blue, StarterProject.Theme.BlueDark));
            Print(2, 21, CreateGradientExample("CYAN", StarterProject.Theme.Cyan, StarterProject.Theme.CyanDark));
            Print(2, 22, CreateGradientExample("GREEN", StarterProject.Theme.Green, StarterProject.Theme.GreenDark));
            Print(34, 18, CreateGradientExample("YELLOW", StarterProject.Theme.Yellow, StarterProject.Theme.YellowDark));
            Print(34, 19, CreateGradientExample("ORANGE", StarterProject.Theme.Orange, StarterProject.Theme.OrangeDark));
            Print(34, 20, CreateGradientExample("BROWN", StarterProject.Theme.Brown, StarterProject.Theme.BrownDark));
            Print(34, 21, CreateGradientExample("GRAY", StarterProject.Theme.Gray, StarterProject.Theme.GrayDark));
            Print(34, 22, CreateGradientExample("WHITE", StarterProject.Theme.White, StarterProject.Theme.Black));

            //Print(2, 23, CreateGradientExample("GOLD", StarterProject.Theme.Gold, StarterProject.Theme.GoldDark));
        }

        private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7)
        {
            return text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
        }
    }
}
