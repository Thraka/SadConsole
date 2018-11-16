using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.SerializedTypes
{
    public interface ISerialize<out TTarget>
    {
        TTarget Save();
    }
}
