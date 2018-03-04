using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor
{
    public interface IEditor
    {
        string File { get; set; }

        Surfaces.BasicSurface Surface { get; }

        Font Font { get; set; }

        void Update();

        void Draw(SpriteBatch batch);

        void Save();
    }
}
