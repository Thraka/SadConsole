using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    public interface IRenderer: IDisposable
    {
        void Attach(ScreenObjectSurface surfaceObject);

        void Detatch(ScreenObjectSurface surfaceObject);

        void Refresh(ScreenObjectSurface surfaceObject);

        void Render(ScreenObjectSurface surfaceObject);
    }
}
