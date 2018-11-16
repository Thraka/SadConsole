using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace Editor
{
    public class SurfaceDocument : IDocument
    {
        protected string filePath;
        protected SadConsole.Console surface;

        public string DisplayTitle { get => $"Surface: {Title}"; }

        public string Title { get => "TEMP 1234567890 1234567890"; set => throw new NotImplementedException(); }

        public string FilePath => filePath;

        public EditorTypes EditorType => EditorTypes.SingleSurface;

        public ScreenObject DrawingScreen => surface;

        public void OnHide()
        {

        }

        public void OnShow()
        {

        }
    }
}
