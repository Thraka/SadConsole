using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Consoles;

namespace SadConsole.GameHelpers
{
    public class Trigger : GameObject
    {
        public string Id { get; private set; }
        public IEnumerable<string> TargetIds { get; private set; }

        public IEnumerable<GameObject> ResolvedTargets { get; private set; }

        public Func<Trigger, GameObjectCollection, Consoles.Console, IEnumerable<GameObjectCollection>, bool> Condition;
        public Action<Trigger, GameObjectCollection, Consoles.Console, IEnumerable<GameObjectCollection>> Result;

        private Trigger()
        {
        }

        public static Trigger FromGameObject(GameObject source)
        {
            if (source.Name.ToLower().Trim().StartsWith("trigger"))
            {
                string id = "";
                StringBuilder targets = new StringBuilder();

                foreach (var setting in source.Settings)
                {
                    string name = setting.Name.ToLower().Trim();
                    if (name == "id")
                        id = setting.Value;
                    else if (name == "target")
                        targets.Append(String.Format("{0};", setting.Value));
                }

                Trigger trigger = (Trigger)source.Clone();
                trigger.Id = id;
                trigger.TargetIds = targets.ToString().Trim(';').Trim().Split(';');

                return trigger;
            }
            else
                throw new Exception("Invalid source object for target object");
        }

        public override void Process(GameObjectCollection parent, Consoles.Console console, IEnumerable<GameObjectCollection> otherCollections = null)
        {
            
        }
    }
}
