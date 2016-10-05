#if SFML
using Point = SFML.System.Vector2i;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;

namespace SadConsole.Consoles
{
    /// <summary>
    /// Represents a list of consoles. By calling the <see cref="Update"/> or <see cref="Render"/> methods, all contained consoles will be called in order.
    /// </summary>
    public class ConsoleList : IConsoleList
    {
        protected List<IConsole> _consoles;
        
        /// <summary>
        /// The parent console.
        /// </summary>
        public IConsoleList Parent { get; set; } 

        public int Count { get { return _consoles.Count; } }

        public bool IsVisible { get; set; } = true;

        public bool DoUpdate { get; set; } = true;

        public bool CanUseKeyboard { get; set; }

        public bool CanUseMouse { get; set; }

        public bool CanFocus { get; set; }

        public bool IsFocused { get; set; }

        public bool ExclusiveFocus { get; set; }

        public Point Position { get; set; }

        public ITextSurfaceRendered TextSurface
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Cursor VirtualCursor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UsePixelPositioning
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoCursorOnFocus
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IConsole this[int index]
        {
            get { return _consoles[index]; }
            set
            {
                if (_consoles[index] != value)
                {
                    var oldConsole = _consoles[index];
                    _consoles[index] = value;
                    RemoveConsolesParent(oldConsole);
                    SetConsolesParent(value);
                }
            }
        }

        public ConsoleList()
        {
            _consoles = new List<IConsole>();
        }

        public virtual void Render()
        {
            if (IsVisible)
            {
                var copyList = new List<IConsole>(_consoles);

                for (int i = 0; i < copyList.Count; i++)
                {
                    copyList[i].Render();
                }
            }
        }

        public virtual void Update()
        {
            if (DoUpdate)
            {
                var copyList = new List<IConsole>(_consoles);

                for (int i = copyList.Count - 1; i >= 0; i--)
                {
                    copyList[i].Update();
                }
            }
        }

        public virtual bool ProcessKeyboard(Input.KeyboardInfo info)
        {
            if (IsVisible)
            {
                var copyList = new List<IConsole>(_consoles);

                for (int i = copyList.Count - 1; i >= 0; i--)
                {
                    if (copyList[i].ProcessKeyboard(info))
                        return true;
                }
            }

            return false;
        }

        public virtual bool ProcessMouse(Input.MouseInfo info)
        {
            info.Console = null;

            if (IsVisible)
            {
                var copyList = new List<IConsole>(_consoles);

                for (int i = copyList.Count - 1; i >= 0; i--)
                {
                    if (copyList[i].ProcessMouse(info))
                    {
                        //var console = this[i];
                        // TODO: Update this to only move console to front if either setting says to, and\or when console is clicked.
                        // TODO: Console that was true with process mouse should be set as active console. (info.Console)
                        //if (i != Count - 1)
                        //{
                        //    this.Remove(console);
                        //    this.Add(console);
                        //}
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all consoles.
        /// </summary>
        public void Clear()
        {
            _consoles.Clear();
        }

        /// <summary>
        /// Returns true if this console list contains the specified console.
        /// </summary>
        /// <param name="console">The console to search for.</param>
        /// <returns></returns>
        public bool Contains(IConsole console)
        {
            return _consoles.Contains(console);
        }

        public bool IsTopConsole(IConsole console)
        {
            if (_consoles.Contains(console))
                return _consoles.IndexOf(console) == _consoles.Count - 1;
            else
                return false;
        }

        /*
        /// <summary>
        /// Determines if a console can become active instead of this one.
        /// </summary>
        /// <param name="askingConsole">The console making the request</param>
        /// <returns>True</returns>
        public bool CanActiveBeTaken(IConsole askingConsole) { return true; }
        */

        #region IParentConsole

        public void Add(IConsole console)
        {
            if (!_consoles.Contains(console))
                _consoles.Add(console);

            SetConsolesParent(console);
        }

        public void Insert(int index, IConsole console)
        {
            if (!_consoles.Contains(console))
                _consoles.Insert(index, console);

            SetConsolesParent(console);
        }

        public void Remove(IConsole console)
        {
            if (_consoles.Contains(console))
                _consoles.Remove(console);

            RemoveConsolesParent(console);
        }

        public void MoveToTop(IConsole console)
        {
            if (_consoles.Contains(console))
            {
                _consoles.Remove(console);
                _consoles.Add(console);
            }
        }

        public void MoveToBottom(IConsole console)
        {
            if (_consoles.Contains(console))
            {
                _consoles.Remove(console);
                _consoles.Insert(0, console);
            }
        }

        public int IndexOf(IConsole console)
        {
            return _consoles.IndexOf(console);
        }

        public IConsole NextValidConsole(IConsole currentConsole)
        {
            if (_consoles.Contains(currentConsole))
            {
                var index = _consoles.IndexOf(currentConsole);
                var counter = 0;
                do
                {
                    index++;
                    counter++;

                    if (index == _consoles.Count)
                        index = 0;

                    if (_consoles[index].IsVisible)
                    {
                        return _consoles[index];
                    }
                } while (counter < _consoles.Count);
                
            }

            return null;
        }

        public IConsole PreviousValidConsole(IConsole currentConsole)
        {
            if (_consoles.Contains(currentConsole))
            {
                var index = _consoles.IndexOf(currentConsole);
                var counter = 0;
                do
                {
                    index--;
                    counter++;

                    if (index == -1)
                        index = _consoles.Count - 1;

                    if (_consoles[index].IsVisible)
                    {
                        return _consoles[index];
                    }
                } while (counter < _consoles.Count);

            }

            return null;
        }

        #endregion

        private bool SetConsolesParent(IConsole console)
        {
            if (console.Parent != this)
            {
                console.Parent = this;
                return true;
            }

            return false;
        }

        private bool RemoveConsolesParent(IConsole console)
        {
            if (console.Parent == this)
            {
                console.Parent = null;
                return true;
            }

            return false;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _consoles.GetEnumerator();
        }

        IEnumerator<IConsole> IEnumerable<IConsole>.GetEnumerator()
        {
            return _consoles.GetEnumerator();
        }

    }
}
