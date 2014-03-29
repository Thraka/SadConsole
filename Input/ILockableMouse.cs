using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Input
{
    public interface ILockableMouse
    {
        void OnMouseExit(Input.MouseInfo info);
        bool IsMouseOver { get; }
        void ExitMouse();
    }
}
