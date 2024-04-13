using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class ImGuiCore
{
    public static class State
    {
        public static bool IsPopupOpen;
        private static ImGuiWindowClass _noTabBarWindowClassPtr;
        public static ImGuiWindowClassPtr NoTabBarWindowClass;

        public static int SelectedDocumentIndex = -1;
        public static Model.Document[] OpenDocuments = [];
        public static string[] OpenDocumentTitles => OpenDocuments.Select(d => d.Name).ToArray();

        public static FileHandlers.IFileHandler[] FileHandlerInstances = [new FileHandlers.SurfaceFile()];

        //public static Dictionary<IFont, Texture2D> FontTexturesPure;

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

            //FontTexturesRebuilt = new();
        }

        public static bool CheckSetPopupOpen(string id) =>
            IsPopupOpen |= ImGui.IsPopupOpen(id);

        public static void SetPopupOpen() =>
            IsPopupOpen = true;

        public static void OpenPopup(string id)
        {
            ImGui.OpenPopup(id);
            IsPopupOpen = true;
        }
        //public Texture2D BuildPureFontTexture(IFont font)
        //{
        //    if (font is SadFont sfont)
        //    {
        //        //RenderTarget2D texture = new(SadConsole.Host.Global.GraphicsDevice, 16 * sfont.GlyphWidth, sfont.TotalGlyphs / 16 * sfont.GlyphHeight);
        //        Texture2D texture = new(SadConsole.Host.Global.GraphicsDevice, 16 * sfont.GlyphWidth, sfont.TotalGlyphs / 16 * sfont.GlyphHeight);
        //        Host.GameTexture wrapper = new(texture, false);
        //        Color[] pixels = wrapper.GetPixels();
        //        Color

        //        for (int i = 0; i < sfont.TotalGlyphs; i++)
        //        {
        //            sfont.edit
        //        }
        //    }
        //}
    }
}
