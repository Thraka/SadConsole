//
// For v10 Alpha 1, this demo is unfinished
//


using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadConsole.Examples;

internal class DemoControls2 : IDemo
{
    public string Title => "Controls (2nd page)";

    public string Description => "This page is a second set of controls, mostly newer controls. This page is incomplete for Alpha 1";

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
        Controls.ThemeColors = Colors.CreateSadConsoleBlue();

        void PanelContent(Panel panelObject)
        {
            Label label = new(10) { DisplayText = "Some Text" };
            Button clear = new(9) { Text = "Clear" };
            clear.Click += (s,e) => { label.ShowStrikethrough = false; label.ShowUnderline = false; };
        }

        Panel panel = new(20, 6) { Position = (1, 1) };
        {
            
        }
        ((PanelTheme)panel.Theme).DrawBorder = true;
        ((PanelTheme)panel.Theme).UseExtendedBorderGlyphs = false;

        Controls.Add(panel);

        panel = new(20, 6);
        {
            Button panelButton1 = new Button(5) { Text = "1", Position = (1, 1) };
            Button panelButton2 = new Button(5) { Text = "2", Position = (1, 2) };
            Button panelButton3 = new Button(5) { Text = "3", Position = (1, 3) };

            panel.Add(panelButton1);
            panel.Add(panelButton2);
            panel.Add(panelButton3);
        }
        ((PanelTheme)panel.Theme).DrawBorder = true;
        ((PanelTheme)panel.Theme).UseInsetBorder = true;
        ((PanelTheme)panel.Theme).UseExtendedBorderGlyphs = false;

        panel.PlaceRelativeTo(Controls[0], Direction.Types.Right, 2);

        Controls.Add(panel);

        Table tableControl = new Table(20, 10, 3);

        int counter = 0;

        for (int y = 0; y < 10; y++)
            for (int x = 0; x < 20; x++)
                tableControl.Cells[y, x].Value = ++counter;

        tableControl.Position = (1, 8);

        Controls.Add(tableControl);


        OnInvalidated();
    }

    private void Clear_Click(object? sender, EventArgs e)
    {
        
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        // If the key we want to watch for is hit, change the themes
        if (keyboard.IsKeyPressed(Keys.F1))
        {
            Controls.ThemeColors = Controls.ThemeColors == null ? UI.Colors.CreateSadConsoleBlue() : null;
            OnInvalidated();
            IsDirty = true;
            return true;
        }

        // Allow the controls to process the keyboard
        return base.ProcessKeyboard(keyboard);
    }

    protected void OnInvalidated()
    {
        var colors = Controls.GetThemeColors();

        Surface.Fill(colors.ControlHostForeground, colors.ControlHostBackground, 0, 0);

        Surface.DrawBox(Controls.OfType<Table>().First().Bounds.Expand(1, 1), ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new(Color.White, Color.Transparent), false, true));
        //this.Print(0, 1, "BUTTONS", colors.YellowDark);
    }
}




