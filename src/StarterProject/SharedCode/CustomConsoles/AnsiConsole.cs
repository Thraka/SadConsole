using System;
using SadConsole;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{
    class AnsiConsole: Console
    {
        int fileIndex = -1;
        string[] files;

        public AnsiConsole(): base(80, 25)
        {
            IsVisible = false;

            files = SadConsole.Serializer.Load<string[]>("./ansi/files.json");

            NextAnsi();
            LoadAnsi();

            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.X - 1, cons.TextSurface.RenderArea.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.X + 1, cons.TextSurface.RenderArea.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.X, cons.TextSurface.RenderArea.Y - 1, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    cons.TextSurface.RenderArea = new Rectangle(cons.TextSurface.RenderArea.X, cons.TextSurface.RenderArea.Y + 1, 80, 25);

                if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    NextAnsi();
                    LoadAnsi();
                }

                if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.L))
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

            doc = new SadConsole.Ansi.Document($"./ansi/{files[fileIndex]}");
        }

        private SadConsole.Ansi.Document doc;
        private SadConsole.Ansi.AnsiWriter writer;
        private string[] lines;
        private int lineCounter = 0;

        private void LoadAnsi()
        {
            Clear();
            writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
            TextSurface = new TextSurface(80, 25 + TimesShiftedUp);
            writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
            textSurface.RenderArea = new Rectangle(0, 0, 80, 25);
            writer = null;
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            


            

            return base.ProcessKeyboard(info);
        }
    }
}
