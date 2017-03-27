using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace SadConsoleEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Documents.IDocument selectedDocument;

        public ObservableCollection<Documents.IDocument> Documents { get; set; }

        public Documents.IDocument SelectedDocument { get
            {
                return selectedDocument;
            }
            set
            {
                RaisePropertyChanged(() => SelectedDocument);
                selectedDocument = value;
                ChangeDocument();
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            Documents = new ObservableCollection<SadConsoleEditor.Documents.IDocument>();

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Messages.NewDocumentMessage>(this, CreateNewDocument);
        }

        public void CreateNewDocument(Messages.NewDocumentMessage message)
        {
            switch (message.DocumentType)
            {
                case SadConsoleEditor.Documents.DocumentTypes.Console:
                    Documents.Add(new Documents.Surface());
                    break;
                default:
                    break;
            }
        }

        private void ChangeDocument()
        {
            //MessengerInstance.Send(new Messages.DocumentChangedMessage(this));
            if (selectedDocument == null)
                SadConsole.Global.CurrentScreen = new SadConsole.Screen();
            else
                SadConsole.Global.CurrentScreen = selectedDocument.PresentationScreen;
        }
    }
}