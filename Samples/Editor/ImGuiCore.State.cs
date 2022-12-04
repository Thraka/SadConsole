using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole;
using SadConsole.Editor.GuiParts;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class ImGuiCore
{
    public static class State
    {
        private static ImGuiWindowClass _noTabBarWindowClassPtr;
        public static ImGuiWindowClassPtr NoTabBarWindowClass;

        public static int SelectedDocumentIndex = -1;
        public static Model.Document[] OpenDocuments = Array.Empty<Model.Document>();
        public static string[] OpenDocumentTitles => OpenDocuments.Select(d => d.Name).ToArray();

        public static Model.Document GetOpenDocument() => OpenDocuments[SelectedDocumentIndex];

        public static class LayoutInfo
        {
            public static float ColorEditBoxWidth = 20;
            public static int TopBarHeight = 0;
        }

        static State()
        {
            unsafe
            {
                fixed(ImGuiWindowClass* pointer = &_noTabBarWindowClassPtr)
                    NoTabBarWindowClass = new ImGuiWindowClassPtr(pointer);

                NoTabBarWindowClass.DockNodeFlagsOverrideSet = (ImGuiDockNodeFlags)(1 << 12)  // ImGuiDockNodeFlags_NoTabBar
                                                             | (ImGuiDockNodeFlags)(1 << 16); // ImGuiDockNodeFlags_NoDocking
                                                             //| (ImGuiDockNodeFlags)(1 << 17); // ImGuiDockNodeFlags_NoDockingSplitMe
            }
        }
    }
}
