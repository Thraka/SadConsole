using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole
{
    public class Screen: IScreen
    {
        public Point Position { get; set; }

        public List<IScreen> Children { get; set; } = new List<IScreen>();

        public IScreen Parent { get; set; }

        public bool IsVisible { get; set; }

        public virtual void Draw(TimeSpan timeElapsed)
        {
            var copyList = new List<IScreen>(Children);

            foreach (var child in copyList)
                child.Draw(timeElapsed);
        }

        public virtual void Update(TimeSpan timeElapsed)
        {
            var copyList = new List<IScreen>(Children);

            foreach (var child in copyList)
                child.Update(timeElapsed);
        }
    }
}
