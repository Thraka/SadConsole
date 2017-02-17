using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using SadConsole.Renderers;
using SadConsole.Input;
using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace SadConsole
{
    /// <summary>
    /// An <see cref="IConsole" implementation that only processes the <see cref="IScreen.Children"/>.
    /// </summary>
    [DataContract]
    public partial class ConsoleContainer : IConsole
    {
        protected IScreen parentConsole;
        protected bool isVisible = true;


        public bool AutoCursorOnFocus { get; set; }

        public bool CanFocus { get; set; }

        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public bool ExclusiveFocus { get; set; }

        public bool IsFocused { get; set; }

        public bool IsVisible { get { return isVisible; } set { isVisible = value; } }

        public IScreen Parent
        {
            get { return parentConsole; }
            set
            {
                if (parentConsole != value)
                {
                    if (parentConsole == null)
                    {
                        parentConsole = value;
                        parentConsole.Children.Add(this);
                    }
                    else
                    {
                        var oldParent = parentConsole;
                        parentConsole = value;

                        oldParent.Children.Remove(this);

                        if (parentConsole != null)
                            parentConsole.Children.Add(this);
                    }
                }
            }
        }

        public Point Position { get; set; }

        public ISurface TextSurface { get; set; }

        public bool UseKeyboard { get; set; }

        public bool UseMouse { get; set; }

        public bool UsePixelPositioning { get; set; }

        public Cursor VirtualCursor { get; }

        /// <summary>
        /// When false, does not perform the code within the <see cref="Update(TimeSpan)"/> method. Defaults to true.
        /// </summary>
        [DataMember]
        public bool DoUpdate { get; set; } = true;

        /// <summary>
        /// When false, does not perform the code within the <see cref="Draw(TimeSpan)"/> method. Defaults to true.
        /// </summary>
        [DataMember]
        public bool DoDraw { get; set; } = true;

        public virtual bool ProcessKeyboard(Input.Keyboard info)
        {
            return false;
        }

        public virtual bool ProcessMouse(Input.Mouse info)
        {
            return false;
        }

        /// <summary>
        /// Updates the cell effects and cursor. Calls Update on <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">Time difference for this frame (if update was called last frame).</param>
        public virtual void Update(TimeSpan delta)
        {
            if (DoUpdate)
            {
                if (isVisible)
                {
                    ProcessMouse(Global.MouseState);

                    if (Console.ActiveConsoles == this)
                        ProcessKeyboard(Global.KeyboardState);
                }

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
            if (DoDraw && IsVisible)
            {
                var copyList = new List<IScreen>(Children);

                foreach (var child in copyList)
                    child.Draw(delta);
            }
        }
    }
}
