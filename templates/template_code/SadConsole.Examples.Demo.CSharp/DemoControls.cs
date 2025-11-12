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
            MaximumValue = 18
        };
        Controls.Add(slider);

        slider = new ScrollBar(Orientation.Vertical, 6)
        {
            Position = new Point(16, 7),
            MaximumValue = 6
        };
        Controls.Add(slider);

        progressTimer = new SadConsole.Components.Timer(TimeSpan.FromSeconds(0.5));
        progressTimer.Start();
        progressTimer.TimerElapsed += (timer, e) =>
        {

            if (prog1.Progress >= 1f)
                prog1.Progress = 0f;
            else
                prog1.Progress += 0.1f;

            if (prog2.Progress >= 1f)
                prog2.Progress = 0f;
            else
                prog2.Progress += 0.1f;
        };

        // We want the timer to run before the controls host does, otherwise other controls
        // might trigger a render pass before the timer event and do to the way update then
        // render works, rendering may clear the IsDirty flag that the timer set on the bar.
        progressTimer.SortOrder = 0;
        Controls.SortOrder = 1;

        SadComponents.Add(progressTimer); // This causes a sort to happen

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

        var txt = new TextBox2(10)
        {
            Position = new Point(51, 9),
            Text = "text",
            Name = "textbox"
        };
        Controls.Add(txt);

        var input = new NumberBox(10)
        {
            Position = new Point(txt.Position.X, txt.Position.Y + 1),
            //MaxLength = 6,
            ShowUpDownButtons = true,
            //NumberMaximum = 255,
            AllowDecimal = true,
            //Text = "25",
            Name = "numberbox"
        };
        Controls.Add(input);

        var password = new TextBox(10)
        {
            Mask = '*',
            Position = new Point(txt.Position.X, txt.Position.Y + 2),
            Text = "text",
            Name = "passwordbox"
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
            Position = new Point(51, 15),
            Name = "checkbox"
        };
        Controls.Add(checkbox);

        // ComboBox
        ComboBox box = new(10, 15, 10, Enumerable.Range(1, 20).Cast<object>().ToArray())
        {
            Position = (checkbox.Bounds.MaxExtentX + 4, checkbox.Position.Y),
            Name = "combobox"
        };
        Controls.Add(box);

        Controls.FocusedControl = null;
        //DisableControlFocusing = true;

        List<Tuple<Color, string>> colors = new()
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

    protected void OnInvalidated()
    {
        var colors = Controls.GetThemeColors();

        Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, 0);

        TextBox textbox = (TextBox)Controls.GetNamedControl("textbox");
        NumberBox numberbox = (NumberBox)Controls.GetNamedControl("numberbox");
        TextBox passbox = (TextBox)Controls.GetNamedControl("passwordbox");

        this.Print(textbox.Position.X, textbox.Position.Y - 2, "TEXTBOX", colors.YellowDark);
        this.Print(textbox.Position.X + textbox.Width + 1, textbox.Position.Y, "<- Normal", colors.YellowDark);
        this.Print(numberbox.Position.X + numberbox.Width + 1, numberbox.Position.Y, "<- Numbers", colors.YellowDark);
        this.Print(passbox.Position.X + passbox.Width + 1, passbox.Position.Y, "<- Masked", colors.YellowDark);


        this.Print(1, 1, "BUTTONS", colors.YellowDark);
        this.Print(16, 1, "BARS", colors.YellowDark);
        this.Print(28, 1, "LISTBOX", colors.YellowDark);
        this.Print(28, 10, "RADIO BUTTON", colors.YellowDark);

        this.Print(51, 1, "SELECTION BUTTON (UP/DN KEYS)", colors.YellowDark);

        CheckBox checkbox = (CheckBox)Controls.GetNamedControl("checkbox");
        this.Print(checkbox.Position.X, checkbox.Position.Y - 2, "CHECKBOX", colors.YellowDark);
        ComboBox combo = (ComboBox)Controls.GetNamedControl("combobox");
        this.Print(combo.Position.X, combo.Position.Y - 2, "COMBOBOX", colors.YellowDark);

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

    private class TextBox2 : TextBox
    {
        private int _oldCaretPosition;
        private ControlStates _oldState;
        private string _editingText = string.Empty;

        public TextBox2(int width) : base(width)
        {
        }

        public override void UpdateAndRedraw(TimeSpan time)
        {
            // IMPORTANT:
            // Code fixed here should go into the NumberBox control

            if (Surface.Effects.Count != 0)
            {
                Surface.Effects.UpdateEffects(time);
                IsDirty = IsDirty || Surface.IsDirty;
            }

            if (!IsDirty) return;

            Colors colors = FindThemeColors();

            RefreshThemeStateColors(colors);

            bool isFocusedSameAsBack = ThemeState.Focused.Background == colors.ControlHostBackground;

            ThemeState.Normal.Background = colors.GetOffColor(ThemeState.Normal.Background, colors.ControlHostBackground);
            ThemeState.MouseOver.Background = colors.GetOffColor(ThemeState.MouseOver.Background, colors.ControlHostBackground);
            ThemeState.MouseDown.Background = colors.GetOffColor(ThemeState.MouseDown.Background, colors.ControlHostBackground);
            ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, colors.ControlHostBackground);

            // Further alter the color to indicate focus
            if (isFocusedSameAsBack)
                ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Focused.Background);

            // If the focused background color is the same as the non-focused, alter it so it stands out
            ThemeState.Focused.Background = colors.GetOffColor(ThemeState.Focused.Background, ThemeState.Normal.Background);

            ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);

            if (IsFocused && (Parent?.Host?.ParentConsole?.IsFocused).GetValueOrDefault(false) && !DisableKeyboard)
            {
                // TextBox was just focused
                if (State.HasFlag(ControlStates.Focused) && !_oldState.HasFlag(ControlStates.Focused))
                {
                    _oldCaretPosition = CaretPosition;
                    _oldState = State;
                    _editingText = Text;
                    Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                    if (Mask == null)
                        Surface.Print(0, 0, Text.Substring(LeftDrawOffset));
                    else
                        Surface.Print(0, 0, Text.Substring(LeftDrawOffset).Masked(Mask.Value));

                    Surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);
                }

                else if (_oldCaretPosition != CaretPosition || _oldState != State || _editingText != Text)
                {
                    Surface.Effects.RemoveAll();
                    Surface.Fill(appearance.Foreground, appearance.Background, 0, Mirror.None);

                    if (Mask == null)
                        Surface.Print(0, 0, Text.Substring(LeftDrawOffset));
                    else
                        Surface.Print(0, 0, Text.Substring(LeftDrawOffset).Masked(Mask.Value));

                    // TODO: If the keyboard repeat is down and the text goes off the end of the textbox and we're hitting the left arrow then sometimes control.LeftDrawOffset can exceed control.CaretPosition
                    // This causes an Out of Bounds error here.  I don't think it's new - I think it's been in for a long time so I'm gonna check in and come back to this.
                    // It might be that we just need to take Max(0, "bad value") below but I think it should be checked into to really understand the situation.
                    Surface.SetEffect(CaretPosition - LeftDrawOffset, 0, CaretEffect);
                    _oldCaretPosition = CaretPosition;
                    _oldState = State;
                    _editingText = Text;
                }
            }
            else
            {
                Surface.Effects.RemoveAll();
                Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, appearance.Mirror);
                _oldState = State;

                if (Mask == null)
                    Surface.Print(0, 0, Text.Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));
                else
                    Surface.Print(0, 0, Text.Masked(Mask.Value).Align(HorizontalAlignment.Left, UseDifferentTextAreaWidth ? TextAreaWidth : Width));
            }

            // Colors for box
            Color topleftcolor = colors.Lines.ComputedColor.GetDark();
            Color bottomrightcolor = colors.Lines.ComputedColor.GetBright();

            // Add box
            Surface.SetDecorator(0, Surface.Width,
                                new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),
                                new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));
            Surface.AddDecorator(0, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
            Surface.AddDecorator(Surface.Width - 1, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
        }
    }
}
