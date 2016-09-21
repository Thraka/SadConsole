using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using System.Runtime.Serialization;

namespace SadConsole.Game
{
    /// <summary>
    /// Groups a <see cref="LayeredTextSurface"/> and a list of <see cref="GameObject"/> types together.
    /// </summary>
    [DataContract]
    public class Scene
    {
        private bool isVisible;

        /// <summary>
        /// The objects for the scene.
        /// </summary>
        [DataMember]
        public GameObjectCollection Objects;

        /// <summary>
        /// The background of the scene.
        /// </summary>
        [DataMember]
        public LayeredTextSurface BackgroundConsole;

        /// <summary>
        /// Width of the backing <see cref="LayeredTextSurface"/>.
        /// </summary>
        public int Width { get { return BackgroundConsole.Width; } }

        /// <summary>
        /// Height of the backing <see cref="LayeredTextSurface"/>.
        /// </summary>
        public int Height { get { return BackgroundConsole.Height; } }


        /// <summary>
        /// Creates a new Scene from an existing <see cref="LayeredTextSurface"/>.
        /// </summary>
        /// <param name="surface">The surface for the scene.</param>
        public Scene(LayeredTextSurface surface)
        {
            BackgroundConsole = surface;
            Objects = new GameObjectCollection();
        }

        public static Scene Load(string file, params Type[] types)
        {
            return SadConsole.Serializer.Load<Scene>(file, types);
        }

        public void Save(string file, params Type[] types)
        {
            SadConsole.Serializer.Save<Scene>(this, file, types);
        }
    }
}
