using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    public class GameObject
    {
        protected Consoles.TextSurfaceRenderer renderer;
        protected bool repositionRects;

        /// <summary>
        /// Where the console should be located on the screen.
        /// </summary>
        protected Point position;

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { Point previousPosition = position; position = value; if (repositionRects) OffsetRects(value); OnPositionChanged(previousPosition); }
        }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get; set; } = false;

        /// <summary>
        /// The current animation.
        /// </summary>
        public Consoles.AnimatedTextSurface Animation { get; set; }

        /// <summary>
        /// When false, this <see cref="GameObject"/> won't be rendered.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// When true, the position of the game object will offset all of the surface rects instead of using a positioning matrix for rendering.
        /// </summary>
        public bool RepositionRects
        {
            get { return repositionRects; }
            set
            {
                repositionRects = value;
                if (value)
                    OffsetRects(position);
                else
                    OffsetRects(position, true);
            }
        }

        /// <summary>
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }

        protected void OffsetRects(Point position, bool force = false)
        {
            if (repositionRects || force)
            {

            }
        }

        /// <summary>
        /// Creates a new GameObject.
        /// </summary>
        public GameObject()
        {
            renderer = new Consoles.TextSurfaceRenderer();
        }

        /// <summary>
        /// Draws the game object.
        /// </summary>
        public void Render()
        {
            if (IsVisible)
            {
                renderer.Render(Animation, position, UsePixelPositioning);   
            }
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        public void Update()
        {
            Animation.Update();
        }
    }
}
