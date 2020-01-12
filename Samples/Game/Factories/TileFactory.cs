using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Factories
{
    class TileFactory: SadConsole.Factory.Factory<TileBlueprint.Config, Tiles.BasicTile>
    {
        public static TileFactory Instance { get; } = new TileFactory();

        private TileFactory()
        {
            // Seed the factory with all the default tiles
            Add(new TileBlueprint("empty"));
            Add(new TileBlueprint("dead", new TileBlueprint.Config(SadRogue.Primitives.ColorAnsi.WhiteBright, SadRogue.Primitives.ColorAnsi.Red, 'x')));
            Add(new TileBlueprint("wall-solid", new TileBlueprint.Config(null, null, 0xDB), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
            Add(new TileBlueprint("wall-blink", new TileBlueprint.Config(null, null, 0xCE), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
            Add(new TileBlueprint("wall-normal", new TileBlueprint.Config(null, null, 0xB2), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
            Add(new TileBlueprint("wall-fake", new TileBlueprint.Config(null, null, 0xDB)));
            Add(new TileBlueprint("wall-breakable", new TileBlueprint.Config(null, null, 0xB1), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
            Add(new TileBlueprint("wall-invisible", new TileBlueprint.Config(null, null, 0xB0), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
            Add(new TileBlueprint("line", new TileBlueprint.Config(null, null, 0xCE), typeof(ObjectComponents.BlockingMove), typeof(ObjectComponents.Touchable)));
        }
    }
}
