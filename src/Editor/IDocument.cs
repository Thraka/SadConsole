using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    interface IDocument
    {
        string Title { get; set; }

        string FilePath { get; set; }

        EditorTypes EditorType { get; }
    }
}
