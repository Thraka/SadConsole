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
        public string DisplayTitle { get => $"Surface: {Title}"; }

        public string Title { get => "TEMP 1234567890 1234567890"; set => throw new NotImplementedException(); }

        public string FilePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public EditorTypes EditorType => EditorTypes.SingleSurface;

        public IScreen DrawingScreen => throw new NotImplementedException();
    }
}
