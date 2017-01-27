using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// A visible screen object.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// The top-left coordinate of the screen object.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Child screen objects related to this one.
        /// </summary>
        List<IScreen> Children { get; set; }

        /// <summary>
        /// A parented screen object.
        /// </summary>
        IScreen Parent { get; set; }

        /// <summary>
        /// Indicates this screen object is visible.
        /// </summary>
        bool IsVisible { get; set; }

        void Draw(TimeSpan timeElapsed);

        void Update(TimeSpan timeElapsed);
    }
}
