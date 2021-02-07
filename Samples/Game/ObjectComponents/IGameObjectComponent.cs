using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ObjectComponents
{
    interface IGameObjectComponent
    {
        void Added(GameObject obj);
        void Removed(GameObject obj);
    }
}
