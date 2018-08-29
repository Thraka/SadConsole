using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using SadConsole.Renderers;
using SadConsole.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using Keyboard = SadConsole.Input.Keyboard;

namespace SadConsole
{
    /// <summary>
    /// An <see cref="IConsole" implementation that only processes the <see cref="IScreenObject.Children"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Console (Container Only)")]
    public partial class ConsoleContainer : BasicNoDraw, IConsole
    {
        public ConsoleContainer() : base(1, 1)
        {
        }

        public Cursor Cursor { get; }
        public bool IsExclusiveMouse { get; set; }
        public bool IsFocused { get; set; }
        public bool UseKeyboard { get; set; }
        public bool UseMouse { get; set; }
        public virtual bool ProcessMouse(MouseConsoleState state)
        {
            return false;
        }

        public virtual bool ProcessKeyboard(Keyboard info)
        {
            return false;
        }

        public void LostMouse(MouseConsoleState state)
        {
        }
    }
}
