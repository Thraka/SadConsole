using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private readonly ObservableCollection<IDocument> _documents;
        private readonly ObservableCollection<Tools.ITool> _tools;
        private IDocument _currentDocument;
        private ICommand _showNewDocument;
        private ICommand _closeDocument;
        private Tools.ITool _currentTool;

        Editor.Xaml.WindowBase window1;

        public string Title => "TEST STRING BINDING";

        public ObservableCollection<IDocument> Documents => _documents;

        public IDocument CurrentDocument { get => _currentDocument; set => _currentDocument = value; }

        public ObservableCollection<Tools.ITool> Tools => _tools;

        public Tools.ITool CurrentTool { get => _currentTool; set => _currentTool = value; }


        public ICommand ShowNewDocument { get => _showNewDocument; private set => _showNewDocument = value; }

        public ICommand CloseDocument { get => _closeDocument; private set => _closeDocument = value; }

        public MainViewModel()
        {
            _documents = new ObservableCollection<IDocument>();
            _tools = new ObservableCollection<Tools.ITool>();

            ShowNewDocument = new DelegateCommand(_ =>
            {
                var content = new Xaml.WindowNewDocument();
                var settings = new Xaml.WindowSettings()
                {
                    Title = "Create new document",
                    ChildContentDataContext = new DocumentViewModel(),
                };

                settings.CloseWindowCommand = new DelegateCommand(o =>
                {
                    bool.TryParse((string)o, out bool result);

                    if (result)
                    {
                        _documents.Add(((DocumentViewModel)settings.ChildContentDataContext).CreateDocument());
                    }

                    settings.Window.Hide();
                });

                Xaml.WindowBase.Show(new Xaml.WindowNewDocument(), settings);
                //window1 = new Xaml.WindowBase();
                //window1.Width = 200;
                //window1.Height = 200;
                //((Noesis.Grid)SadConsole.EditorGameComponent.noesisGUIWrapper.ControlTreeRoot).Children.Add(window1);
            });
        }
    }
}
