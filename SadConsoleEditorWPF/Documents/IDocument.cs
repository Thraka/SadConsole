using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Documents
{
    public interface IDocument
    {
        string File { get; set; }

        DocumentTypes DocumentType { get; }

        IScreen PresentationScreen { get; set; }

        FileLoaders.IFileType[] FileTypes { get; set; }

        void Load(FileLoaders.IFileType fileType, string file);

        FileLoaders.IFileType Save();

        void Resize(int width, int height);

        void Reset();

        void OnSelected();

        void OnDeselected();

        void OnClosed();
    }
}
