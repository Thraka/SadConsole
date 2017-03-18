using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public class Processor :GameObject, ITarget, ICanTarget
    {
        public string Id { get; private set; }
        public string[] TargetIds { get; set; }

        public GameObject[] ResolvedTargets { get; private set; }

        public bool DeepProcess { get; set; }

        public Processor(GameObject source)
        {
            Id = "";
            StringBuilder targets = new StringBuilder();

            foreach (var setting in source.Settings)
            {
                string name = setting.Name.ToLower().Trim();
                if (name == "id")
                    Id = setting.Value;
                else if (name == "target")
                    targets.Append(string.Format("{0};", setting.Value));
                else if (name == "deep")
                    DeepProcess = string.IsNullOrWhiteSpace(setting.Value) ? false : setting.Value.ToBool();
            }

            TargetIds = targets.ToString().Trim(';').Trim().Split(';');

            source.CopyTo(this);
        }
        
        public void Triggered(GameObject source, GameConsole console)
        {
            GameObjectCollection parent = null;
            Parent.TryGetTarget(out parent);

            ResolvedTargets = GameObjectParser.ResolveTargets(this, TargetIds, parent, console, DeepProcess);

            for (int i = 0; i < ResolvedTargets.Length; i++)
            {
                if (ResolvedTargets[i] is ITarget)
                    ((ITarget)ResolvedTargets[i]).Triggered(this, console);
            }
        }
    }
}
