using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.ViewModel
{
    public class NewDocumentViewModel : ViewModelBase
    {
        public Documents.DocumentTypes[] AllDocumentTypes { get; } = new Documents.DocumentTypes[] { Documents.DocumentTypes.Console, Documents.DocumentTypes.LayeredConsole };

        public Documents.DocumentTypes DocumentType { get; set; }

        public string Name { get; set; }

        public RelayCommand CreateDocumentCommand { get; set; }

        public NewDocumentViewModel()
        {
            CreateDocumentCommand = new RelayCommand(() => { MessengerInstance.Send(new Messages.NewDocumentMessage(DocumentType, Name)); });
        }
    }
}
