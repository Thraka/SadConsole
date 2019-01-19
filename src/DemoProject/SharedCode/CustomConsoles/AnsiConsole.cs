using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


using ScrollingConsole = SadConsole.ScrollingConsole;
using SadConsole.Input;
using System.Linq;

namespace StarterProject.CustomConsoles
{
    class AnsiConsole: ScrollingConsole
    {
        int fileIndex = -1;
        string[] files;
        private ScrollingConsole ansiSurface;
        
        public AnsiConsole(): base(80, 23)
        {
            IsVisible = false;
            UseKeyboard = true;
            //files = SadConsole.Serializer.Load<string[]>("./ansi/files.json");
            files = System.IO.Directory.GetFiles("./ansi").Where(f => !f.EndsWith("json")).ToArray();

            NextAnsi();
            LoadAnsi();
        }

        private void NextAnsi()
        {
            fileIndex++;

            if (fileIndex >= files.Length)
                fileIndex = 0;

            doc = new SadConsole.Ansi.Document($"{files[fileIndex]}");
        }

        private SadConsole.Ansi.Document doc;
        private SadConsole.Ansi.AnsiWriter writer;
        private string[] lines;
        private int lineCounter = 0;

        private void LoadAnsi()
        {
            Clear();

            ansiSurface = new ScrollingConsole(80, 25);
            writer = new SadConsole.Ansi.AnsiWriter(doc, ansiSurface);
            writer.ReadEntireDocument();

            Resize(80, ansiSurface.Height + ansiSurface.TimesShiftedUp, true);
            ViewPort = new Rectangle(0, 0, 80, 25);

            writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
            writer = null;
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if (info.IsKeyDown(Keys.Left))
                ViewPort = new Rectangle(ViewPort.Left - 1, ViewPort.Top, 80, 23);

            if (info.IsKeyDown(Keys.Right))
                ViewPort = new Rectangle(ViewPort.Left + 1, ViewPort.Top, 80, 23);

            if (info.IsKeyDown(Keys.Up))
                ViewPort = new Rectangle(ViewPort.Left, ViewPort.Top - 1, 80, 23);

            if (info.IsKeyDown(Keys.Down))
                ViewPort = new Rectangle(ViewPort.Left, ViewPort.Top + 1, 80, 23);

            if (info.IsKeyReleased(Keys.Space))
            {
                NextAnsi();
                LoadAnsi();
            }

            if (info.IsKeyReleased(Keys.L))
            {
                if (writer == null || lineCounter == lines.Length)
                {
                    NextAnsi();
                    lineCounter = 0;
                    Clear();
                    lines = doc.AnsiString.Split('\n');
                    writer = new SadConsole.Ansi.AnsiWriter(doc, this);
                }

                writer.AnsiReadLine(lines[lineCounter], true);

                lineCounter++;

                if (lineCounter > lines.Length)
                    writer = null;
            }

            return true;
        }
    }
}
