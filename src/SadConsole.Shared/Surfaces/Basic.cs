using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;

namespace SadConsole.Surfaces
{
    [JsonConverter(typeof(BasicSurfaceJsonConverter))]
    [System.Diagnostics.DebuggerDisplay("Basic Surface")]
    public class Basic: SurfaceBase
    {
        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">THe height of the surface.</param>
        public Basic(int width, int height) : this(width, height, Global.FontDefault, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="viewPort">Initial value for the <see cref="ViewPort"/> view.</param>
        public Basic(int width, int height, Rectangle viewPort) : this(width, height, Global.FontDefault, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        public Basic(int width, int height, Font font) : this(width, height, font, new Rectangle(0, 0, width, height), null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="viewPort">Initial value for the <see cref="ViewPort"/> view.</param>
        public Basic(int width, int height, Font font, Rectangle viewPort) : this(width, height, font, viewPort, null)
        {

        }

        /// <summary>
        /// Creates a new text surface with the specified width, height, and initial set of cell data.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        /// <param name="font">The font used with rendering.</param>
        /// <param name="initialCells">Seeds the cells with existing values. Array size must match <paramref name="width"/> * <paramref name="height"/>.</param>
        /// <param name="viewPort">Initial value for the <see cref="ViewPort"/> view.</param>
        public Basic(int width, int height, Font font, Rectangle viewPort, Cell[] initialCells):base(width, height, font, viewPort, initialCells)
        {
            Renderer = new Renderers.Basic();
        }

        /// <summary>
        /// Saves the <see cref="SurfaceBase"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="SurfaceBase"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static Basic Load(string file) => Serializer.Load<Basic>(file, Settings.SerializationIsCompressed);
    }
}
