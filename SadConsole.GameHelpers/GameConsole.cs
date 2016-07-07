using SadConsole.Consoles;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// A text surface with metadata to help with common game functions.
    /// </summary>
    public class GameObjectTextSurface : TextSurface
    {
        public List<Trigger> Triggers;

        public List<Processor> Processors;

        public List<Action> Actions;

        public List<GameObject> Objects;

        public void AssociateObjects()
        {
            foreach (var item in base.Keys)
            {
                var newObject = GameObjectParser.Parse(this[item]);

                transformedObjects.Add(new Tuple<Point, GameObject>(item, TransformObject(newObject)));
            }

            foreach (var item in transformedObjects)
            {
                this[item.Item1] = item.Item2;

                if (item.Item2 is Trigger)
                    Triggers.Add((Trigger)item.Item2);

                else if (item.Item2 is Processor)
                    Processors.Add((Processor)item.Item2);

                else if (item.Item2 is Action)
                    Actions.Add((Action)item.Item2);
            }
        }
    }

}
