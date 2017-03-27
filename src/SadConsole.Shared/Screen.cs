using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    /// <summary>
    /// A basic implementation of <see cref="IScreen"/>.
    /// </summary>
    public class Screen: IScreen
    {
        /// <summary>
        /// Position to render the screen at based on <see cref="Position"/> all parents.
        /// </summary>
        protected Point calculatedPosition;

        /// <summary>
        /// The parent screen.
        /// </summary>
        protected IScreen parentScreen;

        /// <summary>
        /// The position of the console.
        /// </summary>
        protected Point position;

        /// <summary>
        /// The position of the console.
        /// </summary>
        public Point Position
        {
            get { return position; }
            set { position = value; OnCalculateRenderPosition(); }
        }

        /// <summary>
        /// The position of this screen relative to the parents.
        /// </summary>
        public Point CalculatedPosition { get { return calculatedPosition; } }

        /// <summary>
        /// Child screen objects.
        /// </summary>
        public ScreenCollection Children { get; }

        /// <summary>
        /// Gets or sets the Parent screen.
        /// </summary>
        public IScreen Parent
        {
            get { return parentScreen; }
            set
            {
                if (parentScreen != value)
                {
                    if (parentScreen == null)
                    {
                        parentScreen = value;
                        parentScreen.Children.Add(this);
                        OnCalculateRenderPosition();
                    }
                    else
                    {
                        var oldParent = parentScreen;
                        parentScreen = value;

                        oldParent.Children.Remove(this);

                        if (parentScreen != null)
                            parentScreen.Children.Add(this);

                        OnCalculateRenderPosition();
                    }
                }
            }
        }

        /// <summary>
        /// Indicates this screen object is visible and should process <see cref="Draw(TimeSpan)"/>.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Indicates the screen object should not process <see cref="Update(TimeSpan)"/>.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Creates a new screen.
        /// </summary>
        public Screen()
        {
            Children = new ScreenCollection(this);
        }

        /// <summary>
        /// Draws all children.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        public virtual void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Draw(timeElapsed);
            }
        }

        /// <summary>
        /// Updates all children.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        public virtual void Update(TimeSpan timeElapsed)
        {
            if (!IsPaused)
            {
                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Update(timeElapsed);
            }
        }

        /// <summary>
        /// Called when the parent position changes.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            calculatedPosition = Position;
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
    }
}
