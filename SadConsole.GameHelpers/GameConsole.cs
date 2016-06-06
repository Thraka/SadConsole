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
        protected LayeredTextSurface _layeredTextSurface;
        protected GameObjectCollection[] _gameObjects;

        public LayeredTextSurface LayeredTextSurface { get { return _layeredTextSurface; } }

        public GameConsole(int width, int height, int layers) :base(width, height)
        {
            textSurface = _layeredTextSurface = new Consoles.LayeredTextSurface(width, height, layers);
            _gameObjects = new GameObjectCollection[_layeredTextSurface.Layers];
            AssociateLayersWithObjects();
        }

        public void AssociateLayersWithObjects()
        {
            for (int i = 0; i < _layeredTextSurface.Layers; i++)
            {
                foreach (var gameObject in _gameObjects[i].Values)
                {
                    gameObject.Layer = i;
                }
                
            }
        }

        public GameObjectCollection GetObjectCollection(int layer)
        {
            if (layer < 0 || layer >= _layeredTextSurface.Layers)
                throw new System.ArgumentOutOfRangeException("layer");

            return _gameObjects[layer];
        }

    }
}
