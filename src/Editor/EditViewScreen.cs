using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    class EditViewScreen : Screen
    {
        private IScreen _brush;
        private IScreen _editorConsole;

        public IScreen Brush
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

        public IScreen EditorConsole
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
