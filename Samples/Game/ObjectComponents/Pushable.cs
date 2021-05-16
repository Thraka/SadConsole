using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ObjectComponents
{
    class Pushable : IFlag, IGameObjectComponent
    {
        public enum Directions
        {
            All,
            Horizontal,
            Vertical,
        }

        public enum Modes
        {
            All,
            PlayerOnly,
            CreatureOnly
        }

        public Directions Direction { get; set; } = Directions.All;

        public Modes Mode { get; set; } = Modes.All;

        public void Added(GameObject obj)
        {
        }

        public void Removed(GameObject obj)
        {
        }
    }
}
