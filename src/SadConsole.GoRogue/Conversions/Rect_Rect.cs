using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using Microsoft.Xna.Framework;


namespace SadConsole
{
    public static class GoRogueConversions_Rect_Rect
    {
        public static Microsoft.Xna.Framework.Rectangle ToMonoGameRectangle(this GoRogue.Rectangle rect) =>
            new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static GoRogue.Rectangle ToGoRogueRectangle(this Microsoft.Xna.Framework.Rectangle rect) =>
            new GoRogue.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
