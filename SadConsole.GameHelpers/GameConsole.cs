using SadConsole.Consoles;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    [DataContract]

    public class GameConsole : Consoles.Console
    {
        protected LayeredTextSurface layeredTextSurface;
        protected GameObjectCollection[] gameObjects;

        public LayeredTextSurface LayeredTextSurface { get { return layeredTextSurface; } }

        public GameConsole(int width, int height, int layers) :base(width, height)
        {
            textSurface = layeredTextSurface = new Consoles.LayeredTextSurface(width, height, layers);
            _renderer = new LayeredTextRenderer();
            gameObjects = new GameObjectCollection[layers];
            for (int i = 0; i < layers; i++)
            {
                gameObjects[i] = new GameObjectCollection();
            }
            AssociateLayersWithObjects();
        }

        public void AssociateLayersWithObjects()
        {
            for (int i = 0; i < layeredTextSurface.LayerCount; i++)
            {
                foreach (var gameObject in gameObjects[i].Values)
                {
                    gameObject.Layer = i;
                }
                
            }
        }

        public GameObjectCollection GetObjectCollection(int layer)
        {
            if (layer < 0 || layer >= layeredTextSurface.LayerCount)
                throw new System.ArgumentOutOfRangeException("layer");

            return gameObjects[layer];
        }

    }
}
