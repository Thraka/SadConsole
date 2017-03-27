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
        /// The position of this screen relative to the parents.
        /// </summary>
        Point CalculatedPosition { get; }

        /// <summary>
        /// Child screen objects related to this one.
        /// </summary>
        ScreenCollection Children { get; }

        /// <summary>
        /// A parented screen object.
        /// </summary>
        IScreen Parent { get; set; }

        /// <summary>
        /// Indicates this screen object is visible and should process <see cref="Draw(TimeSpan)"/>.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Indicates the screen object should not process <see cref="Update(TimeSpan)"/>.
        /// </summary>
        bool IsPaused { get; set; }

        /// <summary>
        /// Called for the draw loop.
        /// </summary>
        /// <param name="timeElapsed">Time since last draw.</param>
        void Draw(TimeSpan timeElapsed);

        /// <summary>
        /// Called for the update loop.
        /// </summary>
        /// <param name="timeElapsed">Time since last draw.</param>
        void Update(TimeSpan timeElapsed);

        /// <summary>
        /// Called when the parent position changes.
        /// </summary>
        void OnCalculateRenderPosition();
    }
}
