using SadConsole.Consoles;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterProject.CustomConsoles
{
    class ViewsAndSubViews: SadConsole.Consoles.CustomConsole
    {
        public ViewsAndSubViews() : base(40, 25)
        {
            // A smaller console. We'll render/process this one AND a sub view of it.
        }
    }
}
