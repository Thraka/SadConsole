using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ObjectComponents
{
    class Pushable : IFlag, IGameObjectComponent
    {
        public enum Modes
        {
            All,
            Horizontal,
            Vertical,
        }

        public Modes Mode { get; set; } = Modes.All;

        public void Added(GameObject obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }
    }
}
