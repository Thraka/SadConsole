using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Model;
internal interface IDocumentSurface
{
    IScreenSurface Surface { get; }
}
