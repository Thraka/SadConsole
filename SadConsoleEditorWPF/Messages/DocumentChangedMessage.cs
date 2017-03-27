using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Messages
{
    public class DocumentChangedMessage : MessageBase
    {
        public DocumentChangedMessage(object sender) : base(sender)
        {
        }

        public DocumentChangedMessage(object sender, object target) : base(sender, target)
        {
        }
    }
}
