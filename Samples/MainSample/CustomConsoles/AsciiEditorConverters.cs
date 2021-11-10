using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Input;
using SadConsole.Instructions;
using SadConsole.Readers;
using SadConsole.Ansi;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class AsciiEditorConverters : SadConsole.Console, IRestartable
    {
        readonly HeaderConsole _header;
        readonly AsciiArt[] _asciiArtPages;

        public AsciiEditorConverters(HeaderConsole header) : base(Program.MainWidth, Program.MainHeight)
        {
            _header = header;

            _asciiArtPages = new AsciiArt[]
            {
                new TitlePage(),
                new AnsiArt("TES-JC.ANS"),
                new AnsiArt("QS-SIERR.ANS"),
                new AnsiArt("ROY-BTC1.ANS"),
                new AnsiArt("ROY-DGZN.ANS"),
                new PlaysciiArt("playscii_welcome_art.psci"),
                new PlaysciiArt("ink_splashes.psci")
            };
        }

        public void Restart()
        {
            Children.Clear();
            Children.Add(_asciiArtPages[0]);
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
                    return NextAsciiArt();
                else if (keyboard.IsKeyPressed(Keys.Left))
                    return PrevAsciiArt();
            }
            return false;
        }

        bool NextAsciiArt() => ChangeAsciiArt(_asciiArtPages.Length - 1, 1, _asciiArtPages[1]);

        bool PrevAsciiArt() => ChangeAsciiArt(0, -1, _asciiArtPages.Last());

        bool ChangeAsciiArt(int testIndex, int step, AsciiArt overlappingPage)
        {
            // check if there are more pages than 1
            if (_asciiArtPages.Length <= 1) return true;

            // get the index of current page
            int currentPageIndex = Array.IndexOf(_asciiArtPages, Children[0]);
            Children.Clear();

            // pull the next page from array and display it (omit the title page)
            int nextIndex = currentPageIndex + step;
            var newPage = currentPageIndex == testIndex ? overlappingPage :
                nextIndex == 0 ? _asciiArtPages[_asciiArtPages.Length - 1] : _asciiArtPages[nextIndex];
            if (newPage is AnsiArt a)
                a.ResetView();
            Children.Add(newPage);

            // change header title and summary to describe the page
            _header.SetHeader(newPage.Title, newPage.Summary);

            // handled
            return true;
        }
    }

    class AsciiArt : ScreenSurface
    {
        public string Title { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;

        public AsciiArt(Description d) : base(Program.MainWidth, Program.MainHeight)
        {
            // set page descriptions
            Title = d.Title;
            Summary = d.Summary;
        }

        public AsciiArt() : base(Program.MainWidth, Program.MainHeight) { }
    }

    class TitlePage : AsciiArt
    {
        static string[] s_ascii = new[] {
            @"    .                  .-.    .  _   *     _   .",
            @"           *          /   \     ((       _/ \       *    .",
            @"         _    .   .--'\/\_ \     `      /    \  *    ___",
            @"     *  / \_    _/ ^      \/\'__        /\/\  /\  __/   \ *",
            @"       /    \  /    .'   _/  /  \  *' /    \/  \/ .`'\_/\   .",
            @"  .   /\/\  /\/ :' __  ^/  ^/    `--./.'  ^  `-.\ _    _:\ _",
            @"     /    \/  \  _/  \-' __/.' ^ _   \_   .'\   _/ \ .  __/ \",
            @"   /\  .-   `. \/     \ / -.   _/ \ -. `_/   \ /    `._/  ^  \",
            @"  /  `-.__ ^   / .-'.--'    . /    `--./ .-'  `-.  `-. `.  -  `.",
            @"@/        `.  / /      `-.   /  .-'   / .   .'   \    \  \  .-  \%",
            @"@&8jgs@@%% @)&@&(88&@.-_=_-=_-=_-=_-=_.8@% &@&&8(8%@%8)(8@%8 8%@)%",
            @"@88:::&(&8&&8:::::%&`.~-_~~-~~_~-~_~-~~=.'@(&%::::%@8&8)::&#@8::::",
            @"`::::::8%@@%:::::@%&8:`.=~~-.~~-.~~=..~'8::::::::&@8:::::&8:::::'",
            @" `::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::.'"
        };

        public TitlePage() : base()
        {
            Print("'._.-= Several formats can be converted to a SadConsole ScreenObject =-._.'", 1, Color.Green);
            Print("Use arrow keys Left and Right to browse Ansi, REXPaint and Playscii examples.", 4);
            Print("Arrow keys Up and Down to scroll Ansi files.", 6);

            int index = 0;
            foreach (string line in s_ascii)
                Surface.Print(0, 9 + index++, $"       {line}");
        }

        void Print(string s, int y, Color? c = null) => Surface.Print(0, y, s.Align(HorizontalAlignment.Center, Surface.Width), c ?? Surface.DefaultForeground);
    }

    record Description(string Title, string Summary)
    {
        public string FontName { get; set; } = string.Empty;
    }

    class AnsiArt : AsciiArt
    {
        public static readonly Dictionary<string, Description> Descriptions = new()
        {
            { "QS-SIERR.ANS", new Description("ANSI Doc - Sierra BBS", "Advertisement for Sierra BBS by Quick Silver (VALiANT collection 1993).") },
            { "TES-JC.ANS", new Description("ANSI Doc - WILDC.A.T.S", "Advertisement for Jet City BBS by Jim Lee (VALiANT collection 1993).") },
            { "ROY-BTC1.ANS", new Description("ANSI Doc - Blocktronicks", "Art by Roy of SAC. Inspired by Font Designs from Amroth/iCE.") }, 
            { "ROY-DGZN.ANS", new Description("ANSI Doc - Digital Zone", "Advertisement for Digital Zone BBS by Roy of SAC.") },
        };

        public AnsiArt(string ansiFileName) : base(Descriptions[ansiFileName])
        {
            int ansiDocWidth = 80;

            // read the doc once to see how many rows it needs
            var doc = new Document($"Res/Ansi/{ansiFileName}");
            var ansiSurface = new ScreenSurface(ansiDocWidth, 25);
            var writer = new AnsiWriter(doc, ansiSurface.Surface);
            writer.ReadEntireDocument();

            // resize this surface to the doc height
            int totalHeight = ansiSurface.Surface.Height + ansiSurface.Surface.TimesShiftedUp;
            (Surface as CellSurface).Resize(ansiDocWidth, Program.MainHeight, ansiDocWidth, totalHeight, false);

            // read the doc again and give it the surface with the proper height
            writer = new AnsiWriter(doc, Surface);
            writer.ReadEntireDocument();
        }

        public bool ScrollView(Direction d)
        {
            Surface.View = Surface.View.ChangePosition(d);
            return true;
        }

        public void ResetView()
        {
            Surface.View = Surface.View.WithY(0);
        }
    }

    class PlaysciiArt : AsciiArt
    {
        public static readonly Dictionary<string, Description> Descriptions = new()
        {
            { "playscii_welcome_art.psci", new Description("Playscii Art - Editor Welcome Screen",
                "Image displayed in the Playscii editor window when the app starts.") { FontName = "c64_petscii" } },
            { "ink_splashes.psci", new Description("Playscii Art - Ink Splashes",
                "Sample image covering the full width and height of the surface.") { FontName = "c64_petscii" } }
        };

        public PlaysciiArt(string playsciiFileName) : base(Descriptions[playsciiFileName])
        {
            Surface.DefaultBackground = Color.Gray;
            Surface.Clear();

            // get playscii font
            IFont font = GetFont(Descriptions[playsciiFileName].FontName);
            ScreenSurface image = Playscii.ToScreenSurface($"Res/Playscii/{playsciiFileName}", font);

            // get playscii background tile
            //image.Font = GetFont("playscii_background");
            image.Surface.DefaultBackground = Color.DarkGray.GetDarkest();
            image.Surface.Clear();

            AddCentered(image);
        }

        static IFont GetFont(string fontName) => !Game.Instance.Fonts.ContainsKey(fontName) ?
            Game.Instance.LoadFont($"Res/Fonts/{fontName}.font") : Game.Instance.Fonts[fontName];

        void AddCentered(ScreenSurface s)
        {
            float fontSizeRatioX = (float)Font.GetFontSize(IFont.Sizes.One).X / s.Font.GetFontSize(IFont.Sizes.One).X,
                fontSizeRatioY = (float)Font.GetFontSize(IFont.Sizes.One).Y / s.Font.GetFontSize(IFont.Sizes.One).Y;
            int x = Convert.ToInt32((Surface.Width * (1 / fontSizeRatioX) - s.Surface.Width) / 2),
                y = Convert.ToInt32((Surface.Height * fontSizeRatioY - s.Surface.Height) / 2);
            s.Position = (x, y);
            Children.Add(s);
        }
    }
}
