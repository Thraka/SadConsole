﻿using Game.Tiles;
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
            obj.AppearanceSingle.Appearance.Foreground = Foreground;
            obj.AppearanceSingle.Appearance.Background = Background;
            obj.AppearanceSingle.Appearance.Glyph = Glyph;
        }

        public void Removed(GameObject obj) { }
    }
}
