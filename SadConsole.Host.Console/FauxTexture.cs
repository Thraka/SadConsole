using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    class FauxTexture : ITexture
    {
        public string ResourcePath => "Invalid";

        public int Height => 273;

        public int Width => 145;

        public FauxTexture() { }

        public void Dispose()
        {
        }
    }
}
