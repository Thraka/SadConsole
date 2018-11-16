using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    class EditViewScreen : ScreenObject
    {
        private ScreenObject _brush;
        private ScreenObject _editorConsole;

        public ScreenObject Brush
        {
            get => _brush;
            set
            {
                if (_brush != null)
                    Children.Remove(_brush);

                _brush = value;
                Children.Add(_brush);
                Children.MoveToTop(_brush);
            }
        }

        public ScreenObject EditorConsole
        {
            get => _editorConsole;
            set
            {
                if (_editorConsole != null)
                    Children.Remove(_editorConsole);

                _editorConsole = value;
                Children.Add(_editorConsole);
                Children.MoveToBottom(_editorConsole);
            }
        }

    }
}
