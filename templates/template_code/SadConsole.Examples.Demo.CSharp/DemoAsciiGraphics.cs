using SadConsole.Ansi;
using SadConsole.Input;
using SadConsole.Readers;
using SadConsole.UI;
using static SadConsole.Examples.RootScreen;

namespace SadConsole.Examples;

internal class DemoAcii : IDemo
{
    public string Title => "Ascii/Ansi/Playscii";

    public string Description => "This demo loads loads different file formats for display in SadConsole.\r\n\r\n[c:r f:Yellow:1]- Plain text ascii\r\n[c:r f:Yellow:1]- Codepage 437 ANSI\r\n[c:r f:Yellow:1]- Playscii\r\n\r\nPress the [c:r f:Red:10]Left Arrow or [c:r f:Red:11]Right Arrow keys to navigate files and [c:r f:Red:8]Up Arrow or [c:r f:Red:10]Down Arrow keys to scroll Ansi files.";

    public string CodeFile => "DemoAsciiGraphics.cs";

    public IScreenSurface CreateDemoScreen() =>
        new AsciiGraphics();

    public override string ToString() =>
        Title;
}

internal class AsciiGraphics : ScreenSurface
{
    private readonly Console _slideDescription;
    private readonly AsciiArt[] _asciiArtPages;

    public AsciiGraphics() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        _asciiArtPages = new AsciiArt[]
        {
                new AsciiGraphicsTitlePage(),
                new AnsiArt("TES-JC.ANS"),
                new AnsiArt("QS-SIERR.ANS"),
                new AnsiArt("ROY-BTC1.ANS"),
                new AnsiArt("ROY-DGZN.ANS"),
                new PlaysciiArt("playscii_welcome_art.psci"),
                new PlaysciiArt("scene.psci"),
                new PlaysciiArt("pseudo_picasso.psci")
        };

        Children.Add(_asciiArtPages[0]);

        _slideDescription = new(60, 3);
        _slideDescription.Position = (Width - _slideDescription.Width, Height + _slideDescription.Height);
        Border.CreateForSurface(_slideDescription, "");

        Children.Add(_slideDescription);

        _slideDescription.Print(0, 0, _asciiArtPages[0].Title, Color.Yellow);
        _slideDescription.Cursor.Move(0, 1).Print(ColoredString.Parser.Parse(_asciiArtPages[0].Summary));
        _slideDescription.SadComponents.Add(new LineCharacterFade(TimeSpan.FromSeconds(0.3)));
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysDown)
        {
            if (Children[0] is AnsiArt a)
            {
                if (keyboard.IsKeyDown(Keys.Up))
                    return a.ScrollView(Direction.Up);
                else if (keyboard.IsKeyDown(Keys.Down))
                    return a.ScrollView(Direction.Down);
            }

            if (keyboard.IsKeyPressed(Keys.Right))
            {
                NextAsciiArt();
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                PrevAsciiArt();
                return true;
            }
        }
        return false;
    }

    private void NextAsciiArt() =>
        ChangeAsciiArt(_asciiArtPages.Length - 1, 1, _asciiArtPages[0]);

    private void PrevAsciiArt() =>
        ChangeAsciiArt(0, -1, _asciiArtPages.Last());

    private void ChangeAsciiArt(int testIndex, int step, AsciiArt overlappingPage)
    {
        // get the index of current page
        int currentPageIndex = Array.IndexOf(_asciiArtPages, Children[0]);

        // pull the next page from array and display it
        int nextIndex = currentPageIndex + step;
        AsciiArt newPage = currentPageIndex == testIndex ? overlappingPage : _asciiArtPages[nextIndex];

        if (newPage is AnsiArt art)
            art.ResetView();

        Children[0] = newPage;

        // change header title and summary to describe the page
        _slideDescription.Clear();
        // Remove old description component
        LineCharacterFade? component = _slideDescription.GetSadComponent<LineCharacterFade>();
        if (component != null)
            _slideDescription.SadComponents.Remove(component);

        // Clear the description window and rewrite it
        _slideDescription.Clear();
        _slideDescription.Print(0, 0, newPage.Title, Color.Yellow);
        _slideDescription.Cursor.Move(0, 1).Print(ColoredString.Parser.Parse(newPage.Summary));
        _slideDescription.SadComponents.Add(new LineCharacterFade(TimeSpan.FromSeconds(0.3)));
    }
}

class AsciiArt : ScreenSurface
{
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;

    public AsciiArt(Description desc) : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height)
    {
        // set page descriptions
        Title = desc.Title;
        Summary = desc.Summary;
    }

    public AsciiArt() : base(GameSettings.ScreenDemoBounds.Width, GameSettings.ScreenDemoBounds.Height) { }
}

class AsciiGraphicsTitlePage : AsciiArt
{
    public static new string Title => "Ascii Graphics";
    public static new string Summary => "Read ANSI, REXPaint and (partially supported) Playscii files.";

    public AsciiGraphicsTitlePage() : base(new Description(Title, Summary))
    {
        string[] lines = File.ReadAllLines("Res/Ascii/ascii_graphics_title.txt");
        for (int i = 0; i < lines.Length; i++)
            Surface.Print(0, i, lines[i], mirror: i == 1 ? Mirror.Vertical : Mirror.None);
    }
}

record Description(string Title, string Summary)
{
    public string FontName { get; set; } = string.Empty;
}

class AnsiArt : AsciiArt
{
    public static readonly Dictionary<string, Description> Descriptions = new()
        {
            { "logo.ans", new Description("ANSI Doc - SadConsole Logo", "Logo for SadConsole.") },
            { "QS-SIERR.ANS", new Description("ANSI Doc - Sierra BBS", "Advertisement for Sierra BBS by Quick Silver (VALiANT collection 1993).") },
            { "TES-JC.ANS", new Description("ANSI Doc - WILDC.A.T.S", "Advertisement for Jet City BBS by Jim Lee (VALiANT collection 1993).") },
            { "ROY-BTC1.ANS", new Description("ANSI Doc - Blocktronicks", "Art by Roy of SAC. Inspired by Font Designs from Amroth/iCE.") },
            { "ROY-DGZN.ANS", new Description("ANSI Doc - Digital Zone", "Advertisement for Digital Zone BBS by Roy of SAC.") },
        };

    public AnsiArt(string ansiFileName) : base(Descriptions[ansiFileName])
    {
        int ansiDocWidth = 80;

        // read the doc once to see how many rows it needs
        Document doc = new($"Res/Ansi/{ansiFileName}");
        ScreenSurface ansiSurface = new(ansiDocWidth, 25);
        AnsiWriter writer = new(doc, ansiSurface.Surface);

        writer.ReadEntireDocument();

        // resize this surface to the doc height
        int totalHeight = ansiSurface.Surface.Height + ansiSurface.Surface.TimesShiftedUp;
        ((CellSurface)Surface).Resize(ansiDocWidth, GameSettings.ScreenDemoBounds.Height, ansiDocWidth, totalHeight, false);

        // read the doc again and give it the surface with the proper height
        writer = new AnsiWriter(doc, Surface);
        writer.ReadEntireDocument();
    }

    public bool ScrollView(Direction d)
    {
        Surface.View = Surface.View.ChangePosition(d);
        return true;
    }

    public void ResetView() =>
        Surface.View = Surface.View.WithY(0);
}

class PlaysciiArt : AsciiArt
{
    public static readonly Dictionary<string, Description> Descriptions = new()
        {
            { "scene.psci", new Description("Playscii Art - RPG Scene",
                "Sample scene made with an RPG Maker VX RTP Tileset.") { FontName = "RPG_Maker_VX_RTP_Tileset_Wide" } },

            { "playscii_welcome_art.psci", new Description("Playscii Art - Editor Welcome Screen",
                "Image displayed in the Playscii editor window when the app starts.") { FontName = "c64_petscii" } },

            { "pseudo_picasso.psci", new Description("Playscii Art - Pseudo Picasso",
                "Image inspired by works of a certain famous artist ;)") { FontName = "jpetscii" } }
        };

    public PlaysciiArt(string playsciiFileName) : base(Descriptions[playsciiFileName])
    {
        // change background of this surface for better clarity
        Surface.DefaultBackground = Color.Gray;
        Surface.Clear();

        // get playscii font
        IFont font = GetFont(Descriptions[playsciiFileName].FontName);

        // convert the playscii file
        ScreenSurface image = Playscii.ToScreenSurface($"{playsciiFileName}", font, zipArchiveName: "Res/Playscii/playscii.zip");

        // change the frame background to cover the transparent tiles
        image.Surface.DefaultBackground = Color.DarkGray.GetDarkest();
        image.Surface.Clear();

        float modifier = 2;
        if (playsciiFileName == "pseudo_picasso.psci")
        {
            image.FontSize *= modifier;
            foreach (IScreenObject child in image.Children)
                ((ScreenSurface)child).FontSize *= modifier;
        }

        // add image to this surface
        AddCentered(image);
    }

    private static IFont GetFont(string fontName) => !Game.Instance.Fonts.ContainsKey(fontName) ?
        Game.Instance.LoadFont($"Res/Fonts/{fontName}.font") : Game.Instance.Fonts[fontName];

    private void AddCentered(ScreenSurface surface)
    {
        Children.Add(surface);
        surface.UsePixelPositioning = true;
        surface.Position = (WidthPixels / 2 - surface.AbsoluteArea.Width / 2, HeightPixels / 2 - surface.AbsoluteArea.Height / 2);
    }
}
