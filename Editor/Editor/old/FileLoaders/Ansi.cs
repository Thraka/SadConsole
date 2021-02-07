using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor.FileLoaders
{
    class Ansi: IFileLoader
    {
        public bool SupportsLoad => true;

        public bool SupportsSave => false;

        public string[] Extensions
        {
            get
            {
                return new string[] { "ans", "ansi" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Ansi";
            }
        }

        public object Load(string file)
        {
            var doc = new SadConsole.Ansi.Document(file);
            var editor = new SadConsole.Surfaces.SurfaceEditor(new SadConsole.Surfaces.NoDrawSurface(80, 1));
            var writer = new SadConsole.Ansi.AnsiWriter(doc, editor);
            writer.ReadEntireDocument();
            editor.TextSurface = new SadConsole.Surfaces.NoDrawSurface(80, 1 + editor.TimesShiftedUp);
            writer = new SadConsole.Ansi.AnsiWriter(doc, editor);
            writer.ReadEntireDocument();
            writer = null;
            var surface = editor.TextSurface;
            editor = null;
            return surface;
        }

        public void Save(object surface, string file)
        {
        }
    }
}
