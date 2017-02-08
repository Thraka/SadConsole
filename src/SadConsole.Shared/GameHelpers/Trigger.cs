using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Consoles;
using Microsoft.Xna.Framework.Input;

namespace SadConsole.GameHelpers
{
    public class Trigger : GameObject, ITrigger
    {
        public string[] TargetNames { get; set; }

        public GameObject[] ResolvedTargets { get; private set; }

        public string Type { get; set; }

        public Func<Trigger, GameObjectTextSurface, bool> Condition;

        public Trigger(GameObject source)
        {
            Id = "";
            StringBuilder targets = new StringBuilder();

            foreach (var setting in source.Settings)
            {
                string name = setting.Name.ToLower().Trim();
                if (name == "target")
                    targets.Append(String.Format("{0};", setting.Value));
                else if (name == "type")
                    Type = setting.Value;
            }

            TargetNames = targets.ToString().Trim(';').Trim().Split(';');

            source.CopyTo(this);
        }

        public virtual void Triggered(GameObject source, GameObjectTextSurface )
        {
            // Check for condition pass
            if (Condition(this, console))
            {
                GameObjectCollection parent = null;
                Parent.TryGetTarget(out parent);

                ResolvedTargets = GameObjectParser.ResolveTargets(this, TargetIds, parent, console, DeepProcess);

                //Result(this, parent, console);

                for (int i = 0; i < ResolvedTargets.Length; i++)
                {
                    if (ResolvedTargets[i] is ITarget)
                        ((ITarget)ResolvedTargets[i]).Triggered(this, console);
                }
            }
        }

        public override void Process(GameConsole console)
        {
            // Check for condition pass
            if (Condition(this, console))
            {
                GameObjectCollection parent = null;
                Parent.TryGetTarget(out parent);

                ResolvedTargets = GameObjectParser.ResolveTargets(this, TargetIds, parent, console, DeepProcess);

                //Result(this, parent, console);

                for (int i = 0; i < ResolvedTargets.Length; i++)
                {
                    if (ResolvedTargets[i] is ITarget)
                        ((ITarget)ResolvedTargets[i]).Triggered(this, console);
                }
            }
        }
    }
}
