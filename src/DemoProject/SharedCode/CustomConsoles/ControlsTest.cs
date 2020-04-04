using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

namespace StarterProject.CustomConsoles
{
    internal class ControlsTest : ControlsConsole
    {
        private readonly Color[] backgroundcycle;
        private int backIndex = 0;
        private readonly SadConsole.Timer progressTimer;

        public ControlsTest() : base(80, 23)
        {
            //ThemeColors = SadConsole.Themes.Colors.CreateFromAnsi();

            var prog1 = new ProgressBar(10, 1, HorizontalAlignment.Left)
            {
                Position = new Point(16, 5)
            };
            Add(prog1);

            var prog2 = new ProgressBar(1, 6, VerticalAlignment.Bottom)
            {
                Position = new Point(18, 7)
            };
            Add(prog2);

            var slider = new ScrollBar(Orientation.Horizontal, 10)
            {
                Position = new Point(16, 3),
                Maximum = 18
            };
            Add(slider);

            slider = new ScrollBar(Orientation.Vertical, 6)
            {
                Position = new Point(16, 7),
                Maximum = 6
            };
            Add(slider);

            progressTimer = new Timer(TimeSpan.FromSeconds(0.5));
            progressTimer.TimerElapsed += (timer, e) => { prog1.Progress = prog1.Progress >= 1f ? 0f : prog1.Progress + 0.1f; prog2.Progress = prog2.Progress >= 1f ? 0f : prog2.Progress + 0.1f; };

            Components.Add(progressTimer);

            var listbox = new SadConsole.Controls.ListBox(20, 6)
            {
                Position = new Point(28, 3)
            };
            listbox.Items.Add("item 1");
            listbox.Items.Add("item 2");
            listbox.Items.Add("item 3");
            listbox.Items.Add("item 4");
            listbox.Items.Add("item 5");
            listbox.Items.Add("item 6");
            listbox.Items.Add("item 7");
            listbox.Items.Add("item 8");
            Add(listbox);

            var radioButton = new RadioButton(20, 1)
            {
                Text = "Group 1 Option 1",
                Position = new Point(28, 12)
            };
            Add(radioButton);

            radioButton = new RadioButton(20, 1)
            {
                Text = "Group 1 Option 2",
                Position = new Point(28, 13)
            };
            Add(radioButton);

            var selButton = new SadConsole.Controls.SelectionButton(24, 1)
            {
                Text = "Selection Button 1",
                Position = new Point(51, 3)
            };
            Add(selButton);

            var selButton1 = new SadConsole.Controls.SelectionButton(24, 1)
            {
                Text = "Selection Button 2",
                Position = new Point(51, 4)
            };
            Add(selButton1);

            var selButton2 = new SadConsole.Controls.SelectionButton(24, 1)
            {
                Text = "Selection Button 3",
                Position = new Point(51, 5)
            };
            Add(selButton2);

            selButton.PreviousSelection = selButton2;
            selButton.NextSelection = selButton1;
            selButton1.PreviousSelection = selButton;
            selButton1.NextSelection = selButton2;
            selButton2.PreviousSelection = selButton1;
            selButton2.NextSelection = selButton;

            var input = new TextBox(10)
            {
                Position = new Point(51, 9)
            };
            Add(input);

            var password = new TextBox(10)
            {
                PasswordChar = "*",
                Position = new Point(65, 9)
            };
            Add(password);

            var button = new SadConsole.Controls.Button(11, 1)
            {
                Text = "Click",
                Position = new Point(1, 3)
            };
            button.Click += (s, a) => Window.Message("This has been clicked -- and your password field contains '" + password.Text + "'", "Close");
            Add(button);

            button = new SadConsole.Controls.Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 5),
                Theme = new Button3dTheme()
            };
            //button.AlternateFont = SadConsole.Global.LoadFont("Fonts/Cheepicus12.font").GetFont(Font.FontSizes.One);
            Add(button);

            button = new SadConsole.Controls.Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 10),
                Theme = new ButtonLinesTheme()
            };
            Add(button);

            var checkbox = new SadConsole.Controls.CheckBox(13, 1)
            {
                Text = "Check box",
                Position = new Point(51, 13)
            };
            Add(checkbox);

            FocusedControl = null;
            //DisableControlFocusing = true;

            var colorValues = ThemeColors ?? Library.Default.Colors;

            List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>
            {
                new Tuple<Color, string>(colorValues.Red, "Red"),
                new Tuple<Color, string>(colorValues.RedDark, "DRed"),
                new Tuple<Color, string>(colorValues.Purple, "Prp"),
                new Tuple<Color, string>(colorValues.PurpleDark, "DPrp"),
                new Tuple<Color, string>(colorValues.Blue, "Blu"),
                new Tuple<Color, string>(colorValues.BlueDark, "DBlu"),
                new Tuple<Color, string>(colorValues.Cyan, "Cya"),
                new Tuple<Color, string>(colorValues.CyanDark, "DCya"),
                new Tuple<Color, string>(colorValues.Green, "Gre"),
                new Tuple<Color, string>(colorValues.GreenDark, "DGre"),
                new Tuple<Color, string>(colorValues.Yellow, "Yel"),
                new Tuple<Color, string>(colorValues.YellowDark, "DYel"),
                new Tuple<Color, string>(colorValues.Orange, "Ora"),
                new Tuple<Color, string>(colorValues.OrangeDark, "DOra"),
                new Tuple<Color, string>(colorValues.Brown, "Bro"),
                new Tuple<Color, string>(colorValues.BrownDark, "DBrow"),
                new Tuple<Color, string>(colorValues.Gray, "Gray"),
                new Tuple<Color, string>(colorValues.GrayDark, "DGray"),
                new Tuple<Color, string>(colorValues.White, "White"),
                new Tuple<Color, string>(colorValues.Black, "Black")
            };

            backgroundcycle = colors.Select(i => i.Item1).ToArray();
            backIndex = 5;

            // Ensure our color changes take affect.
            Invalidate();

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
                {
                    backIndex = 0;
                }

                var colors = (ThemeColors ?? Library.Default.Colors);
                colors.ControlBack = backgroundcycle[backIndex];
                colors.RebuildAppearances();
                //ThemeColors = colors;
                IsDirty = true;
            }


            return base.ProcessKeyboard(info);
        }

        public override bool ProcessMouse(SadConsole.Input.MouseConsoleState state) => base.ProcessMouse(state);

        public override void Update(TimeSpan time) => base.Update(time);

        protected override void Invalidate()
        {
            base.Invalidate();

            var colorValues = ThemeColors ?? Library.Default.Colors;

            Print(1, 1, "BUTTONS", colorValues.ControlHostFore);
            Print(16, 1, "BARS", colorValues.ControlHostFore);
            Print(28, 1, "LISTBOX", colorValues.ControlHostFore);
            Print(28, 10, "RADIO BUTTON", colorValues.ControlHostFore);

            Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", colorValues.ControlHostFore);
            Print(51, 7, "TEXTBOX", colorValues.ControlHostFore);
            Print(65, 7, "(WITH MASK)", colorValues.ControlHostFore);

            Print(51, 11, "CHECKBOX", colorValues.ControlHostFore);

            Print(2, 15, "RED ".CreateColored(colorValues.Red, null) +
                                      "PURPLE ".CreateColored(colorValues.Purple, null) +
                                      "BLUE ".CreateColored(colorValues.Blue, null) +
                                      "CYAN ".CreateColored(colorValues.Cyan, null) +
                                      "GREEN ".CreateColored(colorValues.Green, null) +
                                      "YELLOW ".CreateColored(colorValues.Yellow, null) +
                                      "ORANGE ".CreateColored(colorValues.Orange, null) +
                                      "BROWN ".CreateColored(colorValues.Brown, null) +
                                      "GRAY ".CreateColored(colorValues.Gray, null) +
                                      "WHITE ".CreateColored(colorValues.White, null)
                                      );

            Print(2, 16, "RED ".CreateColored(colorValues.RedDark, null) +
                                      "PURPLE ".CreateColored(colorValues.PurpleDark, null) +
                                      "BLUE ".CreateColored(colorValues.BlueDark, null) +
                                      "CYAN ".CreateColored(colorValues.CyanDark, null) +
                                      "GREEN ".CreateColored(colorValues.GreenDark, null) +
                                      "YELLOW ".CreateColored(colorValues.YellowDark, null) +
                                      "ORANGE ".CreateColored(colorValues.OrangeDark, null) +
                                      "BROWN ".CreateColored(colorValues.BrownDark, null) +
                                      "GRAY ".CreateColored(colorValues.GrayDark, null) +
                                      "BLACK ".CreateColored(colorValues.Black, null)
                                      );
            Print(2, 18, CreateGradientExample("RED", colorValues.Red, colorValues.RedDark));
            Print(2, 19, CreateGradientExample("PURPLE", colorValues.Purple, colorValues.PurpleDark));
            Print(2, 20, CreateGradientExample("BLUE", colorValues.Blue, colorValues.BlueDark));
            Print(2, 21, CreateGradientExample("CYAN", colorValues.Cyan, colorValues.CyanDark));
            Print(2, 22, CreateGradientExample("GREEN", colorValues.Green, colorValues.GreenDark));
            Print(34, 18, CreateGradientExample("YELLOW", colorValues.Yellow, colorValues.YellowDark));
            Print(34, 19, CreateGradientExample("ORANGE", colorValues.Orange, colorValues.OrangeDark));
            Print(34, 20, CreateGradientExample("BROWN", colorValues.Brown, colorValues.BrownDark));
            Print(34, 21, CreateGradientExample("GRAY", colorValues.Gray, colorValues.GrayDark));
            Print(34, 22, CreateGradientExample("WHITE", colorValues.White, colorValues.Black));

            //Print(2, 23, CreateGradientExample("GOLD", colorValues.Gold, colorValues.GoldDark));
        }

        private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7) => text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
    }
}
