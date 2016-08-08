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
        int fileIndex;
        string[] files;

        public AnsiConsole(): base(80, 25)
        {
            IsVisible = false;

            files = SadConsole.Serializer.Load<string[]>("./ansi/files.json");

            LoadAnsi();

            KeyboardHandler = (cons, info) =>
            {

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    cons.DataViewport = new Rectangle(cons.DataViewport.X - 1, cons.DataViewport.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    cons.DataViewport = new Rectangle(cons.DataViewport.X + 1, cons.DataViewport.Y, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    cons.DataViewport = new Rectangle(cons.DataViewport.X, cons.DataViewport.Y - 1, 80, 25);

                if (info.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    cons.DataViewport = new Rectangle(cons.DataViewport.X, cons.DataViewport.Y + 1, 80, 25);

                if (info.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space))
                    NextAnsi();

                return true;
            };
        }

        private void NextAnsi()
        {
            fileIndex++;

            if (fileIndex >= files.Length)
                fileIndex = 0;

            LoadAnsi();
        }

        private void LoadAnsi()
        {
            _textSurface.Clear();
            SadConsole.Ansi.Document doc = new SadConsole.Ansi.Document($"./ansi/{files[fileIndex]}");
            SadConsole.Ansi.AnsiWriter writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
            Data = new TextSurface(80, 25 + _textSurface.TimesShiftedUp);
            writer = new SadConsole.Ansi.AnsiWriter(doc, this);
            writer.ReadEntireDocument();
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            


            

            return base.ProcessKeyboard(info);
        }
    }
}
