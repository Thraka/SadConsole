using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Input;
using System.Linq;
using SadConsole.Controls;
using SadConsole.Themes;

namespace StarterProject.CustomConsoles
{
    class ControlsTest: ControlsConsole
    {
        Color[] backgroundcycle;
        int backIndex = 0;
        SadConsole.Timer progressTimer;
        
        public ControlsTest():base(80, 23)
        {
            var button = new SadConsole.Controls.Button(11, 1)
            {
                Text = "Click",
                Position = new Point(1, 3)
            };
            button.Click += (s, a) => Window.Message("This has been clicked!", "Close");
            Add(button);

            button = new SadConsole.Controls.Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 5),
                Theme = new Button3dTheme()
            };
            Add(button);

            button = new SadConsole.Controls.Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 10),
                Theme = new ButtonLinesTheme()
            };
            Add(button);
            
            var prog1 = new ProgressBar(10, 1, HorizontalAlignment.Left);
            prog1.Position = new Point(16, 5);
            Add(prog1);

            var prog2 = new ProgressBar(1, 6, VerticalAlignment.Bottom);
            prog2.Position = new Point(18, 7);
            Add(prog2);

            var slider = SadConsole.Controls.ScrollBar.Create(Orientation.Horizontal, 10);
            slider.Position = new Point(16, 3);
            slider.Maximum = 18;
            Add(slider);

            slider = SadConsole.Controls.ScrollBar.Create(Orientation.Vertical, 6);
            slider.Position = new Point(16, 7);
            slider.Maximum = 6;
            Add(slider);

            progressTimer = new Timer(0.5, (timer, time) => { prog1.Progress = prog1.Progress >= 1f ? 0f : prog1.Progress + 0.1f; prog2.Progress = prog2.Progress >= 1f ? 0f : prog2.Progress + 0.1f; });

            var listbox = new SadConsole.Controls.ListBox(20, 6);
            listbox.Position = new Point(28, 3);
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

            var radioButton = new RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 1";
            radioButton.Position = new Point(28, 12);
            Add(radioButton);

            radioButton = new RadioButton(20, 1);
            radioButton.Text = "Group 1 Option 2";
            radioButton.Position = new Point(28, 13);
            Add(radioButton);
            
            var selButton = new SadConsole.Controls.SelectionButton(24, 1);
            selButton.Text = "Selection Button 1";
            selButton.Position = new Point(51, 3);
            Add(selButton);

            var selButton1 = new SadConsole.Controls.SelectionButton(24, 1);
            selButton1.Text = "Selection Button 2";
            selButton1.Position = new Point(51, 4);
            Add(selButton1);

            var selButton2 = new SadConsole.Controls.SelectionButton(24, 1);
            selButton2.Text = "Selection Button 3";
            selButton2.Position = new Point(51, 5);
            Add(selButton2);

            selButton.PreviousSelection = selButton2;
            selButton.NextSelection = selButton1;
            selButton1.PreviousSelection = selButton;
            selButton1.NextSelection = selButton2;
            selButton2.PreviousSelection = selButton1;
            selButton2.NextSelection = selButton;

            var input = new TextBox(20);
            input.Position = new Point(51, 9);
            Add(input);

            var checkbox = new SadConsole.Controls.CheckBox(13, 1)
            {
                Text = "Check box",
                Position = new Point(51, 13)
            };
            Add(checkbox);

            FocusedControl = null;
            //DisableControlFocusing = true;
            
            List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>();
            colors.Add(new Tuple<Color, string>(Colors.Red, "Red"));
            colors.Add(new Tuple<Color, string>(Colors.RedDark, "DRed"));
            colors.Add(new Tuple<Color, string>(Colors.Purple, "Prp"));
            colors.Add(new Tuple<Color, string>(Colors.PurpleDark, "DPrp"));
            colors.Add(new Tuple<Color, string>(Colors.Blue, "Blu"));
            colors.Add(new Tuple<Color, string>(Colors.BlueDark, "DBlu"));
            colors.Add(new Tuple<Color, string>(Colors.Cyan, "Cya"));
            colors.Add(new Tuple<Color, string>(Colors.CyanDark, "DCya"));
            colors.Add(new Tuple<Color, string>(Colors.Green, "Gre"));
            colors.Add(new Tuple<Color, string>(Colors.GreenDark, "DGre"));
            colors.Add(new Tuple<Color, string>(Colors.Yellow, "Yel"));
            colors.Add(new Tuple<Color, string>(Colors.YellowDark, "DYel"));
            colors.Add(new Tuple<Color, string>(Colors.Orange, "Ora"));
            colors.Add(new Tuple<Color, string>(Colors.OrangeDark, "DOra"));
            colors.Add(new Tuple<Color, string>(Colors.Brown, "Bro"));
            colors.Add(new Tuple<Color, string>(Colors.BrownDark, "DBrow"));
            colors.Add(new Tuple<Color, string>(Colors.Gray, "Gray"));
            colors.Add(new Tuple<Color, string>(Colors.GrayDark, "DGray"));
            colors.Add(new Tuple<Color, string>(Colors.White, "White"));
            colors.Add(new Tuple<Color, string>(Colors.Black, "Black"));

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

        public override void Update(TimeSpan time)
        {
            progressTimer.Update(time.TotalSeconds);
            base.Update(time);
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Print(1, 1, "BUTTONS", Colors.YellowDark);
            Print(16, 1, "BARS", Colors.YellowDark);
            Print(28, 1, "LISTBOX", Colors.YellowDark);
            Print(28, 10, "RADIO BUTTON", Colors.YellowDark);

            Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", Colors.YellowDark);
            Print(51, 7, "TEXTBOX", Colors.YellowDark);
            Print(51, 11, "CHECKBOX", Colors.YellowDark);

            Print(2, 15, "RED ".CreateColored(Colors.Red, null) +
                                      "PURPLE ".CreateColored(Colors.Purple, null) +
                                      "BLUE ".CreateColored(Colors.Blue, null) +
                                      "CYAN ".CreateColored(Colors.Cyan, null) +
                                      "GREEN ".CreateColored(Colors.Green, null) +
                                      "YELLOW ".CreateColored(Colors.Yellow, null) +
                                      "ORANGE ".CreateColored(Colors.Orange, null) +
                                      "BROWN ".CreateColored(Colors.Brown, null) +
                                      "GRAY ".CreateColored(Colors.Gray, null) +
                                      "WHITE ".CreateColored(Colors.White, null)
                                      );

            Print(2, 16, "RED ".CreateColored(Colors.RedDark, null) +
                                      "PURPLE ".CreateColored(Colors.PurpleDark, null) +
                                      "BLUE ".CreateColored(Colors.BlueDark, null) +
                                      "CYAN ".CreateColored(Colors.CyanDark, null) +
                                      "GREEN ".CreateColored(Colors.GreenDark, null) +
                                      "YELLOW ".CreateColored(Colors.YellowDark, null) +
                                      "ORANGE ".CreateColored(Colors.OrangeDark, null) +
                                      "BROWN ".CreateColored(Colors.BrownDark, null) +
                                      "GRAY ".CreateColored(Colors.GrayDark, null) +
                                      "BLACK ".CreateColored(Colors.Black, null)
                                      );
            Print(2, 18, CreateGradientExample("RED", Colors.Red, Colors.RedDark));
            Print(2, 19, CreateGradientExample("PURPLE", Colors.Purple, Colors.PurpleDark));
            Print(2, 20, CreateGradientExample("BLUE", Colors.Blue, Colors.BlueDark));
            Print(2, 21, CreateGradientExample("CYAN", Colors.Cyan, Colors.CyanDark));
            Print(2, 22, CreateGradientExample("GREEN", Colors.Green, Colors.GreenDark));
            Print(34, 18, CreateGradientExample("YELLOW", Colors.Yellow, Colors.YellowDark));
            Print(34, 19, CreateGradientExample("ORANGE", Colors.Orange, Colors.OrangeDark));
            Print(34, 20, CreateGradientExample("BROWN", Colors.Brown, Colors.BrownDark));
            Print(34, 21, CreateGradientExample("GRAY", Colors.Gray, Colors.GrayDark));
            Print(34, 22, CreateGradientExample("WHITE", Colors.White, Colors.Black));

            //Print(2, 23, CreateGradientExample("GOLD", Colors.Gold, Colors.GoldDark));
        }

        private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7)
        {
            return text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
        }
    }
}
