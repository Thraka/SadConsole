using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor.Editors
{
    public interface IEditorMetadata
    {
        string Id { get; }
        string Title { get; set; }
        string FilePath { get; set; }
        bool IsLoaded { get; set; }
        bool IsSaved { get; set; }
        FileLoaders.IFileLoader LastLoader { get; set; }
        IEditor Create();
    }
}
