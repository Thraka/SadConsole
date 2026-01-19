using SadConsole.Input;

namespace SadConsole.Examples;

internal class DemoMultipleCursors : IDemo
{
    public string Title => "Multiple Cursors";

    public string Description => "Cursors are just components that can be added to an object. This demo has three cursors, each with a different color." +
                                 "\r\n\r\n" +
                                 "Press [c:r f:Red:2]F3 to change the active cursor.";

    public string CodeFile => "DemoMultipleCursors.cs";

    public IScreenSurface CreateDemoScreen() =>
        new MultiCursor();

    public override string ToString() =>
        Title;
}

class MultiCursor : ScreenSurface
{
    private readonly SadConsole.Components.Cursor CursorGreen;
    private readonly SadConsole.Components.Cursor CursorYellow;
    private readonly SadConsole.Components.Cursor CursorPurple;

    public MultiCursor()
        : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        CursorGreen = new SadConsole.Components.Cursor()
        {
            CursorRenderCell = new ColoredGlyph(Color.Green, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            PrintAppearance = new ColoredGlyph(Color.Green, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            IsEnabled = true,
            ApplyCursorEffect = true
        };
        CursorYellow = new SadConsole.Components.Cursor()
        {
            CursorRenderCell = new ColoredGlyph(Color.Yellow, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            PrintAppearance = new ColoredGlyph(Color.Yellow, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            PrintOnlyCharacterData = false,
            IsEnabled = false,
            ApplyCursorEffect = false
        };
        CursorPurple = new SadConsole.Components.Cursor()
        {
            CursorRenderCell = new ColoredGlyph(Color.Purple, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            PrintAppearance = new ColoredGlyph(Color.Purple, Color.Transparent, SadConsole.Components.Cursor.DefaultCursorGlyph),
            PrintOnlyCharacterData = false,
            IsEnabled = false,
            ApplyCursorEffect = false
        };

        SadComponents.Add(CursorGreen);
        SadComponents.Add(CursorYellow);
        SadComponents.Add(CursorPurple);

        CursorGreen.Position = (23, 11);
        CursorYellow.Position = (11, 20);
        CursorPurple.Position = (2, 6);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.F3))
        {
            // Cycle active cursor
            if (CursorGreen.IsEnabled)
            {
                CursorYellow.IsEnabled = true;
                CursorYellow.ApplyCursorEffect = true;
                CursorGreen.IsEnabled = false;
                CursorGreen.ApplyCursorEffect = false;
            }
            else if (CursorYellow.IsEnabled)
            {
                CursorPurple.IsEnabled = true;
                CursorPurple.ApplyCursorEffect = true;
                CursorYellow.IsEnabled = false;
                CursorYellow.ApplyCursorEffect = false;
            }
            else if (CursorPurple.IsEnabled)
            {
                CursorGreen.IsEnabled = true;
                CursorGreen.ApplyCursorEffect = true;
                CursorPurple.IsEnabled = false;
                CursorPurple.ApplyCursorEffect = false;
            }

            return true;
        }
        else
            return base.ProcessKeyboard(keyboard);
    }
}
