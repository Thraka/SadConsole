using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    class ToolBrush : GameHelpers.GameObject
    {
        public ToolBrush(int width, int height) : base(width, height) { }

        public override void Draw(TimeSpan timeElapsed)
        {
            renderer.Render(animation);
        }

        public void EditorDraw(SpriteBatch batch)
        {
            if (IsVisible)
            {
                Point screenOffset = Global.RenderRect.Location - (Global.RenderRect.Location.PixelLocationToConsole(animation.Font.Size.X, animation.Font.Size.Y).ConsoleLocationToPixel(animation.Font.Size.X, animation.Font.Size.Y));

                batch.Draw(animation.LastRenderResult, new Rectangle(animation.Font.GetWorldPosition(calculatedPosition - animation.Center) + screenOffset, new Point(animation.LastRenderResult.Width, animation.LastRenderResult.Height)), Color.White);
            }
        }

        public void Refresh()
        {

        }
    }
}
