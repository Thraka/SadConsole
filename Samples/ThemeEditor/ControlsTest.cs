using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace ThemeEditor
{
    internal class ControlsTest : SadConsole.UI.ControlsConsole
    {
        private readonly Color[] backgroundcycle;
        private readonly SadConsole.Timer progressTimer;

        public ControlsTest() : base(80, 23)
        {
            Controls.ThemeColors = Container.EditingColors;
            Position = new Point(10, 10);

            SadConsole.UI.Border.AddToSurface(this, "Example");

            var prog1 = new ProgressBar(10, 1, HorizontalAlignment.Left)
            {
                Position = new Point(16, 5),
                DisplayTextColor = Color.White
            };
            var theme = (ProgressBarTheme)Library.Default.GetControlTheme(typeof(ProgressBar));
            //theme.Background.SetGlyph(0);
            //theme.Foreground.SetGlyph('=');
            prog1.Theme = theme;

            Controls.Add(prog1);

            var prog2 = new ProgressBar(1, 6, VerticalAlignment.Bottom)
            {
                Position = new Point(18, 7),
                DisplayTextColor = Color.White
            };
            Controls.Add(prog2);

            var slider = new ScrollBar(Orientation.Horizontal, 10)
            {
                Position = new Point(16, 3),
                Maximum = 18
            };
            Controls.Add(slider);

            slider = new ScrollBar(Orientation.Vertical, 6)
            {
                Position = new Point(16, 7),
                Maximum = 6
            };
            Controls.Add(slider);

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
            Controls.Add(listbox);

            var radioButton = new RadioButton(20, 1)
            {
                Text = "Group 1 Option 1",
                Position = new Point(28, 12)
            };
            Controls.Add(radioButton);

            radioButton = new RadioButton(20, 1)
            {
                Text = "Group 1 Option 2",
                Position = new Point(28, 13)
            };
            Controls.Add(radioButton);

            var tmp = new ListBox(5, 11);
            tmp.Items.Add("1");
            tmp.Items.Add("2");
            tmp.Items.Add("3");
            tmp.Items.Add("4");
            tmp.Items.Add("5");
            tmp.Items.Add("11");
            tmp.Items.Add("22");
            tmp.Items.Add("33");
            tmp.Items.Add("44");
            tmp.Items.Add("55");
            tmp.Position = (6, 0);

            var selButton = new SelectionButton(24, 1)
            {
                Text = "Selection Button 1",
                Position = new Point(51, 3)
            };
            Controls.Add(selButton);

            var selButton1 = new SelectionButton(24, 1)
            {
                Text = "Selection Button 2",
                Position = new Point(51, 4)
            };
            Controls.Add(selButton1);

            var selButton2 = new SelectionButton(24, 1)
            {
                Text = "Selection Button 3",
                Position = new Point(51, 5)
            };
            Controls.Add(selButton2);

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
            Controls.Add(input);

            var password = new TextBox(10)
            {
                Mask = '*',
                Position = new Point(65, 9)
            };
            Controls.Add(password);

            var button = new Button(11, 1)
            {
                Text = "Click",
                Position = new Point(1, 3)
            };
            button.Click += (s, a) => SadConsole.UI.Window.Message("This has been clicked -- and your password field contains '" + password.Text + "'", "Close");
            Controls.Add(button);

            button = new Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 5),
                Theme = new Button3dTheme()
            };
            //button.AlternateFont = SadConsole.Global.LoadFont("Fonts/Cheepicus12.font").GetFont(Font.FontSizes.One);
            Controls.Add(button);

            button = new Button(11, 3)
            {
                Text = "Click",
                Position = new Point(1, 10),
                Theme = new ButtonLinesTheme()
            };
            Controls.Add(button);

            var checkbox = new CheckBox(13, 1)
            {
                Text = "Check box",
                Position = new Point(51, 13)
            };
            Controls.Add(checkbox);

            Controls.FocusedControl = null;
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

            RedrawColors();
        }

        public override bool ProcessMouse(SadConsole.Input.MouseScreenObjectState state) => base.ProcessMouse(state);

        public void RedrawColors()
        {
            IsDirty = true;
            Controls.ForceRedrawAllControls();

            var colors = Controls.GetThemeColors();

            Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, 0);

            this.Print(1, 1, "BUTTONS", colors.Title);
            this.Print(16, 1, "BARS", colors.Title);
            this.Print(28, 1, "LISTBOX", colors.Title);
            this.Print(28, 10, "RADIO BUTTON", colors.Title);

            this.Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", colors.Title);
            this.Print(51, 7, "TEXTBOX", colors.Title);
            this.Print(65, 7, "(WITH MASK)", colors.Title);

            this.Print(51, 11, "CHECKBOX", colors.Title);

            this.Print(2, 15, "RED ".CreateColored(colors.Red, null) +
                                      "PURPLE ".CreateColored(colors.Purple, null) +
                                      "BLUE ".CreateColored(colors.Blue, null) +
                                      "CYAN ".CreateColored(colors.Cyan, null) +
                                      "GREEN ".CreateColored(colors.Green, null) +
                                      "YELLOW ".CreateColored(colors.Yellow, null) +
                                      "ORANGE ".CreateColored(colors.Orange, null) +
                                      "BROWN ".CreateColored(colors.Brown, null) +
                                      "GRAY ".CreateColored(colors.Gray, null) +
                                      "WHITE ".CreateColored(colors.White, null)
                                      );

            this.Print(2, 16, "RED ".CreateColored(colors.RedDark, null) +
                                      "PURPLE ".CreateColored(colors.PurpleDark, null) +
                                      "BLUE ".CreateColored(colors.BlueDark, null) +
                                      "CYAN ".CreateColored(colors.CyanDark, null) +
                                      "GREEN ".CreateColored(colors.GreenDark, null) +
                                      "YELLOW ".CreateColored(colors.YellowDark, null) +
                                      "ORANGE ".CreateColored(colors.OrangeDark, null) +
                                      "BROWN ".CreateColored(colors.BrownDark, null) +
                                      "GRAY ".CreateColored(colors.GrayDark, null) +
                                      "BLACK ".CreateColored(colors.Black, null)
                                      );
            this.Print(2, 18, CreateGradientExample("RED", colors.Red, colors.RedDark));
            this.Print(2, 19, CreateGradientExample("PURPLE", colors.Purple, colors.PurpleDark));
            this.Print(2, 20, CreateGradientExample("BLUE", colors.Blue, colors.BlueDark));
            this.Print(2, 21, CreateGradientExample("CYAN", colors.Cyan, colors.CyanDark));
            this.Print(2, 22, CreateGradientExample("GREEN", colors.Green, colors.GreenDark));
            this.Print(34, 18, CreateGradientExample("YELLOW", colors.Yellow, colors.YellowDark));
            this.Print(34, 19, CreateGradientExample("ORANGE", colors.Orange, colors.OrangeDark));
            this.Print(34, 20, CreateGradientExample("BROWN", colors.Brown, colors.BrownDark));
            this.Print(34, 21, CreateGradientExample("GRAY", colors.Gray, colors.GrayDark));
            this.Print(34, 22, CreateGradientExample("WHITE", colors.White, colors.Black));

            //Print(2, 23, CreateGradientExample("GOLD", Library.Default.Colors.Gold, Library.Default.Colors.GoldDark));
        }

        private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7) => text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
    }
}
