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
    }

    public interface ICanTarget
    {
        IEnumerable<string> TargetIds { get; }

        IEnumerable<GameObject> ResolvedTargets { get; }
    }

    public interface ITrigger : ICanTarget, ITarget
    {
        bool DeepProcess { get; }

        void Process(GameObjectCollection parent, Consoles.Console console, IEnumerable<GameObjectCollection> otherCollections = null);
    }

    public interface IProcessor : ICanTarget, ITarget
    {
        void Triggered(ITrigger trigger, GameObjectCollection parent, Consoles.Console console, IEnumerable<GameObjectCollection> otherCollections = null);
    }
}
