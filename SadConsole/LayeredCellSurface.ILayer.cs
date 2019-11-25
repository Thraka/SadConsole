using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// An array of <see cref="ColoredGlyph"/> objects used to represent a 2D surface.
    /// </summary>
    public partial class LayeredScreenObject
    {
        public interface ILayer
        {
            bool IsVisible { get; set; }
            string Name { get; set; }
            CellSurface Surface { get; }
        }

        private class Layer : ILayer
        {
            public string Name { get; set; }

            public bool IsVisible { get; set; }

            public CellSurface Surface { get; }

            public Layer(string name, int width, int height, ColoredGlyph[] cells)
            {
                Surface = new CellSurface(width, height, cells);
                IsVisible = true;
                Name = name;
            }
        }
    }
}
