using SadConsole;
using SadConsoleEditor.Editors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor.FileLoaders
{
    public interface IFileLoader
    {
        string Id { get; }
        bool SupportsLoad { get; }
        bool SupportsSave { get; }
        string FileTypeName { get; }
        string[] Extensions { get; }
        object Load(string file);
        bool Save(object surface, string file);
    }
}
