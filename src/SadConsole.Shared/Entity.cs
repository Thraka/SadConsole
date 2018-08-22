using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using SadConsole.Surfaces;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;

namespace SadConsole
{
    /// <summary>
    /// A positionable and animated game object.
    /// </summary>
    [JsonConverter(typeof(EntityJsonConverter))]
    public class Entity : ScreenObject
    {
        /// <summary>
        /// Automatically forwards the <see cref="Animated.AnimationStateChanged"/> event.
        /// </summary>
        public event System.EventHandler<Animated.AnimationStateChangedEventArgs> AnimationStateChanged;

        /// <summary>
        /// Animation for the game object.
        /// </summary>
        protected Animated animation;

        /// <summary>
        /// Font for the game object.
        /// </summary>
        protected Font font;

        /// <summary>
        /// The offset to render this object at.
        /// </summary>
        protected Point positionOffset;

        /// <summary>
        /// A friendly name of the game object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current animation.
        /// </summary>
        public Animated Animation
        {
            get => animation;
            set
            {
                if (animation != null)
                {
                    animation.State = Animated.AnimationState.Deactivated;
                    animation.AnimationStateChanged -= ForwardAnimationStateChanged;
                    Children.Remove(animation);
                }

                animation = value;
                animation.Font = font;

                animation.AnimationStateChanged += ForwardAnimationStateChanged;
                animation.State = Animated.AnimationState.Activated;
                Children.Add(animation);
            }
        }

        /// <summary>
        /// Collection of animations associated with this game object.
        /// </summary>
        public Dictionary<string, Animated> Animations { get; protected set; } = new Dictionary<string, Animated>();

        /// <summary>
        /// Offsets the position by this amount.
        /// </summary>
        public Point PositionOffset { get { return positionOffset; } set { positionOffset = value; OnCalculateRenderPosition(); } }

        /// <summary>
        /// Creates a new GameObject with the default font.
        /// </summary>
        public Entity(int width, int height) : this(width, height, Global.FontDefault) { }

        /// <summary>
        /// Creates a new GameObject.
        /// </summary>
        public Entity(int width, int height, Font font)
        {
            this.font = font;
            Animation = new Animated("default", width, height, Global.FontDefault);
            animation.CreateFrame();
            Animations.Add("default", animation);
        }

        /// <summary>
        /// Creates a new GameObject with a default animation/
        /// </summary>
        /// <param name="animation">The default animation. The animation will have its <see cref="Surfaces.Animated.Name"/> property changesd to "default".</param>
        public Entity(Animated animation)
        {
            font = animation.Font;
            animation.Name = "default";
            Animation = animation;
            Animations.Add("default", animation);
        }
        
        private void ForwardAnimationStateChanged(object sender, Animated.AnimationStateChangedEventArgs e)
        {
            AnimationStateChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Saves the <see cref="GameObject"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="GameObject"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static Entity Load(string file) => Serializer.Load<Entity>(file, Settings.SerializationIsCompressed);
    }

}
