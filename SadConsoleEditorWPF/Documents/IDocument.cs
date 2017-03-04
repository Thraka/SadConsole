using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Documents
{
    interface IDocument
    {
        string DocumentType { get; set; }

        string File { get; set; }

        IScreen PresentationScreen { get; set; }

        void Load();

        void Save();

        void Resize(int width, int height);

        void Reset();

        void OnSelected();

        void OnDeselected();

        void OnClosed();
    }
}
