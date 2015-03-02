using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public static class GameObjectParser
    {
        public static Dictionary<string, Type> Conversions = new Dictionary<string, Type>();

        public static GameObject Parse(GameObject source)
        {
            if (Conversions.ContainsKey(source.Name))
            {
                var type = Conversions[source.Name];

                return System.Activator.CreateInstance(type, source) as GameObject;
            }

            return source;
        }
    }
}
