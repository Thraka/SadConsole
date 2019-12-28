using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsoleEditor.FileLoaders;
using SadConsoleEditor.Panels;
using SadConsole.Renderers;

namespace SadConsoleEditor.Editors
{
    public enum Editors
    {
        Console,
        Entity,
        Scene,
        GUI
    }

    public interface IEditor
    {
        SurfaceBase Surface { get; }

        IRenderer Renderer { get; }

        Editors EditorType { get; }

        IEditor LinkedEditor { get; set; }

        string EditorTypeName { get; }

        string Title { get; set; }

        CustomPanel[] Panels { get; }

        int Width { get; }

        int Height { get; }

        string DocumentTitle { get; set; }

        void Draw();

        void Update();

        bool ProcessKeyboard(Keyboard info);

        bool ProcessMouse(MouseConsoleState transformedState, bool isInBounds);

        void New(Color foreground, Color background, int width, int height);

        void Load(string file, IFileLoader loader);

        void Save();

        void Resize(int width, int height);

        void Reset();

        void OnSelected();

        void OnDeselected();

        void OnClosed();
    }
}
