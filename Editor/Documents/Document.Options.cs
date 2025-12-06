using System.Runtime.Serialization;
using SadConsole.Editor.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class Document
{
    [DataMember]
    public DocumentOptions Options = new();

    public class DocumentOptions
    {
        public bool UseToolsWindow { get; set; } = true;

        public bool ToolsWindowShowToolsList { get; set; } = true;

        public bool DisableScrolling { get; set; }

        public bool DrawSelf { get; set; }

        public bool HandleMouseOver { get; set; }

        public bool DisableDefaultMouseOver { get; set; }

        public bool UsePalette { get; set; }
        public bool UseSimpleObjects { get; set; }
        public bool UseZones { get; set; }
    }
}
