using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Represents a texture provided by a game host.
    /// </summary>
    public interface ITexture: IDisposable
    {
        /// <summary>
        /// The file path to the texture.
        /// </summary>
        string ResourcePath { get; }

        /// <summary>
        /// The height of the texture.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The width of the texture.
        /// </summary>
        int Width { get; }
    }
}
