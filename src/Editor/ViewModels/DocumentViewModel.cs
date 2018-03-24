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
    class DocumentViewModel: ViewModelBase
    {
        private Color _defaultForeground;
        private Color _defaultBackground = Color.Magenta;
        private EditorTypes _editorType;
        private int _width;
        private int _height;
        private Font _font;
        private ICommand _editFontCommand;
        private ICommand _saveDocumentCommand;
        private DelegateCommand _changeBackgroundColorCommand;

        public Color DefaultForeground { get => _defaultForeground; set { _defaultForeground = value; OnPropertyChanged(); } }
        public Color DefaultBackground { get => _defaultBackground; set { _defaultBackground = value; OnPropertyChanged(); } }
        public EditorTypes EditorType { get => _editorType; set => _editorType = value; }

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }

        public Font Font { get => _font; set => _font = value; }

        public ICommand EditFont { get => _editFontCommand; private set => _editFontCommand = value; }
        public ICommand SaveDocument { get => _saveDocumentCommand; private set => _saveDocumentCommand = value; }

        public ICommand ChangeBackground { get => _changeBackgroundColorCommand; }


        private Xaml.BindableColor _selectedColor = new Xaml.BindableColor() { Color = Noesis.Colors.DarkTurquoise };
        public Xaml.BindableColor SelectedColor { get => _selectedColor; set { _selectedColor = value; OnPropertyChanged(); } }

        public DocumentViewModel()
        {
            _changeBackgroundColorCommand = new DelegateCommand(ChangeBackColor);
        }

        void ChangeBackColor(object parameter)
        {
            Xaml.WindowBase.Show(new Xaml.WindowColorPicker(), new Xaml.WindowSettings() { Title = "COLOR PICKERS", ChildContentDataContext = this });
        }
    }
}
