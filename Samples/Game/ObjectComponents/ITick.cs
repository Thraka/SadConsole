using System;
using System.Collections.Generic;
using System.Text;

namespace Game.ObjectComponents
{
    interface ITick
    {
        int Tick { get; set;  }

        void Action(Screens.Board board, GameObject obj);
    }
}
