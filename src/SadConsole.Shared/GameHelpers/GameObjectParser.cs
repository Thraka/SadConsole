using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public static class GameObjectParser
    {

        static GameObjectParser()
        {
            Conversions.Add("trigger", typeof(Trigger));
            Conversions.Add("processor", typeof(Processor));
            Conversions.Add("action-script", typeof(Action));
        }

        public static Dictionary<string, Type> Conversions = new Dictionary<string, Type>();

        public static GameObject Parse(GameObject source)
        {
            string name = source.Name.ToLower();

            if (Conversions.ContainsKey(name))
            {
                var type = Conversions[name];

                return System.Activator.CreateInstance(type, source) as GameObject;
            }

            return source;
        }

        public static GameObject[] ResolveTargets(GameObject source, IEnumerable<string> TargetIds, GameObjectCollection parent, GameConsole console, bool DeepProcess)
        {
            // If it passed, find all targets and activate them
            List<GameObject> foundTargets = new List<GameObject>(5);


            foreach (var item in parent)
            {
                if (item.Value != source && TargetIds.Contains(item.Value.GetSetting("id")))
                    foundTargets.Add(item.Value);
            }

            // Process all known collections
            if (DeepProcess)
            {
                for (int i = 0; i < console.LayeredTextSurface.LayerCount; i++)
                {
                    var objects = console.GetObjectCollection(i);
                    if (objects != parent)
                    {
                        foreach (var item in objects)
                        {
                            if (item.Value != source && TargetIds.Contains(item.Value.GetSetting("id")))
                                foundTargets.Add(item.Value);
                        }
                    }
                }
            }

            return foundTargets.ToArray();
        }

    }
}
