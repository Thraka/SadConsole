using SadRogue.Primitives;
using SadConsole;
using SadConsole.Input;
using SadConsoleEditor.FileLoaders;
using SadConsoleEditor.Panels;

namespace SadConsoleEditor.Editors
{
    public interface IEditor
    {
        Tools.ITool SelectedTool { get; }

        Console Surface { get; }

        IEditor LinkedEditor { get; set; }

        IEditorMetadata Metadata { get; }

        CustomPanel[] Panels { get; }

        int Width { get; }

        int Height { get; }

        string DocumentTitle { get; set; }

        void Draw();

        void Update();

        bool ProcessKeyboard(Keyboard info);

        bool ProcessMouse(MouseScreenObjectState transformedState, bool isInBounds);

        void New(Color foreground, Color background, int width, int height);

        void Load(string file, IFileLoader loader);

        bool Save(string file, IFileLoader saver);

        void Resize(int width, int height);

        void Reset();

        void OnSelected();

        void OnDeselected();

        void OnClosed();
    }
}
