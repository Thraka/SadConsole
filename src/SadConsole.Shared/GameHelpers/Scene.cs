using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Groups a <see cref="LayeredSurface"/> and a list of <see cref="GameObject"/> types together.
    /// </summary>
    [DataContract]
    public class Scene: LayeredSurface
    {
        /// <summary>
        /// The objects for the scene.
        /// </summary>
        [DataMember]
        public List<GameObject> Objects;

        /// <summary>
        /// Regions defined for the scene.
        /// </summary>
        [DataMember]
        public List<Zone> Zones;

        /// <summary>
        /// Hotspots defined for the scene.
        /// </summary>
        [DataMember]
        public List<Hotspot> Hotspots;

        /// <summary>
        /// Width of the backing <see cref="LayeredSurface"/>.
        /// </summary>
        public int Width { get { return BackgroundSurface.Width; } }

        /// <summary>
        /// Height of the backing <see cref="LayeredSurface"/>.
        /// </summary>
        public int Height { get { return BackgroundSurface.Height; } }

        /// <summary>
        /// Access to the backing console that is internally wrapped in this scene.
        /// </summary>
        public Console BackingConsole { get { return baseConsole; } }

        #region CONSOLE
        public Point Position
        {
            get
            {
                return ((IConsole)baseConsole).Position;
            }

            set
            {
                ((IConsole)baseConsole).Position = value;
                OnCalculateRenderPosition();
            }
        }

        public ISurface TextSurface
        {
            get
            {
                return backgroundSurface;
            }

            set
            {
            }
        }

        public Cursor VirtualCursor
        {
            get
            {
                return ((IConsole)baseConsole).VirtualCursor;
            }
        }

        public IScreen Parent
        {
            get
            {
                return ((IConsole)baseConsole).Parent;
            }

            set
            {
                ((IConsole)baseConsole).Parent = value;
            }
        }

        public bool UsePixelPositioning
        {
            get
            {
                return ((IConsole)baseConsole).UsePixelPositioning;
            }

            set
            {
                ((IConsole)baseConsole).UsePixelPositioning = value;
            }
        }

        public bool UseKeyboard
        {
            get
            {
                return ((IConsole)baseConsole).UseKeyboard;
            }

            set
            {
                ((IConsole)baseConsole).UseKeyboard = value;
            }
        }

        public bool UseMouse
        {
            get
            {
                return ((IConsole)baseConsole).UseMouse;
            }

            set
            {
                ((IConsole)baseConsole).UseMouse = value;
            }
        }

        public bool IsFocused
        {
            get
            {
                return ((IConsole)baseConsole).IsFocused;
            }

            set
            {
                ((IConsole)baseConsole).IsFocused = value;
            }
        }

        public bool IsExclusiveMouse
        {
            get
            {
                return ((IConsole)baseConsole).IsExclusiveMouse;
            }

            set
            {
                ((IConsole)baseConsole).IsExclusiveMouse = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return ((IConsole)baseConsole).IsVisible;
            }

            set
            {
                ((IConsole)baseConsole).IsVisible = value;
            }
        }

        //public bool DoUpdate
        //{
        //    get
        //    {
        //        return ((IConsole)baseConsole).DoUpdate;
        //    }

        //    set
        //    {
        //        ((IConsole)baseConsole).DoUpdate = value;
        //    }
        //}

        public bool ProcessMouse(MouseConsoleState state)
        {
            return ((IConsole)baseConsole).ProcessMouse(state);
        }

        public bool ProcessKeyboard(Keyboard info)
        {
            return ((IConsole)baseConsole).ProcessKeyboard(info);
        }

        public void Update(TimeSpan updateTime)
        {
            ((IConsole)baseConsole).Update(updateTime);
        }

        public void Draw(TimeSpan renderTime)
        {
            ((IConsole)baseConsole).Draw(renderTime);
        }

        public void LostMouse(MouseConsoleState state)
        {
            ((IConsole)baseConsole).LostMouse(state);
        }
        #endregion

        /// <summary>
        /// Called when the parent position changes.
        /// </summary>
        public virtual void OnCalculateRenderPosition()
        {
            relativePosition = Position;
            IScreen parent = Parent;

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

        /// <summary>
        /// Creates a new Scene from an existing <see cref="LayeredSurface"/>.
        /// </summary>
        /// <param name="surface">The surface for the scene.</param>
        public Scene(LayeredSurface surface)
        {
            baseConsole = new Console(surface);
            backgroundSurface = surface;
            Objects = new List<GameObject>();
            Zones = new List<Zone>();
            Hotspots = new List<Hotspot>();
        }

        public static Scene Load(string file, Console baseConsole = null, params Type[] types)
        {
            var scene = SadConsole.Serializer.Load<Scene>(file, types);

            if (baseConsole == null)
                scene.baseConsole = new Console(scene.backgroundSurface) { Renderer = new Renderers.LayeredSurfaceRenderer() };
            else
                scene.baseConsole = baseConsole;

            return scene;
        }

        public void Save(string file, params Type[] types)
        {
            SadConsole.Serializer.Save<Scene>(this, file, types);
        }

        
    }
}
