using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class ControlsTest : SadConsole.UI.ControlsConsole
    {
        private readonly Color[] backgroundcycle;
        private int backIndex = 0;
        private readonly SadConsole.Timer progressTimer;

        public ControlsTest() : base(80, 23)
        {
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

            SadComponents.Add(progressTimer);

            var listbox = new ListBox(20, 6)
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

            var selButton = new SelectionButton(24, 1)
            {
                Text = "Selection Button 1",
                Position = new Point(51, 3)
            };
            Add(selButton);

            var selButton1 = new SelectionButton(24, 1)
            {
                Text = "Selection Button 2",
                Position = new Point(51, 4)
            };
            Add(selButton1);

            var selButton2 = new SelectionButton(24, 1)
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

            var button = new Button(11, 1)
            {
                Text = "Click",
                Position = new Point(1, 3)
            };
            button.Click += (s, a) => SadConsole.UI.Window.Message("This has been clicked -- and your password field contains '" + password.Text + "'", "Close");
            Add(button);

            button = new Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 5),
                Theme = new Button3dTheme()
            };
            //button.AlternateFont = SadConsole.Global.LoadFont("Fonts/Cheepicus12.font").GetFont(Font.FontSizes.One);
            Add(button);

            button = new Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 10),
                Theme = new ButtonLinesTheme()
            };
            Add(button);

            var checkbox = new CheckBox(13, 1)
            {
                Text = "Check box",
                Position = new Point(51, 13)
            };
            Add(checkbox);

            FocusedControl = null;
            //DisableControlFocusing = true;

            List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>
            {
                new Tuple<Color, string>(Library.Default.Colors.Red, "Red"),
                new Tuple<Color, string>(Library.Default.Colors.RedDark, "DRed"),
                new Tuple<Color, string>(Library.Default.Colors.Purple, "Prp"),
                new Tuple<Color, string>(Library.Default.Colors.PurpleDark, "DPrp"),
                new Tuple<Color, string>(Library.Default.Colors.Blue, "Blu"),
                new Tuple<Color, string>(Library.Default.Colors.BlueDark, "DBlu"),
                new Tuple<Color, string>(Library.Default.Colors.Cyan, "Cya"),
                new Tuple<Color, string>(Library.Default.Colors.CyanDark, "DCya"),
                new Tuple<Color, string>(Library.Default.Colors.Green, "Gre"),
                new Tuple<Color, string>(Library.Default.Colors.GreenDark, "DGre"),
                new Tuple<Color, string>(Library.Default.Colors.Yellow, "Yel"),
                new Tuple<Color, string>(Library.Default.Colors.YellowDark, "DYel"),
                new Tuple<Color, string>(Library.Default.Colors.Orange, "Ora"),
                new Tuple<Color, string>(Library.Default.Colors.OrangeDark, "DOra"),
                new Tuple<Color, string>(Library.Default.Colors.Brown, "Bro"),
                new Tuple<Color, string>(Library.Default.Colors.BrownDark, "DBrow"),
                new Tuple<Color, string>(Library.Default.Colors.Gray, "Gray"),
                new Tuple<Color, string>(Library.Default.Colors.GrayDark, "DGray"),
                new Tuple<Color, string>(Library.Default.Colors.White, "White"),
                new Tuple<Color, string>(Library.Default.Colors.Black, "Black")
            };

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

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState state) => base.ProcessMouse(state);

        protected override void OnThemeDrawn()
        {
            this.Print(1, 1, "BUTTONS", Library.Default.Colors.YellowDark);
            this.Print(16, 1, "BARS", Library.Default.Colors.YellowDark);
            this.Print(28, 1, "LISTBOX", Library.Default.Colors.YellowDark);
            this.Print(28, 10, "RADIO BUTTON", Library.Default.Colors.YellowDark);

            this.Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", Library.Default.Colors.YellowDark);
            this.Print(51, 7, "TEXTBOX", Library.Default.Colors.YellowDark);
            this.Print(65, 7, "(WITH MASK)", Library.Default.Colors.YellowDark);

            this.Print(51, 11, "CHECKBOX", Library.Default.Colors.YellowDark);

            this.Print(2, 15, "RED ".CreateColored(Library.Default.Colors.Red, null) +
                                      "PURPLE ".CreateColored(Library.Default.Colors.Purple, null) +
                                      "BLUE ".CreateColored(Library.Default.Colors.Blue, null) +
                                      "CYAN ".CreateColored(Library.Default.Colors.Cyan, null) +
                                      "GREEN ".CreateColored(Library.Default.Colors.Green, null) +
                                      "YELLOW ".CreateColored(Library.Default.Colors.Yellow, null) +
                                      "ORANGE ".CreateColored(Library.Default.Colors.Orange, null) +
                                      "BROWN ".CreateColored(Library.Default.Colors.Brown, null) +
                                      "GRAY ".CreateColored(Library.Default.Colors.Gray, null) +
                                      "WHITE ".CreateColored(Library.Default.Colors.White, null)
                                      );

            this.Print(2, 16, "RED ".CreateColored(Library.Default.Colors.RedDark, null) +
                                      "PURPLE ".CreateColored(Library.Default.Colors.PurpleDark, null) +
                                      "BLUE ".CreateColored(Library.Default.Colors.BlueDark, null) +
                                      "CYAN ".CreateColored(Library.Default.Colors.CyanDark, null) +
                                      "GREEN ".CreateColored(Library.Default.Colors.GreenDark, null) +
                                      "YELLOW ".CreateColored(Library.Default.Colors.YellowDark, null) +
                                      "ORANGE ".CreateColored(Library.Default.Colors.OrangeDark, null) +
                                      "BROWN ".CreateColored(Library.Default.Colors.BrownDark, null) +
                                      "GRAY ".CreateColored(Library.Default.Colors.GrayDark, null) +
                                      "BLACK ".CreateColored(Library.Default.Colors.Black, null)
                                      );
            this.Print(2, 18, CreateGradientExample("RED", Library.Default.Colors.Red, Library.Default.Colors.RedDark));
            this.Print(2, 19, CreateGradientExample("PURPLE", Library.Default.Colors.Purple, Library.Default.Colors.PurpleDark));
            this.Print(2, 20, CreateGradientExample("BLUE", Library.Default.Colors.Blue, Library.Default.Colors.BlueDark));
            this.Print(2, 21, CreateGradientExample("CYAN", Library.Default.Colors.Cyan, Library.Default.Colors.CyanDark));
            this.Print(2, 22, CreateGradientExample("GREEN", Library.Default.Colors.Green, Library.Default.Colors.GreenDark));
            this.Print(34, 18, CreateGradientExample("YELLOW", Library.Default.Colors.Yellow, Library.Default.Colors.YellowDark));
            this.Print(34, 19, CreateGradientExample("ORANGE", Library.Default.Colors.Orange, Library.Default.Colors.OrangeDark));
            this.Print(34, 20, CreateGradientExample("BROWN", Library.Default.Colors.Brown, Library.Default.Colors.BrownDark));
            this.Print(34, 21, CreateGradientExample("GRAY", Library.Default.Colors.Gray, Library.Default.Colors.GrayDark));
            this.Print(34, 22, CreateGradientExample("WHITE", Library.Default.Colors.White, Library.Default.Colors.Black));

            //Print(2, 23, CreateGradientExample("GOLD", Library.Default.Colors.Gold, Library.Default.Colors.GoldDark));
        }

        private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7) => text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
    }
}
