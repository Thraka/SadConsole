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
                batch.Draw(animation.LastRenderResult, new Rectangle(animation.Font.GetWorldPosition(calculatedPosition - animation.Center), new Point(animation.LastRenderResult.Width, animation.LastRenderResult.Height)), Color.White);
        }
    }
}
