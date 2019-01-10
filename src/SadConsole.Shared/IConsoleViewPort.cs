#if XNA
using Microsoft.Xna.Framework;
#endif

namespace SadConsole
{
    /// <summary>
    /// A <see cref="Rectangle"/> for viewing a subset of cells from a <see cref="Console"/>.
    /// </summary>
    public interface IConsoleViewPort
    {
        /// <summary>
        /// The current view of the parent object.
        /// </summary>
        Rectangle ViewPort { get; set; }

        /// <summary>
        /// The maximum width outside of the view port size.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The maximum height outside of the view port size.
        /// </summary>
        int Height { get; }
    }
}
