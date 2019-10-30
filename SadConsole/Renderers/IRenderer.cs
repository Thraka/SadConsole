using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Renderers
{
    public interface IRenderer: IDisposable
    {
        void Attach(Console console);

        void Detatch(Console console);

        void Refresh(Console console);

        void Render();
    }
}
