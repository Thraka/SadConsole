using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Tools
{
    interface ITool
    {
        string ID { get; }
        string Name { get; }
    }
}
