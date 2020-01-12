using System;
using System.Collections.Generic;
using System.Text;
using Game.Tiles;
using SadConsole;
using SadRogue.Primitives;

namespace Game.Factories
{
    class GameObjectBlueprint : SadConsole.Factory.IBlueprint<GameObjectBlueprint.Config, GameObject>
    {
        private readonly Type[] _components;
        private readonly Config _config;

        public string Id { get; }

        public GameObjectBlueprint(string id, Config defaultConfig = null, params Type[] components) =>
            (_components, _config, Id) = (components, defaultConfig ?? Config.Empty, id);

        public GameObject Create(Config config)
        {
            if (config == null)
                config = Config.Empty;

            GameObject obj = new GameObject();

            obj.AddComponent(new ObjectComponents.AppearanceComponent(
                                    config.Foreground.HasValue ? config.Foreground.Value : _config?.Foreground.HasValue ?? false ? _config.Foreground.Value : Color.White,
                                    config.Background.HasValue ? config.Background.Value : _config?.Background.HasValue ?? false ? _config.Background.Value : Color.Black,
                                    config.Glyph.HasValue ? config.Glyph.Value : _config?.Glyph.HasValue ?? false ? _config.Glyph.Value : 0));

            foreach (Type item in _components)
                obj.AddComponent(item.Assembly.CreateInstance(item.FullName));

            if (config.ConfigureLogic == null)
                _config?.ConfigureLogic?.Invoke(obj);
            else
                config.ConfigureLogic(obj);

            return obj;
        }

        public class Config : SadConsole.Factory.BlueprintConfig
        {
            public readonly Color? Foreground;
            public readonly Color? Background;
            public readonly int? Glyph;

            public Action<GameObject> ConfigureLogic;

            public Config(Color? foreground = null, Color? background = null, int? glyph = null, Action<GameObject> configure = null) =>
                (Foreground, Background, Glyph, ConfigureLogic) = (foreground, background, glyph, configure);


            public static new Config Empty { get; } = new Config();
        }

    }
}
