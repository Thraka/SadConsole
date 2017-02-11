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
    public partial class Console : SurfaceEditor, IConsole
    {
        //public static IConsole ActiveConsole;
        public static ActiveConsoleStack ActiveConsoles = new ActiveConsoleStack();

        public class ActiveConsoleStack
        {
            private IConsole activeConsole;

            public IConsole Console
            {
                get
                {
                    return activeConsole;
                }
            }
            List<IConsole> consoles;

            internal ActiveConsoleStack()
            {
                consoles = new List<IConsole>();
                activeConsole = null;
            }

            public void Clear() => consoles.Clear();

            public void Push(IConsole console)
            {
                if (console != activeConsole && console != null)
                {
                    activeConsole = console;
                    consoles.Add(console);
                }
            }

            public void Set(IConsole console)
            {
                if (activeConsole == console)
                    return;

                if (consoles.Count != 0)
                    consoles.Remove(consoles.Last());

                Push(console);
            }
            
            public void Pop(IConsole console)
            {
                if (console == activeConsole)
                {
                    consoles.Remove(console);

                    if (consoles.Count != 0)
                        activeConsole = consoles.Last();
                    else
                        activeConsole = null;
                }
                else
                    consoles.Remove(console);
            }

            public static bool operator !=(ActiveConsoleStack left, IConsole right)
            {
                return left.activeConsole != right;
            }

            public static bool operator ==(ActiveConsoleStack left, IConsole right)
            {
                return left.activeConsole == right;
            }
        }
    }
}