using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using Console = SadConsole.Console;
using SadConsole.Maps;

namespace BasicTutorial
{
    class MapConsole: ScrollingConsole
    {
        private Map _map;

        public Map Map
        {
            get => _map;
            set { _map = value; _map.SyncToSurface(this); }
        }

        public MapConsole(int screenWidth, int screenHeight, Map map) : base(screenWidth, screenHeight)
        {
            _map = map;
            map.SyncToSurface(this);
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            base.Draw(timeElapsed);

            foreach (var gameObject in Map.GameObjects)
                gameObject.Item.Draw(timeElapsed);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);

            foreach (var gameObject in Map.GameObjects)
                gameObject.Item.Update(timeElapsed);

            Map.GameObjects.SyncView();
        }
    }
}
