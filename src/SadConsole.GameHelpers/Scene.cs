#if SFML
using Point = SFML.System.Vector2i;
using Rectangle = SFML.Graphics.IntRect;
using Texture2D = SFML.Graphics.Texture;
using SFML.Graphics;
#elif MONOGAME
using Microsoft.Xna.Framework;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.Game
{
    /// <summary>
    /// Groups a <see cref="LayeredTextSurface"/> and a list of <see cref="GameObject"/> types together.
    /// </summary>
    [DataContract]
    public class Scene: IConsole
    {
        protected bool isVisible;
        protected Console baseConsole;
        [DataMember( Name = "BackgroundSurface")]
        protected LayeredTextSurface backgroundSurface;


        /// <summary>
        /// The objects for the scene.
        /// </summary>
        [DataMember]
        public GameObjectCollection Objects;

        /// <summary>
        /// The background of the scene.
        /// </summary>
        public LayeredTextSurface BackgroundSurface { get { return backgroundSurface; } set { backgroundSurface = value; baseConsole.TextSurface = value; } }

        /// <summary>
        /// Regions defined for the scene.
        /// </summary>
        [DataMember]
        public List<Zone> Zones;

        /// <summary>
        /// Width of the backing <see cref="LayeredTextSurface"/>.
        /// </summary>
        public int Width { get { return BackgroundSurface.Width; } }

        /// <summary>
        /// Height of the backing <see cref="LayeredTextSurface"/>.
        /// </summary>
        public int Height { get { return BackgroundSurface.Height; } }

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
            }
        }

        public ITextSurfaceRendered TextSurface
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

        public IConsoleList Parent
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

        public bool AutoCursorOnFocus
        {
            get
            {
                return ((IConsole)baseConsole).AutoCursorOnFocus;
            }

            set
            {
                ((IConsole)baseConsole).AutoCursorOnFocus = value;
            }
        }

        public bool CanUseKeyboard
        {
            get
            {
                return ((IConsole)baseConsole).CanUseKeyboard;
            }

            set
            {
                ((IConsole)baseConsole).CanUseKeyboard = value;
            }
        }

        public bool CanUseMouse
        {
            get
            {
                return ((IConsole)baseConsole).CanUseMouse;
            }

            set
            {
                ((IConsole)baseConsole).CanUseMouse = value;
            }
        }

        public bool CanFocus
        {
            get
            {
                return ((IConsole)baseConsole).CanFocus;
            }

            set
            {
                ((IConsole)baseConsole).CanFocus = value;
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

        public bool ExclusiveFocus
        {
            get
            {
                return ((IConsole)baseConsole).ExclusiveFocus;
            }

            set
            {
                ((IConsole)baseConsole).ExclusiveFocus = value;
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

        public bool DoUpdate
        {
            get
            {
                return ((IConsole)baseConsole).DoUpdate;
            }

            set
            {
                ((IConsole)baseConsole).DoUpdate = value;
            }
        }

        public bool ProcessMouse(MouseInfo info)
        {
            return ((IConsole)baseConsole).ProcessMouse(info);
        }

        public bool ProcessKeyboard(KeyboardInfo info)
        {
            return ((IConsole)baseConsole).ProcessKeyboard(info);
        }

        public void Update()
        {
            ((IConsole)baseConsole).Update();
        }

        public void Render()
        {
            ((IConsole)baseConsole).Render();
        }
        #endregion

        /// <summary>
        /// Creates a new Scene from an existing <see cref="LayeredTextSurface"/>.
        /// </summary>
        /// <param name="surface">The surface for the scene.</param>
        public Scene(LayeredTextSurface surface)
        {
            baseConsole = new Console(surface);
            backgroundSurface = surface;
            Objects = new GameObjectCollection();
            Zones = new List<Zone>();
        }

        public static Scene Load(string file, Console baseConsole = null, params Type[] types)
        {
            var scene = SadConsole.Serializer.Load<Scene>(file, types);

            if (baseConsole == null)
                scene.baseConsole = new Console(scene.backgroundSurface);

            return scene;
        }

        public void Save(string file, params Type[] types)
        {
            SadConsole.Serializer.Save<Scene>(this, file, types);
        }
        
    }
}
