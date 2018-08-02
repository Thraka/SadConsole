using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using Console = SadConsole.Console;
using SadConsole.Input;
using System.Linq;

namespace StarterProject.CustomConsoles
{
    class AnsiConsole: Console
    {
        int fileIndex = -1;
        string[] files;
        private Basic ansiSurface;
        
        public AnsiConsole(): base(80, 23)
        {
            IsVisible = false;
            UseKeyboard = true;
            //files = SadConsole.Serializer.Load<string[]>("./ansi/files.json");
            files = System.IO.Directory.GetFiles("./ansi").Where(f => !f.EndsWith("json")).ToArray();

            NextAnsi();
            LoadAnsi();

            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Keys.Left))
                    cons.ViewPort = new Rectangle(cons.ViewPort.Left - 1, cons.ViewPort.Top, 80, 23);

                if (info.IsKeyDown(Keys.Right))
                    cons.ViewPort = new Rectangle(cons.ViewPort.Left + 1, cons.ViewPort.Top, 80, 23);

                if (info.IsKeyDown(Keys.Up))
                    cons.ViewPort = new Rectangle(cons.ViewPort.Left, cons.ViewPort.Top - 1, 80, 23);

                if (info.IsKeyDown(Keys.Down))
                    cons.ViewPort = new Rectangle(cons.ViewPort.Left, cons.ViewPort.Top + 1, 80, 23);

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
            };
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

            ansiSurface = new Basic(80, 25);
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
            return base.ProcessKeyboard(info);
        }
    }
}
