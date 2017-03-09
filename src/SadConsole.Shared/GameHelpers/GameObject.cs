using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using SadConsole.Surfaces;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    public partial class GameObject: ISurface
    {
        /// <summary>
        /// Automatically forwards the <see cref="AnimatedTextSurface.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<AnimatedSurface.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Renderer used for drawing the game object.
        /// </summary>
        protected Renderers.ISurfaceRenderer renderer;

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
        protected AnimatedSurface animation;

        /// <summary>
        /// Font for the game object.
        /// </summary>
        protected Font font;

        /// <summary>
        /// Gets the name of this animation.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Font for the game object.
        /// </summary>
        public Font Font
        {
            get { return font; }
            set { font = value; UpdateRects(position, true); }
        }

        /// <summary>
        /// An offset of where the object is rendered.
        /// </summary>
        protected Point renderOffset;

        /// <summary>
        /// Renderer used to draw the animation of the game object to the screen.
        /// </summary>
        public Renderers.ISurfaceRenderer Renderer { get { return renderer; } set { renderer = value; } }

        /// <summary>
        /// Offset applied to drawing the game object.
        /// </summary>
        public Point RenderOffset
        {
            get { return renderOffset; }
            set { renderOffset = value; UpdateRects(position); }
        }

        /// <summary>
        /// Gets or sets the position to render the cells.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { Point previousPosition = position; position = value; UpdateRects(value); OnPositionChanged(previousPosition); }
        }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get { return usePixelPositioning; } set { usePixelPositioning = value; UpdateRects(position); } }

        /// <summary>
        /// Indicates the surface has changed and needs to be rendered.
        /// </summary>
        public bool IsDirty { get; set; } = true;

        /// <summary>
        /// The last texture render pass for this surface.
        /// </summary>
        public RenderTarget2D LastRenderResult { get; set; }

        /// <summary>
        /// The current animation.
        /// </summary>
        public AnimatedSurface Animation
        {
            get { return animation; }
            set
            {
                if (animation != null)
                {
                    animation.State = AnimatedSurface.AnimationState.Deactivated;
                    animation.AnimationStateChanged -= ForwardAnimationStateChanged;
                }

                animation = value;
                animation.Font = font;
                UpdateRects(position, true);

                animation.AnimationStateChanged += ForwardAnimationStateChanged;
                animation.State = AnimatedSurface.AnimationState.Activated;

            }
        }

        /// <summary>
        /// Collection of animations associated with this game object.
        /// </summary>
        public Dictionary<string, AnimatedSurface> Animations { get; protected set; } = new Dictionary<string, AnimatedSurface>();

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
                if (repositionRects != value)
                {
                    repositionRects = value;
                    UpdateRects(position, true);
                }
            }
        }

        public Rectangle AbsoluteArea { get; set; }

        public Rectangle[] RenderRects { get; set; }

        public Cell[] RenderCells
        {
            get
            {
                return ((ISurface)animation).RenderCells;
            }
        }

        public Color Tint
        {
            get { return ((ISurface)animation).Tint; }
            set { ((ISurface)animation).Tint = value; }
        }

        public Rectangle RenderArea { get; set; }

        public int Width
        {
            get
            {
                return ((ISurface)animation).Width;
            }
        }

        public int Height
        {
            get
            {
                return ((ISurface)animation).Height;
            }
        }

        public Color DefaultBackground
        {
            get
            {
                return ((ISurface)animation).DefaultBackground;
            }

            set
            {
                ((ISurface)animation).DefaultBackground = value;
            }
        }

        public Color DefaultForeground
        {
            get
            {
                return ((ISurface)animation).DefaultForeground;
            }

            set
            {
                ((ISurface)animation).DefaultForeground = value;
            }
        }

        public Cell[] Cells
        {
            get
            {
                return ((ISurface)animation).Cells;
            }
        }

        public Cell this[int x, int y]
        {
            get
            {
                return ((ISurface)animation)[x, y];
            }
        }

        public Cell this[int index]
        {
            get
            {
                return ((ISurface)animation)[index];
            }
        }


        /// <summary>
        /// Creates a new GameObject with the default font.
        /// </summary>
        public GameObject() : this(Global.FontDefault) { }

        /// <summary>
        /// Creates a new GameObject.
        /// </summary>
        public GameObject(Font font)
        {
            renderer = new Renderers.SurfaceRenderer();
            animation = new AnimatedSurface("default", 1, 1, Global.FontDefault);
            var frame = animation.CreateFrame();
            frame[0].Glyph = 1;
            this.font = animation.Font = font;
        }

        private void ForwardAnimationStateChanged(object sender, AnimatedSurface.AnimationStateChangedEventArgs e)
        {
            AnimationStateChanged?.Invoke(sender, e);
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

                if (repositionRects && usePixelPositioning)
                {
                    offset = position + renderOffset - new Point(animation.Center.X * font.Size.X, animation.Center.Y * font.Size.Y);
                }
                else if (repositionRects)
                {
                    offset = position + renderOffset - animation.Center;
                    offset = new Point(offset.X * font.Size.X, offset.Y * font.Size.Y);
                }
                else
                {
                    offset = new Point();
                }

                AbsoluteArea = new Rectangle(offset.X, offset.Y, width * font.Size.X, height * font.Size.Y);

                int index = 0;
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        rects[index] = new Rectangle(x * font.Size.X + offset.X, y * font.Size.Y + offset.Y, font.Size.X, font.Size.Y);
                        index++;
                    }
                }

                RenderRects = rects;
                LastRenderResult = new RenderTarget2D(Global.GraphicsDevice, AbsoluteArea.Width, AbsoluteArea.Height, false, Global.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
                IsDirty = true;
            }

        }

        /// <summary>
        /// Forces the rendering rectangles to update with positioning information.
        /// </summary>
        public void UpdateAnimationRectangles()
        {
            UpdateRects(position, true);
        }

        /// <summary>
        /// Draws the game object.
        /// </summary>
        public virtual void Draw(TimeSpan elapsed)
        {
            if (IsVisible)
            {
                Renderer.Render(this, true);

                //if (repositionRects)
                //    Global.DrawCalls.Add(new DrawCallSurface(this, Vector2.Zero));
                //    //renderer.Render(this, NoMatrix);   
                //else
                    Global.DrawCalls.Add(new DrawCallSurface(this, position + renderOffset - animation.Center, false));
                    //renderer.Render(this, position + renderOffset - animation.Center, usePixelPositioning);

            }
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        public virtual void Update()
        {
            Animation.Update();
        }

        /// <summary>
        /// Saves this <see cref="GameObject"/> to a file.
        /// </summary>
        /// <param name="file">The file to save.</param>
        /// <param name="knownTypes">The type of <see cref="GameObject.Renderer"/>.</param>
        public void Save(string file, params Type[] knownTypes)
        {
            Serializer.Save(this, file, Serializer.KnownTypes.Union(knownTypes).Union(new Type[] { typeof(SerializedTypes.GameObjectSerialized), typeof(AnimatedSurface), typeof(AnimatedSurface[]) }));
        }

        /// <summary>
        /// Loads a <see cref="GameObject"/> from a file.
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <param name="knownTypes">The type of <see cref="GameObject.Renderer"/>.</param>
        /// <returns>A new GameObject.</returns>
        public static GameObject Load(string file, params Type[] knownTypes)
        {
            return Serializer.Load<GameObject>(file, Serializer.KnownTypes.Union(knownTypes));
        }
    }
}
