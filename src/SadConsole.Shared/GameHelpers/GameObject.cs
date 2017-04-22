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
    public class GameObject : Screen
    {
        /// <summary>
        /// Automatically forwards the <see cref="AnimatedSurface.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<AnimatedSurface.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Renderer used for drawing the game object.
        /// </summary>
        protected Renderers.ISurfaceRenderer renderer;

        /// <summary>
        /// Pixel positioning flag for position.
        /// </summary>
        protected bool usePixelPositioning;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected AnimatedSurface animation;

        /// <summary>
        /// Font for the game object.
        /// </summary>
        protected Font font;

        /// <summary>
        /// The offset to render this object at.
        /// </summary>
        protected Point positionOffset;

        /// <summary>
        /// Treats the <see cref="IScreen.Position"/> of the game object as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get { return usePixelPositioning; } set { usePixelPositioning = value; } }

        /// <summary>
        /// A friendly name of the game object.
        /// </summary>
        public string Name { get; set; }

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

                animation.AnimationStateChanged += ForwardAnimationStateChanged;
                animation.State = AnimatedSurface.AnimationState.Activated;

            }
        }

        /// <summary>
        /// Collection of animations associated with this game object.
        /// </summary>
        public Dictionary<string, AnimatedSurface> Animations { get; protected set; } = new Dictionary<string, AnimatedSurface>();

        /// <summary>
        /// Offsets the position by this amount.
        /// </summary>
        public Point PositionOffset { get { return positionOffset; } set { positionOffset = value; OnCalculateRenderPosition(); } }

        /// <summary>
        /// Creates a new GameObject with the default font.
        /// </summary>
        public GameObject(int width, int height) : this(width, height, Global.FontDefault) { }

        /// <summary>
        /// Creates a new GameObject.
        /// </summary>
        public GameObject(int width, int height, Font font)
        {
            renderer = new Renderers.SurfaceRenderer();
            this.font = font;
            Animation = new AnimatedSurface("default", width, height, Global.FontDefault);
            animation.CreateFrame();
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new GameObject with a default animation/
        /// </summary>
        /// <param name="animation">The default animation. The animation will have its <see cref="Surfaces.AnimatedSurface.Name"/> property changesd to "default".</param>
        public GameObject(AnimatedSurface animation)
        {
            renderer = new Renderers.SurfaceRenderer();
            font = animation.Font;
            animation.Name = "default";
            Animation = animation;
            Animations.Add("default", animation);
        }


        private void ForwardAnimationStateChanged(object sender, AnimatedSurface.AnimationStateChangedEventArgs e)
        {
            AnimationStateChanged?.Invoke(sender, e);
        }

        public override void OnCalculateRenderPosition()
        {
            calculatedPosition = Position + PositionOffset;
            IScreen parent = Parent;

            while (parent != null)
            {
                calculatedPosition += parent.Position;

                parent = parent.Parent;
            }

            foreach (var child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Renders the game object and any attached children.
        /// </summary>
        /// <param name="timeElapsed">The time since the last call.</param>
        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                renderer.Render(animation);
                
                Global.DrawCalls.Add(new DrawCallSurface(animation, calculatedPosition - animation.Center, usePixelPositioning));

                base.Draw(timeElapsed);
            }
        }

        /// <summary>
        /// Updates the game object animation.
        /// </summary>
        /// <param name="timeElapsed">The time since the last call.</param>
        public override void Update(TimeSpan timeElapsed)
        {
            if (!IsPaused)
            {
                animation.Update();

                base.Update(timeElapsed);
            }
        }

        /// <summary>
        /// Saves the <see cref="GameObject"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save((SerializedTypes.GameObjectSerialized)this, file);
        }

        /// <summary>
        /// Loads a <see cref="GameObject"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static GameObject Load(string file)
        {
            return Serializer.Load<SerializedTypes.GameObjectSerialized>(file);
        }
    }

}
