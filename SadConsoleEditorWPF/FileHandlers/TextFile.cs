using System;
using SadConsole.Consoles;
using System.Linq;

namespace SadConsoleEditor.FileLoaders
{
    class TextFile : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "txt" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "ASCII Text";
            }
        }

        public object Load(string file)
        {
            string[] text = System.IO.File.ReadAllLines(file);
            int maxLineWidth = text.Max(s => s.Length);
            
            SadConsole.Consoles.Console importConsole = new SadConsole.Consoles.Console(maxLineWidth, text.Length);
            importConsole.VirtualCursor.AutomaticallyShiftRowsUp = false;

            foreach (var line in text)
                importConsole.VirtualCursor.Print(line);

            return importConsole.TextSurface;
        }

        public void Save(object surface, string file)
        {
            ITextSurface textSurface = (ITextSurface)surface;
            SurfaceEditor editor = new SurfaceEditor(textSurface);
            string[] lines = new string[textSurface.Height];

            for (int y = 0; y < textSurface.Height; y++)
                lines[y] = editor.GetString(0, y, textSurface.Width);

            System.IO.File.WriteAllLines(file, lines);
        }
    }
}
