using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoControls : IDemo
{
    public string Title => "Controls";

    public string Description => "All of the SadConsole controls are displayed in this demo. You can test each control." +
                                 "\r\n\r\n" +
                                 "Controls use themes to style and color themselves. The bottom part of the demo window " +
                                 "shows a set of colors used by the current theme.";

    public string CodeFile => "DemoControls.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ControlsTest();

    public override string ToString() =>
        Title;
}

class ControlsTest : SadConsole.UI.ControlsConsole
{
    private readonly Color[] backgroundcycle;
    private int backIndex = 0;
    private readonly SadConsole.Components.Timer progressTimer;

    public ControlsTest() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Controls.ThemeColors = GameSettings.ControlColorScheme;

        var prog1 = new ProgressBar(10, 1, HorizontalAlignment.Left)
        {
            Position = new Point(16, 5)
        };
        Controls.Add(prog1);

        var prog2 = new ProgressBar(1, 6, VerticalAlignment.Bottom)
        {
            Position = new Point(18, 7)
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

        progressTimer = new SadConsole.Components.Timer(TimeSpan.FromSeconds(0.5));
        progressTimer.Start();
        progressTimer.TimerElapsed += (timer, e) => { prog1.Progress = prog1.Progress >= 1f ? 0f : prog1.Progress + 0.1f; prog2.Progress = prog2.Progress >= 1f ? 0f : prog2.Progress + 0.1f; };

        SadComponents.Add(progressTimer);

        var listbox = new ListBox(20, 6)
        {
            Position = new Point(28, 3)
        };
        listbox.Items.Add("item 1");
        listbox.Items.Add("item 2");
        listbox.Items.Add(new ColoredString(new ColoredGlyph(Color.AnsiGreenBright, Color.DarkGreen, 'i'),
                                            new ColoredGlyph(Color.AnsiCyanBright, Color.AnsiCyan, 't'),
                                            new ColoredGlyph(Color.DarkGreen, Color.DarkGray, 'e'),
                                            new ColoredGlyph(Color.YellowGreen, Color.DarkGray, 'm'),
                                            new ColoredGlyph(Color.Turquoise, Color.DarkGray, ' '),
                                            new ColoredGlyph(Color.Tomato, Color.DarkGray, '3')
                                            ));
        listbox.Items.Add("item 4");
        listbox.Items.Add("item 5");
        listbox.Items.Add("item 6");
        listbox.Items.Add("item 7");
        listbox.Items.Add("item 8");
        listbox.Items.Add("item 9");

        listbox.SelectedItemExecuted += (s, e) =>
        {
            Window.Message($"Listbox item executed: {e.Item}", "OK");
        };

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

        var input = new NumberBox(10)
        {
            Position = new Point(51, 9),
            MaxLength = 6,
            NumberMaximum = 255,
            AllowDecimal = true
        };
        Controls.Add(input);

        var password = new TextBox(10)
        {
            Mask = '*',
            Position = new Point(65, 9)
        };
        Controls.Add(password);

        ButtonBase button;

        button = new Button(11, 1)
        {
            Text = "Click",
            Position = new Point(1, 3)
        };
        button.Click += (s, a) => SadConsole.UI.Window.Message("This has been clicked -- and your password field contains '" + password.Text + "'", "Close");
        Controls.Add(button);

        button = new Button3d(11, 3)
        {
            Text = "Click",
            Position = new Point(1, 5),
        };
        button.Click += (s, a) => listbox.ScrollToSelectedItem();
        //button.AlternateFont = SadConsole.Global.LoadFont("Fonts/Cheepicus12.font").GetFont(Font.FontSizes.One);
        Controls.Add(button);

        button = new ButtonBox(11, 3)
        {
            Text = "Click",
            Position = new Point(1, 10),
        };
        button.Click += (s, a) => Game.Instance.ResizeWindow(1800, 980, true);
        Controls.Add(button);

        var checkbox = new CheckBox(13, 1)
        {
            Text = "Check box",
            Position = new Point(51, 13)
        };
        Controls.Add(checkbox);

        // ComboBox
        ComboBox box = new ComboBox(10, 15, 10, Enumerable.Range(1, 20).Cast<object>().ToArray())
        {
            Position = (checkbox.Bounds.MaxExtentX + 4, checkbox.Position.Y),
        };
        Controls.Add(box);

        Controls.FocusedControl = null;
        //DisableControlFocusing = true;

        List<Tuple<Color, string>> colors = new List<Tuple<Color, string>>
            {
                new Tuple<Color, string>(Colors.Default.Red, "Red"),
                new Tuple<Color, string>(Colors.Default.RedDark, "DRed"),
                new Tuple<Color, string>(Colors.Default.Purple, "Prp"),
                new Tuple<Color, string>(Colors.Default.PurpleDark, "DPrp"),
                new Tuple<Color, string>(Colors.Default.Blue, "Blu"),
                new Tuple<Color, string>(Colors.Default.BlueDark, "DBlu"),
                new Tuple<Color, string>(Colors.Default.Cyan, "Cya"),
                new Tuple<Color, string>(Colors.Default.CyanDark, "DCya"),
                new Tuple<Color, string>(Colors.Default.Green, "Gre"),
                new Tuple<Color, string>(Colors.Default.GreenDark, "DGre"),
                new Tuple<Color, string>(Colors.Default.Yellow, "Yel"),
                new Tuple<Color, string>(Colors.Default.YellowDark, "DYel"),
                new Tuple<Color, string>(Colors.Default.Orange, "Ora"),
                new Tuple<Color, string>(Colors.Default.OrangeDark, "DOra"),
                new Tuple<Color, string>(Colors.Default.Brown, "Bro"),
                new Tuple<Color, string>(Colors.Default.BrownDark, "DBrow"),
                new Tuple<Color, string>(Colors.Default.Gray, "Gray"),
                new Tuple<Color, string>(Colors.Default.GrayDark, "DGray"),
                new Tuple<Color, string>(Colors.Default.White, "White"),
                new Tuple<Color, string>(Colors.Default.Black, "Black")
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

        OnInvalidated();
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyDown(Keys.F2))
        {
            CharacterSelectWindow window = new(15, 20);
            window.Show(true);
            return true;
        }

        // Allow the controls to process the keyboard
        return base.ProcessKeyboard(keyboard);
    }

    protected void OnInvalidated()
    {
        var colors = Controls.GetThemeColors();

        Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, 0);

        this.Print(1, 1, "BUTTONS", colors.YellowDark);
        this.Print(16, 1, "BARS", colors.YellowDark);
        this.Print(28, 1, "LISTBOX", colors.YellowDark);
        this.Print(28, 10, "RADIO BUTTON", colors.YellowDark);

        this.Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", colors.YellowDark);
        this.Print(51, 7, "TEXTBOX", colors.YellowDark);
        this.Print(65, 7, "(WITH MASK)", colors.YellowDark);

        this.Print(51, 11, "CHECKBOX", colors.YellowDark);
        this.Print(67, 11, "COMBOBOX", colors.YellowDark);

        int colorsStartY = 17;

        this.Print(2, colorsStartY, "RED ".CreateColored(colors.Red, null) +
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

        this.Print(2, colorsStartY + 1, "RED ".CreateColored(colors.RedDark, null) +
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
        this.Print(2, colorsStartY + 3, CreateGradientExample("RED", colors.Red, colors.RedDark));
        this.Print(2, colorsStartY + 4, CreateGradientExample("PURPLE", colors.Purple, colors.PurpleDark));
        this.Print(2, colorsStartY + 5, CreateGradientExample("BLUE", colors.Blue, colors.BlueDark));
        this.Print(2, colorsStartY + 6, CreateGradientExample("CYAN", colors.Cyan, colors.CyanDark));
        this.Print(2, colorsStartY + 7, CreateGradientExample("GREEN", colors.Green, colors.GreenDark));
        this.Print(34, colorsStartY + 3, CreateGradientExample("YELLOW", colors.Yellow, colors.YellowDark));
        this.Print(34, colorsStartY + 4, CreateGradientExample("ORANGE", colors.Orange, colors.OrangeDark));
        this.Print(34, colorsStartY + 5, CreateGradientExample("BROWN", colors.Brown, colors.BrownDark));
        this.Print(34, colorsStartY + 6, CreateGradientExample("GRAY", colors.Gray, colors.GrayDark));
        this.Print(34, colorsStartY + 7, CreateGradientExample("WHITE", colors.White, colors.Black));

        //Print(2, 23, CreateGradientExample("GOLD", Library.Default.Colors.Gold, Library.Default.Colors.GoldDark));
    }

    private ColoredString CreateGradientExample(string text, Color start, Color end, int stringLength = 7) => text.PadRight(stringLength).Substring(0, stringLength).CreateColored(start) + new string((char)219, 15).CreateGradient(start, end) + text.PadLeft(stringLength).Substring(0, stringLength).CreateColored(end);
}
