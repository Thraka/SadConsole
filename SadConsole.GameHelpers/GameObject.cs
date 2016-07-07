using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Game
{
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// A translation matrix of 0, 0, 0.
        /// </summary>
        public static Matrix NoMatrix = Matrix.CreateTranslation(0f, 0f, 0f);

        /// <summary>
        /// Renderer used for drawing the game object.
        /// </summary>
        protected Consoles.TextSurfaceRenderer renderer;

        /// <summary>
        /// Reposition the rects of the animation.
        /// </summary>
        protected bool repositionRects;

        /// <summary>
        /// Pixel positioning flag for position.
        /// </summary>
        protected bool usePixelPositioning;

        /// <summary>
        /// Where the console should be located on the screen.
        /// </summary>
        protected Point position;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected Consoles.AnimatedTextSurface animation;

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { Point previousPosition = position; position = value; if (repositionRects) UpdateRects(value); OnPositionChanged(previousPosition); }
        }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get { return usePixelPositioning; } set { usePixelPositioning = value; UpdateRects(position); } }

        /// <summary>
        /// The current animation.
        /// </summary>
        public Consoles.AnimatedTextSurface Animation { get { return animation; } set { animation = value; UpdateRects(position); } }

        /// <summary>
        /// When false, this <see cref="GameObject"/> won't be rendered.
        /// </summary>
        public bool IsVisible { get; set; } = true;

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
                    UpdateRects(position);
                else
                    UpdateRects(position, true);
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
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }

        /// <summary>
        /// Resets all of the rects of the animation based on <see cref="UsePixelPositioning"/> and if <see cref="RepositionRects"/> is true.
        /// </summary>
        /// <param name="position">The position of the game object.</param>
        /// <param name="force">When true, always repositions rects.</param>
        protected void UpdateRects(Point position, bool force = false)
        {
            if (repositionRects || force)
            {
                var width = Animation.Width;
                var height = Animation.Height;
                var font = Animation.Font;
                Point offset;

                var rects = new Rectangle[width * height];

                if (!repositionRects)
                {
                    offset = new Point(-animation.Center.X * font.Size.X, -animation.Center.Y * font.Size.Y);

                    animation.AbsoluteArea = new Rectangle(offset.X, offset.Y, width * font.Size.X, height * font.Size.Y);
                }
                else if (usePixelPositioning)
                {
                    offset = position + new Point(-animation.Center.X * font.Size.X, -animation.Center.Y * font.Size.Y);
                    animation.AbsoluteArea = new Rectangle(offset.X, offset.Y, width * font.Size.X, height * font.Size.Y);
                }
                else
                {
                    offset = new Point(position.X * font.Size.X, position.Y * font.Size.Y) + new Point(-animation.Center.X * font.Size.X, -animation.Center.Y * font.Size.Y);
                    animation.AbsoluteArea = new Rectangle(offset.X, offset.Y, width * font.Size.X, height * font.Size.Y);
                }

                int index = 0;
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        rects[index] = new Rectangle(x * font.Size.X + offset.X, y * font.Size.Y + offset.Y, font.Size.X, font.Size.Y);
                        index++;
                    }
                }

                animation.RenderRects = rects;
            }
        }

        public void UpdateAnimationRectangles()
        {
            UpdateRects(position, true);
        }
        
        /// <summary>
        /// Draws the game object.
        /// </summary>
        public virtual void Render()
        {
            if (IsVisible)
            {
                if (repositionRects)
                    renderer.Render(Animation, NoMatrix);   
                else
                    renderer.Render(Animation, position, usePixelPositioning);
            }
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        public virtual void Update()
        {
            Animation.Update();
        }
    }
}
