using Microsoft.Xna.Framework;

namespace SadRogue.Entities
{
    public class EntityBase: SadConsole.GameHelpers.GameObject
    {
        public EntityBase(Color foreground, Color background, int glyph) : base(1, 1)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        public void MoveBy(Point change)
        {
            Position += change;
        }

        public override void OnCalculateRenderPosition()
        {
            base.OnCalculateRenderPosition();
            
            //IsVisible = 
        }
    }
}
