using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor.ViewModels
{
    class DocumentViewModel
    {
        private Color _defaultForeground;
        private Color _defaultBackground;
        private EditorTypes _editorType;
        private int _width;
        private int _height;
        private Font _font;
        private ICommand _editFontCommand;
        private ICommand _saveDocumentCommand;

        public Color DefaultForeground { get => _defaultForeground; set => _defaultForeground = value; }
        public Color DefaultBackground { get => _defaultBackground; set => _defaultBackground = value; }
        public EditorTypes EditorType { get => _editorType; set => _editorType = value; }

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }

        public Font Font { get => _font; set => _font = value; }

        public ICommand EditFont { get => _editFontCommand; private set => _editFontCommand = value; }
        public ICommand SaveDocument { get => _saveDocumentCommand; private set => _saveDocumentCommand = value; }


    }
}
