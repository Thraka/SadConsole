using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public interface ITexture: IDisposable
    {
        string ResourcePath { get; }

        int Height { get; }

        int Width { get; }
    }
}
