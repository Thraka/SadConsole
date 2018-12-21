using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadConsole
{
    /// <summary>
    /// A <see cref="Rectangle"/> for viewing a subset of cells from a <see cref="Console"/>.
    /// </summary>
    public interface IScreenObjectViewPort
    {
        Rectangle ViewPort { get; set; }

        int Width { get; }

        int Height { get; }
    }
}
