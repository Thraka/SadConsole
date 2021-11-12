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
using System.IO;

namespace FeatureDemo.CustomConsoles
{
    internal class AsciiGraphics : SadConsole.Console, IRestartable
    {
        readonly HeaderConsole _header;
        readonly AsciiArt[] _asciiArtPages;

        public AsciiGraphics(HeaderConsole header) : base(Program.MainWidth, Program.MainHeight)
        {
            _header = header;

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

        bool NextAsciiArt() => ChangeAsciiArt(_asciiArtPages.Length - 1, 1, _asciiArtPages[0]);

        bool PrevAsciiArt() => ChangeAsciiArt(0, -1, _asciiArtPages.Last());

        bool ChangeAsciiArt(int testIndex, int step, AsciiArt overlappingPage)
        {
            // check if there are more pages than 1
            if (_asciiArtPages.Length <= 1) return true;

            // get the index of current page
            int currentPageIndex = Array.IndexOf(_asciiArtPages, Children[0]);
            Children.Clear();

            // pull the next page from array and display it
            int nextIndex = currentPageIndex + step;
            var newPage = currentPageIndex == testIndex ? overlappingPage : _asciiArtPages[nextIndex];
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
                foreach (var child in image.Children)
                    (child as ScreenSurface).FontSize *= modifier;
            }

            // add image to this surface
            AddCentered(image);
        }

        static IFont GetFont(string fontName) => !Game.Instance.Fonts.ContainsKey(fontName) ?
            Game.Instance.LoadFont($"Res/Fonts/{fontName}.font") : Game.Instance.Fonts[fontName];

        void AddCentered(ScreenSurface s)
        {
            Children.Add(s);
            s.UsePixelPositioning = true;
            s.Position = (Settings.Rendering.RenderWidth / 2 - s.AbsoluteArea.Width / 2, (Settings.Rendering.RenderHeight / 2 - s.AbsoluteArea.Height / 2) - 16);
        }
    }
}
