using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Model
{
    public class Document
    {
        public enum Types
        {
            Surface,
            Scene,
            Animation
        }

        public string Name = "Document 1";

        public Types DocumentType;

        public object Object;

        public IDocumentSettings Settings;
    }
}
