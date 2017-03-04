using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace SadConsoleEditor.Documents
{
    class Console : IDocument
    {
        public string File { get; set; }

        public IScreen PresentationScreen { get; set; }

        public string DocumentType { get; set; } = "Console";

        public void Load()
        {
            
        }

        public void Save()
        {

        }

        public void OnClosed()
        {
            
        }

        public void OnDeselected()
        {
            
        }

        public void OnSelected()
        {
            
        }

        public void Reset()
        {
            
        }

        public void Resize(int width, int height)
        {
            
        }

        
    }
}
