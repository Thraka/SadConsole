using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Editor.Xaml.Converters;

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
        private DelegateCommand _changeForegroundColorCommand;

        public Color DefaultForeground { get => _defaultForeground; set { _defaultForeground = value; OnPropertyChanged(); } }
        public Color DefaultBackground { get => _defaultBackground; set { _defaultBackground = value; OnPropertyChanged(); } }
        public EditorTypes EditorType { get => _editorType; set => _editorType = value; }

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }

        public Font Font { get => _font; set => _font = value; }

        public ICommand EditFont { get => _editFontCommand; private set => _editFontCommand = value; }
        public ICommand SaveDocument { get => _saveDocumentCommand; private set => _saveDocumentCommand = value; }

        public ICommand ChangeBackground { get => _changeBackgroundColorCommand; }
        public ICommand ChangeForeground { get => _changeForegroundColorCommand; }
        
        public DocumentViewModel()
        {
            _changeBackgroundColorCommand = new DelegateCommand(ChangeBackColor);
            _changeForegroundColorCommand = new DelegateCommand(ChangeForeColor);
            _font = SadConsole.Global.FontDefault;
        }

        public DocumentViewModel(IDocument document)
        {
            _changeBackgroundColorCommand = new DelegateCommand(ChangeBackColor);
            _changeForegroundColorCommand = new DelegateCommand(ChangeForeColor);

            switch (document.EditorType)
            {
                case EditorTypes.SingleSurface:
                    break;
                case EditorTypes.LayeredSurface:
                    break;
                case EditorTypes.GameScene:
                    break;
                case EditorTypes.GameObject:
                    break;
                default:
                    break;
            }
        }

        public IDocument CreateDocument()
        {
            switch (EditorType)
            {
                case EditorTypes.SingleSurface:
                    return new SurfaceDocument();
                case EditorTypes.LayeredSurface:
                    break;
                case EditorTypes.GameScene:
                    break;
                case EditorTypes.GameObject:
                    break;
                default:
                    break;
            }

            return null;
        }

        void ChangeBackColor(object parameter)
        {
            var content = new Xaml.WindowColorPicker();
            var settings = new Xaml.WindowSettings()
            {
                Title = "Color",
                ChildContentDataContext = content,
            };

            content.SelectedColor.Color = DefaultBackground.ToWpfColor();

            settings.CloseWindowCommand = new DelegateCommand(o => 
            {
                bool.TryParse((string)o, out bool result);

                if (result)
                    DefaultBackground = content.SelectedColor.Color.ToMonoColor();

                settings.Window.Hide();
            });

            //settings.CloseWindowCommand = new DelegateCommand(_ => settings.Window.Hide());

            Xaml.WindowBase.Show(content, settings);
        }

        void ChangeForeColor(object parameter)
        {
            var content = new Xaml.WindowColorPicker();
            var settings = new Xaml.WindowSettings()
            {
                Title = "Color",
                ChildContentDataContext = content,
            };

            content.SelectedColor.Color = DefaultForeground.ToWpfColor();

            settings.CloseWindowCommand = new DelegateCommand(o => 
            {
                bool.TryParse((string)o, out bool result);

                if (result)
                    DefaultForeground = content.SelectedColor.Color.ToMonoColor();

                settings.Window.Hide();
            });

            //settings.CloseWindowCommand = new DelegateCommand(_ => settings.Window.Hide());

            Xaml.WindowBase.Show(content, settings);
        }
    }
}
