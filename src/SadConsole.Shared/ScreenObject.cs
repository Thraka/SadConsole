using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;

namespace SadConsole
{
    /// <summary>
    /// An object that can be positioned on the screen and receive updates.
    /// </summary>
    public class ScreenObject
    {
        private ScreenObject _parentScreen;
        private Point _position;
        private bool _isVisible = true;
        private bool _isPaused;

        /// <summary>
        /// The poistion of the screen object.
        /// </summary>
        /// <remarks>This position has no substance.</remarks>
        public Point Position
        {
            get => _position;
            set
            {
                var oldPosition = _position;
                _position = value;
                OnCalculateRenderPosition();
                OnPositionChanged(oldPosition);
            }
        }

        /// <summary>
        /// A position that is based on the <see cref="Parent"/> position.
        /// </summary>
        public Point CalculatedPosition { get; protected set; }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        public bool UsePixelPositioning { get; set; } = false;

        /// <summary>
        /// The child objects of this instance.
        /// </summary>
        public ScreenObjectCollection Children { get; }

        /// <summary>
        /// The parent object that this instance is a child of.
        /// </summary>
        public ScreenObject Parent
        {
            get => _parentScreen;
            set
            {
                if (_parentScreen == value) return;
                if (_parentScreen == null)
                {
                    _parentScreen = value;
                    _parentScreen.Children.Add(this);
                    OnCalculateRenderPosition();
                }
                else
                {
                    var oldParent = _parentScreen;
                    _parentScreen = value;

                    oldParent.Children.Remove(this);

                    _parentScreen?.Children.Add(this);

                    OnCalculateRenderPosition();
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of this object.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged();
            }
        }

        /// <summary>
        /// Gets or sets the paused status of this object.
        /// </summary>
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value) return;
                _isPaused = value;
                OnPausedChanged();
            }
        }

        /// <summary>
        /// Creates a new screen.
        /// </summary>
        public ScreenObject()
        {
            Children = new ScreenObjectCollection(this);
        }

        /// <summary>
        /// Draws all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        public virtual void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                var copyList = new List<ScreenObject>(Children);

                foreach (var child in copyList)
                    child.Draw(timeElapsed);
            }
        }

        /// <summary>
        /// Updates all <see cref="Children"/>.
        /// </summary>
        /// <param name="timeElapsed">Time since the last call.</param>
        /// <remarks>Only processes if <see cref="IsPaused"/> is <see langword="false"/>.</remarks>
        public virtual void Update(TimeSpan timeElapsed)
        {
            if (!IsPaused)
            {
                var copyList = new List<ScreenObject>(Children);

                foreach (var child in copyList)
                    child.Update(timeElapsed);
            }
        }

        /// <summary>
        /// Sets a value for <see cref="CalculatedPosition"/> based on the <see cref="Position"/> of this instance and the <see cref="Parent"/> instance.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            CalculatedPosition = Position;
            ScreenObject parent = Parent;

            //TODO couldn't this just be parent.CalculatedPosition + this.Position and then skip the whole loop?
            while (parent != null)
            {
                CalculatedPosition += parent.Position;
                parent = parent.Parent;
            }

            foreach (var child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }

        /// <summary>
        /// Called when the parent console changes for this console.
        /// </summary>
        /// <param name="oldParent">The previous parent.</param>
        /// <param name="newParent">The new parent.</param>
        protected virtual void OnParentChanged(ScreenObject oldParent, ScreenObject newParent) { }

        /// <summary>
        /// Called when the <see cref="Position" /> property changes.
        /// </summary>
        /// <param name="oldLocation">The location before the change.</param>
        protected virtual void OnPositionChanged(Point oldLocation) { }

        /// <summary>
        /// Called when the visibility of the object changes.
        /// </summary>
        protected virtual void OnVisibleChanged() { }

        /// <summary>
        /// Called when the paused status of the object changes.
        /// </summary>
        protected virtual void OnPausedChanged() { }
    }
}
