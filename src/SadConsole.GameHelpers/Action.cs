using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public class Action : GameObject, ITarget
    {
        public string Id { get; set; }

        public string Script { get; set; }

        public Action<GameObject, GameConsole> Result;

        public Action(GameObject source)
        {
            Id = "";
            Script = "";
            StringBuilder targets = new StringBuilder();

            foreach (var setting in source.Settings)
            {
                string name = setting.Name.ToLower().Trim();
                if (name == "id")
                    Id = setting.Value;
                else if (name == "script")
                    Script = setting.Value;
            }

            source.CopyTo(this);
        }

        public void Triggered(GameObject source, GameConsole console)
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
