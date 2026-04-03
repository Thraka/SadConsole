using SadConsole.Input;

namespace SadBBSClient;

internal class Phonebook: ScreenSurface
{
    public readonly List<PhoneBookEntry> Entries;

    public PhoneBookEntry? SelectedEntry { get; set; }

    private int selectedIndex = 0;

    public Phonebook(): base(AppSettings.Instance.Width, AppSettings.Instance.Height)
    {
        Surface.DefaultBackground = Color.Black;
        Surface.Clear();

        Surface.DrawBox(Surface.Area.Expand(-1, -1), ShapeParameters.CreateStyledBoxFilled(ICellSurface.Connected3dBox, new ColoredGlyph(Color.LightBlue, Color.Black), new ColoredGlyph(Color.White, Color.LightBlue)));

        Renderer?.Dispose();
        Renderer = new SadConsole.Renderers.OptimizedScreenSurfaceRenderer();

        Entries = [
                new("SadLogic", "192.168.1.237", 7243),
                new("Alterant BBS", "alterant.ca", 23),
                new("Level 29", "bbs.fozztexx.com", 23),
                new("Black Flag", "blackflagbbs.com", 23),
                new("Capitol Shrill", "capitolshrill.com", 6502),
                new("Deadline", "deadline.aegis-corp.org", 23),
            ];

        SelectedEntry = null;

        RedrawEntries();
    }

    private void RedrawEntries()
    {
        Rectangle area = Surface.Area.Expand(-2, -2);
        Surface.Clear(area);

        int visibleHeight = area.Height;
        int startIndex = Math.Max(0, selectedIndex - visibleHeight + 1);
        int endIndex = Math.Min(Entries.Count, startIndex + visibleHeight);

        for (int i = startIndex; i < endIndex; i++)
        {
            var entry = Entries[i];
            int row = area.Y + (i - startIndex);
            string line = $" {entry.Name,-20} {entry.Address,-30} {entry.Port,5} ";

            if (line.Length > area.Width)
                line = line[..area.Width];
            else
                line = line.PadRight(area.Width);

            Color foreground = i == selectedIndex ? Color.Black : Color.White;
            Color background = i == selectedIndex ? Color.White : Color.Black;

            Surface.Print(area.X, row, line, foreground, background);
        }
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Up))
        {
            selectedIndex = Math.Max(0, selectedIndex - 1);
            RedrawEntries();
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Down))
        {
            selectedIndex = Math.Min(Entries.Count - 1, selectedIndex + 1);
            RedrawEntries();
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Enter))
        {
            SelectedEntry = Entries[selectedIndex];
            IsVisible = false;
            return true;
        }
        else if(keyboard.IsKeyPressed(Keys.Escape))
        {
            SelectedEntry = null;
            IsVisible = false;
            return true;
        }

        return false;
    }
}
