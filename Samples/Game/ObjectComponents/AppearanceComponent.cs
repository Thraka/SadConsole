using Game.Tiles;
using SadRogue.Primitives;

namespace Game.ObjectComponents
{
    class AppearanceComponent : ITileComponent, IGameObjectComponent
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public int Glyph { get; set; }

        public AppearanceComponent(Color foreground, Color background, int glyph) =>
            (Foreground, Background, Glyph) = (foreground, background, glyph);

        public void Added(BasicTile obj)
        {
            obj.Foreground = Foreground;
            obj.Background = Background;
            obj.Glyph = Glyph;
        }

        public void Removed(BasicTile obj) { }

        public void Added(GameObject obj)
        {
            obj.Animation.CurrentFrame[0].Foreground = Foreground;
            obj.Animation.CurrentFrame[0].Background = Background;
            obj.Animation.CurrentFrame[0].Glyph = Glyph;
        }

        public void Removed(GameObject obj) { }
    }
}
