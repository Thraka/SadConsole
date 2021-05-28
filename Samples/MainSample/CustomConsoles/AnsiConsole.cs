using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace FeatureDemo.CustomConsoles
{
    internal class AnsiConsole : Console
    {
        private int fileIndex = -1;
        private readonly string[] files;
        private Console ansiSurface;

        public AnsiConsole() : base(80, 23)
        {
            IsVisible = false;
            UseKeyboard = true;
            //files = SadConsole.Serializer.Load<string[]>("./ansi/files.json");
            files = System.IO.Directory.GetFiles("./Ansi").Where(f => !f.EndsWith("json")).ToArray();

            NextAnsi();
            LoadAnsi();
        }

        private void NextAnsi()
        {
            fileIndex++;

            if (fileIndex >= files.Length)
            {
                fileIndex = 0;
            }

            doc = new SadConsole.Ansi.Document($"{files[fileIndex]}");
        }

        private SadConsole.Ansi.Document doc;
        private SadConsole.Ansi.AnsiWriter writer;
        private string[] lines;
        private int lineCounter = 0;

        private void LoadAnsi()
        {
            this.Clear();

            ansiSurface = new Console(80, 25);
            writer = new SadConsole.Ansi.AnsiWriter(doc, ansiSurface);
            writer.ReadEntireDocument();

            // TODO: Why did I need to add + 2? Wierd. Something broken with ansi :( :( :(
            this.Resize(80, 23, 80, ansiSurface.Height + ansiSurface.TimesShiftedUp + 2, true);
            this.View = new Rectangle(0, 0, 80, 25);

            writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
            writer = null;
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if (info.IsKeyDown(Keys.Left))
            {
                View = View.WithPosition(new Point(View.Position.X - 1, View.Position.Y));
            }

            if (info.IsKeyDown(Keys.Right))
            {
                View = View.WithPosition(new Point(View.Position.X + 1, View.Position.Y));
            }

            if (info.IsKeyDown(Keys.Up))
            {
                View = View.WithPosition(new Point(View.Position.X, View.Position.Y - 1));
            }

            if (info.IsKeyDown(Keys.Down))
            {
                View = View.WithPosition(new Point(View.Position.X, View.Position.Y + 1));
            }

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
                    this.Clear();
                    lines = doc.AnsiString.Split('\n');
                    writer = new SadConsole.Ansi.AnsiWriter(doc, this);
                }

                writer.AnsiReadLine(lines[lineCounter], true);

                lineCounter++;

                if (lineCounter > lines.Length)
                {
                    writer = null;
                }
            }

            return true;
        }
    }
}
