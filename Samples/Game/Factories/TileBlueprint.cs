using ZZTGame.Tiles;

namespace ZZTGame.Factories;

class TileBlueprint : SadConsole.Factory.IBlueprint<TileBlueprint.Config, Tiles.BasicTile>
{
    private readonly Type[] _components;
    private readonly Config _config;


    public string Id { get; }

    public TileBlueprint(string id, Config defaultConfig = null, params Type[] components) =>
        (_components, _config, Id) = (components, defaultConfig ?? Config.Empty, id);

    public BasicTile Create(Config config)
    {
        if (config == null)
            config = Config.Empty;

        BasicTile tile = new BasicTile(Point.None);

        tile.AddComponent(new ObjectComponents.AppearanceComponent(
                                config.Foreground.HasValue ? config.Foreground.Value : _config?.Foreground.HasValue ?? false ? _config.Foreground.Value : Color.White,
                                config.Background.HasValue ? config.Background.Value : _config?.Background.HasValue ?? false ? _config.Background.Value : Color.Black,
                                config.Glyph.HasValue ? config.Glyph.Value : _config?.Glyph.HasValue ?? false ? _config.Glyph.Value : 0));

        foreach (Type item in _components)
            tile.AddComponent(item.Assembly.CreateInstance(item.FullName));

        return tile;
    }

    public class Config: SadConsole.Factory.BlueprintConfig
    {
        public readonly Color? Foreground;
        public readonly Color? Background;
        public readonly int? Glyph;

        public Config(Color? foreground = null, Color? background = null, int? glyph = null) =>
            (Foreground, Background, Glyph) = (foreground, background, glyph);

        public static new Config Empty { get; } = new Config();
    }

}
