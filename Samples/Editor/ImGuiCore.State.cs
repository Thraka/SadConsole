using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Editor.GuiParts;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class ImGuiCore
{
    public static class State
    {
        public static int SelectedDocumentIndex = -1;
        public static Model.Document[] OpenDocuments = Array.Empty<Model.Document>();
        public static string[] OpenDocumentTitles => OpenDocuments.Select(d => d.Name).ToArray();

        public static class LayoutInfo
        {
            public static float ColorEditBoxWidth = 20;
            public static int TopBarHeight = 0;
        }
    }
}
