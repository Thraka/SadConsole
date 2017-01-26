using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Shapes
{
    public interface IShape
    {
        void Draw(SurfaceEditor surface);
    }

    // TODO: Ideas for more shapes: Circle, AdvancedLine (like line but allows inter connected links based on direction of previous and next link)
}
