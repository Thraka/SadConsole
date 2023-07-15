using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.Entities;

public partial class Entity
{
    /// <summary>
    /// An entity that is a an animated surface.
    /// </summary>
    [DataContract]
    public class Animated
    {
        /// <summary>
        /// The animation associated with this animated entity.
        /// </summary>
        [DataMember]
        public AnimatedScreenSurface Animation { get; }

        /// <summary>
        /// Represents the collision rectangle for this animated surface which is the size of the animation frame.
        /// </summary>
        public Rectangle DefaultCollisionRectangle
        {
            get => new Rectangle(0, 0, Animation.CurrentFrame.ViewWidth, Animation.CurrentFrame.ViewHeight);
        }

        /// <summary>
        /// A collision rectangle that you can specify.
        /// </summary>
        /// <remarks>
        /// This rectangle should be declared without using the animation center. Only apply the center when you're testing for collision and reading this rectangle.
        /// </remarks>
        [DataMember]
        public Rectangle CustomCollisionRectangle { get; set; }

        /// <summary>
        /// When <see langword="true"/>, indicates that this animation is dirty and needs to be redrawn.
        /// </summary>
        public bool IsDirty { get => Animation.IsDirty; set => Animation.IsDirty = value; }

        /// <summary>
        /// Creates a new instance of this type from an animated screen surface.
        /// </summary>
        /// <param name="surface">The animation to use.</param>
        public Animated(AnimatedScreenSurface surface) =>
            Animation = surface;

        /// <summary>
        /// Updates the <see cref="Animation"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since this method was last called.</param>
        public void Update(TimeSpan delta) =>
            Animation.Update(delta);
    }
}
