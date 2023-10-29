using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoControls2 : IDemo
{
    public string Title => "Controls (2nd page)";

    public string Description => "This page is a second set of controls, mostly newer controls.";

    public string CodeFile => "DemoControls2.cs";

    public IScreenSurface CreateDemoScreen() =>
        new ControlsTest2();

    public override string ToString() =>
        Title;
}

class ControlsTest2 : SadConsole.UI.ControlsConsole
{
    public ControlsTest2() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        Controls.ThemeColors = GameSettings.ControlColorScheme;

        //
        // Panel control showing a label
        //
        Panel panel = new(20, 6) { Position = (1, 2), Name = "label panel" };
        {
            Label label = new("A label control") { Position = (1, 1), ShowStrikethrough = false, ShowUnderline = true };
            CheckBox strikethrough = new("Strikethrough") { Position = (1, 3) };
            CheckBox underline = new("Underline    ") { Position = (1, 4), IsSelected = true };

            strikethrough.IsSelectedChanged += (s, e) => { label.ShowStrikethrough = ((CheckBox)s!).IsSelected; };
            underline.IsSelectedChanged += (s, e) => { label.ShowUnderline = ((CheckBox)s!).IsSelected; };

            panel.Add(label);
            panel.Add(strikethrough);
            panel.Add(underline);
        }

        panel.DrawBorder = true;
        panel.UseExtendedBorderGlyphs = false;

        Controls.Add(panel);

        //
        // Toggle switch
        //
        ToggleSwitch toggle = new(7, 1)
        {
            Position = (panel.Position.X, panel.Bounds.MaxExtentY + 3)
        };

        Controls.Add(toggle);

        //
        // Table control 
        //
        Table tableControl = new Table(20, 10, 3);

        int counter = 0;

        for (int y = 0; y < 10; y++)
            for (int x = 0; x < 20; x++)
                tableControl.Cells[y, x].Value = ++counter;

        tableControl.Position = (1, 8);

        //Controls.Add(tableControl);

        //
        // Tab control showing two tabs
        //
        Panel tabPanel1 = new Panel(10, 10);
        {
            RadioButton opt1 = new("Tab Orientation Top") { Position = (1, 1), Tag = TabControl.Orientation.Top };
            opt1.IsSelectedChanged += Opt1_IsSelectedChanged;
            opt1.IsSelected = true;
            tabPanel1.Add(opt1);
            opt1 = new("Tab Orientation Bottom") { Position = (1, 2), Tag = TabControl.Orientation.Bottom };
            opt1.IsSelectedChanged += Opt1_IsSelectedChanged;
            tabPanel1.Add(opt1);
            opt1 = new("Tab Orientation Left") { Position = (1, 3), Tag = TabControl.Orientation.Left };
            opt1.IsSelectedChanged += Opt1_IsSelectedChanged;
            tabPanel1.Add(opt1);
            opt1 = new("Tab Orientation Right") { Position = (1, 4), Tag = TabControl.Orientation.Right };
            opt1.IsSelectedChanged += Opt1_IsSelectedChanged;
            tabPanel1.Add(opt1);

            CheckBox check = new CheckBox(15, 1) { Text = "Thick Lines" };
            check.IsSelectedChanged += (s, e) =>
            {
                if (!Controls.HasNamedControl("tab", out ControlBase? tabControl)) return;

                if (((CheckBox)s!).IsSelected)
                    ((TabControl)tabControl).ConnectedLineStyle = ICellSurface.ConnectedLineThick;
                else
                    ((TabControl)tabControl).ConnectedLineStyle = ICellSurface.ConnectedLineThin;

            };

            check.PlaceRelativeTo(opt1, Direction.Types.Down);
        }

        Panel tabPanel2 = new Panel(10, 10);
        {
            CheckBox check = new CheckBox("Checkbox 1") { Position = (2, 1) };
            tabPanel2.Add(check);
            check = new CheckBox(15, 1) { Text = "Checkbox 2", Position = (3, 2) };
            tabPanel2.Add(check);
            check = new CheckBox(15, 1) { Text = "Checkbox 3", Position = (4, 3) };
            tabPanel2.Add(check);
        }
        
        TabControl tab = new TabControl(new[] { new TabItem("Style", tabPanel1) { AutomaticPadding = 0 },
                                                new TabItem("Nothing", tabPanel2) { AutomaticPadding = 0 },
                                              },
                                              35, 15) { Name = "tab" };
        tab.Position = (22, 2);
        Controls.Add(tab);

        // Create the radio buttons for blue/black themes
        RadioButton colorsBlack = new("Ansi Black")
        {
            Tag = Colors.CreateAnsi(),
            Position = (1, 22)
        };
        RadioButton colorsBlue = new("SadConsole Blue")
        {
            Tag = Colors.CreateSadConsoleBlue(),
            Position = (1, 23)
        };

        colorsBlack.IsSelected = GameSettings.ControlColorScheme.Name.Equals("ansi", StringComparison.InvariantCultureIgnoreCase);
        colorsBlue.IsSelected = !colorsBlack.IsSelected;

        colorsBlack.IsSelectedChanged += Colors_IsSelectedChanged;
        colorsBlue.IsSelectedChanged += Colors_IsSelectedChanged;

        Controls.Add(colorsBlack);
        Controls.Add(colorsBlue);

        OnInvalidated();
    }

    private void Colors_IsSelectedChanged(object? sender, EventArgs e)
    {
        if (sender is RadioButton button)
        {
            if (button.IsSelected)
            {
                GameSettings.ControlColorScheme = (Colors)button.Tag!;
                Controls.ThemeColors = GameSettings.ControlColorScheme;
                OnInvalidated();
            }
        }
    }

    private void PanelButton1_Click(object? sender, EventArgs e)
    {
        Window.Ask(ColoredString.Parser.Parse("Enter a [c:r f:blue]number[c:u]          "), "OK", "Cancel", null);
    }

    private void Opt1_IsSelectedChanged(object? sender, EventArgs e)
    {
        if (!Controls.HasNamedControl("tab", out ControlBase? tabControl)) return;

        RadioButton button = ((RadioButton)sender!);
        if (button.IsSelected)
            ((TabControl)tabControl).TabOrientation = (TabControl.Orientation)button.Tag!;
    }

    protected void OnInvalidated()
    {
        var colors = Controls.GetThemeColors();

        Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, 0);

        Panel panel = (Panel)Controls["label panel"];
        TabControl tab = (TabControl)Controls["tab"];

        this.Print(panel.Position.X, panel.Position.Y - 1, "PANEL WITH LABEL", colors.YellowDark);
        this.Print(tab.Position.X, tab.Position.Y - 1, "TAB CONTROL", colors.YellowDark);
        this.Print(1, 21, "COLOR THEME", colors.YellowDark);
        this.Print(1, panel.Bounds.MaxExtentY + 1, "TOGGLE SWITCH", colors.YellowDark);
    }
}




