using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;

namespace SadConsoleEditor.Messages
{
    public class NewDocumentMessage : MessageBase
    {
        public Documents.DocumentTypes DocumentType;
        public string Name;

        public NewDocumentMessage(Documents.DocumentTypes documentType, string name) :base()
        {
            DocumentType = documentType;
            Name = name;
        }

        public NewDocumentMessage(object sender, Documents.DocumentTypes documentType, string name) : base(sender)
        {
            DocumentType = documentType;
            Name = name;
        }

        public NewDocumentMessage(object sender, object target, Documents.DocumentTypes documentType, string name) : base(sender, target)
        {
            DocumentType = documentType;
            Name = name;
        }
    }
}
