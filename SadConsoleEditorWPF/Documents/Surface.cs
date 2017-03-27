using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace SadConsoleEditor.Documents
{
    class Surface : IDocument
    {
        public string File { get; set; }

        public IScreen PresentationScreen { get; set; }

        public FileLoaders.IFileType[] FileTypes { get; set; } = new FileLoaders.IFileType[] { new FileLoaders.BasicSurface() };

        public DocumentTypes DocumentType { get; } = DocumentTypes.Console;

        public Surface() { Reset(); }

        public void Load(FileLoaders.IFileType fileType, string file)
        {
            fileType.Load(file);
        }

        public FileLoaders.IFileType Save()
        {
            return null;
        }

        public void OnClosed()
        {
            
        }

        public void OnDeselected()
        {
            
        }

        public void OnSelected()
        {
            
        }

        public void Reset()
        {
            PresentationScreen = new SadConsole.Console(50, 50);
            ((SadConsole.Console)PresentationScreen).FillWithRandomGarbage();
        }

        public void Resize(int width, int height)
        {
            
        }
        public override string ToString()
        {

            if (string.IsNullOrEmpty(File))
                return "Unsaved.Surface";
            else
                return System.IO.Path.GetFileName(File);

        }

    }
}
