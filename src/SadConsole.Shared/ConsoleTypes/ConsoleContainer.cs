using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using SadConsole.Renderers;
using SadConsole.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// An <see cref="IConsole" implementation that only processes the <see cref="IScreen.Children"/>.
    /// </summary>
    public partial class ConsoleContainer : IConsole
    {
        protected IScreen parentScreen;
        protected Point relativePosition;
        protected Point position;

        public bool AutoCursorOnFocus { get; set; }

        public bool CanFocus { get; set; }
        

        public bool IsExclusiveMouse { get; set; }

        public bool IsFocused { get; set; }

        /// <summary>
        /// Indicates this screen object is visible and should process <see cref="Draw(TimeSpan)"/>.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Indicates the screen object should not process <see cref="Update(TimeSpan)"/>.
        /// </summary>
        public bool IsPaused { get; set; }

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

        public Point Position
        {
            get { return position; }
            set { position = value; OnCalculateRenderPosition(); }
        }

        /// <summary>
        /// The position of this screen relative to the parents.
        /// </summary>
        public Point CalculatedPosition { get { return relativePosition; } }

        public ISurface TextSurface { get; set; }

        public bool UseKeyboard { get; set; } = true;

        public bool UseMouse { get; set; } = true;

        public bool UsePixelPositioning { get; set; }

        public Cursor VirtualCursor { get; }

        public ConsoleContainer()
        {
            Children = new ScreenCollection(this);
        }

        public virtual bool ProcessKeyboard(Input.Keyboard state)
        {
            return false;
        }

        public virtual bool ProcessMouse(Input.MouseConsoleState state)
        {
            return false;
        }

        public virtual void LostMouse(Input.MouseConsoleState state) { }

        /// <summary>
        /// Updates the cell effects and cursor. Calls Update on <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if update was called last frame).</param>
        public virtual void Update(TimeSpan delta)
        {
            if (!IsPaused)
            {
                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Update(delta);
            }
        }

        /// <summary>
        /// The <see cref="Renderer"/> will draw the <see cref="TextSurface"/> and then Add a draw call to <see cref="Global.DrawCalls"/>. Calls Draw on <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if draw was called last frame).</param>
        public virtual void Draw(TimeSpan delta)
        {
            if (IsVisible)
            {
                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Draw(delta);
            }
        }

        /// <summary>
        /// Called when the parent position changes.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            relativePosition = Position;
            IScreen parent = parentScreen;

            while (parent != null)
            {
                relativePosition += parent.Position;
                parent = parent.Parent;
            }

            foreach (var child in Children)
            {
                child.OnCalculateRenderPosition();
            }
        }
    }
}
