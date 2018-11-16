using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public interface IDocument
    {
        SadConsole.ScreenObject DrawingScreen { get; }

        string Title { get; set; }

        string FilePath { get; }

        EditorTypes EditorType { get; }

        void OnShow();
        void OnHide();
    }
}
