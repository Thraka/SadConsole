using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public interface ITarget
    {
        string Id { get; }

        void Triggered(GameObject source, GameConsole console);
    }

    public interface ICanTarget
    {
        string[] TargetIds { get; set; }

        GameObject[] ResolvedTargets { get; }
        bool DeepProcess { get; set; }
    }

    public interface ITrigger : ICanTarget, ITarget
    {
        void Process(GameConsole console);
    }
}
